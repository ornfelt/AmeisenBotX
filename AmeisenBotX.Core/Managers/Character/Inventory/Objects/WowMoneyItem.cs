/// <summary>
/// Represents a WoW money item in the character's inventory.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowMoneyItem class with the specified WoW inventory item.
    /// </summary>
    public class WowMoneyItem : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowMoneyItem class with the specified WoW inventory item.
        /// </summary>
        public WowMoneyItem(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}