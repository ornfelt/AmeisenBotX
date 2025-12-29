using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Core.Logic.Harvest.Modules
{
    /// <summary>
    /// Mining harvest module - checks skills dynamically each tick.
    /// Always loads, but CanHarvest returns false if no mining skill.
    /// </summary>
    public class MiningHarvestModule : IHarvestModule
    {
        private readonly AmeisenBotInterfaces Bot;

        public string Name => "Mining";

        public MiningHarvestModule(AmeisenBotInterfaces bot)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        /// <summary>
        /// Always load - we check skills dynamically in CanHarvest.
        /// Skills may not be available at startup.
        /// </summary>
        public bool ShouldLoad(AmeisenBotInterfaces bot) => true;

        /// <summary>
        /// Get current mining skill (checked each call, not cached).
        /// </summary>
        private int GetMiningSkill()
        {
            if (Bot?.Character?.Skills == null)
            {
                return 0;
            }
            // Check both Mining and Smelting (sometimes reported as the skill line)
            return Bot.Character.Skills.TryGetValue("Mining", out (int val, int max) skill)
                ? skill.val
                : Bot.Character.Skills.TryGetValue("Smelting", out (int val, int max) smelting) ? smelting.val : 0;
        }

        /// <summary>
        /// Fast type check - is this an ore node?
        /// </summary>
        public bool Matches(IWowGameobject gobject)
        {
            return gobject != null && WowHarvestHelper.IsOre(gobject.DisplayId);
        }

        /// <summary>
        /// Skill check - do we have the mining skill to harvest this ore?
        /// NOTE: IsUsable checked globally, Matches() already passed.
        /// </summary>
        public bool CanHarvest(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return false;
            }

            int miningSkill = GetMiningSkill();
            if (miningSkill <= 0)
            {
                return false;
            }

            WowOreId oreId = (WowOreId)gobject.DisplayId;
            WowMapId mapId = Bot?.Objects?.MapId ?? WowMapId.EasternKingdoms;

            return WowHarvestHelper.CanHarvestOre(oreId, miningSkill, mapId);
        }

        public int GetPriority(IWowGameobject gobject)
        {
            if (gobject == null || !WowHarvestHelper.IsOre(gobject.DisplayId))
            {
                return 0;
            }

            WowOreId oreId = (WowOreId)gobject.DisplayId;
            WowMapId mapId = Bot?.Objects?.MapId ?? WowMapId.EasternKingdoms;

            // Handle Titanium/Adamantite using map detection
            return oreId == WowOreId.Titanium
                ? WowHarvestHelper.IsNorthrendMap(mapId) ? 90 : 65
                : oreId switch
                {
                    // Endgame ores (WotLK)
                    WowOreId.Saronite => 70,
                    WowOreId.Cobalt => 60,

                    // TBC ores
                    WowOreId.Khorium => 80,
                    WowOreId.FelIron => 55,
                    WowOreId.Nethercite => 45,

                    // Classic high-value ores
                    WowOreId.RichThorium => 75,
                    WowOreId.SmallThorium => 60,
                    WowOreId.DarkIron => 78,
                    WowOreId.Mithril => 50,
                    WowOreId.Gold => 70,
                    WowOreId.Iron => 40,
                    WowOreId.Silver => 55,
                    WowOreId.Tin => 30,
                    WowOreId.Copper => 20,

                    // Specialty ores
                    WowOreId.Incendicite => 35,
                    WowOreId.LesserBloodstone => 30,

                    _ => 50
                };
        }
    }
}

