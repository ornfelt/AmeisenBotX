using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

/// <summary>
/// Namespace for quests related to the Valley of Strength in Durotar.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Quests.Durotar.ValleyOfStrength
{
    /// <summary>
    /// Initializes a new instance of the QSarkoth2 class.
    /// </summary>
    internal class QSarkoth2 : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QSarkoth2 class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public QSarkoth2(AmeisenBotInterfaces bot)
                    : base(bot, 804, "Sarkoth", 1, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3287 }), new Vector3(-397.76f, -4108.99f, 50.29f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3143 }), new Vector3(-600.13f, -4186.19f, 41.27f)),
                        null)
        { }
    }
}