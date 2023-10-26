using AmeisenBotX.Wow.Objects.Enums;

using System.Drawing;

namespace AmeisenBotX.Utils
{
    public static class WowColorsDrawing
    {
        /// <summary>
        /// Represents a solid brush with a dark primary color.
        /// </summary>
        public static readonly SolidBrush dkPrimaryBrush = new(Color.FromArgb(255, 196, 30, 59));
        /// <summary>
        /// Represents a static read-only SolidBrush with a secondary color of deep sky blue.
        /// </summary>
        public static readonly SolidBrush dkSecondaryBrush = new(Color.FromArgb(255, 0, 209, 255));

        /// <summary>
        /// Represents a solid brush used for a druid's primary color.
        /// </summary>
        public static readonly SolidBrush druidPrimaryBrush = new(Color.FromArgb(255, 255, 125, 10));
        /// <summary>
        /// A static readonly SolidBrush object for the secondary color of a druid.
        /// </summary>
        public static readonly SolidBrush druidSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// Represents the SolidBrush object used for the hunter's primary color.
        /// </summary>
        public static readonly SolidBrush hunterPrimaryBrush = new(Color.FromArgb(255, 171, 212, 115));
        /// <summary>
        /// A static readonly SolidBrush used as the secondary color for the hunter.
        /// </summary>
        public static readonly SolidBrush hunterSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// Represents a solid brush used for the primary color of a mage.
        /// </summary>
        public static readonly SolidBrush magePrimaryBrush = new(Color.FromArgb(255, 105, 204, 240));
        /// <summary>
        /// Represents a solid brush used for the secondary color of a mage.
        /// </summary>
        public static readonly SolidBrush mageSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// Represents the primary brush used for the paladin character in the game.
        /// </summary>
        public static readonly SolidBrush paladinPrimaryBrush = new(Color.FromArgb(255, 245, 140, 186));
        /// <summary>
        /// Represents the secondary brush color for a paladin.
        /// </summary>
        public static readonly SolidBrush paladinSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// Represents the solid brush used for the priest's primary color.
        /// </summary>
        public static readonly SolidBrush priestPrimaryBrush = new(Color.FromArgb(255, 255, 255, 255));
        /// <summary>
        /// Represents a solid brush used for the secondary color of a priest.
        /// </summary>
        public static readonly SolidBrush priestSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// Represents a solid brush with the Rogue primary color.
        /// </summary>
        public static readonly SolidBrush roguePrimaryBrush = new(Color.FromArgb(255, 255, 245, 105));
        /// <summary>
        /// Represents a static readonly SolidBrush object for the secondary color of a rogue element.
        /// The color is set to fully opaque yellow (255, 255, 255, 0).
        /// </summary>
        public static readonly SolidBrush rogueSecondaryBrush = new(Color.FromArgb(255, 255, 255, 0));

        /// <summary>
        /// Represents the solid brush used for the primary color of the shaman.
        /// </summary>
        public static readonly SolidBrush shamanPrimaryBrush = new(Color.FromArgb(255, 0, 112, 222));
        /// <summary>
        /// Represents a static, read-only SolidBrush object with a secondary shaman color.
        /// </summary>
        public static readonly SolidBrush shamanSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// The solid brush with an unknown color.
        /// </summary>
        public static readonly SolidBrush unknownBrush = new(Color.FromArgb(255, 255, 255, 255));
        /// <summary>
        /// Represents a static readonly SolidBrush for the primary color used by the warlock.
        /// </summary>
        public static readonly SolidBrush warlockPrimaryBrush = new(Color.FromArgb(255, 148, 130, 201));
        /// <summary>
        /// Represents a static, read-only SolidBrush object for the Warlock secondary color.
        /// The color is defined as fully opaque with a blue component.
        /// </summary>
        public static readonly SolidBrush warlockSecondaryBrush = new(Color.FromArgb(255, 0, 0, 255));

        /// <summary>
        /// Represents the primary color brush used for the warrior character.
        /// </summary>
        public static readonly SolidBrush warriorPrimaryBrush = new(Color.FromArgb(255, 199, 156, 110));
        /// <summary>
        /// The secondary brush color used for the warrior character.
        /// </summary>
        public static readonly SolidBrush warriorSecondaryBrush = new(Color.FromArgb(255, 255, 0, 0));

        /// <summary>
        /// Returns the corresponding SolidBrush for a given WowClass.
        /// </summary>
        /// <param name="wowClass">The WowClass to get the primary brush for.</param>
        /// <returns>The SolidBrush associated with the WowClass. If the WowClass is not recognized, returns the unknown brush.</returns>
        public static SolidBrush GetClassPrimaryBrush(WowClass wowClass)
        {
            return wowClass switch
            {
                WowClass.Deathknight => dkPrimaryBrush,
                WowClass.Druid => druidPrimaryBrush,
                WowClass.Hunter => hunterPrimaryBrush,
                WowClass.Mage => magePrimaryBrush,
                WowClass.Paladin => paladinPrimaryBrush,
                WowClass.Priest => priestPrimaryBrush,
                WowClass.Rogue => roguePrimaryBrush,
                WowClass.Shaman => shamanPrimaryBrush,
                WowClass.Warlock => warlockPrimaryBrush,
                WowClass.Warrior => warriorPrimaryBrush,
                _ => unknownBrush,
            };
        }
    }
}