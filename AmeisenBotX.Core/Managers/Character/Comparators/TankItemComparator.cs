﻿using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Globalization;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Determines if the given item is better than the current item based on their ratings.
    /// Returns true if the item is better, false otherwise.
    /// </summary>
    /// <param name="current">The current item to compare against.</param>
    /// <param name="item">The item to be checked.</param>
    /// <returns>True if the item is better, false otherwise.</returns>
    public class TankItemComparator : IItemComparator
    {
        /// <summary>
        /// Determines if the given item is better than the current item based on their ratings.
        /// Returns true if the item is better, false otherwise.
        /// </summary>
        /// <param name="current">The current item to compare against.</param>
        /// <param name="item">The item to be checked.</param>
        /// <returns>True if the item is better, false otherwise.</returns>
        public bool IsBetter(IWowInventoryItem current, IWowInventoryItem item)
        {
            if (item == null)
            {
                return false;
            }
            else if (current == null)
            {
                return true;
            }
            else if (item.Stats == null)
            {
                return false;
            }
            else if (current.Stats == null)
            {
                return true;
            }

            double currentRating = GetRating(current, (WowEquipmentSlot)current.EquipSlot);
            double newItemRating = GetRating(item, (WowEquipmentSlot)current.EquipSlot);
            return currentRating < newItemRating;
        }

        /// <summary>
        /// Determines if the specified item is blacklisted.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is blacklisted, false otherwise.</returns>
        public bool IsBlacklistedItem(IWowInventoryItem item)
        {
            return false;
        }

        /// <summary>
        /// Calculates the rating value of an item based on its stats and equipment slot.
        /// </summary>
        /// <param name="item">The item to be evaluated.</param>
        /// <param name="slot">The equipment slot where the item is equipped.</param>
        /// <returns>The rating value of the item.</returns>
        private static double GetRating(IWowInventoryItem item, WowEquipmentSlot slot)
        {
            double rating = 0;

            if (slot.Equals(WowEquipmentSlot.INVSLOT_OFFHAND))
            {
                // shields
                if (item.GetType() == typeof(WowArmor) && ((WowArmor)item).ArmorType.Equals(WowArmorType.Shield))
                {
                    if (item.Stats.TryGetValue("RESISTANCE0_NAME", out string armorString) && double.TryParse(armorString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double armor))
                    {
                        rating += 0.5 * armor;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_STAMINA_SHORT", out string staminaString) && double.TryParse(staminaString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double stamina))
                    {
                        rating += 1f * stamina;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_DEFENSE_SKILL_RATING_SHORT", out string defenseString) && double.TryParse(defenseString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double defense))
                    {
                        rating += 1f * defense;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_BLOCK_VALUE_SHORT", out string blockString) && double.TryParse(blockString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double blockValue))
                    {
                        rating += 5f * blockValue;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_BLOCK_RATING_SHORT", out string blockChanceString) && double.TryParse(blockChanceString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double blockChance))
                    {
                        rating += 10f * blockChance;
                    }
                }
            }
            else if (slot.Equals(WowEquipmentSlot.INVSLOT_MAINHAND))
            {
                // swords
                if (item.GetType() == typeof(WowWeapon) && ((WowWeapon)item).WeaponType.Equals(WowWeaponType.Sword))
                {
                    if (item.Stats.TryGetValue("ITEM_MOD_STRENGTH_SHORT", out string strengthString) && double.TryParse(strengthString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double strength))
                    {
                        rating += 0.5f * strength;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_DAMAGE_PER_SECOND_SHORT", out string dpsString) && double.TryParse(dpsString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double dps))
                    {
                        rating += 1f * dps;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_DEFENSE_SKILL_RATING_SHORT", out string defenseString) && double.TryParse(defenseString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double defense))
                    {
                        rating += 5f * defense;
                    }

                    if (item.Stats.TryGetValue("ITEM_MOD_PARRY_RATING_SHORT", out string parryString) && double.TryParse(parryString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double parry))
                    {
                        rating += 10f * parry;
                    }
                }
            }
            else if (slot.Equals(WowEquipmentSlot.INVSLOT_NECK) || slot.Equals(WowEquipmentSlot.INVSLOT_RING1)
                || slot.Equals(WowEquipmentSlot.INVSLOT_RING2) || slot.Equals(WowEquipmentSlot.INVSLOT_TRINKET1)
                || slot.Equals(WowEquipmentSlot.INVSLOT_TRINKET2))
            {
                // jewelry stats
                if (item.Stats.TryGetValue("ITEM_MOD_STRENGTH_SHORT", out string strengthString) && double.TryParse(strengthString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double strength))
                {
                    rating += 0.5f * strength;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_STAMINA_SHORT", out string staminaString) && double.TryParse(staminaString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double stamina))
                {
                    rating += 1f * stamina;
                }
            }
            else
            {
                // armor stats
                if (item.Stats.TryGetValue("RESISTANCE0_NAME", out string armorString) && double.TryParse(armorString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double armor))
                {
                    rating += 0.5f * armor;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_STRENGTH_SHORT", out string strengthString) && double.TryParse(strengthString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double strength))
                {
                    rating += 0.5f * strength;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_PARRY_RATING_SHORT", out string parryString) && double.TryParse(parryString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double parry))
                {
                    rating += 0.5f * parry;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_BLOCK_RATING_SHORT", out string blockChanceString) && double.TryParse(blockChanceString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double blockChance))
                {
                    rating += 0.5f * blockChance;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_BLOCK_VALUE_SHORT", out string blockString) && double.TryParse(blockString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double blockValue))
                {
                    rating += 0.5f * blockValue;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_STAMINA_SHORT", out string staminaString) && double.TryParse(staminaString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double stamina))
                {
                    rating += 1f * stamina;
                }

                if (item.Stats.TryGetValue("ITEM_MOD_DEFENSE_SKILL_RATING_SHORT", out string defenseString) && double.TryParse(defenseString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double defense))
                {
                    rating += 1f * defense;
                }
            }

            return rating;
        }
    }
}