using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Harvest.Modules
{
    /// <summary>
    /// Quest object harvest module - handles sparkling quest-related objects.
    /// Prevents non-professionals from targeting profession nodes that sparkle.
    /// Always loads as quest objects are high priority.
    /// </summary>
    public class QuestObjectHarvestModule : IHarvestModule
    {
        private readonly AmeisenBotInterfaces Bot;

        public string Name => "QuestObjects";

        public QuestObjectHarvestModule(AmeisenBotInterfaces bot)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        public bool ShouldLoad(AmeisenBotInterfaces bot)
        {
            return bot != null;
        }

        public bool CanHarvest(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return false;
            }

            if (!gobject.IsSparkling)
            {
                return false;
            }

            // Exclude sparkles that are profession nodes we can't harvest
            if (Bot?.Character?.Skills != null)
            {
                int miningSkill = Bot.Character.Skills.TryGetValue("Mining", out (int val, int max) m) ? m.val : 0;
                int herbalismSkill = Bot.Character.Skills.TryGetValue("Herbalism", out (int val, int max) h) ? h.val : 0;

                // Check if it's a mining node we can't mine
                if (WowHarvestHelper.IsOre(gobject.DisplayId) && miningSkill < 1)
                {
                    return false;
                }

                // Check if it's a herb node we can't pick
                if (WowHarvestHelper.IsHerb(gobject.DisplayId, gobject.EntryId) && herbalismSkill < 1)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetPriority(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return 0;
            }

            // Highest priority - quest objectives are critical
            return 200;
        }
    }
}

