using System.Text.Json.Serialization;

namespace AmeisenBotX.Core.Managers.Character.Spells.Objects
{
    public class Spell
    {
        /// <summary>
        /// Gets or sets the cast time property.
        /// </summary>
        /// <value>The cast time.</value>
        [JsonPropertyName("castTime")]
        public int CastTime { get; set; }

        /// <summary>
        /// The costs of the item.
        /// </summary>
        [JsonPropertyName("costs")]
        public int Costs { get; set; }

        /// <summary>
        /// Gets or sets the maximum range.
        /// </summary>
        /// <value>
        /// The maximum range.
        /// </value>
        [JsonPropertyName("maxRange")]
        public int MaxRange { get; set; }

        /// <summary>
        /// Gets or sets the minimum range.
        /// </summary>
        /// <remarks>
        /// This property is used to specify the minimum range.
        /// </remarks>
        /// <value>The minimum range.</value>
        [JsonPropertyName("minRange")]
        public int MinRange { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        /// <summary>
        /// Gets or sets the spellbook Id.
        /// </summary>
        [JsonPropertyName("spellbookId")]
        public int SpellbookId { get; set; }

        /// <summary>
        /// Gets or sets the name of the spell book.
        /// </summary>
        [JsonPropertyName("spellBookName")]
        public string SpellbookName { get; set; }

        /// <summary>
        /// Tries to get the rank as an integer value.
        /// </summary>
        /// <param name="rank">The integer value of the rank if successful, 0 otherwise.</param>
        /// <returns>True if the conversion is successful, false otherwise.</returns>
        public bool TryGetRank(out int rank)
        {
            return int.TryParse(Rank, out rank);
        }
    }
}