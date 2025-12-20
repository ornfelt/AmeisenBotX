using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ
{
    public static unsafe partial class StormLib
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SFILE_FIND_DATA
        {
            public fixed byte cFileName[260];
            public nint szPlainName;
            public nint dwHashIndex;
            public nint dwBlockIndex;
            public nint dwFileSize;
            public nint dwFileFlags;
            public nint dwCompSize;
            public nint dwFileTimeLo;
            public nint dwFileTimeHi;
            public nint lcLocale;
        }

        private const string DLL = "StormLib.dll";

        public const uint MPQ_OPEN_READ_ONLY = 0x0100;

        [LibraryImport(DLL, EntryPoint = "SFileOpenArchive", StringMarshalling = StringMarshalling.Utf16)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SFileOpenArchive(string szMpqName, uint dwPriority, uint dwFlags, out nint phMpq);

        [LibraryImport(DLL, EntryPoint = "SFileOpenFileEx", StringMarshalling = StringMarshalling.Utf8)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SFileOpenFileEx(nint hMpq, string szFileName, uint dwSearchScope, out nint phFile);

        [LibraryImport(DLL, EntryPoint = "SFileGetFileSize")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        internal static partial uint SFileGetFileSize(nint hFile, out uint pdwFileSizeHigh);

        [LibraryImport(DLL, EntryPoint = "SFileReadFile")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SFileReadFile(nint hFile, byte[] lpBuffer, uint dwToRead, out uint pdwRead, nint lpOverlapped);

        [LibraryImport(DLL, EntryPoint = "SFileCloseFile")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SFileCloseFile(nint hFile);

        [LibraryImport(DLL, EntryPoint = "SFileCloseArchive")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SFileCloseArchive(nint hMpq);

        [LibraryImport(DLL, EntryPoint = "SFileFindFirstFile", StringMarshalling = StringMarshalling.Utf8)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        internal static partial nint SFileFindFirstFile(nint hMpq, string szFileName, SFILE_FIND_DATA* data, nint phListfile);

        [LibraryImport(DLL, EntryPoint = "SFileFindClose")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
        internal static partial void SFileFindClose(nint hFind);
    }
}
