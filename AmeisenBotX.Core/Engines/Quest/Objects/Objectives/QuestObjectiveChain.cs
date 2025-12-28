using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    public class QuestObjectiveChain(List<IQuestObjective> questObjectives) : IQuestObjective
    {
        public bool Finished => Progress == 100.0;

        public double Progress
        {
            get
            {
                int count = QuestObjectives.Count;
                if (count == 0)
                {
                    return 100.0;
                }

                int completedIndex = AlreadyCompletedIndex;
                int completed = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i <= completedIndex || QuestObjectives[i].Finished)
                    {
                        completed++;
                    }
                }
                return (double)completed / count * 100.0;
            }
        }

        public List<IQuestObjective> QuestObjectives { get; } = questObjectives;

        private int AlreadyCompletedIndex
        {
            get
            {
                for (int i = QuestObjectives.Count - 1; i >= 0; i--)
                {
                    if (QuestObjectives[i].Finished)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public void Execute()
        {
            int completedIndex = AlreadyCompletedIndex;
            for (int i = 0; i < QuestObjectives.Count; i++)
            {
                if (i > completedIndex && !QuestObjectives[i].Finished)
                {
                    QuestObjectives[i].Execute();
                    return;
                }
            }
        }
    }
}
