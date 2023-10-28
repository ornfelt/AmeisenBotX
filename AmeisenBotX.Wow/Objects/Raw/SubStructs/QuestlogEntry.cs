using System.Runtime.InteropServices;

/// <summary>
/// Represents a sub-structure for raw objects related to quest logs.
/// </summary>
namespace AmeisenBotX.Wow.Objects.Raw.SubStructs
{
    /// <summary>
    /// Represents a quest log entry.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct QuestlogEntry
    {
        /// <summary>
        /// Gets or sets the unique identifier of the object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the process has finished.
        /// </summary>
        public int Finished { get; set; }

        /// <summary>
        /// Gets or sets the progress of party member 1.
        /// </summary>
        public short ProgressPartymember1 { get; set; }

        /// <summary>
        /// Gets or sets the progress of Party Member 2.
        /// </summary>
        public short ProgressPartymember2 { get; set; }

        /// <summary>
        /// Gets or sets the progress of party member number 3.
        /// </summary>
        public short ProgressPartymember3 { get; set; }

        /// <summary>
        /// Gets or sets the progress of party member 4.
        /// </summary>
        public short ProgressPartymember4 { get; set; }

        /// <summary>
        /// Gets or sets the value of Y.
        /// </summary>
        public int Y { get; set; }
    }
}