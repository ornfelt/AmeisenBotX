using AmeisenBotX.Core;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// The AmeisenBotX namespace contains classes for handling bot operations.
/// </summary>
namespace AmeisenBotX
{
    /// <summary>
    /// Event handler for the click event of a button in the RelationshipWindow.
    /// </summary>
    public partial class RelationshipWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipWindow"/> class.
        /// </summary>
        /// <param name="ameisenBot">The <see cref="AmeisenBot"/> to use for the relationship window.</param>
        public RelationshipWindow(AmeisenBot ameisenBot)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for the click event of the ButtonExit button.
        /// Hides the current window.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Event handler for the ButtonRefresh's click event. Calls the DisplayStuff() method.
        /// </summary>
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }

        /// <summary>
        /// Clears the children of the relationshipWrapPanel.
        /// </summary>
        private void DisplayStuff()
        {
            relationshipWrapPanel.Children.Clear();
        }

        /// <summary>
        /// Executes when the window is loaded.
        /// Calls the DisplayStuff method to display content.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayStuff();
        }

        /// <summary>
        /// Event handler for the left mouse button down event on the Window. This method initiates the dragging of the window.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}