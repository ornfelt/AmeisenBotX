using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Quests.Grinder
{
    /// <summary>
    /// Creates a new instance of the QTheBarrensGrindToLevel14 class, which represents a quest for grinding to level 14 in The Barrens.
    /// </summary>
    internal class QTheBarrensGrindToLevel14 : GrindingBotQuest
    {
        /// <summary>
        /// Creates a new instance of the QTheBarrensGrindToLevel14 class, which represents a quest for grinding to level 14 in The Barrens.
        /// </summary>
        /// <param name="bot">An instance of the AmeisenBotInterfaces class used for interacting with the game bot.</param>
        public QTheBarrensGrindToLevel14(AmeisenBotInterfaces bot)
                    : base("TheBarrensGrindToLevel14",
                        new List<IQuestObjective>()
                        {
                    new QuestObjectiveChain(new List<IQuestObjective>()
                    {
                        new GrindingObjective(bot, 14, new List<List<Vector3>> {
                            new()
                            {
                                new Vector3(-43.21f, -2813.28f, 92.99f),
                                new Vector3(-80.01f, -2752.45f, 91.79f),
                                new Vector3(-218.70f, -2922.10f, 91.79f),
                                new Vector3(-229.86f, -3018.29f, 91.79f),
                                new Vector3(-224.10f, -3036.88f, 91.79f),
                                new Vector3(-165.21f, -3037.72f, 91.79f),
                                new Vector3(-126.96f, -3005.08f, 91.79f),
                                new Vector3(-48.97f, -2893.44f, 91.89f),
                            },
                        }),
                    })
                        })
        { }
    }
}