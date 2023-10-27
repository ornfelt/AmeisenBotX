using System.Text.Json.Serialization;

namespace AmeisenBotX.RconClient.Messages
{
    /// <summary>
    /// Represents a message used to keep the connection alive.
    /// </summary>
    public class KeepAliveMessage
    {
        /// <summary>
        /// Gets or sets the value of the "guid" property.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }
    }
}