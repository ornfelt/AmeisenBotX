using AmeisenBotX.Wow.Objects.Enums;
using System.Windows.Media;

/// <summary>
/// Contains static, read-only brushes that are used as colors for DK.
/// </summary>
namespace AmeisenBotX.Utils
{
    /// <summary>
    /// Represents a static, read-only brush that is used as the secondary color for DK.
    /// </summary>
    public static class WowColors
    {
        /// <summary>
        /// Represents a static, read-only brush that is used as the primary color for DK.
        /// </summary>
        public static readonly Brush dkPrimaryBrush = new SolidColorBrush(Color.FromRgb(196, 30, 59));
        /// <summary>
        /// Represents a static readonly brush for the secondary color with the RGB values of (0, 209, 255).
        /// </summary>
        public static readonly Brush dkSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 209, 255));

        /// <summary>
        /// Represents the primary brush for a druid.
        /// </summary>
        public static readonly Brush druidPrimaryBrush = new SolidColorBrush(Color.FromRgb(255, 125, 10));
        /// <summary>
        /// The brush used for the secondary color of the Druid.
        /// </summary>
        public static readonly Brush druidSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// Represents the primary brush color for the Hunter class.
        /// </summary>
        public static readonly Brush hunterPrimaryBrush = new SolidColorBrush(Color.FromRgb(171, 212, 115));
        /// <summary>
        /// This is a public static readonly field that represents the brush used for the secondary color of the hunter.
        /// It is a SolidColorBrush that has a color with RGB values of 0 (red), 0 (green), and 255 (blue).
        /// </summary>
        public static readonly Brush hunterSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// The primary brush used for the mage character.
        /// </summary>
        public static readonly Brush magePrimaryBrush = new SolidColorBrush(Color.FromRgb(105, 204, 240));
        /// <summary>
        /// Defines the brush object to be used as the secondary brush for a mage character.
        /// The brush is created with a blue color. 
        /// </summary>
        public static readonly Brush mageSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// Represents the primary Brush used for the paladin class, with an RGB color value of (245, 140, 186).
        /// </summary>
        public static readonly Brush paladinPrimaryBrush = new SolidColorBrush(Color.FromRgb(245, 140, 186));
        /// <summary>
        /// A static readonly Brush used as the secondary color for the paladin.
        /// </summary>
        public static readonly Brush paladinSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// The primary brush used for the priest's appearance.
        /// </summary>
        public static readonly Brush priestPrimaryBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        /// <summary>
        /// Represents the secondary brush color used for the priest. 
        /// The color is a solid brush with an RGB value of (0, 0, 255).
        /// </summary>
        public static readonly Brush priestSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// The brush used for the rogue primary color.
        /// </summary>
        public static readonly Brush roguePrimaryBrush = new SolidColorBrush(Color.FromRgb(255, 245, 105));
        /// <summary>
        /// Represents a secondary brush for the rogue character.
        /// </summary>
        public static readonly Brush rogueSecondaryBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));

        /// <summary>
        /// Represents the primary brush used by the shaman.
        /// </summary>
        public static readonly Brush shamanPrimaryBrush = new SolidColorBrush(Color.FromRgb(0, 112, 222));
        /// <summary>
        /// Represents the secondary brush used by the shaman.
        /// </summary>
        public static readonly Brush shamanSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// The brush used for the primary color of a warlock.
        /// </summary>
        public static readonly Brush warlockPrimaryBrush = new SolidColorBrush(Color.FromRgb(148, 130, 201));
        /// <summary>
        /// A readonly <see cref="Brush"/> used as the secondary color for warlocks, with an RGB value of (0, 0, 255).
        /// </summary>
        public static readonly Brush warlockSecondaryBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        /// <summary>
        /// Represents the primary brush used for the warrior class.
        /// </summary>
        public static readonly Brush warriorPrimaryBrush = new SolidColorBrush(Color.FromRgb(199, 156, 110));
        /// <summary>
        /// Gets a read-only brush that represents the secondary color for a warrior. The brush is set to a solid color brush using the RGB value (255, 0, 0), creating a red color.
        /// </summary>
        public static readonly Brush warriorSecondaryBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        /// <summary>
        /// Returns the primary brush associated with the specified WoW class.
        /// </summary>
        /// <param name="wowClass">The WoW class.</param>
        /// <returns>The primary brush for the specified WoW class.</returns>
        public static Brush GetClassPrimaryBrush(WowClass wowClass)
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
                _ => new SolidColorBrush(Colors.White),
            };
        }
    }
}