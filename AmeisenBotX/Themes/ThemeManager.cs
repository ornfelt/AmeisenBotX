using AmeisenBotX.Wow.Objects.Enums; // Assuming this is where CombatClass is
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AmeisenBotX.Themes
{
    public static class ThemeManager
    {
        // Standard WoW Class Colors
        private static readonly Dictionary<WowClass, Color> ClassColors = new()
        {
            { WowClass.Warrior,      (Color)ColorConverter.ConvertFromString("#C79C6E") },
            { WowClass.Paladin,      (Color)ColorConverter.ConvertFromString("#F58CBA") },
            { WowClass.Hunter,       (Color)ColorConverter.ConvertFromString("#ABD473") },
            { WowClass.Rogue,        (Color)ColorConverter.ConvertFromString("#FFF569") },
            { WowClass.Priest,       (Color)ColorConverter.ConvertFromString("#FFFFFF") },
            { WowClass.Deathknight,  (Color)ColorConverter.ConvertFromString("#C41F3B") },
            { WowClass.Shaman,       (Color)ColorConverter.ConvertFromString("#0070DE") },
            { WowClass.Mage,         (Color)ColorConverter.ConvertFromString("#69CCF0") },
            { WowClass.Warlock,      (Color)ColorConverter.ConvertFromString("#9482C9") },
            { WowClass.Druid,        (Color)ColorConverter.ConvertFromString("#FF7D0A") },
        };

        public static void ApplyClassTheme(WowClass combatClass)
        {
            if (ClassColors.TryGetValue(combatClass, out Color accentColor))
            {
                SetAccentColor(accentColor);
            }
        }

        public static void SetAccentColor(Color color)
        {
            // Update the main Accent color resource
            Application.Current.Resources["Accent"] = color;

            // Update SolidColorBrushes
            SolidColorBrush accentBrush = new(color);
            accentBrush.Freeze();
            Application.Current.Resources["AccentBrush"] = accentBrush;
            Application.Current.Resources["AccentHoverBrush"] = new SolidColorBrush(ChangeColorBrightness(color, 0.2f));
            Application.Current.Resources["AccentPressedBrush"] = new SolidColorBrush(ChangeColorBrightness(color, -0.2f));

            // Muted/Glow variants
            Color muted = color;
            muted.A = 153; // 60%
            Application.Current.Resources["AccentMutedBrush"] = new SolidColorBrush(muted);

            Color glow = color;
            glow.A = 64; // 25%
            Application.Current.Resources["AccentGlow"] = glow;
            Application.Current.Resources["AccentGlowBrush"] = new SolidColorBrush(glow);

            Color subtle = color;
            subtle.A = 26; // 10%
            Application.Current.Resources["AccentSubtleBrush"] = new SolidColorBrush(subtle);

            // Legacy Support
            Application.Current.Resources["DarkAccent1"] = color;
            Application.Current.Resources["TextAccentColor"] = color;

            // Update Gradient
            Color gradientStart = ChangeColorBrightness(color, 0.2f);
            Color gradientEnd = color;
            LinearGradientBrush accentGradient = new(gradientStart, gradientEnd, new Point(0, 0), new Point(1, 1));
            accentGradient.Freeze();
            Application.Current.Resources["AccentGradientBrush"] = accentGradient;

            // Update Radial Glow
            RadialGradientBrush radialGlow = new()
            {
                GradientOrigin = new Point(0.5, 0.5),
                Center = new Point(0.5, 0.5)
            };
            radialGlow.GradientStops.Add(new GradientStop(glow, 0.0));
            radialGlow.GradientStops.Add(new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1.0));
            radialGlow.Freeze();
            Application.Current.Resources["AccentGlowRadialBrush"] = radialGlow;
        }

        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = ((255 - red) * correctionFactor) + red;
                green = ((255 - green) * correctionFactor) + green;
                blue = ((255 - blue) * correctionFactor) + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
