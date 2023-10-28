/// <summary>
/// Namespace for managing miscellaneous items in the character's inventory.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowMiscellaneousItem class using the specified IWowInventoryItem object as the base item.
    /// </summary>
    public class WowMiscellaneousItem : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowMiscellaneousItem class using the specified IWowInventoryItem object as the base item.
        /// </summary>
        public WowMiscellaneousItem(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}