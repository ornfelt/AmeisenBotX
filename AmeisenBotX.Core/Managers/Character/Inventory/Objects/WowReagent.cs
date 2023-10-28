/// <summary>
/// Represents a reagent item in the character's inventory.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Initializes a new instance of the WowReagent class for a specified WowBasicItem.
    /// </summary>
    public class WowReagent : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowReagent class for a specified WowBasicItem.
        /// </summary>
        public WowReagent(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
        }
    }
}