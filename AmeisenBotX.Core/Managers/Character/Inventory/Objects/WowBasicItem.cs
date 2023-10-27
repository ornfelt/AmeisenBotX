using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    public class WowBasicItem : IWowInventoryItem
    {
        /// <summary>
        /// Initializes a new instance of the WowBasicItem class.
        /// </summary>
        public WowBasicItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WowBasicItem class using an existing IWowInventoryItem object.
        /// </summary>
        protected WowBasicItem(IWowInventoryItem item)
        {
            if (item == null)
            {
                return;
            }

            BagId = item.BagId;
            BagSlot = item.BagSlot;
            Count = item.Count;
            Durability = item.Durability;
            EquipLocation = item.EquipLocation;
            EquipSlot = item.EquipSlot;
            Id = item.Id;
            ItemLevel = item.ItemLevel;
            ItemLink = item.ItemLink;
            ItemQuality = item.ItemQuality;
            MaxDurability = item.MaxDurability;
            MaxStack = item.MaxStack;
            Name = item.Name;
            Price = item.Price;
            RequiredLevel = item.RequiredLevel;
            Stats = item.Stats;
            Subtype = item.Subtype;
            Type = item.Type;
        }

        /// <summary>
        /// Gets or sets the bag ID.
        /// </summary>
        /// <value>
        /// The bag ID.
        /// </value>
        [JsonPropertyName("bagid")]
        public int BagId { get; set; }

        ///<summary>
        /// Gets or sets the bag slot number.
        ///</summary>
        [JsonPropertyName("bagslot")]
        public int BagSlot { get; set; }

        /// <summary>
        /// Gets or sets the count property.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the durability of the object.
        /// </summary>
        /// <value>
        /// The durability of the object.
        /// </value>
        [JsonPropertyName("curDurability")]
        public int Durability { get; set; }

        /// <summary>
        /// Gets or sets the equipment location.
        /// </summary>
        [JsonPropertyName("equiplocation")]
        public string EquipLocation { get; set; }

        /// <summary>
        /// Gets or sets the equip slot for the object.
        /// </summary>
        /// <value>
        /// The equip slot value. Default value is -1.
        /// </value>
        [JsonPropertyName("equipslot")]
        public int EquipSlot { get; set; } = -1;

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Specifies the JSON property name for the "ItemLevel" property.
        /// </summary>
        [JsonPropertyName("level")]
        public int ItemLevel { get; set; }

        /// <summary>
        /// Gets or sets the link for the item.
        /// </summary>
        [JsonPropertyName("link")]
        public string ItemLink { get; set; }

        /// <summary>
        /// Gets or sets the quality of the item.
        /// </summary>
        [JsonPropertyName("quality")]
        public int ItemQuality { get; set; }

        /// <summary>
        /// Gets or sets the maximum durability of the item.
        /// </summary>
        [JsonPropertyName("maxDurability")]
        public int MaxDurability { get; set; }

        /// <summary>
        /// Gets or sets the maximum stack value.
        /// </summary>
        [JsonPropertyName("maxStack")]
        public int MaxStack { get; set; }

        /// <summary>
        /// Gets or sets the value of the "name" property.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sell price for the item.
        /// </summary>
        /// <value>The sell price.</value>
        [JsonPropertyName("sellprice")]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the required level for the property.
        /// </summary>
        [JsonPropertyName("minLevel")]
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Gets or sets the statistics of the object.
        /// </summary>
        [JsonPropertyName("stats")]
        public Dictionary<string, string> Stats { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the property.
        /// </summary>
        [JsonPropertyName("subtype")]
        public string Subtype { get; set; }

        /// <summary>
        /// Gets or sets the type of the property, as defined by the attribute JsonPropertyName.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Overrides the default ToString() method.
        /// Returns a formatted string representing the Bag item, including its BagId, BagSlot, ItemQuality, Type, Name,
        /// ItemLevel, RequiredLevel, Subtype, and Price.
        /// </summary>
        public override string ToString()
        {
            return $"[{BagId}][{BagSlot}] - [{ItemQuality}][{Type}] {Name} (ilvl. {ItemLevel} | lvl.{RequiredLevel} | {Subtype} | {Price})";
        }
    }
}