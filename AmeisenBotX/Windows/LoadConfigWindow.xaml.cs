using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Classes;
using AmeisenBotX.Models;
using AmeisenBotX.Views;
using AmeisenBotX.Views.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.IO.Path;

namespace AmeisenBotX.Windows
{
    public partial class LoadConfigWindow : Window
    {
        private List<BotProfile> allProfiles = [];
        private string currentSearchFilter = "";

        public LoadConfigWindow()
        {
            ConfigToLoad = string.Empty;
            InitializeComponent();
        }

        public string ConfigToLoad { get; set; }

        private string DataPath { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AmeisenBotX\\profiles\\";

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check for older data folder
            string oldDataPath = $"{Directory.GetCurrentDirectory()}\\data\\";

            if (Directory.Exists(oldDataPath))
            {
                MessageBox.Show(
                    $"You need to move the content of your \"\\\\data\\\\\" folder to \"{DataPath}\". Otherwise your profiles may not be displayed.",
                    "New Data Location",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            IOUtils.CreateDirectoryIfNotExists(DataPath);
            LoadAllProfiles();
            RenderProfiles();

            LoadAllProfiles();
            RenderProfiles();
        }

        private void LoadAllProfiles()
        {
            string[] directories = Directory.GetDirectories(DataPath);
            allProfiles = directories
                .Select(BotProfile.FromDirectory)
                .ToList();

            // Update profile count
            int count = allProfiles.Count;
            ProfileCountText.Text = count == 1 ? "1 profile" : $"{count} profiles";
        }

        private void RenderProfiles()
        {
            GroupedContainer.Children.Clear();

            // Filter profiles based on search
            List<BotProfile> filteredProfiles = string.IsNullOrWhiteSpace(currentSearchFilter)
                ? allProfiles
                : allProfiles.Where(p => MatchesSearch(p, currentSearchFilter)).ToList();

            if (allProfiles.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                NoResultsState.Visibility = Visibility.Collapsed;
                AddNewConfigSection();
                return;
            }

            if (filteredProfiles.Count == 0)
            {
                EmptyState.Visibility = Visibility.Collapsed;
                NoResultsState.Visibility = Visibility.Visible;
                return;
            }

            EmptyState.Visibility = Visibility.Collapsed;
            NoResultsState.Visibility = Visibility.Collapsed;

            // Group profiles by realm
            IOrderedEnumerable<IGrouping<string, BotProfile>> grouped = filteredProfiles
                .GroupBy(p => p.Stats?.Realm ?? "Unknown Server")
                .OrderBy(g => g.Key == "Unknown Server" ? 1 : 0)
                .ThenBy(g => g.Key);

            // Add each realm group
            foreach (IGrouping<string, BotProfile> group in grouped)
            {
                AddRealmGroup(group.Key, group.Count(), group.OrderBy(p => p.Stats?.CharacterName ?? p.Name).ToList());
            }

            // Add "New Config" section last
            AddNewConfigSection();
        }

        private bool MatchesSearch(BotProfile profile, string search)
        {
            search = search.ToLowerInvariant();

            // Match against name, character name, class, realm, zone
            if (profile.Name.ToLowerInvariant().Contains(search))
            {
                return true;
            }

            if (profile.Stats != null)
            {
                if (profile.Stats.CharacterName?.ToLowerInvariant().Contains(search) == true)
                {
                    return true;
                }

                if (profile.Stats.Class?.ToLowerInvariant().Contains(search) == true)
                {
                    return true;
                }

                if (profile.Stats.Realm?.ToLowerInvariant().Contains(search) == true)
                {
                    return true;
                }

                if (profile.Stats.Zone?.ToLowerInvariant().Contains(search) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddNewConfigSection()
        {
            // Header for new config section
            TextBlock header = new()
            {
                Text = "➕ Create New",
                Style = (Style)FindResource("RealmHeaderStyle")
            };
            GroupedContainer.Children.Add(header);

            // Separator
            Border separator = new()
            {
                Height = 1,
                Margin = new Thickness(12, 0, 12, 8),
                Background = new SolidColorBrush(Color.FromRgb(64, 64, 64))
            };
            GroupedContainer.Children.Add(separator);

            // Wrap panel for the New Config card
            WrapPanel panel = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(4, 0, 4, 8)
            };

            CharacterCard newConfigCard = CreateCard(BotProfile.CreateNewConfigPlaceholder());
            panel.Children.Add(newConfigCard);

            GroupedContainer.Children.Add(panel);
        }

        private void AddRealmGroup(string realmName, int count, List<BotProfile> profiles)
        {
            // Realm header with count
            TextBlock header = new()
            {
                Text = $"📍 {realmName} ({count})",
                Style = (Style)FindResource("RealmHeaderStyle")
            };
            GroupedContainer.Children.Add(header);

            // Separator line
            Border separator = new()
            {
                Height = 1,
                Margin = new Thickness(12, 0, 12, 8),
                Background = new SolidColorBrush(Color.FromRgb(64, 64, 64))
            };
            GroupedContainer.Children.Add(separator);

            // Wrap panel containing cards
            WrapPanel panel = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(4, 0, 4, 0)
            };

            foreach (BotProfile profile in profiles)
            {
                CharacterCard card = CreateCard(profile);
                panel.Children.Add(card);
            }

            GroupedContainer.Children.Add(panel);
        }

        private CharacterCard CreateCard(BotProfile profile)
        {
            CharacterCard card = new();
            card.SetProfile(profile);
            card.CardClicked += OnCardClicked;
            card.DeleteRequested += OnDeleteRequested;
            // Note: Edit config requires running bot instance, handled in card's Open Folder
            return card;
        }

        private void OnCardClicked(object sender, BotProfile profile)
        {
            if (profile.IsNewConfig)
            {
                CreateNewConfig();
            }
            else
            {
                LaunchProfile(profile);
            }
        }

        private void OnDeleteRequested(object sender, BotProfile profile)
        {
            if (profile == null || profile.IsNewConfig)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the profile \"{profile.Stats?.CharacterName ?? profile.Name}\"?\n\nThis will permanently delete the profile folder and all its contents.",
                "Delete Profile",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string folder = Path.GetDirectoryName(profile.ConfigPath);
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true);
                        LoadAllProfiles();
                        RenderProfiles();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearchFilter = SearchBox.Text?.Trim() ?? "";
            RenderProfiles();
        }

        private void CreateNewConfig()
        {
            // Use the new Create Bot Window
            // We need to access combat classes. Assuming AmeisenBot.CombatClasses is static/available.
            // If not available here (because bot isn't started), we might need to load them or pass empty.
            // However, AmeisenBot.CombatClasses depends on InitCombatClasses which runs on bot start.
            // Since this is the launcher, the bot core might not be fully initialized?
            // Checking AmeisenBot.cs... InitCombatClasses() is static?
            // Actually, LoadConfigWindow is the entry point. The Bot instance might not exist yet.
            // But we can check if we can access the classes statically.

            // Correction: AmeisenBot.CombatClasses is a static property populated by InitCombatClasses.
            // We should ensure InitCombatClasses is called or available. 
            // AmeisenBot.InitCombatClasses() is public static.

            // We don't initialize the full bot here, just get the classes for UI (using new static method)

            // Use the new static method to get classes without a full bot instance
            IEnumerable<CombatClassDescriptor> combatClasses = AmeisenBotX.Core.AmeisenBot.GetAvailableCombatClassDescriptors();

            // Try to find the latest profile to use as a template
            Core.AmeisenBotConfig templateConfig = null;
            if (allProfiles != null && allProfiles.Any())
            {
                BotProfile latestProfile = allProfiles
                    .OrderByDescending(p => p.Stats?.LastPlayed ?? Directory.GetLastWriteTime(p.ConfigPath))
                    .FirstOrDefault();

                if (latestProfile != null && File.Exists(latestProfile.ConfigPath))
                {
                    try
                    {
                        string json = File.ReadAllText(latestProfile.ConfigPath);
                        templateConfig = JsonSerializer.Deserialize<Core.AmeisenBotConfig>(json, new JsonSerializerOptions
                        {
                            AllowTrailingCommas = true,
                            NumberHandling = JsonNumberHandling.AllowReadingFromString,
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch { /* Ignore template load errors */ }
                }
            }

            CreateBotWindow createWindow = new(combatClasses, "", templateConfig);

            if (createWindow.ShowDialog() == true)
            {
                string configPath = Path.Combine(DataPath, createWindow.BotName, "config.json");
                IOUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(configPath));

                File.WriteAllText(configPath, JsonSerializer.Serialize(createWindow.CreatedConfig, new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }));

                LaunchProfile(BotProfile.FromDirectory(Path.GetDirectoryName(configPath)));
            }
        }

        private void LaunchProfile(BotProfile profile)
        {
            ConfigToLoad = profile.ConfigPath;
            Hide();

            MainWindow mainWindow = new(DataPath, ConfigToLoad);
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            Close();
        }


    }
}