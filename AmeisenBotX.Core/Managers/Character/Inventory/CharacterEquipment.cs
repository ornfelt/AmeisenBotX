using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Managers.Character.Inventory
{
    public class CharacterEquipment
    {
        /// <summary>
        /// Represents a private readonly dictionary that maps WowEquipmentSlot to IWowInventoryItem.
        /// </summary>
        private readonly Dictionary<WowEquipmentSlot, IWowInventoryItem> items;

        /// <summary>
        /// Specifies a read-only object that is used as a lock for synchronizing accesses to the 'queryLock' field.
        /// </summary>
        private readonly object queryLock = new();

        /// <summary>
        /// Initializes a new instance of the CharacterEquipment class.
        /// </summary>
        /// <param name="wowInterface">The World of Warcraft interface.</param>
        public CharacterEquipment(IWowInterface wowInterface)
        {
            Wow = wowInterface;
            Items = new();
        }

        /// <summary>
        /// Gets the average item level.
        /// </summary>
        public float AverageItemLevel { get; private set; }

        /// <summary>
        /// Gets or sets the Dictionary of WowEquipmentSlot and IWowInventoryItem representing the items in the inventory.
        /// </summary>
        public Dictionary<WowEquipmentSlot, IWowInventoryItem> Items
        {
            get
            {
                lock (queryLock)
                {
                    return items;
                }
            }
            private init
            {
                lock (queryLock)
                {
                    items = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the private property Wow of type IWowInterface.
        /// </summary>
        private IWowInterface Wow { get; }

        /// <summary>
        /// Determines if the specified WowEquipmentSlot has an enchantment with the given enchantmentId.
        /// </summary>
        public bool HasEnchantment(WowEquipmentSlot slot, int enchantmentId)
        {
            if (!Items.ContainsKey(slot) || Items[slot].Id <= 0)
            {
                return false;
            }

            IWowItem item = Wow.ObjectProvider.All.OfType<IWowItem>()
                .FirstOrDefault(e => e.EntryId == Items[slot].Id);

            return item != null && item.ItemEnchantments.Any(e =>
                e.Id == enchantmentId);
        }

        /// <summary>
        /// Updates the character's equipment items. Retrieves the equipment items in JSON format from Wow API and parses it into a list of WowBasicItem objects.
        /// If the JSON data is empty or null, the update process is skipped.
        /// If the parsing process fails, an error message is logged.
        /// Finally, calculates and updates the average item level of the character.
        /// </summary>
        public void Update()
        {
            string resultJson = Wow.GetEquipmentItems();

            if (string.IsNullOrWhiteSpace(resultJson))
            {
                return;
            }

            try
            {
                List<WowBasicItem> rawEquipment = ItemFactory.ParseItemList(resultJson);

                if (rawEquipment != null && rawEquipment.Any())
                {
                    lock (queryLock)
                    {
                        Items.Clear();

                        foreach (WowBasicItem item in rawEquipment.Select(ItemFactory.BuildSpecificItem))
                        {
                            Items.Add((WowEquipmentSlot)((IWowInventoryItem)item).EquipSlot, item);
                        }
                    }
                }

                AverageItemLevel = GetAverageItemLevel();
            }
            catch (Exception e)
            {
                AmeisenLogger.I.Log("CharacterManager", $"Failed to parse Equipment JSON:\n{resultJson}\n{e}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Calculates the average item level of equipped items, excluding certain equipment slots.
        /// </summary>
        /// <returns>The average item level.</returns>
        private float GetAverageItemLevel()
        {
            float itemLevel = 0.0f;
            int count = 0;

            IList enumValues = Enum.GetValues(typeof(WowEquipmentSlot));

            foreach (object enumValue in enumValues)
            {
                WowEquipmentSlot slot = (WowEquipmentSlot)enumValue;

                if (slot is WowEquipmentSlot.CONTAINER_BAG_1
                    or WowEquipmentSlot.CONTAINER_BAG_2
                    or WowEquipmentSlot.CONTAINER_BAG_3
                    or WowEquipmentSlot.CONTAINER_BAG_4
                    or WowEquipmentSlot.INVSLOT_OFFHAND
                    or WowEquipmentSlot.INVSLOT_TABARD
                    or WowEquipmentSlot.INVSLOT_AMMO
                    or WowEquipmentSlot.NOT_EQUIPABLE)
                {
                    continue;
                }

                if (Items.ContainsKey(slot))
                {
                    itemLevel += Items[slot].ItemLevel;
                }

                ++count;
            }

            return itemLevel /= count;
        }
    }
}