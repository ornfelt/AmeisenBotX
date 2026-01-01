using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Core.Logic.Harvest.Modules
{
    /// <summary>
    /// Herbalism harvest module - checks skills dynamically each tick.
    /// Always loads, but CanHarvest returns false if no herbalism skill.
    /// </summary>
    public class HerbalismHarvestModule : IHarvestModule
    {
        private readonly AmeisenBotInterfaces Bot;

        public string Name => "Herbalism";

        public HerbalismHarvestModule(AmeisenBotInterfaces bot)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        /// <summary>
        /// Always load - we check skills dynamically in CanHarvest.
        /// </summary>
        public bool ShouldLoad(AmeisenBotInterfaces bot) => true;

        /// <summary>
        /// Get current herbalism skill (checked each call, not cached).
        /// </summary>
        private int GetHerbalismSkill()
        {
            return Bot?.Character?.Skills == null
                ? 0
                : Bot.Character.Skills.TryGetValue("Herbalism", out (int val, int max) skill) ? skill.val : 0;
        }

        /// <summary>
        /// Fast type check - is this a herb node?
        /// </summary>
        public bool Matches(IWowGameobject gobject)
        {
            return gobject != null && WowHarvestHelper.IsHerb(gobject.DisplayId, gobject.EntryId);
        }

        /// <summary>
        /// Skill check - do we have the herbalism skill to harvest this herb?
        /// NOTE: IsUsable checked globally, Matches() already passed.
        /// </summary>
        public bool CanHarvest(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return false;
            }

            int herbalismSkill = GetHerbalismSkill();
            return herbalismSkill > 0 && WowHarvestHelper.TryGetHerbId(gobject.DisplayId, gobject.EntryId, out WowHerbId herbId)
                && WowHarvestHelper.CanHarvestHerb(herbId, herbalismSkill);
        }

        public int GetPriority(IWowGameobject gobject)
        {
            return gobject == null
                ? 0
                : !WowHarvestHelper.TryGetHerbId(gobject.DisplayId, gobject.EntryId, out WowHerbId herbId)
                ? 0
                : herbId switch
                {
                    // Ultra-rare endgame herbs
                    WowHerbId.BlackLotus => 100,
                    WowHerbId.Lichbloom => 90,
                    WowHerbId.Icethorn => 85,

                    // High-value endgame herbs
                    WowHerbId.Icecap => 80,
                    WowHerbId.Plaguebloom => 75,
                    WowHerbId.MountainSilversage => 75,
                    WowHerbId.Dreamfoil => 70,
                    WowHerbId.GoldenSansam => 65,

                    // TBC high-value herbs
                    WowHerbId.ManaThistle => 85,
                    WowHerbId.NightmareVine => 75,
                    WowHerbId.Netherbloom => 70,
                    WowHerbId.FlameCap => 65,
                    WowHerbId.AncientLichen => 60,

                    // Northrend herbs
                    WowHerbId.AddersTongue => 70,
                    WowHerbId.TalandrasRose => 65,
                    WowHerbId.TigerLily => 60,
                    WowHerbId.Goldclover => 50,

                    // Classic mid-tier valuable herbs
                    WowHerbId.Gromsblood => 60,
                    WowHerbId.GhostMushroom => 55,
                    WowHerbId.Blindweed => 55,
                    WowHerbId.Sungrass => 50,
                    WowHerbId.ArthasTears => 50,
                    WowHerbId.PurpleLotus => 55,
                    WowHerbId.Firebloom => 50,
                    WowHerbId.Wintersbite => 45,
                    WowHerbId.KhadgarsWhisker => 45,
                    WowHerbId.Goldthorn => 40,
                    WowHerbId.Fadeleaf => 40,
                    WowHerbId.Liferoot => 40,

                    // TBC common herbs
                    WowHerbId.Terocone => 55,
                    WowHerbId.Ragveil => 50,
                    WowHerbId.DreamingGlory => 50,
                    WowHerbId.Felweed => 45,

                    // Classic low-tier herbs
                    WowHerbId.Kingsblood => 35,
                    WowHerbId.GraveMoss => 35,
                    WowHerbId.WildSteelbloom => 35,
                    WowHerbId.Bruiseweed => 30,
                    WowHerbId.Briarthorn => 25,
                    WowHerbId.Stranglekelp => 30,
                    WowHerbId.Mageroyal => 25,
                    WowHerbId.Earthroot => 20,
                    WowHerbId.Silverleaf => 15,
                    WowHerbId.Peacebloom => 15,

                    _ => 50
                };
        }
    }
}

