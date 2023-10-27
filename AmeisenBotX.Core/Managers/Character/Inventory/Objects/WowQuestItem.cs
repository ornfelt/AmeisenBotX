namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowQuestItem class, using the provided WowInventoryItem object as the base item.
    /// </summary>
    public class WowQuestItem : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowQuestItem class, using the provided WowInventoryItem object as the base item.
        /// </summary>
        public WowQuestItem(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}