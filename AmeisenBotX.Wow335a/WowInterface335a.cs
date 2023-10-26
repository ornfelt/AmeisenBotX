using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Events;
using AmeisenBotX.Wow.Hook.Modules;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Constants;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Flags;
using AmeisenBotX.Wow.Shared.Lua;
using AmeisenBotX.Wow335a.Hook;
using AmeisenBotX.Wow335a.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Wow335a
{
    /// <summary>
    /// WowInterface for the game version 3.3.5a 12340.
    /// </summary>
    public class WowInterface335a : IWowInterface
    {
        /// <summary>
        /// Initializes a new instance of the WowInterface335a class.
        /// </summary>
        /// <param name="memory">The WowMemoryApi object for accessing game memory.</param>
        public WowInterface335a(WowMemoryApi memory)
        {
            Memory = memory;
            HookModules = new();

            // lua variable names for the event hook
            string handlerName = BotUtils.FastRandomStringOnlyLetters();
            string tableName = BotUtils.FastRandomStringOnlyLetters();
            string eventHookOutput = BotUtils.FastRandomStringOnlyLetters();

            // name of the frame used to capture wows events
            string eventHookFrameName = BotUtils.FastRandomStringOnlyLetters();
            EventManager = new(LuaDoString, eventHookFrameName);

            // module to process wows events.
            HookModules.Add(new RunLuaHookModule
            (
                (x) =>
                {
                    if (x != IntPtr.Zero
                        && memory.ReadString(x, Encoding.UTF8, out string s)
                        && !string.IsNullOrWhiteSpace(s))
                    {
                        EventManager.OnEventPush(s);
                    }
                },
                null,
                memory,
                LuaEventHook.Get(eventHookFrameName, tableName, handlerName, eventHookOutput),
                eventHookOutput
            ));

            string staticPopupsVarName = BotUtils.FastRandomStringOnlyLetters();
            string oldPoupString = string.Empty;

            // module that monitors the STATIC_POPUP windows.
            HookModules.Add(new RunLuaHookModule
            (
                (x) =>
                {
                    if (x != IntPtr.Zero
                        && memory.ReadString(x, Encoding.UTF8, out string s)
                        && !string.IsNullOrWhiteSpace(s))
                    {
                        if (!oldPoupString.Equals(s, StringComparison.Ordinal))
                        {
                            OnStaticPopup?.Invoke(s);
                            oldPoupString = s;
                        }
                    }
                    else
                    {
                        oldPoupString = string.Empty;
                    }
                },
                null,
                memory,
                $"{staticPopupsVarName}=\"\"for b=1,STATICPOPUP_NUMDIALOGS do local c=_G[\"StaticPopup\"..b]if c:IsShown()then {staticPopupsVarName}={staticPopupsVarName}..b..\":\"..c.which..\"; \"end end",
                staticPopupsVarName
            ));

            string battlegroundStatusVarName = BotUtils.FastRandomStringOnlyLetters();
            string oldBattlegroundStatus = string.Empty;

            // module to monitor the battleground (and queue) status.
            HookModules.Add(new RunLuaHookModule
            (
                (x) =>
                {
                    if (x != IntPtr.Zero
                        && memory.ReadString(x, Encoding.UTF8, out string s)
                        && !string.IsNullOrWhiteSpace(s))
                    {
                        if (!oldBattlegroundStatus.Equals(s, StringComparison.Ordinal))
                        {
                            OnBattlegroundStatus?.Invoke(s);
                            oldBattlegroundStatus = s;
                        }
                    }
                    else
                    {
                        oldBattlegroundStatus = string.Empty;
                    }
                },
                null,
                memory,
                $"{battlegroundStatusVarName}=\"\"for b=1,MAX_BATTLEFIELD_QUEUES do local c,d,e,f,g,h=GetBattlefieldStatus(b)local i=GetBattlefieldTimeWaited(b)/1000;{battlegroundStatusVarName}={battlegroundStatusVarName}..b..\":\"..tostring(c or\"unknown\")..\":\"..tostring(d or\"unknown\")..\":\"..tostring(e or\"unknown\")..\":\"..tostring(f or\"unknown\")..\":\"..tostring(g or\"unknown\")..\":\"..tostring(h or\"unknown\")..\":\"..tostring(i or\"unknown\")..\";\"end",
                battlegroundStatusVarName
            ));

            // module to detect small obstacles that we can jump over
            HookModules.Add(new TracelineJumpHookModule
            (
                null,
                (x) =>
                {
                    IntPtr dataPtr = x.GetDataPointer();

                    if (dataPtr != IntPtr.Zero && Player != null)
                    {
                        Vector3 playerPosition = Player.Position;
                        playerPosition.Z += 1.3f;

                        Vector3 pos = BotUtils.MoveAhead(playerPosition, Player.Rotation, 0.25f);
                        memory.Write(dataPtr, (1.0f, playerPosition, pos));
                    }
                },
                memory
            ));

            ObjectManager = new(memory);

            Hook = new(memory);
            Hook.OnGameInfoPush += ObjectManager.HookManagerOnGameInfoPush;
        }

        /// <summary>
        /// Event that is triggered when the battleground status changes.
        /// </summary>
        public event Action<string> OnBattlegroundStatus;

        /// <summary>
        /// Event that is triggered when a static popup is invoked.
        /// The event passes a string parameter to indicate the popup information.
        /// </summary>
        public event Action<string> OnStaticPopup;

        /// <summary>
        /// Gets or sets the event manager for this object.
        /// </summary>
        public IEventManager Events => EventManager;

        /// <summary>
        /// Gets the number of times the hook has been called.
        /// </summary>
        public int HookCallCount => Hook.HookCallCount;

        /// <summary>
        /// Gets a value indicating whether the Hook is successfully hooked into the game.
        /// </summary>
        public bool IsReady => Hook.IsWoWHooked;

        /// <summary>
        /// Gets or sets the instance of the WowMemoryApi class.
        /// </summary>
        public WowMemoryApi Memory { get; }

        /// <summary>
        /// Gets or sets the object provider for the ObjectManager.
        /// </summary>
        public IObjectProvider ObjectProvider => ObjectManager;

        /// <summary>
        /// Gets the current instance of the World of Warcraft player from the ObjectManager.
        /// </summary>
        public IWowPlayer Player => ObjectManager.Player;

        /// <summary>
        /// Gets the version of World of Warcraft as WoWVersion.WotLK335a.
        /// </summary>
        public WowVersion WowVersion { get; } = WowVersion.WotLK335a;

        /// <summary>
        /// Gets the instance of the SimpleEventManager class used for managing events.
        /// </summary>
        private SimpleEventManager EventManager { get; }

        /// <summary>
        /// Gets or sets the private field representing the EndSceneHook335a hook.
        /// </summary>
        private EndSceneHook335a Hook { get; }

        /// <summary>
        /// Gets the list of hook modules.
        /// </summary>
        /// <returns>A list of hook modules.</returns>
        private List<IHookModule> HookModules { get; }

        /// <summary>
        /// Gets or sets the private ObjectManager335a property.
        /// </summary>
        private ObjectManager335a ObjectManager { get; }

        /// <summary>
        /// Abandons the quests that are not in the provided list of quests.
        /// </summary>
        /// <param name="quests">The list of quests to keep.</param>
        public void AbandonQuestsNotIn(IEnumerable<string> quests)
        {
            Hook.LuaAbandonQuestsNotIn(quests);
        }

        /// <summary>
        /// Accepts a battleground invitation by clicking the specified UI element.
        /// </summary>
        public void AcceptBattlegroundInvite()
        {
            ClickUiElement("StaticPopup1Button1");
        }

        /// <summary>
        /// Accepts a party invitation by executing the Lua code "AcceptGroup();StaticPopup_Hide("PARTY_INVITE")".
        /// </summary>
        public void AcceptPartyInvite()
        {
            LuaDoString("AcceptGroup();StaticPopup_Hide(\"PARTY_INVITE\")");
        }

        /// <summary>
        /// Accepts a quest by executing the Lua command "AcceptQuest()".
        /// </summary>
        public void AcceptQuest()
        {
            LuaDoString($"AcceptQuest()");
        }

        /// <summary>
        /// Accepts available quests and progresses through Gossip options.
        /// </summary>
        public void AcceptQuests()
        {
            LuaDoString("active=GetNumGossipActiveQuests()if active>0 then for a=1,active do if not not select(a*5-5+4,GetGossipActiveQuests())then SelectGossipActiveQuest(a)end end end;available=GetNumGossipAvailableQuests()if available>0 then for a=1,available do if not not not select(a*6-6+3,GetGossipAvailableQuests())then SelectGossipAvailableQuest(a)end end end;if available==0 and active==0 and GetNumGossipOptions()==1 then _,type=GetGossipOptions()if type=='gossip'then SelectGossipOption(1)return end end");
        }

        ///<summary>
        ///Accepts the resurrect popup and calls the Lua function "AcceptResurrect()".
        ///</summary>
        public void AcceptResurrect()
        {
            LuaDoString("AcceptResurrect();");
        }

        /// <summary>
        /// Accepts a summon request and hides the confirm summon popup.
        /// </summary>
        public void AcceptSummon()
        {
            LuaDoString("ConfirmSummon();StaticPopup_Hide(\"CONFIRM_SUMMON\")");
        }

        /// <summary>
        /// Executes the LuaDoString method to buy a trainer service with the specified service index.
        /// </summary>
        /// <param name="serviceIndex">The index of the service to be purchased.</param>
        public void BuyTrainerService(int serviceIndex)
        {
            LuaDoString($"BuyTrainerService({serviceIndex})");
        }

        /// <summary>
        /// Calls a companion of the specified type and with the given index.
        /// </summary>
        public void CallCompanion(int index, string type)
        {
            LuaDoString($"CallCompanion(\"{type}\", {index})");
        }

        /// <summary>
        /// Casts a spell by its name.
        /// </summary>
        /// <param name="name">The name of the spell to cast.</param>
        /// <param name="castOnSelf">Optional. Determines whether the spell should be cast on self. Default is false.</param>
        public void CastSpell(string name, bool castOnSelf = false)
        {
            LuaDoString($"CastSpellByName(\"{name}\"{(castOnSelf ? ", \"player\"" : string.Empty)})");
        }

        /// <summary>
        /// Casts a spell by its ID using LuaDoString.
        /// </summary>
        public void CastSpellById(int spellId)
        {
            LuaDoString($"CastSpellByID({spellId})");
        }

        /// <summary>
        /// Changes the target with the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the target to be changed.</param>
        public void ChangeTarget(ulong guid)
        {
            Hook.TargetGuid(guid);
        }

        ///<summary>
        ///Clears the current target by calling the ChangeTarget method with a value of 0.
        ///</summary>
        public void ClearTarget()
        {
            ChangeTarget(0);
        }

        /// <summary>
        /// Calls the ClickOnTerrain method in the Hook class, passing in the specified position.
        /// </summary>
        public void ClickOnTerrain(Vector3 position)
        {
            Hook.ClickOnTerrain(position);
        }

        /// <summary>
        /// Clicks on the "Train" button in the Blizzard Trainer UI.
        /// </summary>
        public void ClickOnTrainButton()
        {
            LuaDoString("LoadAddOn\"Blizzard_TrainerUI\"f=ClassTrainerTrainButton;f.e=0;if f:GetScript\"OnUpdate\"then f:SetScript(\"OnUpdate\",nil)else f:SetScript(\"OnUpdate\",function(f,a)f.e=f.e+a;if f.e>.01 then f.e=0;f:Click()end end)end");
        }

        /// <summary>
        /// Moves the character to the specified position using click-to-move.
        /// </summary>
        /// <param name="pos">The position to move to.</param>
        /// <param name="guid">The unique ID of the character.</param>
        /// <param name="clickToMoveType">The type of click-to-move action. Default is WowClickToMoveType.Move.</param>
        /// <param name="turnSpeed">The turn speed during the movement. Default is 20.9f.</param>
        /// <param name="distance">The distance to maintain from the target while moving. Default is WowClickToMoveDistance.Move.</param>
        /// <remarks>
        /// This method checks if the specified position is valid and within the allowed range before executing the click-to-move action.
        /// If the position is invalid, the method will return without performing any action.
        /// The turn speed and distance values are applied to the click-to-move action.
        /// If a valid character GUID is provided, it will be used to perform the click-to-move action.
        /// The X position of the specified position is written to memory to initiate the click-to-move action.
        /// </remarks>
        public void ClickToMove(Vector3 pos, ulong guid, WowClickToMoveType clickToMoveType = WowClickToMoveType.Move, float turnSpeed = 20.9f, float distance = WowClickToMoveDistance.Move)
        {
            if (float.IsInfinity(pos.X) || float.IsNaN(pos.X) || MathF.Abs(pos.X) > 17066.6656
                || float.IsInfinity(pos.Y) || float.IsNaN(pos.Y) || MathF.Abs(pos.Y) > 17066.6656
                || float.IsInfinity(pos.Z) || float.IsNaN(pos.Z) || MathF.Abs(pos.Z) > 17066.6656)
            {
                return;
            }

            Memory.Write(Memory.Offsets.ClickToMoveTurnSpeed, turnSpeed);
            Memory.Write(Memory.Offsets.ClickToMoveDistance, distance);

            if (guid > 0)
            {
                Memory.Write(Memory.Offsets.ClickToMoveGuid, guid);
            }

            Memory.Write(Memory.Offsets.ClickToMoveAction, clickToMoveType);
            Memory.Write(Memory.Offsets.ClickToMoveX, pos);
        }

        /// <summary>
        /// Clicks on a specified UI element.
        /// </summary>
        /// <param name="elementName">The name of the UI element.</param>
        public void ClickUiElement(string elementName)
        {
            LuaDoString($"{elementName}:Click()");
        }

        /// <summary>
        /// Confirms the loot roll by invoking the CofirmStaticPopup method.
        /// </summary>
        public void CofirmLootRoll()
        {
            CofirmStaticPopup();
        }

        /// <summary>
        /// Executes a Lua script to confirm the readiness status of a player.
        /// </summary>
        /// <param name="isReady">A boolean indicating if the player is ready or not.</param>
        public void CofirmReadyCheck(bool isReady)
        {
            LuaDoString($"ConfirmReadyCheck({isReady})");
        }

        /// <summary>
        /// This method confirms a static popup by calling LuaDoString with the given commands to equip a pending item, confirm bind on use, and hide specific static popups.
        /// </summary>
        public void CofirmStaticPopup()
        {
            LuaDoString($"EquipPendingItem(0);ConfirmBindOnUse();StaticPopup_Hide(\"AUTOEQUIP_BIND\");StaticPopup_Hide(\"EQUIP_BIND\");StaticPopup_Hide(\"USE_BIND\")");
        }

        /// <summary>
        /// Completes the current quest.
        /// </summary>
        public void CompleteQuest()
        {
            LuaDoString($"CompleteQuest()");
        }

        /// <summary>
        /// Deletes an item by its name from the player's inventory.
        /// </summary>
        public void DeleteItemByName(string itemName)
        {
            LuaDoString($"for b=0,4 do for s=1,GetContainerNumSlots(b) do local l=GetContainerItemLink(b,s); if l and string.find(l, \"{itemName}\") then PickupContainerItem(b,s); DeleteCursorItem(); end; end; end");
        }

        /// <summary>
        /// Dismisses a companion of the specified type.
        /// </summary>
        /// <param name="type">The type of companion to dismiss.</param>
        public void DismissCompanion(string type)
        {
            LuaDoString($"DismissCompanion(\"{type}\")");
        }

        /// <summary>
        /// Performs the necessary cleanup operations and unhooking before the object is disposed.
        /// </summary>
        public void Dispose()
        {
            Hook.Unhook();
        }

        /// <summary>
        /// Equips an item by name and optional item slot.
        /// If the item slot is not specified, the item will be equipped in the first available slot.
        /// </summary>
        /// <param name="newItem">The name of the item to be equipped.</param>
        /// <param name="itemSlot">The optional slot for equipping the item. Default value is -1.</param>
        public void EquipItem(string newItem, int itemSlot = -1)
        {
            if (itemSlot == -1)
            {
                LuaDoString($"EquipItemByName(\"{newItem}\")");
            }
            else
            {
                LuaDoString($"EquipItemByName(\"{newItem}\", {itemSlot})");
            }

            CofirmStaticPopup();
        }

        /// <summary>
        /// Executes the specified Lua script and reads the output.
        /// </summary>
        /// <param name="p">A tuple containing the Lua script and its arguments.</param>
        /// <param name="result">The result obtained after executing the Lua script.</param>
        /// <returns>A boolean value indicating if the execution was successful.</returns>
        public bool ExecuteLuaAndRead((string, string) p, out string result)
        {
            return Hook.ExecuteLuaAndRead(p, out result);
        }

        /// <summary>
        /// Moves the face position to the specified position.
        /// </summary>
        /// <param name="playerBase">The player's base object.</param>
        /// <param name="playerPosition">The player's current position.</param>
        /// <param name="position">The new position to move the face to.</param>
        /// <param name="smooth">Specifies whether the movement should be smooth or not. Default is false.</param>
        public void FacePosition(IntPtr playerBase, Vector3 playerPosition, Vector3 position, bool smooth = false)
        {
            Hook.FacePosition(playerBase, playerPosition, position, smooth);
        }

        /// <summary>
        /// Retrieves a collection of completed quests.
        /// </summary>
        /// <returns>An IEnumerable of integers representing the completed quests.</returns>
        public IEnumerable<int> GetCompletedQuests()
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=''for a,b in pairs(GetQuestsCompleted())do if b then {{v:0}}={{v:0}}..a..';'end end;"), out string result))
            {
                if (result != null && result.Length > 0)
                {
                    return result.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => int.TryParse(e, out int n) ? n : (int?)null)
                        .Where(e => e.HasValue)
                        .Select(e => e.Value);
                }
            }

            return Array.Empty<int>();
        }

        /// <summary>
        /// Executes Lua code to retrieve information about the player's equipment items and returns it in a formatted string.
        /// </summary>
        /// <returns>The information about the player's equipment items in a formatted string.</returns>
        public string GetEquipmentItems()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"[\"for a=0,23 do {v:1}=GetInventoryItemID(\"player\",a)if string.len(tostring({v:1} or\"\"))>0 then {v:2}=GetInventoryItemLink(\"player\",a){v:3}=GetInventoryItemCount(\"player\",a){v:4},{v:5}=GetInventoryItemDurability(a){v:6},{v:7}=GetInventoryItemCooldown(\"player\",a){v:8},{v:9},{v:10},{v:11},{v:12},{v:13},{v:14},{v:15},{v:16},{v:17},{v:18}=GetItemInfo({v:2}){v:19}=GetItemStats({v:2}){v:20}={}for b,c in pairs({v:19})do table.insert({v:20},string.format(\"\\\"%s\\\":\\\"%s\\\"\",b,c))end;{v:0}={v:0}..'{'..'\"id\": \"'..tostring({v:1} or 0)..'\",'..'\"count\": \"'..tostring({v:3} or 0)..'\",'..'\"quality\": \"'..tostring({v:10} or 0)..'\",'..'\"curDurability\": \"'..tostring({v:4} or 0)..'\",'..'\"maxDurability\": \"'..tostring({v:5} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({v:6} or 0)..'\",'..'\"cooldownEnd\": '..tostring({v:7} or 0)..','..'\"name\": \"'..tostring({v:8} or 0)..'\",'..'\"link\": \"'..tostring({v:9} or 0)..'\",'..'\"level\": \"'..tostring({v:11} or 0)..'\",'..'\"minLevel\": \"'..tostring({v:12} or 0)..'\",'..'\"type\": \"'..tostring({v:13} or 0)..'\",'..'\"subtype\": \"'..tostring({v:14} or 0)..'\",'..'\"maxStack\": \"'..tostring({v:15} or 0)..'\",'..'\"equipslot\": \"'..tostring(a or 0)..'\",'..'\"equiplocation\": \"'..tostring({v:16} or 0)..'\",'..'\"stats\": '..\"{\"..table.concat({v:20},\",\")..\"}\"..','..'\"sellprice\": \"'..tostring({v:18} or 0)..'\"'..'}'if a<23 then {v:0}={v:0}..\",\"end end end;{v:0}={v:0}..\"]\""), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Returns the number of free bag slots available.
        /// </summary>
        public int GetFreeBagSlotCount()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=0 for i=1,5 do {v:0}={v:0}+GetContainerNumFreeSlots(i-1)end"), out string sresult)
                && int.TryParse(sresult, out int freeBagSlots)
                 ? freeBagSlots : 0;
        }

        /// <summary>
        /// Retrieves an array of gossip types by executing Lua code and reading the result.
        /// If an exception occurs during the execution, an empty array is returned.
        /// </summary>
        public string[] GetGossipTypes()
        {
            try
            {
                ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"\"function {v:1}(...)for a=1,select(\"#\",...),2 do {v:0}={v:0}..select(a+1,...)..\";\"end end;{v:1}(GetGossipOptions())"), out string result);
                return result.Split(';', StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                // ignored
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Retrieves the inventory items and their information.
        /// </summary>
        public string GetInventoryItems()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"[\"for a=0,4 do {v:1}=GetContainerNumSlots(a)for b=1,{v:1} do {v:2}=GetContainerItemID(a,b)if string.len(tostring({v:2} or\"\"))>0 then {v:3}=GetContainerItemLink(a,b){v:4},{v:5}=GetContainerItemDurability(a,b){v:6},{v:7}=GetContainerItemCooldown(a,b){v:8},{v:9},{v:10},{v:11},{v:12},{v:13},{v:3},{v:14}=GetContainerItemInfo(a,b){v:15},{v:16},{v:17},{v:18},{v:19},{v:20},{v:21},{v:22},{v:23},{v:8},{v:24}=GetItemInfo({v:3}){v:25}=GetItemStats({v:3}){v:26}={}if {v:25} then for c,d in pairs({v:25})do table.insert({v:26},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;end;{v:0}={v:0}..\"{\"..'\"id\": \"'..tostring({v:2} or 0)..'\",'..'\"count\": \"'..tostring({v:9} or 0)..'\",'..'\"quality\": \"'..tostring({v:17} or 0)..'\",'..'\"curDurability\": \"'..tostring({v:4} or 0)..'\",'..'\"maxDurability\": \"'..tostring({v:5} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({v:6} or 0)..'\",'..'\"cooldownEnd\": \"'..tostring({v:7} or 0)..'\",'..'\"name\": \"'..tostring({v:15} or 0)..'\",'..'\"lootable\": \"'..tostring({v:13} or 0)..'\",'..'\"readable\": \"'..tostring({v:12} or 0)..'\",'..'\"link\": \"'..tostring({v:3} or 0)..'\",'..'\"level\": \"'..tostring({v:18} or 0)..'\",'..'\"minLevel\": \"'..tostring({v:19} or 0)..'\",'..'\"type\": \"'..tostring({v:20} or 0)..'\",'..'\"subtype\": \"'..tostring({v:21} or 0)..'\",'..'\"maxStack\": \"'..tostring({v:22} or 0)..'\",'..'\"equiplocation\": \"'..tostring({v:23} or 0)..'\",'..'\"sellprice\": \"'..tostring({v:24} or 0)..'\",'..'\"stats\": '..\"{\"..table.concat({v:26},\",\")..\"}\"..','..'\"bagid\": \"'..tostring(a or 0)..'\",'..'\"bagslot\": \"'..tostring(b or 0)..'\"'..\"}\"{v:0}={v:0}..\",\"end end end;{v:0}={v:0}..\"]\""), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Executes Lua code to retrieve information about an item based on its name or link.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The requested item information in JSON format.</returns>
        public string GetItemByNameOrLink(string itemName)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}=\"{itemName}\";{{v:0}}='noItem';{{v:2}},{{v:3}},{{v:4}},{{v:5}},{{v:6}},{{v:7}},{{v:8}},{{v:9}},{{v:10}},{{v:11}},{{v:12}}=GetItemInfo({{v:1}});{{v:13}}=GetItemStats({{v:3}}){{v:14}}={{}}for c,d in pairs({{v:13}})do table.insert({{v:14}},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;{{v:0}}='{{'..'\"id\": \"0\",'..'\"count\": \"1\",'..'\"quality\": \"'..tostring({{v:4}} or 0)..'\",'..'\"curDurability\": \"0\",'..'\"maxDurability\": \"0\",'..'\"cooldownStart\": \"0\",'..'\"cooldownEnd\": \"0\",'..'\"name\": \"'..tostring({{v:2}} or 0)..'\",'..'\"link\": \"'..tostring({{v:3}} or 0)..'\",'..'\"level\": \"'..tostring({{v:5}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:6}} or 0)..'\",'..'\"type\": \"'..tostring({{v:7}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:8}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:9}} or 0)..'\",'..'\"equiplocation\": \"'..tostring({{v:10}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:12}} or 0)..'\",'..'\"stats\": '..\"{{\"..table.concat({{v:14}},\",\")..\"}}\"..'}}';"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves the item link for a specific loot roll using the provided roll ID.
        /// </summary>
        /// <param name="rollId">The ID of the loot roll to retrieve the item link for.</param>
        /// <returns>The item link for the specified loot roll, or an empty string if the execution fails.</returns>
        public string GetLootRollItemLink(int rollId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetLootRollItemLink({rollId});"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves the current amount of money from the Lua script and returns it as an integer.
        /// If the Lua script execution is successful and the value can be parsed as an integer, it returns the value.
        /// Otherwise, it returns 0.
        /// </summary>
        public int GetMoney()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=GetMoney();"), out string s) ? int.TryParse(s, out int v) ? v : 0 : 0;
        }

        ///<summary>
        /// Retrieves a collection of WowMount objects representing the mounts obtained by the player.
        ///</summary>
        public IEnumerable<WowMount> GetMounts()
        {
            string mountJson = ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=\"[\"{{v:1}}=GetNumCompanions(\"MOUNT\")if {{v:1}}>0 then for b=1,{{v:1}} do {{v:4}},{{v:2}},{{v:3}}=GetCompanionInfo(\"mount\",b){{v:0}}={{v:0}}..\"{{\\\"name\\\":\\\"\"..{{v:2}}..\"\\\",\"..\"\\\"index\\\":\"..b..\",\"..\"\\\"spellId\\\":\"..{{v:3}}..\",\"..\"\\\"mountId\\\":\"..{{v:4}}..\",\"..\"}}\"if b<{{v:1}} then {{v:0}}={{v:0}}..\",\"end end end;{{v:0}}={{v:0}}..\"]\""), out string result) ? result : string.Empty;

            try
            {
                return JsonSerializer.Deserialize<List<WowMount>>(mountJson, new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString });
            }
            catch (Exception e)
            {
                AmeisenLogger.I.Log("CharacterManager", $"Failed to parse Mounts JSON:\n{mountJson}\n{e}", LogLevel.Error);
            }

            return Array.Empty<WowMount>();
        }

        /// <summary>
        /// Retrieves the number of choices in the quest log and assigns it to the out parameter 'numChoices'.
        /// Returns true if the number was successfully retrieved and assigned; otherwise, false.
        /// </summary>
        public bool GetNumQuestLogChoices(out int numChoices)
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetNumQuestLogChoices();"), out string result)
                && int.TryParse(result, out int num))
            {
                numChoices = num;
                return true;
            }

            numChoices = 0;
            return false;
        }

        ///<summary>
        /// Retrieves the item link for a choice in the quest log at the specified index.
        /// Returns true if the item link is successfully retrieved, false otherwise.
        ///</summary>
        ///<param name="index">The index of the choice in the quest log</param>
        ///<param name="itemLink">The retrieved item link</param>
        ///<returns>True if the item link is successfully retrieved, false otherwise</returns>
        public bool GetQuestLogChoiceItemLink(int index, out string itemLink)
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetQuestLogItemLink(\"choice\", {index});"),
                out string result)
                && result != "nil")
            {
                itemLink = result;
                return true;
            }

            itemLink = string.Empty;
            return false;
        }

        /// <summary>
        /// Retrieves the quest log ID by title.
        /// </summary>
        /// <param name="title">The title of the quest.</param>
        /// <param name="questLogId">The variable to store the quest log ID.</param>
        /// <returns>True if the quest log ID is successfully found, otherwise false.</returns>
        public bool GetQuestLogIdByTitle(string title, out int questLogId)
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"for i=1,GetNumQuestLogEntries() do if GetQuestLogTitle(i) == \"{title}\" then {{v:0}}=i;break;end;end;"), out string r1)
                && int.TryParse(r1, out int foundQuestLogId))
            {
                questLogId = foundQuestLogId;
                return true;
            }

            questLogId = 0;
            return false;
        }

        /// <summary>
        /// Retrieves the reaction between two units.
        /// </summary>
        public WowUnitReaction GetReaction(IntPtr a, IntPtr b)
        {
            return (WowUnitReaction)Hook.GetUnitReaction(a, b);
        }

        /// <summary>
        /// Retrieves the dictionary containing the number of ready runes for each type.
        /// </summary>
        /// <returns>A dictionary where the key represents the rune type and the value represents the number of ready runes for that type.</returns>
        public Dictionary<int, int> GetRunesReady()
        {
            Dictionary<int, int> runes = new()
            {
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 0 }
            };

            for (int i = 0; i < 6; ++i)
            {
                if (Memory.Read(Memory.Offsets.RuneType + (4 * i), out int type)
                    && Memory.Read(Memory.Offsets.Runes, out byte runeStatus)
                    && ((1 << i) & runeStatus) != 0)
                {
                    ++runes[type];
                }
            }

            return runes;
        }

        /// <summary>
        /// Retrieves the skills and their corresponding current skill level and maximum skill level.
        /// </summary>
        /// <returns>A dictionary containing the skills as keys and a tuple of the current skill level and maximum skill level as values.</returns>
        public Dictionary<string, (int, int)> GetSkills()
        {
            Dictionary<string, (int, int)> parsedSkills = new();

            try
            {
                ExecuteLuaAndRead(
                    BotUtils.ObfuscateLua(
                        "{v:0}=\"\"{v:1}=GetNumSkillLines()for a=1,{v:1} do local b,c,_,d,_,_,e=GetSkillLineInfo(a)if not c then {v:0}={v:0}..b;if a<{v:1} then {v:0}={v:0}..\":\"..tostring(d or 0)..\"/\"..tostring(e or 0)..\";\"end end end"),
                    out string result);

                if (!string.IsNullOrEmpty(result))
                {
                    IEnumerable<string> skills = new List<string>(result.Split(';')).Select(s => s.Trim());

                    foreach (string x in skills)
                    {
                        string[] splittedParts = x.Split(":");
                        string[] maxSkill = splittedParts[1].Split("/");

                        if (int.TryParse(maxSkill[0], out int currentSkillLevel)
                            && int.TryParse(maxSkill[1], out int maxSkillLevel))
                        {
                            parsedSkills.Add(splittedParts[0], (currentSkillLevel, maxSkillLevel));
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return parsedSkills;
        }

        /// <summary>
        /// Retrieves the cooldown time for a specified spell.
        /// </summary>
        /// <param name="spellName">The name of the spell to retrieve the cooldown for.</param>
        /// <returns>The cooldown time in milliseconds.</returns>
        public int GetSpellCooldown(string spellName)
        {
            int cooldown = 0;

            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}},{{v:2}},{{v:3}}=GetSpellCooldown(\"{spellName}\");{{v:0}}=({{v:1}}+{{v:2}}-GetTime())*1000;if {{v:0}}<0 then {{v:0}}=0 end;"), out string result))
            {
                if (result.Contains('.', StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Split('.')[0];
                }

                if (double.TryParse(result, out double value))
                {
                    cooldown = (int)Math.Round(value);
                }
            }

            return cooldown;
        }

        /// <summary>
        /// Retrieves the name of a spell based on its ID.
        /// </summary>
        /// <param name="spellId">The ID of the spell.</param>
        /// <returns>The name of the spell associated with the provided ID, or an empty string if the spell is not found.</returns>
        public string GetSpellNameById(int spellId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetSpellInfo({spellId});"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves a list of spells from the Lua environment and returns it as an XML string.
        /// </summary>
        public string GetSpells()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}='['{v:1}=GetNumSpellTabs()for a=1,{v:1} do {v:2},{v:3},{v:4},{v:5}=GetSpellTabInfo(a)for b={v:4}+1,{v:4}+{v:5} do {v:6},{v:7}=GetSpellName(b,\"BOOKTYPE_SPELL\")if {v:6} then {v:8},{v:9},_,{v:10},_,_,{v:11},{v:12},{v:13}=GetSpellInfo({v:6},{v:7}){v:0}={v:0}..'{'..'\"spellbookName\": \"'..tostring({v:2} or 0)..'\",'..'\"spellbookId\": \"'..tostring(a or 0)..'\",'..'\"name\": \"'..tostring({v:6} or 0)..'\",'..'\"rank\": \"'..tostring({v:9} or 0)..'\",'..'\"castTime\": \"'..tostring({v:11} or 0)..'\",'..'\"minRange\": \"'..tostring({v:12} or 0)..'\",'..'\"maxRange\": \"'..tostring({v:13} or 0)..'\",'..'\"costs\": \"'..tostring({v:10} or 0)..'\"'..'}'if a<{v:1} or b<{v:4}+{v:5} then {v:0}={v:0}..','end end end end;{v:0}={v:0}..']'"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Executes Lua script and reads the result to obtain the talents.
        /// </summary>
        /// <returns>A string containing the talents in the format: talentName;talentTab;talentIndex;talentRank;talentMaxRank|...</returns>
        public string GetTalents()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"\"{v:4}=GetNumTalentTabs();for g=1,{v:4} do {v:1}=GetNumTalents(g)for h=1,{v:1} do a,b,c,d,{v:2},{v:3},e,f=GetTalentInfo(g,h){v:0}={v:0}..a..\";\"..g..\";\"..h..\";\"..{v:2}..\";\"..{v:3};if h<{v:1} then {v:0}={v:0}..\"|\"end end;if g<{v:4} then {v:0}={v:0}..\"|\"end end"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Returns the money cost, talent cost, and profession cost for the specified service index.
        /// </summary>
        public void GetTrainerServiceCost(int serviceIndex)
        {
            // todo: returns moneyCost, talentCost, professionCost
            LuaDoString($"GetTrainerServiceCost({serviceIndex})");
        }

        /// <summary>
        /// Retrieves information about a trainer service.
        /// </summary>
        public void GetTrainerServiceInfo(int serviceIndex)
        {
            // todo: returns name, rank, category, expanded
            LuaDoString($"GetTrainerServiceInfo({serviceIndex})");
        }

        /// <summary>
        /// Returns the count of trainer services.
        /// </summary>
        public int GetTrainerServicesCount()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetNumTrainerServices()"));
        }

        /// <summary>
        /// Check if the string is casting or channeling a spell
        /// </summary>
        /// <param name="luaunit">player, target, party1...</param>
        /// <returns>(Spellname, duration)</returns>
        public (string, int) GetUnitCastingInfo(WowLuaUnit luaunit)
        {
            string str = ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=\"none,0\";{{v:1}},x,x,x,x,{{v:2}}=UnitCastingInfo(\"{luaunit}\");{{v:3}}=(({{v:2}}/1000)-GetTime())*1000;{{v:0}}={{v:1}}..\",\"..{{v:3}};"), out string result) ? result : string.Empty;

            if (double.TryParse(str.Split(',')[1], out double timeRemaining))
            {
                return (str.Split(',')[0], (int)Math.Round(timeRemaining, 0));
            }

            return (string.Empty, 0);
        }

        /// <summary>
        /// Retrieves the number of unspent talent points for the player.
        /// </summary>
        public int GetUnspentTalentPoints()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetUnspentTalentPoints()"));
        }

        /// <summary>
        /// Interacts with the specified WoW object by right-clicking on it.
        /// </summary>
        /// <param name="obj">The WoW object to interact with.</param>
        public void InteractWithObject(IWowObject obj)
        {
            Hook.ObjectRightClick(obj.BaseAddress);
        }

        /// <summary>
        /// Interacts with a World of Warcraft unit.
        /// </summary>
        /// <param name="unit">The unit to interact with.</param>
        public void InteractWithUnit(IWowUnit unit)
        {
            Hook.InteractWithUnit(unit.BaseAddress);
        }

        /// <summary>
        /// Checks if auto loot is enabled.
        /// </summary>
        public bool IsAutoLootEnabled()
        {
            return int.TryParse(LuaGetCVar("autoLootDefault"), out int result) && result == 1;
        }

        /// <summary>
        /// Returns whether the click-to-move action is active or not.
        /// </summary>
        public bool IsClickToMoveActive()
        {
            return Memory.Read(Memory.Offsets.ClickToMoveAction, out int ctmState)
                && ctmState != 0    // None
                && ctmState != 3    // Stop
                && ctmState != 13;  // Halted
        }

        /// <summary>
        /// Checks if the current user is in a Looking for Group (LFG) group.
        /// </summary>
        public bool IsInLfgGroup()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:1},{v:0}=GetLFGInfoServer()"), out string result)
                && bool.TryParse(result, out bool isInLfg)
                && isInLfg;
        }

        /// <summary>
        /// Determines if there is line of sight between two points in a 3D space, with an optional height adjustment applied.
        /// </summary>
        /// <param name="start">The starting point of the line of sight check.</param>
        /// <param name="end">The ending point of the line of sight check.</param>
        /// <param name="heightAdjust">An optional height adjustment to be added to the Z coordinates of the start and end points.</param>
        /// <returns>True if there is line of sight between the start and end points, false otherwise.</returns>
        public bool IsInLineOfSight(Vector3 start, Vector3 end, float heightAdjust = 1.5f)
        {
            start.Z += heightAdjust;
            end.Z += heightAdjust;
            return Hook.TraceLine(start, end, (uint)WowWorldFrameHitFlag.HitTestLOS);
        }

        /// <summary>
        /// Checks if the specified rune is ready based on its ID.
        /// </summary>
        /// <param name="runeId">The ID of the rune to check.</param>
        /// <returns>True if the rune is ready, otherwise false.</returns>
        public bool IsRuneReady(int runeId)
        {
            return Memory.Read(Memory.Offsets.Runes, out byte runeStatus) && ((1 << runeId) & runeStatus) != 0;
        }

        /// <summary>
        /// Leaves the current battleground by clicking on the specified UI element.
        /// </summary>
        public void LeaveBattleground()
        {
            ClickUiElement("WorldStateScoreFrameLeaveButton");
        }

        /// <summary>
        /// Function to loot all items from the current loot window.
        /// </summary>
        public void LootEverything()
        {
            LuaDoString(BotUtils.ObfuscateLua("{v:0}=GetNumLootItems()for a={v:0},1,-1 do LootSlot(a)ConfirmLootSlot(a)end").Item1);
        }

        /// <summary>
        /// This method loots money and quest items from the loot window.
        /// </summary>
        public void LootMoneyAndQuestItems()
        {
            LuaDoString("for a=GetNumLootItems(),1,-1 do slotType=GetLootSlotType(a)_,_,_,_,b,c=GetLootSlotInfo(a)if not locked and(c or b==LOOT_SLOT_MONEY or b==LOOT_SLOT_CURRENCY)then LootSlot(a)end end");
        }

        /// <summary>
        /// Completes a quest, selects a gossip option and gets the quest reward associated with the given IDs.
        /// </summary>
        /// <param name="questlogId">The ID of the quest in the quest log.</param>
        /// <param name="rewardId">The ID of the reward to be obtained.</param>
        /// <param name="gossipId">The ID of the gossip option to be selected.</param>
        public void LuaCompleteQuestAndGetReward(int questlogId, int rewardId, int gossipId)
        {
            LuaDoString($"SelectGossipActiveQuest({gossipId});CompleteQuest({questlogId});GetQuestReward({rewardId})");
        }

        /// <summary>
        /// Declines a party invite in Lua by hiding the "PARTY_INVITE" static popup.
        /// </summary>
        public void LuaDeclinePartyInvite()
        {
            LuaDoString("StaticPopup_Hide(\"PARTY_INVITE\")");
        }

        /// <summary>
        /// Calls the Lua function "DeclineResurrect()" to decline a resurrection request.
        /// </summary>
        public void LuaDeclineResurrect()
        {
            LuaDoString("DeclineResurrect()");
        }

        /// <summary>
        /// Executes a Lua script contained in a string.
        /// </summary>
        /// <param name="v">The Lua script to be executed.</param>
        /// <returns>True if the Lua script was executed successfully, otherwise false.</returns>
        public bool LuaDoString(string v)
        {
            return Hook.LuaDoString(v);
        }

        /// <summary>
        /// Retrieves the value of a Lua console variable using the specified CVar name.
        /// </summary>
        /// <param name="CVar">The name of the CVar.</param>
        /// <returns>The value of the specified CVar as a string, or an empty string if the execution fails.</returns>
        public string LuaGetCVar(string CVar)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetCVar(\"{CVar}\");"), out string s) ? s : string.Empty;
        }

        /// <summary>
        /// Retrieves the title of an active quest by its gossip ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip.</param>
        /// <param name="title">The title of the active quest if found, otherwise empty string.</param>
        /// <returns>True if the active quest title is found, false otherwise.</returns>
        public bool LuaGetGossipActiveQuestTitleById(int gossipId, out string title)
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"local g1,_,_,_,g2,_,_,_,g3,_,_,_,g4,_,_,_,g5,_,_,_,g6 = GetGossipActiveQuests(); local gps={{g1,g2,g3,g4,g5,g6}}; {{v:0}}=gps[{gossipId}]"), out string r1))
            {
                if (r1 == "nil")
                {
                    title = string.Empty;
                    return false;
                }

                title = r1;
                return true;
            }

            title = string.Empty;
            return false;
        }

        /// <summary>
        /// Retrieves the Gossip ID of an active quest title in Lua.
        /// </summary>
        /// <param name="title">The title of the active quest.</param>
        /// <param name="gossipId">The output parameter to store the found Gossip ID. Returns 0 if not found.</param>
        /// <returns>Returns true if the Gossip ID is found, otherwise false.</returns>
        public bool LuaGetGossipIdByActiveQuestTitle(string title, out int gossipId)
        {
            gossipId = 0;

            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"local g1,_,_,_,g2,_,_,_,g3,_,_,_,g4,_,_,_,g5,_,_,_,g6 = GetGossipActiveQuests(); local gps={{g1,g2,g3,g4,g5,g6}}; for k,v in pairs(gps) do if v == \"{title}\" then {{v:0}}=k; break end; end;"), out string r1)
                && int.TryParse(r1, out int foundGossipId))
            {
                gossipId = foundGossipId;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves the Gossip ID for a given available quest title.
        /// </summary>
        /// <param name="title">The title of the quest.</param>
        /// <param name="gossipId">The variable to store the Gossip ID if found.</param>
        /// <returns>True if the Gossip ID was found, false otherwise.</returns>
        public bool LuaGetGossipIdByAvailableQuestTitle(string title, out int gossipId)
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"local g1,_,_,_,_,g2,_,_,_,_,g3,_,_,_,_,g4,_,_,_,_,g5,_,_,_,_,g6 = GetGossipAvailableQuests(); local gps={{g1,g2,g3,g4,g5,g6}}; for k,v in pairs(gps) do if v == \"{title}\" then {{v:0}}=k; break end; end;"), out string r1)
                && int.TryParse(r1, out int foundGossipId))
            {
                gossipId = foundGossipId;
                return true;
            }

            gossipId = 0;
            return false;
        }

        /// <summary>
        /// Returns the count of available gossip options.
        /// </summary>
        public int LuaGetGossipOptionsCount()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetNumGossipOptions()"));
        }

        /// <summary>
        /// Retrieves information about an item in the player's inventory based on its slot.
        /// </summary>
        /// <param name="itemslot">The slot of the item in the inventory.</param>
        /// <returns>A JSON-formatted string containing information about the item, including its ID, count, quality, durability, cooldown, name, level, minimum level, type, subtype, maximum stack size, equipment slot, and sell price. If the item is not found, an empty string is returned.</returns>
        public string LuaGetItemBySlot(int itemslot)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:8}}={itemslot};{{v:0}}='noItem';{{v:1}}=GetInventoryItemID('player',{{v:8}});{{v:2}}=GetInventoryItemCount('player',{{v:8}});{{v:3}}=GetInventoryItemQuality('player',{{v:8}});{{v:4}},{{v:5}}=GetInventoryItemDurability({{v:8}});{{v:6}},{{v:7}}=GetInventoryItemCooldown('player',{{v:8}});{{v:9}},{{v:10}},{{v:11}},{{v:12}},{{v:13}},{{v:14}},{{v:15}},{{v:16}},{{v:17}},{{v:18}},{{v:19}}=GetItemInfo(GetInventoryItemLink('player',{{v:8}}));{{v:0}}='{{'..'\"id\": \"'..tostring({{v:1}} or 0)..'\",'..'\"count\": \"'..tostring({{v:2}} or 0)..'\",'..'\"quality\": \"'..tostring({{v:3}} or 0)..'\",'..'\"curDurability\": \"'..tostring({{v:4}} or 0)..'\",'..'\"maxDurability\": \"'..tostring({{v:5}} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({{v:6}} or 0)..'\",'..'\"cooldownEnd\": '..tostring({{v:7}} or 0)..','..'\"name\": \"'..tostring({{v:9}} or 0)..'\",'..'\"link\": \"'..tostring({{v:10}} or 0)..'\",'..'\"level\": \"'..tostring({{v:12}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:13}} or 0)..'\",'..'\"type\": \"'..tostring({{v:14}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:15}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:16}} or 0)..'\",'..'\"equipslot\": \"'..tostring({{v:17}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:19}} or 0)..'\"'..'}}';"), out string result) ? result : string.Empty;
        }

        ///<summary>
        /// Executes a Lua script to retrieve the stats of an item specified by the provided item link.
        /// The item link should be in the format "{v:1}='{itemLink}'{v:0}='' {v:2}={} {v:3}=GetItemStats({v:1},{v:2}) {v:0}='{\"stamina\": \"'..tostring({v:2}[\"ITEM_MOD_STAMINA_SHORT\"]or 0)..
        ///    '\",'..'\"agility\": \"'..tostring({v:2}[\"ITEM_MOD_AGILITY_SHORT\"]or 0)..
        ///    '\",'..'\"strength\": \"'..tostring({v:2}[\"ITEM_MOD_STRENGTH_SHORT\"]or 0)..
        ///    '\",'..'\"intellect\": \"'..tostring({v:2}[\"ITEM_MOD_INTELLECT_SHORT\"]or 0)..'\",'..
        ///    '\"spirit\": \"'..tostring({v:2}[\"ITEM_MOD_SPIRIT_SHORT\"]or 0)..
        ///    '\",'..'\"attackpower\": \"'..tostring({v:2}[\"ITEM_MOD_ATTACK_POWER_SHORT\"]or 0)..
        ///    '\",'..'\"spellpower\": \"'..tostring({v:2}[\"ITEM_MOD_SPELL_POWER_SHORT\"]or 0)..
        ///    '\",'..'\"mana\": \"'..tostring({v:2}[\"ITEM_MOD_MANA_SHORT\"]or 0)..'\""..'}}'"), 
        ///    and returns the results as a string. If the execution is successful, the result will contain the acquired item stats.
        ///    Otherwise, it will return an empty string.
        ///</summary>
        public string LuaGetItemStats(string itemLink)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}=\"{itemLink}\"{{v:0}}=''{{v:2}}={{}}{{v:3}}=GetItemStats({{v:1}},{{v:2}}){{v:0}}='{{'..'\"stamina\": \"'..tostring({{v:2}}[\"ITEM_MOD_STAMINA_SHORT\"]or 0)..'\",'..'\"agility\": \"'..tostring({{v:2}}[\"ITEM_MOD_AGILITY_SHORT\"]or 0)..'\",'..'\"strenght\": \"'..tostring({{v:2}}[\"ITEM_MOD_STRENGHT_SHORT\"]or 0)..'\",'..'\"intellect\": \"'..tostring({{v:2}}[\"ITEM_MOD_INTELLECT_SHORT\"]or 0)..'\",'..'\"spirit\": \"'..tostring({{v:2}}[\"ITEM_MOD_SPIRIT_SHORT\"]or 0)..'\",'..'\"attackpower\": \"'..tostring({{v:2}}[\"ITEM_MOD_ATTACK_POWER_SHORT\"]or 0)..'\",'..'\"spellpower\": \"'..tostring({{v:2}}[\"ITEM_MOD_SPELL_POWER_SHORT\"]or 0)..'\",'..'\"mana\": \"'..tostring({{v:2}}[\"ITEM_MOD_MANA_SHORT\"]or 0)..'\"'..'}}'"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Returns a boolean value indicating if the Lua unit has any stealable buffs.
        /// </summary>
        public bool LuaHasUnitStealableBuffs(string luaUnit)
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=0;local y=0;for i=1,40 do local n,_,_,_,_,_,_,_,{{v:1}}=UnitAura(\"{luaUnit}\",i);if {{v:1}}==1 then {{v:0}}=1;end end"));
        }

        /// <summary>
        /// Returns a boolean indicating if the Lua background invite is ready.
        /// </summary>
        public bool LuaIsBgInviteReady()
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua("{v:0}=0;for i=1,2 do local x=GetBattlefieldPortExpiration(i) if x>0 then {v:0}=1 end end"));
        }

        /// <summary>
        /// Checks if the specified Lua unit is a ghost.
        /// </summary>
        /// <param name="luaUnit">The Lua unit to check.</param>
        /// <returns>True if the Lua unit is a ghost, false otherwise.</returns>
        public bool LuaIsGhost(string luaUnit)
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=UnitIsGhost(\"{luaUnit}\");"));
        }

        /// <summary>
        /// Kicks NPCs out of the vehicle by using a Lua script.
        /// </summary>
        public void LuaKickNpcsOutOfVehicle()
        {
            LuaDoString("for i=1,2 do EjectPassengerFromSeat(i) end");
        }

        /// <summary>
        /// Queues the player into a battleground with the specified name.
        /// </summary>
        /// <param name="bgName">The name of the battleground to queue for.</param>
        public void LuaQueueBattlegroundByName(string bgName)
        {
            LuaDoString(BotUtils.ObfuscateLua($"for i=1,GetNumBattlegroundTypes() do {{v:0}}=GetBattlegroundInfo(i)if {{v:0}}==\"{bgName}\"then JoinBattlefield(i) end end").Item1);
        }

        /// <summary>
        /// This method sells all items in the player's inventory by executing a Lua script on the game client.
        /// It iterates through each container slot and retrieves the item link. 
        /// If an item is found, it gets the item information and adds its value to a running total. 
        /// Finally, it uses the LuaDoString method to execute the script and sell the items.
        /// </summary>
        public void LuaSellAllItems()
        {
            LuaDoString("local a,b,c=0;for d=0,4 do for e=1,GetContainerNumSlots(d)do c=GetContainerItemLink(d,e)if c then b={GetItemInfo(c)}a=a+b[11]UseContainerItem(d,e)end end end");
        }

        /// <summary>
        /// Sells items in Lua by their name.
        /// </summary>
        /// <param name="itemName">The name of the item to be sold.</param>
        public void LuaSellItemsByName(string itemName)
        {
            LuaDoString($"for a=0,4,1 do for b=1,GetContainerNumSlots(a),1 do local c=GetContainerItemLink(a,b)if c and string.find(c,\"{itemName}\")then UseContainerItem(a,b)end end end");
        }

        /// <summary>
        /// Sends an item mail to a specific character.
        /// </summary>
        /// <param name="itemName">The name of the item to be sent.</param>
        /// <param name="receiver">The name of the character who will receive the item.</param>
        public void LuaSendItemMailToCharacter(string itemName, string receiver)
        {
            LuaDoString($"for a=0,4 do for b=0,36 do I=GetContainerItemLink(a,b)if I and I:find(\"{itemName}\")then UseContainerItem(a,b)end end end;SendMailNameEditBox:SetText(\"{receiver}\")");
            ClickUiElement("SendMailMailButton");
        }

        /// <summary>
        /// Sets the target unit in Lua by executing the Lua command "TargetUnit("unit");".
        /// </summary>
        public void LuaTargetUnit(string unit)
        {
            LuaDoString($"TargetUnit(\"{unit}\");");
        }

        /// <summary>
        /// Executes a Lua function to query the completed quests.
        /// </summary>
        public void QueryQuestsCompleted()
        {
            LuaDoString("QueryQuestsCompleted()");
        }

        /// <summary>
        /// Repairs all items.
        /// </summary>
        public void RepairAllItems()
        {
            LuaDoString("RepairAllItems()");
        }

        /// <summary>
        /// Calls the Lua function "RepopMe()" to respawn the player character.
        /// </summary>
        public void RepopMe()
        {
            LuaDoString("RepopMe()");
        }

        /// <summary>
        /// Retrieves the player's corpse by executing the Lua function "RetrieveCorpse()".
        /// </summary>
        public void RetrieveCorpse()
        {
            LuaDoString("RetrieveCorpse()");
        }

        /// <summary>
        /// Roll something on a dropped item
        /// </summary>
        /// <param name="rollId">The rolls id to roll on</param>
        /// <param name="rollType">Need, Greed or Pass</param>
        public void RollOnLoot(int rollId, WowRollType rollType)
        {
            if (rollType == WowRollType.Need)
            {
                // first we need to check whether we can roll a need on this, otherwise the bot
                // might not roll at all
                LuaDoString($"_,_,_,_,_,canNeed=GetLootRollItemInfo({rollId});if canNeed then RollOnLoot({rollId}, {(int)rollType}) else RollOnLoot({rollId}, 2) end");
            }
            else
            {
                LuaDoString($"RollOnLoot({rollId}, {(int)rollType})");
            }
        }

        /// <summary>
        /// Selects the active quest for gossip with the given ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip.</param>
        public void SelectGossipActiveQuest(int gossipId)
        {
            LuaDoString($"SelectGossipActiveQuest({gossipId})");
        }

        /// <summary>
        /// Selects the available quest with the specified gossip ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip.</param>
        public void SelectGossipAvailableQuest(int gossipId)
        {
            LuaDoString($"SelectGossipAvailableQuest({gossipId})");
        }

        /// <summary>
        /// Selects a gossip option based on the provided gossip ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip option to select.</param>
        public void SelectGossipOption(int gossipId)
        {
            LuaDoString($"SelectGossipOption(max({gossipId}, GetNumGossipOptions()))");
        }

        /// <summary>
        /// Selects a gossip option with the given ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip option to select.</param>
        public void SelectGossipOptionSimple(int gossipId)
        {
            LuaDoString($"SelectGossipOption({gossipId})");
        }

        /// <summary>
        /// Selects a quest in the QuestFrame based on the given quest name, gossip ID, and availability.
        /// If the QuestFrame is currently shown, it searches for the quest by its icon identifier and text.
        /// If found, it clicks on the quest to select it.
        /// If not found, it searches for the quest by its icon identifier and gossip ID.
        /// If found, it clicks on the quest to select it.
        /// If the QuestFrame is not shown, it searches for the quest in the available quests list based on the quest name.
        /// If found, it selects the quest using the appropriate select function.
        /// If not found, it selects the quest based on the given gossip ID using the appropriate select function.
        /// </summary>
        public void SelectQuestByNameOrGossipId(string questName, int gossipId, bool isAvailableQuest)
        {
            string identifier = isAvailableQuest ? "AvailableQuestIcon" : "ActiveQuestIcon";
            string selectFunction = isAvailableQuest ? "SelectGossipAvailableQuest" : "SelectGossipActiveQuest";

            LuaDoString($"if QuestFrame ~= nil and QuestFrame:IsShown() then " +
                        $"local foundQuest=false; for i=1,20 do local f=getglobal(\"QuestTitleButton\"..i); if f then local fi=getglobal(\"QuestTitleButton\"..i..\"QuestIcon\"); if fi and fi:GetTexture() ~= nil and string.find(fi:GetTexture(), \"{identifier}\") and f:GetText() ~= nil and string.find(f:GetText(), \"{questName}\") then f:Click(); foundQuest=true; break; end; else break; end; end; " +
                        $"if not foundQuest then for i=1,20 do local f=getglobal(\"QuestTitleButton\"..i); if f then local fi=getglobal(\"QuestTitleButton\"..i..\"QuestIcon\"); if fi and fi:GetTexture() ~= nil and string.find(fi:GetTexture(), \"{identifier}\") and f:GetID() == {gossipId} then f:Click(); break; end; else break; end; end; end; " +
                        $"else " +
                        $"local foundQuest=false; local g1,_,_,_,_,g2,_,_,_,_,g3,_,_,_,_,g4,_,_,_,_,g5,_,_,_,_,g6 = GetGossipAvailableQuests(); local gps={{g1,g2,g3,g4,g5,g6}}; for k,v in pairs(gps) do if v == \"{questName}\" then {selectFunction}(k); foundQuest=true; break end; end; " +
                        $"if not foundQuest then {selectFunction}({gossipId}); end; " +
                        $"end");
        }

        /// <summary>
        /// Selects a quest log entry using the specified questLogEntry parameter.
        /// </summary>
        public void SelectQuestLogEntry(int questLogEntry)
        {
            LuaDoString($"SelectQuestLogEntry({questLogEntry})");
        }

        /// <summary>
        /// Executes the Lua function "GetQuestReward" with the given ID as parameter.
        /// </summary>
        public void SelectQuestReward(int id)
        {
            LuaDoString($"GetQuestReward({id})");
        }

        ///<summary>
        ///Sends a chat message to the default chat frame.
        ///</summary>
        ///<param name="message">The message to be sent.</param>
        public void SendChatMessage(string message)
        {
            LuaDoString($"DEFAULT_CHAT_FRAME.editBox:SetText(\"{message}\") ChatEdit_SendText(DEFAULT_CHAT_FRAME.editBox, 0)");
        }

        /// <summary>
        /// Sets the facing angle of the player.
        /// </summary>
        /// <param name="playerBase">The base address of the player.</param>
        /// <param name="angle">The angle at which the player should be facing.</param>
        /// <param name="smooth">Optional parameter to indicate if the movement should be smoothed. Default is false.</param>
        public void SetFacing(IntPtr playerBase, float angle, bool smooth = false)
        {
            Hook.SetFacing(playerBase, angle, smooth);
        }

        /// <summary>
        /// Sets the player's Looking for Group role based on the specified combat class role.
        /// </summary>
        /// <param name="combatClassRole">The combat class role to set as the player's Looking for Group role.</param>
        public void SetLfgRole(WowRole combatClassRole)
        {
            int[] roleBools = new int[3]
            {
                combatClassRole == WowRole.Tank ? 1:0,
                combatClassRole == WowRole.Heal ? 1:0,
                combatClassRole == WowRole.Dps ? 1:0
            };

            LuaDoString($"SetLFGRoles(0, {roleBools[0]}, {roleBools[1]}, {roleBools[2]});LFDRoleCheckPopupAcceptButton:Click()");
        }

        /// <summary>
        /// Sets the render state to the specified state.
        /// </summary>
        public void SetRenderState(bool state)
        {
            Hook.SetRenderState(state);
        }

        /// <summary>
        /// Sets up the hook by calling the Hook method with specified parameters.
        /// </summary>
        public bool Setup()
        {
            return Hook.Hook(7, HookModules);
        }

        /// <summary>
        /// Sets the check for whether the world is loaded or not.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c>, the check is enabled; otherwise, it is disabled.</param>
        public void SetWorldLoadedCheck(bool enabled)
        {
            Hook.BotOverrideWorldLoadedCheck(enabled);
        }

        /// <summary>
        /// Starts the auto attack and sends a chat message "/startattack".
        /// </summary>
        public void StartAutoAttack()
        {
            // UnitOnRightClick(wowUnit);
            SendChatMessage("/startattack");
        }

        /// <summary>
        /// Stops the current casting spell.
        /// </summary>
        public void StopCasting()
        {
            LuaDoString("SpellStopCasting()");
        }

        /// <summary>
        /// Stops the click-to-move feature.
        /// </summary>
        public void StopClickToMove()
        {
            if (IsClickToMoveActive())
            {
                // TODO: find better fix for spinning bug
                LuaDoString("MoveBackwardStart();MoveBackwardStop();");

                Hook.CallObjectFunction(Player.BaseAddress, Memory.Offsets.FunctionPlayerClickToMoveStop, null, false, out _);
            }
        }

        ///<summary>
        ///Executes a tick action.
        ///If the world is loaded, updates the WoW objects using the ObjectManager.
        ///Then, calls the GameInfoTick method from Hook class passing the player and the target as parameters.
        ///</summary>
        public void Tick()
        {
            if (ObjectManager.RefreshIsWorldLoaded())
            {
                ObjectManager.UpdateWowObjects();
            }

            Hook.GameInfoTick(ObjectManager.Player, ObjectManager.Target);
        }

        /// <summary>
        /// Determines if the specified UI elements are visible.
        /// </summary>
        /// <param name="uiElements">The array of UI elements to check.</param>
        /// <returns>True if any of the UI elements are visible, false otherwise.</returns>
        public bool UiIsVisible(params string[] uiElements)
        {
            StringBuilder sb = new();

            for (int i = 0; i < uiElements.Length; ++i)
            {
                sb.Append($"{uiElements[i]}:IsVisible()");

                if (i < uiElements.Length - 1)
                {
                    sb.Append($" or ");
                }
            }

            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=0 if {sb} then {{v:0}}=1 end"));
        }

        /// <summary>
        /// Uses the item in the specified bag slot of the container.
        /// </summary>
        public void UseContainerItem(int bagId, int bagSlot)
        {
            LuaDoString($"UseContainerItem({bagId}, {bagSlot})");
        }

        /// <summary>
        /// Uses the inventory item in the specified equipment slot.
        /// </summary>
        public void UseInventoryItem(WowEquipmentSlot equipmentSlot)
        {
            LuaDoString($"UseInventoryItem({(int)equipmentSlot})");
        }

        /// <summary>
        /// Uses an item by its name using the LuaSellItemsByName method.
        /// </summary>
        public void UseItemByName(string itemName)
        {
            LuaSellItemsByName(itemName);
        }

        /// <summary>
        /// Executes a Lua command and returns the result as an integer. 
        /// If the command execution is successful and the result can be parsed as an integer, 
        /// the parsed integer value is returned. Otherwise, 0 is returned.
        /// </summary>
        private int ExecuteLuaInt((string, string) cmdVar)
        {
            return ExecuteLuaAndRead(cmdVar, out string s)
                && int.TryParse(s, out int i)
                 ? i : 0;
        }

        /// <summary>
        /// Executes a Lua command and returns a boolean result indicating whether the execution was successful.
        /// The command is specified as a tuple consisting of two strings: cmdVar.Item1 represents the command to be executed,
        /// and cmdVar.Item2 represents an additional variable to be passed with the command.
        /// If the execution is successful, it checks if the resulting string can be parsed as an integer (int.TryParse).
        /// If the parsing is successful and the integer value is equal to 1, it returns true; otherwise, it returns false.
        /// </summary>
        private bool ExecuteLuaIntResult((string, string) cmdVar)
        {
            return ExecuteLuaAndRead(cmdVar, out string s)
                && int.TryParse(s, out int i)
                && i == 1;
        }
    }
}