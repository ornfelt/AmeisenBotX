using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Windows;
using System;
using System.IO;
using System.Windows;

namespace AmeisenBotX
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private string DataPath { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AmeisenBotX\\profiles\\";

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.OnStartup(e);

            IOUtils.CreateDirectoryIfNotExists(DataPath);

            if (e.Args.Length > 0)
            {
                string arg = e.Args[0].Trim();
                string configPath;

                // Check if it's a full path to config.json or just a profile name
                if (File.Exists(arg))
                {
                    configPath = arg;
                }
                else if (File.Exists(Path.Combine(arg, "config.json")))
                {
                    configPath = Path.Combine(arg, "config.json");
                }
                else
                {
                    // Assume it's a profile name in the DataPath
                    configPath = Path.Combine(DataPath, arg, "config.json");
                }

                if (File.Exists(configPath))
                {
                    MainWindow mainWindow = new(DataPath, configPath);
                    MainWindow = mainWindow;
                    mainWindow.Show();
                    return;
                }
                else
                {
                    MessageBox.Show(
                        $"Profile not found: {arg}\n\nExpected config at: {configPath}",
                        "Profile Not Found",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }

            // Fallback to launcher
            LoadConfigWindow loadConfigWindow = new();
            MainWindow = loadConfigWindow;
            loadConfigWindow.Show();
        }


        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // MessageBox.Show($"Uncaught Exception: {e.Exception.Message}\nStack Trace:\n{e.Exception.StackTrace}", "Bot Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            AmeisenLogger.I.Log("GlobalExceptionHandler", $"Uncaught Exception: {e.Exception.Message}\nStack Trace:\n{e.Exception.StackTrace}", LogLevel.Error);
            e.Handled = true;
        }
    }
}