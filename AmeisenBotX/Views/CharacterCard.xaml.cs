using AmeisenBotX.Models;
using AmeisenBotX.Utils;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AmeisenBotX.Views
{
    /// <summary>
    /// Character card control for displaying a bot profile with portrait.
    /// </summary>
    public partial class CharacterCard : UserControl
    {
        // Faction colors
        private static readonly Color AllianceColor = Color.FromRgb(0, 112, 222);
        private static readonly Color HordeColor = Color.FromRgb(180, 30, 30);

        /// <summary>
        /// Event raised when the card is clicked.
        /// </summary>
        public event EventHandler<BotProfile> CardClicked;

        /// <summary>
        /// Event raised when delete is requested.
        /// </summary>
        public event EventHandler<BotProfile> DeleteRequested;

        /// <summary>
        /// The bot profile this card represents.
        /// </summary>
        public BotProfile Profile { get; private set; }

        public CharacterCard()
        {
            InitializeComponent();
            MouseLeftButtonUp += OnCardClicked;
        }

        /// <summary>
        /// Sets the profile to display on this card.
        /// </summary>
        public void SetProfile(BotProfile profile)
        {
            Profile = profile;

            // Hide context menu for New Config card
            if (profile.IsNewConfig)
            {
                CardBorder.ContextMenu = null;
            }

            if (profile.IsNewConfig)
            {
                ProfileName.Text = profile.Name;
                NewConfigIcon.Visibility = Visibility.Visible;
                PortraitImage.Visibility = Visibility.Collapsed;
                FallbackIcon.Visibility = Visibility.Collapsed;
                PortraitBorder.Visibility = Visibility.Collapsed;
                LevelBadge.Visibility = Visibility.Collapsed;
                StatsPanel.Visibility = Visibility.Collapsed;
                LastPlayedText.Visibility = Visibility.Collapsed;
                FactionIndicator.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (profile.HasStats)
                {
                    ProfileName.Text = profile.Stats.CharacterName;
                    
                    // Show level badge
                    LevelBadge.Visibility = Visibility.Visible;
                    LevelText.Text = profile.Stats.Level.ToString();

                    // Show class with WoW class color
                    string classText = profile.Stats.Class ?? "";
                    string zoneText = profile.Stats.Zone ?? "";
                    
                    if (!string.IsNullOrEmpty(classText))
                    {
                        ClassText.Text = classText;
                        ClassText.Foreground = GetClassBrush(classText);
                        StatsPanel.Visibility = Visibility.Visible;

                        if (!string.IsNullOrEmpty(zoneText))
                        {
                            ZoneSeparator.Visibility = Visibility.Visible;
                            ZoneText.Text = zoneText;
                        }
                    }
                    else if (!string.IsNullOrEmpty(zoneText))
                    {
                        ZoneText.Text = zoneText;
                        StatsPanel.Visibility = Visibility.Visible;
                    }

                    // Show last played
                    string lastPlayed = profile.Stats.GetLastPlayedRelative();
                    if (!string.IsNullOrEmpty(lastPlayed))
                    {
                        LastPlayedText.Text = lastPlayed;
                        LastPlayedText.Visibility = Visibility.Visible;
                    }

                    // Show faction indicator
                    ApplyFactionStyle(profile.Stats.Faction);
                }
                else
                {
                    ProfileName.Text = profile.Name;
                    LevelBadge.Visibility = Visibility.Collapsed;
                    StatsPanel.Visibility = Visibility.Collapsed;
                    LastPlayedText.Visibility = Visibility.Collapsed;
                    FactionIndicator.Visibility = Visibility.Collapsed;
                }

                // Handle portrait
                if (profile.HasPortrait)
                {
                    LoadPortrait(profile.PortraitPath);
                    NewConfigIcon.Visibility = Visibility.Collapsed;
                    PortraitImage.Visibility = Visibility.Visible;
                    FallbackIcon.Visibility = Visibility.Collapsed;
                    PortraitBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    NewConfigIcon.Visibility = Visibility.Collapsed;
                    PortraitImage.Visibility = Visibility.Collapsed;
                    FallbackIcon.Visibility = Visibility.Visible;
                    PortraitBorder.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Applies faction-specific styling to the card.
        /// </summary>
        private void ApplyFactionStyle(string faction)
        {
            if (string.IsNullOrEmpty(faction)) return;

            Color factionColor = faction == "Alliance" ? AllianceColor : HordeColor;

            // Show faction indicator bar at top
            FactionIndicator.Background = new SolidColorBrush(factionColor);
            FactionIndicator.Visibility = Visibility.Visible;

            // Tint portrait border with faction color
            PortraitGradient1.Color = factionColor;
            PortraitGradient2.Color = Color.FromRgb(
                (byte)(factionColor.R * 0.7),
                (byte)(factionColor.G * 0.7),
                (byte)(factionColor.B * 0.7));
        }

        /// <summary>
        /// Gets the WoW class color brush for a class string.
        /// </summary>
        private static Brush GetClassBrush(string className)
        {
            if (Enum.TryParse<WowClass>(className, true, out var wowClass))
            {
                return WowColors.GetClassPrimaryBrush(wowClass);
            }
            return new SolidColorBrush(Color.FromRgb(136, 136, 136));
        }

        private void LoadPortrait(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();

                    PortraitBrush.ImageSource = bitmap;
                }
            }
            catch
            {
                PortraitImage.Visibility = Visibility.Collapsed;
                FallbackIcon.Visibility = Visibility.Visible;
            }
        }

        private void OnCardClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CardClicked?.Invoke(this, Profile);
        }

        private void MenuOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Profile != null && !Profile.IsNewConfig)
            {
                string folder = Path.GetDirectoryName(Profile.ConfigPath);
                if (Directory.Exists(folder))
                {
                    Process.Start("explorer.exe", folder);
                }
            }
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, Profile);
        }
    }
}
