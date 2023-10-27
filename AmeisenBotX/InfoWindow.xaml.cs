using AmeisenBotX.Core;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Views;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AmeisenBotX
{
    /// <summary>
    /// Represents a window that provides information about the AmeisenBot.
    /// </summary>
    public partial class InfoWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the InfoWindow class.
        /// </summary>
        /// <param name="ameisenBot">The AmeisenBot object to be used in the InfoWindow.</param>
        public InfoWindow(AmeisenBot ameisenBot)
        {
            AmeisenBot = ameisenBot;
            CurrentDisplayMode = DisplayMode.Equipment;
            InitializeComponent();
        }

        /// <summary>
        /// Enum that represents the different display modes in the application.
        /// The available modes are Equipment, Inventory, and Spells.
        /// </summary>
        private enum DisplayMode
        {
            Equipment,
            Inventory,
            Spells
        }

        /// <summary>
        /// Gets or sets the AmeisenBot object for this instance.
        /// </summary>
        private AmeisenBot AmeisenBot { get; set; }

        /// <summary>
        /// Gets or sets the current display mode.
        /// </summary>
        private DisplayMode CurrentDisplayMode { get; set; }

        /// <summary>
        /// Handles the click event of the Equipment button.
        /// Sets the CurrentDisplayMode to Equipment and calls the DisplayStuff method.
        /// </summary>
        private void ButtonEquipment_Click(object sender, RoutedEventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Equipment;
            DisplayStuff();
        }

        /// <summary>
        /// Event handler for the click event of the Exit button. Hides the current window.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Event handler for the button click event of the inventory button. 
        /// Sets the current display mode to inventory and displays the related content.
        /// </summary>
        private void ButtonInventory_Click(object sender, RoutedEventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Inventory;
            DisplayStuff();
        }

        /// <summary>
        /// Event handler for the click event of the ButtonRefresh button. 
        /// Calls the DisplayStuff method to refresh the display.
        /// </summary>
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }

        /// <summary>
        /// Event handler for when the ButtonSpells is clicked.
        /// Sets the CurrentDisplayMode to DisplayMode.Spells and invokes the DisplayStuff method.
        /// </summary>
        private void ButtonSpells_Click(object sender, RoutedEventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Spells;
            DisplayStuff();
        }

        /// <summary>
        /// This method is responsible for displaying the appropriate items or spells based on the current display mode. 
        /// It clears the children of the equipmentWrapPanel and updates the labelAvgItemLvl content to show the average item level. 
        /// It then switches on the CurrentDisplayMode and performs the necessary actions based on the chosen mode.
        /// For DisplayMode.Equipment, it sets the buttonEquipment border color to DarkAccent1, and the buttonInventory and buttonSpells border colors to DarkBorder.
        /// It retrieves the equipment items from AmeisenBot.Bot.Character.Equipment.Items and adds an ItemDisplay for each item to the equipmentWrapPanel.
        /// For DisplayMode.Inventory, it sets the buttonInventory border color to DarkAccent1, and the buttonEquipment and buttonSpells border colors to DarkBorder.
        /// It retrieves the inventory items from AmeisenBot.Bot.Character.Inventory.Items and adds an ItemDisplay for each item to the equipmentWrapPanel.
        /// For DisplayMode.Spells, it sets the buttonSpells border color to DarkAccent1, and the buttonEquipment and buttonInventory border colors to DarkBorder.
        /// It retrieves the spells from AmeisenBot.Bot.Character.SpellBook.Spells, groups them by name, selects the first spell from each group, and adds a SpellDisplay for each spell to the equipmentWrapPanel.
        /// </summary>
        private void DisplayStuff()
        {
            equipmentWrapPanel.Children.Clear();
            labelAvgItemLvl.Content = Math.Ceiling(AmeisenBot.Bot.Character.Equipment.AverageItemLevel);

            switch (CurrentDisplayMode)
            {
                case DisplayMode.Equipment:
                    buttonEquipment.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkAccent1"]);
                    buttonInventory.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonSpells.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);

                    IWowInventoryItem[] equipmentItems = AmeisenBot.Bot.Character.Equipment.Items.Values.ToArray();

                    foreach (IWowInventoryItem invItem in equipmentItems)
                    {
                        equipmentWrapPanel.Children.Add(new ItemDisplay(invItem));
                    }

                    break;

                case DisplayMode.Inventory:
                    buttonEquipment.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonInventory.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkAccent1"]);
                    buttonSpells.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);

                    IWowInventoryItem[] inventoryItems = AmeisenBot.Bot.Character.Inventory.Items.ToArray();

                    foreach (IWowInventoryItem invItem in inventoryItems)
                    {
                        equipmentWrapPanel.Children.Add(new ItemDisplay(invItem));
                    }

                    break;

                case DisplayMode.Spells:
                    buttonEquipment.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonInventory.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonSpells.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkAccent1"]);

                    foreach (Spell spell in AmeisenBot.Bot.Character.SpellBook.Spells.GroupBy(e => e.Name).Select(e => e.First()))
                    {
                        equipmentWrapPanel.Children.Add(new SpellDisplay(spell));
                    }

                    break;
            }
        }

        /// <summary>
        /// Event handler for when the window is loaded.
        /// Calls the DisplayStuff method.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }

        /// <summary>
        /// Event handler for when the left mouse button is pressed on the window.
        /// This method allows the window to be dragged around the screen.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}