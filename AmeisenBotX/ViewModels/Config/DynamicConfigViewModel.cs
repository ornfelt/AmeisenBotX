using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using AmeisenBotX.Core;

namespace AmeisenBotX.ViewModels.Config
{
    public class DynamicConfigViewModel : INotifyPropertyChanged
    {
        private readonly AmeisenBotConfig _config;
        private string _searchText;

        public DynamicConfigViewModel(AmeisenBotConfig config)
        {
            _config = config;
            Properties = new ObservableCollection<ConfigPropertyViewModel>();
            
            // Populate properties via Reflection
            foreach (var prop in typeof(AmeisenBotConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Skip attributes that shouldn't be edited
                if (prop.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() != null) continue;
                if (!prop.CanWrite) continue;

                ConfigPropertyViewModel viewModel = null;

                if (prop.Name == "BuiltInCombatClassName")
                {
                    viewModel = new SelectionConfigProperty(_config, prop, GetCombatClasses());
                }
                else if (prop.Name == "PathToWowExe" || prop.Name.EndsWith("Profile") || prop.Name.EndsWith("File"))
                {
                    viewModel = new FileConfigProperty(_config, prop);
                }
                else if (prop.PropertyType == typeof(bool))
                    viewModel = new BoolConfigProperty(_config, prop);
                else if (prop.PropertyType == typeof(string))
                    viewModel = new StringConfigProperty(_config, prop);
                else if (prop.PropertyType == typeof(int))
                    viewModel = new IntConfigProperty(_config, prop);
                else if (prop.PropertyType == typeof(float))
                    viewModel = new FloatConfigProperty(_config, prop);
                else if (prop.PropertyType == typeof(double))
                    viewModel = new DoubleConfigProperty(_config, prop);
                else if (prop.PropertyType.IsEnum)
                    viewModel = new EnumConfigProperty(_config, prop);
                
                // Add more types here (List, etc) if needed
                
                if (viewModel != null)
                {
                    string rawCategory = viewModel.Category ?? "";

                    // Skip "Map" category as per user request
                    if (string.Equals(rawCategory, "Map", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (CategoryPriorities.TryGetValue(rawCategory, out int order))
                    {
                        viewModel.CategoryOrder = order;
                    }
                    else
                    {
                        viewModel.CategoryOrder = 999;
                    }

                    if (CategoryEmojis.TryGetValue(rawCategory, out string emoji))
                    {
                        viewModel.Category = $"{emoji} {rawCategory}";
                    }

                    // HEURISTIC: Enable Slider for recognized numeric patterns
                    if (viewModel is IntConfigProperty || viewModel is FloatConfigProperty || viewModel is DoubleConfigProperty)
                    {
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
                             viewModel.Maximum = 100;
                             
                             // Exceptions for search radii that might be larger
                             if (prop.Name.Contains("Merchant") || prop.Name.Contains("Repair")) 
                             {
                                 viewModel.Maximum = 500;
                             }
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

                    Properties.Add(viewModel);
                }
            }

            // Setup CollectionView for grouping and filtering
            PropertiesView = CollectionViewSource.GetDefaultView(Properties);
            PropertiesView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ConfigPropertyViewModel.Category)));
            PropertiesView.Filter = FilterProperties;
            PropertiesView.SortDescriptions.Add(new SortDescription(nameof(ConfigPropertyViewModel.CategoryOrder), ListSortDirection.Ascending));
            PropertiesView.SortDescriptions.Add(new SortDescription(nameof(ConfigPropertyViewModel.Name), ListSortDirection.Ascending));
        }

        private static readonly System.Collections.Generic.Dictionary<string, int> CategoryPriorities = new()
        {
            { "Login", 0 },
            { "Execution", 1 },
            { "Profiles", 2 },
            { "General", 3 },
            { "Combat", 4 },
            { "Regeneration", 5 },
            { "Consumables", 6 },
            { "Mounts", 7 },
            { "Looting", 8 },
            { "Inventory", 9 },
            { "Navigation", 10 },
            { "Questing", 11 },
            { "Party", 12 },
            { "Dungeon", 13 },
            { "PvP", 14 },
            { "Social", 15 },
            { "Chat", 16 },
            { "Productivity", 17 },
            { "Map", 18 },
            { "Performance", 19 },
            { "Debug", 20 },
            { "AI", 21 },
            { "Remote Control", 99 }
        };

        private static readonly System.Collections.Generic.Dictionary<string, string> CategoryEmojis = new()
        {
            { "Login", "🔐" },
            { "Execution", "⚙️" },
            { "Profiles", "📜" },
            { "General", "🌐" },
            { "Combat", "⚔️" },
            { "Regeneration", "🌭" },
            { "Consumables", "🧪" },
            { "Mounts", "🐎" },
            { "Looting", "💰" },
            { "Inventory", "🎒" },
            { "Navigation", "🧭" },
            { "Questing", "📜" },
            { "Party", "👥" },
            { "Dungeon", "🏰" },
            { "PvP", "⚔️" },
            { "Social", "💬" },
            { "Chat", "🗨️" },
            { "Productivity", "📈" },
            { "Map", "🗺️" },
            { "Performance", "🚀" },
            { "Debug", "🐞" },
            { "AI", "🧠" },
            { "Remote Control", "📱" }
        };

        private System.Collections.Generic.IEnumerable<string> GetCombatClasses()
        {
            var interfaceType = typeof(AmeisenBotX.Core.Engines.Combat.Classes.ICombatClass);
            var types = interfaceType.Assembly.GetTypes()
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Select(p => p.Name)
                .OrderBy(x => x)
                .ToList();
            
            // Ensure empty is an option if allowed, or at least return what we found
            return types;
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
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (obj is ConfigPropertyViewModel vm)
            {
                return vm.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       vm.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       vm.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
