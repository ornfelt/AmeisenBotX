using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Autopilot.Quest;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Autopilot.Services
{
    public class QuestBatch
    {
        public List<ParsedQuestObjective> Objectives { get; set; } = new List<ParsedQuestObjective>();
        public Vector3 Center { get; set; }
        public double Score { get; set; }
    }

    public class QuestBatcher
    {
        public List<QuestBatch> BatchObjectives(List<ParsedQuestObjective> objectives, float clusterRadius = 200f)
        {
            var batches = new List<QuestBatch>();
            var remaining = objectives.Where(o => o.Location != Vector3.Zero).ToList();

            while (remaining.Any())
            {
                var seed = remaining.First();
                remaining.RemoveAt(0);

                var batch = new QuestBatch();
                batch.Objectives.Add(seed);
                
                var cluster = remaining.Where(o => o.Location.GetDistance(seed.Location) <= clusterRadius).ToList();
                foreach (var item in cluster)
                {
                    batch.Objectives.Add(item);
                    remaining.Remove(item);
                }

                // Average location for the batch center
                float avgX = batch.Objectives.Average(o => o.Location.X);
                float avgY = batch.Objectives.Average(o => o.Location.Y);
                float avgZ = batch.Objectives.Average(o => o.Location.Z);
                batch.Center = new Vector3(avgX, avgY, avgZ);

                batches.Add(batch);
            }

            return batches;
        }

        public void CalculateBatchScores(List<QuestBatch> batches, IWowPlayer player, List<ParsedQuest> activeQuests)
        {
            if (player == null) return;

            foreach (var batch in batches)
            {
                double score = 0;
                
                // Base Score from Objective Types
                // TalkTo/Event super high priority
                if (batch.Objectives.Any(o => o.Type == QuestObjectiveType.TalkTo || o.Type == QuestObjectiveType.Event))
                {
                    score += 5000;
                }
                else
                {
                    // Combat/Collect base
                    score += 1000;
                }

                // Level Bonus: Prefer green/easy quests
                // Estimate quest level from first objective's quest via activeQuests (requires mapping)
                // For simplicity, we assume lower level quests are prioritized by the user logic requested
                // We'll try to find the Quest Level.
                var firstObj = batch.Objectives.FirstOrDefault();
                if (firstObj != null)
                {
                    // Find associated quest
                    var quest = activeQuests.FirstOrDefault(q => q.Objectives.Any(o => o.OriginalText == firstObj.OriginalText)); // Heuristic
                    if (quest != null && quest.Level > 0)
                    {
                        int levelDiff = player.Level - quest.Level;
                        // Higher diff (lower quest level) = More bonus. Green/Grey quests are fast.
                        score += levelDiff * 20; 
                    }
                }

                // Count Bonus: Fewer items remaining = Higher Score
                int totalRemaining = batch.Objectives.Sum(o => o.RequiredCount - o.CurrentCount);
                if (totalRemaining > 0)
                {
                    // e.g. 1 item left vs 10 items left. 
                    // 1 item = -10 penalty. 10 items = -100 penalty.
                    score -= totalRemaining * 10;
                }

                // Distance Penalty
                float dist = batch.Center.GetDistance(player.Position);
                // 100 yards = -10 score. 1000 yards = -100 score.
                score -= dist / 10.0;

                batch.Score = score;
            }
        }
    }
}
