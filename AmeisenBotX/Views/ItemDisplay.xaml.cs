using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AmeisenBotX.Views
{
    public partial class ItemDisplay : UserControl
    {
        public ItemDisplay(IWowInventoryItem wowItem, Bitmap icon = null, double? itemScore = null)
        {
            WowItem = wowItem;
            Icon = icon;
            ItemScore = itemScore;
            InitializeComponent();
        }

        private IWowInventoryItem WowItem { get; }

        private Bitmap Icon { get; }

        private double? ItemScore { get; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            labelItemName.Content = WowItem.Name;
            labelItemId.Content = ItemScore.HasValue 
                ? $"Id: {WowItem.Id} | Val: {ItemScore.Value:F1}"
                : $"Id: {WowItem.Id}";

            labelItemType.Content = $"{WowItem.Type} - {WowItem.Subtype} - iLvl {WowItem.ItemLevel} - {WowItem.Durability}/{WowItem.MaxDurability}";
            labelItemCount.Content = WowItem.Count > 1 ? WowItem.Count : string.Empty;

            labelItemName.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(((WowItemQuality)WowItem.ItemQuality).GetColor()));

            if (Icon != null)
            {
                imageIcon.Source = Icon.ToImageSource();
            }
        }
    }
}