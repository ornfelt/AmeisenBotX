using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Namespace containing views related to the AmeisenBotX application.
/// </summary>
namespace AmeisenBotX.Views
{
    ///<summary>
    /// Class representing the display of an item in a user control.
    ///</summary>
    public partial class ItemDisplay : UserControl
    {
        ///<summary>
        /// Constructor for the ItemDisplay class.
        ///</summary>
        ///<param name="wowItem">The WowInventoryItem object to be displayed.</param>
        public ItemDisplay(IWowInventoryItem wowItem)
        {
            WowItem = wowItem;
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the private property WowItem, which represents an inventory item in the Wow game.
        /// </summary>
        private IWowInventoryItem WowItem { get; }

        /// <summary>
        /// Event handler for when the UserControl is loaded.
        /// Sets the content of the labelItemName and labelItemId to the Name and Id of the WowItem, respectively.
        /// Sets the content of the labelIcon based on the type of the WowItem.
        /// Sets the content of the labelItemType to display the Type, Subtype, ItemLevel, Durability, and MaxDurability of the WowItem.
        /// Sets the foreground color of the labelItemName based on the ItemQuality of the WowItem.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            labelItemName.Content = WowItem.Name;
            labelItemId.Content = WowItem.Id;

            if (WowItem.GetType() == typeof(WowWeapon)) { labelIcon.Content = "🗡"; }
            else if (WowItem.GetType() == typeof(WowArmor)) { labelIcon.Content = "🛡"; }
            else if (WowItem.GetType() == typeof(WowConsumable)) { labelIcon.Content = "🍏"; }
            else if (WowItem.GetType() == typeof(WowContainer)) { labelIcon.Content = "🎒"; }
            else if (WowItem.GetType() == typeof(WowGem)) { labelIcon.Content = "💎"; }
            else if (WowItem.GetType() == typeof(WowKey)) { labelIcon.Content = "🗝️"; }
            else if (WowItem.GetType() == typeof(WowMoneyItem)) { labelIcon.Content = "💰"; }
            else if (WowItem.GetType() == typeof(WowProjectile) || WowItem.GetType() == typeof(WowQuiver)) { labelIcon.Content = "🏹"; }
            else if (WowItem.GetType() == typeof(WowQuestItem)) { labelIcon.Content = "💡"; }
            else if (WowItem.GetType() == typeof(WowReagent)) { labelIcon.Content = "🧪"; }
            else if (WowItem.GetType() == typeof(WowRecipe)) { labelIcon.Content = "📜"; }
            else if (WowItem.GetType() == typeof(WowTradeGoods)) { labelIcon.Content = "📦"; }
            else if (WowItem.GetType() == typeof(WowMiscellaneousItem)) { labelIcon.Content = "📦"; }
            else { labelIcon.Content = "❓"; }

            labelItemType.Content = $"{WowItem.Type} - {WowItem.Subtype} - iLvl {WowItem.ItemLevel} - {WowItem.Durability}/{WowItem.MaxDurability}";

            labelItemName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(((WowItemQuality)WowItem.ItemQuality).GetColor()));
        }
    }
}