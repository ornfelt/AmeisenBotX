using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.Crossroads
{
    /// <summary>
    /// Initializes a new instance of the QSerenaBloodfeather class.
    /// </summary>
    internal class QSerenaBloodfeather : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QSerenaBloodfeather class.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public QSerenaBloodfeather(AmeisenBotInterfaces bot)
                    : base(bot, 876, "Serena Bloodfeather", 12, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3449 }), new Vector3(-474.89f, -2607.74f, 127.89f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3449 }), new Vector3(-474.89f, -2607.74f, 127.89f)),
                        new List<IQuestObjective>()
                        {
                    new QuestObjectiveChain(new List<IQuestObjective>()
                    {
                        new KillAndLootQuestObjective(bot, new List<int> { 3452 }, 1, 5067, new List<List<Vector3>> {
                            new()
                            {
                                new Vector3(790.37f, -1345.77f, 90.62f),
                            },
                        }),
                    })
                        })
        { }
    }
}