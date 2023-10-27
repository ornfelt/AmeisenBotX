using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using System.Windows;
using System.Windows.Controls;

namespace AmeisenBotX.Views
{
    /// <summary>
    /// Represents a control used to display a spell.
    /// </summary>
    public partial class SpellDisplay : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpellDisplay"/> class with the provided <paramref name="spell"/> object.
        /// </summary>
        public SpellDisplay(Spell spell)
        {
            Spell = spell;
            InitializeComponent();
        }

        /// <summary>
        /// Gets the private Spell.
        /// </summary>
        private Spell Spell { get; }

        /// <summary>
        /// This method is called when the UserControl is loaded. It sets the content of the labelSpellName and labelSpellRank labels with the corresponding properties of the Spell object. It also sets the content of the labelItemType label with the concatenated string of Spell.SpellbookName, Spell.Costs, Spell.CastTime, Spell.MinRange, and Spell.MaxRange properties. 
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            labelSpellName.Content = Spell.Name;
            labelSpellRank.Content = Spell.Rank;

            // labelIcon.Content = "❓";

            labelItemType.Content = $"{Spell.SpellbookName} - {Spell.Costs} - {Spell.CastTime}s - {Spell.MinRange}m => {Spell.MaxRange}m";

            // labelSpellName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BotUtils.GetColorByQuality(WowItem.ItemQuality)));
        }
    }
}