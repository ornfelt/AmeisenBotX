using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Core
{
    public class QuestPriorityModule : ITargetSelectionModule
    {
        public string Name => "QuestPriority";

        // Always active, but only matters if we have active quests
        public bool IsActive(AmeisenBotInterfaces bot) => 
            bot.Autopilot?.QuestPulse?.ActiveObjectives?.Count > 0;

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            float weight = bot.GetQuestTargetWeight(target);
            if (weight > 0)
            {
                // Base priority 50 * weight + distance bonus
                // Weight 1.0 -> 50 + dist
                // Weight 1.5 (nearly done) -> 75 + dist
                float baseScore = 50f * weight;

                // Distance bonus (up to 10)
                float dist = target.DistanceTo(bot.Player);
                float distBonus = Math.Max(0f, 1f - (dist / 100f)) * 10f;
                
                // Cap total score to 85f to ensure we never override TankAssist (90) or SelfDefense (100)
                return Math.Min(baseScore + distBonus, 85f);
            }
            return 0f;
        }
    }
}
