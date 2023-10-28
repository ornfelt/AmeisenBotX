/// <summary>
/// Represents a gem item in the character's inventory.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WowGem"/> class with the specified <see cref="IWowInventoryItem"/>.
    /// </summary>
    public class WowGem : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WowGem"/> class with the specified <see cref="IWowInventoryItem"/>.
        /// </summary>
        public WowGem(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}