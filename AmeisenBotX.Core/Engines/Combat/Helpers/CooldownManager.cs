using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using System;
using System.Collections.Generic;

/// <summary>
/// The AmeisenBotX.Core.Engines.Combat.Helpers namespace contains helper classes and methods for managing combat in the AmeisenBotX game.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CooldownManager"/> class with the provided collection of spells.
    /// The cooldowns dictionary is created and populated with spell names as keys and their corresponding current time in UTC as values.
    /// </summary>
    public class CooldownManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownManager"/> class with the provided collection of spells.
        /// The cooldowns dictionary is created and populated with spell names as keys and their corresponding current time in UTC as values.
        /// </summary>
        /// <param name="spells">The collection of spells to be used for initializing the cooldowns.</param>
        public CooldownManager(IEnumerable<Spell> spells)
        {
            Cooldowns = new();

            if (spells != null)
            {
                foreach (Spell spell in spells)
                {
                    if (!Cooldowns.ContainsKey(spell.Name))
                    {
                        Cooldowns.Add(spell.Name, DateTime.UtcNow);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of cooldowns for each string key.
        /// The keys are strings and the values are DateTimes representing the expiration time of the cooldown.
        /// </summary>
        /// <returns>A Dictionary of string keys and DateTime values.</returns>
        public Dictionary<string, DateTime> Cooldowns { get; }

        /// <summary>
        /// Retrieves the remaining cooldown time in milliseconds for a given spell.
        /// </summary>
        /// <param name="spellname">The name of the spell.</param>
        /// <returns>The remaining cooldown time in milliseconds for the given spell. If the spell name is null, empty, or whitespace, 0 is returned.</returns>
        public int GetSpellCooldown(string spellname)
        {
            if (string.IsNullOrWhiteSpace(spellname))
            {
                return 0;
            }

            spellname = spellname.ToUpperInvariant();

            if (Cooldowns.ContainsKey(spellname))
            {
                return (int)(Cooldowns[spellname] - DateTime.UtcNow).TotalMilliseconds;
            }

            return 0;
        }

        /// <summary>
        /// Checks if a spell is currently on cooldown.
        /// </summary>
        /// <param name="spellname">The name of the spell.</param>
        /// <returns>True if the spell is on cooldown, false otherwise.</returns>
        public bool IsSpellOnCooldown(string spellname)
        {
            if (string.IsNullOrWhiteSpace(spellname))
            {
                return false;
            }

            if (Cooldowns.TryGetValue(spellname.ToUpperInvariant(), out DateTime dateTime))
            {
                return dateTime > DateTime.UtcNow;
            }

            return false;
        }

        /// <summary>
        /// Sets the cooldown for a spell with the specified name.
        /// </summary>
        /// <param name="spellname">The name of the spell.</param>
        /// <param name="cooldownLeftMs">The remaining cooldown time in milliseconds.</param>
        /// <returns>True if the cooldown was successfully set, otherwise false.</returns>
        public bool SetSpellCooldown(string spellname, int cooldownLeftMs)
        {
            if (string.IsNullOrWhiteSpace(spellname))
            {
                return false;
            }

            spellname = spellname.ToUpperInvariant();

            if (!Cooldowns.ContainsKey(spellname))
            {
                Cooldowns.Add(spellname, DateTime.UtcNow + TimeSpan.FromMilliseconds(cooldownLeftMs));
            }
            else
            {
                Cooldowns[spellname] = DateTime.UtcNow + TimeSpan.FromMilliseconds(cooldownLeftMs);
            }

            return true;
        }
    }
}