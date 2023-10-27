namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowContainer class with the specified wowBasicItem.
    /// </summary>
    public class WowContainer : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowContainer class with the specified wowBasicItem.
        /// </summary>
        public WowContainer(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}