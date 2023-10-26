using System.Runtime.InteropServices;

namespace AmeisenBotX.Memory.Structs
{
    /// <summary>
    /// Represents the state of a FASM error.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FasmStateError
    {
        /// <summary>
        /// Gets or sets the condition value.
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public int ErrorCode { get; set; }
    }
}