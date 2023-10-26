using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Flags;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Raw
{
    /// <summary>
    /// Represents a World of Warcraft aura for version 3.3.5a.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WowAura335a : IWowAura
    {
        /// <summary>
        /// Gets or sets the value of the Creator property.
        /// </summary>
        public ulong Creator { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of a spell.
        /// </summary>
        public int SpellId { get; set; }

        /// <summary>
        /// Gets or sets the Flags property.
        /// </summary>
        public byte Flags { get; set; }

        /// <summary>
        /// Gets or sets the level in bytes.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the number of items currently on the stack.
        /// </summary>
        public byte StackCount { get; set; }

        /// <summary>
        /// Gets or sets the unknown byte value.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the duration of an event in seconds.
        /// </summary>
        public uint Duration { get; set; }

        /// <summary>
        /// Gets or sets the end time, represented as a unsigned integer.
        /// </summary>
        public uint EndTime { get; set; }

        /// <summary>
        /// Checks if the WoW aura is active.
        /// </summary>
        public bool IsActive => ((WowAuraFlag)Flags).HasFlag(WowAuraFlag.Active);

        /// <summary>
        /// Determines if the aura is harmful based on the value of the Flags property.
        /// </summary>
        public bool IsHarmful => ((WowAuraFlag)Flags).HasFlag(WowAuraFlag.Harmful);

        /// <summary>
        /// Determines if the aura is passive.
        /// </summary>
        public bool IsPassive => ((WowAuraFlag)Flags).HasFlag(WowAuraFlag.Passive);

        /// <summary>
        /// Returns a string representation of the object. The string includes the SpellId, Level, StackCount, Creator,
        /// IsHarmful and IsPassive properties.
        /// </summary>
        public override string ToString()
        {
            return $"{SpellId} (lvl. {Level}) x{StackCount} [CG: {Creator}], Harmful: {IsHarmful}, Passive: {IsPassive}";
        }
    }
}