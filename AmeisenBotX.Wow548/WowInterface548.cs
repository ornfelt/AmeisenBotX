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
using AmeisenBotX.Wow548.Hook;
using AmeisenBotX.Wow548.Objects;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Wow548
{
    /// <summary>
    /// WowInterface for the game version 5.4.8 18414.
    /// </summary>
    public class WowInterface548 : IWowInterface
    {
        /// <summary>
        /// Initializes a new instance of the WowInterface548 class.
        /// </summary>
        /// <param name="memory">The WowMemoryApi instance to be used.</param>
        public WowInterface548(WowMemoryApi memory)
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
                $"{battlegroundStatusVarName}=\"\"for b=1,2 do local c,d,e,f,g,h=GetBattlefieldStatus(b)local i=GetBattlefieldTimeWaited(b)/1000;{battlegroundStatusVarName}={battlegroundStatusVarName}..b..\":\"..tostring(c or\"unknown\")..\":\"..tostring(d or\"unknown\")..\":\"..tostring(e or\"unknown\")..\":\"..tostring(f or\"unknown\")..\":\"..tostring(g or\"unknown\")..\":\"..tostring(h or\"unknown\")..\":\"..tostring(i or\"unknown\")..\";\"end",
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
        /// Event triggered when the battleground status changes.
        /// </summary>
        public event Action<string>? OnBattlegroundStatus;

        /// <summary>
        /// Event that is raised when a static popup is triggered.
        /// </summary>
        public event Action<string>? OnStaticPopup;

        /// <summary>
        /// Gets or sets the event manager for managing events.
        /// </summary>
        public IEventManager Events => EventManager;

        /// <summary>
        /// Gets the current count of the hook calls.
        /// </summary>
        public int HookCallCount => Hook.HookCallCount;

        /// <summary>
        /// Gets a value indicating whether the hook is ready for use.
        /// </summary>
        public bool IsReady => Hook.IsWoWHooked;

        /// <summary>
        /// Gets the WowMemoryApi property.
        /// </summary>
        public WowMemoryApi Memory { get; }

        /// <summary>
        /// Gets the object provider.
        /// </summary>
        public IObjectProvider ObjectProvider => ObjectManager;

        /// <summary>
        /// Gets the current World of Warcraft player from the ObjectManager.
        /// </summary>
        public IWowPlayer Player => ObjectManager.Player;

        /// <summary>
        /// Gets or sets the version of the World of Warcraft game.
        /// </summary>
        /// <value>
        /// The version of the game.
        /// </value>
        public WowVersion WowVersion { get; } = WowVersion.MoP548;

        /// <summary>
        /// Gets the private SimpleEventManager object used for managing events.
        /// </summary>
        private SimpleEventManager EventManager { get; }

        /// <summary>
        /// Gets or sets the private EndSceneHook548.
        /// </summary>
        private EndSceneHook548 Hook { get; }

        /// <summary>
        /// Gets or sets the list of hook modules.
        /// </summary>
        private List<IHookModule> HookModules { get; }

        /// <summary>
        /// Gets the private ObjectManager548 ObjectManager.
        /// </summary>
        private ObjectManager548 ObjectManager { get; }

        /// <summary>
        /// Abandons the quests not present in the specified collection.
        /// </summary>
        /// <param name="quests">The collection of quests to abandon.</param>
        public void AbandonQuestsNotIn(IEnumerable<string> quests)
        {
            Hook.LuaAbandonQuestsNotIn(quests);
        }

        /// <summary>
        /// Accepts a battleground invite by clicking the UI element with the name "StaticPopup1Button1".
        /// </summary>
        public void AcceptBattlegroundInvite()
        {
            ClickUiElement("StaticPopup1Button1");
        }

        /// <summary>
        /// Accepts a party invitation by sending the appropriate Lua command and hiding the party invitation popup.
        /// </summary>
        public void AcceptPartyInvite()
        {
            LuaDoString("AcceptGroup();StaticPopup_Hide(\"PARTY_INVITE\")");
        }

        ///<summary>
        ///Accepts the current quest by executing a Lua script command.
        ///</summary>
        public void AcceptQuest()
        {
            LuaDoString($"AcceptQuest()");
        }

        /// <summary>
        /// Accepts available quests and completes active quests in the current dialogue.
        /// </summary>
        public void AcceptQuests()
        {
            LuaDoString("active=GetNumGossipActiveQuests()if active>0 then for a=1,active do if not not select(a*5-5+4,GetGossipActiveQuests())then SelectGossipActiveQuest(a)end end end;available=GetNumGossipAvailableQuests()if available>0 then for a=1,available do if not not not select(a*6-6+3,GetGossipAvailableQuests())then SelectGossipAvailableQuest(a)end end end;if available==0 and active==0 and GetNumGossipOptions()==1 then _,type=GetGossipOptions()if type=='gossip'then SelectGossipOption(1)return end end");
        }

        /// <summary>
        /// Executes the Lua function "AcceptResurrect()" and processes the resurrect action.
        /// </summary>
        public void AcceptResurrect()
        {
            LuaDoString("AcceptResurrect();");
        }

        /// <summary>
        /// Accepts a summon by invoking the Lua function "ConfirmSummon()" and hides the "CONFIRM_SUMMON" static popup.
        /// </summary>
        public void AcceptSummon()
        {
            LuaDoString("ConfirmSummon();StaticPopup_Hide(\"CONFIRM_SUMMON\")");
        }

        /// <summary>
        /// This method is used to buy a trainer service with the specified service index.
        /// It calls the LuaDoString method to execute the Lua code "BuyTrainerService(serviceIndex)".
        /// </summary>
        public void BuyTrainerService(int serviceIndex)
        {
            LuaDoString($"BuyTrainerService({serviceIndex})");
        }

        /// <summary>
        /// Calls a companion with the specified index and type.
        /// </summary>
        /// <param name="index">The index of the companion to call.</param>
        /// <param name="type">The type of the companion to call.</param>
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
        /// Casts a spell by the specified spell ID.
        /// </summary>
        /// <param name="spellId">The ID of the spell to be cast.</param>
        public void CastSpellById(int spellId)
        {
            LuaDoString($"CastSpellByID({spellId})");
        }

        /// <summary>
        /// Changes the target by calling the Hook.TargetGuid() method with the provided guid.
        /// </summary>
        public void ChangeTarget(ulong guid)
        {
            Hook.TargetGuid(guid);
        }

        /// <summary>
        /// Clears the target by calling the ChangeTarget method with a parameter of 0.
        /// </summary>
        public void ClearTarget()
        {
            ChangeTarget(0);
        }

        /// <summary>
        /// Clicks on the terrain at the specified position.
        /// </summary>
        public void ClickOnTerrain(Vector3 position)
        {
            Hook.ClickOnTerrain(position);
        }

        /// <summary>
        /// Method to click on the train button in the Blizzard Trainer UI.
        /// </summary>
        public void ClickOnTrainButton()
        {
            LuaDoString("LoadAddOn\"Blizzard_TrainerUI\"f=ClassTrainerTrainButton;f.e=0;if f:GetScript\"OnUpdate\"then f:SetScript(\"OnUpdate\",nil)else f:SetScript(\"OnUpdate\",function(f,a)f.e=f.e+a;if f.e>.01 then f.e=0;f:Click()end end)end");
        }

        /// <summary>
        /// Moves the player character to the specified position using click-to-move functionality.
        /// </summary>
        /// <param name="pos">The target position to move to.</param>
        /// <param name="guid">The unique identifier of the player character.</param>
        /// <param name="clickToMoveType">The type of click-to-move action to perform. Default is Move.</param>
        /// <param name="turnSpeed">The turn speed of the player character. Default is 20.9f.</param>
        /// <param name="distance">The maximum distance to move before stopping. Default is Move.</param>
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
        /// Clicks the specified UI element.
        /// </summary>
        /// <param name="elementName">The name of the UI element to be clicked.</param>
        public void ClickUiElement(string elementName)
        {
            LuaDoString($"{elementName}:Click()");
        }

        /// <summary>
        /// Confirms the loot roll by calling the CofirmStaticPopup() method.
        /// </summary>
        public void CofirmLootRoll()
        {
            CofirmStaticPopup();
        }

        /// <summary>
        /// Executes a Lua function to confirm the readiness check status.
        /// </summary>
        /// <param name="isReady">A boolean value indicating whether the player is ready or not.</param>
        public void CofirmReadyCheck(bool isReady)
        {
            LuaDoString($"ConfirmReadyCheck({isReady})");
        }

        /// <summary>
        /// This method is used to confirm a static popup for equipping and binding items. It executes a Lua string that performs a series of actions to confirm the static popup and hide it.
        /// </summary>
        public void CofirmStaticPopup()
        {
            LuaDoString($"EquipPendingItem(0);ConfirmBindOnUse();StaticPopup_Hide(\"AUTOEQUIP_BIND\");StaticPopup_Hide(\"EQUIP_BIND\");StaticPopup_Hide(\"USE_BIND\")");
        }

        /// <summary>
        /// Completes the current quest by executing the Lua command "CompleteQuest()".
        /// </summary>
        public void CompleteQuest()
        {
            LuaDoString($"CompleteQuest()");
        }

        /// <summary>
        /// Deletes an item from the player's inventory by its name.
        /// </summary>
        /// <param name="itemName">The name of the item to be deleted.</param>
        public void DeleteItemByName(string itemName)
        {
            LuaDoString($"for b=0,4 do for s=1,GetContainerNumSlots(b) do local l=GetContainerItemLink(b,s); if l and string.find(l, \"{itemName}\") then PickupContainerItem(b,s); DeleteCursorItem(); end; end; end");
        }

        /// <summary>
        /// Dismisses the companion of the specified type.
        /// </summary>
        /// <param name="type">The type of the companion to dismiss.</param>
        public void DismissCompanion(string type)
        {
            LuaDoString($"DismissCompanion(\"{type}\")");
        }

        /// <summary>
        /// Disposes the instance and unhooks any event handlers.
        /// </summary>
        public void Dispose()
        {
            Hook.Unhook();
        }

        /// <summary>
        /// Equips an item by its name in the specified item slot, or in any available slot if no slot is specified.
        /// </summary>
        /// <param name="newItem">The name of the item to be equipped.</param>
        /// <param name="itemSlot">The optional item slot to equip the item in. If not specified, the item will be equipped in any available slot.</param>
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
        /// Executes Lua script and reads the result.
        /// </summary>
        /// <param name="p">A tuple containing two strings.</param>
        /// <param name="result">The output result of executing the Lua script.</param>
        /// <returns>True if the Lua script was executed successfully and the result was obtained, false otherwise.</returns>
        public bool ExecuteLuaAndRead((string, string) p, out string result)
        {
            return Hook.ExecuteLuaAndRead(p, out result);
        }

        /// <summary>
        /// Updates the face position of a player in the game based on the provided parameters. 
        /// </summary>
        /// <param name="playerBase">The base address of the player.</param>
        /// <param name="playerPosition">The current position of the player.</param>
        /// <param name="position">The new position to be set for the player's face.</param>
        /// <param name="smooth">Determines whether the transition between positions should be smooth or abrupt (optional, default is false).</param>
        public void FacePosition(IntPtr playerBase, Vector3 playerPosition, Vector3 position, bool smooth = false)
        {
            Hook.FacePosition(playerBase, playerPosition, position, smooth);
        }

        /// <summary>
        /// Retrieves a collection of completed quests.
        /// </summary>
        /// <returns>An enumerable collection of integers representing the completed quests.</returns>
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
        /// Returns a formatted string representation of the equipment items in the player's inventory.
        /// The format is JSON-like and includes the following properties:
        ///   - "id": The item ID.
        ///   - "count": The number of items of the same ID in the inventory.
        ///   - "quality": The quality (rarity) of the item.
        ///   - "curDurability": The current durability of the item.
        ///   - "maxDurability": The maximum durability of the item.
        ///   - "cooldownStart": The start time of the item's cooldown, if any.
        ///   - "cooldownEnd": The end time of the item's cooldown, if any.
        ///   - "name": The name of the item.
        ///   - "link": The link (URL) of the item.
        ///   - "level": The required level to use the item.
        ///   - "minLevel": The minimum level to obtain the item.
        ///   - "type": The type of item (e.g., weapon, armor, consumable).
        ///   - "subtype": The subtype of item (e.g., sword, chest, potion).
        ///   - "maxStack": The maximum stack size for the item.
        ///   - "equipslot": The equipment slot index for the item.
        ///   - "equiplocation": The location of the equipment slot (e.g., head, hands, legs).
        ///   - "stats": An object containing additional statistics of the item.
        ///   - "sellprice": The sell price of the item.
        /// </summary>
        /// <returns>A formatted string representation of the equipment items in the player's inventory.</returns>
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
        /// Retrieves an array of gossip types by executing a Lua script and reading the result.
        /// </summary>
        /// <returns>An array of strings representing the gossip types.</returns>
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

        ///<summary>Returns an XML string that represents the inventory items. Each item in the inventory is represented as a JSON object containing various properties such as ID, count, quality, current durability, maximum durability, cooldown start, cooldown end, name, lootable status, readability, item link, level, minimum level, type, subtype, maximum stack size, equip location, sell price, and stats. The JSON objects for each item are enclosed in square brackets and separated by commas. The string is returned as the result or an empty string if the execution of the Lua script fails.</summary>
        public string GetInventoryItems()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"[\"for a=0,4 do {v:1}=GetContainerNumSlots(a)for b=1,{v:1} do {v:2}=GetContainerItemID(a,b)if string.len(tostring({v:2} or\"\"))>0 then {v:3}=GetContainerItemLink(a,b){v:4},{v:5}=GetContainerItemDurability(a,b){v:6},{v:7}=GetContainerItemCooldown(a,b){v:8},{v:9},{v:10},{v:11},{v:12},{v:13},{v:3},{v:14}=GetContainerItemInfo(a,b){v:15},{v:16},{v:17},{v:18},{v:19},{v:20},{v:21},{v:22},{v:23},{v:8},{v:24}=GetItemInfo({v:3}){v:25}=GetItemStats({v:3}){v:26}={}if {v:25} then for c,d in pairs({v:25})do table.insert({v:26},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;end;{v:0}={v:0}..\"{\"..'\"id\": \"'..tostring({v:2} or 0)..'\",'..'\"count\": \"'..tostring({v:9} or 0)..'\",'..'\"quality\": \"'..tostring({v:17} or 0)..'\",'..'\"curDurability\": \"'..tostring({v:4} or 0)..'\",'..'\"maxDurability\": \"'..tostring({v:5} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({v:6} or 0)..'\",'..'\"cooldownEnd\": \"'..tostring({v:7} or 0)..'\",'..'\"name\": \"'..tostring({v:15} or 0)..'\",'..'\"lootable\": \"'..tostring({v:13} or 0)..'\",'..'\"readable\": \"'..tostring({v:12} or 0)..'\",'..'\"link\": \"'..tostring({v:3} or 0)..'\",'..'\"level\": \"'..tostring({v:18} or 0)..'\",'..'\"minLevel\": \"'..tostring({v:19} or 0)..'\",'..'\"type\": \"'..tostring({v:20} or 0)..'\",'..'\"subtype\": \"'..tostring({v:21} or 0)..'\",'..'\"maxStack\": \"'..tostring({v:22} or 0)..'\",'..'\"equiplocation\": \"'..tostring({v:23} or 0)..'\",'..'\"sellprice\": \"'..tostring({v:24} or 0)..'\",'..'\"stats\": '..\"{\"..table.concat({v:26},\",\")..\"}\"..','..'\"bagid\": \"'..tostring(a or 0)..'\",'..'\"bagslot\": \"'..tostring(b or 0)..'\"'..\"}\"{v:0}={v:0}..\",\"end end end;{v:0}={v:0}..\"]\""), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves item information by name or link.
        /// Executes Lua script with the provided item name and reads the result.
        /// Returns the result as a string.
        /// If the Lua script execution is successful, the result will be the item information.
        /// If the Lua script execution fails, the result will be an empty string.
        /// </summary>
        /// <param name="itemName">The name of the item or its link.</param>
        /// <returns>A string containing the item information or an empty string.</returns>
        public string GetItemByNameOrLink(string itemName)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}=\"{itemName}\";{{v:0}}='noItem';{{v:2}},{{v:3}},{{v:4}},{{v:5}},{{v:6}},{{v:7}},{{v:8}},{{v:9}},{{v:10}},{{v:11}},{{v:12}}=GetItemInfo({{v:1}});{{v:13}}=GetItemStats({{v:3}}){{v:14}}={{}}for c,d in pairs({{v:13}})do table.insert({{v:14}},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;{{v:0}}='{{'..'\"id\": \"0\",'..'\"count\": \"1\",'..'\"quality\": \"'..tostring({{v:4}} or 0)..'\",'..'\"curDurability\": \"0\",'..'\"maxDurability\": \"0\",'..'\"cooldownStart\": \"0\",'..'\"cooldownEnd\": \"0\",'..'\"name\": \"'..tostring({{v:2}} or 0)..'\",'..'\"link\": \"'..tostring({{v:3}} or 0)..'\",'..'\"level\": \"'..tostring({{v:5}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:6}} or 0)..'\",'..'\"type\": \"'..tostring({{v:7}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:8}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:9}} or 0)..'\",'..'\"equiplocation\": \"'..tostring({{v:10}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:12}} or 0)..'\",'..'\"stats\": '..\"{{\"..table.concat({{v:14}},\",\")..\"}}\"..'}}';"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves the item link for a particular loot roll identified by the given rollId.
        /// </summary>
        /// <param name="rollId">The identifier for the loot roll.</param>
        /// <returns>The item link for the specified loot roll. Returns an empty string if the execution fails or no result is obtained.</returns>
        public string GetLootRollItemLink(int rollId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetLootRollItemLink({rollId});"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves the amount of money.
        /// </summary>
        public int GetMoney()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=GetMoney();"), out string s) ? int.TryParse(s, out int v) ? v : 0 : 0;
        }

        /// <summary>
        /// Retrieves a list of WowMount objects representing the mounts available to the character.
        /// </summary>
        /// <returns>An IEnumerable of WowMount objects.</returns>
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
        /// Retrieves the number of choices available in the quest log.
        /// </summary>
        /// <param name="numChoices">An output parameter that will contain the number of choices if the operation is successful.</param>
        /// <returns>True if the operation is successful, otherwise false.</returns>
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

        /// <summary>
        /// Gets the item link of a choice item in the quest log at the specified index.
        /// </summary>
        /// <param name="index">The index of the choice item in the quest log.</param>
        /// <param name="itemLink">The item link of the choice item.</param>
        /// <returns>True if the item link is obtained successfully, otherwise false.</returns>
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
        /// Finds the quest log ID by its title.
        /// </summary>
        /// <param name="title">The title of the quest log to search for.</param>
        /// <param name="questLogId">The ID of the quest log found, if it exists.</param>
        /// <returns>Returns true if the quest log is found and its ID is successfully assigned to questLogId, otherwise returns false.</returns>
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
        /// Returns the reaction between two WowUnit instances.
        /// </summary>
        /// <param name="a">The first WowUnit.</param>
        /// <param name="b">The second WowUnit.</param>
        /// <returns>The WowUnitReaction between the two WowUnit instances.</returns>
        public WowUnitReaction GetReaction(IntPtr a, IntPtr b)
        {
            return (WowUnitReaction)Hook.GetUnitReaction(a, b);
        }

        /// <summary>
        /// Retrieves the current number of ready runes and their respective types.
        /// </summary>
        /// <returns>A dictionary containing the rune types as keys and their corresponding counts as values.</returns>
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
        /// Gets the skills available to the player's class.
        /// </summary>
        /// <returns>A dictionary where the keys represent skill names and the values represent the skill level range.</returns>
        public Dictionary<string, (int, int)> GetSkills()
        {
            // TODO: adjust this for mop
            Dictionary<string, (int, int)> skills = new();

            switch (Player.Class)
            {
                case WowClass.Paladin:
                case WowClass.Warrior:
                    skills.Add("Plate Mail", (999, 999));
                    skills.Add("Mail", (999, 999));
                    skills.Add("Leather", (999, 999));
                    skills.Add("Cloth", (999, 999));
                    skills.Add("Shield", (999, 999));
                    break;

                case WowClass.Deathknight:
                    skills.Add("Plate Mail", (999, 999));
                    skills.Add("Mail", (999, 999));
                    skills.Add("Leather", (999, 999));
                    skills.Add("Cloth", (999, 999));
                    break;

                case WowClass.Hunter:
                case WowClass.Shaman:
                    skills.Add("Mail", (999, 999));
                    skills.Add("Leather", (999, 999));
                    skills.Add("Cloth", (999, 999));
                    break;

                case WowClass.Druid:
                case WowClass.Rogue:
                case WowClass.Monk:
                    skills.Add("Leather", (999, 999));
                    skills.Add("Cloth", (999, 999));
                    break;

                case WowClass.Priest:
                case WowClass.Mage:
                case WowClass.Warlock:
                    skills.Add("Cloth", (999, 999));
                    break;

                default:
                    break;
            }

            switch (Player.Class)
            {
                case WowClass.Warrior:
                    skills.Add("Axes", (999, 999));
                    skills.Add("Two-Handed Axes", (999, 999));
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Fist Weapons", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Two-Handed Maces", (999, 999));
                    skills.Add("Polearms", (999, 999));
                    skills.Add("Staves", (999, 999));
                    skills.Add("Swords", (999, 999));
                    skills.Add("Two-Handed Swords", (999, 999));
                    break;

                case WowClass.Paladin:
                    skills.Add("Axes", (999, 999));
                    skills.Add("Two-Handed Axes", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Two-Handed Maces", (999, 999));
                    skills.Add("Polearms", (999, 999));
                    skills.Add("Swords", (999, 999));
                    skills.Add("Two-Handed Swords", (999, 999));
                    break;

                case WowClass.Hunter:
                    skills.Add("Bows", (999, 999));
                    skills.Add("Crossbows", (999, 999));
                    skills.Add("Guns", (999, 999));

                    skills.Add("Axes", (999, 999));
                    skills.Add("Two-Handed Axes", (999, 999));
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Fist Weapons", (999, 999));
                    skills.Add("Polearms", (999, 999));
                    skills.Add("Staves", (999, 999));
                    skills.Add("Swords", (999, 999));
                    skills.Add("Two-Handed Swords", (999, 999));
                    break;

                case WowClass.Rogue:
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Fist Weapons", (999, 999));
                    skills.Add("Axes", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Swords", (999, 999));
                    break;

                case WowClass.Priest:
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Staves", (999, 999));
                    skills.Add("Wands", (999, 999));
                    break;

                case WowClass.Shaman:
                    skills.Add("Axes", (999, 999));
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Fist Weapons", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Two-Handed Maces", (999, 999));
                    skills.Add("Staves", (999, 999));
                    break;

                case WowClass.Mage:
                case WowClass.Warlock:
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Staves", (999, 999));
                    skills.Add("Swords", (999, 999));
                    skills.Add("Wands", (999, 999));
                    break;

                case WowClass.Monk:
                    skills.Add("Polearms", (999, 999));
                    skills.Add("Staves", (999, 999));
                    skills.Add("Axes", (999, 999));
                    skills.Add("Fist Weapons", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Swords", (999, 999));
                    break;

                case WowClass.Druid:
                    skills.Add("Daggers", (999, 999));
                    skills.Add("Fist Weapons", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Two-Handed Maces", (999, 999));
                    skills.Add("Polearms", (999, 999));
                    skills.Add("Staves", (999, 999));
                    break;

                case WowClass.Deathknight:
                    skills.Add("Axes", (999, 999));
                    skills.Add("Two-Handed Axes", (999, 999));
                    skills.Add("Maces", (999, 999));
                    skills.Add("Two-Handed Maces", (999, 999));
                    skills.Add("Polearms", (999, 999));
                    skills.Add("Swords", (999, 999));
                    skills.Add("Two-Handed Swords", (999, 999));
                    break;

                default:
                    break;
            }

            return skills;
        }

        /// <summary>
        /// Retrieves the cooldown of a specified spell.
        /// </summary>
        /// <param name="spellName">The name of the spell to retrieve the cooldown for.</param>
        /// <returns>The cooldown of the specified spell in milliseconds.</returns>
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
        /// <returns>The name of the spell, or an empty string if the spell ID is not found.</returns>
        public string GetSpellNameById(int spellId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetSpellInfo({spellId});"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves a list of spells available to the character.
        /// </summary>
        public string GetSpells()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua(@"
                {v:0}='['
                {v:1}=GetNumSpellTabs()

                for a=1,{v:1} do
                    local {v:2},{v:3},{v:4},{v:5}=GetSpellTabInfo(a)

                    for b={v:4}+1,{v:4}+{v:5} do
                        local {v:6},{v:7}=GetSpellBookItemName(b,""BOOKTYPE_SPELL"")

                        if {v:6} then
                            local {v:8},{v:9},_,{v:10},_,_,{v:11},{v:12},{v:13}=GetSpellInfo({v:6})

                            {v:0}={v:0}..'{'..'""spellbookName"": ""'..tostring({v:2} or 0)..'"",'..'""spellbookId"": ""'..tostring(a or 0)..'"",'..'""name"": ""'..tostring({v:6} or 0)..'"",'..'""rank"": ""'..tostring({v:9} or 0)..'"",'..'""castTime"": ""'..tostring({v:11} or 0)..'"",'..'""minRange"": ""'..tostring({v:12} or 0)..'"",'..'""maxRange"": ""'..tostring({v:13} or 0)..'"",'..'""costs"": ""'..tostring({v:10} or 0)..'""'..'}'

                            if a<{v:1} or b<{v:4}+{v:5} then
                                {v:0}={v:0}..','
                            end
                        end
                    end
                end;

                {v:0}={v:0}..']'"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Gets the talents information as a formatted string.
        /// </summary>
        /// <returns>An empty string or the result of executing a Lua script.</returns>
        public string GetTalents()
        {
            // TODO: adjust this for mop
            return string.Empty; // ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"\"{v:4}=GetNumTalentTabs();for g=1,{v:4} do {v:1}=GetNumTalents(g)for h=1,{v:1} do a,b,c,d,{v:2},{v:3},e,f=GetTalentInfo(g,h){v:0}={v:0}..a..\";\"..g..\";\"..h..\";\"..{v:2}..\";\"..{v:3};if h<{v:1} then {v:0}={v:0}..\"|\"end end;if g<{v:4} then {v:0}={v:0}..\"|\"end end"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Gets the cost of the trainer service for the specified index.
        /// </summary>
        public void GetTrainerServiceCost(int serviceIndex)
        {
            // todo: returns moneyCost, talentCost, professionCost
            LuaDoString($"GetTrainerServiceCost({serviceIndex})");
        }

        /// <summary>
        /// Returns the name, rank, category, and expanded information of a trainer service.
        /// </summary>
        public void GetTrainerServiceInfo(int serviceIndex)
        {
            // todo: returns name, rank, category, expanded
            LuaDoString($"GetTrainerServiceInfo({serviceIndex})");
        }

        /// <summary>
        /// Gets the count of trainer services.
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
        /// Retrieves the total number of unspent talent points for the current player.
        /// </summary>
        public int GetUnspentTalentPoints()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetUnspentTalentPoints()"));
        }

        /// <summary>
        /// Interacts with the specified World of Warcraft object.
        /// </summary>
        /// <param name="obj">The object to interact with.</param>
        public void InteractWithObject(IWowObject obj)
        {
            Hook.InteractWithUnit(obj.Guid);
        }

        /// <summary>
        /// Interacts with the specified World of Warcraft unit.
        /// </summary>
        /// <param name="unit">The unit to interact with.</param>
        public void InteractWithUnit(IWowUnit unit)
        {
            Hook.InteractWithUnit(unit.Guid);
        }

        /// <summary>
        /// Checks if auto looting is enabled.
        /// </summary>
        public bool IsAutoLootEnabled()
        {
            return int.TryParse(LuaGetCVar("autoLootDefault"), out int result) && result == 1;
        }

        /// <summary>
        /// Determines if click-to-move is currently active.
        /// </summary>
        public bool IsClickToMoveActive()
        {
            return Memory.Read(Memory.Offsets.ClickToMoveAction, out int ctmState)
                && ctmState != 0    // None
                && ctmState != 3    // Stop
                && ctmState != 13;  // Halted
        }

        /// <summary>
        /// Checks if the current bot is in a looking for group (LFG) group.
        /// </summary>
        public bool IsInLfgGroup()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:1},{v:0}=GetLFGInfoServer()"), out string result)
                && bool.TryParse(result, out bool isInLfg)
                && isInLfg;
        }

        /// <summary>
        /// Determines if there is a clear line of sight between two points in 3D space, taking into account a height adjustment.
        /// </summary>
        /// <param name="start">The starting position in 3D space.</param>
        /// <param name="end">The ending position in 3D space.</param>
        /// <param name="heightAdjust">The amount to adjust the height of the starting and ending positions.</param>
        /// <returns>True if there is a clear line of sight between the start and end positions, otherwise false.</returns>
        public bool IsInLineOfSight(Vector3 start, Vector3 end, float heightAdjust = 1.5f)
        {
            start.Z += heightAdjust;
            end.Z += heightAdjust;
            return Hook.TraceLine(start, end, (uint)WowWorldFrameHitFlag.HitTestLOS);
        }

        /// <summary>
        /// Determines if the rune with the given ID is ready.
        /// </summary>
        public bool IsRuneReady(int runeId)
        {
            return Memory.Read(Memory.Offsets.Runes, out byte runeStatus) && ((1 << runeId) & runeStatus) != 0;
        }

        /// <summary>
        /// Clicks on the "Leave" button in the World State Score Frame to exit the battleground.
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
        /// Function to loot money and quest items.
        /// </summary>
        public void LootMoneyAndQuestItems()
        {
            LuaDoString("for a=GetNumLootItems(),1,-1 do slotType=GetLootSlotType(a)_,_,_,_,b,c=GetLootSlotInfo(a)if not locked and(c or b==LOOT_SLOT_MONEY or b==LOOT_SLOT_CURRENCY)then LootSlot(a)end end");
        }

        /// <summary>
        /// Completes a quest in the Lua scripting language and retrieves the quest reward.
        /// </summary>
        /// <param name="questlogId">The ID of the quest log to complete.</param>
        /// <param name="rewardId">The ID of the quest reward to retrieve.</param>
        /// <param name="gossipId">The ID of the gossip option that triggers the quest completion.</param>
        public void LuaCompleteQuestAndGetReward(int questlogId, int rewardId, int gossipId)
        {
            LuaDoString($"SelectGossipActiveQuest({gossipId});CompleteQuest({questlogId});GetQuestReward({rewardId})");
        }

        /// <summary>
        /// Declines a party invite by hiding the party invite static popup.
        /// </summary>
        public void LuaDeclinePartyInvite()
        {
            LuaDoString("StaticPopup_Hide(\"PARTY_INVITE\")");
        }

        /// <summary>
        /// Calls the Lua function "DeclineResurrect()" to decline a resurrect request.
        /// </summary>
        public void LuaDeclineResurrect()
        {
            LuaDoString("DeclineResurrect()");
        }

        /// <summary>
        /// Executes a Lua string and returns a boolean indicating whether the operation was successful.
        /// </summary>
        /// <param name="v">The Lua string to be executed.</param>
        /// <returns>A boolean indicating the success of the Lua string execution.</returns>
        public bool LuaDoString(string v)
        {
            return Hook.LuaDoString(v);
        }

        /// <summary>
        /// Executes a Lua script to obtain the value of a specified Console Variable (CVar) and returns it as a string. 
        /// </summary>
        /// <param name="CVar">The name of the Console Variable to retrieve the value from.</param>
        /// <returns>The value of the specified Console Variable as a string. If the Lua script execution fails or the value 
        /// cannot be retrieved, an empty string is returned.</returns>
        public string LuaGetCVar(string CVar)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetCVar(\"{CVar}\");"), out string s) ? s : string.Empty;
        }

        /// <summary>
        /// Retrieves the active quest title by its ID from the Lua gossip system.
        /// </summary>
        /// <param name="gossipId">The ID of the quest in the gossip system.</param>
        /// <param name="title">The output variable to store the quest title.</param>
        /// <returns>True if the quest title was successfully retrieved, False otherwise.</returns>
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
        /// Retrieves the Gossip ID of an active quest based on its title.
        /// </summary>
        /// <param name="title">The title of the quest.</param>
        /// <param name="gossipId">The Gossip ID of the active quest, if found.</param>
        /// <returns>True if the Gossip ID was successfully retrieved, false otherwise.</returns>
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
        /// Retrieves the gossip ID associated with a given available quest title.
        /// </summary>
        /// <param name="title">The title of the quest.</param>
        /// <param name="gossipId">The variable to store the found gossip ID.</param>
        /// <returns><see langword="true"/> if the gossip ID was successfully found and stored in <paramref name="gossipId"/>, otherwise <see langword="false"/>.</returns>
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
        /// Returns the number of available gossip options in Lua script.
        /// </summary>
        public int LuaGetGossipOptionsCount()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetNumGossipOptions()"));
        }

        /// <summary>
        /// Retrieves information about an item in the player's inventory based on the specified item slot.
        /// </summary>
        /// <param name="itemslot">The slot number of the item.</param>
        /// <returns>A JSON-formatted string containing various attributes of the item, such as its ID, count, quality, durability, cooldown, name, level, type, and sell price. If the item cannot be found, an empty string is returned.</returns>
        public string LuaGetItemBySlot(int itemslot)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:8}}={itemslot};{{v:0}}='noItem';{{v:1}}=GetInventoryItemID('player',{{v:8}});{{v:2}}=GetInventoryItemCount('player',{{v:8}});{{v:3}}=GetInventoryItemQuality('player',{{v:8}});{{v:4}},{{v:5}}=GetInventoryItemDurability({{v:8}});{{v:6}},{{v:7}}=GetInventoryItemCooldown('player',{{v:8}});{{v:9}},{{v:10}},{{v:11}},{{v:12}},{{v:13}},{{v:14}},{{v:15}},{{v:16}},{{v:17}},{{v:18}},{{v:19}}=GetItemInfo(GetInventoryItemLink('player',{{v:8}}));{{v:0}}='{{'..'\"id\": \"'..tostring({{v:1}} or 0)..'\",'..'\"count\": \"'..tostring({{v:2}} or 0)..'\",'..'\"quality\": \"'..tostring({{v:3}} or 0)..'\",'..'\"curDurability\": \"'..tostring({{v:4}} or 0)..'\",'..'\"maxDurability\": \"'..tostring({{v:5}} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({{v:6}} or 0)..'\",'..'\"cooldownEnd\": '..tostring({{v:7}} or 0)..','..'\"name\": \"'..tostring({{v:9}} or 0)..'\",'..'\"link\": \"'..tostring({{v:10}} or 0)..'\",'..'\"level\": \"'..tostring({{v:12}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:13}} or 0)..'\",'..'\"type\": \"'..tostring({{v:14}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:15}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:16}} or 0)..'\",'..'\"equipslot\": \"'..tostring({{v:17}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:19}} or 0)..'\"'..'}}';"), out string result) ? result : string.Empty;
        }

        ///<summary>
        /// Retrieves the item statistics for a given item link in Lua.
        ///</summary>
        ///<param name="itemLink">The link of the item.</param>
        ///<returns>The item statistics as a string.</returns>
        public string LuaGetItemStats(string itemLink)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}=\"{itemLink}\"{{v:0}}=''{{v:2}}={{}}{{v:3}}=GetItemStats({{v:1}},{{v:2}}){{v:0}}='{{'..'\"stamina\": \"'..tostring({{v:2}}[\"ITEM_MOD_STAMINA_SHORT\"]or 0)..'\",'..'\"agility\": \"'..tostring({{v:2}}[\"ITEM_MOD_AGILITY_SHORT\"]or 0)..'\",'..'\"strenght\": \"'..tostring({{v:2}}[\"ITEM_MOD_STRENGHT_SHORT\"]or 0)..'\",'..'\"intellect\": \"'..tostring({{v:2}}[\"ITEM_MOD_INTELLECT_SHORT\"]or 0)..'\",'..'\"spirit\": \"'..tostring({{v:2}}[\"ITEM_MOD_SPIRIT_SHORT\"]or 0)..'\",'..'\"attackpower\": \"'..tostring({{v:2}}[\"ITEM_MOD_ATTACK_POWER_SHORT\"]or 0)..'\",'..'\"spellpower\": \"'..tostring({{v:2}}[\"ITEM_MOD_SPELL_POWER_SHORT\"]or 0)..'\",'..'\"mana\": \"'..tostring({{v:2}}[\"ITEM_MOD_MANA_SHORT\"]or 0)..'\"'..'}}'"), out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Checks if a Lua unit has any stealable buffs.
        /// </summary>
        /// <param name="luaUnit">The Lua unit to check for stealable buffs.</param>
        /// <returns>True if the Lua unit has any stealable buffs, otherwise false.</returns>
        public bool LuaHasUnitStealableBuffs(string luaUnit)
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=0;local y=0;for i=1,40 do local n,_,_,_,_,_,_,_,{{v:1}}=UnitAura(\"{luaUnit}\",i);if {{v:1}}==1 then {{v:0}}=1;end end"));
        }

        /// <summary>
        /// Checks if the bot is ready to accept a background invitation in Lua.
        /// </summary>
        public bool LuaIsBgInviteReady()
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua("{v:0}=0;for i=1,2 do local x=GetBattlefieldPortExpiration(i) if x>0 then {v:0}=1 end end"));
        }

        /// <summary>
        /// Checks if the specified lua unit is a ghost.
        /// </summary>
        /// <param name="luaUnit">The lua unit to be checked.</param>
        /// <returns>Returns true if the lua unit is a ghost, otherwise false.</returns>
        public bool LuaIsGhost(string luaUnit)
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=UnitIsGhost(\"{luaUnit}\");"));
        }

        /// <summary>
        /// Kicks all NPCs out of the vehicle by calling the Lua script to eject passengers from each seat.
        /// </summary>
        public void LuaKickNpcsOutOfVehicle()
        {
            LuaDoString("for i=1,2 do EjectPassengerFromSeat(i) end");
        }

        /// <summary>
        /// Queues for a battleground using the specified battleground name.
        /// </summary>
        /// <param name="bgName">The name of the battleground to queue for.</param>
        public void LuaQueueBattlegroundByName(string bgName)
        {
            LuaDoString(BotUtils.ObfuscateLua($"for i=1,GetNumBattlegroundTypes() do {{v:0}}=GetBattlegroundInfo(i)if {{v:0}}==\"{bgName}\"then JoinBattlefield(i) end end").Item1);
        }

        /// <summary>
        /// Uses Lua script to sell all items in the player's inventory, retrieving the item information and summing up their sell value.
        /// </summary>
        public void LuaSellAllItems()
        {
            LuaDoString("local a,b,c=0;for d=0,4 do for e=1,GetContainerNumSlots(d)do c=GetContainerItemLink(d,e)if c then b={GetItemInfo(c)}a=a+b[11]UseContainerItem(d,e)end end end");
        }

        /// <summary>
        /// Sells all items with the specified name in the Lua script.
        /// </summary>
        public void LuaSellItemsByName(string itemName)
        {
            LuaDoString($"for a=0,4,1 do for b=1,GetContainerNumSlots(a),1 do local c=GetContainerItemLink(a,b)if c and string.find(c,\"{itemName}\")then UseContainerItem(a,b)end end end");
        }

        ///<summary>
        ///Sends an item mail to a specific character.
        ///</summary>
        public void LuaSendItemMailToCharacter(string itemName, string receiver)
        {
            LuaDoString($"for a=0,4 do for b=0,36 do I=GetContainerItemLink(a,b)if I and I:find(\"{itemName}\")then UseContainerItem(a,b)end end end;SendMailNameEditBox:SetText(\"{receiver}\")");
            ClickUiElement("SendMailMailButton");
        }

        /// <summary>
        /// Sets the Lua target unit by executing a Lua string.
        /// </summary>
        /// <param name="unit">The unit to target.</param>
        public void LuaTargetUnit(string unit)
        {
            LuaDoString($"TargetUnit(\"{unit}\");");
        }

        /// <summary>
        /// Executes a Lua command to query the number of quests completed.
        /// </summary>
        public void QueryQuestsCompleted()
        {
            LuaDoString("QueryQuestsCompleted()");
        }

        /// <summary>
        /// Repairs all items using the LuaDoString function to execute the "RepairAllItems()" Lua command.
        /// </summary>
        public void RepairAllItems()
        {
            LuaDoString("RepairAllItems()");
        }

        /// <summary>
        /// Calls the Lua function "RepopMe()" to respawn the player.
        /// </summary>
        public void RepopMe()
        {
            LuaDoString("RepopMe()");
        }

        /// <summary>
        /// Retrieves the player's corpse using a Lua string command.
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
        /// Executes a Lua command to select an active quest gossip by its ID.
        /// </summary>
        /// <param name="gossipId">The ID of the quest gossip to be selected.</param>
        public void SelectGossipActiveQuest(int gossipId)
        {
            LuaDoString($"SelectGossipActiveQuest({gossipId})");
        }

        /// <summary>
        /// Selects a gossip available quest based on the given gossip ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip available quest to select.</param>
        public void SelectGossipAvailableQuest(int gossipId)
        {
            LuaDoString($"SelectGossipAvailableQuest({gossipId})");
        }

        ///<summary>
        ///Selects a gossip option based on the provided gossipId.
        ///</summary>
        public void SelectGossipOption(int gossipId)
        {
            LuaDoString($"SelectGossipOption(max({gossipId}, GetNumGossipOptions()))");
        }

        /// <summary>
        /// Selects a simple gossip option with the specified ID.
        /// </summary>
        /// <param name="gossipId">The ID of the gossip option to select.</param>
        public void SelectGossipOptionSimple(int gossipId)
        {
            LuaDoString($"SelectGossipOption({gossipId})");
        }

        ///<summary>
        /// Selects a quest by either its name or gossip ID.
        ///</summary>
        ///<param name="questName">The name of the quest.</param>
        ///<param name="gossipId">The ID of the gossip.</param>
        ///<param name="isAvailableQuest">Determines if the quest is available or active.</param>
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
        /// Selects a specific quest log entry by calling the LuaDoString method.
        /// </summary>
        /// <param name="questLogEntry">The index of the quest log entry to be selected.</param>
        public void SelectQuestLogEntry(int questLogEntry)
        {
            LuaDoString($"SelectQuestLogEntry({questLogEntry})");
        }

        /// <summary>
        /// Selects a quest reward based on the given ID.
        /// </summary>
        public void SelectQuestReward(int id)
        {
            LuaDoString($"GetQuestReward({id})");
        }

        /// <summary>
        /// Sends a chat message with the provided message string.
        /// </summary>
        public void SendChatMessage(string message)
        {
            LuaDoString($"DEFAULT_CHAT_FRAME.editBox:SetText(\"{message}\") ChatEdit_SendText(DEFAULT_CHAT_FRAME.editBox, 0)");
        }

        /// <summary>
        /// Sets the facing direction of the player.
        /// </summary>
        /// <param name="playerBase">The base address of the player</param>
        /// <param name="angle">The angle at which the player should be facing</param>
        /// <param name="smooth">Determines whether the facing should be smooth or not</param>
        public void SetFacing(IntPtr playerBase, float angle, bool smooth = false)
        {
            Hook.SetFacing(playerBase, angle, smooth);
        }

        /// <summary>
        /// Sets the Looking for Group (LFG) role based on the specified combat class role.
        /// </summary>
        /// <param name="combatClassRole">The combat class role to set the LFG role as.</param>
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
        /// Sets the render state based on the provided boolean value.
        /// </summary>
        public void SetRenderState(bool state)
        {
            Hook.SetRenderState(state);
        }

        /// <summary>
        /// Setup the Hook by calling the Hook method of the Hook class with the specified parameters 7 and HookModules. Returns a boolean value indicating if the setup was successful.
        /// </summary>
        public bool Setup()
        {
            return Hook.Hook(7, HookModules);
        }

        /// <summary>
        /// Sets the flag for overriding the world loaded check in the bot.
        /// </summary>
        /// <param name="enabled">Determines whether the world loaded check should be overridden or not.</param>
        public void SetWorldLoadedCheck(bool enabled)
        {
            Hook.BotOverrideWorldLoadedCheck(enabled);
        }

        ///<summary>
        /// Method to initiate auto-attack. It sends a chat message to start the attack.
        ///</summary>
        public void StartAutoAttack()
        {
            // UnitOnRightClick(wowUnit);
            SendChatMessage("/startattack");
        }

        /// <summary>
        /// Stops the current casting of a spell.
        /// </summary>
        public void StopCasting()
        {
            LuaDoString("SpellStopCasting()");
        }

        /// <summary>
        /// Stops click to move if it is active.
        /// </summary>
        public void StopClickToMove()
        {
            if (IsClickToMoveActive())
            {
                // TODO: find better fix for spinning bug
                // LuaDoString("MoveBackwardStart();MoveBackwardStop();");
                // Hook.CallObjectFunction(Player.BaseAddress,
                // Memory.Offsets.FunctionPlayerClickToMoveStop, null, false, out _);
                ClickToMove(Player.Position, 0, WowClickToMoveType.Stop);
            }
        }

        /// <summary>
        /// Updates the game state by refreshing the world loaded status and updating WoW objects.
        /// Also triggers the game info tick using the player and target objects from the object manager.
        /// </summary>
        public void Tick()
        {
            if (ObjectManager.RefreshIsWorldLoaded())
            {
                ObjectManager.UpdateWowObjects();
            }

            Hook.GameInfoTick(ObjectManager.Player, ObjectManager.Target);
        }

        /// <summary>
        /// Determines whether the specified UI elements are visible.
        /// </summary>
        /// <param name="uiElements">The UI elements to check.</param>
        /// <returns>True if any of the UI elements are visible, otherwise false.</returns>
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
        /// Uses a container item from a specific bag and slot.
        /// </summary>
        /// <param name="bagId">The id of the bag containing the item.</param>
        /// <param name="bagSlot">The slot of the item inside the bag.</param>
        public void UseContainerItem(int bagId, int bagSlot)
        {
            LuaDoString($"UseContainerItem({bagId}, {bagSlot})");
        }

        /// <summary>
        /// Uses the inventory item in the specified equipment slot.
        /// </summary>
        /// <param name="equipmentSlot">The equipment slot to use.</param>
        public void UseInventoryItem(WowEquipmentSlot equipmentSlot)
        {
            LuaDoString($"UseInventoryItem({(int)equipmentSlot})");
        }

        /// <summary>
        /// Uses the specified item by its name.
        /// </summary>
        /// <param name="itemName">The name of the item to be used.</param>
        public void UseItemByName(string itemName)
        {
            LuaSellItemsByName(itemName);
        }

        /// <summary>
        /// Executes a Lua function and returns the result as an integer.
        /// </summary>
        /// <param name="cmdVar">The command and variable to execute in Lua.</param>
        /// <returns>The result of the Lua function as an integer. If the function fails to execute or the result cannot be parsed as an integer, 0 is returned.</returns>
        private int ExecuteLuaInt((string, string) cmdVar)
        {
            return ExecuteLuaAndRead(cmdVar, out string s)
                && int.TryParse(s, out int i)
                 ? i : 0;
        }

        /// <summary>
        /// Executes a Lua command and returns a boolean indicating if the result is an integer value of 1.
        /// </summary>
        /// <param name="cmdVar">A tuple containing the Lua command as a string and the variable to store the result in.</param>
        /// <returns>A boolean indicating if the result of the Lua command is an integer value of 1.</returns>
        private bool ExecuteLuaIntResult((string, string) cmdVar)
        {
            return ExecuteLuaAndRead(cmdVar, out string s)
                && int.TryParse(s, out int i)
                && i == 1;
        }
    }
}