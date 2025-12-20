using System.Runtime.InteropServices;

namespace AmeisenBotX.Memory.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FasmMemoryRegion
    {
        nint Address;
        nint Size;
    }
}
