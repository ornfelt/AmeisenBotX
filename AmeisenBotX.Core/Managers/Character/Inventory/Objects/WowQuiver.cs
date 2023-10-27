namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WowQuiver"/> class.
    /// </summary>
    /// <param name="wowBasicItem">The <see cref="IWowInventoryItem"/> to initialize the quiver with.</param>
    public class WowQuiver : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WowQuiver"/> class.
        /// </summary>
        /// <param name="wowBasicItem">The <see cref="IWowInventoryItem"/> to initialize the quiver with.</param>
        public WowQuiver(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}