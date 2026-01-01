using AmeisenBotX.Core;
using System.Windows;

namespace AmeisenBotX.Windows
{
    public partial class RelationshipWindow : Window
    {
        public RelationshipWindow(AmeisenBot ameisenBot)
        {
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }

        private void DisplayStuff()
        {
            relationshipWrapPanel.Children.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }


    }
}