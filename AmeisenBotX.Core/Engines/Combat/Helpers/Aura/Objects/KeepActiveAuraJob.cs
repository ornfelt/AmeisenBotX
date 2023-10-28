using AmeisenBotX.Wow.Cache;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a namespace for helper classes related to combat auras.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects
{
    /// <summary>
    /// Represents a job that keeps an aura active.
    /// </summary>
    public class KeepActiveAuraJob : IAuraJob
    {
        /// <summary>
        /// Initializes a new instance of the KeepActiveAuraJob class.
        /// </summary>
        /// <param name="db">The IAmeisenBotDb object.</param>
        /// <param name="name">The name of the job.</param>
        /// <param name="action">The action to be performed by the job.</param>
        public KeepActiveAuraJob(IAmeisenBotDb db, string name, Func<bool> action)
        {
            Db = db;
            Name = name;
            Action = action;
        }

        /// <summary>
        /// Gets or sets the action to be performed which returns a boolean value.
        /// </summary>
        public Func<bool> Action { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the instance of the AmeisenBot database interface.
        /// </summary>
        private IAmeisenBotDb Db { get; }

        /// <summary>
        /// Runs a check on a collection of World of Warcraft auras and returns a boolean value.
        /// The method checks if the provided auras collection is not null, there are no auras with the same SpellId as the current instance's Name property,
        /// and calls the Action method. Returns false if any of the conditions are not met, otherwise returns true.
        /// </summary>
        /// <param name="auras">The collection of World of Warcraft auras to be checked.</param>
        /// <returns>True if the check passes, false otherwise.</returns>
        public bool Run(IEnumerable<IWowAura> auras)
        {
            return auras != null && !auras.Any(e => Db.GetSpellName(e.SpellId).Equals(Name, StringComparison.OrdinalIgnoreCase)) && Action();
        }
    }
}