using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    public class BasicStrengthComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the BasicStrengthComparator class with optional armor and weapon type blacklists.
        /// The GearscoreFactory is set to a new instance using the following stat weightings:
        /// - Strength: 3.0
        /// - Attack Power: 3.0
        /// - Armor: 2.0
        /// - Crit: 2.0
        /// - DPS: 2.0
        /// </summary>
        public BasicStrengthComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new(new()
            {
                { WowStatType.STRENGTH, 3.0 },
                { WowStatType.ATTACK_POWER, 3.0 },
                { WowStatType.ARMOR, 2.0 },
                { WowStatType.CRIT, 2.0 },
                { WowStatType.DPS, 2.0 },
            });
        }
    }
}