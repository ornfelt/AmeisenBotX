using AmeisenBotX.Common.Keyboard;
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
        private static readonly JsonSerializerOptions Options = new()
        {
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true,
        };

        private static readonly JsonSerializerOptions OptionsFields = new()
        {
            WriteIndented = true,
            IncludeFields = true
        };

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

        public bool IsAutoPositionSetup { get; private set; }

        public KeyboardHook KeyboardHook { get; }

        public double M11 { get; private set; }

        public double M22 { get; private set; }

        public AmeisenBotOverlay Overlay { get; private set; }

        public bool RenderState { get; set; }

        private AmeisenBot AmeisenBot { get; set; }

        private string ConfigPath { get; }

        private Brush CurrentTickTimeBadBrush { get; }

        private Brush CurrentTickTimeGoodBrush { get; }

        private Brush DarkBackgroundBrush { get; }

        private Brush DarkForegroundBrush { get; }

        private string DataPath { get; }

        private DevToolsWindow DevToolsWindow { get; set; }

        private bool DrawOverlay { get; set; }

        private InfoWindow InfoWindow { get; set; }

        private TimegatedEvent LabelUpdateEvent { get; }

        private nint MainWindowHandle { get; set; }

        private MapWindow MapWindow { get; set; }

        private bool NeedToClearOverlay { get; set; }

        private SolidColorBrush NoticifactionColor { get; set; }

        private bool NotificationBlinkState { get; set; }

        private SolidColorBrush NotificationBrush { get; }

        private SolidColorBrush NotificationGmBrush { get; }

        private long NotificationLastTimestamp { get; set; }

        private SolidColorBrush NotificationTransparentBrush { get; }

        private SolidColorBrush NotificationWhiteBrush { get; }

        private bool PendingNotification { get; set; }

        private RelationshipWindow RelationshipWindow { get; set; }

        private Dictionary<BotMode, Window> StateConfigWindows { get; set; }

        private Brush TextAccentBrush { get; }

        private bool PortraitCapturedThisSession { get; set; }

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
        private static bool TryLoadConfig(string configPath, out AmeisenBotConfig config)
        {
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                config = File.Exists(configPath)
                    ? JsonSerializer.Deserialize<AmeisenBotConfig>(File.ReadAllText(configPath), Options)
                    : new();

                config.Path = configPath;
                return true;
            }

            config = null;
            return false;
        }

        private void ButtonClearCache_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Bot.Db.Clear();
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            Views.Config.DynamicConfigWindow configWindow = new(AmeisenBot.Config);
            if (configWindow.ShowDialog() == true)
            {
                // Changes are already bound to the object, so we just reload and save
                AmeisenBot.ReloadConfig(AmeisenBot.Config);
                File.WriteAllText(AmeisenBot.Config.Path, JsonSerializer.Serialize(AmeisenBot.Config, Options));

                KeyboardHook.Clear();
                LoadHotkeys();
            }
        }

        private void ButtonDevTools_Click(object sender, RoutedEventArgs e)
        {
            DevToolsWindow ??= new(AmeisenBot);
            DevToolsWindow.Show();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonNotification_Click(object sender, RoutedEventArgs e)
        {
            PendingNotification = false;
            NotificationBlinkState = false;

            buttonNotification.Foreground = NotificationWhiteBrush;
            buttonNotification.Background = NotificationTransparentBrush;
        }

        private void ButtonStartPause_Click(object sender, RoutedEventArgs e)
        {
            StartPause();
        }

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

        private void ButtonToggleAutopilot_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.Autopilot = !AmeisenBot.Config.Autopilot;
            buttonToggleAutopilot.Foreground = AmeisenBot.Config.Autopilot ? CurrentTickTimeGoodBrush : DarkForegroundBrush;
        }

        private void ButtonToggleInfoWindow_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow ??= new(AmeisenBot);
            InfoWindow.Show();
        }

        private void ButtonToggleMapWindow_Click(object sender, RoutedEventArgs e)
        {
            MapWindow ??= new(AmeisenBot);
            MapWindow.Show();
        }

        private void ButtonToggleOverlay_Click(object sender, RoutedEventArgs e)
        {
            DrawOverlay = !DrawOverlay;
            buttonToggleOverlay.Foreground = DrawOverlay ? CurrentTickTimeGoodBrush : DarkForegroundBrush;
        }

        private void ButtonToggleRelationshipWindow_Click(object sender, RoutedEventArgs e)
        {
            RelationshipWindow ??= new(AmeisenBot);
            RelationshipWindow.Show();
        }

        private void ButtonToggleRendering_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nint wowHwnd = AmeisenBot.Bot.Memory.Process?.MainWindowHandle ?? nint.Zero;

                if (wowHwnd == nint.Zero)
                {
                    AmeisenLogger.I.Log("PortraitCapture", "WoW window not available", LogLevel.Warning);
                    return;
                }

                // Capture portrait using the full workflow helper
                using var portrait = Core.Utils.PortraitCapture.CapturePortrait(
                    wowHwnd,
                    AmeisenBot.Bot.Wow.LuaDoString,
                    unit: "player",
                    outputSize: 128,
                    renderDelayMs: 100);

                if (portrait != null)
                {
                    string portraitPath = Path.Combine(Path.GetDirectoryName(ConfigPath), "portrait.png");
                    portrait.Save(portraitPath, System.Drawing.Imaging.ImageFormat.Png);
                    AmeisenLogger.I.Log("PortraitCapture", $"Saved: {portraitPath}", LogLevel.Debug);
                }
                else
                {
                    AmeisenLogger.I.Log("PortraitCapture", "Capture failed", LogLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("PortraitCapture", $"Error: {ex.Message}", LogLevel.Error);
            }
        }

        private void ComboboxStateOverride_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AmeisenBot != null)
            {
                ((AmeisenBotLogic)AmeisenBot.Logic).ChangeMode((BotMode)comboboxStateOverride.SelectedItem);
                buttonStateConfig.IsEnabled = StateConfigWindows.ContainsKey((BotMode)comboboxStateOverride.SelectedItem);
            }
        }

        private void LoadHotkeys()
        {
            if (AmeisenBot.Config.Hotkeys.TryGetValue("StartStop", out Keybind kv))
            {
                KeyboardHook.AddHotkey((KeyCode)kv.Key, (KeyCode)kv.Mod, StartPause);
            }
        }

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

                        // Auto-capture portrait on first login if it doesn't exist
                        if (!PortraitCapturedThisSession)
                        {
                            CapturePortraitIfMissing();
                        }
                    }

                    labelCombatState.Content = AmeisenBot.Bot.Player.IsInCombat;

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
        private void SaveConfig()
        {
            if (AmeisenBot != null
                && AmeisenBot.Config != null
                && !string.IsNullOrWhiteSpace(AmeisenBot.Config.Path)
                && Directory.Exists(Path.GetDirectoryName(AmeisenBot.Config.Path)))
            {
                File.WriteAllText(AmeisenBot.Config.Path, JsonSerializer.Serialize(AmeisenBot.Config, OptionsFields));
            }
        }

        /// <summary>
        /// Captures the player portrait if it doesn't exist yet.
        /// Called on first login to generate the portrait.png for the profile.
        /// </summary>
        private void CapturePortraitIfMissing()
        {
            try
            {
                string portraitPath = Path.Combine(Path.GetDirectoryName(ConfigPath), "portrait.png");

                if (!File.Exists(portraitPath))
                {
                    nint hwnd = AmeisenBot?.Bot?.Memory?.Process?.MainWindowHandle ?? nint.Zero;

                    if (hwnd != nint.Zero && AmeisenBot?.Bot?.Wow != null)
                    {
                        using var portrait = Core.Utils.PortraitCapture.CapturePortrait(
                            hwnd,
                            AmeisenBot.Bot.Wow.LuaDoString,
                            unit: "player",
                            outputSize: 128);

                        if (portrait != null)
                        {
                            portrait.Save(portraitPath, System.Drawing.Imaging.ImageFormat.Png);
                            AmeisenLogger.I.Log("PortraitCapture", $"Auto-captured portrait: {portraitPath}", LogLevel.Debug);
                        }
                    }
                }

                PortraitCapturedThisSession = true;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("PortraitCapture", $"Auto-capture failed: {ex.Message}", LogLevel.Warning);
                PortraitCapturedThisSession = true; // Don't retry
            }
        }

        /// <summary>
        /// Updates the portrait before exiting to keep it fresh.
        /// </summary>
        private void UpdatePortraitOnExit()
        {
            try
            {
                nint hwnd = AmeisenBot?.Bot?.Memory?.Process?.MainWindowHandle ?? nint.Zero;

                if (hwnd != nint.Zero && AmeisenBot?.Bot?.Wow != null)
                {
                    using var portrait = Core.Utils.PortraitCapture.CapturePortrait(
                        hwnd,
                        AmeisenBot.Bot.Wow.LuaDoString,
                        unit: "player",
                        outputSize: 128);

                    if (portrait != null)
                    {
                        string portraitPath = Path.Combine(Path.GetDirectoryName(ConfigPath), "portrait.png");
                        portrait.Save(portraitPath, System.Drawing.Imaging.ImageFormat.Png);
                        AmeisenLogger.I.Log("PortraitCapture", $"Updated portrait on exit: {portraitPath}", LogLevel.Debug);
                    }
                }

                // Save character stats
                SaveProfileStats();
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("PortraitCapture", $"Exit update failed: {ex.Message}", LogLevel.Warning);
            }
        }

        /// <summary>
        /// Saves character stats (name, level, class, realm) to the profile folder.
        /// </summary>
        private void SaveProfileStats()
        {
            try
            {
                var player = AmeisenBot?.Bot?.Player;
                if (player == null) return;

                // Get player name from database
                string playerName = AmeisenBot.Bot.Db.GetUnitName(player, out string name) ? name : "Unknown";

                var stats = new Models.ProfileStats
                {
                    CharacterName = playerName,
                    Level = player.Level,
                    Class = player.Class.ToString(),
                    Realm = AmeisenBot.Config?.Realm ?? "",
                    Zone = AmeisenBot.Bot.Objects?.ZoneName ?? "",
                    LastPlayed = DateTime.Now,
                    Faction = GetPlayerFaction()
                };

                string profileFolder = Path.GetDirectoryName(ConfigPath);
                stats.Save(profileFolder);

                AmeisenLogger.I.Log("ProfileStats", $"Saved stats: {stats.CharacterName} Lv{stats.Level} {stats.Class}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("ProfileStats", $"Failed to save stats: {ex.Message}", LogLevel.Warning);
            }
        }

        /// <summary>
        /// Gets the player's faction as a string.
        /// </summary>
        private string GetPlayerFaction()
        {
            try
            {
                return AmeisenBot?.Bot?.Player?.IsAlliance() == true ? "Alliance" : "Horde";
            }
            catch
            {
                return "";
            }
        }

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Update portrait before exit
            UpdatePortraitOnExit();

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
                                new WindowInteropHelper(this).Handle,
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
                    if (MainWindowHandle != nint.Zero && AmeisenBot.Config.BotWindowRect != new Memory.Win32.Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 })
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ButtonAiDebug_Click(object sender, RoutedEventArgs e)
        {
            AiDebugWindow win = new(AmeisenBot.Bot);
            win.Show();
        }

        private void ButtonBtDebug_Click(object sender, RoutedEventArgs e)
        {
            BehaviorTreeDebugWindow win = new(AmeisenBot.Bot);
            win.Show();
        }

    }
}