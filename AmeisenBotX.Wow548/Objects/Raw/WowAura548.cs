using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Flags;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Raw
{
    /// <summary>
    /// Represents a World of Warcraft aura with the structure layout defined as Sequential.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowAura548 : IWowAura
    {
        /// <summary>
        /// Array of fixed (non-resizable) integers with a length of 7.
        /// </summary>
        public fixed int Pad0[7];

        /// <summary>
        /// Gets or sets the flags as a byte value.
        /// </summary>
        public byte Flags { get; set; }

        /// <summary>
        /// Gets or sets the count of elements in the stack.
        /// </summary>
        public byte StackCount { get; set; }

        /// <summary>
        /// Gets or sets the unknown byte value.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the level in bytes.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the creator of the object.
        /// </summary>
        public ulong Creator { get; set; }

        /// <summary>
        /// Gets or sets the spell ID.
        /// </summary>
        public int SpellId { get; set; }

        /// <summary>
        /// Represents five fixed integers for padding, allocated in memory.
        /// </summary>
        public fixed int Pad1[5];

        /// <summary>
        /// Determines if the aura is active based on the flag value.
        /// </summary>
        public bool IsActive => ((WowAuraFlag)Flags).HasFlag(WowAuraFlag.Active);

        /// <summary>
        /// Determines if the Aura is harmful based on the flags it possesses.
        /// </summary>
        public bool IsHarmful => ((WowAuraFlag)Flags).HasFlag(WowAuraFlag.Harmful);

        /// <summary>
        /// Checks if the aura flag is a passive ability.
        /// </summary>
        public bool IsPassive => ((WowAuraFlag)Flags).HasFlag(WowAuraFlag.Passive);

        /// <summary>
        /// Overrides the ToString() method to provide a string representation of the object's properties, including the SpellId, Level, StackCount, Creator, IsHarmful, and IsPassive.
        /// </summary>
        public override string ToString()
        {
            return $"{SpellId} (lvl. {Level}) x{StackCount} [CG: {Creator}], Harmful: {IsHarmful}, Passive: {IsPassive}";
        }
    }
}