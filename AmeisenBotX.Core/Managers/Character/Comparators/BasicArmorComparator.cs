using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Initializes a new instance of the BasicArmorComparator class with optional armor and weapon type blacklists.
    /// </summary>
    /// <param name="armorTypeBlacklist">A list of WowArmorType to blacklist.</param>
    /// <param name="weaponTypeBlacklist">A list of WowWeaponType to blacklist.</param>
    public class BasicArmorComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the BasicArmorComparator class with optional armor and weapon type blacklists.
        /// </summary>
        /// <param name="armorTypeBlacklist">A list of WowArmorType to blacklist.</param>
        /// <param name="weaponTypeBlacklist">A list of WowWeaponType to blacklist.</param>
        public BasicArmorComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new(new()
            {
                { WowStatType.STAMINA, 2.0 },
                { WowStatType.ARMOR, 2.5 },
            });
        }
    }
}