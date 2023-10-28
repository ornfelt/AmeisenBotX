using AmeisenBotX.Wow.Objects.Enums;

/// <summary>
/// Defines the objects used in the AmeisenBotX World of Warcraft bot.
/// </summary>
namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Collection of extensions to mainly enums at the moment.
    /// </summary>
    public static class AmeisenBotExtensions
    {
        /// <summary>
        /// Returns the color code corresponding to the specified item quality.
        /// </summary>
        /// <param name="itemQuality">The quality of the item.</param>
        /// <returns>The color code.</returns>
        public static string GetColor(this WowItemQuality itemQuality)
        {
            return itemQuality switch
            {
                WowItemQuality.Unique => "#00ccff",
                WowItemQuality.Poor => "#9d9d9d",
                WowItemQuality.Common => "#ffffff",
                WowItemQuality.Uncommon => "#1eff00",
                WowItemQuality.Rare => "#0070dd",
                WowItemQuality.Epic => "#a335ee",
                WowItemQuality.Legendary => "#ff8000",
                WowItemQuality.Artifact => "#e6cc80",
                WowItemQuality.Heirloom => "#bed3e5",
                _ => "#ffffff",
            };
        }

        /// <summary>
        /// Determines if the specified map is a battleground map.
        /// </summary>
        public static bool IsBattlegroundMap(this WowMapId map)
        {
            return map is WowMapId.AlteracValley
                or WowMapId.WarsongGulch
                or WowMapId.ArathiBasin
                or WowMapId.EyeOfTheStorm
                or WowMapId.StrandOfTheAncients;
        }

        /// <summary>
        /// Determines if the given zone is a capital city zone based on the specified faction.
        /// </summary>
        /// <param name="zone">The WowZoneId to check.</param>
        /// <param name="isAlliance">A boolean value indicating if the faction is Alliance or not.</param>
        /// <returns>True if the zone is a capital city for the specified faction, otherwise false.</returns>
        public static bool IsCapitalCityZone(this WowZoneId zone, bool isAlliance)
        {
            return isAlliance
                ? zone is WowZoneId.StormwindCity
                    or WowZoneId.Ironforge
                    or WowZoneId.Teldrassil
                    or WowZoneId.TheExodar
                : zone is WowZoneId.Orgrimmar
                    or WowZoneId.Undercity
                    or WowZoneId.ThunderBluff
                    or WowZoneId.SilvermoonCity;
        }

        ///<summary>
        ///Checks if the given WowMapId is a dungeon map.
        ///</summary>
        ///<param name="map">The WowMapId to check.</param>
        ///<returns>
        ///True if the map is a dungeon map, false otherwise.
        ///</returns>
        public static bool IsDungeonMap(this WowMapId map)
        {
            // classic
            return map is WowMapId.RagefireChasm
                or WowMapId.WailingCaverns
                or WowMapId.Deadmines
                or WowMapId.ShadowfangKeep
                or WowMapId.StormwindStockade
                // tbc
                or WowMapId.HellfireRamparts
                or WowMapId.TheBloodFurnace
                or WowMapId.TheSlavePens
                or WowMapId.TheUnderbog
                or WowMapId.TheSteamvault
                // wotlk
                or WowMapId.UtgardeKeep
                or WowMapId.AzjolNerub
                or WowMapId.HallsOfLighting
                or WowMapId.TheForgeOfSouls
                or WowMapId.PitOfSaron;
        }

        /// <summary>
        /// Determines if the given map is a raid map.
        /// </summary>
        public static bool IsRaidMap(this WowMapId map)
        {
            // classic
            return map is WowMapId.MoltenCore
                or WowMapId.OnyxiasLair
                or WowMapId.BlackwingLair
                or WowMapId.RuinsOfAhnQiraj
                or WowMapId.AhnQirajTemple
                or WowMapId.Naxxramas
                // tbc
                or WowMapId.GruulsLair
                or WowMapId.Karazhan
                or WowMapId.MagtheridonsLair
                or WowMapId.SerpentshrineCavern
                or WowMapId.TempestKeep
                or WowMapId.TheBattleForMountHyjal
                or WowMapId.BlackTemple
                or WowMapId.ZulAman
                or WowMapId.TheSunwell
                // wotlk
                or WowMapId.VaultOfArchavon
                or WowMapId.TheObsidianSanctum
                or WowMapId.TheEyeOfEternity
                or WowMapId.Ulduar
                or WowMapId.TrialOfTheCrusader
                or WowMapId.IcecrownCitadel
                or WowMapId.TheRubySanctum;
        }
    }
}