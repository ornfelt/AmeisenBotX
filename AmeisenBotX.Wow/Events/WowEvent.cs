using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Wow.Events
{
    public class WowEvent
    {
        /// <summary>
        /// Gets or sets the arguments for the code.
        /// </summary>
        /// <value>A list of strings representing the arguments.</value>
        [JsonPropertyName("args")]
        public List<string> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        [JsonPropertyName("event")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the object.
        /// </summary>
        [JsonPropertyName("time")]
        public long Timestamp { get; set; }
    }
}