using AmeisenBotX.Wow.Combatlog.Enums;

namespace AmeisenBotX.Wow335a.Combatlog.Enums
{
    public class CombatlogFields335a : ICombatlogFields
    {
        /// <summary>
        /// Gets the unique identifier for the destination. The value is set to 5 by default.
        /// </summary>
        public int DestinationGuid { get; } = 5;

        /// <summary>
        /// Gets the numeric value representing the destination name.
        /// The value is set to 6.
        /// </summary>
        public int DestinationName { get; } = 6;

        /// <summary>
        /// Gets the flags.
        /// </summary>
        public int Flags { get; } = 4;

        /// <summary>
        /// Gets the value of the Source property as an integer. The value is always 2.
        /// </summary>
        public int Source { get; } = 2;

        /// <summary>
        /// Gets the source name.
        /// </summary>
        public int SourceName { get; } = 3;

        /// <summary>
        /// Gets the amount of spells, which is set to 11 and can only be accessed
        /// but not modified.
        /// </summary>
        public int SpellAmount { get; } = 11;

        /// <summary>
        /// Gets the amount of spells over.
        /// </summary>
        public int SpellAmountOver { get; } = 12;

        /// <summary>
        /// Gets the spell ID.
        /// </summary>
        public int SpellSpellId { get; } = 8;

        /// <summary>
        /// Gets the amount of damage inflicted when swinging.
        /// </summary>
        public int SwingDamageAmount { get; } = 8;

        /// <summary>
        /// Gets the target flags.
        /// </summary>
        public int TargetFlags { get; } = 7;

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