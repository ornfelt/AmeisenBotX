using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Cache.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Represents a window for developer tools.
/// </summary>
namespace AmeisenBotX
{
    /// <summary>
    /// Represents a window for developer tools.
    /// </summary>
    public partial class DevToolsWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevToolsWindow"/> class.
        /// </summary>
        /// <param name="ameisenBot">The <see cref="AmeisenBot"/> used for this instance of the <see cref="DevToolsWindow"/>.</param>
        public DevToolsWindow(AmeisenBot ameisenBot)
        {
            AmeisenBot = ameisenBot;
            InitializeComponent();
        }

        /// <summary>
        /// Represents the main tabs in the application.
        /// </summary>
        private enum MainTab
        {
            CachePoi = 0,
            CacheOre,
            CacheHerb,
            CacheNames,
            CacheReactions,
            CacheSpellNames,
            NearWowObjects,
            Lua,
            Events,
            Logs,
            ClientPatches
        }

        /// <summary>
        /// Represents the possible tabs for displaying near World of Warcraft objects.
        /// </summary>
        private enum NearWowObjectsTab
        {
            Unselected = -1,
            Items,
            Containers,
            Units,
            Players,
            GameObjects,
            DynamicObjects,
            Corpses,
            AiGroups,
            AreaTriggers
        }

        /// <summary>
        /// Gets the private instance of the AmeisenBot class.
        /// </summary>
        private AmeisenBot AmeisenBot { get; }

        /// <summary>
        /// Copies the data of the nearest object from the given ListView.
        /// </summary>
        /// <param name="listView">The ListView from which to copy the data.</param>
        private static void CopyDataOfNearestObject(ItemsControl listView)
        {
            ItemCollection listItems = listView.Items;
            if (listItems.Count == 0)
            {
                return;
            }

            object firstItem = listItems[0];
            if (firstItem == null)
            {
                return;
            }

            string dataString = firstItem.ToString();
            if (string.IsNullOrEmpty(dataString) || string.IsNullOrWhiteSpace(dataString))
            {
                return;
            }

            string[] splitByGuid = dataString.Split(" Guid:", 2);
            string entryId = splitByGuid[0].Replace("EntryId: ", string.Empty);

            string[] splitByPos = dataString.Split("Pos: [", 2);
            string[] splitByBrace = splitByPos[1].Split("]", 2);

            string[] posComponents = splitByBrace[0].Split(", ");
            string[] cleanComponents = { "", "", "" };

            for (int i = 0; i < posComponents.Length; i++)
            {
                cleanComponents[i] = posComponents[i].Split(".")[0];
            }

            string finalPosStr = "new Vector3(" + cleanComponents[0] + ", " + cleanComponents[1] + ", " + cleanComponents[2] + ")";
            Clipboard.SetDataObject(entryId + ", " + finalPosStr);
        }

        /// <summary>
        /// Clears the text in the textboxEventResult.
        /// </summary>
        private void ButtonEventClear_Click(object sender, RoutedEventArgs e)
        {
            textboxEventResult.Text = string.Empty;
        }

        /// <summary>
        /// Subscribes to a WoW event using the specified event name and callback method.
        /// </summary>
        private void ButtonEventSubscribe_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Bot.Wow.Events.Subscribe(textboxEventName.Text, OnWowEventFired);
        }

        /// <summary>
        /// Handles the click event of the ButtonEventUnsubscribe control and unsubscribes from the specified WoW event.
        /// </summary>
        private void ButtonEventUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Bot.Wow.Events.Unsubscribe(textboxEventName.Text, OnWowEventFired);
        }

        /// <summary>
        /// Event handler for the click event of the ButtonExit button.
        /// Unsubscribes the OnLog event handler from the AmeisenLogger.I instance and hides the current form.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            AmeisenLogger.I.OnLog -= OnLog;
            Hide();
        }

        /// <summary>
        /// Executes Lua code and updates the result in the TextBox. 
        /// If successful, the result is displayed in the TextBox; otherwise, "Failed to execute LUA..." is displayed.
        /// </summary>
        private void ButtonLuaExecute_Click(object sender, RoutedEventArgs e)
        {
            textboxLuaResult.Text = AmeisenBot.Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua(textboxLuaCode.Text), out string result)
                ? result : "Failed to execute LUA...";
        }

        /// <summary>
        /// Event handler for the click event of the "Copy" button. Executes the Lua code specified in the input text box using AmeisenBot.Bot.Wow.LuaDoString method.
        /// </summary>
        private void ButtonLuaExecute_Copy_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Bot.Wow.LuaDoString(textboxLuaCode.Text);
        }

        /// <summary>
        /// Event handler for the Refresh button click event. Calls the RefreshActiveData method to update the active data.
        /// </summary>
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshActiveData();
        }

        /// <summary>
        /// Method for climbing steep slopes. It reads the player's base memory address and performs multi-level pointer redirection to find the appropriate memory location to write the climb angle. The current implementation is messy and needs improvement.
        /// </summary>
        private void ClimbSteepSlopesChecked(object sender, RoutedEventArgs e)
        {
            // Todo: find a better way, multi-level pointer redirection very messy
            AmeisenBot.Bot.Memory.Read<IntPtr>(AmeisenBot.Bot.Memory.Offsets.PlayerBase, out IntPtr PlayerBase1);
            AmeisenBot.Bot.Memory.Read<IntPtr>(IntPtr.Add(PlayerBase1, 0x34), out IntPtr PlayerBase2);
            AmeisenBot.Bot.Memory.Read<IntPtr>(IntPtr.Add(PlayerBase2, 0x24), out IntPtr PlayerBase);
            AmeisenBot.Bot.Memory.Write<float>(IntPtr.Add(PlayerBase, (int)AmeisenBot.Bot.Memory.Offsets.ClimbAngle), 255);
        }

        /// <summary>
        /// Method for climbing steep slopes in the game.
        /// This method reads the player's base address from memory and adjusts the climb angle to enable climbing steep slopes.
        /// </summary>
        private void ClimbSteepSlopesUnchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Bot.Memory.Read<IntPtr>(AmeisenBot.Bot.Memory.Offsets.PlayerBase, out IntPtr PlayerBase1);
            AmeisenBot.Bot.Memory.Read<IntPtr>(IntPtr.Add(PlayerBase1, 0x34), out IntPtr PlayerBase2);
            AmeisenBot.Bot.Memory.Read<IntPtr>(IntPtr.Add(PlayerBase2, 0x24), out IntPtr PlayerBase);
            AmeisenBot.Bot.Memory.Write<float>(IntPtr.Add(PlayerBase, (int)AmeisenBot.Bot.Memory.Offsets.ClimbAngle), 1);
        }

        /// <summary>
        /// Disables collisions between M2 objects when checked.
        /// </summary>
        private void DisableM2CollisionsChecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disables M2 collisions without checking for collision type.
        /// </summary>
        private void DisableM2CollisionsUnchecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disables WMOCollisions when checked.
        /// </summary>
        private void DisableWMOCollisionsChecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disables the collisions for the Windows Mixed Reality object unchecked.
        /// </summary>
        private void DisableWMOCollisionsUnchecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the KeyDown event for the ListViewNearWowObjects. If the pressed key is not 'C', it returns. Otherwise, it performs a switch statement based on the selected tab in the tab control. Performs different actions based on the selected tab.
        /// </summary>
        private void ListViewNearWowObjects_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.C)
            {
                return;
            }

            switch ((NearWowObjectsTab)tabControlNearWowObjects.SelectedIndex)
            {
                case NearWowObjectsTab.Unselected:
                    break;

                case NearWowObjectsTab.Items:
                    CopyDataOfNearestObject(listViewItems);
                    break;

                case NearWowObjectsTab.Containers:
                    CopyDataOfNearestObject(listViewContainers);
                    break;

                case NearWowObjectsTab.Units:
                    CopyDataOfNearestObject(listViewUnits);
                    break;

                case NearWowObjectsTab.Players:
                    CopyDataOfNearestObject(listViewPlayers);
                    break;

                case NearWowObjectsTab.GameObjects:
                    CopyDataOfNearestObject(listViewGameObjects);
                    break;

                case NearWowObjectsTab.DynamicObjects:
                    CopyDataOfNearestObject(listViewDynamicObjects);
                    break;

                case NearWowObjectsTab.Corpses:
                    CopyDataOfNearestObject(listViewCorpses);
                    break;

                case NearWowObjectsTab.AiGroups:
                    break;

                case NearWowObjectsTab.AreaTriggers:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handles logging based on the specified log level and log message.
        /// </summary>
        private void OnLog(LogLevel loglevel, string log)
        {
            Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    if (loglevel > (LogLevel)comboboxLoglevels.SelectedItem)
                    {
                        return;
                    }

                    textboxLogs.AppendText($"{log}\n");
                    textboxLogs.ScrollToEnd();
                }
                catch { }
            });
        }

        /// <summary>
        /// Invoked when the Wow event is fired.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event.</param>
        /// <param name="args">The arguments of the event.</param>
        private void OnWowEventFired(long timestamp, List<string> args)
        {
            Dispatcher.Invoke(() =>
            {
                textboxEventResult.AppendText($"{timestamp} - {JsonSerializer.Serialize(args)}\n");
            });
        }

        /// <summary>
        /// Refreshes the active data based on the selected main tab in the UI.
        /// Clears the items in the corresponding list view based on the selected main tab, and adds new items based on the data from the AmeisenBot database.
        /// </summary>
        private void RefreshActiveData()
        {
            switch ((MainTab)tabcontrolMain.SelectedIndex)
            {
                case MainTab.CachePoi:
                    {
                        listviewCachePoi.Items.Clear();

                        foreach ((WowMapId mapId, Dictionary<PoiType, List<Vector3>> dictionary) in AmeisenBot.Bot.Db.AllPointsOfInterest()
                            .OrderBy(e => e.Key))
                        {
                            foreach ((PoiType poiType, List<Vector3> list) in dictionary.OrderBy(e => e.Key))
                            {
                                listviewCachePoi.Items.Add($"{mapId} {poiType}: {JsonSerializer.Serialize(list)}");
                            }
                        }
                        break;
                    }
                case MainTab.CacheOre:
                    {
                        listviewCacheOre.Items.Clear();

                        foreach ((WowMapId mapId, Dictionary<WowOreId, List<Vector3>> dictionary) in AmeisenBot.Bot.Db.AllOreNodes()
                            .OrderBy(e => e.Key))
                        {
                            foreach ((WowOreId oreId, List<Vector3> list) in dictionary.OrderBy(e => e.Key))
                            {
                                listviewCacheOre.Items.Add($"{mapId} {oreId}: {JsonSerializer.Serialize(list)}");
                            }
                        }
                        break;
                    }
                case MainTab.CacheHerb:
                    {
                        listviewCacheHerb.Items.Clear();

                        foreach ((WowMapId mapId, Dictionary<WowHerbId, List<Vector3>> dictionary) in AmeisenBot.Bot.Db.AllHerbNodes()
                            .OrderBy(e => e.Key))
                        {
                            foreach ((WowHerbId herbId, List<Vector3> list) in dictionary.OrderBy(e => e.Key))
                            {
                                listviewCacheHerb.Items.Add($"{mapId} {herbId}: {JsonSerializer.Serialize(list)}");
                            }
                        }
                        break;
                    }
                case MainTab.CacheNames:
                    {
                        listviewCacheNames.Items.Clear();

                        foreach (KeyValuePair<ulong, string> x in AmeisenBot.Bot.Db.AllNames()
                            .OrderBy(e => e.Value))
                        {
                            listviewCacheNames.Items.Add(x);
                        }
                        break;
                    }
                case MainTab.CacheReactions:
                    {
                        listviewCacheReactions.Items.Clear();

                        // todo: resolve ... name = mapIdPair implies enum WowMapId not int
                        foreach (KeyValuePair<int, Dictionary<int, WowUnitReaction>> mapIdPair in AmeisenBot.Bot.Db.AllReactions()
                            .OrderBy(e => e.Key))
                        {
                            // todo: same ... typePair as enum? or intPair as int?
                            foreach (KeyValuePair<int, WowUnitReaction> typePair in mapIdPair.Value
                                .OrderBy(e => e.Key))
                            {
                                listviewCacheReactions.Items.Add($"{mapIdPair.Key} {typePair.Key}: {typePair.Value}");
                            }
                        }
                        break;
                    }
                case MainTab.CacheSpellNames:
                    {
                        listviewCacheSpellnames.Items.Clear();

                        foreach (KeyValuePair<int, string> keyValuePair in AmeisenBot.Bot.Db.AllSpellNames()
                            .OrderBy(kvp => kvp.Value))
                        {
                            listviewCacheSpellnames.Items.Add(keyValuePair);
                        }
                        break;
                    }
                case MainTab.NearWowObjects:
                    switch ((NearWowObjectsTab)tabControlNearWowObjects.SelectedIndex)
                    {
                        case NearWowObjectsTab.Unselected:
                            break;

                        // case NearWowObjectsTab.Items: { listViewItems.Items.Clear();
                        //
                        // List<(IWowObject, double)> wowObjects = AmeisenBot.Bot.Objects.WowObjects
                        // .TakeWhile(wowObject => wowObject != null) .Select(wowObject =>
                        // (wowObject,
                        // Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position),
                        // 2))) .ToList();
                        //
                        // foreach ((IWowObject wowObject, double distanceTo) in wowObjects .Where(e
                        // => e.Item1.Type == WowObjectType.Item) .OrderBy(e => e.Item2)) {
                        // listViewItems.Items.Add($"EntryId: {wowObject.EntryId} Guid:
                        // {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale}
                        // Distance: {distanceTo}"); } break; } case NearWowObjectsTab.Containers: { listViewContainers.Items.Clear();
                        //
                        // List<(IWowObject, double)> wowObjects = AmeisenBot.Bot.Objects.WowObjects
                        // .TakeWhile(wowObject => wowObject != null) .Select(wowObject =>
                        // (wowObject,
                        // Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position),
                        // 2))) .ToList();
                        //
                        // foreach ((IWowObject wowObject, double distanceTo) in wowObjects .Where(e
                        // => e.Item1.Type == WowObjectType.Container) .OrderBy(e => e.Item2)) {
                        // listViewContainers.Items.Add($"EntryId: {wowObject.EntryId} Guid:
                        // {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale}
                        // Distance: {distanceTo}"); } break; }
                        case NearWowObjectsTab.Units:
                            {
                                listViewUnits.Items.Clear();

                                List<(IWowUnit, double)> wowObjects = AmeisenBot.Bot.Objects.All
                                    .OfType<IWowUnit>()
                                    .TakeWhile(wowObject => wowObject != null)
                                    .Select(wowObject => (wowObject, Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position), 2)))
                                    .ToList();

                                foreach ((IWowUnit wowObject, double distanceTo) in wowObjects
                                    .OrderBy(e => e.Item2))
                                {
                                    listViewUnits.Items.Add($"EntryId: {wowObject.EntryId} Guid: {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale} Distance: {distanceTo}");
                                }
                                break;
                            }
                        case NearWowObjectsTab.Players:
                            {
                                listViewPlayers.Items.Clear();

                                List<(IWowPlayer, double)> wowObjects = AmeisenBot.Bot.Objects.All
                                    .OfType<IWowPlayer>()
                                    .TakeWhile(wowObject => wowObject != null)
                                    .Select(wowObject => (wowObject, Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position), 2)))
                                    .ToList();

                                foreach ((IWowPlayer wowObject, double distanceTo) in wowObjects
                                    .OrderBy(e => e.Item2))
                                {
                                    listViewPlayers.Items.Add($"EntryId: {wowObject.EntryId} Guid: {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale} Distance: {distanceTo}");
                                }
                                break;
                            }
                        case NearWowObjectsTab.GameObjects:
                            {
                                listViewGameObjects.Items.Clear();

                                List<(IWowGameobject, double)> wowObjects = AmeisenBot.Bot.Objects.All
                                    .OfType<IWowGameobject>()
                                    .TakeWhile(wowObject => wowObject != null)
                                    .Select(wowObject => (wowObject, Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position), 2)))
                                    .ToList();

                                foreach ((IWowGameobject wowObject, double distanceTo) in wowObjects
                                    .OrderBy(e => e.Item2))
                                {
                                    listViewGameObjects.Items.Add($"EntryId: {wowObject.EntryId} Guid: {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale} Distance: {distanceTo}");
                                }
                                break;
                            }
                        case NearWowObjectsTab.DynamicObjects:
                            {
                                listViewDynamicObjects.Items.Clear();

                                List<(IWowDynobject, double)> wowObjects = AmeisenBot.Bot.Objects.All
                                    .OfType<IWowDynobject>()
                                    .TakeWhile(wowObject => wowObject != null)
                                    .Select(wowObject => (wowObject, Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position), 2)))
                                    .ToList();

                                foreach ((IWowDynobject wowObject, double distanceTo) in wowObjects
                                    .OrderBy(e => e.Item2))
                                {
                                    listViewDynamicObjects.Items.Add($"EntryId: {wowObject.EntryId} Guid: {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale} Distance: {distanceTo}");
                                }
                                break;
                            }
                        // case NearWowObjectsTab.Corpses: { listViewCorpses.Items.Clear();
                        //
                        // List<(IWowObject, double)> wowObjects = AmeisenBot.Bot.Objects.WowObjects
                        // .TakeWhile(wowObject => wowObject != null) .Select(wowObject =>
                        // (wowObject,
                        // Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position),
                        // 2))) .ToList();
                        //
                        // foreach ((IWowObject wowObject, double distanceTo) in wowObjects .Where(e
                        // => e.Item1.Type == WowObjectType.Corpse) .OrderBy(e => e.Item2)) {
                        // listViewCorpses.Items.Add($"EntryId: {wowObject.EntryId} Guid:
                        // {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale}
                        // Distance: {distanceTo}"); } break; } case NearWowObjectsTab.AiGroups: { listViewAiGroups.Items.Clear();
                        //
                        // List<(IWowObject, double)> wowObjects = AmeisenBot.Bot.Objects.WowObjects
                        // .TakeWhile(wowObject => wowObject != null) .Select(wowObject =>
                        // (wowObject,
                        // Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position),
                        // 2))) .ToList();
                        //
                        // foreach ((IWowObject wowObject, double distanceTo) in wowObjects .Where(e
                        // => e.Item1.Type == WowObjectType.AiGroup) .OrderBy(e => e.Item2)) {
                        // listViewAiGroups.Items.Add($"EntryId: {wowObject.EntryId} Guid:
                        // {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale}
                        // Distance: {distanceTo}"); } break; } case NearWowObjectsTab.AreaTriggers:
                        // { listViewAreaTriggers.Items.Clear();
                        //
                        // List<(IWowObject, double)> wowObjects = AmeisenBot.Bot.Objects.WowObjects
                        // .TakeWhile(wowObject => wowObject != null) .Select(wowObject =>
                        // (wowObject,
                        // Math.Round(wowObject.Position.GetDistance(AmeisenBot.Bot.Player.Position),
                        // 2))) .ToList();
                        //
                        // foreach ((IWowObject wowObject, double distanceTo) in wowObjects .Where(e
                        // => e.Item1.Type == WowObjectType.AiGroup) .OrderBy(e => e.Item2)) {
                        // listViewAreaTriggers.Items.Add($"EntryId: {wowObject.EntryId} Guid:
                        // {wowObject.Guid} Pos: [{wowObject.Position}] Scale: {wowObject.Scale}
                        // Distance: {distanceTo}"); } break; }

                        default:
                            break;
                    }
                    break;

                case MainTab.Lua: break;
                case MainTab.Events: break;
                case MainTab.Logs: break;
                case MainTab.ClientPatches: break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Event handler for when the selection in the TabControlMain changes.
        /// Refreshes the active data.
        /// </summary>
        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshActiveData();
        }

        ///<summary>
        ///Event handler for when the window is loaded.
        ///</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (LogLevel l in Enum.GetValues(typeof(LogLevel)))
            {
                comboboxLoglevels.Items.Add(l);
            }

            comboboxLoglevels.SelectedItem = AmeisenLogger.I.ActiveLogLevel;
            AmeisenLogger.I.OnLog += OnLog;
        }

        /// <summary>
        /// Event handler for the mouse left button down event on the window.
        /// This method allows the window to be dragged when the left mouse button is pressed.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}