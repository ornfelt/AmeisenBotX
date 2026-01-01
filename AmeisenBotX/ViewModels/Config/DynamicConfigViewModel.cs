using AmeisenBotX.Core;
using AmeisenBotX.Core.Engines.Combat.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace AmeisenBotX.ViewModels.Config
{
    public class DynamicConfigViewModel : INotifyPropertyChanged
    {
        private readonly AmeisenBotConfig _config;
        private readonly System.Collections.Generic.IEnumerable<AmeisenBotX.Core.Engines.Combat.Classes.ICombatClass> _combatClasses;
        private string _searchText;

        public DynamicConfigViewModel(AmeisenBotConfig config, System.Collections.Generic.IEnumerable<AmeisenBotX.Core.Engines.Combat.Classes.ICombatClass> combatClasses)
        {
            _config = config;
            _combatClasses = combatClasses;
            Properties = [];

            // Build property list with pre-computed sorting
            List<ConfigPropertyViewModel> propertyList = [];

            foreach (PropertyInfo prop in typeof(AmeisenBotConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() != null)
                {
                    continue;
                }

                if (!prop.CanWrite)
                {
                    continue;
                }

                ConfigPropertyViewModel viewModel = CreatePropertyViewModel(prop);
                if (viewModel == null)
                {
                    continue;
                }

                // Skip "Map" category
                string rawCategory = viewModel.Category ?? "";
                if (rawCategory.Equals("Map", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Apply category order and emoji
                viewModel.CategoryOrder = CategoryPriorities.TryGetValue(rawCategory, out int order) ? order : 999;
                if (CategoryEmojis.TryGetValue(rawCategory, out string emoji))
                {
                    viewModel.Category = $"{emoji} {rawCategory}";
                }

                // Apply slider heuristics for numeric properties
                ApplySliderHeuristics(viewModel, prop);

                propertyList.Add(viewModel);
            }

            // Pre-sort: by category order, then by name
            foreach (ConfigPropertyViewModel vm in propertyList.OrderBy(p => p.CategoryOrder).ThenBy(p => p.Name))
            {
                Properties.Add(vm);
            }

            // Setup CollectionView for grouping and filtering (no sorting needed - already sorted)
            PropertiesView = CollectionViewSource.GetDefaultView(Properties);
            PropertiesView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ConfigPropertyViewModel.Category)));
            PropertiesView.Filter = FilterProperties;
        }

        private ConfigPropertyViewModel CreatePropertyViewModel(PropertyInfo prop)
        {
            return prop.Name == "BuiltInCombatClassName"
                ? new ObjectSelectionConfigProperty(_config, prop, GetCombatClasses())
                : (prop.Name.Contains("Portrait") || prop.Name.Contains("Image")) && prop.PropertyType == typeof(string)
                ? new ImageConfigProperty(_config, prop)
                : prop.Name == "PathToWowExe" || prop.Name.EndsWith("Profile") || prop.Name.EndsWith("File")
                ? new FileConfigProperty(_config, prop)
                : prop.GetCustomAttribute<PasswordPropertyTextAttribute>()?.Password == true
                ? new PasswordConfigProperty(_config, prop)
                : prop.PropertyType switch
                {
                    Type t when t == typeof(bool) => new BoolConfigProperty(_config, prop),
                    Type t when t == typeof(string) => new StringConfigProperty(_config, prop),
                    Type t when t == typeof(int) => new IntConfigProperty(_config, prop),
                    Type t when t == typeof(float) => new FloatConfigProperty(_config, prop),
                    Type t when t == typeof(double) => new DoubleConfigProperty(_config, prop),
                    Type t when t.IsEnum => new EnumConfigProperty(_config, prop),
                    _ => null
                };
        }

        private static void ApplySliderHeuristics(ConfigPropertyViewModel viewModel, PropertyInfo prop)
        {
            if (viewModel is not (IntConfigProperty or FloatConfigProperty or DoubleConfigProperty))
            {
                return;
            }

            if (prop.Name.Contains("Percent") || prop.Name.Contains("Threshold"))
            {
                viewModel.UseSlider = true;
                viewModel.Minimum = 0;
                viewModel.Maximum = 100;
            }
            else if (prop.Name.Contains("Distance") || prop.Name.Contains("Range") || prop.Name.Contains("Radius"))
            {
                viewModel.UseSlider = true;
                viewModel.Minimum = 0;
                viewModel.Maximum = prop.Name.Contains("Merchant") || prop.Name.Contains("Repair") ? 500 : 100;
            }
            else if (prop.Name.Contains("Fps"))
            {
                viewModel.UseSlider = true;
                viewModel.Minimum = 10;
                viewModel.Maximum = 144;
            }
            else if (prop.Name.Contains("Slots"))
            {
                viewModel.UseSlider = true;
                viewModel.Minimum = 0;
                viewModel.Maximum = 20;
            }
        }

        // Category priorities: WoW gameplay flow first, bot/technical last
        private static readonly System.Collections.Generic.Dictionary<string, int> CategoryPriorities = new()
        {
            // === Character & Core Gameplay ===
            { "General", 0 },           // Basic character settings
            { "Combat", 1 },            // How your character fights
            { "Regeneration", 2 },      // Eating/drinking
            { "Consumables", 3 },       // Potions/food
            
            // === Activities ===
            { "Questing", 10 },         // Quest automation
            { "Professions", 11 },      // Gathering/crafting
            { "Productivity", 12 },     // Mail/AH automation
            { "Dungeon", 13 },          // Dungeon settings
            { "PvP", 14 },              // Battleground/arena
            
            // === Social ===
            { "Party", 20 },            // Group settings
            { "Social", 21 },           // Guild/friends
            { "Chat", 22 },             // Chat automation
            
            // === World ===
            { "Navigation", 30 },       // Pathfinding
            { "Movement", 31 },         // Movement settings
            { "Inventory", 32 },        // Bag/vendor management
            { "Looting", 33 },          // What to loot
            { "Mounts", 34 },           // Mount usage
            
            // === Bot Configuration (Advanced) ===
            { "Login", 80 },            // Auto-login
            { "Profiles", 81 },         // Profile paths
            { "Execution", 82 },        // Bot execution settings
            { "Performance", 83 },      // FPS/throttling
            
            // === Developer/Debug ===
            { "Debug", 90 },            // Debug options
            { "AI", 91 },               // AI settings
            { "Map", 92 },              // Map overlay (hidden)
            { "Remote Control", 99 }    // Remote control
        };

        private static readonly System.Collections.Generic.Dictionary<string, string> CategoryEmojis = new()
        {
            // Character & Core
            { "General", "⚙️" },
            { "Combat", "⚔️" },
            { "Regeneration", "🍖" },
            { "Consumables", "🧪" },
            
            // Activities
            { "Questing", "📜" },
            { "Professions", "⛏️" },
            { "Productivity", "📬" },
            { "Dungeon", "🏰" },
            { "PvP", "🗡️" },
            
            // Social
            { "Party", "👥" },
            { "Social", "💬" },
            { "Chat", "🗨️" },
            
            // World
            { "Navigation", "🧭" },
            { "Movement", "🏃" },
            { "Inventory", "🎒" },
            { "Looting", "💰" },
            { "Mounts", "🐎" },
            
            // Bot Config
            { "Login", "🔐" },
            { "Profiles", "📁" },
            { "Execution", "▶️" },
            { "Performance", "🚀" },
            
            // Debug
            { "Debug", "🐞" },
            { "AI", "🧠" },
            { "Map", "🗺️" },
            { "Remote Control", "📱" }
        };

        private System.Collections.Generic.IEnumerable<IConfigItem> GetCombatClasses()
        {
            List<IConfigItem> list =
            [
                new CombatClassDisplayWrapper(null) // "None" option
            ];

            if (_combatClasses != null)
            {
                foreach (ICombatClass cc in _combatClasses)
                {
                    list.Add(new CombatClassDisplayWrapper(cc));
                }
            }

            return list.OrderBy(x => x.DisplayName).ToList();
        }

        private class CombatClassDisplayWrapper : IConfigItem
        {
            private readonly AmeisenBotX.Core.Engines.Combat.Classes.ICombatClass _instance;

            public CombatClassDisplayWrapper(AmeisenBotX.Core.Engines.Combat.Classes.ICombatClass instance)
            {
                _instance = instance;
            }

            public string DisplayName
            {
                get
                {
                    return _instance == null ? "None" : $"{_instance.DisplayName} ({_instance.Author})";
                }
            }

            public string ConfigValue
            {
                get
                {
                    if (_instance == null)
                    {
                        return string.Empty;
                    }
                    // Use Type Name (e.g. "PaladinHoly") as the stable ID for config.
                    // LoadClassByName has been updated to support this match.
                    return _instance.GetType().Name;
                }
            }

            public override string ToString() => DisplayName;
        }

        public ObservableCollection<ConfigPropertyViewModel> Properties { get; }

        public ICollectionView PropertiesView { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                PropertiesView.Refresh();
            }
        }

        private bool FilterProperties(object obj)
        {
            return string.IsNullOrWhiteSpace(SearchText) || (obj is ConfigPropertyViewModel vm && (vm.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       vm.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       vm.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
