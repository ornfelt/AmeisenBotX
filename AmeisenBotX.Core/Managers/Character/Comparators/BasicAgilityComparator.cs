using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

/// <summary>
/// Provides classes for managing and comparing character attributes in the AmeisenBotX framework.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Initializes a new instance of the BasicAgilityComparator class with optional armor and weapon type blacklists.
    /// </summary>
    public class BasicAgilityComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the BasicAgilityComparator class with optional armor and weapon type blacklists.
        /// </summary>
        public BasicAgilityComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new(new()
            {
                { WowStatType.AGILITY, 3.0 },
                { WowStatType.ATTACK_POWER, 2.0 },
                { WowStatType.CRIT, 2.2 },
                { WowStatType.ARMOR, 2.0 },
            });
        }
    }
}