using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.OutpostBridge
{
    /// <summary>
    /// Initializes a new instance of the QCrossroadsConscription class.
    /// </summary>
    internal class QCrossroadsConscription : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QCrossroadsConscription class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for the quest.</param>
        public QCrossroadsConscription(AmeisenBotInterfaces bot)
                    : base(bot, 842, "Crossroads Conscription", 10, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3337 }), new Vector3(303.43f, -3686.16f, 27.15f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3338 }), new Vector3(-482.48f, -2670.19f, 97.52f)),
                        null)
        { }
    }
}