using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a namespace that contains classes and interfaces related to managing chat functionality in the game.
/// </summary>
namespace AmeisenBotX.Core.Managers.Chat
{
    /// <summary>
    /// Represents a chat manager interface that provides functionality for handling chat messages in the game.
    /// </summary>
    public interface IChatManager
    {
        /// <summary>
        /// Represents the event that is triggered when a new chat message is received.
        /// </summary>
        event Action<WowChatMessage> OnNewChatMessage;

        /// <summary>
        /// Gets the list of WowChatMessage objects representing the chat messages.
        /// </summary>
        List<WowChatMessage> ChatMessages { get; }

        /// <summary>
        /// Tries to parse a message with the specified parameters.
        /// </summary>
        /// <param name="type">The type of WowChat.</param>
        /// <param name="timestamp">The timestamp of the message.</param>
        /// <param name="args">The list of string arguments.</param>
        /// <returns>True if the message was successfully parsed, false otherwise.</returns>
        bool TryParseMessage(WowChat type, long timestamp, List<string> args);
    }
}