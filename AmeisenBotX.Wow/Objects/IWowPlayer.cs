using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using System.Collections.Generic;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a player character in the World of Warcraft game.
    /// </summary>
    public interface IWowPlayer : IWowUnit
    {
        /// <summary>
        /// Gets the number of combo points.
        /// </summary>
        int ComboPoints { get; }

        /// <summary>
        /// Gets a value indicating whether the object is currently flying.
        /// </summary>
        bool IsFlying { get; }

        /// <summary>
        /// Gets a value indicating whether the object is a ghost.
        /// </summary>
        bool IsGhost { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is outdoors.
        /// </summary>
        bool IsOutdoors { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object is swimming.
        /// </summary>
        bool IsSwimming { get; }

        /// <summary>
        /// Gets a value indicating whether the object is underwater.
        /// </summary>
        bool IsUnderwater { get; }

        /// <summary>
        /// Gets the visible enchantments of an item.
        /// </summary>
        /// <returns>An IEnumerable collection of VisibleItemEnchantment.</returns>
        IEnumerable<VisibleItemEnchantment> ItemEnchantments { get; }

        /// <summary>
        /// Gets the next level XP value.
        /// </summary>
        int NextLevelXp { get; }

        /// <summary>
        /// Gets the IEnumerable of QuestlogEntry objects representing the quest log entries.
        /// </summary>
        IEnumerable<QuestlogEntry> QuestlogEntries { get; }

        /// <summary>
        /// Gets the type of the object, which is a player.
        /// </summary>
        public new WowObjectType Type => WowObjectType.Player;

        ///<summary>
        ///Gets or sets the value of Xp.
        ///</summary>
        int Xp { get; }

        /// <summary>
        /// Gets the XpPercentage property. 
        /// </summary>
        double XpPercentage { get; }

        /// <summary>
        /// Determines if the entity belongs to an alliance.
        /// </summary>
        bool IsAlliance();

        /// <summary>
        /// Checks if the current entity belongs to the Horde faction.
        /// </summary>
        bool IsHorde();
    }
}