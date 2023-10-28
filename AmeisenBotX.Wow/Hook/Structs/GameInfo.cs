using System.Runtime.InteropServices;

/// <summary>
/// Represents a boolean value indicating if the subject is outdoors.
/// </summary>
namespace AmeisenBotX.Wow.Hook.Structs
{
    /// <summary>
    /// General game information will be stored in this struct.
    ///
    /// There exists one instance in wow's memory that will be modified by asm code. This saves us a
    /// lot of calls to engine functions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GameInfo
    {
        /// <summary>
        /// Represents a boolean value indicating if the subject is outdoors.
        /// </summary>
        public bool isOutdoors;
        /// <summary>
        /// Represents the result of a line of sight check.
        /// </summary>
        public int losCheckResult;
    }
}