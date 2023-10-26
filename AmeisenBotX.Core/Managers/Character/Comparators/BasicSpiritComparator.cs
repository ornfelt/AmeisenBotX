using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    public class BasicSpiritComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the BasicSpiritComparator class.
        /// </summary>
        /// <param name="armorTypeBlacklist">A list of armor types to be excluded from the comparison. Default is null.</param>
        /// <param name="weaponTypeBlacklist">A list of weapon types to be excluded from the comparison. Default is null.</param>
        public BasicSpiritComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new(new()
            {
                { WowStatType.INTELLECT, 2.5 },
                { WowStatType.SPIRIT, 2.5 },
                { WowStatType.SPELL_POWER, 2.5 },
                { WowStatType.MP5, 2.0 },
                { WowStatType.ARMOR, 2.0 },
            });
        }
    }
}