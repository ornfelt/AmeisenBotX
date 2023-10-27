using System.Windows;
using System.Windows.Input;

namespace AmeisenBotX.Views
{
    /// <summary>
    /// Constructor for creating a confirm window with a custom title, message, okay button text, and cancel button text.
    /// </summary>
    /// <param name="title">The title of the confirm window.</param>
    /// <param name="message">The message displayed in the confirm window.</param>
    /// <param name="btnOkayText">The text shown on the okay button. Default value is "✔️ Okay".</param>
    /// <param name="btnCancelText">The text shown on the cancel button. Default value is "❌ Cancel".</param>
    public partial class ConfirmWindow : Window
    {
        /// <summary>
        /// Constructor for creating a confirm window with a custom title, message, okay button text, and cancel button text.
        /// </summary>
        /// <param name="title">The title of the confirm window.</param>
        /// <param name="message">The message displayed in the confirm window.</param>
        /// <param name="btnOkayText">The text shown on the okay button. Default value is "✔️ Okay".</param>
        /// <param name="btnCancelText">The text shown on the cancel button. Default value is "❌ Cancel".</param>
        public ConfirmWindow(string title, string message, string btnOkayText = "✔️ Okay", string btnCancelText = "❌ Cancel")
        {
            InitializeComponent();

            messageTitle.Content = title;
            messageLabel.Text = message;

            if (message.Length > 64)
            {
                messageLabel.FontSize = 12;
            }

            buttonOkay.Content = btnOkayText;
            buttonCancel.Content = btnCancelText;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Okay button has been pressed.
        /// </summary>
        public bool OkayPressed { get; private set; }

        /// <summary>
        /// Event handler for the cancel button click event.
        /// Sets the OkayPressed property to false and closes the current form.
        /// </summary>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            OkayPressed = false;
            Close();
        }

        /// <summary>
        /// Event handler for when the Okay button is clicked.
        /// Sets the OkayPressed property to true and closes the window.
        /// </summary>
        private void ButtonOkay_Click(object sender, RoutedEventArgs e)
        {
            OkayPressed = true;
            Close();
        }

        /// <summary>
        /// Event handler for when the window is loaded.
        /// This event handler sets the position of the window in relation to the mouse cursor.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);

            System.Windows.Media.Matrix transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            Point mouse = transform.Transform(pointToScreen);
            Left = mouse.X - (Width / 2);
            Top = mouse.Y - (Height / 2);
        }

        /// <summary>
        /// Event handler for the left mouse button down event in the Window.
        /// Allows the user to drag and move the Window.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}