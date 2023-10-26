namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    public class WowKey : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowKey class, based on a specified WowInventoryItem.
        /// </summary>
        public WowKey(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}