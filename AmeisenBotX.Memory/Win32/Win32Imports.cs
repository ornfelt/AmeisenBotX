using System;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Memory.Win32
{
    public static unsafe class Win32Imports
    {
        /// <summary>
        /// The extended window style of a window.
        /// </summary>
        public const int GWL_EXSTYLE = -0x14;
        /// <summary>
        /// Represents the constant value for the style of a window in the Windows API.
        /// The value is set to -16.
        /// </summary>
        public const int GWL_STYLE = -16;
        /// <summary>
        /// Specifies that the StartInfo structure's ShowWindow member should be used.
        /// </summary>
        public const int STARTF_USESHOWWINDOW = 1;
        /// <summary>
        /// Constant representing the SW_SHOWMINNOACTIVE value, which indicates that a window should be minimized and activated if necessary.
        /// </summary>
        public const int SW_SHOWMINNOACTIVE = 7;
        /// <summary>
        /// Specifies that a window is shown in its most recent size and position, but is not activated.
        /// </summary>
        public const int SW_SHOWNOACTIVATE = 4;
        /// <summary>
        /// Specifies that a window is to be positioned without being activated.
        /// </summary>
        public const int SWP_NOACTIVATE = 0x10;
        /// <summary>
        /// Represents a constant value indicating that the window's Z-order should not be changed.
        /// </summary>
        public const int SWP_NOZORDER = 0x4;

        /// <summary>
        /// Represents the allocation types for memory regions in a process.
        /// </summary>
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        /// <summary>
        /// Specifies the memory protection flags for a memory region.
        /// </summary>
        [Flags]
        public enum MemoryProtectionFlag
        {
            NoAccess = 0x1,
            ReadOnly = 0x2,
            ReadWrite = 0x4,
            WriteCopy = 0x8,
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        /// <summary>
        /// Specifies the access rights to a process.
        /// This enumeration type is used by the OpenProcess and AccessCheckAndAuditAlarm functions.
        /// </summary>
        [Flags]
        public enum ProcessAccessFlag
        {
            All = 0x1F0FFF,
            Terminate = 0x1,
            CreateThread = 0x2,
            VirtualMemoryOperation = 0x8,
            VirtualMemoryRead = 0x10,
            VirtualMemoryWrite = 0x20,
            DuplicateHandle = 0x40,
            CreateProcess = 0x80,
            SetQuota = 0x100,
            SetInformation = 0x200,
            QueryInformation = 0x400,
            QueryLimitedInformation = 0x1000,
            Synchronize = 0x100000
        }

        /// <summary>
        /// Flags that specify the access rights for a thread.
        /// </summary>
        [Flags]
        public enum ThreadAccessFlag
        {
            Terminate = 0x1,
            SuspendResume = 0x2,
            GetContext = 0x8,
            SetContext = 0x10,
            SetInformation = 0x20,
            QueryInformation = 0x40,
            SetThreadToken = 0x80,
            Impersonate = 0x100,
            DirectImpersonation = 0x200
        }

        /// <summary>
        /// Represents flags that specify window states and behaviors.
        /// </summary>
        [Flags]
        public enum WindowFlag
        {
            NoSize = 0x1,
            NoMove = 0x2,
            NoZOrder = 0x4,
            NoRedraw = 0x8,
            NoActivate = 0x10,
            DrawFrame = 0x20,
            ShowWindow = 0x40,
            HideWindow = 0x80,
            NoCopyBits = 0x100,
            NoOwnerZOrder = 0x200,
            NoSendChanging = 0x400,
            Defererase = 0x2000,
            AsyncWindowPos = 0x4000
        }

        /// <summary>
        /// Represents the window styles that can be applied to a window.
        /// </summary>
        [Flags]
        public enum WindowStyle : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = WS_GROUP,
            WS_MAXIMIZEBOX = WS_TABSTOP,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
        }

        ///<summary>
        ///Imports the user32 library and provides the functionality to retrieve the coordinates of the bounding rectangle for a specified window.
        ///</summary>
        ///<param name="windowHandle">The handle to the window for which the coordinates are to be retrieved.</param>
        ///<param name="rectangle">A reference to a Rect structure that will receive the coordinates of the window's bounding rectangle.</param>
        ///<returns>True if the function succeeds, otherwise false.</returns>
        [DllImport("user32", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr windowHandle, ref Rect rectangle);

        /// <summary>
        /// The CloseHandle function closes an open object handle.
        /// </summary>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr threadHandle);

        /// <summary>
        /// The CreateProcess function creates a new process, which runs independently of the calling process.
        /// </summary>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool CreateProcess
                (
                    string lpApplicationName,
                    string lpCommandLine,
                    IntPtr lpProcessAttributes,
                    IntPtr lpThreadAttributes,
                    bool bInheritHandles,
                    uint dwCreationFlags,
                    IntPtr lpEnvironment,
                    string lpCurrentDirectory,
                    [In] ref StartupInfo lpStartupInfo,
                    out ProcessInformation lpProcessInformation
                );

        /// <summary>
        /// The external method <see cref="GetClientRect"/> is used to retrieve the coordinates of the client area for the specified window handle.
        /// </summary>
        [DllImport("user32", SetLastError = true)]
        internal static extern bool GetClientRect(IntPtr windowHandle, ref Rect rectangle);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// </summary>
        [DllImport("user32", SetLastError = true)]
        internal static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves information about the specified window. The function retrieves the specified 32-bit (long) value associated with the window.
        /// </summary>
        [DllImport("user32", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr windowHandle, int index);

        /// <summary>
        /// Retrieves the identifier of the process that owns the specified window handle.
        /// </summary>
        /// <param name="windowHandle">The handle of the window.</param>
        /// <param name="processId">The process identifier.</param>
        /// <returns>Returns the identifier of the process that owns the window handle, or 0 if the function fails.</returns>
        [DllImport("user32", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(IntPtr windowHandle, int processId);

        /// <summary>
        /// Signature for a platform invocation to the NtReadVirtualMemory function in the ntdll module.
        /// </summary>
        [DllImport("ntdll", SetLastError = true)]
        internal static extern bool NtReadVirtualMemory(IntPtr processHandle, IntPtr baseAddress, void* buffer, int size, out IntPtr numberOfBytesRead);

        /// <summary>
        ///     Specifies a platform invoke method that resumes a suspended thread.
        /// </summary>
        [DllImport("ntdll", SetLastError = true)]
        internal static extern bool NtResumeThread(IntPtr threadHandle, out IntPtr suspendCount);

        /// <summary>
        /// External method used to suspend a thread using the Ntdll library.
        /// </summary>
        [DllImport("ntdll", SetLastError = true)]
        internal static extern bool NtSuspendThread(IntPtr threadHandle, out IntPtr previousSuspendCount);

        /// <summary>
        /// Extern method for writing virtual memory in a process.
        /// </summary>
        [DllImport("ntdll", SetLastError = true)]
        internal static extern bool NtWriteVirtualMemory(IntPtr processHandle, IntPtr baseAddress, void* buffer, int size, out IntPtr numberOfBytesWritten);

        /// <summary>
        /// Retrieves a handle to an existing process.
        /// </summary>
        /// <param name="processAccess">The desired access to the process.</param>
        /// <param name="inheritHandle">Whether or not the handle is inheritable by child processes.</param>
        /// <param name="processId">The identifier of the process to open.</param>
        /// <returns>A handle to the specified process.</returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern IntPtr OpenProcess(ProcessAccessFlag processAccess, bool inheritHandle, int processId);

        /// <summary>
        /// Extern method for opening a thread using kernel32 library.
        /// </summary>
        /// <param name="threadAccess">Flags specifying the desired access to the thread.</param>
        /// <param name="inheritHandle">Determines whether the returned handle will be inherited by child processes. </param>
        /// <param name="threadId">ID of the thread to be opened.</param>
        /// <returns>Returns a handle to the specified thread.</returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern IntPtr OpenThread(ThreadAccessFlag threadAccess, bool inheritHandle, uint threadId);

        /// <summary>
        /// The external method SetForegroundWindow is used to bring the specified window to the foreground and give it the input focus.
        /// </summary>
        [DllImport("user32", SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// The SetParent function changes the parent window of the specified child window.
        /// </summary>
        /// <param name="hWndChild">A handle to the child window.</param>
        /// <param name="hWndNewParent">A handle to the new parent window.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the previous parent window.
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Imports a function from the user32 library that sets a specified window attribute.
        /// </summary>
        /// <param name="windowHandle">The handle to the window.</param>
        /// <param name="index">The index of the attribute to set.</param>
        /// <param name="newLong">The new value of the attribute.</param>
        /// <returns>The return value is the previous value of the attribute.</returns>
        [DllImport("user32", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr windowHandle, int index, int newLong);

        /// <summary>
        /// The SetWindowPos function changes the position and size of the specified window. It also changes the window's Z order.
        /// </summary>
        /// <param name="windowHandle">The handle to the window.</param>
        /// <param name="windowHandleInsertAfter">The handle to the window to precede the positioned window in the Z order.</param>
        /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="wFlags">The window sizing and positioning flags.</param>
        /// <returns>
        /// If the function succeeds, it returns a handle to the window. If the function fails, it returns IntPtr.Zero.
        /// </returns>
        [DllImport("user32", SetLastError = true)]
        internal static extern IntPtr SetWindowPos(IntPtr windowHandle, IntPtr windowHandleInsertAfter, int x, int y, int cx, int cy, int wFlags);

        /// <summary>
        /// Imports the VirtualAllocEx function from the kernel32 library.
        /// Allocates memory within a specified process, with the specified size and allocation type, and sets the memory protection.
        /// Returns a pointer to the base address of the allocated memory block.
        /// </summary>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr processHandle, IntPtr address, uint size, AllocationType allocationType, MemoryProtectionFlag memoryProtection);

        /// <summary>
        /// The external method that frees allocated memory within a specified process.
        /// </summary>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool VirtualFreeEx(IntPtr processHandle, IntPtr address, int size, AllocationType allocationType);

        /// <summary>
        /// The DllImport attribute allows managed code to call functions in unmanaged DLLs.
        /// This particular function imports the VirtualProtectEx function from the kernel32 DLL.
        /// It allows us to change the protection attributes of a region of memory in a process.
        /// </summary>
        /// <param name="hProcess">A handle to the process whose memory protection attributes will be changed.</param>
        /// <param name="lpAddress">A pointer to the base address of the region of memory to be protected.</param>
        /// <param name="dwSize">The size of the region of memory to be protected (in bytes).</param>
        /// <param name="flNewProtect">The new memory protection attributes to be assigned to the region.</param>
        /// <param name="lpflOldProtect">A pointer to a variable that receives the old memory protection attributes of the region.</param>
        /// <returns><c>true</c> if the function succeeds, <c>false</c> otherwise.</returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, MemoryProtectionFlag flNewProtect, out MemoryProtectionFlag lpflOldProtect);

        /// <summary>
        /// Represents process information including the process and thread handles, as well as the process and thread IDs.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessInformation
        {
            /// <summary>
            /// Represents a handle to an open process.
            /// </summary>
            public IntPtr hProcess;
            /// <summary>
            /// Represents a handle to a thread.
            /// </summary>
            public IntPtr hThread;
            /// <summary>
            /// Gets or sets the process ID.
            /// </summary>
            public int dwProcessId;
            /// <summary>
            /// Gets or sets the thread identifier.
            /// </summary>
            public int dwThreadId;
        }

        /// <summary>
        /// Represents startup information for a process.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct StartupInfo
        {
            /// <summary>
            /// Represents the size, in bytes, of a particular data element.
            /// </summary>
            public int cb;
            /// <summary>
            /// Gets or sets the reserved area of the code.
            /// </summary>
            public string lpReserved;
            /// <summary>
            /// Gets or sets the name of the logical desktop that the current thread is associated with.
            /// </summary>
            public string lpDesktop;
            /// <summary>
            /// The title of the lp.
            /// </summary>
            public string lpTitle;
            /// <summary>
            /// Gets or sets the X-coordinate of the window's position.
            /// </summary>
            public int dwX;
            /// <summary>
            /// The value of dwY represents the current position of an element on the y-axis.
            /// </summary>
            public int dwY;
            /// <summary>
            /// Gets or sets the X size of the object.
            /// </summary>
            public int dwXSize;
            /// <summary>
            /// Represents the Y dimension size of the dwYSize property.
            /// </summary>
            public int dwYSize;
            /// <summary>
            /// The number of X characters in the dwXCountChars variable.
            /// </summary>
            public int dwXCountChars;
            /// <summary>
            /// Represents the count of characters in the Y dimension.
            /// </summary>
            public int dwYCountChars;
            /// <summary>
            /// The dwell fill attribute.
            /// </summary>
            public int dwFillAttribute;
            /// <summary>
            /// Represents the dwFlags field.
            /// </summary>
            public int dwFlags;
            /// <summary>
            /// Gets or sets the show window value.
            /// </summary>
            public short wShowWindow;
            /// <summary>
            /// Represents the reserved field 2 for a given object as a short data type.
            /// </summary>
            public short cbReserved2;
            /// <summary>
            /// Gets or sets the reserved pointer 2.
            /// </summary>
            public IntPtr lpReserved2;
            /// <summary>
            /// Gets or sets the handle for the standard input stream.
            /// </summary>
            public IntPtr hStdInput;
            /// <summary>
            /// Gets or sets the handle to the standard output device.
            /// </summary>
            public IntPtr hStdOutput;
            /// <summary>
            /// Gets or sets the standard error handle for the console.
            /// </summary>
            public IntPtr hStdError;
        }
    }
}