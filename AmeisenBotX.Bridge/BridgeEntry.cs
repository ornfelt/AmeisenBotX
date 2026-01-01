using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Entry point for the injected bridge DLL.
/// Initialized by the native bootstrap loader.
/// </summary>
public static partial class BridgeEntry
{
    private static BridgeServer? _server;

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool MessageBeep(uint uType);

    /// <summary>
    /// Entry point called by the injector/bootstrap.
    /// </summary>
    /// <param name="parameter">Pointer to the initialization string (IPC name pipeline).</param>
    [UnmanagedCallersOnly(EntryPoint = "Init")]
    public static int Init(nint parameter)
    {
        try
        {
            // Debug beep
            MessageBeep(0);

            // 1. Read the IPC name from the pointer
            string? initData = Marshal.PtrToStringAnsi(parameter);
            if (string.IsNullOrEmpty(initData))
            {
                return -1;
            }

            // Format: "IpcName|BotName"
            string[] parts = initData.Split('|');
            string ipcName = parts[0];
            string botName = parts.Length > 1 ? parts[1] : "UnknownBot";

            // 2. Initialize Logger
            BridgeLogger.Init(botName);
            BridgeLogger.Log($"[BridgeEntry] Initializing... IPC: {ipcName}");

            // 3. Start Bridge Server
            _server = new BridgeServer(ipcName);
            _server.Start();

            // 4. Signal readiness via separate mapped section (created by injector)
            SignalReady(ipcName + "_SIGNAL");

            BridgeLogger.Log("[BridgeEntry] Initialization complete.");
            return 0;
        }
        catch (Exception ex)
        {
            try
            {
                BridgeLogger.Log($"[BridgeEntry] Fatal Error: {ex}");
            }
            catch { /* Ignore */ }
            return -1;
        }
    }

    private static void SignalReady(string signalName)
    {
        try
        {
            string ntPath = NtApi.ToNtPath(signalName);

            // Open the signal section created by the injector
            int status = NtApi.OpenSection(ntPath, out nint sectionHandle);
            if (!NtApi.NT_SUCCESS(status))
            {
                BridgeLogger.Log($"[BridgeEntry] Failed to open signal section: 0x{status:X}");
                return;
            }

            try
            {
                // Map it
                status = NtApi.MapViewOfSection(sectionHandle, 4, out nint baseAddress);
                if (!NtApi.NT_SUCCESS(status))
                {
                    BridgeLogger.Log($"[BridgeEntry] Failed to map signal section: 0x{status:X}");
                    return;
                }

                try
                {
                    // Write 1 to signal success
                    unsafe
                    {
                        Unsafe.WriteUnaligned((void*)baseAddress, 1);
                    }
                }
                finally
                {
                    NtApi.UnmapViewOfSection(baseAddress);
                }
            }
            finally
            {
                NtApi.Close(sectionHandle);
            }
        }
        catch (Exception ex)
        {
            BridgeLogger.Log($"[BridgeEntry] Error signaling ready: {ex.Message}");
        }
    }
}
