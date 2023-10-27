using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Managers.Character.Inventory
{
    public class CharacterInventory
    {
        /// <summary>
        /// Represents a private readonly list of objects that implement the IWowInventoryItem interface.
        /// </summary>
        private readonly List<IWowInventoryItem> items;

        /// <summary>
        /// A lock object used for synchronizing access to the query.
        /// </summary>
        private readonly object queryLock = new();

        /// <summary>
        /// Initializes a new instance of the CharacterInventory class.
        /// </summary>
        /// <param name="wowInterface">The WoW Interface.</param>
        /// <param name="config">The AmeisenBot Configuration.</param>
        public CharacterInventory(IWowInterface wowInterface, AmeisenBotConfig config)
        {
            Wow = wowInterface;
            Config = config;
            Items = new();
        }

        /// <summary>
        /// Gets the number of free bag slots.
        /// </summary>
        public int FreeBagSlots { get; private set; }

        /// <summary>
        /// Gets or sets the list of WOW inventory items.
        /// </summary>
        /// <remarks>
        /// This property is thread-safe and can be accessed by multiple threads simultaneously.
        /// </remarks>
        /// <returns>
        /// The list of WOW inventory items.
        /// </returns>
        public List<IWowInventoryItem> Items
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
        /// Gets the AmeisenBotConfig object.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the delete action is confirmed.
        /// </summary>
        private bool ConfirmDelete { get; set; }

        /// <summary>
        /// Gets or sets the confirm delete time.
        /// </summary>
        private DateTime ConfirmDeleteTime { get; set; }

        /// <summary>
        /// Gets or sets the WoW interface for this object.
        /// </summary>
        private IWowInterface Wow { get; }

        /// <summary>
        /// Destroys an item based on its name.
        /// </summary>
        /// <param name="name">The name of the item to be destroyed.</param>
        /// <param name="stringComparison">The comparison method for the name.</param>
        public void DestroyItemByName(string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (HasItemByName(name, stringComparison))
            {
                Wow.DeleteItemByName(name);
            }
        }

        /// <summary>
        /// Checks if there is an item with the specified name in the collection of items.
        /// </summary>
        /// <param name="name">The name of the item to check for.</param>
        /// <param name="stringComparison">The type of string comparison to use (optional, defaults to OrdinalIgnoreCase).</param>
        /// <returns>True if an item with the specified name is found, otherwise false.</returns>
        public bool HasItemByName(string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return Items.Any(e => e.Name.Equals(name, stringComparison));
        }

        /// <summary>
        /// Executes the action of deleting an item from a static popup.
        /// </summary>
        /// <param name="id">The unique identifier of the static popup.</param>
        public void OnStaticPopupDeleteItem(int id)
        {
            ConfirmDelete = false;
            Wow.ClickUiElement($"StaticPopup{id}Button1");
            AmeisenLogger.I.Log("Inventory", $"Confirmed Deleting");
        }

        /// <summary>
        /// Tries to destroy trash items from the inventory. 
        /// </summary>
        /// <param name="maxQuality">The maximum quality of items to be destroyed. Defaults to Poor quality.</param>
        public void TryDestroyTrash(WowItemQuality maxQuality = WowItemQuality.Poor)
        {
            if (DateTime.UtcNow - ConfirmDeleteTime > TimeSpan.FromSeconds(10))
            {
                // after 10s reset confirm stuff
                ConfirmDelete = false;
            }
            else if (ConfirmDelete)
            {
                // still waiting to confirm deletion
                return;
            }

            foreach (IWowInventoryItem item in Items.Where(e => e.Price > 0 && e.ItemQuality == (int)maxQuality).OrderBy(e => e.Price))
            {
                if (!Config.ItemSellBlacklist.Any(e => e.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    AmeisenLogger.I.Log("Inventory", $"Deleting Trash: {item.Name}");
                    Wow.DeleteItemByName(item.Name);
                    ConfirmDelete = true;
                    ConfirmDeleteTime = DateTime.UtcNow;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the character's inventory by retrieving the number of free bag slots and retrieving a JSON string representation of the inventory items.
        /// Parses the JSON string and builds specific items based on the parsed basic items.
        /// </summary>
        public void Update()
        {
            FreeBagSlots = Wow.GetFreeBagSlotCount();
            string resultJson = Wow.GetInventoryItems();

            try
            {
                List<WowBasicItem> basicItems = ItemFactory.ParseItemList(resultJson);

                if (basicItems is not { Count: > 0 })
                {
                    return;
                }

                lock (queryLock)
                {
                    Items.Clear();

                    foreach (WowBasicItem basicItem in basicItems)
                    {
                        Items.Add(ItemFactory.BuildSpecificItem(basicItem));
                    }
                }
            }
            catch (Exception e)
            {
                AmeisenLogger.I.Log("CharacterManager", $"Failed to parse Inventory JSON:\n{resultJson}\n{e}", LogLevel.Error);
            }
        }
    }
}