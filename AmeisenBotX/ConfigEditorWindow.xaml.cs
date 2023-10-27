using AmeisenBotX.Common.Keyboard.Enums;
using AmeisenBotX.Common.Keyboard.Objects;
using AmeisenBotX.Core;
using AmeisenBotX.Core.Engines.Battleground;
using AmeisenBotX.Core.Logic.Idle.Actions;
using AmeisenBotX.Utils;
using AmeisenBotX.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AmeisenBotX
{
    /// <summary>
    /// Creates a new instance of the ConfigEditorWindow class.
    /// </summary>
    /// <param name="dataDir">The data directory path.</param>
    /// <param name="ameisenBot">A reference to an AmeisenBot instance.</param>
    /// <param name="initialConfig">An optional initial AmeisenBotConfig instance.</param>
    /// <param name="initialConfigName">An optional initial config name.</param>
    public partial class ConfigEditorWindow : Window
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="dataDir">The data directory path.</param>
        /// <param name="ameisenBot">A <see cref="AmeisenBot"/> instance.</param>
        /// <param name="initialConfig">A <see cref="AmeisenBotConfig"/> instance (Optional).</param>
        /// <param name="initialConfigName">A initial config name (Optional).</param>
        public ConfigEditorWindow(string dataDir, AmeisenBot ameisenBot, AmeisenBotConfig initialConfig = null, string initialConfigName = "")
        {
            InitializeComponent();

            DataDir = dataDir;
            NewConfig = initialConfig == null;
            Config = initialConfig ?? new();
            AmeisenBot = ameisenBot;
            ConfigName = initialConfigName;
            SaveConfig = NewConfig;

            NormalBorderBrush = new((Color)FindResource("DarkBorder"));
            ErrorBorderBrush = new((Color)FindResource("DarkError"));

            NormalBorderBrush.Freeze();
            ErrorBorderBrush.Freeze();
        }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBot class.
        /// </summary>
        public AmeisenBot AmeisenBot { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the action should be canceled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether something has changed.
        /// </summary>
        public bool ChangedSomething { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the AmeisenBot.
        /// </summary>
        public AmeisenBotConfig Config { get; private set; }

        /// <summary>
        /// Gets or sets the configuration name.
        /// </summary>
        public string ConfigName { get; private set; }

        /// <summary>
        /// Gets the directory where the data is stored.
        /// </summary>
        public string DataDir { get; }

        /// <summary>
        /// Gets or sets the list of IdleActionWrapper items.
        /// </summary>
        public List<IdleActionWrapper> IdleActionItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a new configuration is being used.
        /// </summary>
        public bool NewConfig { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the config should be saved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the config should be saved; otherwise, <c>false</c>.
        /// </value>
        public bool SaveConfig { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether the window has been loaded.
        /// </summary>
        public bool WindowLoaded { get; set; }

        /// <summary>
        /// Gets or sets the solid color brush used for the error border.
        /// </summary>
        private SolidColorBrush ErrorBorderBrush { get; }

        /// <summary>
        /// Gets the SolidColorBrush used for the normal border of an element.
        /// </summary>
        private SolidColorBrush NormalBorderBrush { get; }

        /// <summary>
        /// Adds all the available battleground engines to the combobox and sets the default selection to "None".
        /// </summary>
        private void AddBattlegroundEngines()
        {
            comboboxBattlegroundEngine.Items.Add("None");

            for (int i = 0; i < AmeisenBot.BattlegroundEngines.Count(); ++i)
            {
                comboboxBattlegroundEngine.Items.Add(AmeisenBot.BattlegroundEngines.ElementAt(i).ToString());
            }

            comboboxBattlegroundEngine.SelectedIndex = 0;
        }

        /// <summary>
        /// Adds the available combat classes to the combobox.
        /// The "None" option is added first, followed by the
        /// built-in combat classes.
        /// </summary>
        private void AddCombatClasses()
        {
            comboboxBuiltInCombatClass.Items.Add("None");

            for (int i = 0; i < AmeisenBot.CombatClasses.Count(); ++i)
            {
                comboboxBuiltInCombatClass.Items.Add(AmeisenBot.CombatClasses.ElementAt(i).ToString());
            }

            comboboxBuiltInCombatClass.SelectedIndex = 0;
        }

        /// <summary>
        /// Event handler for the ButtonDone Click event.
        /// Validates the fields and saves the configuration settings.
        /// Closes the current window if validation is successful.
        /// </summary>
        private void ButtonDone_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                ConfigName = textboxConfigName.Text.Trim();

                Config.AntiAfkMs = int.Parse(textboxAntiAfk.Text, CultureInfo.InvariantCulture);
                Config.AoeDetectionAvoid = checkboxAvoidAoe.IsChecked.GetValueOrDefault(false);
                Config.AoeDetectionIncludePlayers = checkboxAvoidAoePlayers.IsChecked.GetValueOrDefault(false);
                Config.AutoAcceptQuests = checkboxAutoAcceptQuests.IsChecked.GetValueOrDefault(false);
                Config.AutoChangeRealmlist = checkboxAutoChangeRealmlist.IsChecked.GetValueOrDefault(false);
                Config.AutocloseWow = checkboxAutocloseWow.IsChecked.GetValueOrDefault(false);
                Config.AutoDisableRender = checkboxAutoDisableRendering.IsChecked.GetValueOrDefault(false);
                Config.AutojoinBg = checkboxAutoJoinBg.IsChecked.GetValueOrDefault(false);
                Config.AutojoinLfg = checkboxAutoJoinLfg.IsChecked.GetValueOrDefault(false);
                Config.AutoLogin = checkboxAutoLogin.IsChecked.GetValueOrDefault(false);
                Config.AutoPositionWow = checkboxAutoPositionWow.IsChecked.GetValueOrDefault(false);
                Config.AutoRepair = checkboxAutoRepair.IsChecked.GetValueOrDefault(false);
                Config.AutostartWow = checkboxAutoStartWow.IsChecked.GetValueOrDefault(false);
                Config.AutoTalkToNearQuestgivers = checkboxAutoTalkToQuestgivers.IsChecked.GetValueOrDefault(false);
                Config.BagSlotsToGoSell = (int)MathF.Round((float)sliderMinFreeBagSlots.Value);
                Config.BattlegroundEngine = comboboxBattlegroundEngine.SelectedItem != null ? comboboxBattlegroundEngine.SelectedItem.ToString() : string.Empty;
                Config.BattlegroundUsePartyMode = checkboxBattlegroundUsePartyMode.IsChecked.GetValueOrDefault(false);
                Config.BuiltInCombatClassName = comboboxBuiltInCombatClass.SelectedItem != null ? comboboxBuiltInCombatClass.SelectedItem.ToString() : string.Empty;
                Config.CharacterSlot = int.Parse(textboxCharacterSlot.Text, CultureInfo.InvariantCulture);
                Config.CustomCombatClassFile = textboxCombatClassFile.Text;
                Config.DrinkStartPercent = sliderDrinkStart.Value;
                Config.DrinkUntilPercent = sliderDrinkUntil.Value;
                Config.DungeonUsePartyMode = checkboxDungeonUsePartyMode.IsChecked.GetValueOrDefault(false);
                Config.EatDrinkAbortFollowParty = checkboxEatDrinkPartyFollowAbort.IsChecked.GetValueOrDefault(false);
                Config.EatDrinkAbortFollowPartyDistance = (float)sliderEatDrinkPartyFollowAbort.Value;
                Config.EatStartPercent = sliderEatStart.Value;
                Config.EatUntilPercent = sliderEatUntil.Value;
                Config.EventPullMs = int.Parse(textboxEventPull.Text, CultureInfo.InvariantCulture);
                Config.FollowGroupLeader = checkboxFollowGroupLeader.IsChecked.GetValueOrDefault(false);
                Config.FollowGroupMembers = checkboxGroupMembers.IsChecked.GetValueOrDefault(false);
                Config.FollowPositionDynamic = checkboxDynamicPosition.IsChecked.GetValueOrDefault(false);
                Config.FollowSpecificCharacter = checkboxFollowSpecificCharacter.IsChecked.GetValueOrDefault(false);
                Config.Friends = textboxFriends.Text;
                Config.IdleActions = checkboxIdleActions.IsChecked.GetValueOrDefault(false);
                Config.IgnoreCombatWhileMounted = checkboxIgnoreCombatMounted.IsChecked.GetValueOrDefault(false);
                Config.ItemRepairThreshold = sliderRepair.Value;
                Config.ItemSellBlacklist = new(textboxItemSellBlacklist.Text.Split(",", StringSplitOptions.RemoveEmptyEntries));
                Config.JobEngineMailHeader = textboxMailHeader.Text;
                Config.JobEngineMailReceiver = textboxMailReceiver.Text;
                Config.JobEngineMailText = textboxMailText.Text;
                Config.LootOnlyMoneyAndQuestitems = checkboxLootOnlyMoneyAndQuestitems.IsChecked.GetValueOrDefault(false);
                Config.LootUnits = checkboxLooting.IsChecked.GetValueOrDefault(false);
                Config.LootUnitsRadius = MathF.Round((float)sliderLootRadius.Value);
                Config.MaxFollowDistance = (int)MathF.Round((float)sliderMaxFollowDistance.Value);
                Config.MaxFps = (int)MathF.Round((float)sliderMaxFps.Value);
                Config.MaxFpsCombat = (int)MathF.Round((float)sliderMaxFpsCombat.Value);
                Config.MerchantNpcSearchRadius = (float)sliderMerchantSearchRadius.Value;
                Config.MinFollowDistance = (int)MathF.Round((float)sliderMinFollowDistance.Value);
                Config.Mounts = textboxMounts.Text;
                Config.NameshServerPort = int.Parse(textboxNavmeshServerPort.Text, CultureInfo.InvariantCulture);
                Config.NavmeshServerIp = textboxNavmeshServerIp.Text;
                Config.OnlyFriendsMode = checkboxOnlyFriendsMode.IsChecked.GetValueOrDefault(false);
                Config.OnlySupportMaster = checkboxOnlySupportMaster.IsChecked.GetValueOrDefault(false);
                Config.Password = textboxPassword.Password;
                Config.PathToWowExe = textboxWowPath.Text;
                Config.PermanentNameCache = checkboxPermanentNameCache.IsChecked.GetValueOrDefault(false);
                Config.PermanentReactionCache = checkboxPermanentReactionCache.IsChecked.GetValueOrDefault(false);
                Config.RconEnabled = checkboxEnableRcon.IsChecked.GetValueOrDefault(true);
                Config.RconInterval = int.Parse(textboxRconScreenshotInterval.Text, CultureInfo.InvariantCulture);
                Config.RconSendScreenshots = checkboxEnableRconScreenshots.IsChecked.GetValueOrDefault(false);
                Config.RconServerAddress = textboxRconAddress.Text;
                Config.RconServerGuid = textboxRconGUID.Text;
                Config.RconServerImage = textboxRconImage.Text;
                Config.RconTickMs = int.Parse(textboxRconInterval.Text, CultureInfo.InvariantCulture);
                Config.Realm = textboxRealm.Text;
                Config.Realmlist = textboxRealmlist.Text;
                Config.ReleaseSpirit = checkboxReleaseSpirit.IsChecked.GetValueOrDefault(false);
                Config.SaveBotWindowPosition = checkboxSaveBotWindowPosition.IsChecked.GetValueOrDefault(false);
                Config.SaveWowWindowPosition = checkboxSaveWowWindowPosition.IsChecked.GetValueOrDefault(false);
                Config.SellBlueItems = checkboxSellBlueItems.IsChecked.GetValueOrDefault(false);
                Config.SellGrayItems = checkboxSellGrayItems.IsChecked.GetValueOrDefault(false);
                Config.SellGreenItems = checkboxSellGreenItems.IsChecked.GetValueOrDefault(false);
                Config.SellPurpleItems = checkboxSellPurpleItems.IsChecked.GetValueOrDefault(false);
                Config.SellWhiteItems = checkboxSellWhiteItems.IsChecked.GetValueOrDefault(false);
                Config.SpecificCharacterToFollow = textboxFollowSpecificCharacterName.Text;
                Config.StateMachineTickMs = int.Parse(textboxStatemachineTick.Text, CultureInfo.InvariantCulture);
                Config.StayCloseToGroupInCombat = checkboxStayCloseToGroupInCombat.IsChecked.GetValueOrDefault(false);
                Config.SupportRange = (float)sliderAssistRange.Value;
                Config.UseBuiltInCombatClass = checkboxBuiltinCombatClass.IsChecked.GetValueOrDefault(true);
                Config.UseMounts = checkboxUseMounts.IsChecked.GetValueOrDefault(false);
                Config.UseMountsInParty = checkboxUseMountsInParty.IsChecked.GetValueOrDefault(false);
                Config.UseOnlySpecificMounts = checkboxOnlySpecificMounts.IsChecked.GetValueOrDefault(false);
                Config.Username = textboxUsername.Text;

                Config.MovementSettings.EnableDistanceMovedJumpCheck = checkboxDistanceMovedJumpCheck.IsChecked.GetValueOrDefault(false);
                Config.MovementSettings.MaxSteering = (float)sliderMaxSteeringNormal.Value / 10.0f;
                Config.MovementSettings.MaxSteeringCombat = (float)sliderMaxSteeringCombat.Value / 10.0f;
                Config.MovementSettings.MaxVelocity = (float)sliderMaxVelocity.Value / 10.0f;
                Config.MovementSettings.WaypointCheckThresholdMounted = sliderWaypointThresholdMount.Value;
                Config.MovementSettings.SeperationDistance = (float)sliderPlayerSeperationDistance.Value;
                Config.MovementSettings.WaypointCheckThreshold = sliderWaypointThreshold.Value;

                if (Enum.TryParse(comboboxStartStopBotBindingAltKey.Text.ToString(), out KeyCode mod)
                    && Enum.TryParse(comboboxStartStopBotBindingKey.Text.ToString(), out KeyCode key))
                {
                    if (!Config.Hotkeys.ContainsKey("StartStop"))
                    {
                        Config.Hotkeys.Add("StartStop", new() { Key = (int)key, Mod = (int)mod });
                    }
                    else
                    {
                        Config.Hotkeys["StartStop"] = new() { Key = (int)key, Mod = (int)mod };
                    }
                }

                Config.IdleActionsEnabled.Clear();

                foreach (IdleActionWrapper x in IdleActionItems)
                {
                    Config.IdleActionsEnabled.Add(x.Name, x.IsEnabled);
                }

                SaveConfig = true;
                Close();
            }
        }

        /// <summary>
        /// Handles the click event of the Exit button. If a change has been made, a confirmation window is shown,
        /// asking the user if they want to cancel. If the user clicks 'Yes', the Cancel flag is set to true and
        /// the current window is closed. If the user clicks 'No', the method returns without any further action.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            if (ChangedSomething)
            {
                ConfirmWindow confirmWindow = new("Unsaved Changes!", "Are you sure that you wan't to cancel?", "Yes", "No");
                confirmWindow.ShowDialog();

                if (!confirmWindow.OkayPressed)
                {
                    return;
                }
            }

            Cancel = true;
            Close();
        }

        /// <summary>
        /// Event handler for when the "Open Combat Class File" button is clicked.
        /// Displays an open file dialog and allows the user to select a C# file.
        /// If a file is selected, the filepath is displayed in the combat class file textbox
        /// and a flag to indicate that something has changed is set to true.
        /// </summary>
        private void ButtonOpenCombatClassFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "C# Files|*.cs"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault(false))
            {
                textboxCombatClassFile.Text = openFileDialog.FileName;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the ButtonOpenImage click event. Opens a file dialog to select an image file (PNG or JPEG), and if a file is chosen, sets the content of the textboxRconImage to the base64 encoded string of the selected image file. It also sets the ChangedSomething flag to true.
        /// </summary>
        private void ButtonOpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "PNG|*.png|JPEG|*.jpg;"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault(false))
            {
                textboxRconImage.Text = $"data:image/{System.IO.Path.GetExtension(openFileDialog.FileName).ToUpperInvariant()};base64,{Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))}";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the "Open WoW Exe" button is clicked. Opens a file dialog to allow the user to select the WoW executable file. If a file is selected, updates the text in the "WowPath" textbox with the selected file path and sets the "ChangedSomething" flag to true.
        /// </summary>
        private void ButtonOpenWowExe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "WoW Executeable|*.exe"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault(false))
            {
                textboxWowPath.Text = openFileDialog.FileName;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the CheckboxAutoStartWow is checked. 
        /// If the WindowLoaded flag is true, it enables the textboxWowPath and buttonOpenWowExe controls
        /// and sets the ChangedSomething flag to true.
        /// </summary>
        private void CheckboxAutoStartWow_Checked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                textboxWowPath.IsEnabled = true;
                buttonOpenWowExe.IsEnabled = true;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the CheckboxAutoStartWow is unchecked.
        /// It disables the textboxWowPath, sets its text to empty, disables the buttonOpenWowExe,
        /// and sets the ChangedSomething flag to true if the WindowLoaded flag is true.
        /// </summary>
        private void CheckboxAutoStartWow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                textboxWowPath.IsEnabled = false;
                textboxWowPath.Text = string.Empty;
                buttonOpenWowExe.IsEnabled = false;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the checkbox for the built-in combat class is checked.
        /// Hides the open combat class file button, the combat class textbox, and shows the built-in combat class combobox.
        /// Sets the ChangedSomething variable to true.
        /// </summary>
        private void CheckboxBuiltinCombatClass_Checked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                buttonOpenCombatClassFile.Visibility = Visibility.Hidden;
                textboxCombatClassFile.Visibility = Visibility.Hidden;
                comboboxBuiltInCombatClass.Visibility = Visibility.Visible;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the CheckboxCustomCombatClass is checked.
        /// </summary>
        private void CheckboxCustomCombatClass_Checked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                buttonOpenCombatClassFile.Visibility = Visibility.Visible;
                textboxCombatClassFile.Visibility = Visibility.Visible;
                comboboxBuiltInCombatClass.Visibility = Visibility.Hidden;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the checkbox "CheckboxFollowSpecificCharacter" is checked.
        /// If the window is loaded, enables the textbox "textboxFollowSpecificCharacterName" and sets the "ChangedSomething" flag to true.
        /// </summary>
        private void CheckboxFollowSpecificCharacter_Checked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                textboxFollowSpecificCharacterName.IsEnabled = true;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the CheckboxFollowSpecificCharacter is unchecked.
        /// If the window is loaded, the textboxFollowSpecificCharacterName is disabled,
        /// its text is cleared, and the ChangedSomething flag is set to true.
        /// </summary>
        private void CheckboxFollowSpecificCharacter_Unchecked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                textboxFollowSpecificCharacterName.IsEnabled = false;
                textboxFollowSpecificCharacterName.Text = string.Empty;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the CheckboxOnlyFriendsMode is checked.
        /// Enables the textboxFriends and sets the ChangedSomething flag to true.
        /// </summary>
        private void CheckboxOnlyFriendsMode_Checked(object sender, RoutedEventArgs e)
        {
            if (textboxFriends != null)
            {
                textboxFriends.IsEnabled = true;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event that is triggered when the CheckboxOnlyFriendsMode is unchecked. Disables the textboxFriends control and sets the ChangedSomething flag to true.
        /// </summary>
        private void CheckboxOnlyFriendsMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (textboxFriends != null)
            {
                textboxFriends.IsEnabled = false;
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Enables the textboxMounts when the CheckboxOnlySpecificMounts is checked, only if the window is loaded.
        /// </summary>
        private void CheckboxOnlySpecificMounts_Checked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                textboxMounts.IsEnabled = true;
            }
        }

        /// <summary>
        /// Event handler for when the CheckboxOnlySpecificMounts is unchecked.
        /// Disables the textboxMounts control if WindowLoaded flag is true.
        /// </summary>
        private void CheckboxOnlySpecificMounts_Unchecked(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                textboxMounts.IsEnabled = false;
            }
        }

        /// <summary>
        /// Event handler for the selection changed event of the ComboboxBattlegroundEngine control.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments containing information about the selection change.</param>
        private void ComboboxBattlegroundEngine_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                if (comboboxBattlegroundEngine.SelectedItem == null || comboboxBattlegroundEngine.SelectedItem.ToString() == "None")
                {
                    labelBattlegroundEngineDescription.Content = "...";
                    ChangedSomething = true;
                }
                else
                {
                    IBattlegroundEngine battlegroundEngine = AmeisenBot.BattlegroundEngines.FirstOrDefault(e => e.ToString().Equals(comboboxBattlegroundEngine.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase));

                    if (battlegroundEngine != null)
                    {
                        labelBattlegroundEngineDescription.Content = battlegroundEngine.Description;
                        ChangedSomething = true;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the GridViewColumnHeader is loaded.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void GridViewColumnHeader_Loaded(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader columnHeader = sender as GridViewColumnHeader;

            if (columnHeader.Template.FindName("HeaderBorder", columnHeader) is Border HeaderBorder)
            {
                HeaderBorder.Background = HeaderBorder.Background;
            }

            if (columnHeader.Template.FindName("HeaderHoverBorder", columnHeader) is Border HeaderHoverBorder)
            {
                HeaderHoverBorder.BorderBrush = HeaderHoverBorder.BorderBrush;
            }

            if (columnHeader.Template.FindName("UpperHighlight", columnHeader) is Rectangle UpperHighlight)
            {
                UpperHighlight.Visibility = UpperHighlight.Visibility;
            }

            if (columnHeader.Template.FindName("PART_HeaderGripper", columnHeader) is Thumb PART_HeaderGripper)
            {
                PART_HeaderGripper.Background = PART_HeaderGripper.Background;
            }
        }

        /// <summary>
        /// Loads the configuration values to the user interface.
        /// </summary>
        private void LoadConfigToUi()
        {
            checkboxAutoAcceptQuests.IsChecked = Config.AutoAcceptQuests;
            checkboxAutoChangeRealmlist.IsChecked = Config.AutoChangeRealmlist;
            checkboxAutocloseWow.IsChecked = Config.AutocloseWow;
            checkboxAutoDisableRendering.IsChecked = Config.AutoDisableRender;
            checkboxAutoJoinBg.IsChecked = Config.AutojoinBg;
            checkboxAutoJoinLfg.IsChecked = Config.AutojoinLfg;
            checkboxAutoLogin.IsChecked = Config.AutoLogin;
            checkboxAutoPositionWow.IsChecked = Config.AutoPositionWow;
            checkboxAutoRepair.IsChecked = Config.AutoRepair;
            checkboxAutoStartWow.IsChecked = Config.AutostartWow;
            checkboxAutoTalkToQuestgivers.IsChecked = Config.AutoTalkToNearQuestgivers;
            checkboxAvoidAoe.IsChecked = Config.AoeDetectionAvoid;
            checkboxAvoidAoePlayers.IsChecked = Config.AoeDetectionIncludePlayers;
            checkboxBattlegroundUsePartyMode.IsChecked = Config.BattlegroundUsePartyMode;
            checkboxBuiltinCombatClass.IsChecked = Config.UseBuiltInCombatClass;
            checkboxDungeonUsePartyMode.IsChecked = Config.DungeonUsePartyMode;
            checkboxDynamicPosition.IsChecked = Config.FollowPositionDynamic;
            checkboxEatDrinkPartyFollowAbort.IsChecked = Config.EatDrinkAbortFollowParty;
            checkboxEnableRcon.IsChecked = Config.RconEnabled;
            checkboxEnableRconScreenshots.IsChecked = Config.RconSendScreenshots;
            checkboxFollowGroupLeader.IsChecked = Config.FollowGroupLeader;
            checkboxFollowSpecificCharacter.IsChecked = Config.FollowSpecificCharacter;
            checkboxGroupMembers.IsChecked = Config.FollowGroupMembers;
            checkboxIdleActions.IsChecked = Config.IdleActions;
            checkboxIgnoreCombatMounted.IsChecked = Config.IgnoreCombatWhileMounted;
            checkboxLooting.IsChecked = Config.LootUnits;
            checkboxLootOnlyMoneyAndQuestitems.IsChecked = Config.LootOnlyMoneyAndQuestitems;
            checkboxOnlyFriendsMode.IsChecked = Config.OnlyFriendsMode;
            checkboxOnlySpecificMounts.IsChecked = Config.UseOnlySpecificMounts;
            checkboxOnlySupportMaster.IsChecked = Config.OnlySupportMaster;
            checkboxPermanentNameCache.IsChecked = Config.PermanentNameCache;
            checkboxPermanentReactionCache.IsChecked = Config.PermanentReactionCache;
            checkboxReleaseSpirit.IsChecked = Config.ReleaseSpirit;
            checkboxSaveBotWindowPosition.IsChecked = Config.SaveBotWindowPosition;
            checkboxSaveWowWindowPosition.IsChecked = Config.SaveWowWindowPosition;
            checkboxSellBlueItems.IsChecked = Config.SellBlueItems;
            checkboxSellGrayItems.IsChecked = Config.SellGrayItems;
            checkboxSellGreenItems.IsChecked = Config.SellGreenItems;
            checkboxSellPurpleItems.IsChecked = Config.SellPurpleItems;
            checkboxSellWhiteItems.IsChecked = Config.SellWhiteItems;
            checkboxStayCloseToGroupInCombat.IsChecked = Config.StayCloseToGroupInCombat;
            checkboxUseMounts.IsChecked = Config.UseMounts;
            checkboxUseMountsInParty.IsChecked = Config.UseMountsInParty;
            comboboxBattlegroundEngine.Text = Config.BattlegroundEngine;
            comboboxBuiltInCombatClass.Text = Config.BuiltInCombatClassName;
            sliderAssistRange.Value = Config.SupportRange;
            sliderDrinkStart.Value = Config.DrinkStartPercent;
            sliderDrinkUntil.Value = Config.DrinkUntilPercent;
            sliderEatDrinkPartyFollowAbort.Value = Config.EatDrinkAbortFollowPartyDistance;
            sliderEatStart.Value = Config.EatStartPercent;
            sliderEatUntil.Value = Config.EatUntilPercent;
            sliderLootRadius.Value = Math.Round(Config.LootUnitsRadius);
            sliderMaxFollowDistance.Value = Config.MaxFollowDistance;
            sliderMaxFps.Value = Config.MaxFps;
            sliderMaxFpsCombat.Value = Config.MaxFpsCombat;
            sliderMerchantSearchRadius.Value = Config.MerchantNpcSearchRadius;
            sliderMinFollowDistance.Value = Config.MinFollowDistance;
            sliderMinFreeBagSlots.Value = Config.BagSlotsToGoSell;
            sliderRepair.Value = Config.ItemRepairThreshold;
            textboxAntiAfk.Text = Config.AntiAfkMs.ToString(CultureInfo.InvariantCulture);
            textboxCharacterSlot.Text = Config.CharacterSlot.ToString(CultureInfo.InvariantCulture);
            textboxCombatClassFile.Text = Config.CustomCombatClassFile;
            textboxEventPull.Text = Config.EventPullMs.ToString(CultureInfo.InvariantCulture);
            textboxFollowSpecificCharacterName.Text = Config.SpecificCharacterToFollow;
            textboxFriends.Text = Config.Friends;
            textboxItemSellBlacklist.Text = string.Join(",", Config.ItemSellBlacklist ?? new List<string>());
            textboxMailHeader.Text = Config.JobEngineMailHeader;
            textboxMailReceiver.Text = Config.JobEngineMailReceiver;
            textboxMailText.Text = Config.JobEngineMailText;
            textboxMounts.Text = Config.Mounts;
            textboxNavmeshServerIp.Text = Config.NavmeshServerIp;
            textboxNavmeshServerPort.Text = Config.NameshServerPort.ToString(CultureInfo.InvariantCulture);
            textboxPassword.Password = Config.Password;
            textboxRconAddress.Text = Config.RconServerAddress;
            textboxRconGUID.Text = Config.RconServerGuid;
            textboxRconImage.Text = Config.RconServerImage;
            textboxRconInterval.Text = Config.RconTickMs.ToString(CultureInfo.InvariantCulture);
            textboxRconScreenshotInterval.Text = Config.RconInterval.ToString(CultureInfo.InvariantCulture);
            textboxRealm.Text = Config.Realm;
            textboxRealmlist.Text = Config.Realmlist;
            textboxStatemachineTick.Text = Config.StateMachineTickMs.ToString(CultureInfo.InvariantCulture);
            textboxUsername.Text = Config.Username;
            textboxWowPath.Text = Config.PathToWowExe;

            checkboxDistanceMovedJumpCheck.IsChecked = Config.MovementSettings.EnableDistanceMovedJumpCheck;
            sliderMaxSteeringCombat.Value = Config.MovementSettings.MaxSteeringCombat * 10.0f;
            sliderMaxSteeringNormal.Value = Config.MovementSettings.MaxSteering * 10.0f;
            sliderMaxVelocity.Value = Config.MovementSettings.MaxVelocity * 10.0f;
            sliderPlayerSeperationDistance.Value = Config.MovementSettings.SeperationDistance;
            sliderWaypointThreshold.Value = Config.MovementSettings.WaypointCheckThreshold;
            sliderWaypointThresholdMount.Value = Config.MovementSettings.WaypointCheckThresholdMounted;

            // Load keybinding settings
            if (Config.Hotkeys.TryGetValue("StartStop", out Keybind kv))
            {
                comboboxStartStopBotBindingAltKey.Text = ((KeyCode)kv.Mod).ToString();
                comboboxStartStopBotBindingKey.Text = ((KeyCode)kv.Key).ToString();
            }
            else
            {
                comboboxStartStopBotBindingAltKey.Text = KeyCode.None.ToString();
                comboboxStartStopBotBindingKey.Text = KeyCode.None.ToString();
            }

            // Idle Actions
            IdleActionItems = new();

            if (AmeisenBot?.Bot.IdleActions.IdleActions != null)
            {
                foreach (IIdleAction x in AmeisenBot.Bot.IdleActions.IdleActions)
                {
                    bool state = Config.IdleActionsEnabled.TryGetValue(x.ToString(), out bool b) && b;

                    IdleActionItems.Add(new()
                    {
                        Name = x.ToString(),
                        IsEnabled = state
                    });
                }
            }

            listviewIdleActions.ItemsSource = IdleActionItems;

            ChangedSomething = false;
        }

        /// <summary>
        /// Handles the event of validating input for numbers only in a text box.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The TextCompositionEventArgs that contains information about the input.</param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Event handler for when the value of the SliderAssistRange is changed.
        /// If the window is loaded, updates the content of labelGroupAssistRange with the new value rounded to the nearest whole number,
        /// and sets ChangedSomething to true.
        /// </summary>
        private void SliderAssistRange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelGroupAssistRange.Content = $"Assist Range (m): {Math.Round(e.NewValue)}";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the SliderDrinkStart value is changed.
        /// </summary>
        private void SliderDrinkStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelDrinkStart.Content = $"Drink Start: {Math.Round(e.NewValue)} %";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the value of the SliderDrinkUntil changes.
        /// Updates the content of the labelDrinkUntil with the new value rounded to the nearest whole number followed by a percentage sign.
        /// Sets the ChangedSomething flag to true.
        /// </summary>
        private void SliderDrinkUntil_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelDrinkUntil.Content = $"Drink Until: {Math.Round(e.NewValue)} %";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the ValueChanged event of the SliderEatDrinkPartyFollowAbort Slider. 
        /// Updates the labelEatDrinkPartyFollowAbort content with the new value in meters rounded to the nearest whole number. 
        /// Sets the ChangedSomething flag to true if the WindowLoaded flag is true.
        /// </summary>
        private void SliderEatDrinkPartyFollowAbort_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelEatDrinkPartyFollowAbort.Content = $"Abort Distance: {Math.Round(e.NewValue)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the value of the SliderEatStart is changed.
        /// </summary>
        private void SliderEatStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelEatStart.Content = $"Eat Start: {Math.Round(e.NewValue)} %";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the "SliderEatUntil" value changes.
        /// Changes the content of "labelEatUntil" to display the new value as a percentage and sets the "ChangedSomething" flag to true.
        /// </summary>
        private void SliderEatUntil_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelEatUntil.Content = $"Eat Until: {Math.Round(e.NewValue)} %";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Handles the event when the value of the SliderLootRadius changes.
        /// If the window has finished loading, updates the labelLootRadius content to display the new loot radius value rounded to the nearest whole number.
        /// Sets the ChangedSomething flag to true.
        /// </summary>
        private void SliderLootRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelLootRadius.Content = $"Loot Radius: {Math.Round(e.NewValue)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the SliderMaxFollowDistance value is changed.
        /// </summary>
        private void SliderMaxFollowDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMaxFollowDistance.Content = $"Max Follow Distance: {Math.Round(e.NewValue)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the ValueChanged event of the SliderMaxFps control.
        /// Sets the content of the labelMaxFps to the rounded value of the changed slider value,
        /// and sets the ChangedSomething flag to true.
        /// </summary>
        private void SliderMaxFps_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMaxFps.Content = $"Max FPS: {Math.Round(e.NewValue)}";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the SliderMaxFpsCombat value changes.
        /// Updates the labelMaxFpsCombat content with the new value and sets ChangedSomething to true.
        /// </summary>
        private void SliderMaxFpsCombat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMaxFpsCombat.Content = $"Max FPS Combat: {Math.Round(e.NewValue)}";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the SliderMaxSteeringCombat value changes.
        /// </summary>
        private void SliderMaxSteeringCombat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMaxSteeringCombat.Content = $"Max Steering: {MathF.Round((float)e.NewValue / 10.0f, 2)}m/s";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the SliderMaxSteeringNormal value is changed.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments containing the new value.</param>
        private void SliderMaxSteeringNormal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMaxSteeringNormal.Content = $"Max Steering: {MathF.Round((float)e.NewValue / 10.0f, 2)}m/s";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the SliderMaxVelocity_ValueChanged event.
        /// Updates the labelMaxVelocity.Content based on the new value of the Slider.
        /// Sets the ChangedSomething flag to true.
        /// </summary>
        private void SliderMaxVelocity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMaxVelocity.Content = $"Max Velocity: {MathF.Round((float)e.NewValue / 10.0f, 2)}m/s";
                ChangedSomething = true;
            }
        }

        ///<summary>
        ///Handles the event when the value of the SliderMerchantSearchRadius is changed.
        ///If the window is loaded, it sets the content of labelMerchantSearchRadius to the new search radius value, rounded to the nearest whole number.
        ///It also sets ChangedSomething to true.
        ///</summary>
        private void SliderMerchantSearchRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMerchantSearchRadius.Content = $"Search Radius (m): {Math.Round(e.NewValue)}";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the ValueChanged event of the SliderMinFollowDistance control.
        /// Updates the labelMinFollowDistance content with the new value and sets ChangedSomething to true if the WindowLoaded flag is true.
        /// </summary>
        private void SliderMinFollowDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMinFollowDistance.Content = $"Min Follow Distance: {Math.Round(e.NewValue)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the value of the SliderMinFreeBagSlots changes.
        /// Updates the content of the labelMinFreeBagSlots with the new value rounded to the nearest integer,
        /// and sets the ChangedSomething flag to true.
        /// </summary>
        private void SliderMinFreeBagSlots_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelMinFreeBagSlots.Content = $"Min Free Bag Slots: {(int)Math.Round(e.NewValue)}";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the SliderPlayerSeperationDistance value changes.
        /// </summary>
        private void SliderPlayerSeperationDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelPlayerSeperationDistance.Content = $"Player Seperation Distance: {MathF.Round((float)e.NewValue, 2)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the ValueChanged event of the SliderRepair object.
        /// Updates the content of labelRepairThreshold with the rounded value of e.NewValue and sets ChangedSomething to true.
        /// Only executes if WindowLoaded is true.
        /// </summary>
        private void SliderRepair_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelRepairThreshold.Content = $"Repair: {Math.Round(e.NewValue)}%";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for the ValueChanged event of the SliderWaypointThreshold.
        /// Updates the labelWaypointThreshold content with the rounded value of the new slider value (e.NewValue) and sets ChangedSomething to true.
        /// </summary>
        private void SliderWaypointThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelWaypointThreshold.Content = $"Waypoint Threshold: {MathF.Round((float)e.NewValue, 2)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Event handler for when the value of the SliderWaypointThresholdMount changes.
        /// Updates the label displaying the waypoint threshold for Mount and sets the ChangedSomething flag to true.
        /// </summary>
        private void SliderWaypointThresholdMount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WindowLoaded)
            {
                labelWaypointThresholdMount.Content = $"Waypoint Threshold (Mount): {MathF.Round((float)e.NewValue, 2)}m";
                ChangedSomething = true;
            }
        }

        /// <summary>
        /// Validates the auto login fields and sets the border brush for each field accordingly.
        /// </summary>
        /// <param name="failed">A boolean indicating whether the validation failed or not.</param>
        /// <returns>A boolean indicating whether the validation failed or not.</returns>
        private bool ValidateAutoLogin(bool failed)
        {
            if (checkboxAutoLogin.IsChecked.GetValueOrDefault(false))
            {
                if (textboxUsername.Text.Length == 0)
                {
                    textboxUsername.BorderBrush = ErrorBorderBrush;
                    failed = true;
                }
                else
                {
                    textboxUsername.BorderBrush = NormalBorderBrush;
                }

                if (textboxPassword.Password.Length == 0)
                {
                    textboxPassword.BorderBrush = ErrorBorderBrush;
                    failed = true;
                }
                else
                {
                    textboxPassword.BorderBrush = NormalBorderBrush;
                }

                if (textboxCharacterSlot.Text.Length == 0
                    || !int.TryParse(textboxCharacterSlot.Text, out int charslot)
                    || charslot < 0
                    || charslot > 9)
                {
                    textboxCharacterSlot.BorderBrush = ErrorBorderBrush;
                    failed = true;
                }
                else
                {
                    textboxCharacterSlot.BorderBrush = NormalBorderBrush;
                }
            }

            return failed;
        }

        /// <summary>
        /// Validates the autostart for Wow.
        /// </summary>
        /// <param name="failed">A boolean value indicating if the validation failed.</param>
        private bool ValidateAutostartWow(bool failed)
        {
            if (checkboxAutoStartWow.IsChecked.GetValueOrDefault(false))
            {
                if (textboxWowPath.Text.Length == 0)
                {
                    textboxConfigName.BorderBrush = ErrorBorderBrush;
                    failed = true;
                }
                else
                {
                    textboxConfigName.BorderBrush = NormalBorderBrush;
                }
            }

            return failed;
        }

        /// <summary>
        /// Validates the config name.
        /// </summary>
        /// <param name="failed">A boolean indicating the validation status.</param>
        /// <returns>A boolean indicating whether the validation was successful or not.</returns>
        private bool ValidateConfigName(bool failed)
        {
            Regex regex = new("^([a-zA-Z]:)?(\\\\[^<>:\"/\\\\|?*]+)+\\\\?$");

            if (textboxConfigName.Text.Length == 0
                || regex.IsMatch(textboxConfigName.Text))
            {
                textboxConfigName.BorderBrush = ErrorBorderBrush;
                failed = true;
            }
            else
            {
                textboxConfigName.BorderBrush = NormalBorderBrush;
            }

            return failed;
        }

        /// <summary>
        /// Validates all the fields.
        /// </summary>
        /// <returns>Returns true if all the fields are valid, otherwise false.</returns>
        private bool ValidateFields()
        {
            bool failed = false;
            failed = ValidateConfigName(failed);
            failed = ValidateAutoLogin(failed);
            failed = ValidateAutostartWow(failed);
            failed = ValidateSpecificFollow(failed);
            failed = ValidateNavmeshServer(failed);
            return !failed;
        }

        /// <summary>
        /// Validates the Navmesh Server IP address and port number.
        /// </summary>
        /// <param name="failed">Indicates whether validation has failed.</param>
        /// <returns>Returns true if the validation failed, false otherwise.</returns>
        private bool ValidateNavmeshServer(bool failed)
        {
            if (textboxNavmeshServerIp.Text.Length == 0
                || !IPAddress.TryParse(textboxNavmeshServerIp.Text, out IPAddress _))
            {
                textboxNavmeshServerIp.BorderBrush = ErrorBorderBrush;
                failed = true;
            }
            else
            {
                textboxNavmeshServerIp.BorderBrush = NormalBorderBrush;
            }

            if (textboxNavmeshServerPort.Text.Length == 0
                || !int.TryParse(textboxNavmeshServerPort.Text, out int port)
                || port < 0
                || port > ushort.MaxValue)
            {
                textboxNavmeshServerPort.BorderBrush = ErrorBorderBrush;
                failed = true;
            }
            else
            {
                textboxNavmeshServerPort.BorderBrush = NormalBorderBrush;
            }

            return failed;
        }

        /// <summary>
        /// Validates if the specific follow checkbox is checked and if the specific character name textbox is empty.
        /// Sets the border color of the textbox based on the validation.
        /// </summary>
        /// <param name="failed">Indicates if the validation failed.</param>
        /// <returns>True if the validation failed, otherwise false.</returns>
        private bool ValidateSpecificFollow(bool failed)
        {
            if (checkboxFollowSpecificCharacter.IsChecked.GetValueOrDefault(false))
            {
                if (textboxFollowSpecificCharacterName.Text.Length == 0)
                {
                    textboxFollowSpecificCharacterName.BorderBrush = ErrorBorderBrush;
                    failed = true;
                }
                else
                {
                    textboxFollowSpecificCharacterName.BorderBrush = NormalBorderBrush;
                }
            }

            return failed;
        }

        /// <summary>
        /// Event handler for when the Window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded = true;

            if (!NewConfig)
            {
                textboxConfigName.IsEnabled = false;
                textboxConfigName.Text = ConfigName;
                labelHeader.Content = $"AmeisenBotX - {ConfigName}";

                AddCombatClasses();
                AddBattlegroundEngines();
            }
            else
            {
                buttonOpenCombatClassFile.Visibility = Visibility.Hidden;
                comboboxBuiltInCombatClass.Visibility = Visibility.Hidden;
                checkboxCustomCombatClass.Visibility = Visibility.Hidden;
                checkboxBuiltinCombatClass.Visibility = Visibility.Hidden;

                tabitemCombat.Visibility = Visibility.Collapsed;
                tabitemBattleground.Visibility = Visibility.Collapsed;
            }

            textboxCombatClassFile.Visibility = Visibility.Hidden;

            // add hotkeys to comboboxes
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                switch (k)
                {
                    case KeyCode.None:
                        comboboxStartStopBotBindingKey.Items.Add(k.ToString());
                        comboboxStartStopBotBindingAltKey.Items.Add(k.ToString());
                        break;

                    case KeyCode.LControlKey:
                    case KeyCode.RControlKey:
                    case KeyCode.LShiftKey:
                    case KeyCode.RShiftKey:
                    case KeyCode.LWin:
                    case KeyCode.RWin:
                    case KeyCode.LMenu:
                    case KeyCode.RMenu:
                    case KeyCode.Alt:
                        comboboxStartStopBotBindingAltKey.Items.Add(k.ToString());
                        break;

                    default:
                        comboboxStartStopBotBindingKey.Items.Add(k.ToString());
                        break;
                }
            }

            LoadConfigToUi();
        }

        /// <summary>
        /// Handles the event of left mouse button being pressed down on the window. Initiates window dragging.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}