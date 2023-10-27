using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    public class BasicIntellectComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the BasicIntellectComparator class.
        /// </summary>
        /// <param name="armorTypeBlacklist">The list of armor types to be blacklisted. Default value is null.</param>
        /// <param name="weaponTypeBlacklist">The list of weapon types to be blacklisted. Default value is null.</param>
        public BasicIntellectComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new(new()
            {
                { WowStatType.INTELLECT, 2.5 },
                { WowStatType.SPELL_POWER, 2.5 },
                { WowStatType.ARMOR, 2.0 },
                { WowStatType.MP5, 2.0 },
                { WowStatType.HASTE, 2.0 },
            });
        }
    }
}