using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Wow
{
    /// <summary>
    /// Static helper class for harvest-related WoW game knowledge.
    /// Contains skill requirements, map detection, and node identification.
    /// </summary>
    public static class WowHarvestHelper
    {
        #region Mining

        /// <summary>
        /// Get the required Mining skill level to harvest an ore node.
        /// </summary>
        /// <param name="oreId">The ore type.</param>
        /// <param name="mapId">Current map (used for Titanium/Adamantite disambiguation).</param>
        /// <returns>Required skill level, or int.MaxValue if unknown.</returns>
        public static int GetRequiredMiningSkill(WowOreId oreId, WowMapId mapId)
        {
            // Handle Titanium/Adamantite collision (both DisplayId 6798)
            if (oreId == WowOreId.Titanium)
            {
                return IsNorthrendMap(mapId) ? 450 : 325; // Titanium vs Adamantite
            }

            return oreId switch
            {
                WowOreId.Copper => 1,
                WowOreId.Tin => 65,
                WowOreId.Silver => 75,
                WowOreId.Incendicite => 75,
                WowOreId.Iron => 125,
                WowOreId.Gold => 155,
                WowOreId.Mithril => 175,
                WowOreId.DarkIron => 230,
                WowOreId.SmallThorium => 245,
                WowOreId.RichThorium => 275,
                WowOreId.FelIron => 300,
                WowOreId.Nethercite => 325,
                WowOreId.Cobalt => 350,
                WowOreId.Khorium => 375,
                WowOreId.Saronite => 400,
                WowOreId.LesserBloodstone => 75,
                _ => int.MaxValue
            };
        }

        /// <summary>
        /// Check if a displayId represents a known ore node.
        /// </summary>
        public static bool IsOre(int displayId)
        {
            return Enum.IsDefined(typeof(WowOreId), displayId);
        }

        /// <summary>
        /// Check if the player can harvest an ore node based on skill and map.
        /// </summary>
        public static bool CanHarvestOre(WowOreId oreId, int miningSkill, WowMapId mapId)
        {
            return miningSkill >= GetRequiredMiningSkill(oreId, mapId);
        }

        #endregion

        #region Herbalism

        /// <summary>
        /// Get the required Herbalism skill level to harvest a herb.
        /// </summary>
        public static int GetRequiredHerbalismSkill(WowHerbId herbId)
        {
            return herbId switch
            {
                // Skill 1 (always harvestable)
                WowHerbId.Peacebloom => 1,
                WowHerbId.Silverleaf => 1,

                // Low-level herbs
                WowHerbId.Earthroot => 15,
                WowHerbId.Mageroyal => 50,
                WowHerbId.Briarthorn => 70,
                WowHerbId.Stranglekelp => 85,
                WowHerbId.Bruiseweed => 85,

                // Mid-level herbs
                WowHerbId.WildSteelbloom => 115,
                WowHerbId.GraveMoss => 120,
                WowHerbId.Kingsblood => 125,
                WowHerbId.Liferoot => 150,
                WowHerbId.Fadeleaf => 160,
                WowHerbId.Goldthorn => 170,
                WowHerbId.KhadgarsWhisker => 185,
                WowHerbId.Wintersbite => 195,

                // High-level classic herbs
                WowHerbId.Firebloom => 205,
                WowHerbId.PurpleLotus => 210,
                WowHerbId.ArthasTears => 220,
                WowHerbId.Sungrass => 230,
                WowHerbId.Blindweed => 235,
                WowHerbId.GhostMushroom => 245,
                WowHerbId.Gromsblood => 250,
                WowHerbId.GoldenSansam => 260,
                WowHerbId.Dreamfoil => 270,
                WowHerbId.MountainSilversage => 280,
                WowHerbId.Plaguebloom => 285,
                WowHerbId.Icecap => 290,
                WowHerbId.BlackLotus => 300,

                // TBC herbs
                WowHerbId.Felweed => 300,
                WowHerbId.DreamingGlory => 315,
                WowHerbId.Ragveil => 325,
                WowHerbId.Terocone => 325,
                WowHerbId.FlameCap => 335,
                WowHerbId.AncientLichen => 340,
                WowHerbId.Netherbloom => 350,
                WowHerbId.NightmareVine => 365,
                WowHerbId.ManaThistle => 375,

                // Northrend herbs (WotLK)
                WowHerbId.Goldclover => 350,
                WowHerbId.TigerLily => 375,
                WowHerbId.TalandrasRose => 385,
                WowHerbId.AddersTongue => 400,
                WowHerbId.Lichbloom => 425,
                WowHerbId.Icethorn => 435,

                _ => int.MaxValue
            };
        }

        /// <summary>
        /// Check if a displayId or entryId represents a known herb.
        /// </summary>
        public static bool IsHerb(int displayId, int entryId)
        {
            return Enum.IsDefined(typeof(WowHerbId), displayId)
                || Enum.IsDefined(typeof(WowHerbId), entryId);
        }

        /// <summary>
        /// Try to resolve herb ID from displayId or entryId.
        /// </summary>
        /// <returns>True if resolved, false otherwise.</returns>
        public static bool TryGetHerbId(int displayId, int entryId, out WowHerbId herbId)
        {
            if (Enum.IsDefined(typeof(WowHerbId), displayId))
            {
                herbId = (WowHerbId)displayId;
                return true;
            }

            if (Enum.IsDefined(typeof(WowHerbId), entryId))
            {
                herbId = (WowHerbId)entryId;
                return true;
            }

            herbId = default;
            return false;
        }

        /// <summary>
        /// Check if the player can harvest a herb based on skill.
        /// </summary>
        public static bool CanHarvestHerb(WowHerbId herbId, int herbalismSkill)
        {
            return herbalismSkill >= GetRequiredHerbalismSkill(herbId);
        }

        #endregion

        #region Map Detection

        /// <summary>
        /// Check if a map is in Northrend (continent + all dungeons/raids).
        /// Used for Titanium/Adamantite DisplayId disambiguation.
        /// </summary>
        public static bool IsNorthrendMap(WowMapId mapId)
        {
            return mapId is WowMapId.Northrend
                or WowMapId.EbonHold
                // Northrend Dungeons
                or WowMapId.UtgardeKeep
                or WowMapId.UtgardePinnacle
                or WowMapId.AzjolNerub
                or WowMapId.AhnkahetTheOldKingdom
                or WowMapId.DrakTharonKeep
                or WowMapId.VioletHold
                or WowMapId.Gundrak
                or WowMapId.HallsOfStone
                or WowMapId.HallsOfLighting
                or WowMapId.TheOculus
                or WowMapId.TheCullingOfStratholme
                or WowMapId.TrialOfTheChampion
                or WowMapId.TheForgeOfSouls
                or WowMapId.PitOfSaron
                or WowMapId.HallsOfReflection
                // Northrend Raids
                or WowMapId.VaultOfArchavon
                or WowMapId.TheObsidianSanctum
                or WowMapId.TheEyeOfEternity
                or WowMapId.Naxxramas
                or WowMapId.Ulduar
                or WowMapId.TrialOfTheCrusader
                or WowMapId.IcecrownCitadel
                or WowMapId.TheRubySanctum;
        }

        #endregion
    }
}
