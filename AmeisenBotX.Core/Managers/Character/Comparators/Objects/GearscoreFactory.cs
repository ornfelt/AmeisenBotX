using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AmeisenBotX.Core.Managers.Character.Comparators.Objects
{
    public class GearscoreFactory
    {
        /// <summary>
        /// Initializes a new instance of the GearscoreFactory class with the specified stat multiplicators.
        /// </summary>
        /// <param name="statMultiplicators">A dictionary containing stat names as keys and their respective multiplicators as values.</param>
        public GearscoreFactory(Dictionary<string, double> statMultiplicators)
        {
            StatMultiplicators = statMultiplicators;
        }

        /// <summary>
        /// Gets the dictionary of string keys and double values representing the stat multiplicators.
        /// </summary>
        private Dictionary<string, double> StatMultiplicators { get; }

        /// <summary>
        /// Calculates the score based on the provided inventory item's stats and stat multiplicators.
        /// </summary>
        /// <param name="item">The inventory item to calculate the score for.</param>
        /// <returns>The calculated score.</returns>
        public double Calculate(IWowInventoryItem item)
        {
            double score = 0;

            for (int i = 0; i < StatMultiplicators.Count; ++i)
            {
                KeyValuePair<string, double> keyValuePair = StatMultiplicators.ElementAt(i);

                if (item.Stats.TryGetValue(keyValuePair.Key, out string stat))
                {
                    if ((stat.Contains('.') || stat.Contains(',')) && double.TryParse(stat, NumberStyles.Any, CultureInfo.InvariantCulture, out double statDoubleValue))
                    {
                        score += statDoubleValue * keyValuePair.Value;
                    }
                    else if (int.TryParse(stat, out int statIntValue))
                    {
                        score += statIntValue * keyValuePair.Value;
                    }
                }
            }

            return score;
        }
    }
}