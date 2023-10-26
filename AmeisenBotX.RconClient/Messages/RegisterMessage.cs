using System.Text.Json.Serialization;

namespace AmeisenBotX.RconClient.Messages
{
    public class RegisterMessage
    {
        /// <summary>
        /// Gets or sets the value of the "Class" property.
        /// </summary>
        /// <value>
        /// The value of the "Class" property.
        /// </value>
        [JsonPropertyName("class")]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the gender of the individual.
        /// </summary>
        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the image property.
        /// </summary>
        /// <value>The image property.</value>
        [JsonPropertyName("image")]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the race of the entity.
        /// </summary>
        [JsonPropertyName("race")]
        public string Race { get; set; }

        /// <summary>
        /// Gets or sets the role of the item as indicated by the JSON property name "role".
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}