using AmeisenBotX.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmeisenBotX.StateConfig
{
    public partial class StateGrindingConfigWindow : Window, IStateConfigWindow
    {
        /// <summary>
        /// Initializes a new instance of the StateGrindingConfigWindow class.
        /// </summary>
        /// <param name="ameisenBot">The AmeisenBot instance.</param>
        /// <param name="config">The AmeisenBotConfig instance.</param>
        public StateGrindingConfigWindow(AmeisenBot ameisenBot, AmeisenBotConfig config)
        {
            AmeisenBot = ameisenBot;
            Config = config;
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether something has been changed.
        /// </summary>
        public bool ChangedSomething { get; set; }

        /// <summary>
        /// Gets or sets the AmeisenBot configuration.
        /// </summary>
        public AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the data should be saved.
        /// </summary>
        public bool ShouldSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window has been loaded.
        /// </summary>
        public bool WindowLoaded { get; private set; }

        /// <summary>
        /// Gets the AmeisenBot instance associated with this class.
        /// </summary>
        private AmeisenBot AmeisenBot { get; }

        /// <summary>
        /// Adds the available profiles to the comboboxProfile.
        /// Sets the default selection to "None".
        /// </summary>
        private void AddProfiles()
        {
            comboboxProfile.Items.Add("None");

            for (int i = 0; i < AmeisenBot.GrindingProfiles.Count(); ++i)
            {
                comboboxProfile.Items.Add(AmeisenBot.GrindingProfiles.ElementAt(i).ToString());
            }

            comboboxProfile.SelectedIndex = 0;
        }

        /// <summary>
        /// Event handler for when the "Done" button is clicked.
        /// Sets the GrindingProfile configuration to the selected text in the combobox.
        /// Sets ShouldSave to true.
        /// Hides the current window.
        /// </summary>
        private void ButtonDone_Click(object sender, RoutedEventArgs e)
        {
            Config.GrindingProfile = comboboxProfile.Text;

            ShouldSave = true;
            Hide();
        }

        /// <summary>
        /// Event handler for when the Exit button is clicked.
        /// Hides the current window after confirming with the user if there are unsaved changes.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            // if (ChangedSomething) { ConfirmWindow confirmWindow = new ConfirmWindow("Unsaved
            // Changes!", "Are you sure that you wan't to cancel?", "Yes", "No"); confirmWindow.ShowDialog();
            //
            // if (!confirmWindow.OkayPressed) { return; } }

            Hide();
        }

        /// <summary>
        /// Event handler for the selection changed event of the ComboboxProfile.
        /// Sets the ChangedSomething property to true if the ComboBox is loaded with a new selection.
        /// </summary>
        private void ComboboxProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Method called when the window is loaded. Sets the WindowLoaded variable to true, adds profiles, and if the GrindingProfile in the Config is not empty, sets the text of the comboboxProfile to the GrindingProfile value.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded = true;

            AddProfiles();

            if (!string.IsNullOrEmpty(Config.GrindingProfile))
            {
                comboboxProfile.Text = Config.GrindingProfile;
            }
        }

        /// <summary>
        /// Handles the event when the left mouse button is pressed down on the window.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}