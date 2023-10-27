namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowRecipe class.
    /// </summary>
    /// <param name="wowBasicItem">The basic item used as a base for the recipe.</param>
    public class WowRecipe : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowRecipe class.
        /// </summary>
        /// <param name="wowBasicItem">The basic item used as a base for the recipe.</param>
        public WowRecipe(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}