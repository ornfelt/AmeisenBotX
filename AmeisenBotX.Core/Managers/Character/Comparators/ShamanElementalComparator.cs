using AmeisenBotX.Core.Managers.Character.Comparators.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Initializes a new instance of the ShamanElementalComparator class with optional armor and weapon type blacklists.
    /// </summary>
    public class ShamanElementalComparator : BasicComparator
    {
        /// <summary>
        /// Initializes a new instance of the ShamanElementalComparator class with optional armor and weapon type blacklists.
        /// Sets the GearscoreFactory property to a new instance of GearscoreFactory, and assigns default weight values to different WowStatTypes.
        /// </summary>
        public ShamanElementalComparator(List<WowArmorType> armorTypeBlacklist = null, List<WowWeaponType> weaponTypeBlacklist = null) : base(armorTypeBlacklist, weaponTypeBlacklist)
        {
            GearscoreFactory = new GearscoreFactory(new Dictionary<string, double>
            {
                { WowStatType.HIT, 3.0 },         // Hard-cap 17%
                { WowStatType.HASTE, 3.0 },       // Soft-cap 1000 - 1200
                { WowStatType.CRIT, 2.5 },        // Soft-cap 48.3%
                { WowStatType.SPELL_POWER, 2.5 }, // uncapped
                { WowStatType.INTELLECT, 2.0 },   // uncapped
                { WowStatType.MP5, 2.0 },         // uncapped
                { WowStatType.STAMINA, 1.5 },     // uncapped
                { WowStatType.SPIRIT, 1.5 }       // uncapped
            });
        }
    }
}