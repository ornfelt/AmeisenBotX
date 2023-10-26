using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Wow.Cache.Structs
{
    /// <summary>
    /// Represents an interface for a unit that can like different factions in World of Warcraft.
    /// </summary>
    internal interface ILikeUnit
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity likes the alliance.
        /// </summary>
        public bool LikesAlliance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person likes the Horde faction.
        /// </summary>
        public bool LikesHorde { get; set; }

        /// <summary>
        /// Determines if the character likes a specific WoW unit based on their race.
        /// </summary>
        /// <param name="wowUnit">The WoW unit to check.</param>
        /// <returns>True if the character likes the WoW unit, false otherwise.</returns>
        public bool LikesUnit(IWowUnit wowUnit)
        {
            return (LikesAlliance && (wowUnit.Race == WowRace.Human || wowUnit.Race == WowRace.Gnome ||
                                      wowUnit.Race == WowRace.Draenei || wowUnit.Race == WowRace.Dwarf ||
                                      wowUnit.Race == WowRace.Nightelf)) ||
                   (LikesHorde && (wowUnit.Race == WowRace.Orc || wowUnit.Race == WowRace.Troll ||
                                   wowUnit.Race == WowRace.Bloodelf || wowUnit.Race == WowRace.Undead ||
                                   wowUnit.Race == WowRace.Tauren));
        }
    }
}