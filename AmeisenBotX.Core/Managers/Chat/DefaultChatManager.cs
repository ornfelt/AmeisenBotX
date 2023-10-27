using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace AmeisenBotX.Core.Managers.Chat
{
    public class DefaultChatManager : IChatManager
    {
        /// <summary>
        /// Creates a new instance of the DefaultChatManager class.
        /// </summary>
        /// <param name="config">The AmeisenBotConfig object containing the configuration settings.</param>
        /// <param name="dataPath">The path to the data.</param>
        public DefaultChatManager(AmeisenBotConfig config, string dataPath)
        {
            Config = config;
            DataPath = dataPath;
            ChatMessages = new List<WowChatMessage>();
        }

        ///<summary>
        /// Event triggered when a new chat message is received.
        ///</summary>
        public event Action<WowChatMessage> OnNewChatMessage;

        /// <summary>
        /// Gets the list of WowChatMessage objects representing chat messages.
        /// </summary>
        public List<WowChatMessage> ChatMessages { get; }

        /// <summary>
        /// Gets the private AmeisenBotConfig property.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets the data path.
        /// </summary>
        private string DataPath { get; }

        /// <summary>
        /// Returns the file path for the specific chat protocol based on the given type.
        /// The file path includes the DataPath and the current date in the format of 'dd-M-yyyy'.
        /// </summary>
        /// <param name="type">The type of chat protocol.</param>
        /// <returns>The file path for the chat protocol.</returns>
        public string ProtocolName(string type)
        {
            return $"{DataPath}\\\\chatprotocols\\\\chat-{type}-{DateTime.Now:dd-M-yyyy}.txt";
        }

        /// <summary>
        /// Tries to parse a chat message and adds it to the list of chat messages. 
        /// If the number of arguments is less than 6, returns false indicating parsing failure.
        /// If Config.ChatProtocols is enabled, the parsed chat message is written to a protocol-specific file.
        /// The OnNewChatMessage event is invoked with the parsed chat message.
        /// Returns true if the chat message was successfully parsed and added to the list.
        /// </summary>
        /// <param name="type">The type of chat message (WowChat).</param>
        /// <param name="timestamp">The timestamp of the chat message.</param>
        /// <param name="args">The list of string arguments for the chat message.</param>
        /// <returns>True if the chat message was successfully parsed and added to the list, otherwise false.</returns>
        public bool TryParseMessage(WowChat type, long timestamp, List<string> args)
        {
            if (args.Count < 6)
            {
                return false;
            }

            WowChatMessage chatMessage = new(type, timestamp, args);
            ChatMessages.Add(chatMessage);

            if (Config.ChatProtocols)
            {
                try
                {
                    string typeName = chatMessage.Type switch
                    {
                        WowChat.ADDON => "misc",
                        WowChat.CHANNEL => "channel",
                        WowChat.DND => "misc",
                        WowChat.FILTERED => "filtered",
                        WowChat.GUILD => "guild",
                        WowChat.GUILD_ACHIEVEMENT => "guild",
                        WowChat.IGNORED => "misc",
                        WowChat.MONSTER_EMOTE => "npc",
                        WowChat.MONSTER_PARTY => "npc",
                        WowChat.MONSTER_SAY => "npc",
                        WowChat.MONSTER_WHISPER => "npc",
                        WowChat.MONSTER_YELL => "npc",
                        WowChat.RAID_BOSS_EMOTE => "npc",
                        WowChat.RAID_BOSS_WHISPER => "npc",
                        WowChat.SYSTEM => "system",
                        _ => "normal",
                    };

                    string protocolName = ProtocolName(typeName);
                    string dirName = Path.GetDirectoryName(protocolName);
                    IOUtils.CreateDirectoryIfNotExists(dirName);
                    File.AppendAllText(protocolName, $"{chatMessage}\n");
                }
                catch { }
            }

            OnNewChatMessage?.Invoke(chatMessage);

            return true;
        }
    }
}