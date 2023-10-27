using AmeisenBotX.Wow.Combatlog.Enums;

namespace AmeisenBotX.Wow548.Combatlog.Enums
{
    /// <summary>
    /// Represents a set of combat log fields.
    /// </summary>
    public class CombatlogFields548 : ICombatlogFields
    {
        /// <summary>
        /// Gets the unique identifier for the destination, which is set to 7.
        /// </summary>
        public int DestinationGuid { get; } = 7;

        /// <summary>
        /// Gets the destination name.
        /// </summary>
        public int DestinationName { get; } = 8;

        /// <summary>
        /// Gets the flags value.
        /// </summary>
        public int Flags { get; } = 5;

        /// <summary>
        /// Gets the value of the Source property.
        /// </summary>
        public int Source { get; } = 3;

        /// <summary>
        /// Gets the value of the SourceName property.
        /// </summary>
        public int SourceName { get; } = 4;

        /// <summary>
        /// Gets the amount of spells.
        /// </summary>
        public int SpellAmount { get; } = 14;

        /// <summary>
        /// Gets the amount of spells a character can cast over a certain threshold.
        /// </summary>
        public int SpellAmountOver { get; } = 15;

        /// <summary>
        /// Gets the spell ID.
        /// </summary>
        public int SpellSpellId { get; } = 11;

        /// <summary>
        /// Gets the amount of damage done by a swing.
        /// </summary>
        public int SwingDamageAmount { get; } = 14;

        /// <summary>
        /// Gets the target flags value.
        /// </summary>
        public int TargetFlags { get; } = 9;

        /// <summary>
        /// Gets the timestamp value.
        /// </summary>
        public int Timestamp { get; } = 0;

        /// <summary>
        /// Gets the type value.
        /// </summary>
        public int Type { get; } = 1;
    }
}