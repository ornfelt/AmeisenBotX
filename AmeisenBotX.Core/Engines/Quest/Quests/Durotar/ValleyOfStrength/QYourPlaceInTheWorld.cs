using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

/// <summary>
/// Represents a namespace for quests related to the Valley of Strength in Durotar.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Quests.Durotar.ValleyOfStrength
{
    /// <summary>
    /// Represents a quest that helps you discover your place in the world.
    /// </summary>
    internal class QYourPlaceInTheWorld : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QYourPlaceInTheWorld class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object to use for communication.</param>
        public QYourPlaceInTheWorld(AmeisenBotInterfaces bot)
                    : base(bot, 4641, "Your Place In The World", 1, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 10176 }), new Vector3(-610.07f, -4253.52f, 39.04f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3143 }), new Vector3(-600.13f, -4186.19f, 41.27f)),
                        null)
        { }
    }
}