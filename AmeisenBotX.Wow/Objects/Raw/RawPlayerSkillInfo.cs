using System.Runtime.InteropServices;

/// <summary>
/// Represents the raw information of a player skill.
/// </summary>
namespace AmeisenBotX.Wow.Objects.Raw
{
    /// <summary>
    /// Represents the raw information of a player skill.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawPlayerSkillInfo
    {
        /// <summary>
        /// Gets or sets the Id property.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets the bonus, represented as an unsigned short integer.
        /// </summary>
        public ushort Bonus { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for an unsigned short integer.
        /// </summary>
        public ushort MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the modifier.
        /// </summary>
        public short Modifier { get; set; }

        /// <summary>
        /// Gets or sets the skill step.
        /// </summary>
        public ushort SkillStep { get; set; }

        /// <summary>
        /// Gets or sets the value of the public ushort property.
        /// </summary>
        public ushort Value { get; set; }
    }
}