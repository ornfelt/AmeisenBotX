using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a chat message in the game "World of Warcraft".
    /// </summary>
    public record WowChatMessage
    {
        /// <summary>
        /// Initializes a new instance of the WowChatMessage class.
        /// </summary>
        /// <param name="type">The type of the chat message.</param>
        /// <param name="timestamp">The timestamp of the chat message.</param>
        /// <param name="args">The list of arguments for the chat message.</param>
        public WowChatMessage(WowChat type, long timestamp, List<string> args)
        {
            Type = type;
            Timestamp = timestamp;
            Author = args[1];
            Channel = args[3];
            Flags = args[5];
            Language = args[2];
            Message = args[0];
        }

        /// <summary>
        /// Gets or sets the author of the content.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the channel string.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp in long format.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the type of the WowChat.
        /// </summary>
        public WowChat Type { get; set; }

        /// <summary>
        /// Overrides the default ToString method to display the message along with the type, channel, flags, language, and author.
        /// </summary>
        public override string ToString()
        {
            return $"[{Type}]{(Channel.Length > 0 ? $"[{Channel}]" : "[]")}{(Flags.Length > 0 ? $"[{Flags}]" : "")}{(Language.Length > 0 ? $"[{Language}]" : "[]")} {Author}: {Message}";
        }
    }
}