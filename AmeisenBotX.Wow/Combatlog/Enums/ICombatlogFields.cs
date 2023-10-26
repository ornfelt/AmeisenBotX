namespace AmeisenBotX.Wow.Combatlog.Enums
{
    ///<summary>Interface for combat log fields.</summary>
    public interface ICombatlogFields
    {
        /// <summary>
        /// Gets the destination Guid.
        /// </summary>
        int DestinationGuid { get; }

        /// <summary>
        /// Gets the destination name.
        /// </summary>
        int DestinationName { get; }

        /// <summary>
        /// Gets the flags value.
        /// </summary>
        int Flags { get; }

        /// <summary>
        /// Gets the source value.
        /// </summary>
        int Source { get; }

        /// <summary>
        /// Gets the source name.
        /// </summary>
        int SourceName { get; }

        /// <summary>
        /// Gets the amount of spell.
        /// </summary>
        int SpellAmount { get; }

        /// <summary>
        /// Gets the amount of spell overages.
        /// </summary>
        int SpellAmountOver { get; }

        /// <summary>
        /// Gets the unique identifier of the spell.
        /// </summary>
        int SpellSpellId { get; }

        /// <summary>
        /// Gets the amount of swing damage.
        /// </summary>
        int SwingDamageAmount { get; }

        /// <summary>
        /// Gets or sets the target flags.
        /// </summary>
        int TargetFlags { get; }

        /// <summary>
        /// Gets the timestamp value.
        /// </summary>
        int Timestamp { get; }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        int Type { get; }
    }
}