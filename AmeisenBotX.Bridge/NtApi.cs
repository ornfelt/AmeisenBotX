using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Low-level NTDLL API wrapper for stealthy IPC operations.
/// Uses native NT APIs instead of kernel32 for reduced detection surface.
/// </summary>
public static unsafe partial class NtApi
{
    private static readonly int _sessionId;

    static NtApi()
    {
        // Cache session ID for path resolution
        ProcessIdToSessionId(GetCurrentProcessId(), out var id);
        _sessionId = (int)id;
    }

    // ============================================================
    // NTSTATUS Codes
    // ============================================================
    
    public const int STATUS_SUCCESS = 0;
    public const int STATUS_OBJECT_NAME_NOT_FOUND = unchecked((int)0xC0000034);
    public const int STATUS_ACCESS_DENIED = unchecked((int)0xC0000022);
    public const int STATUS_OBJECT_NAME_COLLISION = unchecked((int)0xC0000035);
    public const int STATUS_OBJECT_NAME_EXISTS = 0x40000000;
    
    // ============================================================
    // Section Access Rights
    // ============================================================
    
    public const uint SECTION_QUERY = 0x0001;
    public const uint SECTION_MAP_WRITE = 0x0002;
    public const uint SECTION_MAP_READ = 0x0004;
    public const uint SECTION_MAP_EXECUTE = 0x0008;
    public const uint SECTION_EXTEND_SIZE = 0x0010;
    public const uint SECTION_ALL_ACCESS = 0x000F001F;
    public const uint SECTION_MAP_READ_WRITE = SECTION_MAP_READ | SECTION_MAP_WRITE;
    
    // ============================================================
    // Memory Protection
    // ============================================================
    
    public const uint PAGE_READONLY = 0x02;
    public const uint PAGE_READWRITE = 0x04;
    public const uint PAGE_EXECUTE_READWRITE = 0x40;
    public const uint SEC_COMMIT = 0x08000000;
    
    // ============================================================
    // Object Attributes
    // ============================================================
    
    public const uint OBJ_INHERIT = 0x00000002;
    public const uint OBJ_OPENIF = 0x00000080;
    public const uint OBJ_CASE_INSENSITIVE = 0x00000040;
    
    /// <summary>ViewUnmap - view is unmapped when section handle is closed</summary>
    public const uint ViewUnmap = 2;
    
    /// <summary>Current process pseudo-handle</summary>
    public static readonly nint CurrentProcess = -1;
    
    // ============================================================
    // Structures
    // ============================================================
    
    [StructLayout(LayoutKind.Sequential)]
    public struct UNICODE_STRING
    {
        public ushort Length;
        public ushort MaximumLength;
        public nint Buffer;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct OBJECT_ATTRIBUTES
    {
        public int Length;
        public nint RootDirectory;
        public UNICODE_STRING* ObjectName;
        public uint Attributes;
        public nint SecurityDescriptor;
        public nint SecurityQualityOfService;
        
        /// <summary>Creates OBJECT_ATTRIBUTES for a named object.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OBJECT_ATTRIBUTES Create(UNICODE_STRING* name) => new()
        {
            Length = sizeof(OBJECT_ATTRIBUTES),
            RootDirectory = nint.Zero,
            ObjectName = name,
            Attributes = OBJ_CASE_INSENSITIVE,
            SecurityDescriptor = nint.Zero,
            SecurityQualityOfService = nint.Zero
        };
    }
    
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LARGE_INTEGER
    {
        [FieldOffset(0)] public long QuadPart;
        [FieldOffset(0)] public uint LowPart;
        [FieldOffset(4)] public int HighPart;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LARGE_INTEGER(long value) => new() { QuadPart = value };
    }
    
    // ============================================================
    // P/Invoke Declarations (LibraryImport source generator)
    // ============================================================
    
    [LibraryImport("ntdll")]
    public static partial int NtCreateSection(
        out nint SectionHandle,
        uint DesiredAccess,
        OBJECT_ATTRIBUTES* ObjectAttributes,
        LARGE_INTEGER* MaximumSize,
        uint SectionPageProtection,
        uint AllocationAttributes,
        nint FileHandle);
    
    [LibraryImport("ntdll")]
    public static partial int NtOpenSection(
        out nint SectionHandle,
        uint DesiredAccess,
        OBJECT_ATTRIBUTES* ObjectAttributes);
    
    [LibraryImport("ntdll")]
    public static partial int NtMapViewOfSection(
        nint SectionHandle,
        nint ProcessHandle,
        ref nint BaseAddress,
        nuint ZeroBits,
        nuint CommitSize,
        LARGE_INTEGER* SectionOffset,
        ref nuint ViewSize,
        uint InheritDisposition,
        uint AllocationType,
        uint Win32Protect);
    
    [LibraryImport("ntdll")]
    public static partial int NtUnmapViewOfSection(
        nint ProcessHandle,
        nint BaseAddress);
    
    [LibraryImport("ntdll")]
    public static partial int NtClose(nint Handle);
    
    [LibraryImport("ntdll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial void RtlInitUnicodeString(
        out UNICODE_STRING DestinationString,
        string SourceString);

    [LibraryImport("kernel32.dll")]
    private static partial int GetCurrentProcessId();

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ProcessIdToSessionId(int dwProcessId, out uint pSessionId);
    
    // ============================================================
    // High-Level Wrapper Methods
    // ============================================================
    
    /// <summary>
    /// Creates a named section (shared memory) backed by the pagefile.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CreateSection(string name, long size, out nint sectionHandle)
    {
        RtlInitUnicodeString(out var objectName, name);
        var objAttr = OBJECT_ATTRIBUTES.Create(&objectName);
        LARGE_INTEGER maxSize = size;
        
        return NtCreateSection(
            out sectionHandle,
            SECTION_ALL_ACCESS,
            &objAttr,
            &maxSize,
            PAGE_READWRITE,
            SEC_COMMIT,
            nint.Zero);
    }
    
    /// <summary>
    /// Opens an existing named section.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OpenSection(string name, out nint sectionHandle)
    {
        RtlInitUnicodeString(out var objectName, name);
        var objAttr = OBJECT_ATTRIBUTES.Create(&objectName);
        
        return NtOpenSection(out sectionHandle, SECTION_MAP_READ_WRITE, &objAttr);
    }
    
    /// <summary>
    /// Maps a section into the current process's address space.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MapViewOfSection(nint sectionHandle, nuint size, out nint baseAddress)
    {
        baseAddress = nint.Zero;
        var viewSize = size;
        
        return NtMapViewOfSection(
            sectionHandle,
            CurrentProcess,
            ref baseAddress,
            0, 0, null,
            ref viewSize,
            ViewUnmap,
            0,
            PAGE_READWRITE);
    }
    
    /// <summary>
    /// Unmaps a view of a section from the current process.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnmapViewOfSection(nint baseAddress)
        => NtUnmapViewOfSection(CurrentProcess, baseAddress);
    
    /// <summary>
    /// Closes a handle (section, file, etc.).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Close(nint handle)
        => NtClose(handle);
    
    /// <summary>
    /// Converts a Win32-style name to NT path, handling Session prefix correctly.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToNtPath(string win32Name) => win32Name switch
    {
        _ when win32Name.StartsWith("Local\\", StringComparison.OrdinalIgnoreCase) 
            => string.Concat($"\\Sessions\\{_sessionId}\\BaseNamedObjects\\", win32Name.AsSpan(6)),
        
        _ when win32Name.StartsWith("Global\\", StringComparison.OrdinalIgnoreCase) 
            => string.Concat("\\BaseNamedObjects\\", win32Name.AsSpan(7)),
        
        _ when win32Name.StartsWith('\\') 
            => win32Name,
        
        _ when _sessionId != 0 
            => string.Concat($"\\Sessions\\{_sessionId}\\BaseNamedObjects\\", win32Name),
            
        _ 
            => string.Concat("\\BaseNamedObjects\\", win32Name)
    };
    
    /// <summary>
    /// Checks if an NTSTATUS indicates success.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NT_SUCCESS(int status) => status >= 0;    
}
