using System.Text.Json.Serialization;

namespace AmeisenBotX.RconClient.Messages
{
    /// <summary>
    /// Represents a data message containing information about the number of free bag slots and combat class.
    /// </summary>
    public class DataMessage
    {
        /// <summary>
        /// Gets or sets the number of free bag slots available for items.
        /// </summary>
        [JsonPropertyName("bagslotsfree")]
        public int BagSlotsFree { get; set; }

        /// <summary>
        /// Gets or sets the combat class.
        /// </summary>
        /// <value>The combat class.</value>
        [JsonPropertyName("combatclass")]
        public string CombatClass { get; set; }

        /// <summary>
        /// Gets or sets the current profile.
        /// </summary>
        [JsonPropertyName("currentprofile")]
        public string CurrentProfile { get; set; }

        /// <summary>
        /// Gets or sets the energy property.
        /// </summary>
        [JsonPropertyName("energy")]
        public int Energy { get; set; }

        /// <summary>
        /// Gets or sets the experience value.
        /// </summary>
        /// <value>
        /// The experience value.
        /// </value>
        [JsonPropertyName("exp")]
        public int Exp { get; set; }

        /// <summary>
        /// Gets or sets the Guid of the object.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the health of the object.
        /// </summary>
        [JsonPropertyName("health")]
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets the item level.
        /// </summary>
        [JsonPropertyName("itemlevel")]
        public int ItemLevel { get; set; }

        /// <summary>
        /// Gets or sets the level of the object.
        /// </summary>
        [JsonPropertyName("level")]
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the map name.
        /// </summary>
        [JsonPropertyName("mapname")]
        public string MapName { get; set; }

        /// <summary>
        /// Gets or sets the maximum energy property.
        /// </summary>
        [JsonPropertyName("maxenergy")]
        public int MaxEnergy { get; set; }

        /// <summary>
        /// Gets or sets the maximum experience.
        /// </summary>
        [JsonPropertyName("maxexp")]
        public int MaxExp { get; set; }

        /// <summary>
        /// Gets or sets the maximum health.
        /// </summary>
        /// <value>
        /// The maximum health.
        /// </value>
        [JsonPropertyName("maxhealth")]
        public int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the value of the money property.
        /// </summary>
        /// <remarks>
        /// This property is decorated with the JsonPropertyName attribute, specifying that the property should be mapped to the "money" field when serializing or deserializing JSON data.
        /// </remarks>
        [JsonPropertyName("money")]
        public int Money { get; set; }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        [JsonPropertyName("posx")]
        public float PosX { get; set; }

        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>
        /// The Y position.
        /// </value>
        [JsonPropertyName("posy")]
        public float PosY { get; set; }

        /// <summary>
        /// Gets or sets the value of the PosZ property.
        /// </summary>
        /// <remarks>
        /// This property represents the positional value along the Z-axis.
        /// </remarks>
        [JsonPropertyName("posz")]
        public float PosZ { get; set; }

        /// <summary>
        /// Gets or sets the state of the object.
        /// </summary>
        /// <value>The state.</value>
        [JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the name of the subzone.
        /// </summary>
        [JsonPropertyName("subzonename")]
        public string SubZoneName { get; set; }

        /// <summary>
        /// Gets or sets the zone name.
        /// </summary>
        /// <remarks>
        /// The name of the zone as specified by the JsonPropertyName attribute.
        /// </remarks>
        [JsonPropertyName("zonename")]
        public string ZoneName { get; set; }
    }
}