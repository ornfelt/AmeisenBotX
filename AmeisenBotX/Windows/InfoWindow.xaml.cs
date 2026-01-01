using AmeisenBotX.Core;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AmeisenBotX.Windows
{
    public partial class InfoWindow : Window
    {
        public InfoWindow(AmeisenBot ameisenBot)
        {
            AmeisenBot = ameisenBot;
            CurrentDisplayMode = DisplayMode.Equipment;
            InitializeComponent();
        }

        private enum DisplayMode
        {
            Equipment,
            Inventory,
            Spells
        }

        private AmeisenBot AmeisenBot { get; set; }

        private DisplayMode CurrentDisplayMode { get; set; }

        private void ButtonEquipment_Click(object sender, RoutedEventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Equipment;
            DisplayStuff();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ButtonInventory_Click(object sender, RoutedEventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Inventory;
            DisplayStuff();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }

        private void ButtonSpells_Click(object sender, RoutedEventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Spells;
            DisplayStuff();
        }

        private void DisplayStuff()
        {
            equipmentWrapPanel.Children.Clear();
            labelAvgItemLvl.Text = Math.Ceiling(AmeisenBot.Bot.Character.Equipment.AverageItemLevel).ToString();

            switch (CurrentDisplayMode)
            {
                case DisplayMode.Equipment:
                    buttonEquipment.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkAccent1"]);
                    buttonInventory.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonSpells.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);

                    // Calculate score and sort descending
                    var equipmentList = AmeisenBot.Bot.Character.Equipment.Items.Values
                        .Select(i => new { Item = i, Score = ItemEvaluator.CalculateSortScore(AmeisenBot.Bot, AmeisenBot.Config, i) })
                        .OrderByDescending(x => x.Score)
                        .ToList();

                    foreach (var entry in equipmentList)
                    {
                        equipmentWrapPanel.Children.Add(new ItemDisplay(entry.Item, AmeisenBot.Bot.GetIconByItemId(entry.Item.Id), entry.Score));
                    }

                    break;

                case DisplayMode.Inventory:
                    buttonEquipment.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonInventory.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkAccent1"]);
                    buttonSpells.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);

                    // Calculate score and sort descending
                    var inventoryList = AmeisenBot.Bot.Character.Inventory.Items
                        .Select(i => new { Item = i, Score = ItemEvaluator.CalculateSortScore(AmeisenBot.Bot, AmeisenBot.Config, i) })
                        .OrderByDescending(x => x.Score)
                        .ToList();

                    foreach (var entry in inventoryList)
                    {
                        equipmentWrapPanel.Children.Add(new ItemDisplay(entry.Item, AmeisenBot.Bot.GetIconByItemId(entry.Item.Id), entry.Score));
                    }

                    break;

                case DisplayMode.Spells:
                    buttonEquipment.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonInventory.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkBorder"]);
                    buttonSpells.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["DarkAccent1"]);

                    List<SpellDisplay> displays = [];

                    foreach (Spell spell in AmeisenBot.Bot.Character.SpellBook.Spells.GroupBy(e => e.Name).Select(e => e.First()))
                    {
                        displays.Add(new SpellDisplay(spell, AmeisenBot.Bot.GetIconBySpellname(spell.Name)));
                    }

                    List<SpellDisplay> sortedDisplays = [.. displays.OrderBy(s => {
                            return s.Spell.Rank?.Equals("Racial", StringComparison.OrdinalIgnoreCase) == true
                                ? 2
                                : s.Spell.Rank?.Equals("Passive", StringComparison.OrdinalIgnoreCase) == true
                                ? 3
                                : s.Spell.Rank?.Equals("Passive", StringComparison.OrdinalIgnoreCase) == true || s.Spell.Rank?.Equals("Racial Passive", StringComparison.OrdinalIgnoreCase) == true
                                ? 4
                                : 1; })
                        .ThenByDescending(s => s.Spell.TryGetRank(out int r) ? r : 0)
                        .ThenBy(s => s.Spell.SpellbookName)];

                    foreach (SpellDisplay d in sortedDisplays)
                    {
                        equipmentWrapPanel.Children.Add(d);
                    }

                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }


    }
}