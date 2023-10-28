using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

/// <summary>
/// Represents a basic stamina comparator that compares stamina values of items.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Represents a basic stamina comparator that compares stamina values of items.
    /// </summary>
    public class BasicStaminaComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicStaminaComparator"/> class.
        /// </summary>
        /// <param name="armorTypeBlacklist">The list of armor types to blacklist.</param>
        /// <param name="weaponTypeBlacklist">The list of weapon types to blacklist.</param>
        public BasicStaminaComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new(new()
            {
                { WowStatType.STAMINA, 4.0 },
                { WowStatType.STRENGTH, 2.5 },
                { WowStatType.ARMOR, 2.0 },
            });
        }
    }
}