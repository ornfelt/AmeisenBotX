using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

/// <summary>
/// The AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.Crossroads namespace contains classes related to quests in The Barrens Crossroads area.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.Crossroads
{
    /// <summary>
    /// The QLetterToJinZil class represents a quest called "Letter to Jin'Zil".
    /// It is initialized with a bot and has an ID of 1060.
    /// The quest requires a minimum level of 15 and a maximum level of 1.
    /// The starting point is determined by finding the closest quest giver with the NPC ID 3449 based on the bot's position.
    /// </summary>
    internal class QLetterToJinZil : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QLetterToJinZil class with the given bot.
        /// The quest has an ID of 1060 and a name of "Letter to Jin'Zil".
        /// It requires a minimum level of 15 and a maximum level of 1.
        /// The starting point is determined by finding the closest quest giver with the NPC ID 3449,
        /// which is obtained by using the bot's position. The starting position is set to (-474.89f, -2607.74f, 127.89f).
        /// The ending point is determined by finding the closest quest giver with the NPC ID 3995,
        /// which is obtained by using the bot's position. The ending position is set to (-272.48f, -394.08f, 17.21f).
        /// There are no additional parameters for this quest.
        /// </summary>
        public QLetterToJinZil(AmeisenBotInterfaces bot)
                    : base(bot, 1060, "Letter to Jin'Zil", 15, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3449 }), new Vector3(-474.89f, -2607.74f, 127.89f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3995 }), new Vector3(-272.48f, -394.08f, 17.21f)),
                        null)
        { }
    }
}