using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Combatlog.Enums;
using AmeisenBotX.Wow.Combatlog.Objects;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AmeisenBotX.Wow.Combatlog
{
    /// <summary>
    /// Represents a default combat log parser for a specific type, implementing the ICombatlogParser interface.
    /// </summary>
    public class DefaultCombatlogParser<T> : ICombatlogParser
            where T : ICombatlogFields, new()
    {
        /// <summary>
        /// Event that is triggered when damage is inflicted.
        /// </summary>
        public event Action<ulong, ulong, int, int, int> OnDamage;

        /// <summary>
        /// Event triggered when a heal action occurs.
        /// The parameters represent the amount of health restored:
        /// - The first parameter represents the initial health value.
        /// - The second parameter represents the final health value.
        /// - The third, fourth, and fifth parameters represent additional information related to the heal.
        /// </summary>
        public event Action<ulong, ulong, int, int, int> OnHeal;

        /// <summary>
        /// Event that is triggered when a party kill occurs.
        /// The event provides two parameters, both of type ulong, representing the ID of the killer and the ID of the killed party.
        /// </summary>
        public event Action<ulong, ulong> OnPartyKill;

        /// <summary>
        /// Represents an event that is raised when a unit dies.
        /// </summary>
        public event Action<ulong> OnUnitDied;

        /// <summary>
        /// Gets or sets the combat log fields.
        /// </summary>
        protected T CombatlogFields { get; } = new T();

        /// <summary>
        /// Parses a combat log entry based on the provided timestamp and arguments.
        /// </summary>
        /// <param name="timestamp">The timestamp of the combat log entry.</param>
        /// <param name="args">The list of arguments for the combat log entry.</param>
        public void Parse(long timestamp, List<string> args)
        {
            if (BasicCombatlogEntry.TryParse(CombatlogFields, args, out BasicCombatlogEntry entry))
            {
                AmeisenLogger.I.Log("CombatLogParser", $"[{timestamp}] Parsing CombatLog: {JsonSerializer.Serialize(args)}", LogLevel.Verbose);

                switch (entry.Type)
                {
                    case CombatlogEntryType.PARTY:
                        switch (entry.Subtype)
                        {
                            case CombatlogEntrySubtype.KILL:
                                AmeisenLogger.I.Log("CombatLogParser", $"OnPartyKill({entry.SourceGuid}, {entry.DestinationGuid})");
                                OnPartyKill?.Invoke(entry.SourceGuid, entry.DestinationGuid);
                                break;
                        }
                        break;

                    case CombatlogEntryType.UNIT:
                        switch (entry.Subtype)
                        {
                            case CombatlogEntrySubtype.DIED:
                                AmeisenLogger.I.Log("CombatLogParser", $"OnUnitDied({entry.SourceGuid})");
                                OnUnitDied?.Invoke(entry.SourceGuid);
                                break;
                        }
                        break;

                    case CombatlogEntryType.SWING:
                        switch (entry.Subtype)
                        {
                            case CombatlogEntrySubtype.DAMAGE:
                                if (int.TryParse(entry.Args[CombatlogFields.SwingDamageAmount], out int damage))
                                {
                                    AmeisenLogger.I.Log("CombatLogParser", $"OnDamage({entry.SourceGuid}, {entry.DestinationGuid}, {entry.Args[CombatlogFields.SwingDamageAmount]})");
                                    OnDamage?.Invoke(entry.SourceGuid, entry.DestinationGuid, -1, damage, 0);
                                }
                                break;
                        }
                        break;

                    case CombatlogEntryType.SPELL:
                        switch (entry.Subtype)
                        {
                            case CombatlogEntrySubtype.DAMAGE:
                                if (int.TryParse(entry.Args[CombatlogFields.SpellAmount], out int spellAmount)
                                    && int.TryParse(entry.Args[CombatlogFields.SpellAmountOver], out int spellAmountOver)
                                    && int.TryParse(entry.Args[CombatlogFields.SpellSpellId], out int spellSpellId))
                                {
                                    AmeisenLogger.I.Log("CombatLogParser", $"OnDamage({entry.SourceGuid}, {entry.DestinationGuid}, {entry.Args[CombatlogFields.SpellSpellId]}, {entry.Args[CombatlogFields.SpellAmount]}, {entry.Args[CombatlogFields.SpellAmountOver]})");
                                    OnDamage?.Invoke(entry.SourceGuid, entry.DestinationGuid, spellSpellId, spellAmount, spellAmountOver);
                                }
                                break;

                            case CombatlogEntrySubtype.HEAL:
                                if (int.TryParse(entry.Args[CombatlogFields.SpellAmount], out int spellAmount2)
                                    && int.TryParse(entry.Args[CombatlogFields.SpellAmountOver], out int spellAmountOver2)
                                    && int.TryParse(entry.Args[CombatlogFields.SpellSpellId], out int spellSpellId2))
                                {
                                    AmeisenLogger.I.Log("CombatLogParser", $"OnHeal({entry.SourceGuid}, {entry.DestinationGuid}, {entry.Args[CombatlogFields.SpellSpellId]}, {entry.Args[CombatlogFields.SpellAmount]}, {entry.Args[CombatlogFields.SpellAmountOver]})");
                                    OnHeal?.Invoke(entry.SourceGuid, entry.DestinationGuid, spellSpellId2, spellAmount2, spellAmountOver2);
                                }
                                break;
                        }
                        break;
                }
            }
            else
            {
                AmeisenLogger.I.Log("CombatLogParser", $"Parsing failed: {JsonSerializer.Serialize(args)}", LogLevel.Warning);
            }
        }
    }
}