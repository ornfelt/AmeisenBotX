using AmeisenBotX.Common.Utils;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace AmeisenBotX
{
    /// <summary>
    /// Represents a window for loading configurations.
    /// </summary>
    public partial class LoadConfigWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the LoadConfigWindow class.
        /// </summary>
        public LoadConfigWindow()
        {
            ConfigToLoad = string.Empty;
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the configuration to be loaded.
        /// </summary>
        public string ConfigToLoad { get; set; }

        /// <summary>
        /// Gets the path where the data is stored for the AmeisenBotX profiles in the Application Data folder.
        /// </summary>
        private string DataPath { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AmeisenBotX\\profiles\\";

        /// <summary>
        /// Event handler for the ButtonClose Click event.
        /// Closes the current window.
        /// </summary>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event handler for the SelectionChanged event of the ComboboxSelectedConfig control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The SelectionChangedEventArgs associated with the event.</param>
        private void ComboboxSelectedConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboboxSelectedConfig.SelectedItem == null)
            {
                return;
            }

            if ((string)comboboxSelectedConfig.SelectedItem == "New Config")
            {
                ConfigEditorWindow configEditor = new(DataPath, null);
                configEditor.ShowDialog();

                if (configEditor.Cancel)
                {
                    comboboxSelectedConfig.SelectedItem = null;
                    return;
                }

                if (configEditor.ConfigName != null && configEditor.Config != null)
                {
                    ConfigToLoad = Path.Combine(DataPath, configEditor.ConfigName, "config.json");
                    IOUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(ConfigToLoad));

                    File.WriteAllText(ConfigToLoad, JsonSerializer.Serialize(configEditor.Config, new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString }));
                }
            }
            else
            {
                ConfigToLoad = Path.Combine(DataPath, (string)comboboxSelectedConfig.SelectedItem, "config.json");
            }

            Hide();

            MainWindow mainWindow = new(DataPath, ConfigToLoad);
            mainWindow.Show();

            Close();
        }

        /// <summary>
        /// Event handler for when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // check for older data folder that users need to migrate to the new location
            string oldDataPath = $"{Directory.GetCurrentDirectory()}\\data\\";

            if (Directory.Exists(oldDataPath))
            {
                MessageBox.Show($"You need to move the content of your \"\\\\data\\\\\" folder to \"{DataPath}\". Otherwise your profiles may not be displayed.", "New Data Location", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // create our data folder, default will be placed at "%APPDATA%/AmeisenBotX/..."
            IOUtils.CreateDirectoryIfNotExists(DataPath);

            comboboxSelectedConfig.Items.Add("New Config");

            string[] directories = Directory.GetDirectories(DataPath);

            foreach (string str in directories)
            {
                comboboxSelectedConfig.Items.Add(Path.GetFileName(str));
            }

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                string botnameParam = args[1];

                if (comboboxSelectedConfig.Items.Contains(botnameParam))
                {
                    comboboxSelectedConfig.SelectedItem = botnameParam;
                }
            }
        }

        /// <summary>
        /// Event handler for when the left mouse button is pressed down on the window.
        /// Allows the window to be dragged around.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}