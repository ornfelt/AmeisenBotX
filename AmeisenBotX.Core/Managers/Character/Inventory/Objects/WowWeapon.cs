using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Represents a weapon in the game World of Warcraft.
    /// </summary>
    public class WowWeapon : WowBasicItem
    {
        /// <summary>
        /// Initializes a new instance of the WowWeapon class.
        /// </summary>
        /// <param name="wowBasicItem">The WowInventoryItem used to create the weapon.</param>
        public WowWeapon(IWowInventoryItem wowBasicItem) : base(wowBasicItem)
        {
            WeaponType = Enum.TryParse(GetWeaponTypeName(Subtype.ToLowerInvariant()), true, out WowWeaponType weaponType)
                ? weaponType : WowWeaponType.Misc;
        }

        /// <summary>
        /// Gets or sets the weapon type of the WowWeaponType.
        /// </summary>
        public WowWeaponType WeaponType { get; set; }

        /// <summary>
        /// Returns the weapon type name based on the given subType.
        /// </summary>
        /// <param name="subType">The subType of the weapon.</param>
        /// <returns>The weapon type name.</returns>
        private static string GetWeaponTypeName(string subType)
        {
            if (subType.StartsWith("Main Hand"))
            {
                subType = subType.Replace("Main Hand", "");

                if (subType.EndsWith("s"))
                {
                    subType = subType.Remove(subType.Length - 1);
                }

                return subType;
            }

            if (subType.StartsWith("Off Hand"))
            {
                subType = subType.Replace("Off Hand", "");

                if (subType.EndsWith("s"))
                {
                    subType = subType.Remove(subType.Length - 1);
                }

                return subType;
            }

            if (subType.StartsWith("One-Handed"))
            {
                subType = subType.Replace("One-Handed", "");

                if (subType.EndsWith("s"))
                {
                    subType = subType.Remove(subType.Length - 1);
                }

                return subType;
            }

            if (subType.StartsWith("One-Hand"))
            {
                subType = subType.Replace("One-Hand", "");

                if (subType.EndsWith("s"))
                {
                    subType = subType.Remove(subType.Length - 1);
                }

                return subType;
            }

            if (subType.StartsWith("Staves"))
            {
                return "Staff";
            }

            if (subType.Contains('-'))
            {
                string handedness = subType.Replace("-", string.Empty).Split(" ", 2)[0];
                string weaponType = subType.Replace("-", string.Empty).Split(" ", 2)[1];

                if (weaponType.EndsWith("s"))
                {
                    weaponType = weaponType.Remove(weaponType.Length - 1);
                }

                if (handedness.EndsWith("ed"))
                {
                    handedness = handedness.Remove(handedness.Length - 2);
                }

                return weaponType + handedness;
            }

            if (subType.EndsWith("s"))
            {
                return subType.Remove(subType.Length - 1);
            }

            return subType;
        }
    }
}