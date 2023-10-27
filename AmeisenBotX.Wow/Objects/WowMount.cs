using System.Text.Json.Serialization;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Gets or sets the spell ID.
    /// </summary>
    public class WowMount
    {
        /// <summary>
        /// Gets or sets the index value.
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the mount ID.
        /// </summary>
        [JsonPropertyName("mountId")]
        public int MountId { get; set; }

        /// <summary>
        /// Gets or sets the name property.
        /// </summary>
        /// <value>The name property.</value>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the spell ID.
        /// </summary>
        [JsonPropertyName("spellId")]
        public int SpellId { get; set; }
    }
}