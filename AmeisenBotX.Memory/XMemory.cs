using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Memory.Structs;
using AmeisenBotX.Memory.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static AmeisenBotX.Memory.Win32.Win32Imports;

namespace AmeisenBotX.Memory
{
    public unsafe partial class XMemory : IMemoryApi
    {
        // FASM configuration, if you encounter fasm error, try to increase the values
        private const int FASM_MEMORY_SIZE = 8192;

        private const int FASM_PASSES = 100;

        // initial memory pool size
        private const int INITIAL_POOL_SIZE = 16384;

        // lock needs to be static as FASM isn't thread safe
        private static readonly Lock fasmLock = new();

        private readonly Lock allocLock = new();
        private ulong rpmCalls;
        private ulong wpmCalls;

        public XMemory()
        {
            if (!File.Exists("FASM.dll"))
            {
                throw new FileNotFoundException("The mandatory \"FASM.dll\" could not be found on your system, download it from the Flat Assembler forum!");
            }
        }

        ///<inheritdoc cref="IMemoryApi.MainThreadHandle"/>
        public nint MainThreadHandle { get; private set; }

        ///<inheritdoc cref="IMemoryApi.MemoryAllocations"/>
        public Dictionary<nint, uint> MemoryAllocations => AllocationPools.ToDictionary(e => e.Address, e => (uint)e.Size);

        ///<inheritdoc cref="IMemoryApi.Process"/>
        public Process Process { get; private set; }

        ///<inheritdoc cref="IMemoryApi.ProcessHandle"/>
        public nint ProcessHandle { get; private set; }

        ///<inheritdoc cref="IMemoryApi.RpmCallCount"/>
        public ulong RpmCallCount
        {
            get
            {
                ulong val = rpmCalls;
                rpmCalls = 0;
                return val;
            }
        }

        ///<inheritdoc cref="IMemoryApi.WpmCallCount"/>
        public ulong WpmCallCount
        {
            get
            {
                ulong val = wpmCalls;
                wpmCalls = 0;
                return val;
            }
        }

        private List<AllocationPool> AllocationPools { get; set; }

        private bool Initialized { get; set; }

        ///<inheritdoc cref="IMemoryApi.AllocateMemory"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllocateMemory(uint size, out nint address)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (size <= 0) { throw new ArgumentOutOfRangeException(nameof(size), "size must be > 0"); }
#endif
            using (allocLock.EnterScope())
            {
                for (int i = 0; i < AllocationPools.Count; ++i)
                {
                    if (AllocationPools[i].Reserve((int)size, out address))
                    {
                        AmeisenLogger.I.Log("XMemory", $"Reserved {size} bytes in Pool[{i}] at: 0x{address:X}");
                        return true;
                    }
                }

                // we need a new pool
                nint newPoolSize = Math.Max((int)size, INITIAL_POOL_SIZE);
                nint newPoolAddress = nint.Zero;
                int result = NtAllocateVirtualMemory(ProcessHandle, ref newPoolAddress, 0, ref newPoolSize, AllocationType.Commit, MemoryProtectionFlag.ExecuteReadWrite);

                if (result == 0 && newPoolAddress != nint.Zero)
                {
                    AllocationPool pool = new(newPoolAddress, newPoolSize.ToInt32());
                    AllocationPools.Add(pool);

                    AmeisenLogger.I.Log("XMemory", $"Created new Pool with {newPoolSize} bytes at: 0x{newPoolAddress:X}");

                    if (pool.Reserve((int)size, out address))
                    {
                        AmeisenLogger.I.Log("XMemory", $"Reserved {size} bytes in Pool[{AllocationPools.Count - 1}] at: 0x{address:X}");
                        return true;
                    }
                }

                address = nint.Zero;
                return false;
            }
        }

        ///<inheritdoc cref="IMemoryApi.Dispose"/>
        public void Dispose()
        {
            EjectDll("AmeisenBotX.Bridge.dll");
            GC.SuppressFinalize(this);
            _ = NtClose(MainThreadHandle);
            _ = NtClose(ProcessHandle);
            FreeAllMemory();
        }

        ///<inheritdoc cref="IMemoryApi.FocusWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FocusWindow(nint windowHandle, Rect rect, bool resizeWindow = true)
        {
            WindowFlag flags = WindowFlag.AsyncWindowPos | WindowFlag.NoActivate;

            if (!resizeWindow) { flags |= WindowFlag.NoSize; }

            if (rect.Left > 0 && rect.Right > 0 && rect.Top > 0 && rect.Bottom > 0)
            {
                SetWindowPos(windowHandle, nint.Zero, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, (int)flags);
            }
        }

        ///<inheritdoc cref="IMemoryApi.FreeAllMemory"/>
        public void FreeAllMemory()
        {
            using (allocLock.EnterScope())
            {
                if (AllocationPools != null)
                {
                    AmeisenLogger.I.Log("XMemory", $"Freeing all memory Pools...");

                    foreach (AllocationPool allocPool in AllocationPools)
                    {
                        nint addr = allocPool.Address;
                        nint size = 0;
                        NtFreeVirtualMemory(ProcessHandle, ref addr, ref size, AllocationType.Release);
                    }

                    AllocationPools.Clear();
                }
            }
        }

        ///<inheritdoc cref="IMemoryApi.FreeMemory"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FreeMemory(nint address)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
#endif
            using (allocLock.EnterScope())
            {
                for (int i = 0; i < AllocationPools.Count; ++i)
                {
                    if (AllocationPools[i].Free(address, out int size)
                        && ZeroMemory(address, size))
                    {
                        AmeisenLogger.I.Log("XMemory", $"Freed {size} bytes in Pool[{i}] at: 0x{address:X}");

                        // pool freeing is not needed at the moment, disabling it to reduce memory new allocations
                        if (false && AllocationPools[i].Allocations.Count == 0)
                        {
                            nint addr = AllocationPools[i].Address;
                            nint s = 0;

                            if (NtFreeVirtualMemory(ProcessHandle, ref addr, ref s, AllocationType.Release) == 0)
                            {
                                AmeisenLogger.I.Log("XMemory", $"Freed Pool[{i}] with {AllocationPools[i].Size} bytes at: 0x{addr:X}");
                                AllocationPools.RemoveAt(i);
                            }
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        ///<inheritdoc cref="IMemoryApi.GetClientSize"/>
        public Rect GetClientSize()
        {
            Rect rect = new();
            GetClientRect(Process.MainWindowHandle, ref rect);
            return rect;
        }

        ///<inheritdoc cref="IMemoryApi.GetForegroundWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint GetForegroundWindow()
        {
            return Win32Imports.GetForegroundWindow();
        }

        ///<inheritdoc cref="IMemoryApi.GetWindowPosition"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect GetWindowPosition()
        {
            Rect rect = new();

            if (Process != null)
            {
                GetWindowRect(Process.MainWindowHandle, ref rect);
            }

            return rect;
        }

        ///<inheritdoc cref="IMemoryApi.Init"/>
        public virtual bool Init(Process process, nint processHandle, nint mainThreadHandle)
        {
            Process = process ?? throw new ArgumentNullException(nameof(process), "process cannot be null");

#if DEBUG
            if (processHandle == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(processHandle), "processHandle must be > 0"); }
            if (mainThreadHandle == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(mainThreadHandle), "mainThreadHandle must be > 0"); }
#endif
            if (Process == null || Process.HasExited)
            {
                return false;
            }

            ProcessHandle = processHandle;

            if (ProcessHandle == nint.Zero)
            {
                return false;
            }

            MainThreadHandle = mainThreadHandle;

            if (MainThreadHandle == nint.Zero)
            {
                return false;
            }

            AllocationPools = [];

            // reserve initial pool
            if (INITIAL_POOL_SIZE > 0)
            {
                nint poolSize = INITIAL_POOL_SIZE;
                nint initialPoolAddress = nint.Zero;
                NtAllocateVirtualMemory(ProcessHandle, ref initialPoolAddress, 0, ref poolSize, AllocationType.Commit, MemoryProtectionFlag.ExecuteReadWrite);

                if (initialPoolAddress == nint.Zero)
                {
                    return false;
                }

                AllocationPools.Add(new(initialPoolAddress, INITIAL_POOL_SIZE));
            }

            Initialized = true;
            return true;
        }

        ///<inheritdoc cref="IMemoryApi.InjectAssembly"/>
        public bool InjectAssembly(IEnumerable<string> asm, nint address, bool patchMemProtection = false)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (!asm.Any()) { throw new ArgumentOutOfRangeException(nameof(asm), "asm must contain atleast one instruction"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
#endif
            using (fasmLock.EnterScope())
            {
                fixed (byte* pBytes = stackalloc byte[FASM_MEMORY_SIZE])
                {
                    if (FasmAssemble($"use32\norg 0x{address:X08}\n{string.Join('\n', asm)}", pBytes, FASM_MEMORY_SIZE, FASM_PASSES, nint.Zero) == 0)
                    {
                        FasmStateOk state = *(FasmStateOk*)pBytes;

                        if (patchMemProtection)
                        {
                            if (ProtectMemory(address, state.OutputLength, MemoryProtectionFlag.ExecuteReadWrite, out MemoryProtectionFlag oldMemoryProtection))
                            {
                                bool status = NtWriteVirtualMemory(ProcessHandle, address, (void*)state.OutputData, (int)state.OutputLength, out _) == 0;
                                ProtectMemory(address, state.OutputLength, oldMemoryProtection, out _);
                                return status;
                            }
                        }
                        else
                        {
                            return NtWriteVirtualMemory(ProcessHandle, address, (void*)state.OutputData, (int)state.OutputLength, out _) == 0;
                        }
                    }

                    // use this to read the error FasmStateError stateError = *(FasmStateError*)pBytes;
                    return false;
                }
            }
        }

        ///<inheritdoc cref="IMemoryApi.EjectDll"/>
        public bool EjectDll(string dllName)
        {
            try
            {
                if (string.IsNullOrEmpty(dllName))
                {
                    return false;
                }

                Process.Refresh();
                if (Process.HasExited)
                {
                    return true;
                }

                nint hModule = nint.Zero;

                foreach (ProcessModule pm in Process.Modules)
                {
                    if (pm.ModuleName.Equals(dllName, StringComparison.OrdinalIgnoreCase))
                    {
                        hModule = pm.BaseAddress;
                        break;
                    }
                }

                if (hModule == nint.Zero)
                {
                    return true; // Already loaded/not present
                }

                AmeisenLogger.I.Log("EjectDll", $"Ejecting {dllName} at 0x{hModule:X}...");

                nint kernel32 = GetModuleHandle("kernel32.dll");
                if (kernel32 == nint.Zero)
                {
                    return false;
                }

                nint freeLib = GetProcAddress(kernel32, "FreeLibrary");
                if (freeLib == nint.Zero)
                {
                    return false;
                }

                int status = NtCreateThreadEx(out nint hThread, ProcessAccessFlag.All, nint.Zero, ProcessHandle, freeLib, hModule, false, 0, 0, 0, nint.Zero);
                if (status != 0 || hThread == nint.Zero)
                {
                    AmeisenLogger.I.Log("EjectDll", $"NtCreateThreadEx (FreeLibrary) failed: 0x{status:X}", LogLevel.Error);
                    return false;
                }

                NtWaitForSingleObject(hThread, false, nint.Zero);
                NtClose(hThread);
                return true;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("EjectDll", $"Failed to eject (Process exiting?): {ex.Message}", LogLevel.Warning);
                return false;
            }
        }

        ///<inheritdoc cref="IMemoryApi.InjectDll"/>
        public bool InjectDll(string dllPath, string entryPoint, string argument = null)
        {
            if (!File.Exists(dllPath))
            {
                AmeisenLogger.I.Log("InjectDll", $"DLL not found: {dllPath}", LogLevel.Error);
                return false;
            }

            // 1. Resolve addresses (Kernel32 is usually at same address in all processes)
            nint kernel32 = GetModuleHandle("kernel32.dll");
            if (kernel32 == nint.Zero)
            {
                return false;
            }

            nint loadLibraryAddr = GetProcAddress(kernel32, "LoadLibraryA");
            nint getProcAddr = GetProcAddress(kernel32, "GetProcAddress");

            if (loadLibraryAddr == nint.Zero || getProcAddr == nint.Zero)
            {
                return false;
            }

            nint dataAddr = nint.Zero;
            nint codeAddr = nint.Zero;
            nint argAddr = nint.Zero;

            try
            {
                // 2. Allocate memory for Path and EntryPoint strings
                if (!AllocateMemory(1024, out dataAddr))
                {
                    return false;
                }

                if (!AllocateMemory(1024, out codeAddr))
                {
                    return false;
                }

                // 3. Write Strings
                byte[] pathBytes = Encoding.ASCII.GetBytes(dllPath + "\0");
                byte[] entryBytes = Encoding.ASCII.GetBytes(entryPoint + "\0");

                WriteBytes(dataAddr, pathBytes);
                WriteBytes(dataAddr + pathBytes.Length, entryBytes);

                nint pathPtr = dataAddr;
                nint entryPtr = dataAddr + pathBytes.Length;

                // Handle optional argument
                if (!string.IsNullOrEmpty(argument))
                {
                    byte[] argBytes = Encoding.ASCII.GetBytes(argument + "\0");
                    if (AllocateMemory((uint)argBytes.Length, out argAddr))
                    {
                        WriteBytes(argAddr, argBytes);
                    }
                }

                // 4. Assemble Loader Shellcode
                List<string> asm =
                [
                    $"push {pathPtr}",
                    $"call {loadLibraryAddr}",     // LoadLibraryA(path)
                    "test eax, eax",
                    "jz error",
                    "push eax",                   // Save hModule -> Stack: [hModule]
                    $"push {entryPtr}",            // ProcName -> Stack: [hModule, ProcName]
                    "push eax",                   // hModule -> Stack: [hModule, ProcName, hModule]
                    $"call {getProcAddr}",         // GetProcAddress(hModule, entryPoint) -> Stack: [hModule] (Arguments popped? GetProcAddress is StdCall)
                                                  // Wait, stdcall pops arguments. So [hModule] is still on stack? 
                                                  // No, we pushed hModule (saved), ProcName, hModule (arg).
                                                  // GetProcAddress pops 2 args (ProcName, hModule). 
                                                  // Stack is now [hModule]. 
                                                  // Correct? No. "push eax" (Save hModule) is NOT used by GetProcAddress.
                                                  // The last 2 pushes are args.
                    "test eax, eax",
                    "jz error_pop",               // If failed, we need to clean stack? Or just exit.
                                                  // If GetProcAddress failed, [hModule] is still on stack.
                    
                    // We have [hModule] on stack. We don't strictly need it anymore, but it's there.
                    // We can just pop it or ignore it. 
                    // Wait, calling convention! 
                    // Let's clean up [hModule] before call? Or keeps it?
                    // EAX has FunctionAddress.
                    
                    // Call Init(arg)
                    $"push {argAddr}",             // Push Argument
                    "call eax",                   // Call Init
                    
                    // Init is presumably stdcall, so it pops argAddr.
                    // Stack still has [hModule]. We should pop it to be clean, or just ret.
                    // Since we are creating a thread, 'ret' usually expects stack empty? 
                    // Thread proc: DWORD WINAPI ThreadProc(LPVOID lpParameter).
                    // We are executing raw code. 'ret' returns to... kernel32 thread starter?
                    // We should probably explicitly ExitThread(1) or just ret.
                    // If we 'ret', we should balance stack corresponding to what we received?
                    // CreateRemoteThread starts us. We technically receive lpParameter on stack?
                    // But we ignore it.
                    // We should clean up our own pushes.
                    
                    "pop ecx",                    // Pop saved hModule
                    "mov eax, 1",
                    "ret",

                    "error_pop:",
                    "pop ecx",                    // Pop saved hModule
                    "error:",
                    "mov eax, 0",
                    "ret"
                ];

                if (!InjectAssembly(asm, codeAddr))
                {
                    AmeisenLogger.I.Log("InjectDll", "Failed to assemble shellcode", LogLevel.Error);
                    return false;
                }

                // 5. Execute via NtCreateThreadEx
                AmeisenLogger.I.Log("InjectDll", $"Executing loader at 0x{codeAddr:X} via NtCreateThreadEx...");

                int status = NtCreateThreadEx(out nint hThread, ProcessAccessFlag.All, nint.Zero, ProcessHandle, codeAddr, nint.Zero, false, 0, 0, 0, nint.Zero);

                if (status != 0 || hThread == nint.Zero)
                {
                    AmeisenLogger.I.Log("InjectDll", $"NtCreateThreadEx failed (Status: 0x{status:X})", LogLevel.Error);
                    return false;
                }

                // Wait up to 5s using NtWaitForSingleObject
                long timeoutVal = -5000 * 10000; // 5 seconds in 100ns units (negative for relative time)
                NtWaitForSingleObject(hThread, false, (nint)(&timeoutVal));

                // Wait for DLL to load using stealthy PEB walk
                int retries = 50;
                while (retries-- > 0)
                {
                    nint baseAddr = GetModuleBaseAddressFromPeb(Path.GetFileName(dllPath));
                    if (baseAddr != nint.Zero)
                    {
                        AmeisenLogger.I.Log("InjectDll", "DLL Loaded successfully (Found in PEB).", LogLevel.Debug);

                        HideModule(Path.GetFileName(dllPath));
                        ErasePEHeader(Path.GetFileName(dllPath));
                        return true;
                    }
                    Thread.Sleep(100);
                }

                AmeisenLogger.I.Log("InjectDll", "Timed out waiting for DLL to load in PEB.", LogLevel.Error);
                return false;
            }
            finally
            {
                // Cleanup
                if (dataAddr != nint.Zero)
                {
                    FreeMemory(dataAddr);
                }

                if (codeAddr != nint.Zero)
                {
                    FreeMemory(codeAddr);
                }

                if (argAddr != nint.Zero)
                {
                    FreeMemory(argAddr);
                }
            }
        }

        ///<inheritdoc cref="IMemoryApi.PatchMemory"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PatchMemory<T>(nint address, T data) where T : unmanaged
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
#endif
            uint size = (uint)sizeof(T);

            if (ProtectMemory(address, size, MemoryProtectionFlag.ExecuteReadWrite, out MemoryProtectionFlag oldMemoryProtection))
            {
                Write(address, data);
                ProtectMemory(address, size, oldMemoryProtection, out _);
            }
        }

        ///<inheritdoc cref="IMemoryApi.ProtectMemory"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ProtectMemory(nint address, uint size, MemoryProtectionFlag memoryProtection, out MemoryProtectionFlag oldMemoryProtection)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (size <= 0) { throw new ArgumentOutOfRangeException(nameof(size), "size must be > 0"); }
#endif
            nint s = new(size);
            return NtProtectVirtualMemory(ProcessHandle, ref address, ref s, memoryProtection, out oldMemoryProtection) == 0;
        }

        ///<inheritdoc cref="IMemoryApi.Read"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Read<T>(nint address, out T value) where T : unmanaged
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            //if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
#endif
            int size = sizeof(T);

            fixed (byte* pBuffer = stackalloc byte[size])
            {
                if (RpmGateWay(address, pBuffer, size))
                {
                    value = *(T*)pBuffer;
                    return true;
                }
            }

            value = default;
            return false;
        }

        ///<inheritdoc cref="IMemoryApi.ReadBytes"/>
        ///<inheritdoc cref="IMemoryApi.ReadBytes"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBytes(nint address, int size, out byte[] bytes)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (size <= 0) { throw new ArgumentOutOfRangeException(nameof(size), "size must be > 0"); }
#endif
            bytes = new byte[size];
            return ReadBytes(address, (Span<byte>)bytes);
        }

        ///<inheritdoc cref="IMemoryApi.ReadBytes"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBytes(nint address, Span<byte> buffer)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (buffer.Length <= 0) { throw new ArgumentOutOfRangeException(nameof(buffer), "buffer size must be > 0"); }
#endif
            fixed (byte* pBuffer = buffer)
            {
                return RpmGateWay(address, pBuffer, buffer.Length);
            }
        }

        ///<inheritdoc cref="IMemoryApi.ReadString"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadString(nint address, Encoding encoding, out string value, int bufferSize = 512)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding), "encoding cannot be null"); }
            if (bufferSize <= 0) { throw new ArgumentOutOfRangeException(nameof(bufferSize), "bufferSize must be > 0"); }
#endif
            StringBuilder sb = new(bufferSize);

            fixed (byte* pBuffer = stackalloc byte[bufferSize])
            {
                int i;
                // Check if encoding is Unicode (UTF-16) which requires 2-byte null terminator
                bool isUnicode = encoding == Encoding.Unicode || encoding == Encoding.BigEndianUnicode;
                int charSize = isUnicode ? 2 : 1;

                do
                {
                    if (!RpmGateWay(address, pBuffer, bufferSize))
                    {
                        value = string.Empty;
                        return false;
                    }

                    i = 0;

                    if (isUnicode)
                    {
                        // For Unicode, look for 2-byte null terminator (0x00 0x00)
                        while (i < bufferSize - 1 && !(pBuffer[i] == 0 && pBuffer[i + 1] == 0))
                        {
                            i += 2;
                        }
                    }
                    else
                    {
                        // For ASCII/UTF-8, look for single null byte
                        while (i < bufferSize && pBuffer[i] != 0)
                        {
                            ++i;
                        }
                    }

                    address += i;

                    sb.Append(encoding.GetString(pBuffer, i));
                } while (i == bufferSize || (isUnicode && i == bufferSize - 1));

                value = sb.ToString();
                return true;
            }
        }

        ///<inheritdoc cref="IMemoryApi.ResizeParentWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeParentWindow(int offsetX, int offsetY, int width, int height)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
#endif
            SetWindowPos(Process.MainWindowHandle, nint.Zero, offsetX, offsetY, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        ///<inheritdoc cref="IMemoryApi.ResumeMainThread"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResumeMainThread()
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
#endif
            NtResumeThread(MainThreadHandle, out _);
        }

        ///<inheritdoc cref="IMemoryApi.SetForegroundWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetForegroundWindow(nint windowHandle)
        {
            Win32Imports.SetForegroundWindow(windowHandle);
        }

        ///<inheritdoc cref="IMemoryApi.MinimizeWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MinimizeWindow(nint windowHandle)
        {
            Win32Imports.ShowWindow(windowHandle, SW_SHOWMINNOACTIVE);
        }

        ///<inheritdoc cref="IMemoryApi.HideWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void HideWindow(nint windowHandle)
        {
            Win32Imports.ShowWindow(windowHandle, SW_HIDE);
        }

        ///<inheritdoc cref="IMemoryApi.SetupAutoPosition"/>
        public void SetupAutoPosition(nint mainWindowHandle, int offsetX, int offsetY, int width, int height)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
#endif
            if (Process == null)
            {
                return;
            }

            if (Process.MainWindowHandle != nint.Zero && mainWindowHandle != nint.Zero)
            {
                // Optimization: If already parented, just resize and return
                if (GetParent(Process.MainWindowHandle) == mainWindowHandle)
                {
                    ResizeParentWindow(offsetX, offsetY, width, height);
                    return;
                }

                // Hide window immediately to prevent cursor flickering during parenting
                // Hide window immediately to prevent cursor flickering during parenting
                Win32Imports.ShowWindow(Process.MainWindowHandle, SW_HIDE);

                // Retry loop with exponential backoff - most systems parent quickly
                int maxAttempts = 20; // 1 second max with backoff
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    SetParent(Process.MainWindowHandle, mainWindowHandle);

                    nint currentParent = GetParent(Process.MainWindowHandle);
                    if (currentParent == mainWindowHandle)
                    {
                        // Success - apply styles and position while still hidden
                        int style = GetWindowLong(Process.MainWindowHandle, GWL_STYLE);
                        style &= ~(int)WindowStyle.WS_CAPTION & ~(int)WindowStyle.WS_THICKFRAME & ~(int)WindowStyle.WS_BORDER;
                        _ = SetWindowLong(Process.MainWindowHandle, GWL_STYLE, style);

                        ResizeParentWindow(offsetX, offsetY, width, height);

                        // NOW show the window - appears already in correct position
                        Win32Imports.ShowWindow(Process.MainWindowHandle, SW_SHOW);
                        return;
                    }

                    // Exponential backoff: 25ms, 30ms, 35ms... up to 120ms
                    Thread.Sleep(25 + (attempt * 5));
                }

                // Final fallback - show anyway even if parenting failed
                AmeisenLogger.I.Log("XMemory", "SetupAutoPosition: Failed to parent window after retries", LogLevel.Warning);
                int finalStyle = GetWindowLong(Process.MainWindowHandle, GWL_STYLE);
                finalStyle &= ~(int)WindowStyle.WS_CAPTION & ~(int)WindowStyle.WS_THICKFRAME & ~(int)WindowStyle.WS_BORDER;
                _ = SetWindowLong(Process.MainWindowHandle, GWL_STYLE, finalStyle);
                ResizeParentWindow(offsetX, offsetY, width, height);
                Win32Imports.ShowWindow(Process.MainWindowHandle, SW_SHOW);
            }
        }

        ///<inheritdoc cref="IMemoryApi.SetWindowPosition"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWindowPosition(nint windowHandle, Rect rect, bool resizeWindow = true)
        {
            WindowFlag flags = WindowFlag.AsyncWindowPos | WindowFlag.NoZOrder | WindowFlag.NoActivate;

            if (!resizeWindow) { flags |= WindowFlag.NoSize; }

            if (rect.Left > 0 && rect.Right > 0 && rect.Top > 0 && rect.Bottom > 0)
            {
                SetWindowPos(windowHandle, nint.Zero, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, (int)flags);
            }
        }

        ///<inheritdoc cref="IMemoryApi.ShowWindow"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShowWindow(nint windowHandle)
        {
            Win32Imports.ShowWindow(windowHandle, SW_SHOW);
        }

        ///<inheritdoc cref="IMemoryApi.StartProcessNoActivate"/>
        public Process StartProcessNoActivate(string processCmd, out nint processHandle, out nint threadHandle, bool startHidden = false, Rect? windowRect = null)
        {
            int flags = STARTF_USESHOWWINDOW;

            StartupInfo startupInfo = new()
            {
                cb = Marshal.SizeOf<StartupInfo>(),
                wShowWindow = (short)(startHidden ? SW_HIDE : SW_SHOWMINNOACTIVE)
            };

            // If window position specified, start WoW directly at that location
            if (windowRect.HasValue && windowRect.Value.Right > 0 && windowRect.Value.Bottom > 0)
            {
                flags |= STARTF_USEPOSITION | STARTF_USESIZE;
                startupInfo.dwX = windowRect.Value.Left;
                startupInfo.dwY = windowRect.Value.Top;
                startupInfo.dwXSize = windowRect.Value.Right - windowRect.Value.Left;
                startupInfo.dwYSize = windowRect.Value.Bottom - windowRect.Value.Top;
            }

            startupInfo.dwFlags = flags;

            if (CreateProcess(null, processCmd, nint.Zero, nint.Zero, true, 0x10, nint.Zero, null, ref startupInfo, out ProcessInformation processInformation))
            {
                processHandle = processInformation.hProcess;
                threadHandle = processInformation.hThread;
                return Process.GetProcessById(processInformation.dwProcessId);
            }
            else
            {
                processHandle = nint.Zero;
                threadHandle = nint.Zero;
                return null;
            }
        }

        ///<inheritdoc cref="IMemoryApi.SuspendMainThread"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SuspendMainThread()
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
#endif
            NtSuspendThread(MainThreadHandle, out _);
        }

        ///<inheritdoc cref="IMemoryApi.Write"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Write<T>(nint address, T value) where T : unmanaged
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
#endif
            return WpmGateWay(address, &value, sizeof(T));
        }

        ///<inheritdoc cref="IMemoryApi.WriteBytes"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteBytes(nint address, byte[] bytes)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (bytes?.Length <= 0) { throw new ArgumentOutOfRangeException(nameof(bytes), "bytes size must be > 0"); }
#endif
            return WriteBytes(address, (ReadOnlySpan<byte>)bytes);
        }

        ///<inheritdoc cref="IMemoryApi.WriteBytes"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteBytes(nint address, ReadOnlySpan<byte> buffer)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (buffer.Length <= 0) { throw new ArgumentOutOfRangeException(nameof(buffer), "buffer size must be > 0"); }
#endif
            fixed (byte* pBytes = buffer)
            {
                return WpmGateWay(address, pBytes, buffer.Length);
            }
        }

        ///<inheritdoc cref="IMemoryApi.ZeroMemory"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ZeroMemory(nint address, int size)
        {
#if DEBUG
            if (!Initialized) { throw new InvalidOperationException("call Init() before you do anything with this class"); }
            if (address == nint.Zero) { throw new ArgumentOutOfRangeException(nameof(address), "address must be > 0"); }
            if (size <= 0) { throw new ArgumentOutOfRangeException(nameof(size), "size must be > 0"); }
#endif
            // Use stack buffer to avoid heap allocations
            const int ChunkSize = 1024;
            byte* chunk = stackalloc byte[ChunkSize];
            Unsafe.InitBlock(chunk, 0, ChunkSize); // Guarantee zeroed

            int remaining = size;
            nint currentAddr = address;

            while (remaining > 0)
            {
                int writeSize = Math.Min(remaining, ChunkSize);
                if (!WpmGateWay(currentAddr, chunk, writeSize))
                {
                    return false;
                }
                remaining -= writeSize;
                currentAddr += writeSize;
            }
            return true;
        }

        /// <summary>
        /// FASM assembler library is used to assembly our injection stuff.
        /// </summary>
        /// <param name="szSource">Assembly instructions.</param>
        /// <param name="lpMemory">Output bytes</param>
        /// <param name="nSize">Output buffer size</param>
        /// <param name="nPassesLimit">FASM pass limit</param>
        /// <param name="hDisplayPipe">FASM display pipe</param>
        /// <returns>FASM status struct pointer</returns>
        [LibraryImport("FASM.dll", EntryPoint = "fasm_Assemble", StringMarshalling = StringMarshalling.Utf8)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        private static partial int FasmAssemble(string szSource, byte* lpMemory, int nSize, int nPassesLimit, nint hDisplayPipe);

        /// <summary>
        /// Get FASM assembler version.
        /// </summary>
        /// <returns>Version</returns>
        [LibraryImport("FASM.dll", EntryPoint = "fasm_GetVersion")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        private static partial int FasmGetVersion();

        /// <summary>
        /// Gateway function to monitor ReadProcessMemory calls.
        /// </summary>
        /// <param name="baseAddress">Address of the memory to read</param>
        /// <param name="buffer">Output bytes</param>
        /// <param name="size">Size of the memory to read</param>
        /// <returns>True if read was successful, false if not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RpmGateWay(nint baseAddress, void* buffer, int size)
        {
            ++rpmCalls;
            return NtReadVirtualMemory(ProcessHandle, baseAddress, buffer, size, out _) == 0;
        }

        /// <summary>
        /// Gateway function to monitor WriteProcessMemory calls.
        /// </summary>
        /// <param name="baseAddress">Address of the memory to write</param>
        /// <param name="buffer">Input bytes</param>
        /// <param#
        /// name="size">Size of the memory to write</param>
        /// <returns>True if write was successful, false if not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool WpmGateWay(nint baseAddress, void* buffer, int size)
        {
            ++wpmCalls;
            return NtWriteVirtualMemory(ProcessHandle, baseAddress, buffer, size, out _) == 0;
        }

        public bool HideModule(string moduleName)
        {
            try
            {
                // Try exact match first
                if (IteratePebModules(moduleName, out int entryAddress, out _, unlink: true))
                {
                    AmeisenLogger.I.Log("Stealth", $"Unlinked module {moduleName} from PEB.", LogLevel.Debug);
                    return true;
                }

                // Try with .dll extension if not already present
                if (!moduleName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    string nameWithDll = moduleName + ".dll";
                    if (IteratePebModules(nameWithDll, out entryAddress, out _, unlink: true))
                    {
                        AmeisenLogger.I.Log("Stealth", $"Unlinked module {nameWithDll} from PEB.", LogLevel.Debug);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("HideModule", ex.Message, LogLevel.Error);
            }
            return false;
        }

        public bool ErasePEHeader(string moduleName)
        {
            nint baseAddr = GetModuleBaseAddressFromPeb(moduleName);
            return baseAddr != nint.Zero && ZeroMemory(baseAddr, 4096);
        }

        private nint GetModuleBaseAddressFromPeb(string moduleName)
        {
            // Try exact match first
            if (IteratePebModules(moduleName, out _, out nint baseAddr, unlink: false))
            {
                return baseAddr;
            }

            // If not found and doesn't end with .dll, try with .dll extension
            // (Windows stores modules in PEB with full filename including .dll)
            if (!moduleName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                if (IteratePebModules(moduleName + ".dll", out _, out baseAddr, unlink: false))
                {
                    AmeisenLogger.I.Log("PebWalker", $"Found {moduleName} as {moduleName}.dll in PEB", LogLevel.Debug);
                    return baseAddr;
                }
            }

            return nint.Zero;
        }

        private void UnlinkListEntry(int entryAddr)
        {
            if (Read<int>(entryAddr, out int flink) && Read<int>(entryAddr + 0x04, out int blink))
            {
                Write<int>(blink, flink);
                Write<int>(flink + 0x04, blink);
            }
        }

        private bool IteratePebModules(string moduleName, out int entryAddress, out nint baseAddr, bool unlink)
        {
            entryAddress = 0;
            baseAddr = nint.Zero;

            // 1. Get 32-bit PEB address (32-bit bot reading 32-bit WoW)
            if (NtQueryInformationProcess(ProcessHandle, PROCESSINFOCLASS.ProcessBasicInformation, out PROCESS_BASIC_INFORMATION pbi, Marshal.SizeOf<PROCESS_BASIC_INFORMATION>(), out _) != 0 || pbi.PebBaseAddress == nint.Zero)
            {
                AmeisenLogger.I.Log("PebWalker", "Failed to get PEB address via ProcessBasicInformation", LogLevel.Error);
                return false;
            }

            nint pebAddress = pbi.PebBaseAddress;
            AmeisenLogger.I.Log("PebWalker", $"Searching for '{moduleName}' in PEB at 0x{pebAddress:X}", LogLevel.Debug);

            // 2. Read Ldr pointer (PEB + 0x0C)
            if (!Read<int>(pebAddress + 0x0C, out int ldrPtr) || ldrPtr == 0)
            {
                AmeisenLogger.I.Log("PebWalker", "Failed to read Ldr pointer from PEB+0x0C", LogLevel.Error);
                return false;
            }

            // 3. Walk InMemoryOrderModuleList (Ldr + 0x14)
            int head = ldrPtr + 0x14;
            if (!Read<int>(head, out int current))
            {
                AmeisenLogger.I.Log("PebWalker", "Failed to read InMemoryOrderModuleList head", LogLevel.Error);
                return false;
            }

            int count = 0;
            int limit = 200;

            while (current != head && current != 0 && limit-- > 0)
            {
                count++;

                // Read FullDllName (contains full path like "C:\path\to\module.dll")
                // IMPORTANT: 'current' points to InMemoryOrderLinks (+0x08 into _LDR_DATA_TABLE_ENTRY)
                // FullDllName is at absolute +0x24, so relative to current: +0x24 - 0x08 = +0x1C
                // UNICODE_STRING: +0x00 Length (USHORT), +0x02 MaxLength (USHORT), +0x04 Buffer (PWSTR)
                int fullDllNameOffset = current + 0x1C;  // Relative to InMemoryOrderLinks position

                // Read UNICODE_STRING length and buffer pointer
                if (Read<ushort>(fullDllNameOffset, out ushort strLength) &&
                    Read<int>(fullDllNameOffset + 0x04, out int bufferPtr) && bufferPtr != 0)
                {

                    if (ReadString(bufferPtr, Encoding.Unicode, out string fullPath, 512))
                    {
                        // Extract just the filename from the full path
                        string fileName = System.IO.Path.GetFileName(fullPath);

                        if (fileName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                        {
                            // DllBase is at absolute +0x18, relative to current: +0x18 - 0x08 = +0x10
                            if (Read<int>(current + 0x10, out int dllBase32))
                            {
                                baseAddr = dllBase32;
                                AmeisenLogger.I.Log("PebWalker", $"FOUND: '{fileName}' at 0x{baseAddr:X}", LogLevel.Debug);
                            }

                            if (unlink)
                            {
                                // Zero out the full path buffer
                                ZeroMemory(bufferPtr, fullPath.Length * 2);

                                int entryStart = current - 0x08;
                                UnlinkListEntry(entryStart + 0x00); // InLoadOrderLinks
                                UnlinkListEntry(entryStart + 0x08); // InMemoryOrderLinks
                                UnlinkListEntry(entryStart + 0x10); // InInitializationOrderLinks
                            }

                            entryAddress = current;
                            return true;
                        }
                    }
                }

                if (!Read<int>(current, out int next))
                {
                    AmeisenLogger.I.Log("PebWalker", $"Failed to read next entry after {count} modules", LogLevel.Debug);
                    break;
                }
                current = next;
            }

            AmeisenLogger.I.Log("PebWalker", $"NOT FOUND: '{moduleName}' (scanned {count} modules)", LogLevel.Warning);
            return false;
        }
    }
}