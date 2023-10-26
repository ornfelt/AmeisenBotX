using System.Text.Json.Serialization;

namespace AmeisenBotX.RconClient.Messages
{
    public class ImageMessage
    {
        /// <summary>
        /// Gets or sets the Guid property.
        /// </summary>
        /// <remarks>
        /// The Guid is used to uniquely identify an object.
        /// </remarks>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the image name.
        /// </summary>
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }
}