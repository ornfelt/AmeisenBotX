/// <summary>
/// Represents a trade good item in the character's inventory.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowTradeGoods class with the specified IWowInventoryItem.
    /// </summary>
    public class WowTradeGoods : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowTradeGoods class with the specified IWowInventoryItem.
        /// </summary>
        public WowTradeGoods(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}