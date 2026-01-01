using AmeisenBotX.Core;
using System.ComponentModel;
using System.Reflection;

namespace AmeisenBotX.ViewModels.Config
{
    public abstract class ConfigPropertyViewModel : INotifyPropertyChanged
    {
        protected readonly AmeisenBotConfig Config;
        protected readonly PropertyInfo Property;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConfigPropertyViewModel(AmeisenBotConfig config, PropertyInfo property)
        {
            Config = config;
            Property = property;

            Name = GetDisplayName(property);
            Description = GetDescription(property);
            Category = GetCategory(property);
        }

        public string Name { get; }
        public string Description { get; }
        public string Category { get; set; }

        /// <summary>
        /// Used for sorting categories in the UI.
        /// </summary>
        public int CategoryOrder { get; set; } = 999;

        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 100;
        public bool UseSlider { get; set; } = false;

        public abstract object Value { get; set; }

        public bool IsVisible { get; set; } = true;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string GetDisplayName(PropertyInfo property)
        {
            DisplayNameAttribute attr = property.GetCustomAttribute<DisplayNameAttribute>();
            return attr != null ? attr.DisplayName : property.Name;
        }

        private static string GetDescription(PropertyInfo property)
        {
            DescriptionAttribute attr = property.GetCustomAttribute<DescriptionAttribute>();
            return attr != null ? attr.Description : string.Empty;
        }

        private static string GetCategory(PropertyInfo property)
        {
            CategoryAttribute attr = property.GetCustomAttribute<CategoryAttribute>();
            return attr != null ? attr.Category : "General";
        }
    }
}
