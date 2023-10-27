using AmeisenBotX.Wow.Cache;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects
{
    /// <summary>
    /// Initializes a new instance of the KeepBestActiveAuraJob class.
    /// </summary>
    /// <param name="db">The IAmeisenBotDb object.</param>
    /// <param name="actions">The collection of actions as a tuple of strings and functions.</param>
    public class KeepBestActiveAuraJob : IAuraJob
    {
        /// <summary>
        /// Initializes a new instance of the KeepBestActiveAuraJob class.
        /// </summary>
        /// <param name="db">The IAmeisenBotDb object.</param>
        /// <param name="actions">The collection of actions as a tuple of strings and functions.</param>
        public KeepBestActiveAuraJob(IAmeisenBotDb db, IEnumerable<(string, Func<bool>)> actions)
        {
            Db = db;
            Actions = actions;
        }

        /// <summary>
        /// Gets or sets the collection of actions.
        /// Each action is represented by a tuple containing a string and a function that returns a boolean value.
        /// </summary>
        public IEnumerable<(string, Func<bool>)> Actions { get; set; }

        /// <summary>
        /// Gets or sets the IAmeisenBotDb instance used for database operations.
        /// </summary>
        private IAmeisenBotDb Db { get; }

        /// <summary>
        /// Executes a sequence of actions based on the provided auras.
        /// Checks if any aura's spell name matches the name in the Actions dictionary.
        /// If a match is found, returns false.
        /// If there is no match, executes the action function and returns true if it succeeds.
        /// Returns false if no matches are found and no action succeeds.
        /// </summary>
        /// <param name="auras">The collection of auras to check.</param>
        /// <returns>True if an action succeeds, false otherwise.</returns>
        public bool Run(IEnumerable<IWowAura> auras)
        {
            foreach ((string name, Func<bool> actionFunc) in Actions)
            {
                if (auras.Any(e => Db.GetSpellName(e.SpellId).Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
                else if (actionFunc())
                {
                    return true;
                }
            }

            return false;
        }
    }
}