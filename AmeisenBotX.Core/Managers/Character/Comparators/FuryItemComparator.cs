﻿using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Globalization;

/// <summary>
/// Represents a namespace for managing character comparators in the AmeisenBotX.Core.Managers.Character.Comparators namespace.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Represents a comparator for Fury items that implements the IItemComparator interface.
    /// </summary>
    public class FuryItemComparator : IItemComparator
    {
        /// <summary>
        /// Initializes a new instance of the FuryItemComparator class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public FuryItemComparator(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets or sets the Bot object of type AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Compares two WoW inventory items and determines if the second item is better than the first item.
        /// </summary>
        /// <param name="current">The current inventory item to compare.</param>
        /// <param name="item">The new inventory item to compare.</param>
        /// <returns>True if the new item is better, false otherwise.</returns>
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
        /// Determines if the provided item is a blacklisted item.
        /// Returns false.
        /// </summary>
        public bool IsBlacklistedItem(IWowInventoryItem item)
        {
            return false;
        }

        /// <summary>
        /// Calculates the rating of an inventory item based on its stats and equipment slot.
        /// </summary>
        /// <param name="item">The inventory item to calculate the rating for.</param>
        /// <param name="slot">The equipment slot of the item.</param>
        /// <returns>The rating of the inventory item.</returns>
        private double GetRating(IWowInventoryItem item, WowEquipmentSlot slot)
        {
            double rating = 0;

            if (item.Stats.TryGetValue("ITEM_MOD_CRIT_MELEE_RATING_SHORT", out string meleeCritString)
                && double.TryParse(meleeCritString, NumberStyles.Any, CultureInfo.InvariantCulture, out double meleeCrit))
            {
                rating += 0.5f * meleeCrit;
            }

            if (item.Stats.TryGetValue("ITEM_MOD_CRIT_RATING_SHORT", out string critString)
                && double.TryParse(critString, NumberStyles.Any, CultureInfo.InvariantCulture, out double crit))
            {
                rating += 0.5f * crit;
            }

            if (item.Stats.TryGetValue("ITEM_MOD_AGILITY_SHORT", out string agilityString)
                && double.TryParse(agilityString, NumberStyles.Any, CultureInfo.InvariantCulture, out double agility))
            {
                rating += 0.5f * agility;
            }

            if (item.Stats.TryGetValue("ITEM_MOD_ATTACK_POWER_SHORT", out string attackString)
                && double.TryParse(attackString, NumberStyles.Any, CultureInfo.InvariantCulture, out double attack))
            {
                rating += 0.5f * attack;
            }

            if (item.Stats.TryGetValue("ITEM_MOD_STRENGTH_SHORT", out string strengthString)
                && double.TryParse(strengthString, NumberStyles.Any, CultureInfo.InvariantCulture, out double strength))
            {
                rating += 1f * strength;
            }

            if (slot.Equals(WowEquipmentSlot.INVSLOT_OFFHAND) || slot.Equals(WowEquipmentSlot.INVSLOT_MAINHAND))
            {
                // also 2nd weapons
                if (item.GetType() == typeof(WowWeapon)
                    && (Bot.Player.IsAlliance() ? ((WowWeapon)item).WeaponType.Equals(WowWeaponType.Sword) : ((WowWeapon)item).WeaponType.Equals(WowWeaponType.Axe)
                    || (Bot.Character.SpellBook.IsSpellKnown("Titan's Grip")
                    && Bot.Player.IsAlliance() ? ((WowWeapon)item).WeaponType.Equals(WowWeaponType.SwordTwoHand) : ((WowWeapon)item).WeaponType.Equals(WowWeaponType.AxeTwoHand))))
                {
                    if (item.Stats.TryGetValue("ITEM_MOD_DAMAGE_PER_SECOND_SHORT", out string dpsString)
                        && double.TryParse(dpsString, NumberStyles.Any, CultureInfo.InvariantCulture, out double dps))
                    {
                        rating += 2f * dps;
                    }
                }
            }
            else if (!(slot.Equals(WowEquipmentSlot.INVSLOT_NECK) || slot.Equals(WowEquipmentSlot.INVSLOT_RING1)
                || slot.Equals(WowEquipmentSlot.INVSLOT_RING2) || slot.Equals(WowEquipmentSlot.INVSLOT_TRINKET1)
                || slot.Equals(WowEquipmentSlot.INVSLOT_TRINKET2)))
            {
                // armor stats
                if (item.Stats.TryGetValue("RESISTANCE0_NAME", out string armorString)
                    && double.TryParse(armorString, NumberStyles.Any, CultureInfo.InvariantCulture, out double armor))
                {
                    rating += 0.1f * armor;
                }
            }

            return rating;
        }
    }
}