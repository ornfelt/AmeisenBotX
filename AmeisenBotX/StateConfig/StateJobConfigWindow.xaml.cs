using AmeisenBotX.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmeisenBotX.StateConfig
{
    /// <summary>
    /// Represents a window for configuring state jobs in the application.
    /// </summary>
    public partial class StateJobConfigWindow : Window, IStateConfigWindow
    {
        /// <summary>
        /// Initializes a new instance of the StateJobConfigWindow class with the specified AmeisenBot and AmeisenBotConfig.
        /// </summary>
        public StateJobConfigWindow(AmeisenBot ameisenBot, AmeisenBotConfig config)
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
        /// Gets or sets the configuration for the AmeisenBot.
        /// </summary>
        public AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the data should be saved.
        /// </summary>
        public bool ShouldSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window has finished loading.
        /// </summary>
        public bool WindowLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the AmeisenBot instance.
        /// </summary>
        private AmeisenBot AmeisenBot { get; }

        /// <summary>
        /// Adds the job profiles to the comboboxProfile dropdown list.
        /// </summary>
        private void AddProfiles()
        {
            comboboxProfile.Items.Add("None");

            for (int i = 0; i < AmeisenBot.JobProfiles.Count(); ++i)
            {
                comboboxProfile.Items.Add(AmeisenBot.JobProfiles.ElementAt(i).ToString());
            }

            comboboxProfile.SelectedIndex = 0;
        }

        /// <summary>
        /// Event handler for when the "Done" button is clicked.
        /// Sets the job profile to the selected value in the combobox.
        /// Sets ShouldSave to true.
        /// Hides the current form.
        /// </summary>
        private void ButtonDone_Click(object sender, RoutedEventArgs e)
        {
            Config.JobProfile = comboboxProfile.Text;

            ShouldSave = true;
            Hide();
        }

        /// <summary>
        /// Event handler for when the Exit button is clicked.
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
        /// Event handler for the SelectionChanged event of the ComboboxProfile.
        /// Sets the ChangedSomething boolean to true if the window is loaded.
        /// </summary>
        private void ComboboxProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the Window is loaded.
        /// Sets WindowLoaded flag to true.
        /// Calls AddProfiles method.
        /// If Config.JobProfile is not null or empty, sets the text of comboboxProfile to Config.JobProfile.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded = true;

            AddProfiles();

            if (!string.IsNullOrEmpty(Config.JobProfile))
            {
                comboboxProfile.Text = Config.JobProfile;
            }
        }

        /// <summary>
        /// Handles the event when the left button of the mouse is pressed down on the window.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}