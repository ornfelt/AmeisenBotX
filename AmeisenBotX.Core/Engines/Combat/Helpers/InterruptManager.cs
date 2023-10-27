using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InterruptManager"/> class.
    /// </summary>
    public class InterruptManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptManager"/> class.
        /// </summary>
        public InterruptManager()
        {
            InterruptSpells = new();
        }

        /// <summary>
        /// Represents a delegate that takes an IWowUnit parameter and returns a bool indicating whether a cast interrupt was successful.
        /// </summary>
        public delegate bool CastInterruptFunction(IWowUnit target);

        /// <summary>
        /// Gets or sets the sorted list of interrupt spells using their respective integers as keys.
        /// </summary>
        public SortedList<int, CastInterruptFunction> InterruptSpells { get; set; }

        /// <summary>
        /// Executes spell interruption logic on a collection of WoW units.
        /// </summary>
        /// <param name="units">The collection of WoW units to be checked for spell interruption.</param>
        /// <returns>Returns true if a spell was successfully interrupted, false otherwise.</returns>
        public bool Tick(IEnumerable<IWowUnit> units)
        {
            if (InterruptSpells != null && InterruptSpells.Count > 0 && units != null && units.Any())
            {
                IWowUnit selectedUnit = units.FirstOrDefault(e => e != null && e.IsCasting);

                if (selectedUnit != null)
                {
                    foreach (KeyValuePair<int, CastInterruptFunction> keyValuePair in InterruptSpells)
                    {
                        if (keyValuePair.Value(selectedUnit))
                        {
                            AmeisenLogger.I.Log("Interrupt", $"Interrupted \"{selectedUnit}\" using CastInterruptFunction: \"{keyValuePair.Key}\"");
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}