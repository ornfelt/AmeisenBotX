using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Routines;

/// <summary>
/// Periodically checks for upgrades in the inventory and equips them.
/// Uses ItemEvaluator to compare items.
/// </summary>
public class EquipUpgradesRoutine
{
    private readonly AmeisenBotInterfaces Bot;
    private readonly AmeisenBotConfig Config;
    private readonly TimegatedEvent CheckEvent;

    // Minimum score difference to be considered an upgrade (prevents flipping)
    private const double UpgradeThreshold = 5.0;

    public EquipUpgradesRoutine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        Bot = bot;
        Config = config;
        CheckEvent = new(TimeSpan.FromSeconds(15)); // Check every 15s don't spam
    }

    public void Update()
    {
        if (!CheckEvent.Run())
        {
            return;
        }

        if (Bot.Player == null || Bot.Player.IsDead || Bot.Player.IsInCombat || Bot.Player.IsCasting)
        {
            return;
        }

        CheckForUpgrades();
    }

    private void CheckForUpgrades()
    {
        // Scan inventory for equipment
        foreach (IWowInventoryItem newItem in Bot.Character.Inventory.Items.ToList())
        {
            if (!IsEquippable(newItem))
            {
                continue;
            }

            // Explicitly skip profession tools (like Skinning Knife)
            if (ItemEvaluator.ProfessionToolNames.Contains(newItem.Name))
            {
                continue;
            }

            // Evaluate the new item
            double newScore = ItemEvaluator.EvaluateItem(Bot, Config, newItem).Score;

            if (newScore <= 0)
            {
                continue; // Not feasible/usable
            }

            // Find corresponding slots
            List<WowEquipmentSlot> slots = GetPossibleSlots(newItem);

            if (slots.Count == 0)
            {
                continue;
            }

            // Find the best slot to upgrade (the one with the lowest score)
            WowEquipmentSlot bestSlotToReplace = WowEquipmentSlot.NOT_EQUIPABLE;
            double lowestCurrentScore = double.MaxValue;
            bool slotEmpty = false;

            foreach (WowEquipmentSlot slot in slots)
            {
                if (Bot.Character.Equipment.Items.TryGetValue(slot, out IWowInventoryItem currentItem))
                {
                    double currentScore = ItemEvaluator.EvaluateItem(Bot, Config, currentItem).Score;
                    if (currentScore < lowestCurrentScore)
                    {
                        lowestCurrentScore = currentScore;
                        bestSlotToReplace = slot;
                    }
                }
                else
                {
                    // Slot is empty - always equip!
                    lowestCurrentScore = -1; // Force equip
                    bestSlotToReplace = slot;
                    slotEmpty = true;
                    break;
                }
            }

            // Check if it's an upgrade
            if (slotEmpty || (newScore > lowestCurrentScore + UpgradeThreshold))
            {
                AmeisenLogger.I.Log("AutoEquip", $"Found upgrade: {newItem.Name} (Score: {newScore:F1}) > Slot {bestSlotToReplace} (Score: {lowestCurrentScore:F1})");
                EquipItem(newItem, bestSlotToReplace);
                return; // Only equip one item per tick to allow refreshes
            }
        }
    }

    private void EquipItem(IWowInventoryItem item, WowEquipmentSlot slot)
    {
        // Use Lua to equip to specific slot to handle rings/trinkets correctly
        // PickupContainerItem(bag, slot) then PickupInventoryItem(slotID)
        Bot.Wow.LuaDoString($@"
                PickupContainerItem({item.BagId}, {item.BagSlot});
                EquipCursorItem({(int)slot});
            ");
    }

    private bool IsEquippable(IWowInventoryItem item)
    {
        string type = item.Type?.ToLowerInvariant() ?? "";
        return type is "armor" or "weapon";
    }

    private static readonly Dictionary<string, WowEquipmentSlot[]> SlotMappings = new()
        {
            { "INVTYPE_HEAD", [WowEquipmentSlot.INVSLOT_HEAD] },
            { "INVTYPE_NECK", [WowEquipmentSlot.INVSLOT_NECK] },
            { "INVTYPE_SHOULDER", [WowEquipmentSlot.INVSLOT_SHOULDER] },
            { "INVTYPE_BODY", [WowEquipmentSlot.INVSLOT_SHIRT] },
            { "INVTYPE_CHEST", [WowEquipmentSlot.INVSLOT_CHEST] },
            { "INVTYPE_ROBE", [WowEquipmentSlot.INVSLOT_CHEST] },
            { "INVTYPE_WAIST", [WowEquipmentSlot.INVSLOT_WAIST] },
            { "INVTYPE_LEGS", [WowEquipmentSlot.INVSLOT_LEGS] },
            { "INVTYPE_FEET", [WowEquipmentSlot.INVSLOT_FEET] },
            { "INVTYPE_WRIST", [WowEquipmentSlot.INVSLOT_WRIST] },
            { "INVTYPE_HAND", [WowEquipmentSlot.INVSLOT_HANDS] },
            { "INVTYPE_FINGER", [WowEquipmentSlot.INVSLOT_RING1, WowEquipmentSlot.INVSLOT_RING2] },
            { "INVTYPE_TRINKET", [WowEquipmentSlot.INVSLOT_TRINKET1, WowEquipmentSlot.INVSLOT_TRINKET2] },
            { "INVTYPE_CLOAK", [WowEquipmentSlot.INVSLOT_BACK] },
            { "INVTYPE_WEAPON", [WowEquipmentSlot.INVSLOT_MAINHAND, WowEquipmentSlot.INVSLOT_OFFHAND] },
            { "INVTYPE_WEAPONMAINHAND", [WowEquipmentSlot.INVSLOT_MAINHAND] },
            { "INVTYPE_2HWEAPON", [WowEquipmentSlot.INVSLOT_MAINHAND] },
            { "INVTYPE_WEAPONOFFHAND", [WowEquipmentSlot.INVSLOT_OFFHAND] },
            { "INVTYPE_SHIELD", [WowEquipmentSlot.INVSLOT_OFFHAND] },
            { "INVTYPE_HOLDABLE", [WowEquipmentSlot.INVSLOT_OFFHAND] },
            { "INVTYPE_RANGED", [WowEquipmentSlot.INVSLOT_RANGED] },
            { "INVTYPE_THROWN", [WowEquipmentSlot.INVSLOT_RANGED] },
            { "INVTYPE_RELIC", [WowEquipmentSlot.INVSLOT_RANGED] }
        };

    private List<WowEquipmentSlot> GetPossibleSlots(IWowInventoryItem item)
    {
        return string.IsNullOrEmpty(item.EquipLocation) ? [] : SlotMappings.TryGetValue(item.EquipLocation, out global::AmeisenBotX.Wow.Objects.Enums.WowEquipmentSlot[] slots) ? [.. slots] : [];
    }
}
