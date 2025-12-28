using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Wow
{
    /// <summary>
    /// WoW-enforced class mechanics: which classes can use which armor/weapon types,
    /// what skill names WoW uses, and level-based armor upgrades.
    /// </summary>
    public static class WowClassHelper
    {
        #region Armor Class Restrictions

        /// <summary>
        /// WoW-enforced armor type restrictions by class.
        /// Note: All classes can wear "lower" armor types (e.g., Warriors can wear Cloth).
        /// </summary>
        public static readonly Dictionary<WowArmorType, HashSet<WowClass>> ArmorTypeClasses = new()
        {
            [WowArmorType.Plate] = [WowClass.Warrior, WowClass.Paladin, WowClass.Deathknight],
            [WowArmorType.Mail] = [WowClass.Hunter, WowClass.Shaman, WowClass.Warrior, WowClass.Paladin, WowClass.Deathknight],
            [WowArmorType.Leather] = [WowClass.Hunter, WowClass.Shaman, WowClass.Rogue, WowClass.Druid, WowClass.Warrior, WowClass.Paladin, WowClass.Deathknight],
            [WowArmorType.Cloth] = [WowClass.Mage, WowClass.Warlock, WowClass.Priest, WowClass.Hunter, WowClass.Shaman, WowClass.Rogue, WowClass.Druid, WowClass.Warrior, WowClass.Paladin, WowClass.Deathknight],
        };

        /// <summary>
        /// Check if a class can equip a given armor type.
        /// </summary>
        public static bool CanClassWearArmor(WowClass wowClass, WowArmorType armorType)
        {
            return ArmorTypeClasses.TryGetValue(armorType, out HashSet<WowClass> classes) && classes.Contains(wowClass);
        }

        /// <summary>
        /// WoW-enforced level 40 armor upgrade rules.
        /// Warriors/Paladins/DKs upgrade from Mail to Plate at 40.
        /// Hunters/Shamans upgrade from Leather to Mail at 40.
        /// </summary>
        public static WowArmorType GetPreferredArmorType(WowClass wowClass, int level)
        {
            return wowClass switch
            {
                WowClass.Warrior or WowClass.Paladin or WowClass.Deathknight => level >= 40 ? WowArmorType.Plate : WowArmorType.Mail,
                WowClass.Hunter or WowClass.Shaman => level >= 40 ? WowArmorType.Mail : WowArmorType.Leather,
                WowClass.Rogue or WowClass.Druid => WowArmorType.Leather,
                WowClass.Mage or WowClass.Warlock or WowClass.Priest => WowArmorType.Cloth,
                _ => WowArmorType.Cloth
            };
        }

        #endregion

        #region Skill Name Mappings

        /// <summary>
        /// WoW's internal skill names for armor types.
        /// Used to check if a character has learned a skill.
        /// </summary>
        public static string GetArmorSkillName(WowArmorType armorType)
        {
            return armorType switch
            {
                WowArmorType.Plate => "Plate Mail",
                WowArmorType.Mail => "Mail",
                WowArmorType.Leather => "Leather",
                WowArmorType.Cloth => "Cloth",
                WowArmorType.Shield => "Shield",
                WowArmorType.Totem => "Totem",
                WowArmorType.Libram => "Libram",
                WowArmorType.Idol => "Idol",
                WowArmorType.Sigil => "Sigil",
                _ => null
            };
        }

        /// <summary>
        /// WoW's internal skill names for weapon types.
        /// </summary>
        public static string GetWeaponSkillName(WowWeaponType weaponType)
        {
            return weaponType switch
            {
                WowWeaponType.Bow => "Bows",
                WowWeaponType.Crossbow => "Crossbows",
                WowWeaponType.Gun => "Guns",
                WowWeaponType.Wand => "Wands",
                WowWeaponType.Thrown => "Thrown",
                WowWeaponType.Axe => "Axes",
                WowWeaponType.AxeTwoHand => "Two-Handed Axes",
                WowWeaponType.Mace => "Maces",
                WowWeaponType.MaceTwoHand => "Two-Handed Maces",
                WowWeaponType.Sword => "Swords",
                WowWeaponType.SwordTwoHand => "Two-Handed Swords",
                WowWeaponType.Dagger => "Daggers",
                WowWeaponType.Fist => "Fist Weapons",
                WowWeaponType.Polearm => "Polearms",
                WowWeaponType.Staff => "Staves",
                _ => null
            };
        }

        #endregion

        #region Equipment Slot Mappings

        /// <summary>
        /// WoW's INVTYPE string for each equipment slot.
        /// </summary>
        public static string GetEquipLocationForSlot(WowEquipmentSlot slot)
        {
            return slot switch
            {
                WowEquipmentSlot.INVSLOT_AMMO => "INVTYPE_AMMO",
                WowEquipmentSlot.INVSLOT_HEAD => "INVTYPE_HEAD",
                WowEquipmentSlot.INVSLOT_NECK => "INVTYPE_NECK",
                WowEquipmentSlot.INVSLOT_SHOULDER => "INVTYPE_SHOULDER",
                WowEquipmentSlot.INVSLOT_SHIRT => "INVTYPE_BODY",
                WowEquipmentSlot.INVSLOT_CHEST => "INVTYPE_CHEST|INVTYPE_ROBE",
                WowEquipmentSlot.INVSLOT_WAIST => "INVTYPE_WAIST",
                WowEquipmentSlot.INVSLOT_LEGS => "INVTYPE_LEGS",
                WowEquipmentSlot.INVSLOT_FEET => "INVTYPE_FEET",
                WowEquipmentSlot.INVSLOT_WRIST => "INVTYPE_WRIST",
                WowEquipmentSlot.INVSLOT_HANDS => "INVTYPE_HAND",
                WowEquipmentSlot.INVSLOT_RING1 or WowEquipmentSlot.INVSLOT_RING2 => "INVTYPE_FINGER",
                WowEquipmentSlot.INVSLOT_TRINKET1 or WowEquipmentSlot.INVSLOT_TRINKET2 => "INVTYPE_TRINKET",
                WowEquipmentSlot.INVSLOT_BACK => "INVTYPE_CLOAK",
                WowEquipmentSlot.INVSLOT_MAINHAND => "INVTYPE_2HWEAPON|INVTYPE_WEAPON|INVTYPE_WEAPONMAINHAND",
                WowEquipmentSlot.INVSLOT_OFFHAND => "INVTYPE_SHIELD|INVTYPE_WEAPONOFFHAND|INVTYPE_HOLDABLE",
                WowEquipmentSlot.INVSLOT_RANGED => "INVTYPE_RANGED|INVTYPE_THROWN|INVTYPE_RANGEDRIGHT|INVTYPE_RELIC",
                WowEquipmentSlot.INVSLOT_TABARD => "INVTYPE_TABARD",
                WowEquipmentSlot.CONTAINER_BAG_1 or WowEquipmentSlot.CONTAINER_BAG_2 or
                WowEquipmentSlot.CONTAINER_BAG_3 or WowEquipmentSlot.CONTAINER_BAG_4 => "INVTYPE_BAG|INVTYPE_QUIVER",
                _ => null
            };
        }

        #endregion

        #region Class-Specific Equipment

        /// <summary>
        /// Relic types by class (Sigil for DK, Libram for Paladin, Idol for Druid, Totem for Shaman).
        /// </summary>
        public static WowArmorType? GetRelicType(WowClass wowClass)
        {
            return wowClass switch
            {
                WowClass.Deathknight => WowArmorType.Sigil,
                WowClass.Paladin => WowArmorType.Libram,
                WowClass.Druid => WowArmorType.Idol,
                WowClass.Shaman => WowArmorType.Totem,
                _ => null
            };
        }

        #endregion
    }
}
