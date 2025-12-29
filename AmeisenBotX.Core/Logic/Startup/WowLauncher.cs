using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Memory.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;

namespace AmeisenBotX.Core.Logic.Startup
{
    /// <summary>
    /// Handles WoW process startup including:
    /// - Process launching with hidden/positioned window
    /// - Window hook for position control
    /// - Memory attachment
    /// - Bridge DLL injection with IPC verification
    /// </summary>
    public class WowLauncher(AmeisenBotConfig config, AmeisenBotInterfaces bot)
    {

        // Hook delegate (keep alive to prevent GC)
        private Win32Imports.WinEventProc _winEventProc;

        // Signal for hook-driven window detection
        private volatile bool _windowCreated;

        /// <summary>
        /// Event fired when WoW has been successfully started and attached.
        /// </summary>
        public event Action OnWoWStarted;

        /// <summary>
        /// Start WoW process, attach memory, inject bridge DLL.
        /// </summary>
        public BtStatus StartWow()
        {
            if (!File.Exists(config.PathToWowExe))
            {
                AmeisenLogger.I.Log("WowLauncher", $"WoW executable not found: {config.PathToWowExe}", LogLevel.Error);
                return BtStatus.Failed;
            }

            AmeisenLogger.I.Log("WowLauncher", "Starting WoW Process");

            // Determine startup window position
            Rect? startRect = DetermineStartPosition();

            // Start process
            Process p = bot.Memory.StartProcessNoActivate(
                $"\"{config.PathToWowExe}\" -windowed -d3d9",
                out nint processHandle,
                out nint mainThreadHandle,
                startHidden: false,
                windowRect: startRect);

            if (p == null)
            {
                AmeisenLogger.I.Log("WowLauncher", "Failed to start WoW process", LogLevel.Error);
                return BtStatus.Failed;
            }

            // Setup window hook and wait for window creation
            if (!WaitForWindow(p, startRect))
            {
                AmeisenLogger.I.Log("WowLauncher", "Timeout waiting for WoW window", LogLevel.Warning);
            }

            // Set process priority
            TrySetProcessPriority(p, ProcessPriorityClass.High);

            // Memory attachment needs no artificial delay

            AmeisenLogger.I.Log("WowLauncher", $"Attaching XMemory to {p.ProcessName} ({p.Id})");

            // Initialize memory
            if (!bot.Memory.Init(p, processHandle, mainThreadHandle))
            {
                AmeisenLogger.I.Log("WowLauncher", "Failed to attach XMemory", LogLevel.Error);
                p.Kill();
                return BtStatus.Failed;
            }

            bot.Memory.Offsets.Init(bot.Memory.Process.MainModule.BaseAddress);

            // Inject bridge DLL
            if (!InjectBridgeDll(p))
            {
                AmeisenLogger.I.Log("WowLauncher", "Bridge injection failed or verification timed out", LogLevel.Warning);
            }

            // Ensure window is visible (only if not using AutoPosition - it handles showing)
            if (!config.AutoPositionWow)
                EnsureWindowVisible();

            OnWoWStarted?.Invoke();
            return BtStatus.Success;
        }

        private Rect? DetermineStartPosition()
        {
            if (config.AutoPositionWow)
            {
                // Force off-screen start to prevent center flash
                return new Rect { Left = -32000, Top = -32000, Right = -31200, Bottom = -31400 };
            }

            return config.SaveWowWindowPosition && config.WowWindowRect.Right > 0 && config.WowWindowRect.Bottom > 0
                ? config.WowWindowRect
                : null;
        }

        private bool WaitForWindow(Process p, Rect? startRect)
        {
            nint hook = nint.Zero;

            try
            {
                // Install window creation hook
                _winEventProc = new Win32Imports.WinEventProc(WindowHookProc);
                hook = Win32Imports.SetWinEventHook(
                    Win32Imports.EVENT_OBJECT_CREATE,
                    Win32Imports.EVENT_OBJECT_CREATE,
                    nint.Zero,
                    _winEventProc,
                    (uint)p.Id,
                    0,
                    Win32Imports.WINEVENT_OUTOFCONTEXT | Win32Imports.WINEVENT_SKIPOWNPROCESS);

                p.WaitForInputIdle();

                // Increase timeout to handle slow startups (HDD/Load times)
                int maxWaitMs = (int)TimingConfig.WowWindowWaitTimeout.TotalMilliseconds;
                int waited = 0;

                _windowCreated = false;

                while (waited < maxWaitMs)
                {
                    p.Refresh();
                    if (p.MainWindowHandle != nint.Zero)
                    {
                        // Window exists - apply positioning if not using AutoPosition
                        // (AutoPosition handles hiding/showing in SetupAutoPosition)
                        if (!config.AutoPositionWow && startRect.HasValue)
                        {
                            bot.Memory.SetWindowPosition(p.MainWindowHandle, startRect.Value);
                        }
                        return true;
                    }
                    
                    // If hook detected window, check again immediately without sleep
                    if (_windowCreated)
                    {
                        _windowCreated = false; // Reset and continue checking
                        continue;
                    }
                    
                    Thread.Sleep(50);
                    waited += 50;
                }

                return false;
            }
            finally
            {
                if (hook != nint.Zero)
                {
                    Win32Imports.UnhookWinEvent(hook);
                }
            }
        }

        private void WindowHookProc(nint hWinEventHook, uint eventType, nint hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Only signal window creation - actual positioning is handled in WaitForWindow/SetupAutoPosition
            if (eventType == Win32Imports.EVENT_OBJECT_CREATE && idObject == 0 && idChild == 0)
            {
                _windowCreated = true;
            }
        }

        private bool InjectBridgeDll(Process p)
        {
            string bridgePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AmeisenBotX.Bridge.Native.dll");

            // Generate unique IPC name
            string user = !string.IsNullOrEmpty(config.Username) ? config.Username : "Bot";
            string uniqueId = $"{user}_{Environment.ProcessId}";
            string ipcName = $"Local\\WM_IPC_{uniqueId}";
            // Pass IPC name first, then bot name/ID
            string bridgeArg = $"{ipcName}|{user}";

            try
            {
                // Create a separate signal MMF for verification
                // BridgeEnty writes to: ipcName + "_SIGNAL"
                string signalName = $"{ipcName}_SIGNAL";
                using MemoryMappedFile signalMmf = MemoryMappedFile.CreateOrOpen(signalName, 4, MemoryMappedFileAccess.ReadWrite);
                using MemoryMappedViewAccessor accessor = signalMmf.CreateViewAccessor(0, 4);
                accessor.Write(0, 0); // Clear state

                if (!bot.Memory.InjectDll(bridgePath, "Init", bridgeArg))
                {
                    AmeisenLogger.I.Log("WowLauncher", "Failed to inject AmeisenBotX.Bridge", LogLevel.Warning);
                    return false;
                }

                // Poll for IPC verification with exponential backoff
                for (int i = 0; i < 20; i++)
                {
                    // Check for signal '1' from BridgeEntry
                    if (accessor.ReadInt32(0) == 1)
                    {
                        AmeisenLogger.I.Log("WowLauncher", $"Bridge injected and verified via IPC ({ipcName})");
                        return true;
                    }
                    Thread.Sleep(10 + (i * 5)); // Backoff: 10ms, 15ms, 20ms...
                }

                AmeisenLogger.I.Log("WowLauncher", "Bridge injected but IPC verification timed out", LogLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("WowLauncher", $"IPC Error: {ex.Message}", LogLevel.Warning);
                return false;
            }
        }

        private void EnsureWindowVisible()
        {
            if (bot.Memory.Process.MainWindowHandle != nint.Zero)
            {
                bot.Memory.ShowWindow(bot.Memory.Process.MainWindowHandle);
            }
        }

        private void TrySetProcessPriority(Process p, ProcessPriorityClass priority)
        {
            try
            {
                p.PriorityClass = priority;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("WowLauncher", $"Failed to set priority: {ex.Message}", LogLevel.Warning);
            }
        }

        /// <summary>
        /// Check and accept TOS/EULA by modifying config.wtf.
        /// Also disables the intro movie.
        /// </summary>
        public BtStatus CheckTosAndEula()
        {
            try
            {
                string configWtfPath = Path.Combine(Directory.GetParent(config.PathToWowExe).FullName, "wtf", "config.wtf");

                if (File.Exists(configWtfPath))
                {
                    bool editedFile = false;
                    string content = File.ReadAllText(configWtfPath);

                    if (!content.Contains("SET READEULA \"0\"", StringComparison.OrdinalIgnoreCase))
                    {
                        editedFile = true;

                        if (content.Contains("SET READEULA", StringComparison.OrdinalIgnoreCase))
                        {
                            content = content.Replace("SET READEULA \"0\"", "SET READEULA \"1\"", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            content += "\nSET READEULA \"1\"";
                        }
                    }

                    if (!content.Contains("SET READTOS \"0\"", StringComparison.OrdinalIgnoreCase))
                    {
                        editedFile = true;

                        if (content.Contains("SET READTOS", StringComparison.OrdinalIgnoreCase))
                        {
                            content = content.Replace("SET READTOS \"0\"", "SET READTOS \"1\"", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            content += "\nSET READTOS \"1\"";
                        }
                    }

                    if (!content.Contains("SET MOVIE \"0\"", StringComparison.OrdinalIgnoreCase))
                    {
                        editedFile = true;

                        if (content.Contains("SET MOVIE", StringComparison.OrdinalIgnoreCase))
                        {
                            content = content.Replace("SET MOVIE \"0\"", "SET MOVIE \"1\"", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            content += "\nSET MOVIE \"1\"";
                        }
                    }

                    if (editedFile)
                    {
                        File.SetAttributes(configWtfPath, FileAttributes.Normal);
                        File.WriteAllText(configWtfPath, content);
                        File.SetAttributes(configWtfPath, FileAttributes.ReadOnly);
                    }
                }

                return BtStatus.Success;
            }
            catch
            {
                AmeisenLogger.I.Log("WowLauncher", "Cannot write to config.wtf");
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// Change realmlist in config.wtf if AutoChangeRealmlist is enabled.
        /// </summary>
        public BtStatus ChangeRealmlist()
        {
            if (!config.AutoChangeRealmlist)
            {
                return BtStatus.Success;
            }

            try
            {
                AmeisenLogger.I.Log("WowLauncher", "Changing Realmlist");
                string configWtfPath = Path.Combine(Directory.GetParent(config.PathToWowExe).FullName, "wtf", "config.wtf");

                if (File.Exists(configWtfPath))
                {
                    bool editedFile = false;
                    List<string> content = [.. File.ReadAllLines(configWtfPath)];

                    if (!content.Any(e => e.Contains($"SET REALMLIST {config.Realmlist}", StringComparison.OrdinalIgnoreCase)))
                    {
                        bool found = false;

                        for (int i = 0; i < content.Count; ++i)
                        {
                            if (content[i].Contains("SET REALMLIST", StringComparison.OrdinalIgnoreCase))
                            {
                                editedFile = true;
                                content[i] = $"SET REALMLIST {config.Realmlist}";
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            editedFile = true;
                            content.Add($"SET REALMLIST {config.Realmlist}");
                        }
                    }

                    if (editedFile)
                    {
                        File.SetAttributes(configWtfPath, FileAttributes.Normal);
                        File.WriteAllLines(configWtfPath, content);
                        File.SetAttributes(configWtfPath, FileAttributes.ReadOnly);
                    }
                }

                return BtStatus.Success;
            }
            catch
            {
                AmeisenLogger.I.Log("WowLauncher", "Cannot write realmlist to config.wtf");
            }

            return BtStatus.Failed;
        }
    }
}
