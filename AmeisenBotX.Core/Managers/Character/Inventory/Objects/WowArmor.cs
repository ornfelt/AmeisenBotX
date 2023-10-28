using AmeisenBotX.Wow.Objects.Enums;
using System;

/// <summary>
/// Represents a type of armor in the game Wow. Inherits from WowBasicItem.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Represents a type of armor in the game Wow. Inherits from WowBasicItem.
    /// </summary>
    public class WowArmor : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowArmor class with the provided
        /// wowBasicItem.
        /// If the subtype of wowBasicItem ends with 's', removes the last character.
        /// Assigns the ArmorType property of the instance based on the parsed
        /// value of the subtype. If parsing fails, assigns WowArmorType.Misc.
        /// </summary>
        public WowArmor(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
            if (Subtype.ToLowerInvariant().EndsWith("s"))
            {
                Subtype = Subtype.Remove(Subtype.Length - 1);
            }

            ArmorType = Enum.TryParse(Subtype, true, out WowArmorType armorType)
                ? armorType : WowArmorType.Misc;
        }

        /// <summary>
        /// Gets the type of armor for the Wow item.
        /// </summary>
        public WowArmorType ArmorType { get; }
    }
}