using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

/// <summary>
/// Represents a namespace that contains classes related to handling the behavior of Kolkar Leaders in the game.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.OutpostStonetalon
{
    /// <summary>
    /// Represents a class that handles the behavior of Kolkar Leaders in the game.
    /// </summary>
    internal class QKolkarLeaders : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QKolkarLeaders class.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public QKolkarLeaders(AmeisenBotInterfaces bot)
                    : base(bot, 850, "Kolkar Leaders", 11, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3389 }), new Vector3(-307.14f, -1971.95f, 96.48f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3389 }), new Vector3(-307.14f, -1971.95f, 96.48f)),
                        new List<IQuestObjective>()
                        {
                    new QuestObjectiveChain(new List<IQuestObjective>()
                    {
                        new KillAndLootQuestObjective(bot, new List<int> { 3394 }, 1, 5022, new List<List<Vector3>> {
                            new()
                            {
                                new Vector3(23.49f, -1714.62f, 101.47f),
                            },
                        }),
                    })
                        })
        { }
    }
}