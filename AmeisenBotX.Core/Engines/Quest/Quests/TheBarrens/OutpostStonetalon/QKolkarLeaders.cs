using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;

namespace AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.OutpostStonetalon
{
    internal class QKolkarLeaders(AmeisenBotInterfaces bot) : BotQuest(bot, 850, "Kolkar Leaders", 11, 1,
            () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, [3389]), new Vector3(-307.14f, -1971.95f, 96.48f)),
            () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, [3389]), new Vector3(-307.14f, -1971.95f, 96.48f)),
            [
                    new QuestObjectiveChain(
                    [
                        new KillAndLootQuestObjective(bot, [3394], 1, 5022, [
                            [
                                new Vector3(23.49f, -1714.62f, 101.47f),
                            ],
                        ]),
                    ])
                ])
    {
    }
}
