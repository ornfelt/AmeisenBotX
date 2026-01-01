using AmeisenBotX.Common.Keyboard.Objects;
using AmeisenBotX.Core.Engines.AI;
using AmeisenBotX.Core.Engines.Movement.Settings;
using AmeisenBotX.Memory.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Core
{
    public class AmeisenBotConfig
    {
        [Category("General")]
        [DisplayName("Anti AFK Interval (ms)")]
        [Description("Time in milliseconds between Anti-AFK actions.")]
        public int AntiAfkMs { get; set; } = 1000;

        [Category("Combat")]
        [DisplayName("Avoid AoE")]
        public bool AoeDetectionAvoid { get; set; } = false;

        [Category("Combat")]
        [DisplayName("AoE Extension")]
        public float AoeDetectionExtends { get; set; } = 1.0f;

        [Category("Combat")]
        [DisplayName("AoE Includes Players")]
        public bool AoeDetectionIncludePlayers { get; set; } = false;

        [Category("Questing")]
        [DisplayName("Auto Accept Quests")]
        public bool AutoAcceptQuests { get; set; } = true;

        [Category("Login")]
        [DisplayName("Auto Change Realmlist")]
        public bool AutoChangeRealmlist { get; set; } = true;

        [Category("Execution")]
        [DisplayName("Auto Close WoW")]
        public bool AutocloseWow { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Auto Destroy Trash")]
        public bool AutoDestroyTrash { get; set; } = true;

        [Category("Debug")]
        [DisplayName("Auto Disable Render")]
        public bool AutoDisableRender { get; set; } = false;

        [Category("PvP")]
        [DisplayName("Auto Join BG")]
        public bool AutojoinBg { get; set; } = true;

        [Category("Dungeon")]
        [DisplayName("Auto Join LFG")]
        public bool AutojoinLfg { get; set; } = true;

        [Category("Login")]
        [DisplayName("Auto Login")]
        public bool AutoLogin { get; set; } = true;

        [Category("Navigation")]
        [DisplayName("Enable Autopilot")]
        public bool Autopilot { get; set; } = false;

        [Category("Execution")]
        [DisplayName("Auto Position WoW Window")]
        public bool AutoPositionWow { get; set; } = false;

        [Category("General")]
        [DisplayName("Auto Repair")]
        public bool AutoRepair { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Auto Sell")]
        public bool AutoSell { get; set; } = true;

        [Category("Execution")]
        [DisplayName("Auto Low GFX")]
        [Description("Automatically sets WoW graphics settings to low for performance.")]
        public bool AutoSetUlowGfxSettings { get; set; } = true;

        [Category("Execution")]
        [DisplayName("Auto Start WoW")]
        public bool AutostartWow { get; set; } = true;

        [Category("Questing")]
        [DisplayName("Talk to Questgivers")]
        public bool AutoTalkToNearQuestgivers { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Min Free Slots to Sell")]
        public int BagSlotsToGoSell { get; set; } = 4;

        [Category("PvP")]
        [DisplayName("Battleground Engine")]
        public string BattlegroundEngine { get; set; } = string.Empty;

        [Category("PvP")]
        [DisplayName("BG Party Mode")]
        public bool BattlegroundUsePartyMode { get; set; } = false;

        [Browsable(false)]
        public Rect BotWindowRect { get; set; } = new Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 };

        [Category("Combat")]
        [DisplayName("Combat Class")]
        [Description("The name of the Built-In Combat Class to use.")]
        public string BuiltInCombatClassName { get; set; } = string.Empty;

        [Category("Professions")]
        [DisplayName("Craft Bandages")]
        [Description("If true, the bot will craft bandages from cloth when idle.")]
        [DefaultValue(true)]
        public bool CraftBandages { get; set; } = true;

        [Category("Navigation")]
        [DisplayName("Cache POIs")]
        public bool CachePointsOfInterest { get; set; } = true;

        [Category("Login")]
        [DisplayName("Character Slot")]
        [Description("The index of the character to login (starts at 0).")]
        public int CharacterSlot { get; set; } = 0;

        [Category("Chat")]
        [DisplayName("Log Chat Protocols")]
        public bool ChatProtocols { get; set; } = false;

        [Category("Combat")]
        [DisplayName("Custom CC Dependencies")]
        public List<string> CustomCombatClassDependencies { get; set; } = [];

        [Category("Combat")]
        [DisplayName("Custom CC File")]
        public string CustomCombatClassFile { get; set; } = string.Empty;

        [Category("Regeneration")]
        [DisplayName("Drink Start %")]
        public double DrinkStartPercent { get; set; } = 65.0;

        [Category("Regeneration")]
        [DisplayName("Drink Stop %")]
        public double DrinkUntilPercent { get; set; } = 85.0;

        [Category("Dungeon")]
        [DisplayName("Dungeon Party Mode")]
        public bool DungeonUsePartyMode { get; set; } = false;

        [Category("Party")]
        [DisplayName("Abort Follow (Eat/Drink)")]
        public bool EatDrinkAbortFollowParty { get; set; } = true;

        [Category("Party")]
        [DisplayName("Abort Follow Distance")]
        public float EatDrinkAbortFollowPartyDistance { get; set; } = 25.0f;

        [Category("Regeneration")]
        [DisplayName("Eat Start %")]
        public double EatStartPercent { get; set; } = 65.0;

        [Category("Regeneration")]
        [DisplayName("Eat Stop %")]
        public double EatUntilPercent { get; set; } = 85.0;

        [Category("General")]
        [DisplayName("Event Pull (ms)")]
        public int EventPullMs { get; set; } = 500;

        [Category("Consumables")]
        [DisplayName("Health Potion Threshold %")]
        public int HealthPotionThreshold { get; set; } = 40;

        [Category("Consumables")]
        [DisplayName("Mana Potion Threshold %")]
        public int ManaPotionThreshold { get; set; } = 20;

        [Category("Party")]
        [DisplayName("Follow Leader")]
        public bool FollowGroupLeader { get; set; } = false;

        [Category("Party")]
        [DisplayName("Follow Members")]
        public bool FollowGroupMembers { get; set; } = false;

        [Category("Party")]
        [DisplayName("Dynamic Positioning")]
        public bool FollowPositionDynamic { get; set; } = false;

        [Category("Party")]
        [DisplayName("Follow Specific Character")]
        public bool FollowSpecificCharacter { get; set; } = false;

        [Category("Social")]
        [DisplayName("Friends List")]
        public string Friends { get; set; } = string.Empty;

        [Category("General")]
        [DisplayName("Ghost Res Distance")]
        public float GhostResurrectThreshold { get; set; } = 24.0f;

        [Category("Profiles")]
        [DisplayName("Grinding Profile")]
        public string GrindingProfile { get; set; } = string.Empty;

        // UseCustomPortrait is now implicit - derived from whether CustomPortraitPath has a value
        [JsonIgnore]
        [Browsable(false)]
        public bool UseCustomPortrait => !string.IsNullOrWhiteSpace(CustomPortraitPath);

        [Category("Profiles")]
        [DisplayName("Custom Portrait")]
        [Description("Custom portrait image for this profile. If set, auto-capture is disabled.")]
        public string CustomPortraitPath { get; set; } = string.Empty;

        [Browsable(false)]
        public Dictionary<string, Keybind> Hotkeys { get; set; } = [];

        [Category("General")]
        [DisplayName("Enable Idle Actions")]
        public bool IdleActions { get; set; } = false;

        [Browsable(false)]
        public Dictionary<string, bool> IdleActionsEnabled { get; set; } = [];

        [Category("Combat")]
        [DisplayName("Ignore Combat (Mounted)")]
        public bool IgnoreCombatWhileMounted { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Repair Threshold %")]
        public double ItemRepairThreshold { get; set; } = 25.0;

        [Category("Inventory")]
        [DisplayName("Sell Blacklist")]
        public List<string> ItemSellBlacklist { get; set; } = [];

        [Category("Productivity")]
        [DisplayName("Mail Header")]
        public string JobEngineMailHeader { get; set; } = string.Empty;

        [Category("Productivity")]
        [DisplayName("Mail Receiver")]
        public string JobEngineMailReceiver { get; set; } = string.Empty;

        [Category("Productivity")]
        [DisplayName("Mail Text")]
        public string JobEngineMailText { get; set; } = string.Empty;

        [Category("Profiles")]
        [DisplayName("Job Profile")]
        public string JobProfile { get; set; } = string.Empty;

        [Category("Looting")]
        [DisplayName("Loot Money/Quest Only")]
        public bool LootOnlyMoneyAndQuestitems { get; set; } = false;

        [Category("Looting")]
        [DisplayName("Loot Units")]
        public bool LootUnits { get; set; } = true;

        [Category("Looting")]
        [DisplayName("Loot Radius")]
        public float LootUnitsRadius { get; set; } = 20.0f;

        [Category("Map")]
        [DisplayName("Render Path")]
        public bool MapRenderCurrentPath { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Dungeon Nodes")]
        public bool MapRenderDungeonNodes { get; set; } = false;

        [Category("Map")]
        [DisplayName("Render Herbs")]
        public bool MapRenderHerbs { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Me")]
        public bool MapRenderMe { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Ores")]
        public bool MapRenderOres { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Quest POIs")]
        public bool MapRenderQuestPois { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Player Extra")]
        public bool MapRenderPlayerExtra { get; set; } = false;

        [Category("Map")]
        [DisplayName("Render Player Names")]
        public bool MapRenderPlayerNames { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Players")]
        public bool MapRenderPlayers { get; set; } = true;

        [Category("Map")]
        [DisplayName("Render Unit Extra")]
        public bool MapRenderUnitExtra { get; set; } = false;

        [Category("Map")]
        [DisplayName("Render Unit Names")]
        public bool MapRenderUnitNames { get; set; } = false;

        [Category("Map")]
        [DisplayName("Render Units")]
        public bool MapRenderUnits { get; set; } = true;

        [Category("Party")]
        [DisplayName("Max Follow Distance")]
        public int MaxFollowDistance { get; set; } = 100;

        [Category("Performance")]
        [DisplayName("Max FPS")]
        public int MaxFps { get; set; } = 60;

        [Category("Performance")]
        [DisplayName("Max FPS (Combat)")]
        public int MaxFpsCombat { get; set; } = 60;

        [Category("General")]
        [DisplayName("Merchant Search Radius")]
        public float MerchantNpcSearchRadius { get; set; } = 50.0f;

        [Category("Party")]
        [DisplayName("Min Follow Distance")]
        public int MinFollowDistance { get; set; } = 5;

        [Category("Mounts")]
        [DisplayName("Mount Names")]
        [Description("Comma-separated list of specific mount names to use.")]
        public string Mounts { get; set; } = string.Empty;

        [Category("Movement")]
        [DisplayName("Movement Settings")]
        public MovementSettings MovementSettings { get; set; } = new();

        [Category("AI")]
        [DisplayName("AI Settings")]
        public AiSettings AiSettings { get; set; } = new();

        [Category("Remote Control")]
        [DisplayName("Server Port")]
        public int NameshServerPort { get; set; } = 47110;

        [Category("Remote Control")]
        [DisplayName("Server IP")]
        public string NavmeshServerIp { get; set; } = "127.0.0.1";

        [Category("Social")]
        [DisplayName("Friends Mode Only")]
        public bool OnlyFriendsMode { get; set; } = false;

        [Category("Social")]
        [DisplayName("Support Master Only")]
        public bool OnlySupportMaster { get; set; } = false;

        [Category("Login")]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        public string Password { get; set; } = string.Empty;

        [JsonIgnore()]
        [Browsable(false)]
        public string Path { get; set; } = string.Empty;

        [Category("Execution")]
        [DisplayName("WoW Executable Path")]
        [Description("The absolute path to Wow.exe.")]
        public string PathToWowExe { get; set; } = string.Empty;

        [Category("General")]
        [DisplayName("Cache Names")]
        public bool PermanentNameCache { get; set; } = true;

        [Category("General")]
        [DisplayName("Cache Reactions")]
        public bool PermanentReactionCache { get; set; } = true;

        [Category("Profiles")]
        [DisplayName("Quest Profile")]
        public string QuestProfile { get; set; } = string.Empty;

        [Category("Remote Control")]
        [DisplayName("RCON Enabled")]
        public bool RconEnabled { get; set; } = false;

        [Category("Remote Control")]
        [DisplayName("RCON Interval (ms)")]
        public int RconInterval { get; set; } = 5000;

        [Category("Remote Control")]
        [DisplayName("RCON Send Screenshots")]
        public bool RconSendScreenshots { get; set; } = false;

        [Category("Remote Control")]
        [DisplayName("RCON Address")]
        public string RconServerAddress { get; set; } = "https://localhost:47111";

        [Category("Remote Control")]
        [DisplayName("RCON GUID")]
        public string RconServerGuid { get; set; } = Guid.NewGuid().ToString();

        [Category("Remote Control")]
        [DisplayName("RCON Image")]
        public string RconServerImage { get; set; } = string.Empty;

        [Category("Remote Control")]
        [DisplayName("RCON Tick (ms)")]
        public double RconTickMs { get; set; } = 1000;

        [Category("Login")]
        [DisplayName("Realm Name")]
        public string Realm { get; set; } = "AmeisenRealm";

        [Category("Login")]
        [DisplayName("Realmlist")]
        public string Realmlist { get; set; } = "127.0.0.1";

        [Category("General")]
        [DisplayName("Auto Release Spirit")]
        public bool ReleaseSpirit { get; set; } = false;

        [Category("General")]
        [DisplayName("Repair Search Radius")]
        public float RepairNpcSearchRadius { get; set; } = 50.0f;

        [Category("Looting")]
        [DisplayName("Roll Greed")]
        public bool RollGreedOnItems { get; set; } = true;

        [Category("General")]
        [DisplayName("Save Bot Position")]
        public bool SaveBotWindowPosition { get; set; } = false;

        [Category("Execution")]
        [DisplayName("Save WoW Position")]
        public bool SaveWowWindowPosition { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Sell Blue Items")]
        public bool SellBlueItems { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Sell Gray Items")]
        public bool SellGrayItems { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Sell Green Items")]
        public bool SellGreenItems { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Sell Purple Items")]
        public bool SellPurpleItems { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Sell White Items")]
        public bool SellWhiteItems { get; set; } = false;

        [Category("Party")]
        [DisplayName("Follow Target")]
        public string SpecificCharacterToFollow { get; set; } = string.Empty;

        [Category("Performance")]
        [DisplayName("Tick Rate (ms)")]
        public int StateMachineTickMs { get; set; } = 20;

        [Category("Combat")]
        [DisplayName("Stick to Group (Combat)")]
        public bool StayCloseToGroupInCombat { get; set; } = false;

        [Category("Combat")]
        [DisplayName("Support Range")]
        public float SupportRange { get; set; } = 64.0f;

        [Category("General")]
        [DisplayName("Auto Train Spells")]
        public bool TrainSpells { get; set; } = false;

        [Category("Combat")]
        [DisplayName("Use Built-in CC")]
        public bool UseBuiltInCombatClass { get; set; } = true;

        [Category("Combat")]
        [DisplayName("Use First Aid")]
        [Description("If true, the bot will use bandages to heal out of combat.")]
        [DefaultValue(true)]
        public bool UseFirstAid { get; set; } = true;

        [Category("Mounts")]
        [DisplayName("Use Mounts")]
        public bool UseMounts { get; set; } = true;

        [Category("Mounts")]
        [DisplayName("Use Mounts (Party)")]
        public bool UseMountsInParty { get; set; } = true;

        [Category("Mounts")]
        [DisplayName("Use Specific Mounts")]
        public bool UseOnlySpecificMounts { get; set; } = false;

        [Category("Login")]
        [DisplayName("Username")]
        public string Username { get; set; } = string.Empty;

        [Browsable(false)]
        public Rect WowWindowRect { get; set; } = new Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 };
    }
}
