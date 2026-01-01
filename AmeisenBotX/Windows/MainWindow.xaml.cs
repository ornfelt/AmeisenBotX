using AmeisenBotX.BehaviorTree.Objects;
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
using AmeisenBotX.Models;
using AmeisenBotX.Overlay;
using AmeisenBotX.Overlay.Utils;
using AmeisenBotX.Utils;
using AmeisenBotX.Windows.StateConfig;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace AmeisenBotX.Windows
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

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
            TextAccentBrush = new SolidColorBrush((Color)FindResource("TextAccentColor"));
            AccentBrush = (SolidColorBrush)FindResource("AccentBrush");

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

        private Brush AccentBrush { get; }

        private string ConfigPath { get; }

        private Brush CurrentTickTimeBadBrush { get; }

        private Brush CurrentTickTimeGoodBrush { get; }
        
        private AutopilotDebugWindow AutopilotDebugWindow { get; set; }

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
                    // Calculate position relative to screen to avoid Border/Chrome offsets
                    Point wowRectScreen = wowRect.PointToScreen(new Point(0, 0));
                    Point windowScreen = this.PointToScreen(new Point(0, 0));
                    Vector wowRectSize = wowRect.PointToScreen(new Point(wowRect.ActualWidth, wowRect.ActualHeight)) - wowRectScreen;

                    AmeisenBot.Bot.Memory.ResizeParentWindow
                    (
                        (int)(wowRectScreen.X - windowScreen.X),
                        (int)(wowRectScreen.Y - windowScreen.Y),
                        (int)wowRectSize.X,
                        (int)wowRectSize.Y
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
            Views.Config.DynamicConfigWindow configWindow = new(AmeisenBot.Config, AmeisenBot.CombatClasses);
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
            if (sender is System.Windows.Controls.Primitives.ToggleButton toggle)
            {
                AmeisenBot.Config.Autopilot = toggle.IsChecked ?? false;
            }
        }

        private void ButtonToggleAutopilotDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            AutopilotDebugWindow ??= new(AmeisenBot);
            AutopilotDebugWindow.Show();
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
            if (sender is System.Windows.Controls.Primitives.ToggleButton toggle)
            {
                DrawOverlay = toggle.IsChecked ?? false;
            }
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
                using System.Drawing.Bitmap portrait = Core.Utils.PortraitCapture.CapturePortrait(
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
                    if (PendingNotification)
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

                // Skip if custom portrait is set (UseCustomPortrait means user set one via config)
                // or if portrait already exists
                if (AmeisenBot.Config.UseCustomPortrait || File.Exists(portraitPath))
                {
                    PortraitCapturedThisSession = true;
                    return;
                }

                nint hwnd = AmeisenBot?.Bot?.Memory?.Process?.MainWindowHandle ?? nint.Zero;

                // Skip if minimized or not in game
                if (hwnd == nint.Zero || IsIconic(hwnd))
                {
                    return;
                }

                if (AmeisenBot?.Bot?.Wow == null || AmeisenBot.Bot.Player == null || !AmeisenBot.Bot.Objects.IsWorldLoaded)
                {
                    return;
                }

                using System.Drawing.Bitmap portrait = Core.Utils.PortraitCapture.CapturePortrait(
                    hwnd,
                    AmeisenBot.Bot.Wow.LuaDoString,
                    unit: "player",
                    outputSize: 128);

                if (portrait != null)
                {
                    portrait.Save(portraitPath, System.Drawing.Imaging.ImageFormat.Png);
                    AmeisenLogger.I.Log("PortraitCapture", $"Auto-captured: {portraitPath}", LogLevel.Debug);
                }

                PortraitCapturedThisSession = true;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("PortraitCapture", $"Failed: {ex.Message}", LogLevel.Warning);
                PortraitCapturedThisSession = true;
            }
        }

        /// <summary>
        /// Updates the portrait before exiting to keep it fresh.
        /// </summary>
        /// <summary>
        /// Updates the portrait before exiting to keep it fresh.
        /// </summary>
        private void UpdatePortraitOnExit()
        {
            try
            {
                // If using custom portrait, do not overwrite on exit
                if (AmeisenBot.Config.UseCustomPortrait)
                {
                    SaveProfileStats();
                    return;
                }

                nint hwnd = AmeisenBot?.Bot?.Memory?.Process?.MainWindowHandle ?? nint.Zero;

                // 1. Check if Minimized (Iconic)
                if (hwnd == nint.Zero || IsIconic(hwnd))
                {
                    // Skip if minimized
                    SaveProfileStats();
                    return;
                }

                // 2. Check if actually InGame
                if (AmeisenBot?.Bot?.Wow == null || AmeisenBot.Bot.Player == null || !AmeisenBot.Bot.Objects.IsWorldLoaded)
                {
                    SaveProfileStats();
                    return;
                }

                using System.Drawing.Bitmap portrait = Core.Utils.PortraitCapture.CapturePortrait(
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
                IWowPlayer player = AmeisenBot?.Bot?.Player;
                if (player == null)
                {
                    return;
                }

                // Get player name from database
                string playerName = AmeisenBot.Bot.Db.GetUnitName(player, out string name) ? name : "Unknown";

                ProfileStats stats = new()
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
                buttonStartPause.Foreground = CurrentTickTimeGoodBrush; // Green for "Start" (Paused state)
            }
            else
            {
                AmeisenBot.Resume();
                buttonStartPause.Content = "⏸";
                buttonStartPause.Foreground = AccentBrush; // Scheme Color for "Pause" (Running state)
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

            labelCurrentCombatclass.Text = AmeisenBot.Bot.CombatClass == null ? $"No CombatClass" : $"{AmeisenBot.Bot.CombatClass.DisplayName}";

            progressbarSecondary.Maximum = maxSecondary;
            progressbarSecondary.Value = secondary;
            labelCurrentSecondary.Content = BotUtils.BigValueToString(secondary);

            progressbarHealth.Foreground = primaryBrush;
            progressbarSecondary.Foreground = secondaryBrush;
            labelCurrentClass.Foreground = primaryBrush;

            // Update Theme if Class changes
            if (AmeisenBot.Bot.Player.Class != _lastWowClass)
            {
                Themes.ThemeManager.ApplyClassTheme(AmeisenBot.Bot.Player.Class);
                _lastWowClass = AmeisenBot.Bot.Player.Class;
            }
        }

        private WowClass? _lastWowClass;


        private void UpdateBottomLabels()
        {
            labelCurrentObjectCount.Text = AmeisenBot.Bot.Objects.ObjectCount.ToString(CultureInfo.InvariantCulture).PadLeft(4);

            float executionMs = AmeisenBot.CurrentExecutionMs;

            if (float.IsNaN(executionMs) || float.IsInfinity(executionMs))
            {
                executionMs = 0;
            }

            labelCurrentTickTime.Text = executionMs.ToString(CultureInfo.InvariantCulture).PadLeft(4);

            if (executionMs <= AmeisenBot.Config.StateMachineTickMs)
            {
                labelCurrentTickTime.Foreground = CurrentTickTimeGoodBrush;
            }
            else
            {
                labelCurrentTickTime.Foreground = CurrentTickTimeBadBrush;
                AmeisenLogger.I.Log("MainWindow", $"High executionMs ({executionMs}), something blocks our thread or CPU is to slow", LogLevel.Warning);
            }

            // Navmesh Status Update
            if (AmeisenBot.Bot.PathfindingHandler.IsConnected)
            {
                double latency = AmeisenBot.Bot.PathfindingHandler.AverageLatency;

                string unit;
                string value;

                if (latency < 0.001)
                {
                    unit = " ns";
                    value = $"{latency * 1000000:0}";
                }
                else if (latency < 1.0)
                {
                    unit = " µs";
                    value = $"{latency * 1000:0}";
                }
                else
                {
                    unit = " ms";
                    value = $"{latency:0}";
                }

                labelNavStatus.Text = value;
                labelNavUnit.Text = unit;
                labelNavUnit.Visibility = Visibility.Visible;

                labelNavStatus.Foreground = latency < 20 ? CurrentTickTimeGoodBrush : latency < 50 ? Brushes.Yellow : CurrentTickTimeBadBrush;
            }
            else
            {
                labelNavStatus.Text = "DC";
                labelNavUnit.Visibility = Visibility.Collapsed;
                labelNavStatus.Foreground = CurrentTickTimeBadBrush;
            }

            labelHookCallCount.Text = AmeisenBot.Bot.Wow.HookCallCount.ToString(CultureInfo.InvariantCulture).PadLeft(2);

            if (AmeisenBot.Bot.Wow.HookCallCount <= (AmeisenBot.Bot.Player.IsInCombat ? AmeisenBot.Config.MaxFpsCombat : AmeisenBot.Config.MaxFps))
            {
                labelHookCallCount.Foreground = CurrentTickTimeGoodBrush;
            }
            else
            {
                labelHookCallCount.Foreground = CurrentTickTimeBadBrush;
                AmeisenLogger.I.Log("MainWindow", "High HookCall count, maybe increase your FPS", LogLevel.Warning);
            }

            labelRpmCallCount.Text = AmeisenBot.Bot.Memory.RpmCallCount.ToString(CultureInfo.InvariantCulture).PadLeft(5);
            labelWpmCallCount.Text = AmeisenBot.Bot.Memory.WpmCallCount.ToString(CultureInfo.InvariantCulture).PadLeft(3);

            // Update behavior tree state
            if (AmeisenBot.Bot.BehaviorTree != null)
            {
                INode lastNode = AmeisenBot.Bot.BehaviorTree.LastExecutedNode;
                labelBtState.Text = lastNode?.Name ?? AmeisenBot.Bot.BehaviorTree.LastStatus.ToString();
            }
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
            AutopilotDebugWindow?.Close();

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
            comboboxStateOverride.Items.Add(BotMode.Auto);
            comboboxStateOverride.Items.Add(BotMode.Grinding);
            comboboxStateOverride.Items.Add(BotMode.Jobs);
            comboboxStateOverride.Items.Add(BotMode.PvP);
            comboboxStateOverride.Items.Add(BotMode.Questing);
            comboboxStateOverride.Items.Add(BotMode.Testing);

            comboboxStateOverride.SelectedIndex = 0;

            // display the PID, maybe remove this when not debugging
            labelPID.Text = Environment.ProcessId.ToString();

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
                    if (presentationSource != null)
                    {
                        M11 = presentationSource.CompositionTarget.TransformToDevice.M11;
                        M22 = presentationSource.CompositionTarget.TransformToDevice.M22;
                    }

                    AmeisenBot.Logic.OnWoWStarted += () =>
                    {
                        Dispatcher.Invoke(() => UpdateWowWindowPosition());
                    };

                    // If WoW is already running, trigger immediately!
                    if (AmeisenBot.Bot.Memory.Process != null && !AmeisenBot.Bot.Memory.Process.HasExited)
                    {
                        Dispatcher.Invoke(() => UpdateWowWindowPosition());
                    }
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

                buttonToggleAutopilot.IsChecked = AmeisenBot.Config.Autopilot;

                // Initialize Start/Pause button state
                if (AmeisenBot.IsRunning)
                {
                    buttonStartPause.Content = "⏸";
                    buttonStartPause.Foreground = AccentBrush;
                }
                else
                {
                    buttonStartPause.Content = "▶";
                    buttonStartPause.Foreground = CurrentTickTimeGoodBrush;
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(WndProc);
        }

        private const int WM_SIZING = 0x0214;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 7;
        private const int WMSZ_BOTTOMRIGHT = 8;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            if (msg == WM_SIZING)
            {
                // Marshal the RECT from lParam
                RECT rect = Marshal.PtrToStructure<RECT>(lParam);

                // Fixed dimensions of bot chrome
                double sidePanelAndBorder = 200 + (ActualWidth - wowRect.ActualWidth); // ~208 with border? Actually safer to use: ActualWidth - wowRect.ActualWidth + 200
                // We know Left Panel is 200. Let's use hardcoded approximations or calculate dynamically?
                // Calculate dynamic chrome size:
                double chromeWidth = this.ActualWidth - wowRect.ActualWidth;
                double chromeHeight = this.ActualHeight - wowRect.ActualHeight;

                // Usually Left Panel 200 + Borders. 
                // Let's assume Left Panel Width is constant 200.

                // Add margins/padding if any. 
                // Grid borders: +1 left, +1 right? Window border?
                // Total Window Width = 200 + WoWWidth.
                // Total Window Height = 56 + WoWHeight.

                double w = rect.Right - rect.Left;
                double h = rect.Bottom - rect.Top;

                // Adjust w or h to match aspect ratio
                // Target: (w - 200) / (h - 56) = 4/3
                // => w - 200 = (h - 56) * 1.333
                // => w = ((h - 56) * 1.333) + 200

                // Or: h - 56 = (w - 200) * 0.75
                // => h = ((w - 200) * 0.75) + 56

                // Constants including typical window chrome (borders/caption don't exist in custom window? Wait, WindowStyle=None, ResizeMode=CanResizeWithGrip)
                // ResizeMode=CanResizeWithGrip adds some non-client area? No, WindowStyle=None removes standard frame.
                // We draw our own borders. 
                // So Total Width = Client Width.

                // Calculate offsets based on current measurement
                // Use consts for safety based on XAML
                const double SidebarWidth = 200.0;
                const double TopBottomHeight = 56.0;

                int edge = (int)wParam;

                if (edge is WMSZ_LEFT or WMSZ_RIGHT)
                {
                    // Width is master, adjust Height
                    // h = ((w - SidebarWidth) * 0.75) + TopBottomHeight
                    double newW = w;
                    double newH = ((newW - SidebarWidth) * 0.75) + TopBottomHeight;

                    if (edge == WMSZ_LEFT)
                    {
                        // Logic for left resize? Standard behavior is just width change.
                        // But usually we want to constrain the aspect. 
                        // If I drag RIGHT edge, I change Width -> Calculate new Height.
                        rect.Bottom = rect.Top + (int)newH;
                    }
                    else
                    {
                        rect.Bottom = rect.Top + (int)newH;
                    }
                }
                else if (edge is WMSZ_TOP or WMSZ_BOTTOM)
                {
                    // Height is master, adjust Width
                    // w = ((h - TopBottomHeight) * 1.333) + SidebarWidth
                    double newH = h;
                    double newW = ((newH - TopBottomHeight) * (4.0 / 3.0)) + SidebarWidth;

                    rect.Right = rect.Left + (int)newW;
                }
                else
                {
                    // Diagonals. Which one dominates? Usually Width.
                    // Let's defer to Width logic.
                    // w = ((h - 56) * 1.33) + 200 ?? No.

                    // Let's enforce WIDTH based on HEIGHT for smoothness?
                    // Or prioritize Width? 
                    // Let's prioritize Width:
                    double newW = w;
                    double newH = ((newW - SidebarWidth) * 0.75) + TopBottomHeight;

                    if (edge is WMSZ_TOPLEFT or WMSZ_TOPRIGHT)
                    {
                        // Stick to bottom? No, top changes.
                        // Actually if I drag TopLeft, Top and Left change.
                        // I set rect.Top based on new Height? 

                        // If TopLeft: Left changes Width. Top changes Height.
                        // Let's recalculate Height based on Width.

                        // rect.Top = rect.Bottom - (int)newH; 
                        // But dragging TOPLEFT moves Top AND Left.

                        // Standard practice: Adjust the coordinate that ISN'T fixed.
                        // TOPLEFT: Bottom and Right are anchors. 
                        // Adjust Top to match new Width (determined by Left).
                        rect.Top = rect.Bottom - (int)newH;
                    }
                    else // BOTTOMLEFT or BOTTOMRIGHT
                    {
                        // Top is anchor. Left/Right moves width.
                        // Adjust Bottom.
                        rect.Bottom = rect.Top + (int)newH;
                    }

                }

                Marshal.StructureToPtr(rect, lParam, true);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void StatusBarBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded && e.PreviousSize.Height > 0)
            {
                double diff = e.NewSize.Height - e.PreviousSize.Height;
                if (Math.Abs(diff) > 0.1)
                {
                    this.Height += diff;

                    // Adjust WoW window immediately
                    if (AmeisenBot.Config.AutoPositionWow)
                    {
                        UpdateWowWindowPosition(true);
                    }
                }
            }
        }

        private void UpdateWowWindowPosition(bool force = false)
        {
            if (IsAutoPositionSetup && !force)
            {
                return;
            }

            if (AmeisenBot?.Bot?.Memory == null)
            {
                return;
            }

            // Get the actual position of wowRect relative to the window
            // This fixes the bug where Margin was used (which is 0,0 in new layout)
            // Calculate position relative to screen to avoid Border/Chrome offsets
            Point wowRectScreen = wowRect.PointToScreen(new Point(0, 0));
            Point windowScreen = this.PointToScreen(new Point(0, 0));
            Vector wowRectSize = wowRect.PointToScreen(new Point(wowRect.ActualWidth, wowRect.ActualHeight)) - wowRectScreen;

            AmeisenBot.Bot.Memory.SetupAutoPosition
            (
                new WindowInteropHelper(this).Handle,
                (int)(wowRectScreen.X - windowScreen.X),
                (int)(wowRectScreen.Y - windowScreen.Y),
                (int)wowRectSize.X,
                (int)wowRectSize.Y
            );

            IsAutoPositionSetup = true;
        }
    }
}