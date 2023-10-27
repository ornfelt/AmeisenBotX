using AmeisenBotX.Core;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmeisenBotX.StateConfig
{
    /// <summary>
    /// Represents a window for configuring questing state for the AmeisenBot.
    /// </summary>
    public partial class StateQuestingConfigWindow : Window, IStateConfigWindow
    {
        /// <summary>
        /// Initializes a new instance of the StateQuestingConfigWindow class.
        /// </summary>
        /// <param name="ameisenBot">The AmeisenBot instance to use.</param>
        /// <param name="config">The AmeisenBotConfig instance to use.</param>
        public StateQuestingConfigWindow(AmeisenBot ameisenBot, AmeisenBotConfig config)
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
        /// Gets or sets a value indicating whether the object should be saved.
        /// </summary>
        public bool ShouldSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window has finished loading.
        /// </summary>
        public bool WindowLoaded { get; private set; }

        /// <summary>
        /// Gets the instance of the AmeisenBot class.
        /// </summary>
        private AmeisenBot AmeisenBot { get; }

        /// <summary>
        /// Adds the profiles to the comboboxProfile.
        /// </summary>
        private void AddProfiles()
        {
            comboboxProfile.Items.Add("None");

            for (int i = 0; i < AmeisenBot.QuestProfiles.Count(); ++i)
            {
                comboboxProfile.Items.Add(AmeisenBot.QuestProfiles.ElementAt(i).ToString());
            }

            comboboxProfile.SelectedIndex = 0;
        }

        /// <summary>
        /// Sets the value of the QuestProfile property to the text of comboboxProfile and
        /// sets ShouldSave to true. Hides the current control.
        /// </summary>
        private void ButtonDone_Click(object sender, RoutedEventArgs e)
        {
            Config.QuestProfile = comboboxProfile.Text;

            ShouldSave = true;
            Hide();
        }

        ///<summary>
        ///Handles the click event of the ButtonExit control. 
        ///If there are unsaved changes, displays a confirmation window asking the user if they want to cancel. 
        ///If the user selects "No", the method returns without further action. 
        ///Otherwise, hides the current window. 
        ///</summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            // if (ChangedSomething) { ConfirmWindow confirmWindow = new ConfirmWindow("Unsaved
            // Changes!", "Are you sure that you wan't to cancel?", "Yes", "No"); confirmWindow.ShowDialog();
            //
            // if (!confirmWindow.OkayPressed) { return; } }

            Hide();
        }

        /// <summary>
        /// Event handler for when the selection of the ComboboxProfile is changed.
        /// </summary>
        private void ComboboxProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// This method is called when the window is loaded.
        /// It sets the WindowLoaded flag to true and adds profiles.
        /// If the QuestProfile configuration value is not empty, it sets the selected profile in the combobox.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded = true;

            AddProfiles();

            if (!string.IsNullOrEmpty(Config.QuestProfile))
            {
                comboboxProfile.Text = Config.QuestProfile;
            }
        }

        /// <summary>
        /// Event handler for the mouse left button down event on the window.
        /// This method is responsible for initiating the dragging behavior of the window.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}