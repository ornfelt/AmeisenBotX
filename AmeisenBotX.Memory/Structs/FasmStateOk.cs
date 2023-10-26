using System;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Memory.Structs
{
    /// <summary>
    /// Represents a struct that holds information about the state of the FASM assembler.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FasmStateOk
    {
        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        /// Gets or sets the length of the output.
        /// </summary>
        public uint OutputLength { get; set; }

        /// <summary>
        /// Gets or sets the output data pointer.
        /// </summary>
        public IntPtr OutputData { get; set; }
    }
}