/// <summary>
/// Represents a consumable item in the character's inventory.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowConsumable class with the specified WowInventoryItem object as a base.
    /// </summary>
    public class WowConsumable : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowConsumable class with the specified WowInventoryItem object as a base.
        /// </summary>
        public WowConsumable(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}