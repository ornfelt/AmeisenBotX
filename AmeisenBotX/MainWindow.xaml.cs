﻿using AmeisenBotX.Common.Keyboard;
using AmeisenBotX.Common.Keyboard.Enums;
using AmeisenBotX.Common.Keyboard.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core;
using AmeisenBotX.Core.Logic;
using AmeisenBotX.Core.Logic.Enums;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Overlay;
using AmeisenBotX.Overlay.Utils;
using AmeisenBotX.StateConfig;
using AmeisenBotX.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace AmeisenBotX
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class with the specified data path and config path.
        /// Throws FileNotFoundException if the data path or config path does not exist.
        /// Sets the DataPath and ConfigPath properties.
        /// Initializes various brushes and freezes them for improved performance.
        /// Creates a new LabelUpdateEvent with a TimeSpan of 1 second.
        /// Sets the RenderState to true.
        /// Initializes and enables a new KeyboardHook.
        /// </summary>
        /// <param name="dataPath">The path to the data directory.</param>
        /// <param name="configPath">The path to the config file.</param>
        public MainWindow(string dataPath, string configPath)
        {
            if (!Directory.Exists(dataPath)) { throw new FileNotFoundException(dataPath); }
            if (!File.Exists(configPath)) { throw new FileNotFoundException(configPath); }

            DataPath = dataPath;
            ConfigPath = configPath;

            InitializeComponent();

            CurrentTickTimeBadBrush = new SolidColorBrush(Color.FromRgb(255, 0, 80));
            CurrentTickTimeGoodBrush = new SolidColorBrush(Color.FromRgb(160, 255, 0));
            DarkForegroundBrush = new SolidColorBrush((Color)FindResource("DarkForeground"));
            DarkBackgroundBrush = new SolidColorBrush((Color)FindResource("DarkBackground"));
            TextAccentBrush = new SolidColorBrush((Color)FindResource("TextAccent"));

            NotificationGmBrush = new(Colors.Cyan);
            NotificationBrush = new(Colors.Pink);
            NotificationWhiteBrush = new(Colors.White);
            NotificationTransparentBrush = new(Colors.Transparent);

            CurrentTickTimeBadBrush.Freeze();
            CurrentTickTimeGoodBrush.Freeze();
            DarkForegroundBrush.Freeze();
            DarkBackgroundBrush.Freeze();
            TextAccentBrush.Freeze();

            NotificationGmBrush.Freeze();
            NotificationBrush.Freeze();
            NotificationWhiteBrush.Freeze();
            NotificationTransparentBrush.Freeze();

            LabelUpdateEvent = new(TimeSpan.FromSeconds(1));

            RenderState = true;

            KeyboardHook = new KeyboardHook();
            KeyboardHook.Enable();
        }

        /// <summary>
        /// Gets a value indicating whether the auto position setup is enabled.
        /// </summary>
        public bool IsAutoPositionSetup { get; private set; }

        /// <summary>
        /// Gets the KeyboardHook object.
        /// </summary>
        public KeyboardHook KeyboardHook { get; }

        /// <summary>
        /// Gets or sets the value of M11.
        /// </summary>
        public double M11 { get; private set; }

        /// <summary>
        /// Gets or sets the value of the element at row 2 and column 2 of a two-dimensional matrix.
        /// </summary>
        public double M22 { get; private set; }

        /// <summary>
        /// Gets or sets the AmeisenBot overlay.
        /// </summary>
        public AmeisenBotOverlay Overlay { get; private set; }

        /// <summary>
        /// Gets or sets the render state.
        /// </summary>
        public bool RenderState { get; set; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBot class for this object.
        /// </summary>
        private AmeisenBot AmeisenBot { get; set; }

        /// <summary>
        /// Gets the path of the configuration file.
        /// </summary>
        private string ConfigPath { get; }

        /// <summary>
        /// Gets or sets the brush used to visualize the current tick time being in a bad state.
        /// </summary>
        private Brush CurrentTickTimeBadBrush { get; }

        /// <summary>
        /// Gets the brush for the current tick time if it is considered good.
        /// </summary>
        private Brush CurrentTickTimeGoodBrush { get; }

        /// <summary>
        /// Gets or sets the brush used for dark background.
        /// </summary>
        private Brush DarkBackgroundBrush { get; }

        /// <summary>
        /// Gets the dark foreground brush.
        /// </summary>
        private Brush DarkForegroundBrush { get; }

        /// <summary>
        /// Gets the path of the data.
        /// </summary>
        private string DataPath { get; }

        /// <summary>
        /// Gets or sets the private DevToolsWindow property.
        /// </summary>
        private DevToolsWindow DevToolsWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to draw the overlay.
        /// </summary>
        private bool DrawOverlay { get; set; }

        /// <summary>
        /// Gets or sets the private InfoWindow for this object.
        /// </summary>
        private InfoWindow InfoWindow { get; set; }

        /// <summary>
        /// Gets or sets the private time-gated event for label updates.
        /// </summary>
        private TimegatedEvent LabelUpdateEvent { get; }

        /// <summary>
        /// Gets or sets the handle to the main window.
        /// </summary>
        private IntPtr MainWindowHandle { get; set; }

        /// <summary>
        /// Gets or sets the private MapWindow object.
        /// </summary>
        private MapWindow MapWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the overlay needs to be cleared.
        /// </summary>
        private bool NeedToClearOverlay { get; set; }

        /// <summary>
        /// Gets or sets the color of the notification.
        /// </summary>
        private SolidColorBrush NoticifactionColor { get; set; }

        /// <summary>
        /// Gets or sets the current state of the notification blink.
        /// </summary>
        private bool NotificationBlinkState { get; set; }

        /// <summary>
        /// Gets the SolidColorBrush used for the notification.
        /// </summary>
        private SolidColorBrush NotificationBrush { get; }

        /// <summary>
        /// Gets or sets the SolidColorBrush for the NotificationGmBrush.
        /// </summary>
        private SolidColorBrush NotificationGmBrush { get; }

        /// <summary>
        /// Gets or sets the last timestamp of the notification.
        /// </summary>
        private long NotificationLastTimestamp { get; set; }

        /// <summary>
        /// Gets the SolidColorBrush used for transparent notifications.
        /// </summary>
        private SolidColorBrush NotificationTransparentBrush { get; }

        /// <summary>
        /// Gets the solid color brush used for notification background color as white.
        /// </summary>
        private SolidColorBrush NotificationWhiteBrush { get; }

        /// <summary>
        /// Gets or sets a value indicating whether there is a pending notification.
        /// </summary>
        private bool PendingNotification { get; set; }

        /// <summary>
        /// Gets or sets the private RelationshipWindow object. 
        /// </summary>
        private RelationshipWindow RelationshipWindow { get; set; }

        /// <summary>
        /// Gets or sets the dictionary that maps BotMode to Window objects representing the configuration
        /// windows for each mode.
        /// </summary>
        private Dictionary<BotMode, Window> StateConfigWindows { get; set; }

        /// <summary>
        /// Gets or sets the brush used to accentuate the text.
        /// </summary>
        private Brush TextAccentBrush { get; }

        /// <summary>
        /// Used to resize the wow window when autoposition is enabled
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);

            if (AmeisenBot != null && IsAutoPositionSetup)
            {
                Dispatcher.Invoke(() =>
                {
                    AmeisenBot.Bot.Memory.ResizeParentWindow
                    (
                        (int)((wowRect.Margin.Left + 1) * M11),
                        (int)((wowRect.Margin.Top + 1) * M22),
                        (int)((wowRect.ActualWidth - 1) * M11),
                        (int)((wowRect.ActualHeight - 1) * M22)
                    );
                });
            }

            return size;
        }

        /// <summary>
        /// Tries to load a config file from the specified path and assigns it to the provided AmeisenBotConfig object.
        /// Returns true if successful; otherwise, returns false.
        /// </summary>
        /// <param name="configPath">The path of the config file to be loaded.</param>
        /// <param name="config">The AmeisenBotConfig object where the loaded config will be assigned.</param>
        /// <returns>Returns true if the config file was successfully loaded; otherwise, returns false.</returns>
        private static bool TryLoadConfig(string configPath, out AmeisenBotConfig config)
        {
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                config = File.Exists(configPath)
                    ? JsonSerializer.Deserialize<AmeisenBotConfig>(File.ReadAllText(configPath), new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString })
                    : new();

                config.Path = configPath;
                return true;
            }

            config = null;
            return false;
        }

        /// <summary>
        /// Event handler for when the Clear Cache button is clicked.
        /// It clears the cache in the AmeisenBot database.
        /// </summary>
        private void ButtonClearCache_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Bot.Db.Clear();
        }

        /// <summary>
        /// Event handler for the ButtonConfig click event. Opens the ConfigEditorWindow and displays the current configuration. If the user saves the configuration, it updates the AmeisenBot's configuration, writes the updated configuration to a file, clear the keyboard hook, and reload the hotkeys.
        /// </summary>
        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigEditorWindow configWindow = new(DataPath, AmeisenBot, AmeisenBot.Config, AmeisenBot.AccountName);
            configWindow.ShowDialog();

            if (configWindow.SaveConfig)
            {
                AmeisenBot.ReloadConfig(configWindow.Config);
                File.WriteAllText(AmeisenBot.Config.Path, JsonSerializer.Serialize(configWindow.Config, new JsonSerializerOptions() { WriteIndented = true }));

                KeyboardHook.Clear();
                LoadHotkeys();
            }
        }

        /// <summary>
        /// Event handler for the "ButtonDevTools_Click" event. Creates a new instance of the "DevToolsWindow" if it doesn't already exist and shows it.
        /// </summary>
        private void ButtonDevTools_Click(object sender, RoutedEventArgs e)
        {
            DevToolsWindow ??= new(AmeisenBot);
            DevToolsWindow.Show();
        }

        /// <summary>
        /// Event handler for the ButtonExit_Click event.
        /// Closes the current window.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event handler for the ButtonNotification click event.
        /// Resets the PendingNotification and NotificationBlinkState variables to false.
        /// Sets the foreground color of the buttonNotification to NotificationWhiteBrush.
        /// Sets the background color of the buttonNotification to NotificationTransparentBrush.
        /// </summary>
        private void ButtonNotification_Click(object sender, RoutedEventArgs e)
        {
            PendingNotification = false;
            NotificationBlinkState = false;

            buttonNotification.Foreground = NotificationWhiteBrush;
            buttonNotification.Background = NotificationTransparentBrush;
        }

        /// <summary>
        /// Handles the click event of the ButtonStartPause control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ButtonStartPause_Click(object sender, RoutedEventArgs e)
        {
            StartPause();
        }

        /// <summary>
        /// Event handler for when the ButtonStateConfig is clicked.
        /// If the StateConfigWindows dictionary contains the selected item from the comboboxStateOverride,
        /// the corresponding window is opened and shown as a dialog.
        /// If the ShouldSave property of the selected window is true, the AmeisenBot config is reloaded
        /// with the selected window's config and the SaveConfig method is called.
        /// </summary>
        private void ButtonStateConfig_Click(object sender, RoutedEventArgs e)
        {
            if (StateConfigWindows.ContainsKey((BotMode)comboboxStateOverride.SelectedItem))
            {
                Window selectedWindow = StateConfigWindows[(BotMode)comboboxStateOverride.SelectedItem];
                selectedWindow.ShowDialog();

                if (((IStateConfigWindow)selectedWindow).ShouldSave)
                {
                    AmeisenBot.ReloadConfig(((IStateConfigWindow)selectedWindow).Config);
                    SaveConfig();
                }
            }
        }

        ///<summary>
        ///Toggles the autopilot feature of the AmeisenBot.
        ///</summary>
        private void ButtonToggleAutopilot_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.Autopilot = !AmeisenBot.Config.Autopilot;
            buttonToggleAutopilot.Foreground = AmeisenBot.Config.Autopilot ? CurrentTickTimeGoodBrush : DarkForegroundBrush;
        }

        /// <summary>
        /// Event handler for the click event of the ButtonToggleInfoWindow button.
        /// Creates a new instance of the InfoWindow if it doesn't already exist, and then shows it.
        /// </summary>
        private void ButtonToggleInfoWindow_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow ??= new(AmeisenBot);
            InfoWindow.Show();
        }

        /// <summary>
        /// Event handler for the ButtonToggleMapWindow button click.
        /// Creates a new MapWindow if it is currently null and sets the AmeisenBot property.
        /// Then shows the MapWindow.
        /// </summary>
        private void ButtonToggleMapWindow_Click(object sender, RoutedEventArgs e)
        {
            MapWindow ??= new(AmeisenBot);
            MapWindow.Show();
        }

        /// <summary>
        /// Toggles the overlay visibility and updates the UI accordingly.
        /// </summary>
        private void ButtonToggleOverlay_Click(object sender, RoutedEventArgs e)
        {
            DrawOverlay = !DrawOverlay;
            buttonToggleOverlay.Foreground = DrawOverlay ? CurrentTickTimeGoodBrush : DarkForegroundBrush;
        }

        /// <summary>
        /// Handles the click event for the ButtonToggleRelationshipWindow.
        /// If the RelationshipWindow is null, creates a new instance and displays it.
        /// </summary>
        private void ButtonToggleRelationshipWindow_Click(object sender, RoutedEventArgs e)
        {
            RelationshipWindow ??= new(AmeisenBot);
            RelationshipWindow.Show();
        }

        ///<summary>
        ///Toggles the rendering of a button when clicked.
        ///</summary>
        private void ButtonToggleRendering_Click(object sender, RoutedEventArgs e)
        {
            // ((BasicCombatClass)AmeisenBot.Bot.CombatClass).TargetProviderTank.Get(out IEnumerable<IWowUnit> units);
            // WowCreatureType type = AmeisenBot.Bot.Objects.Target.ReadType(AmeisenBot.Bot.Memory);
            // bool x = AmeisenBot.Bot.Player.IsSwimming;
            // bool y = AmeisenBot.Bot.Player.IsFlying;

            foreach (IWowDynobject aoe in AmeisenBot.Bot.Objects.All.OfType<IWowDynobject>().Where(e => e.Caster == 1))
            {
                Vector3 pos = aoe.Position;
            }
        }

        /// <summary>
        /// Handles the event when the selection of ComboboxStateOverride changes.
        /// If AmeisenBot is not null, it changes the mode of AmeisenBotLogic to the selected item in ComboboxStateOverride.
        /// It also enables or disables the buttonStateConfig based on the presence of StateConfigWindows for the selected BotMode.
        /// </summary>
        private void ComboboxStateOverride_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AmeisenBot != null)
            {
                ((AmeisenBotLogic)AmeisenBot.Logic).ChangeMode((BotMode)comboboxStateOverride.SelectedItem);
                buttonStateConfig.IsEnabled = StateConfigWindows.ContainsKey((BotMode)comboboxStateOverride.SelectedItem);
            }
        }

        /// <summary>
        /// Loads the hotkeys for the AmeisenBot.
        /// </summary>
        private void LoadHotkeys()
        {
            if (AmeisenBot.Config.Hotkeys.TryGetValue("StartStop", out Keybind kv))
            {
                KeyboardHook.AddHotkey((KeyCode)kv.Key, (KeyCode)kv.Mod, StartPause);
            }
        }

        /// <summary>
        /// Updates the bot information based on the player's class and triggers label update event if necessary.
        /// </summary>
        /// <param name="wowObjects">The collection of WoW objects.</param>
        private void OnObjectUpdateComplete(IEnumerable<IWowObject> wowObjects)
        {
            Dispatcher.Invoke(() =>
            {
                IWowPlayer player = AmeisenBot.Bot.Player;

                if (player != null)
                {
                    switch (player.Class)
                    {
                        case WowClass.Deathknight:
                            UpdateBotInfo(player.MaxRunicPower, player.RunicPower, WowColors.dkPrimaryBrush, WowColors.dkSecondaryBrush);
                            break;

                        case WowClass.Druid:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.druidPrimaryBrush, WowColors.druidSecondaryBrush);
                            break;

                        case WowClass.Hunter:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.hunterPrimaryBrush, WowColors.hunterSecondaryBrush);
                            break;

                        case WowClass.Mage:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.magePrimaryBrush, WowColors.mageSecondaryBrush);
                            break;

                        case WowClass.Paladin:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.paladinPrimaryBrush, WowColors.paladinSecondaryBrush);
                            break;

                        case WowClass.Priest:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.priestPrimaryBrush, WowColors.priestSecondaryBrush);
                            break;

                        case WowClass.Rogue:
                            UpdateBotInfo(player.MaxEnergy, player.Energy, WowColors.roguePrimaryBrush, WowColors.rogueSecondaryBrush);
                            break;

                        case WowClass.Shaman:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.shamanPrimaryBrush, WowColors.shamanSecondaryBrush);
                            break;

                        case WowClass.Warlock:
                            UpdateBotInfo(player.MaxMana, player.Mana, WowColors.warlockPrimaryBrush, WowColors.warlockSecondaryBrush);
                            break;

                        case WowClass.Warrior:
                            UpdateBotInfo(player.MaxRage, player.Rage, WowColors.warriorPrimaryBrush, WowColors.warriorSecondaryBrush);
                            break;
                    }

                    if (LabelUpdateEvent.Run())
                    {
                        UpdateBottomLabels();
                    }

                    if (DrawOverlay)
                    {
                        Overlay ??= new(AmeisenBot.Bot.Memory.Process.MainWindowHandle);
                        OverlayRenderCurrentPath();

                        Overlay?.Draw();
                        NeedToClearOverlay = true;
                    }
                    else if (NeedToClearOverlay)
                    {
                        Overlay.Clear();
                        NeedToClearOverlay = false;
                    }
                }
            });
        }

        /// <summary>
        /// Handles the Whisper event by checking for new messages and updating the notification UI accordingly.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event.</param>
        /// <param name="args">The arguments associated with the event.</param>
        private void OnWhisper(long timestamp, List<string> args)
        {
            if (!PendingNotification)
            {
                WowChatMessage message = AmeisenBot.Bot.Chat.ChatMessages
                    .Where(e => e.Timestamp > NotificationLastTimestamp)
                    .FirstOrDefault(e => e.Type == WowChat.WHISPER);

                if (message != null)
                {
                    PendingNotification = true;
                    NotificationLastTimestamp = message.Timestamp;

                    NoticifactionColor = message.Flags.Contains("GM", StringComparison.OrdinalIgnoreCase) ? NotificationGmBrush : NotificationBrush;
                }
            }
            else
            {
                if (NotificationBlinkState)
                {
                    buttonNotification.Foreground = DarkBackgroundBrush;
                    buttonNotification.Background = NoticifactionColor;
                }
                else
                {
                    buttonNotification.Foreground = NotificationWhiteBrush;
                    buttonNotification.Background = NotificationTransparentBrush;
                }

                NotificationBlinkState = !NotificationBlinkState;
            }
        }

        /// <summary>
        /// Renders the current path overlay on the screen.
        /// </summary>
        private void OverlayRenderCurrentPath()
        {
            if (AmeisenBot.Bot.Movement.Path != null
                && AmeisenBot.Bot.Movement.Path.Any())
            {
                // explicitly copy the path as it might change during rendering
                List<Vector3> currentNodes = AmeisenBot.Bot.Movement.Path.ToList();

                for (int i = 0; i < currentNodes.Count; ++i)
                {
                    Vector3 start = currentNodes[i];
                    Vector3 end = i == 0 ? AmeisenBot.Bot.Player.Position : currentNodes[i - 1];

                    System.Drawing.Color lineColor = System.Drawing.Color.White;
                    System.Drawing.Color startDot = System.Drawing.Color.Cyan;
                    System.Drawing.Color endDot = i == 0 ? System.Drawing.Color.Orange : i == currentNodes.Count ? System.Drawing.Color.Orange : System.Drawing.Color.Cyan;

                    Memory.Win32.Rect windowRect = AmeisenBot.Bot.Memory.GetClientSize();

                    if (OverlayMath.WorldToScreen(windowRect, AmeisenBot.Bot.Objects.Camera, start, out System.Drawing.Point startPoint)
                        && OverlayMath.WorldToScreen(windowRect, AmeisenBot.Bot.Objects.Camera, end, out System.Drawing.Point endPoint))
                    {
                        Overlay.AddLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, lineColor);
                        Overlay.AddRectangle(startPoint.X - 4, startPoint.Y - 4, 7, 7, startDot);
                        Overlay.AddRectangle(endPoint.X - 4, endPoint.Y - 4, 7, 7, endDot);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the position of the bot window if the configuration allows it.
        /// </summary>
        private void SaveBotWindowPosition()
        {
            if (AmeisenBot != null && AmeisenBot.Config != null && AmeisenBot.Config.SaveBotWindowPosition)
            {
                try
                {
                    Memory.Win32.Rect rc = new();
                    Memory.Win32.Win32Imports.GetWindowRect(MainWindowHandle, ref rc);
                    AmeisenBot.Config.BotWindowRect = rc;
                }
                catch (Exception e)
                {
                    AmeisenLogger.I.Log("AmeisenBot", $"Failed to save bot window position:\n{e}", LogLevel.Error);
                }
            }
        }

        /// <summary>
        /// Saves the AmeisenBot configuration to a file.
        /// </summary>
        private void SaveConfig()
        {
            if (AmeisenBot != null
                && AmeisenBot.Config != null
                && !string.IsNullOrWhiteSpace(AmeisenBot.Config.Path)
                && Directory.Exists(Path.GetDirectoryName(AmeisenBot.Config.Path)))
            {
                File.WriteAllText(AmeisenBot.Config.Path, JsonSerializer.Serialize(AmeisenBot.Config, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true }));
            }
        }

        /// <summary>
        /// Starts or pauses the AmeisenBot depending on its current state. 
        /// If the AmeisenBot is running, it will be paused and the start/pause button will be updated accordingly. 
        /// If the AmeisenBot is not running, it will be resumed and the start/pause button will be updated accordingly. 
        /// </summary>
        private void StartPause()
        {
            if (AmeisenBot.IsRunning)
            {
                AmeisenBot.Pause();
                buttonStartPause.Content = "▶";
                buttonStartPause.Foreground = TextAccentBrush;
            }
            else
            {
                AmeisenBot.Resume();
                buttonStartPause.Content = "||";
                buttonStartPause.Foreground = DarkForegroundBrush;
            }
        }

        ///<summary>
        ///Updates the information displayed in the bot's user interface.
        ///</summary>
        ///<param name="maxSecondary">The maximum value of the secondary progress bar.</param>
        ///<param name="secondary">The current value of the secondary progress bar.</param>
        ///<param name="primaryBrush">The brush used for the primary progress bar and class label.</param>
        ///<param name="secondaryBrush">The brush used for the secondary progress bar.</param>
        private void UpdateBotInfo(int maxSecondary, int secondary, Brush primaryBrush, Brush secondaryBrush)
        {
            labelPlayerName.Content = AmeisenBot.Bot.Db.GetUnitName(AmeisenBot.Bot.Player, out string name) ? name : "unknown";

            labelMapName.Content = AmeisenBot.Bot.Objects.MapId.ToString();
            labelZoneName.Content = AmeisenBot.Bot.Objects.ZoneName;
            labelZoneSubName.Content = AmeisenBot.Bot.Objects.ZoneSubName;

            labelCurrentLevel.Content = $"{AmeisenBot.Bot.Player.Level} (iLvl. {Math.Round(AmeisenBot.Bot.Character.Equipment.AverageItemLevel)})";
            labelCurrentRace.Content = $"{AmeisenBot.Bot.Player.Race} {AmeisenBot.Bot.Player.Gender}";
            labelCurrentClass.Content = AmeisenBot.Bot.Player.Class;

            progressbarExp.Maximum = AmeisenBot.Bot.Player.NextLevelXp;
            progressbarExp.Value = AmeisenBot.Bot.Player.Xp;
            labelCurrentExp.Content = $"{Math.Round(AmeisenBot.Bot.Player.XpPercentage)}%";

            progressbarHealth.Maximum = AmeisenBot.Bot.Player.MaxHealth;
            progressbarHealth.Value = AmeisenBot.Bot.Player.Health;
            labelCurrentHealth.Content = BotUtils.BigValueToString(AmeisenBot.Bot.Player.Health);

            labelCurrentCombatclass.Content = AmeisenBot.Bot.CombatClass == null ? $"No CombatClass" : AmeisenBot.Bot.CombatClass.ToString();

            progressbarSecondary.Maximum = maxSecondary;
            progressbarSecondary.Value = secondary;
            labelCurrentSecondary.Content = BotUtils.BigValueToString(secondary);

            progressbarHealth.Foreground = primaryBrush;
            progressbarSecondary.Foreground = secondaryBrush;
            labelCurrentClass.Foreground = primaryBrush;
        }

        /// <summary>
        /// Updates the labels at the bottom of the window.
        /// </summary>
        private void UpdateBottomLabels()
        {
            labelCurrentObjectCount.Content = AmeisenBot.Bot.Objects.ObjectCount.ToString(CultureInfo.InvariantCulture).PadLeft(4);

            float executionMs = AmeisenBot.CurrentExecutionMs;

            if (float.IsNaN(executionMs) || float.IsInfinity(executionMs))
            {
                executionMs = 0;
            }

            labelCurrentTickTime.Content = executionMs.ToString(CultureInfo.InvariantCulture).PadLeft(4);

            if (executionMs <= AmeisenBot.Config.StateMachineTickMs)
            {
                labelCurrentTickTime.Foreground = CurrentTickTimeGoodBrush;
            }
            else
            {
                labelCurrentTickTime.Foreground = CurrentTickTimeBadBrush;
                AmeisenLogger.I.Log("MainWindow", $"High executionMs ({executionMs}), something blocks our thread or CPU is to slow", LogLevel.Warning);
            }

            labelHookCallCount.Content = AmeisenBot.Bot.Wow.HookCallCount.ToString(CultureInfo.InvariantCulture).PadLeft(2);

            if (AmeisenBot.Bot.Wow.HookCallCount <= (AmeisenBot.Bot.Player.IsInCombat ? AmeisenBot.Config.MaxFpsCombat : AmeisenBot.Config.MaxFps))
            {
                labelHookCallCount.Foreground = CurrentTickTimeGoodBrush;
            }
            else
            {
                labelHookCallCount.Foreground = CurrentTickTimeBadBrush;
                AmeisenLogger.I.Log("MainWindow", "High HookCall count, maybe increase your FPS", LogLevel.Warning);
            }

            labelRpmCallCount.Content = AmeisenBot.Bot.Memory.RpmCallCount.ToString(CultureInfo.InvariantCulture).PadLeft(5);
            labelWpmCallCount.Content = AmeisenBot.Bot.Memory.WpmCallCount.ToString(CultureInfo.InvariantCulture).PadLeft(3);
        }

        /// <summary>
        /// Event handler for the window closing event. It performs various actions to clean up and save data before the window is closed.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveBotWindowPosition();

            KeyboardHook.Disable();

            Overlay?.Exit();
            AmeisenBot?.Dispose();

            InfoWindow?.Close();
            MapWindow?.Close();
            DevToolsWindow?.Close();
            RelationshipWindow?.Close();

            if (StateConfigWindows != null)
            {
                foreach (Window window in StateConfigWindows.Values)
                {
                    window.Close();
                }
            }

            SaveConfig();

            AmeisenLogger.I.Stop();
        }

        /// <summary>
        /// Executes when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // obtain a window handle (HWND) to out current WPF window
            MainWindowHandle = new WindowInteropHelper(this).EnsureHandle();

            comboboxStateOverride.Items.Add(BotMode.None);
            comboboxStateOverride.Items.Add(BotMode.Grinding);
            comboboxStateOverride.Items.Add(BotMode.Jobs);
            comboboxStateOverride.Items.Add(BotMode.PvP);
            comboboxStateOverride.Items.Add(BotMode.Questing);
            comboboxStateOverride.Items.Add(BotMode.Testing);

            comboboxStateOverride.SelectedIndex = 0;

            // display the PID, maybe remove this when not debugging
            labelPID.Content = $"PID: {Environment.ProcessId}";

            if (TryLoadConfig(ConfigPath, out AmeisenBotConfig config))
            {
                AmeisenBot = new(Path.GetFileName(config.Path), config);

                // capture whisper messages and display them in the bots ui as a flashing button
                AmeisenBot.Bot.Wow.Events?.Subscribe("CHAT_MSG_WHISPER", OnWhisper);

                // events used to update our GUI
                AmeisenBot.Bot.Objects.OnObjectUpdateComplete += OnObjectUpdateComplete;

                // handle the autoposition function where the wow window gets "absorbed" by the bots window
                if (AmeisenBot.Config.AutoPositionWow)
                {
                    // this is used to measure the size of wow's window
                    PresentationSource presentationSource = PresentationSource.FromVisual(this);
                    M11 = presentationSource.CompositionTarget.TransformToDevice.M11;
                    M22 = presentationSource.CompositionTarget.TransformToDevice.M22;

                    AmeisenBot.Logic.OnWoWStarted += () =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            AmeisenBot.Bot.Memory.SetupAutoPosition
                            (
                                MainWindowHandle,
                                (int)((wowRect.Margin.Left + 1) * M11),
                                (int)((wowRect.Margin.Top + 1) * M22),
                                (int)((wowRect.ActualWidth - 1) * M11),
                                (int)((wowRect.ActualHeight - 1) * M22)
                            );
                        });

                        IsAutoPositionSetup = true;
                    };
                }

                AmeisenLogger.I.Log("AmeisenBot", "Loading Hotkeys", LogLevel.Verbose);
                LoadHotkeys();

                AmeisenBot.Start();

                StateConfigWindows = new()
                {
                    { BotMode.Jobs, new StateJobConfigWindow(AmeisenBot, AmeisenBot.Config) },
                    { BotMode.Grinding, new StateGrindingConfigWindow(AmeisenBot, AmeisenBot.Config) },
                    { BotMode.Questing, new StateQuestingConfigWindow(AmeisenBot, AmeisenBot.Config) },
                };

                if (AmeisenBot.Config.Autopilot)
                {
                    buttonToggleAutopilot.Foreground = CurrentTickTimeGoodBrush;
                }

                // load our old window position
                if (AmeisenBot.Config.SaveBotWindowPosition)
                {
                    if (MainWindowHandle != IntPtr.Zero && AmeisenBot.Config.BotWindowRect != new Memory.Win32.Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 })
                    {
                        AmeisenBot.Bot.Memory.SetWindowPosition(MainWindowHandle, AmeisenBot.Config.BotWindowRect);
                        AmeisenLogger.I.Log("AmeisenBot", $"Loaded window position: {AmeisenBot.Config.BotWindowRect}", LogLevel.Verbose);
                    }
                    else
                    {
                        AmeisenLogger.I.Log("AmeisenBot", $"Unable to load window position of {MainWindowHandle} to {AmeisenBot.Config.BotWindowRect}", LogLevel.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Check your config, maybe it contains some invalid stuff.\n\n{ConfigPath}", "Failed to load Config", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Event handler for the left mouse button down event on the Window control.
        /// This method initiates dragging of the window when the left mouse button is pressed down.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}