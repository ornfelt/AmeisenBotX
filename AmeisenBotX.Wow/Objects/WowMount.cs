﻿using System.Text.Json.Serialization;

/// <summary>
/// Represents a namespace for World of Warcraft objects related to mounts.
/// </summary>
namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a World of Warcraft mount.
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