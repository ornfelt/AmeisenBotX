using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a namespace that manages character spells.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Spells
{
    /// <summary>
    /// Represents a spell book that contains a collection of spells.
    /// </summary>
    public class SpellBook
    {
        /// <summary>
        /// Initializes a new instance of the SpellBook class.
        /// </summary>
        /// <param name="wowInterface">The WoW interface.</param>
        public SpellBook(IWowInterface wowInterface)
        {
            Wow = wowInterface;
        }

        /// <summary>
        /// Represents a delegate that is used to notify when a spell book is updated.
        /// </summary>
        public delegate void SpellBookUpdate();

        /// <summary>
        /// Event triggered when the Spell Book is updated.
        /// </summary>
        public event SpellBookUpdate OnSpellBookUpdate;

        /// <summary>
        /// Gets or sets the collection of Spells.
        /// </summary>
        /// <value>
        /// The collection of Spells.
        /// </value>
        public IEnumerable<Spell> Spells { get; private set; }

        /// <summary>
        /// Gets or sets the WOW interface.
        /// </summary>
        private IWowInterface Wow { get; }

        /// <summary>
        /// Retrieves a spell object by its name.
        /// </summary>
        /// <param name="spellname">The name of the spell to retrieve.</param>
        /// <returns>The spell object with the specified name.</returns>
        public Spell GetSpellByName(string spellname)
        {
            return Spells?.FirstOrDefault(e => string.Equals(e.Name, spellname, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether the specified spell is known.
        /// </summary>
        /// <param name="spellname">The name of the spell to check.</param>
        /// <returns>True if the spell is known, false otherwise.</returns>
        public bool IsSpellKnown(string spellname)
        {
            return Spells != null && Spells.Any(e => string.Equals(e.Name, spellname, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>

        /// Tries to get a spell by its name.

        /// </summary>

        /// <param name="spellname">The name of the spell to retrieve.</param>

        /// <param name="spell">The returned spell. If found, it is assigned to this parameter.</param>

        /// <returns>True if the spell was found, otherwise false.</returns>
        public bool TryGetSpellByName(string spellname, out Spell spell)
        {
            spell = GetSpellByName(spellname);
            return spell != null;
        }

        /// <summary>
        /// Updates the list of spells by retrieving the raw spells data from Wow API and then parses it into a list of Spell objects.
        /// The spells are sorted by name and rank in ascending order.
        /// After the update, the OnSpellBookUpdate event is invoked.
        /// If an exception occurs during the parsing process, an error message with the failed JSON and the exception details is logged.
        /// </summary>
        public void Update()
        {
            string rawSpells = Wow.GetSpells();

            try
            {
                Spells = JsonSerializer.Deserialize<List<Spell>>(rawSpells, new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString })
                    .OrderBy(e => e.Name)
                    .ThenByDescending(e => e.Rank);

                OnSpellBookUpdate?.Invoke();
            }
            catch (Exception e)
            {
                AmeisenLogger.I.Log("CharacterManager", $"Failed to parse Spells JSON:\n{rawSpells}\n{e}", LogLevel.Error);
            }
        }
    }
}