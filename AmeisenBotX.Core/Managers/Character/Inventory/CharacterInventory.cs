using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBotX.Core.Managers.Character.Inventory
{
    public class CharacterInventory
    {
        private readonly List<IWowInventoryItem> items;

        private readonly Lock queryLock = new();

        public CharacterInventory(IWowInterface wowInterface, AmeisenBotConfig config)
        {
            Wow = wowInterface;
            Config = config;
            Items = [];
        }

        public int FreeBagSlots { get; private set; }

        public List<IWowInventoryItem> Items
        {
            get
            {
                using (queryLock.EnterScope())
                {
                    return items;
                }
            }
            private init
            {
                using (queryLock.EnterScope())
                {
                    items = value;
                }
            }
        }

        private AmeisenBotConfig Config { get; }

        private bool ConfirmDelete { get; set; }

        private DateTime ConfirmDeleteTime { get; set; }

        private IWowInterface Wow { get; }

        public void DestroyItemByName(string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (HasItemByName(name, stringComparison))
            {
                Wow.DeleteItemByName(name);
            }
        }

        public bool HasItemByName(string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return Items.Any(e => e.Name.Equals(name, stringComparison));
        }

        public void OnStaticPopupDeleteItem(int id)
        {
            ConfirmDelete = false;
            Wow.ClickUiElement($"StaticPopup{id}Button1");
            AmeisenLogger.I.Log("Inventory", $"Confirmed Deleting");
        }

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

                using (queryLock.EnterScope())
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

        public IWowInventoryItem GetItem(int id) => Items.FirstOrDefault(e => e.Id == id);

        public IWowInventoryItem GetItem(string name) => Items.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public int GetItemCount(int id)
        {
            return Items.Where(e => e.Id == id).Sum(e => e.Count);
        }
    }
}
