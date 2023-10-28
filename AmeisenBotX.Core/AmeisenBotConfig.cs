using AmeisenBotX.Common.Keyboard.Objects;
using AmeisenBotX.Core.Engines.Movement.Settings;
using AmeisenBotX.Memory.Win32;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// summary>
/// Gets or sets the flag indicating whether area of effect detection should be avoided.
/// </summary>
namespace AmeisenBotX.Core
{
    /// <summary>
    /// Represents the configuration settings for the AmeisenBot.
    /// </summary>
    public class AmeisenBotConfig
    {
        /// <summary>
        /// Gets or sets the duration in milliseconds for anti-AFK functionality.
        /// </summary>
        public int AntiAfkMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the flag indicating whether area of effect detection should be avoided.
        /// </summary>
        public bool AoeDetectionAvoid { get; set; } = false;

        /// <summary>
        /// Gets or sets the extent of the area of effect (AOE) detection, default value is 1.0f.
        /// </summary>
        public float AoeDetectionExtends { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets a value indicating whether area-of-effect detection includes players.
        /// </summary>
        public bool AoeDetectionIncludePlayers { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether quests are automatically accepted.
        /// </summary>
        public bool AutoAcceptQuests { get; set; } = true;

        ///<summary>Gets or sets a value indicating whether the realmlist should be automatically changed.</summary>
        public bool AutoChangeRealmlist { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Wow feature should automatically close.
        /// </summary>
        public bool AutocloseWow { get; set; } = true;

        ///<summary>
        ///Gets or sets a value indicating whether the trash is automatically destroyed.
        ///</summary>
        public bool AutoDestroyTrash { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether automatic rendering is disabled.
        /// The default value is false.
        /// </summary>
        public bool AutoDisableRender { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the auto join background is enabled.
        /// </summary>
        public bool AutojoinBg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player will automatically join Looking for Group (LFG) interactions. 
        /// The default value is true.
        /// </summary>
        public bool AutojoinLfg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether auto login is enabled.
        /// </summary>
        public bool AutoLogin { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot is enabled or disabled.
        /// </summary>
        public bool Autopilot { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether auto position for "Wow" is enabled.
        /// </summary>
        public bool AutoPositionWow { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the auto repair mode is enabled.
        /// </summary>
        public bool AutoRepair { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the auto-selling feature is enabled.
        /// </summary>
        public bool AutoSell { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the low graphics settings are automatically set.
        /// </summary>
        public bool AutoSetUlowGfxSettings { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Wow application should be automatically started.
        /// </summary>
        public bool AutostartWow { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the character should automatically initiate conversations with nearby quest givers.
        /// </summary>
        public bool AutoTalkToNearQuestgivers { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of bag slots to go sell.
        /// </summary>
        public int BagSlotsToGoSell { get; set; } = 4;

        /// <summary>
        /// Gets or sets the BattlegroundEngine property, which represents the engine used for the battleground.
        /// </summary>
        public string BattlegroundEngine { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a boolean value indicating whether the battleground should use party mode.
        /// </summary>
        public bool BattlegroundUsePartyMode { get; set; } = false;

        /// <summary>
        /// Gets or sets the rectangle representing the boundaries of the bot window.
        /// </summary>
        public Rect BotWindowRect { get; set; } = new Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 };

        /// <summary>
        /// Gets or sets the built-in combat class name.
        /// </summary>
        public string BuiltInCombatClassName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to cache points of interest.
        /// </summary>
        public bool CachePointsOfInterest { get; set; } = true;

        /// <summary>
        /// Gets or sets the character slot.
        /// </summary>
        public int CharacterSlot { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether chat protocols are enabled or not.
        /// </summary>
        public bool ChatProtocols { get; set; } = false;

        /// Gets or sets the list of custom combat class dependencies.
        public List<string> CustomCombatClassDependencies { get; set; } = new();

        /// <summary>
        /// Gets or sets the file path for the custom combat class.
        /// </summary>
        public string CustomCombatClassFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start percentage for drinking.
        /// </summary>
        public double DrinkStartPercent { get; set; } = 65.0;

        /// <summary>
        /// Gets or sets the drink until percent, which represents the maximum percentage of drink consumption.
        /// </summary>
        public double DrinkUntilPercent { get; set; } = 85.0;

        /// <summary>
        /// Gets or sets a value indicating whether the dungeon is using party mode. 
        /// Default value is false.
        /// </summary>
        public bool DungeonUsePartyMode { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user should eat, drink, abort, or follow the party.
        /// </summary>
        public bool EatDrinkAbortFollowParty { get; set; } = true;

        /// <summary>
        /// Gets or sets the distance at which the character will eat, drink, abort, or follow a party.
        /// </summary>
        public float EatDrinkAbortFollowPartyDistance { get; set; } = 25.0f;

        /// <summary>
        /// Gets or sets the starting percentage for eating.
        /// </summary>
        public double EatStartPercent { get; set; } = 65.0;

        /// <summary>
        /// Gets or sets the percent value until which the item can be eaten.
        /// </summary>
        public double EatUntilPercent { get; set; } = 85.0;

        /// <summary>
        /// Gets or sets the interval, in milliseconds, for pulling events.
        /// </summary>
        public int EventPullMs { get; set; } = 500;

        /// <summary>
        /// Gets or sets a value indicating whether the object should follow the group leader.
        /// </summary>
        public bool FollowGroupLeader { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to follow group members.
        /// </summary>
        public bool FollowGroupMembers { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the position should be dynamically followed.
        /// </summary>
        public bool FollowPositionDynamic { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the code should follow a specific character.
        /// </summary>
        public bool FollowSpecificCharacter { get; set; } = false;

        /// <summary>
        /// Gets or sets the friend list.
        /// </summary>
        public string Friends { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the threshold value for resurrecting a ghost, 
        /// with a default value of 24.0f.
        /// </summary>
        public float GhostResurrectThreshold { get; set; } = 24.0f;

        /// <summary>
        /// Gets or sets the grinding profile string value.
        /// </summary>
        public string GrindingProfile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of hotkeys with their corresponding string keys.
        /// </summary>
        public Dictionary<string, Keybind> Hotkeys { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether idle actions are enabled.
        /// </summary>
        public bool IdleActions { get; set; } = false;

        /// <summary>
        /// Gets or sets a dictionary of idle actions and their corresponding enabled status.
        /// </summary>
        public Dictionary<string, bool> IdleActionsEnabled { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether combat should be ignored while the character is mounted.
        /// </summary>
        public bool IgnoreCombatWhileMounted { get; set; } = true;

        /// <summary>
        /// Gets or sets the threshold value for item repair.
        /// </summary>
        /// <value>The threshold value for item repair.</value>
        public double ItemRepairThreshold { get; set; } = 25.0;

        /// <summary>
        /// Gets or sets the list of items that are blacklisted for selling.
        /// </summary>
        public List<string> ItemSellBlacklist { get; set; }

        /// <summary>
        /// Gets or sets the mail header for the job engine.
        /// </summary>
        public string JobEngineMailHeader { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email receiver for the job engine.
        /// </summary>
        public string JobEngineMailReceiver { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text of the email for the job engine.
        /// </summary>
        public string JobEngineMailText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the job profile.
        /// </summary>
        /// <value>The job profile.</value>
        public string JobProfile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the loot should only consist of money and quest items.
        /// </summary>
        public bool LootOnlyMoneyAndQuestitems { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the units should be looted or not.
        /// </summary>
        public bool LootUnits { get; set; } = true;

        /// <summary>
        /// Gets or sets the radius within which units can be looted.
        /// </summary>
        public float LootUnitsRadius { get; set; } = 20.0f;

        /// <summary>
        /// Gets or sets a boolean value indicating whether the current path should be rendered on the map.
        /// </summary>
        public bool MapRenderCurrentPath { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether dungeon nodes should be rendered on the map.
        /// </summary>
        public bool MapRenderDungeonNodes { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the herbs on the map should be rendered.
        /// Default value is true.
        /// </summary>
        public bool MapRenderHerbs { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Map should be rendered or not.
        /// </summary>
        public bool MapRenderMe { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the map should render ores.
        /// </summary>
        public bool MapRenderOres { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean value indicating whether to render extra details for the player on the map.
        /// </summary>
        public bool MapRenderPlayerExtra { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether player names are rendered on the map.
        /// </summary>
        public bool MapRenderPlayerNames { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the players should be rendered on the map.
        /// </summary>
        public bool MapRenderPlayers { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether an extra render unit is mapped.
        /// </summary>
        public bool MapRenderUnitExtra { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the unit names should be rendered on the map.
        /// </summary>
        public bool MapRenderUnitNames { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to render map units.
        /// </summary>
        public bool MapRenderUnits { get; set; } = true;

        ///<summary>Sets or gets the maximum follow distance.</summary>
        public int MaxFollowDistance { get; set; } = 100;

        /// <summary>
        /// Gets or sets the maximum frames per second.
        /// </summary>
        public int MaxFps { get; set; } = 60;

        /// <summary>
        /// Gets or sets the maximum frames per second for combat.
        /// </summary>
        public int MaxFpsCombat { get; set; } = 60;

        /// <summary>
        /// Gets or sets the radius used for searching merchant NPCs.
        /// </summary>
        public float MerchantNpcSearchRadius { get; set; } = 50.0f;

        /// <summary>
        /// Gets or sets the minimum follow distance.
        /// </summary>
        public int MinFollowDistance { get; set; } = 6;

        /// <summary>
        /// Gets or sets the Mounts property as a string. Default value is an empty string.
        /// </summary>
        public string Mounts { get; set; } = string.Empty;

        /// Represents the movement settings for an object.
        public MovementSettings MovementSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets the port number for the Namesh server. The default value is 47110.
        /// </summary>
        public int NameshServerPort { get; set; } = 47110;

        /// <summary>
        /// Gets or sets the IP address of the Navmesh Server.
        /// </summary>
        public string NavmeshServerIp { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets a value indicating whether the only friends mode is enabled.
        /// </summary>
        public bool OnlyFriendsMode { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether only the master is supported.
        /// </summary>
        public bool OnlySupportMaster { get; set; } = false;

        /// <summary>
        /// Gets or sets the password as a string.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Ignores the property during JSON serialization.
        /// </summary>
        [JsonIgnore()]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the World of Warcraft executable.
        /// </summary>
        public string PathToWowExe { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the name cache is permanent.
        /// </summary>
        public bool PermanentNameCache { get; set; } = true;

        ///<summary>
        ///Gets or sets a value indicating whether the permanent reaction cache is enabled.
        ///</summary>
        public bool PermanentReactionCache { get; set; } = true;

        /// <summary>
        /// Gets or sets the quest profile.
        /// </summary>
        public string QuestProfile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether RCON is enabled.
        /// </summary>
        public bool RconEnabled { get; set; } = false;

        /// <summary>
        /// Represents the interval, in milliseconds, for the RCON (Remote Console) communication.
        /// </summary>
        public int RconInterval { get; set; } = 5000;

        /// <summary>
        /// Gets or sets a value indicating whether the RconSendScreenshots property is enabled or disabled. By default, it is set to false.
        /// </summary>
        public bool RconSendScreenshots { get; set; } = false;

        /// <summary>
        /// Gets or sets the RCON server address.
        /// </summary>
        /// <value>The RCON server address as a string.</value>
        public string RconServerAddress { get; set; } = "https://localhost:47111";

        /// Represents the globally unique identifier (GUID) of the RCON server.
        public string RconServerGuid { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the image of the RCON server, defaulting to an empty string.
        /// </summary>
        public string RconServerImage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tick in milliseconds for the Rcon.
        /// </summary>
        public double RconTickMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the realm for the object. Default value is "AmeisenRealm".
        /// </summary>
        public string Realm { get; set; } = "AmeisenRealm";

        ///<summary>
        ///Gets or sets the Realmlist for the server connection.
        ///The default value is "127.0.0.1".
        ///</summary>
        public string Realmlist { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets a value indicating whether to release the spirit.
        /// </summary>
        public bool ReleaseSpirit { get; set; } = false;

        /// <summary>
        /// Gets or sets the radius within which the NPC can search for repair options.
        /// </summary>
        public float RepairNpcSearchRadius { get; set; } = 50.0f;

        /// <summary>
        /// Gets or sets a value indicating whether the player should roll greed on items.
        /// </summary>
        public bool RollGreedOnItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the bot window position is saved.
        /// </summary>
        public bool SaveBotWindowPosition { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to save the position of the Wow window.
        /// </summary>
        public bool SaveWowWindowPosition { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether blue items can be sold.
        /// </summary>
        public bool SellBlueItems { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether gray items are being sold.
        /// </summary>
        public bool SellGrayItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean value indicating whether green items can be sold.
        /// </summary>
        public bool SellGreenItems { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether selling of purple items is enabled. Default value is false.
        /// </summary>
        public bool SellPurpleItems { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to sell white items.
        /// </summary>
        public bool SellWhiteItems { get; set; } = false;

        /// <summary>
        /// Gets or sets the specific character to be followed.
        /// </summary>
        public string SpecificCharacterToFollow { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time in milliseconds for each tick of the state machine.
        /// The default value is 20 milliseconds.
        /// </summary>
        public int StateMachineTickMs { get; set; } = 20;

        /// <summary>
        /// Gets or sets a value indicating if the player should stay close to the group during combat.
        /// </summary>
        /// <value>True if the player should stay close to the group, false otherwise.</value>
        public bool StayCloseToGroupInCombat { get; set; } = false;

        /// <summary>
        /// Gets or sets the support range, measured in units. The default value is 64.0f.
        /// </summary>
        public float SupportRange { get; set; } = 64.0f;

        /// <summary>
        /// Gets or sets a value indicating whether the spells should be trained.
        /// </summary>
        public bool TrainSpells { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use the built-in combat class.
        /// </summary>
        public bool UseBuiltInCombatClass { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether mounts are used.
        /// </summary>
        public bool UseMounts { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether mounts are used in the party.
        /// </summary>
        public bool UseMountsInParty { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether only specific mounts should be used.
        /// </summary>
        public bool UseOnlySpecificMounts { get; set; } = false;

        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rectangle representing the window dimensions.
        /// </summary>
        public Rect WowWindowRect { get; set; } = new Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 };
    }
}