using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Aura
{
    /// <summary>
    /// Initializes a new instance of the GroupAuraManager class.
    /// </summary>
    public class GroupAuraManager
    {
        /// <summary>
        /// Initializes a new instance of the GroupAuraManager class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance to be assigned to the Bot property.</param>
        public GroupAuraManager(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            SpellsToKeepActiveOnParty = new();
            RemoveBadAurasSpells = new();
            LastBuffed = new();
        }

        /// <summary>
        /// Delegate to cast a spell on a unit.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="guid">The unique identifier of the target unit.</param>
        /// <returns>Returns true if the spell was successfully cast; otherwise, false.</returns>
        public delegate bool CastSpellOnUnit(string spellName, ulong guid);

        /// <summary>
        /// Gets or sets the list of spells used to remove bad auras from the unit.
        /// </summary>
        public List<((string, WowDispelType), CastSpellOnUnit)> RemoveBadAurasSpells { get; private set; }

        /// <summary>
        /// Gets or sets the list of spells to keep active on the party.
        /// </summary>
        public List<(string, CastSpellOnUnit)> SpellsToKeepActiveOnParty { get; private set; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces that represents the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the dictionary of last buffed events, where the keys are of type ulong and represent unique identifiers,
        /// and the values are of type TimegatedEvent and represent the last buffed events.
        /// </summary>
        private Dictionary<ulong, TimegatedEvent> LastBuffed { get; }

        /// <summary>
        /// Executes actions related to maintaining active spells on party members.
        /// </summary>
        public bool Tick()
        {
            if (SpellsToKeepActiveOnParty?.Count > 0)
            {
                foreach (IWowUnit wowUnit in Bot.Objects.Partymembers.Where(e => e.Guid != Bot.Wow.PlayerGuid && !e.IsDead))
                {
                    foreach ((string, CastSpellOnUnit) auraCombo in SpellsToKeepActiveOnParty)
                    {
                        if (!wowUnit.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == auraCombo.Item1))
                        {
                            if (!LastBuffed.ContainsKey(wowUnit.Guid))
                            {
                                LastBuffed.Add(wowUnit.Guid, new(TimeSpan.FromSeconds(30)));
                            }
                            else if (LastBuffed[wowUnit.Guid].Run())
                            {
                                return auraCombo.Item2.Invoke(auraCombo.Item1, wowUnit.Guid);
                            }
                        }
                    }
                }
            }

            // TODO: recognize bad spells and dispell them if (RemoveBadAurasSpells?.Count > 0) {
            // foreach (IWowUnit wowUnit in Bot.ObjectManager.Partymembers) { foreach (WowAura
            // wowAura in wowUnit.Auras.Where(e => e.IsHarmful)) { foreach (((string, DispelType),
            // CastSpellOnUnit) dispelCombo in RemoveBadAurasSpells) {
            //
            // } } } }

            return false;
        }
    }
}