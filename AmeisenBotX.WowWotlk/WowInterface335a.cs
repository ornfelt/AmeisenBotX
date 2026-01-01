using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.MPQ;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Events;
using AmeisenBotX.Wow.Hook.Modules;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Constants;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Flags;
using AmeisenBotX.Wow.Shared.Lua;
using AmeisenBotX.WowWotlk.Hook;
using AmeisenBotX.WowWotlk.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmeisenBotX.WowWotlk
{
    /// <summary>
    /// WowInterface for the game version 3.3.5a 12340.
    /// </summary>
    public class WowInterface335a : IWowInterface
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public WowInterface335a(WowMemoryApi memory, string wowDirectory)
        {
            Memory = memory;
            HookModules = [];

            // lua variable names for the spell usability module
            string spellUsabilityVarName = BotUtils.FastRandomStringOnlyLetters();
            string lastSpellCheckTimeVarName = BotUtils.FastRandomStringOnlyLetters();
            string oldSpellUsabilityString = string.Empty;

            // module to check usable spells efficiently
            HookModules.Add(new RunLuaHookModule
            (
                (x) =>
                {
                    if (x != nint.Zero
                        && memory.ReadString(x, Encoding.UTF8, out string s)
                        && !string.IsNullOrWhiteSpace(s))
                    {
                        if (!oldSpellUsabilityString.Equals(s, StringComparison.Ordinal))
                        {
                            try
                            {
                                _usableSpells = [.. s.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)];
                            }
                            catch { /* handle parse error silently */ }
                            oldSpellUsabilityString = s;
                        }
                    }
                },
                null,
                memory,
                BotUtils.ObfuscateLua($"if not {lastSpellCheckTimeVarName} or GetTime() - {lastSpellCheckTimeVarName} > 0.5 then {lastSpellCheckTimeVarName} = GetTime(); {spellUsabilityVarName} = \"\"; local numTabs = GetNumSpellTabs(); for i=1,numTabs do local _, _, offset, numSpells = GetSpellTabInfo(i); for j=offset+1,offset+numSpells do local spellName, rank = GetSpellName(j, \"BOOKTYPE_SPELL\"); if spellName then local _, _, _, _, _, _, _, _, _, _, spellId = GetSpellInfo(spellName, rank); if spellId then local usable, noMana = IsUsableSpell(spellName); local start, duration = GetSpellCooldown(spellName); if usable and not noMana and (start == 0 or duration <= 1.5) then {spellUsabilityVarName} = {spellUsabilityVarName} .. spellId .. \";\"; end end end end end end").Item1,
                spellUsabilityVarName
            ));

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
                    if (x != nint.Zero
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
                    if (x != nint.Zero
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
                    if (x != nint.Zero
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
                    nint dataPtr = x.GetDataPointer();

                    if (dataPtr != nint.Zero && Player != null)
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

            Mpq = new(wowDirectory);
            Dbc = new(Mpq);
            Mpq.PreloadAsync();
        }

        public event Action<string> OnBattlegroundStatus;

        public event Action<string> OnStaticPopup;

        public IEventManager Events => EventManager;

        public int HookCallCount => Hook.HookCallCount;

        public bool IsReady => Hook.IsWoWHooked;

        public WowMemoryApi Memory { get; }

        public IObjectProvider ObjectProvider => ObjectManager;

        public IWowPlayer Player => ObjectManager.Player;

        public WowVersion WowVersion { get; } = WowVersion.WotLK335a;

        private SimpleEventManager EventManager { get; }

        private EndSceneHook335a Hook { get; }

        public MpqBridge Mpq { get; }

        public DbcBridge Dbc { get; }

        private List<IHookModule> HookModules { get; }

        private ObjectManager335a ObjectManager { get; }

        public void AbandonQuestsNotIn(IEnumerable<string> quests)
        {
            Hook.LuaAbandonQuestsNotIn(quests);
        }

        public void AcceptBattlegroundInvite()
        {
            ClickUiElement("StaticPopup1Button1");
        }

        public void AcceptPartyInvite()
        {
            LuaDoString("AcceptGroup();StaticPopup_Hide(\"PARTY_INVITE\")");
        }

        public void AcceptQuest()
        {
            LuaDoString($"AcceptQuest()");
        }

        public void AcceptQuests()
        {
            LuaDoString("active=GetNumGossipActiveQuests()if active>0 then for a=1,active do if not not select(a*5-5+4,GetGossipActiveQuests())then SelectGossipActiveQuest(a)end end end;available=GetNumGossipAvailableQuests()if available>0 then for a=1,available do if not not not select(a*6-6+3,GetGossipAvailableQuests())then SelectGossipAvailableQuest(a)end end end;if available==0 and active==0 and GetNumGossipOptions()==1 then _,type=GetGossipOptions()if type=='gossip'then SelectGossipOption(1)return end end");
        }

        public void AcceptResurrect()
        {
            LuaDoString("AcceptResurrect();");
        }

        public void AcceptSummon()
        {
            LuaDoString("ConfirmSummon();StaticPopup_Hide(\"CONFIRM_SUMMON\")");
        }

        public void ApplyBotCVars(int maxFps)
        {
            // WotLK 3.3.5a specific CVars for maximum bot performance
            StringBuilder cvars = new();

            // Core FPS settings
            cvars.Append($"pcall(SetCVar,\"maxfps\",\"{maxFps}\");");
            cvars.Append($"pcall(SetCVar,\"maxfpsbk\",\"{maxFps}\");");
            cvars.Append("pcall(SetCVar,\"AutoInteract\",\"1\");");
            cvars.Append("pcall(SetCVar,\"AutoLootDefault\",\"0\");");

            // Display - Potato mode
            cvars.Append("pcall(SetCVar,\"gxWindow\",\"1\");");
            cvars.Append("pcall(SetCVar,\"gxMaximize\",\"0\");");
            cvars.Append("pcall(SetCVar,\"gxResolution\",\"640x480\");");
            cvars.Append("pcall(SetCVar,\"gxRefresh\",\"60\");");
            cvars.Append("pcall(SetCVar,\"gxMultisampleQuality\",\"0\");");
            cvars.Append("pcall(SetCVar,\"gxmultisample\",\"1\");");
            cvars.Append("pcall(SetCVar,\"gxcolorbits\",\"16\");");
            cvars.Append("pcall(SetCVar,\"gxdepthbits\",\"16\");");
            cvars.Append("pcall(SetCVar,\"bitdepth\",\"16\");");
            cvars.Append("pcall(SetCVar,\"gxTripleBuffer\",\"0\");");
            cvars.Append("pcall(SetCVar,\"gxVSync\",\"0\");");
            cvars.Append("pcall(SetCVar,\"gxCursor\",\"0\");");
            cvars.Append("pcall(SetCVar,\"gxFixLag\",\"0\");");

            // Shadows - Off
            cvars.Append("pcall(SetCVar,\"shadowlevel\",\"0\");");
            cvars.Append("pcall(SetCVar,\"shadowlod\",\"0\");");
            cvars.Append("pcall(SetCVar,\"extshadowquality\",\"0\");");
            cvars.Append("pcall(SetCVar,\"mapshadows\",\"0\");");
            cvars.Append("pcall(SetCVar,\"showshadow\",\"0\");");
            cvars.Append("pcall(SetCVar,\"hwPCF\",\"0\");");

            // Draw Distance - Minimum
            cvars.Append("pcall(SetCVar,\"farclip\",\"100\");");
            cvars.Append("pcall(SetCVar,\"horizonfarclip\",\"100\");");
            cvars.Append("pcall(SetCVar,\"overridefarclip\",\"0\");");
            cvars.Append("pcall(SetCVar,\"unitdrawdist\",\"1\");");

            // Ground Effects - Off
            cvars.Append("pcall(SetCVar,\"groundeffectdensity\",\"0\");");
            cvars.Append("pcall(SetCVar,\"groundeffectdist\",\"0\");");
            cvars.Append("pcall(SetCVar,\"detaildensity\",\"0\");");
            cvars.Append("pcall(SetCVar,\"detailDoodadAlpha\",\"0\");");
            cvars.Append("pcall(SetCVar,\"terrainMipLevel\",\"1\");");

            // Water - Off
            cvars.Append("pcall(SetCVar,\"showwater\",\"0\");");
            cvars.Append("pcall(SetCVar,\"waterlod\",\"0\");");
            cvars.Append("pcall(SetCVar,\"watermaxlod\",\"0\");");
            cvars.Append("pcall(SetCVar,\"waterparticulates\",\"0\");");
            cvars.Append("pcall(SetCVar,\"waterripples\",\"0\");");
            cvars.Append("pcall(SetCVar,\"waterspecular\",\"0\");");
            cvars.Append("pcall(SetCVar,\"waterwaves\",\"0\");");

            // Sky - Off
            cvars.Append("pcall(SetCVar,\"skyshow\",\"0\");");
            cvars.Append("pcall(SetCVar,\"skyclouddensity\",\"0\");");
            cvars.Append("pcall(SetCVar,\"skycloudlod\",\"0\");");
            cvars.Append("pcall(SetCVar,\"skysunglare\",\"0\");");

            // Particles & Effects - Off
            cvars.Append("pcall(SetCVar,\"particledensity\",\"0\");");
            cvars.Append("pcall(SetCVar,\"spelleffectlevel\",\"0\");");
            cvars.Append("pcall(SetCVar,\"ffx\",\"0\");");
            cvars.Append("pcall(SetCVar,\"showfootprints\",\"0\");");
            cvars.Append("pcall(SetCVar,\"showfootprintparticles\",\"0\");");
            cvars.Append("pcall(SetCVar,\"weatherDensity\",\"0\");");

            // Textures - Lowest
            cvars.Append("pcall(SetCVar,\"basemip\",\"1\");");
            cvars.Append("pcall(SetCVar,\"textureloddist\",\"0\");");
            cvars.Append("pcall(SetCVar,\"anisotropic\",\"0\");");
            cvars.Append("pcall(SetCVar,\"alphalevel\",\"0\");");
            cvars.Append("pcall(SetCVar,\"gxTextureCacheSize\",\"16\");");

            // Lighting - Off
            cvars.Append("pcall(SetCVar,\"light\",\"0\");");
            cvars.Append("pcall(SetCVar,\"maxlights\",\"0\");");
            cvars.Append("pcall(SetCVar,\"specular\",\"0\");");
            cvars.Append("pcall(SetCVar,\"characterAmbient\",\"0\");");

            // Models & LOD - Maximum culling
            cvars.Append("pcall(SetCVar,\"lod\",\"0\");");
            cvars.Append("pcall(SetCVar,\"loddist\",\"1\");");
            cvars.Append("pcall(SetCVar,\"maxlod\",\"0\");");
            cvars.Append("pcall(SetCVar,\"doodadanim\",\"0\");");
            cvars.Append("pcall(SetCVar,\"M2UseLod\",\"1\");");
            cvars.Append("pcall(SetCVar,\"M2Faster\",\"1\");");
            cvars.Append("pcall(SetCVar,\"smallcull\",\"1\");");

            // Environment
            cvars.Append("pcall(SetCVar,\"environmentDetail\",\"0\");");
            cvars.Append("pcall(SetCVar,\"fog\",\"0\");");
            cvars.Append("pcall(SetCVar,\"fullalpha\",\"0\");");
            cvars.Append("pcall(SetCVar,\"pixelshader\",\"0\");");
            cvars.Append("pcall(SetCVar,\"vertexshader\",\"0\");");

            // Sound - Off
            cvars.Append("pcall(SetCVar,\"Sound_EnableSFX\",\"0\");");
            cvars.Append("pcall(SetCVar,\"Sound_EnableMusic\",\"0\");");
            cvars.Append("pcall(SetCVar,\"Sound_EnableAmbience\",\"0\");");
            cvars.Append("pcall(SetCVar,\"Sound_EnableErrorSpeech\",\"0\");");
            cvars.Append("pcall(SetCVar,\"Sound_MasterVolume\",\"0\");");

            // UI - Minimal
            cvars.Append("pcall(SetCVar,\"timingmethod\",\"1\");");
            cvars.Append("pcall(SetCVar,\"scriptProfile\",\"0\");");
            cvars.Append("pcall(SetCVar,\"UIFasterFrameRate\",\"1\");");
            cvars.Append("pcall(SetCVar,\"showTargetCastbar\",\"0\");");
            cvars.Append("pcall(SetCVar,\"showTargetOfTarget\",\"0\");");
            cvars.Append("pcall(SetCVar,\"showPartyPets\",\"0\");");
            cvars.Append("pcall(SetCVar,\"showArenaEnemyFrames\",\"0\");");
            cvars.Append("pcall(SetCVar,\"nameplateShowAll\",\"0\");");
            cvars.Append("pcall(SetCVar,\"nameplateShowEnemies\",\"0\");");
            cvars.Append("pcall(SetCVar,\"nameplateShowFriends\",\"0\");");
            cvars.Append("pcall(SetCVar,\"enableCombatText\",\"0\");");
            cvars.Append("pcall(SetCVar,\"ShowErrors\",\"0\");");
            cvars.Append("pcall(SetCVar,\"scriptErrors\",\"0\");");

            // Bot optimizations
            cvars.Append("pcall(SetCVar,\"blockTrades\",\"1\");");
            cvars.Append("pcall(SetCVar,\"blockChannelInvites\",\"1\");");

            LuaDoString(cvars.ToString());
        }

        public void BuyTrainerService(int serviceIndex)
        {
            LuaDoString($"BuyTrainerService({serviceIndex})");
        }

        public void CallCompanion(int index, string type)
        {
            LuaDoString($"CallCompanion(\"{type}\", {index})");
        }

        public void CastSpell(string name, bool castOnSelf = false)
        {
            LuaDoString($"CastSpellByName(\"{name}\"{(castOnSelf ? ", \"player\"" : string.Empty)})");
        }



        private HashSet<int> _usableSpells = [];

        public bool CanCastSpell(int spellId)
        {
            return _usableSpells.Contains(spellId);
        }

        public void CastSpellById(int spellId)
        {
            LuaDoString($"CastSpellByID({spellId})");
        }

        public void ChangeTarget(ulong guid)
        {
            Hook.TargetGuid(guid);
        }

        public void ClearTarget()
        {
            ChangeTarget(0);
        }

        public void ClickOnTerrain(Vector3 position)
        {
            Hook.ClickOnTerrain(position);
        }

        public void ClickOnTrainButton()
        {
            LuaDoString("LoadAddOn\"Blizzard_TrainerUI\"f=ClassTrainerTrainButton;f.e=0;if f:GetScript\"OnUpdate\"then f:SetScript(\"OnUpdate\",nil)else f:SetScript(\"OnUpdate\",function(f,a)f.e=f.e+a;if f.e>.01 then f.e=0;f:Click()end end)end");
        }

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

        public void ClickUiElement(string elementName)
        {
            LuaDoString($"{elementName}:Click()");
        }

        public void ConfirmLootRoll()
        {
            ConfirmStaticPopup();
        }

        public void ConfirmReadyCheck(bool isReady)
        {
            LuaDoString($"ConfirmReadyCheck({isReady})");
        }

        public void ConfirmStaticPopup()
        {
            LuaDoString($"EquipPendingItem(0);ConfirmBindOnUse();StaticPopup_Hide(\"AUTOEQUIP_BIND\");StaticPopup_Hide(\"EQUIP_BIND\");StaticPopup_Hide(\"USE_BIND\")");
        }

        public void CompleteQuest()
        {
            LuaDoString($"CompleteQuest()");
        }

        public void DeleteItemByName(string itemName)
        {
            LuaDoString($"for b=0,4 do for s=1,GetContainerNumSlots(b) do local l=GetContainerItemLink(b,s); if l and string.find(l, \"{itemName}\") then PickupContainerItem(b,s); DeleteCursorItem(); end; end; end");
        }

        public void DismissCompanion(string type)
        {
            LuaDoString($"DismissCompanion(\"{type}\")");
        }

        public void Dispose()
        {
            Hook.Unhook();
        }

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

            ConfirmStaticPopup();
        }

        public bool ExecuteLuaAndRead((string, string) p, out string result)
        {
            return Hook.ExecuteLuaAndRead(p, out result);
        }

        public void FacePosition(nint playerBase, Vector3 playerPosition, Vector3 position, bool smooth = false)
        {
            Hook.FacePosition(playerBase, playerPosition, position, smooth);
        }

        public IEnumerable<int> GetCompletedQuests()
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=''for a,b in pairs(GetQuestsCompleted())do if b then {{v:0}}={{v:0}}..a..';'end end;"), out string result))
            {
                if (result != null && result.Length > 0)
                {
                    return result.Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => int.TryParse(e, out int n) ? n : (int?)null)
                        .Where(e => e.HasValue)
                        .Select(e => e.Value);
                }
            }

            return Array.Empty<int>();
        }

        public string GetEquipmentItems()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"[\"for a=0,23 do {v:1}=GetInventoryItemID(\"player\",a)if string.len(tostring({v:1} or\"\"))>0 then {v:2}=GetInventoryItemLink(\"player\",a){v:3}=GetInventoryItemCount(\"player\",a){v:4},{v:5}=GetInventoryItemDurability(a){v:6},{v:7}=GetInventoryItemCooldown(\"player\",a){v:8},{v:9},{v:10},{v:11},{v:12},{v:13},{v:14},{v:15},{v:16},{v:17},{v:18}=GetItemInfo({v:2}){v:19}=GetItemStats({v:2}){v:20}={}for b,c in pairs({v:19})do table.insert({v:20},string.format(\"\\\"%s\\\":\\\"%s\\\"\",b,c))end;{v:0}={v:0}..'{'..'\"id\": \"'..tostring({v:1} or 0)..'\",'..'\"count\": \"'..tostring({v:3} or 0)..'\",'..'\"quality\": \"'..tostring({v:10} or 0)..'\",'..'\"curDurability\": \"'..tostring({v:4} or 0)..'\",'..'\"maxDurability\": \"'..tostring({v:5} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({v:6} or 0)..'\",'..'\"cooldownEnd\": '..tostring({v:7} or 0)..','..'\"name\": \"'..tostring({v:8} or 0)..'\",'..'\"link\": \"'..tostring({v:9} or 0)..'\",'..'\"level\": \"'..tostring({v:11} or 0)..'\",'..'\"minLevel\": \"'..tostring({v:12} or 0)..'\",'..'\"type\": \"'..tostring({v:13} or 0)..'\",'..'\"subtype\": \"'..tostring({v:14} or 0)..'\",'..'\"maxStack\": \"'..tostring({v:15} or 0)..'\",'..'\"equipslot\": \"'..tostring(a or 0)..'\",'..'\"equiplocation\": \"'..tostring({v:16} or 0)..'\",'..'\"stats\": '..\"{\"..table.concat({v:20},\",\")..\"}\"..','..'\"sellprice\": \"'..tostring({v:18} or 0)..'\"'..'}'if a<23 then {v:0}={v:0}..\",\"end end end;{v:0}={v:0}..\"]\""), out string result) ? result : string.Empty;
        }

        public int GetContainerNumSlots(int bagId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetContainerNumSlots({bagId})"), out string result)
                && int.TryParse(result, out int slots) ? slots : 0;
        }

        public int GetFreeBagSlotCount()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=0 for i=1,5 do {v:0}={v:0}+GetContainerNumFreeSlots(i-1)end"), out string sresult)
                && int.TryParse(sresult, out int freeBagSlots)
                 ? freeBagSlots : 0;
        }

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

            return [];
        }

        public string GetInventoryItems()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"[\"for a=0,4 do {v:1}=GetContainerNumSlots(a)for b=1,{v:1} do {v:2}=GetContainerItemID(a,b)if string.len(tostring({v:2} or\"\"))>0 then {v:3}=GetContainerItemLink(a,b){v:4},{v:5}=GetContainerItemDurability(a,b){v:6},{v:7}=GetContainerItemCooldown(a,b){v:8},{v:9},{v:10},{v:11},{v:12},{v:13},{v:3},{v:14}=GetContainerItemInfo(a,b){v:15},{v:16},{v:17},{v:18},{v:19},{v:20},{v:21},{v:22},{v:23},{v:8},{v:24}=GetItemInfo({v:3}){v:25}=GetItemStats({v:3}){v:26}={}if {v:25} then for c,d in pairs({v:25})do table.insert({v:26},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;end;{v:0}={v:0}..\"{\"..'\"id\": \"'..tostring({v:2} or 0)..'\",'..'\"count\": \"'..tostring({v:9} or 0)..'\",'..'\"quality\": \"'..tostring({v:17} or 0)..'\",'..'\"curDurability\": \"'..tostring({v:4} or 0)..'\",'..'\"maxDurability\": \"'..tostring({v:5} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({v:6} or 0)..'\",'..'\"cooldownEnd\": \"'..tostring({v:7} or 0)..'\",'..'\"name\": \"'..tostring({v:15} or 0)..'\",'..'\"lootable\": \"'..tostring({v:13} or 0)..'\",'..'\"readable\": \"'..tostring({v:12} or 0)..'\",'..'\"link\": \"'..tostring({v:3} or 0)..'\",'..'\"level\": \"'..tostring({v:18} or 0)..'\",'..'\"minLevel\": \"'..tostring({v:19} or 0)..'\",'..'\"type\": \"'..tostring({v:20} or 0)..'\",'..'\"subtype\": \"'..tostring({v:21} or 0)..'\",'..'\"maxStack\": \"'..tostring({v:22} or 0)..'\",'..'\"equiplocation\": \"'..tostring({v:23} or 0)..'\",'..'\"sellprice\": \"'..tostring({v:24} or 0)..'\",'..'\"stats\": '..\"{\"..table.concat({v:26},\",\")..\"}\"..','..'\"bagid\": \"'..tostring(a or 0)..'\",'..'\"bagslot\": \"'..tostring(b or 0)..'\"'..\"}\"{v:0}={v:0}..\",\"end end end;{v:0}={v:0}..\"]\""), out string result) ? result : string.Empty;
        }

        public string GetItemByNameOrLink(string itemName)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}=\"{itemName}\";{{v:0}}='noItem';{{v:2}},{{v:3}},{{v:4}},{{v:5}},{{v:6}},{{v:7}},{{v:8}},{{v:9}},{{v:10}},{{v:11}},{{v:12}}=GetItemInfo({{v:1}});{{v:13}}=GetItemStats({{v:3}}){{v:14}}={{}}for c,d in pairs({{v:13}})do table.insert({{v:14}},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;{{v:0}}='{{'..'\"id\": \"0\",'..'\"count\": \"1\",'..'\"quality\": \"'..tostring({{v:4}} or 0)..'\",'..'\"curDurability\": \"0\",'..'\"maxDurability\": \"0\",'..'\"cooldownStart\": \"0\",'..'\"cooldownEnd\": \"0\",'..'\"name\": \"'..tostring({{v:2}} or 0)..'\",'..'\"link\": \"'..tostring({{v:3}} or 0)..'\",'..'\"level\": \"'..tostring({{v:5}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:6}} or 0)..'\",'..'\"type\": \"'..tostring({{v:7}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:8}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:9}} or 0)..'\",'..'\"equiplocation\": \"'..tostring({{v:10}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:12}} or 0)..'\",'..'\"stats\": '..\"{{\"..table.concat({{v:14}},\",\")..\"}}\"..'}}';"), out string result) ? result : string.Empty;
        }

        public string GetLootRollItemLink(int rollId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetLootRollItemLink({rollId});"), out string result) ? result : string.Empty;
        }

        public int GetMoney()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=GetMoney();"), out string s) ? int.TryParse(s, out int v) ? v : 0 : 0;
        }

        public IEnumerable<WowMount> GetMounts()
        {
            string mountJson = ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=\"[\"{{v:1}}=GetNumCompanions(\"MOUNT\")if {{v:1}}>0 then for b=1,{{v:1}} do {{v:4}},{{v:2}},{{v:3}}=GetCompanionInfo(\"mount\",b){{v:0}}={{v:0}}..\"{{\\\"name\\\":\\\"\"..{{v:2}}..\"\\\",\"..\"\\\"index\\\":\"..b..\",\"..\"\\\"spellId\\\":\"..{{v:3}}..\",\"..\"\\\"mountId\\\":\"..{{v:4}}..\",\"..\"}}\"if b<{{v:1}} then {{v:0}}={{v:0}}..\",\"end end end;{{v:0}}={{v:0}}..\"]\""), out string result) ? result : string.Empty;

            try
            {
                return JsonSerializer.Deserialize<List<WowMount>>(mountJson, Options);
            }
            catch (Exception e)
            {
                AmeisenLogger.I.Log("CharacterManager", $"Failed to parse Mounts JSON:\n{mountJson}\n{e}", LogLevel.Error);
            }

            return [];
        }

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

        public WowUnitReaction GetReaction(nint a, nint b)
        {
            return (WowUnitReaction)Hook.GetUnitReaction(a, b);
        }

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

        public Dictionary<string, (int, int)> GetSkills()
        {
            Dictionary<string, (int, int)> parsedSkills = [];

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

        public string GetSpellNameById(int spellId)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetSpellInfo({spellId});"), out string result) ? result : string.Empty;
        }

        public string GetSpells()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}='['{v:1}=GetNumSpellTabs()for a=1,{v:1} do {v:2},{v:3},{v:4},{v:5}=GetSpellTabInfo(a)for b={v:4}+1,{v:4}+{v:5} do {v:6},{v:7}=GetSpellName(b,\"BOOKTYPE_SPELL\")if {v:6} then {v:8},{v:9},_,{v:10},_,_,{v:11},{v:12},{v:13}=GetSpellInfo({v:6},{v:7}){v:0}={v:0}..'{'..'\"spellbookName\": \"'..tostring({v:2} or 0)..'\",'..'\"spellbookId\": \"'..tostring(a or 0)..'\",'..'\"name\": \"'..tostring({v:6} or 0)..'\",'..'\"rank\": \"'..tostring({v:9} or 0)..'\",'..'\"castTime\": \"'..tostring({v:11} or 0)..'\",'..'\"minRange\": \"'..tostring({v:12} or 0)..'\",'..'\"maxRange\": \"'..tostring({v:13} or 0)..'\",'..'\"costs\": \"'..tostring({v:10} or 0)..'\"'..'}'if a<{v:1} or b<{v:4}+{v:5} then {v:0}={v:0}..','end end end end;{v:0}={v:0}..']'"), out string result) ? result : string.Empty;
        }

        public string GetTalents()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"\"{v:4}=GetNumTalentTabs();for g=1,{v:4} do {v:1}=GetNumTalents(g)for h=1,{v:1} do a,b,c,d,{v:2},{v:3},e,f=GetTalentInfo(g,h){v:0}={v:0}..a..\";\"..g..\";\"..h..\";\"..{v:2}..\";\"..{v:3};if h<{v:1} then {v:0}={v:0}..\"|\"end end;if g<{v:4} then {v:0}={v:0}..\"|\"end end"), out string result) ? result : string.Empty;
        }

        public void GetTrainerServiceCost(int serviceIndex)
        {
            // todo: returns moneyCost, talentCost, professionCost
            LuaDoString($"GetTrainerServiceCost({serviceIndex})");
        }

        public void GetTrainerServiceInfo(int serviceIndex)
        {
            // todo: returns name, rank, category, expanded
            LuaDoString($"GetTrainerServiceInfo({serviceIndex})");
        }

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

            return double.TryParse(str.Split(',')[1], out double timeRemaining)
                ? (str.Split(',')[0], (int)Math.Round(timeRemaining, 0))
                : (string.Empty, 0);
        }

        public int GetUnspentTalentPoints()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetUnspentTalentPoints()"));
        }

        public void InspectUnit(string unit)
        {
            LuaDoString($"NotifyInspect(\"{unit}\")");
        }

        public (int primaryTree, int tree1, int tree2, int tree3) GetInspectedUnitTalentSpec()
        {
            // Get talent points in each tree for the inspected unit
            // GetTalentTabInfo(tabIndex, inspect, isGuild, talentGroup) returns: name, iconTexture, pointsSpent, fileName
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua(
                "{v:0}=\"0;0;0;0\";" +
                "local _,_,t1=GetTalentTabInfo(1,true,false);" +
                "local _,_,t2=GetTalentTabInfo(2,true,false);" +
                "local _,_,t3=GetTalentTabInfo(3,true,false);" +
                "if t1 and t2 and t3 then " +
                "local primary=1;" +
                "if t2>t1 and t2>t3 then primary=2 elseif t3>t1 and t3>t2 then primary=3 end;" +
                "{v:0}=primary..\";\"..t1..\";\"..t2..\";\"..t3;" +
                "end"), out string result))
            {
                string[] parts = result.Split(';');
                if (parts.Length == 4
                    && int.TryParse(parts[0], out int primary)
                    && int.TryParse(parts[1], out int t1)
                    && int.TryParse(parts[2], out int t2)
                    && int.TryParse(parts[3], out int t3))
                {
                    return (primary, t1, t2, t3);
                }
            }

            return (0, 0, 0, 0);
        }

        public void InteractWithObject(IWowObject obj)
        {
            Hook.ObjectRightClick(obj.BaseAddress);
        }

        public void InteractWithUnit(IWowUnit unit)
        {
            Hook.InteractWithUnit(unit.BaseAddress);
        }

        public bool IsAutoLootEnabled()
        {
            return int.TryParse(LuaGetCVar("autoLootDefault"), out int result) && result == 1;
        }

        public bool IsClickToMoveActive()
        {
            return Memory.Read(Memory.Offsets.ClickToMoveAction, out int ctmState)
                && ctmState != 0    // None
                && ctmState != 3    // Stop
                && ctmState != 13;  // Halted
        }

        public bool IsInLfgGroup()
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:1},{v:0}=GetLFGInfoServer()"), out string result)
                && bool.TryParse(result, out bool isInLfg)
                && isInLfg;
        }

        public bool IsInLineOfSight(Vector3 start, Vector3 end, float heightAdjust = 1.5f)
        {
            start.Z += heightAdjust;
            end.Z += heightAdjust;
            return Hook.TraceLine(start, end, (uint)WowWorldFrameHitFlag.HitTestLOS);
        }

        public bool TraceLine(Vector3 start, Vector3 end, uint flags) => Hook.TraceLine(start, end, flags);

        public bool IsRuneReady(int runeId)
        {
            return Memory.Read(Memory.Offsets.Runes, out byte runeStatus) && ((1 << runeId) & runeStatus) != 0;
        }

        public void LeaveBattleground()
        {
            ClickUiElement("WorldStateScoreFrameLeaveButton");
        }

        public void LootEverything()
        {
            LuaDoString(BotUtils.ObfuscateLua("{v:0}=GetNumLootItems()for a={v:0},1,-1 do LootSlot(a)ConfirmLootSlot(a)end").Item1);
        }

        public void LootMoneyAndQuestItems()
        {
            LuaDoString("for a=GetNumLootItems(),1,-1 do local l=GetLootSlotLink(a) local _,_,_,_,locked=GetLootSlotInfo(a) local q=false if l then local _,_,_,_,_,t=GetItemInfo(l) if t=='Quest' then q=true end end if not locked and (not l or q) then LootSlot(a) end end");
        }

        public void LuaCompleteQuestAndGetReward(int questlogId, int rewardId, int gossipId)
        {
            LuaDoString($"SelectGossipActiveQuest({gossipId});CompleteQuest({questlogId});GetQuestReward({rewardId})");
        }

        public void LuaDeclinePartyInvite()
        {
            LuaDoString("StaticPopup_Hide(\"PARTY_INVITE\")");
        }

        public void LuaDeclineResurrect()
        {
            LuaDoString("DeclineResurrect()");
        }

        public bool LuaDoString(string v)
        {
            return Hook.LuaDoString(v);
        }

        public string LuaGetCVar(string CVar)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetCVar(\"{CVar}\");"), out string s) ? s : string.Empty;
        }

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

        public int LuaGetGossipOptionsCount()
        {
            return ExecuteLuaInt(BotUtils.ObfuscateLua("{v:0}=GetNumGossipOptions()"));
        }

        public string LuaGetItemBySlot(int itemslot)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:8}}={itemslot};{{v:0}}='noItem';{{v:1}}=GetInventoryItemID('player',{{v:8}});{{v:2}}=GetInventoryItemCount('player',{{v:8}});{{v:3}}=GetInventoryItemQuality('player',{{v:8}});{{v:4}},{{v:5}}=GetInventoryItemDurability({{v:8}});{{v:6}},{{v:7}}=GetInventoryItemCooldown('player',{{v:8}});{{v:9}},{{v:10}},{{v:11}},{{v:12}},{{v:13}},{{v:14}},{{v:15}},{{v:16}},{{v:17}},{{v:18}},{{v:19}}=GetItemInfo(GetInventoryItemLink('player',{{v:8}}));{{v:0}}='{{'..'\"id\": \"'..tostring({{v:1}} or 0)..'\",'..'\"count\": \"'..tostring({{v:2}} or 0)..'\",'..'\"quality\": \"'..tostring({{v:3}} or 0)..'\",'..'\"curDurability\": \"'..tostring({{v:4}} or 0)..'\",'..'\"maxDurability\": \"'..tostring({{v:5}} or 0)..'\",'..'\"cooldownStart\": \"'..tostring({{v:6}} or 0)..'\",'..'\"cooldownEnd\": '..tostring({{v:7}} or 0)..','..'\"name\": \"'..tostring({{v:9}} or 0)..'\",'..'\"link\": \"'..tostring({{v:10}} or 0)..'\",'..'\"level\": \"'..tostring({{v:12}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:13}} or 0)..'\",'..'\"type\": \"'..tostring({{v:14}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:15}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:16}} or 0)..'\",'..'\"equipslot\": \"'..tostring({{v:17}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:19}} or 0)..'\"'..'}}';"), out string result) ? result : string.Empty;
        }

        public string LuaGetItemStats(string itemLink)
        {
            return ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}=\"{itemLink}\"{{v:0}}=''{{v:2}}={{}}{{v:3}}=GetItemStats({{v:1}},{{v:2}}){{v:0}}='{{'..'\"stamina\": \"'..tostring({{v:2}}[\"ITEM_MOD_STAMINA_SHORT\"]or 0)..'\",'..'\"agility\": \"'..tostring({{v:2}}[\"ITEM_MOD_AGILITY_SHORT\"]or 0)..'\",'..'\"strenght\": \"'..tostring({{v:2}}[\"ITEM_MOD_STRENGHT_SHORT\"]or 0)..'\",'..'\"intellect\": \"'..tostring({{v:2}}[\"ITEM_MOD_INTELLECT_SHORT\"]or 0)..'\",'..'\"spirit\": \"'..tostring({{v:2}}[\"ITEM_MOD_SPIRIT_SHORT\"]or 0)..'\",'..'\"attackpower\": \"'..tostring({{v:2}}[\"ITEM_MOD_ATTACK_POWER_SHORT\"]or 0)..'\",'..'\"spellpower\": \"'..tostring({{v:2}}[\"ITEM_MOD_SPELL_POWER_SHORT\"]or 0)..'\",'..'\"mana\": \"'..tostring({{v:2}}[\"ITEM_MOD_MANA_SHORT\"]or 0)..'\"'..'}}'"), out string result) ? result : string.Empty;
        }

        public bool LuaHasUnitStealableBuffs(string luaUnit)
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=0;local y=0;for i=1,40 do local n,_,_,_,_,_,_,_,{{v:1}}=UnitAura(\"{luaUnit}\",i);if {{v:1}}==1 then {{v:0}}=1;end end"));
        }

        public bool LuaIsBgInviteReady()
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua("{v:0}=0;for i=1,2 do local x=GetBattlefieldPortExpiration(i) if x>0 then {v:0}=1 end end"));
        }

        public bool LuaIsGhost(string luaUnit)
        {
            return ExecuteLuaIntResult(BotUtils.ObfuscateLua($"{{v:0}}=UnitIsGhost(\"{luaUnit}\");"));
        }

        public void LuaKickNpcsOutOfVehicle()
        {
            LuaDoString("for i=1,2 do EjectPassengerFromSeat(i) end");
        }

        public void LuaQueueBattlegroundByName(string bgName)
        {
            LuaDoString(BotUtils.ObfuscateLua($"for i=1,GetNumBattlegroundTypes() do {{v:0}}=GetBattlegroundInfo(i)if {{v:0}}==\"{bgName}\"then JoinBattlefield(i) end end").Item1);
        }

        public void LuaSellAllItems()
        {
            LuaDoString("local a,b,c=0;for d=0,4 do for e=1,GetContainerNumSlots(d)do c=GetContainerItemLink(d,e)if c then b={GetItemInfo(c)}a=a+b[11]UseContainerItem(d,e)end end end");
        }

        public void LuaSellItemsByName(string itemName)
        {
            LuaDoString($"for a=0,4,1 do for b=1,GetContainerNumSlots(a),1 do local c=GetContainerItemLink(a,b)if c and string.find(c,\"{itemName}\")then UseContainerItem(a,b)end end end");
        }

        public void LuaSendItemMailToCharacter(string itemName, string receiver)
        {
            LuaDoString($"for a=0,4 do for b=0,36 do I=GetContainerItemLink(a,b)if I and I:find(\"{itemName}\")then UseContainerItem(a,b)end end end;SendMailNameEditBox:SetText(\"{receiver}\")");
            ClickUiElement("SendMailMailButton");
        }

        public void LuaTargetUnit(string unit)
        {
            LuaDoString($"TargetUnit(\"{unit}\");");
        }

        public void QueryQuestsCompleted()
        {
            LuaDoString("QueryQuestsCompleted()");
        }

        public void RepairAllItems()
        {
            LuaDoString("RepairAllItems()");
        }

        public void RepopMe()
        {
            LuaDoString("RepopMe()");
        }

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

        public void SelectGossipActiveQuest(int gossipId)
        {
            LuaDoString($"SelectGossipActiveQuest({gossipId})");
        }

        public void SelectGossipAvailableQuest(int gossipId)
        {
            LuaDoString($"SelectGossipAvailableQuest({gossipId})");
        }

        public void SelectGossipOption(int gossipId)
        {
            LuaDoString($"SelectGossipOption(max({gossipId}, GetNumGossipOptions()))");
        }

        public void SelectGossipOptionSimple(int gossipId)
        {
            LuaDoString($"SelectGossipOption({gossipId})");
        }

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

        public void SelectQuestLogEntry(int questLogEntry)
        {
            LuaDoString($"SelectQuestLogEntry({questLogEntry})");
        }

        public void SelectQuestReward(int id)
        {
            LuaDoString($"GetQuestReward({id})");
        }

        public void SendChatMessage(string message)
        {
            LuaDoString($"DEFAULT_CHAT_FRAME.editBox:SetText(\"{message}\") ChatEdit_SendText(DEFAULT_CHAT_FRAME.editBox, 0)");
        }

        public void SetFacing(nint playerBase, float angle, bool smooth = false)
        {
            Hook.SetFacing(playerBase, angle, smooth);
        }

        public void SetLfgRole(WowRole combatClassRole)
        {
            int[] roleBools =
            [
                combatClassRole == WowRole.Tank ? 1 : 0,
                combatClassRole == WowRole.Heal ? 1 : 0,
                combatClassRole == WowRole.Dps ? 1 : 0
            ];

            LuaDoString($"SetLFGRoles(0, {roleBools[0]}, {roleBools[1]}, {roleBools[2]});LFDRoleCheckPopupAcceptButton:Click()");
        }

        public void SetRenderState(bool state)
        {
            Hook.SetRenderState(state);
        }

        public bool Setup()
        {
            return Hook.Hook(7, HookModules);
        }

        public void SetWorldLoadedCheck(bool enabled)
        {
            Hook.BotOverrideWorldLoadedCheck(enabled);
        }

        public void StartAutoAttack()
        {
            // UnitOnRightClick(wowUnit);
            SendChatMessage("/startattack");
        }

        public void StopCasting()
        {
            LuaDoString("SpellStopCasting()");
        }

        public void StopClickToMove()
        {
            if (IsClickToMoveActive())
            {
                // TODO: find better fix for spinning bug
                LuaDoString("MoveBackwardStart();MoveBackwardStop();");

                Hook.CallObjectFunction(Player.BaseAddress, Memory.Offsets.FunctionPlayerClickToMoveStop, null, false, out _);
            }
        }

        public void Tick()
        {
            if (ObjectManager.RefreshIsWorldLoaded())
            {
                ObjectManager.UpdateWowObjects();
            }

            Hook.GameInfoTick(ObjectManager.Player, ObjectManager.Target);
        }

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

        public void UseContainerItem(int bagId, int bagSlot)
        {
            LuaDoString($"UseContainerItem({bagId}, {bagSlot})");
        }

        public void UseInventoryItem(WowEquipmentSlot equipmentSlot)
        {
            LuaDoString($"UseInventoryItem({(int)equipmentSlot})");
        }

        public void UseItemByName(string itemName)
        {
            LuaSellItemsByName(itemName);
        }

        private int ExecuteLuaInt((string, string) cmdVar)
        {
            return ExecuteLuaAndRead(cmdVar, out string s)
                && int.TryParse(s, out int i)
                 ? i : 0;
        }

        private bool ExecuteLuaIntResult((string, string) cmdVar)
        {
            return ExecuteLuaAndRead(cmdVar, out string s)
                && int.TryParse(s, out int i)
                && i == 1;
        }
    }
}
