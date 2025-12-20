using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Utils;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AmeisenBotX.Views
{
    public partial class SpellDisplay : UserControl
    {
        public SpellDisplay(Spell spell, Bitmap icon = null)
        {
            Spell = spell;
            Icon = icon;
            InitializeComponent();
        }

        public Spell Spell { get; }

        public Bitmap Icon { get; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            labelSpellName.Content = Spell.Name;
            labelSpellRank.Content = Spell.Rank;

            string[] parts = [
                Spell.Costs > 0 ? $"{Spell.Costs}" : null,
                Spell.CastTime > 0 ? $"{Spell.CastTime}s" : "Instant",
                Spell.MaxRange > 0 ? $"{Spell.MinRange}-{Spell.MaxRange}m" : null
            ];

            labelItemType.Content = $"{Spell.SpellbookName} » {string.Join(" · ", parts.Where(s => s != null))}";

            if (Icon != null)
            {
                imageIcon.Source = Icon.ToImageSource();
            }
        }
    }
}