using System.Text.Json.Serialization;

namespace AmeisenBotX.RconClient.Messages
{
    public class KeepAliveMessage
    {
        /// <summary>
        /// Gets or sets the value of the "guid" property.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }
    }
}