using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
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

        /// <summary>
        /// Fast type check - is this a sparkling quest object?
        /// </summary>
        public bool Matches(IWowGameobject gobject)
        {
            return gobject != null && gobject.IsSparkling;
        }

        /// <summary>
        /// Check if we should interact with this sparkling object.
        /// Excludes profession nodes we can't harvest.
        /// NOTE: IsUsable checked globally, Matches() already passed.
        /// </summary>
        public bool CanHarvest(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return false;
            }

            // Check if this is a profession node
            bool isOre = WowHarvestHelper.IsOre(gobject.DisplayId);
            bool isHerb = WowHarvestHelper.IsHerb(gobject.DisplayId, gobject.EntryId);

            // Profession nodes (ore/herb) should be handled by specialized modules
            // (MiningHarvestModule, HerbalismHarvestModule) which have proper skill checks.
            // This module only handles non-profession sparkling objects (quest items, etc.)
            if (isOre || isHerb)
            {
                return false;
            }

            // Additional safety: Goober type objects are usually profession nodes
            // that might not be in the WowOreId/WowHerbId enums.
            // Only match explicit quest-related sparkling objects (chests, quest items).
            if (gobject.GameObjectType == WowGameObjectType.Goober)
            {
                return false;
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

