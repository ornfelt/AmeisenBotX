using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Movement.Providers.Basic;
using AmeisenBotX.Core.Engines.Movement.Providers.Special;
using AmeisenBotX.Core.Logic.Enums;
using AmeisenBotX.Core.Logic.Leafs;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Core.Logic.StaticDeathRoutes;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Memory.Win32;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Shared.Lua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AmeisenBotX.Core.Logic
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmeisenBotLogic"/> class with the provided config and interfaces.
    /// </summary>
    public class AmeisenBotLogic : IAmeisenBotLogic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticDeathRoutes"/> class and adds the <see cref="ForgeOfSoulsDeathRoute"/> and <see cref="PitOfSaronDeathRoute"/> routes.
        /// </summary>
        private readonly List<IStaticDeathRoute> StaticDeathRoutes = new()
        {
            new ForgeOfSoulsDeathRoute(),
            new PitOfSaronDeathRoute()
        };

        ///<summary>
        ///Constructor for the AmeisenBotLogic class.
        ///</summary>
        public AmeisenBotLogic(AmeisenBotConfig config, AmeisenBotInterfaces bot)
        {
            Config = config;
            Bot = bot;

            FirstStart = true;
            FirstLogin = true;
            Random = new();

            Mode = BotMode.None;

            AntiAfkEvent = new(TimeSpan.FromMilliseconds(1200));
            CharacterUpdateEvent = new(TimeSpan.FromMilliseconds(5000));
            EatBlockEvent = new(TimeSpan.FromMilliseconds(30000));
            EatEvent = new(TimeSpan.FromMilliseconds(250));
            IdleActionEvent = new(TimeSpan.FromMilliseconds(1000));
            LoginAttemptEvent = new(TimeSpan.FromMilliseconds(500));
            LootTryEvent = new(TimeSpan.FromMilliseconds(750));
            PartymembersFightEvent = new(TimeSpan.FromMilliseconds(1000));
            RenderSwitchEvent = new(TimeSpan.FromMilliseconds(1000));
            UnitsLootedCleanupEvent = new(TimeSpan.FromMilliseconds(1000));
            UpdateFood = new(TimeSpan.FromMilliseconds(1000));

            UnitsLooted = new();
            UnitsToLoot = new();

            MovementManager = new
            (
                new List<IMovementProvider>()
                {
                    new DungeonMovementProvider(bot),
                    new SimpleCombatMovementProvider(bot),
                    new FollowMovementProvider(bot, config),
                }
            );

            // OPEN WORLD -----------------------------

            INode openworldGhostNode = new Selector
            (
                () => CanUseStaticPaths(),
                // prefer static paths
                new SuccessLeaf(() => Bot.Movement.DirectMove(StaticRoute.GetNextPoint(Bot.Player.Position))),
                // run to corpse by position
                new Leaf(RunToCorpseAndRetrieveIt)
            );

            INode combatNode = new Selector
            (
                () => Bot.CombatClass == null,
                // start autoattacking if we have no combat class loaded
                new Selector
                (
                    () => Bot.Target == null,
                    new SuccessLeaf(() => Bot.Wow.StartAutoAttack()),
                    new Selector
                    (
                        () => !Bot.Player.IsInMeleeRange(Bot.Target),
                        new Leaf(() => MoveToPosition(Bot.Target.Position)),
                        new Selector
                        (
                            () => !BotMath.IsFacing(Bot.Player.Position, Bot.Player.Rotation, Bot.Target.Position),
                            new SuccessLeaf(() => Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, Bot.Target.Position)),
                            new Selector
                            (
                                () => !Bot.Player.IsAutoAttacking,
                                new SuccessLeaf(() => { Bot.Wow.StartAutoAttack(); /*Bot.Wow.StopClickToMove();*/ }),
                                new SuccessLeaf()
                            )
                        )
                    )
                ),
                // TODO: handle tactics here run combat class logic
                new SuccessLeaf(() => Bot.CombatClass.Execute())
            );

            INode interactWithMerchantNode = new InteractWithUnitLeaf(Bot, () => Merchant, new SuccessLeaf(() => SpeakToMerchantRoutine.Run(Bot, Merchant)));
            INode interactWithClassTrainerNode = new InteractWithUnitLeaf(Bot, () => ClassTrainer, new SuccessLeaf(() => SpeakToClassTrainerRoutine.Run(Bot, ClassTrainer)));
            INode interactWithProfessionTrainerNode = new InteractWithUnitLeaf(Bot, () => ProfessionTrainer, new SuccessLeaf(() => SpeakToClassTrainerRoutine.Run(Bot, ProfessionTrainer)));

            INode jobsNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Jobs.Execute()),
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (() => !Bot.Player.IsMounted && NeedToFight(), combatNode),
                (NeedToRepairOrSell, interactWithMerchantNode),
                // (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat))
            );

            INode grindingNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Grinding.Execute()),
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (NeedToFight, combatNode),
                (NeedToRepairOrSell, interactWithMerchantNode),
                (NeedToTrainSpells, interactWithClassTrainerNode),
                (NeedToTrainSecondarySkills, interactWithProfessionTrainerNode),
                (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat))
            );

            INode questingNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Quest.Execute()),
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (NeedToFight, combatNode),
                (NeedToRepairOrSell, interactWithMerchantNode),
                (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat))
            );

            INode pvpNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Pvp.Execute()),
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (NeedToFight, combatNode),
                (NeedToRepairOrSell, interactWithMerchantNode),
                (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat))
            );

            INode testingNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Test.Execute()),
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (() => Bot.Player.IsGhost, openworldGhostNode)
            );

            INode openworldNode = new Waterfall
            (
                // do idle stuff as fallback
                new SuccessLeaf(() => Bot.CombatClass?.OutOfCombatExecute()),
                // handle main open world states
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (NeedToFight, combatNode),
                (NeedToRepairOrSell, interactWithMerchantNode),
                (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat)),
                (NeedToTalkToQuestgiver, new InteractWithUnitLeaf(Bot, () => QuestGiverToTalkTo)),
                (() => Config.IdleActions && IdleActionEvent.Run(), new SuccessLeaf(() => Bot.IdleActions.Tick(Config.Autopilot)))
            );

            // SPECIAL ENVIRONMENTS -----------------------------

            INode battlegroundNode = new Waterfall
            (
                new SuccessLeaf(() => { Bot.Battleground.Execute(); }),
                // leave battleground once it is finished
                (IsBattlegroundFinished, new SuccessLeaf(() => { Bot.Wow.LeaveBattleground(); Bot.Battleground.Reset(); })),
                // only handle dead state here, ghost should only be a problem on AV as the
                // graveyard might get lost while we are a ghost
                (() => Bot.Player.IsDead, new Leaf(Dead)),
                (NeedToFight, combatNode),
                (NeedToEat, new Leaf(Eat))
            );

            INode dungeonNode = new Waterfall
            (
                new Selector
                (
                    () => Config.DungeonUsePartyMode,
                    // just follow when we use party mode in dungeon
                    openworldNode,
                    new SuccessLeaf(() => Bot.Dungeon.Execute())
                ),
                (() => Bot.Player.IsDead, new Leaf(DeadDungeon)),
                (
                    NeedToFight,
                    new Selector
                    (
                        NeedToFollowTactic,
                        new SuccessLeaf(),
                        combatNode
                    )
                ),
                (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat))
            );

            INode raidNode = new Waterfall
            (
                new Selector
                (
                    () => Config.DungeonUsePartyMode,
                    // just follow when we use party mode in raid
                    new Leaf(Move),
                    new SuccessLeaf(() => Bot.Dungeon.Execute())
                ),
                (
                    NeedToFight,
                    new Selector
                    (
                        NeedToFollowTactic,
                        new SuccessLeaf(),
                        combatNode
                    )
                ),
                (NeedToLoot, new Leaf(LootNearUnits)),
                (NeedToEat, new Leaf(Eat))
            );

            // GENERIC -----------------------------

            INode mainLogicNode = new Annotator
            (
                // run the update stuff before we execute the main logic objects will be updated
                // here for example
                new SuccessLeaf(() => Bot.Wow.Tick()),
                new Selector
                (
                    () => Bot.Objects.IsWorldLoaded && Bot.Player != null && Bot.Objects != null,
                    new Annotator
                    (
                        // update stuff that needs us to be ingame
                        new SuccessLeaf(UpdateIngame),
                        new Waterfall
                        (
                            // open world auto behavior as fallback
                            openworldNode,
                            // handle movement
                            (MovementManager.NeedToMove, new Leaf(Move)),
                            // handle special environments
                            (() => Bot.Objects.MapId.IsBattlegroundMap(), battlegroundNode),
                            (() => Bot.Objects.MapId.IsDungeonMap(), dungeonNode),
                            (() => Bot.Objects.MapId.IsRaidMap(), raidNode),
                            // handle open world modes
                            (() => Mode == BotMode.Grinding, grindingNode),
                            (() => Mode == BotMode.Jobs, jobsNode),
                            (() => Mode == BotMode.Questing, questingNode),
                            (() => Mode == BotMode.PvP, pvpNode),
                            (() => Mode == BotMode.Testing, testingNode)
                        )
                    ),
                    // we are most likely in the loading screen or player/objects are null
                    new SuccessLeaf(() =>
                    {
                        // make sure we dont run after we leave the loadingscreen
                        Bot.Movement.StopMovement();
                    })
                )
            );

            Tree = new
            (
                new Waterfall
                (
                    // run the anti afk and main logic if wow is running and we are logged in
                    new Annotator
                    (
                        new SuccessLeaf(AntiAfk),
                        mainLogicNode
                    ),
                    // accept tos and eula, start wow
                    (
                        () => Bot.Memory.Process == null || Bot.Memory.Process.HasExited,
                        new Sequence
                        (
                            new Leaf(CheckTosAndEula),
                            new Leaf(ChangeRealmlist),
                            new Leaf(StartWow)
                        )
                    ),
                    // setup interface and login
                    (() => !Bot.Wow.IsReady, new Leaf(SetupWowInterface)),
                    (NeedToLogin, new SuccessLeaf(Login))
                )
            );
        }

        /// <summary>
        /// Event triggered when World of Warcraft starts.
        /// </summary>
        public event Action OnWoWStarted;

        /// <summary>
        /// Gets or sets the mode of the bot.
        /// </summary>
        public BotMode Mode { get; private set; }

        /// <summary>
        /// Gets or sets the TimegatedEvent for the AntiAfkEvent.
        /// </summary>
        private TimegatedEvent AntiAfkEvent { get; }

        /// <summary>
        /// Gets or sets a value indicating whether any party members are currently in a fight.
        /// </summary>
        private bool ArePartymembersInFight { get; set; }

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces object representing the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent called CharacterUpdateEvent.
        /// </summary>
        private TimegatedEvent CharacterUpdateEvent { get; }

        /// <summary>
        /// Gets or sets the private property representing the ClassTrainer of the WowUnit.
        /// </summary>
        private IWowUnit ClassTrainer { get; set; }

        /// <summary>
        /// Gets the AmeisenBot configuration.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets the timestamp when the dungeon died.
        /// </summary>
        private DateTime DungeonDiedTimestamp { get; set; }

        /// <summary>
        /// Gets the timegated event for eating a block.
        /// </summary>
        private TimegatedEvent EatBlockEvent { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent called EatEvent.
        /// </summary>
        private TimegatedEvent EatEvent { get; }

        /// <summary>
        /// Gets or sets a value indicating whether it is the user's first login.
        /// </summary>
        private bool FirstLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is the first start.
        /// </summary>
        private bool FirstStart { get; set; }

        /// <summary>
        /// Gets or sets the collection of food items in the WoW inventory.
        /// </summary>
        /// <returns>The collection of food items.</returns>
        private IEnumerable<IWowInventoryItem> Food { get; set; }

        /// <summary>
        /// Gets the timegated event for idle action.
        /// </summary>
        private TimegatedEvent IdleActionEvent { get; }

        /// <summary>
        /// Gets or sets the DateTime object representing the time when the player started the game.
        /// </summary>
        private DateTime IngameSince { get; set; }

        /// <summary>
        /// Gets or sets the timegated event for login attempts.
        /// </summary>
        private TimegatedEvent LoginAttemptEvent { get; }

        /// <summary>
        /// Gets or sets the number of attempts made to loot items.
        /// </summary>
        private int LootTry { get; set; }

        /// <summary>
        /// Gets the private TimegatedEvent LootTryEvent.
        /// </summary>
        private TimegatedEvent LootTryEvent { get; }

        /// <summary>
        /// Gets or sets the merchant interface for interacting with the World of Warcraft unit.
        /// </summary>
        private IWowUnit Merchant { get; set; }

        ///<summary>
        ///Gets or sets the private instance of the MovementManager class.
        ///</summary>
        private MovementManager MovementManager { get; }

        /// <summary>
        /// Gets the TimegatedEvent representing a party members fight event.
        /// </summary>
        private TimegatedEvent PartymembersFightEvent { get; }

        /// <summary>
        /// Gets or sets the profession trainer for the WOW unit.
        /// </summary>
        private IWowUnit ProfessionTrainer { get; set; }

        /// <summary>
        /// Gets or sets the quest giver unit to talk to.
        /// </summary>
        private IWowUnit QuestGiverToTalkTo { get; set; }

        /// <summary>
        /// Gets the instance of the Random class.
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// Gets or sets the timegated event responsible for rendering the switch event.
        /// </summary>
        private TimegatedEvent RenderSwitchEvent { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the static routes have been searched.
        /// </summary>
        private bool SearchedStaticRoutes { get; set; }

        /// <summary>
        /// Gets or sets the static death route.
        /// </summary>
        private IStaticDeathRoute StaticRoute { get; set; }

        /// <summary>
        /// Gets the private Tree property.
        /// </summary>
        private Tree Tree { get; }

        /// <summary>
        /// Gets or sets the list of units looted, represented by their ulong values.
        /// </summary>
        private List<ulong> UnitsLooted { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent UnitsLootedCleanupEvent property.
        /// </summary>
        private TimegatedEvent UnitsLootedCleanupEvent { get; }

        /// <summary>
        /// Gets the queue of units to loot.
        /// </summary>
        private Queue<ulong> UnitsToLoot { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent for updating food.
        /// </summary>
        private TimegatedEvent UpdateFood { get; }

        /// <summary>
        /// Determines the appropriate trainer subtype for the given class.
        /// </summary>
        /// <param name="wowClass">The class for which to determine the trainer subtype.</param>
        /// <returns>The corresponding trainer subtype for the given class.</returns>
        public static NpcSubType DecideClassTrainer(WowClass wowClass)
        {
            return wowClass switch
            {
                WowClass.Warrior => NpcSubType.WarriorTrainer,
                WowClass.Paladin => NpcSubType.PaladinTrainer,
                WowClass.Hunter => NpcSubType.HunterTrainer,
                WowClass.Rogue => NpcSubType.RougeTrainer,
                WowClass.Priest => NpcSubType.PriestTrainer,
                WowClass.Deathknight => NpcSubType.DeathKnightTrainer,
                WowClass.Shaman => NpcSubType.ShamanTrainer,
                WowClass.Mage => NpcSubType.MageTrainer,
                WowClass.Warlock => NpcSubType.WarlockTrainer,
                WowClass.Druid => NpcSubType.DruidTrainer,
            };
        }

        /// <summary>
        /// Changes the mode of the bot and performs actions based on the new mode.
        /// </summary>
        /// <param name="mode">The new bot mode to be set.</param>
        public void ChangeMode(BotMode mode)
        {
            Mode = mode;

            switch (Mode)
            {
                case BotMode.Questing:
                    Bot.Quest.Enter();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Executes the Tick method of the Tree class.
        /// </summary>
        public void Tick()
        {
            Tree.Tick();
        }

        /// <summary>
        /// Method to prevent the player from going AFK.
        /// It checks if the AntiAfkEvent is currently running and if so, updates the TickCount in the Bot's Memory and sets the Timegate of the AntiAfkEvent to a random value between 300 and 2300 milliseconds.
        /// </summary>
        private void AntiAfk()
        {
            if (AntiAfkEvent.Run())
            {
                Bot.Memory.Write(Bot.Memory.Offsets.TickCount, Environment.TickCount);
                AntiAfkEvent.Timegate = TimeSpan.FromMilliseconds(Random.Next(300, 2300));
            }
        }

        /// <summary>
        /// This method searches for static death routes, this is needed when pathfinding cannot
        /// find a good route from the graveyard to th dungeon entry. For example the ICC dungeons
        /// are only reachable by flying, its easier to use static routes.
        /// </summary>
        /// <returns>True when a static path can be used, false if not</returns>
        private bool CanUseStaticPaths()
        {
            if (!SearchedStaticRoutes)
            {
                if (Bot.Memory.Read(Bot.Memory.Offsets.CorpsePosition, out Vector3 corpsePosition))
                {
                    SearchedStaticRoutes = true;

                    Vector3 endPosition = Bot.Dungeon.Profile != null ? Bot.Dungeon.Profile.WorldEntry : corpsePosition;
                    IStaticDeathRoute staticRoute = StaticDeathRoutes.FirstOrDefault(e => e.IsUseable(Bot.Objects.MapId, Bot.Player.Position, endPosition));

                    if (staticRoute != null)
                    {
                        StaticRoute = staticRoute;
                        StaticRoute.Init(Bot.Player.Position);
                    }
                    else
                    {
                        staticRoute = StaticDeathRoutes.FirstOrDefault(e => e.IsUseable(Bot.Objects.MapId, Bot.Player.Position, corpsePosition));

                        if (staticRoute != null)
                        {
                            StaticRoute = staticRoute;
                            StaticRoute.Init(Bot.Player.Position);
                        }
                    }
                }
            }

            return StaticRoute != null;
        }

        ///<summary>
        /// Changes the Realmlist in the config.wtf file.
        ///</summary>
        ///<returns>
        /// The status of the Realmlist change. Returns BtStatus.Success if successful, 
        /// BtStatus.Failed if an error occurred.
        ///</returns>
        private BtStatus ChangeRealmlist()
        {
            if (!Config.AutoChangeRealmlist)
            {
                return BtStatus.Success;
            }

            try
            {
                AmeisenLogger.I.Log("StartWow", "Changing Realmlist");
                string configWtfPath = Path.Combine(Directory.GetParent(Config.PathToWowExe).FullName, "wtf", "config.wtf");

                if (File.Exists(configWtfPath))
                {
                    bool editedFile = false;
                    List<string> content = File.ReadAllLines(configWtfPath).ToList();

                    if (!content.Any(e => e.Contains($"SET REALMLIST {Config.Realmlist}", StringComparison.OrdinalIgnoreCase)))
                    {
                        bool found = false;

                        for (int i = 0; i < content.Count; ++i)
                        {
                            if (content[i].Contains("SET REALMLIST", StringComparison.OrdinalIgnoreCase))
                            {
                                editedFile = true;
                                content[i] = $"SET REALMLIST {Config.Realmlist}";
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            editedFile = true;
                            content.Add($"SET REALMLIST {Config.Realmlist}");
                        }
                    }

                    if (editedFile)
                    {
                        File.SetAttributes(configWtfPath, FileAttributes.Normal);
                        File.WriteAllLines(configWtfPath, content);
                        File.SetAttributes(configWtfPath, FileAttributes.ReadOnly);
                    }
                }

                return BtStatus.Success;
            }
            catch
            {
                AmeisenLogger.I.Log("StartWow", "Cannot write realmlist to config.wtf");
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// This method is used to check and update the configuration file "config.wtf" 
        /// to ensure that the necessary settings (READEULA, READTOS, and MOVIE) are correctly set.
        /// If the file does not exist, it does not make any changes and returns a Failed status.
        /// If the file exists and any of the necessary settings are missing or incorrect,
        /// it updates the file with the correct settings and sets its attributes to read-only.
        /// After making the necessary changes, it returns a Success status.
        /// If any error occurs while checking or updating the file, it logs an error message and returns a Failed status.
        /// </summary>
        /// <returns>The status of the configuration check and update operation, either Success or Failed.</returns>
        private BtStatus CheckTosAndEula()
        {
            try
            {
                string configWtfPath = Path.Combine(Directory.GetParent(Config.PathToWowExe).FullName, "wtf", "config.wtf");

                if (File.Exists(configWtfPath))
                {
                    bool editedFile = false;
                    string content = File.ReadAllText(configWtfPath);

                    if (!content.Contains("SET READEULA \"0\"", StringComparison.OrdinalIgnoreCase))
                    {
                        editedFile = true;

                        if (content.Contains("SET READEULA", StringComparison.OrdinalIgnoreCase))
                        {
                            content = content.Replace("SET READEULA \"0\"", "SET READEULA \"1\"", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            content += "\nSET READEULA \"1\"";
                        }
                    }

                    if (!content.Contains("SET READTOS \"0\"", StringComparison.OrdinalIgnoreCase))
                    {
                        editedFile = true;

                        if (content.Contains("SET READTOS", StringComparison.OrdinalIgnoreCase))
                        {
                            content = content.Replace("SET READTOS \"0\"", "SET READTOS \"1\"", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            content += "\nSET READTOS \"1\"";
                        }
                    }

                    if (!content.Contains("SET MOVIE \"0\"", StringComparison.OrdinalIgnoreCase))
                    {
                        editedFile = true;

                        if (content.Contains("SET MOVIE", StringComparison.OrdinalIgnoreCase))
                        {
                            content = content.Replace("SET MOVIE \"0\"", "SET MOVIE \"1\"", StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            content += "\nSET MOVIE \"1\"";
                        }
                    }

                    if (editedFile)
                    {
                        File.SetAttributes(configWtfPath, FileAttributes.Normal);
                        File.WriteAllText(configWtfPath, content);
                        File.SetAttributes(configWtfPath, FileAttributes.ReadOnly);
                    }
                }

                return BtStatus.Success;
            }
            catch
            {
                AmeisenLogger.I.Log("StartWow", "Cannot write to config.wtf");
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// Checks if the character is dead and performs appropriate actions accordingly.
        /// If the character is dead, sets SearchedStaticRoutes to false and checks if the character is in a battleground map or if Config.ReleaseSpirit is true.
        /// If true, calls Bot.Wow.RepopMe() and returns BtStatus.Success, otherwise returns BtStatus.Ongoing.
        /// </summary>
        private BtStatus Dead()
        {
            SearchedStaticRoutes = false;

            if (Config.ReleaseSpirit || Bot.Objects.MapId.IsBattlegroundMap())
            {
                Bot.Wow.RepopMe();
                return BtStatus.Success;
            }

            return BtStatus.Ongoing;
        }

        /// <summary>
        /// Checks if the dungeon is dead.
        /// If no party members are in fight, it checks if the DungeonDiedTimestamp has been set.
        /// If not, it sets the DungeonDiedTimestamp to the current time.
        /// If the DungeonDiedTimestamp has been set and the time difference between the current time and the DungeonDiedTimestamp is greater than 30 seconds,
        /// it resurrects the bot, resets SearchedStaticRoutes, and returns BtStatus.Success.
        /// If there are party members alive that can resurrect the bot or the time difference between the current time and the DungeonDiedTimestamp is greater than 30 seconds,
        /// it resurrects the bot, resets SearchedStaticRoutes, and returns BtStatus.Success.
        /// Otherwise, it returns BtStatus.Ongoing.
        /// </summary>
        private BtStatus DeadDungeon()
        {
            if (!ArePartymembersInFight)
            {
                if (DungeonDiedTimestamp == default)
                {
                    DungeonDiedTimestamp = DateTime.UtcNow;
                }
                else if (DateTime.UtcNow - DungeonDiedTimestamp > TimeSpan.FromSeconds(30))
                {
                    Bot.Wow.RepopMe();
                    SearchedStaticRoutes = false;
                    return BtStatus.Success;
                }
            }

            if ((!ArePartymembersInFight && DateTime.UtcNow - DungeonDiedTimestamp > TimeSpan.FromSeconds(30))
                || Bot.Objects.Partymembers.Any(e => !e.IsDead
                    && (e.Class == WowClass.Paladin || e.Class == WowClass.Druid || e.Class == WowClass.Priest || e.Class == WowClass.Shaman)))
            {
                // if we died 30s ago or no one that can ress us is alive
                Bot.Wow.RepopMe();
                SearchedStaticRoutes = false;
                return BtStatus.Success;
            }

            return BtStatus.Ongoing;
        }

        /// <summary>
        /// Method to handle eating and drinking for the Bot. It checks if the Bot needs to eat or drink based on configured percentages, 
        /// and if so, determines whether to use food, refreshment, or water. It also checks if the Bot is already eating or drinking,
        /// and returns ongoing status if it is. Returns success status if no eating or drinking is needed.
        /// </summary>
        private BtStatus Eat()
        {
            if (EatEvent.Run())
            {
                bool needToEat = Bot.Player.HealthPercentage < Config.EatUntilPercent;
                bool needToDrink = Bot.Player.ManaPercentage < Config.DrinkUntilPercent;

                bool isEating = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Food");
                bool isDrinking = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Drink");

                if (isEating && isDrinking)
                {
                    return BtStatus.Ongoing;
                }

                IWowInventoryItem refreshment = Food.FirstOrDefault(e => Enum.IsDefined(typeof(WowRefreshment), e.Id));

                if (needToEat && needToDrink && refreshment != null)
                {
                    if (refreshment != null)
                    {
                        Bot.Wow.UseItemByName(refreshment.Name);
                        return BtStatus.Ongoing;
                    }
                }

                IWowInventoryItem food = Food.FirstOrDefault(e => Enum.IsDefined(typeof(WowFood), e.Id));

                if (!isEating && needToEat && (food != null || refreshment != null))
                {
                    // only use food if its not very lowlevel, otherwise try to use a refreshment
                    if (food != null && (refreshment == null || food.RequiredLevel >= Bot.Player.Level - 5))
                    {
                        Bot.Wow.UseItemByName(food.Name);
                        return BtStatus.Ongoing;
                    }

                    if (refreshment != null)
                    {
                        Bot.Wow.UseItemByName(refreshment.Name);
                        return BtStatus.Ongoing;
                    }
                }

                IWowInventoryItem water = Food.FirstOrDefault(e => Enum.IsDefined(typeof(WowWater), e.Id));

                if (!isDrinking && needToDrink && (water != null || refreshment != null))
                {
                    // only use water if its not very lowlevel, otherwise try to use a refreshment
                    if (water != null && (refreshment == null || water.RequiredLevel >= Bot.Player.Level - 5))
                    {
                        Bot.Wow.UseItemByName(water.Name);
                        return BtStatus.Ongoing;
                    }

                    if (refreshment != null)
                    {
                        Bot.Wow.UseItemByName(refreshment.Name);
                        return BtStatus.Ongoing;
                    }
                }
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Retrieves a collection of all lootable units within a certain radius of the player's position,
        /// excluding units that have already been looted by the bot.
        /// </summary>
        /// <returns>
        /// An IEnumerable object representing the lootable units.
        /// </returns>
        private IEnumerable<IWowUnit> GetLootableUnits()
        {
            return Bot.Objects.All.OfType<IWowUnit>()
                .Where(e => e.IsLootable
                    && !UnitsLooted.Contains(e.Guid)
                    && e.Position.GetDistance(Bot.Player.Position) < Config.LootUnitsRadius);
        }

        /// <summary>
        /// Checks if the battleground has finished.
        /// </summary>
        private bool IsBattlegroundFinished()
        {
            return Bot.Memory.Read(Bot.Memory.Offsets.BattlegroundFinished, out int bgFinished)
                && bgFinished == 1;
        }

        ///<summary>
        ///Checks if there is a repair NPC near the player.
        ///If a repair NPC is found, it is assigned to the 'unit' out parameter.
        ///Returns true if a repair NPC is found, false otherwise.
        ///</summary>
        private bool IsRepairNpcNear(out IWowUnit unit)
        {
            unit = Bot.Objects.All.OfType<IWowUnit>()
                    .FirstOrDefault(e => e.GetType() != typeof(IWowPlayer)
                                         && !e.IsDead
                                         && e.IsRepairer
                && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Hostile
                && Bot.Player.DistanceTo(e) <= Config.RepairNpcSearchRadius);

            return unit != null;
        }

        /// <summary>
        /// Determines if there is a vendor NPC near the player character and assigns the NPC object to the out parameter.
        /// </summary>
        private bool IsVendorNpcNear(out IWowUnit unit)
        {
            unit = Bot.Objects.All.OfType<IWowUnit>()
                .FirstOrDefault(e => e.GetType() != typeof(IWowPlayer)
                    && !e.IsDead
                    && e.IsVendor
                    && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Hostile
                    && e.Position.GetDistance(Bot.Player.Position) < Config.RepairNpcSearchRadius);

            return unit != null;
        }

        /// <summary>
        /// Loads the window position of the World of Warcraft window.
        /// </summary>
        private void LoadWowWindowPosition()
        {
            if (Config.SaveWowWindowPosition && !Config.AutoPositionWow)
            {
                if (Bot.Memory.Process.MainWindowHandle != IntPtr.Zero && Config.WowWindowRect != new Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 })
                {
                    Bot.Memory.SetWindowPosition(Bot.Memory.Process.MainWindowHandle, Config.WowWindowRect);
                    AmeisenLogger.I.Log("AmeisenBot", $"Loaded window position: {Config.WowWindowRect}", LogLevel.Verbose);
                }
                else
                {
                    AmeisenLogger.I.Log("AmeisenBot", $"Unable to load window position of {Bot.Memory.Process.MainWindowHandle} to {Config.WowWindowRect}", LogLevel.Warning);
                }
            }
        }

        /// <summary>
        /// Logs in the user to the game.
        /// </summary>
        private void Login()
        {
            Bot.Wow.SetWorldLoadedCheck(true);

            if (FirstLogin)
            {
                FirstLogin = true;
                SetCVars();
            }

            // needed to prevent direct logout due to inactivity
            AntiAfk();

            if (LoginAttemptEvent.Run())
            {
                Bot.Wow.LuaDoString(LuaLogin.Get(Config.Username, Config.Password, Config.Realm, Config.CharacterSlot));
            }

            Bot.Wow.SetWorldLoadedCheck(false);
        }

        /// <summary>
        /// Attempts to loot units near the bot.
        /// </summary>
        /// <returns>Returns the status of the looting operation.</returns>
        private BtStatus LootNearUnits()
        {
            IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(UnitsToLoot.Peek());

            if (unit == null || !unit.IsLootable || LootTry > 2)
            {
                UnitsLooted.Add(UnitsToLoot.Dequeue());
                LootTry = 0;
                return BtStatus.Failed;
            }

            if (unit.Position != Vector3.Zero && Bot.Player.DistanceTo(unit) > 3.0f)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, unit.Position);
                return BtStatus.Ongoing;
            }
            else if (LootTryEvent.Run())
            {
                if (Bot.Memory.Read(Bot.Memory.Offsets.LootWindowOpen, out byte lootOpen)
                    && lootOpen > 0)
                {
                    Bot.Wow.LootEverything();

                    UnitsLooted.Add(UnitsToLoot.Dequeue());
                    LootTry = 0;

                    Bot.Wow.ClickUiElement("LootCloseButton");
                    return BtStatus.Success;
                }
                else
                {
                    Bot.Wow.StopClickToMove();
                    Bot.Wow.InteractWithUnit(unit);
                    ++LootTry;
                }
            }

            return BtStatus.Ongoing;
        }

        /// <summary>
        /// Moves the object to the target position.
        /// </summary>
        /// <returns>The status of the movement.</returns>
        private BtStatus Move()
        {
            return MoveToPosition(MovementManager.Target);
        }

        /// <summary>
        /// Moves the bot to the specified position.
        /// </summary>
        /// <param name="position">The position to move to.</param>
        /// <param name="movementAction">The movement action to use (default is MovementAction.Move).</param>
        /// <returns>The status of the movement operation.</returns>
        private BtStatus MoveToPosition(Vector3 position, MovementAction movementAction = MovementAction.Move)
        {
            if (position != Vector3.Zero && Bot.Player.DistanceTo(position) > 3.0f)
            {
                Bot.Movement.SetMovementAction(movementAction, position);
                return BtStatus.Ongoing;
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Determines if the player needs to eat.
        /// </summary>
        /// <returns>True if the player needs to eat; otherwise, false.</returns>
        private bool NeedToEat()
        {
            // is eating blocked, used to prevent shredding of food
            if (!EatBlockEvent.Ready)
            {
                return false;
            }

            // when we are in a group an they move too far away, abort eating and dont start eating
            // for 30s
            if (Config.EatDrinkAbortFollowParty && Bot.Objects.PartymemberGuids.Any() && Bot.Player.DistanceTo(Bot.Objects.CenterPartyPosition) > Config.EatDrinkAbortFollowPartyDistance)
            {
                EatBlockEvent.Run();
                return false;
            }

            bool isEating = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Food");
            bool isDrinking = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Drink");

            // still eating/drinking, wait until threshold is reached
            if ((isEating && Bot.Player.HealthPercentage < Config.EatUntilPercent)
                || (isDrinking && Bot.Player.MaxMana > 0 && Bot.Player.ManaPercentage < Config.DrinkUntilPercent))
            {
                return true;
            }

            if (UpdateFood.Run())
            {
                Food = Bot.Character.Inventory.Items
                    .Where(e => e.RequiredLevel <= Bot.Player.Level)
                    .OrderByDescending(e => e.ItemLevel);
            }

            return Bot.Player.HealthPercentage < Config.EatUntilPercent
                   && (Food.Any(e => Enum.IsDefined(typeof(WowFood), e.Id))
                       || Food.Any(e => Enum.IsDefined(typeof(WowRefreshment), e.Id)))
                || Bot.Player.MaxMana > 0 && Bot.Player.ManaPercentage < Config.DrinkUntilPercent
                   && (Food.Any(e => Enum.IsDefined(typeof(WowWater), e.Id))
                       || Food.Any(e => Enum.IsDefined(typeof(WowRefreshment), e.Id)));
        }

        /// <summary>
        /// Checks if the player and party members need to engage in a fight.
        /// </summary>
        private bool NeedToFight()
        {
            if (PartymembersFightEvent.Run())
            {
                ArePartymembersInFight = Bot.Objects.Partymembers.Any(e => e.IsInCombat && e.DistanceTo(Bot.Player) < Config.SupportRange)
                    || Bot.Objects.All.OfType<IWowUnit>().Any(e => e.IsInCombat
                        && (e.IsTaggedByMe || !e.IsTaggedByOther)
                        && (e.TargetGuid == Bot.Player.Guid || Bot.Objects.Partymembers.Any(x => x.Guid == e.TargetGuid))
                        && Bot.Wow.GetReaction(Bot.Player.BaseAddress, e.BaseAddress) == WowUnitReaction.Hostile);
            }

            return Bot.Player.IsInCombat
                || ArePartymembersInFight;
        }

        /// <summary>
        /// Determines if the bot needs to follow a tactic or not.
        /// </summary>
        /// <returns>True if the bot needs to follow a tactic, otherwise false.</returns>
        private bool NeedToFollowTactic()
        {
            return Bot.Tactic.Execute() && !Bot.Tactic.AllowAttacking;
        }

        /// <summary>
        /// Determines if the user needs to login.
        /// </summary>
        private bool NeedToLogin()
        {
            return Bot.Memory.Read(Bot.Memory.Offsets.IsIngame, out int isIngame) && isIngame == 0;
        }

        /// <summary>
        /// Checks if there is a need to loot units.
        /// </summary>
        /// <returns>Returns true if there are units to loot, false otherwise.</returns>
        private bool NeedToLoot()
        {
            if (UnitsLootedCleanupEvent.Run())
            {
                UnitsLooted.RemoveAll((guid) =>
                {
                    // remove unit from looted list when its gone or seen alive
                    IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(guid);
                    return unit != null && !unit.IsDead;
                });
            }

            foreach (IWowUnit unit in GetLootableUnits())
            {
                if (!UnitsLooted.Contains(unit.Guid) && !UnitsToLoot.Contains(unit.Guid))
                {
                    UnitsToLoot.Enqueue(unit.Guid);
                }
            }

            return UnitsToLoot.Count > 0;
        }

        /// <summary>
        /// Checks whether the character needs to repair or sell items.
        /// </summary>
        /// <returns>
        /// Returns true if the character needs to repair or sell items; otherwise, false.
        /// </returns>
        private bool NeedToRepairOrSell()
        {
            bool needToRepair = Bot.Character.Equipment.Items.Any(e => e.Value.MaxDurability > 0 && e.Value.Durability / (double)e.Value.MaxDurability * 100.0 <= Config.ItemRepairThreshold);

            bool needToSell = Bot.Character.Inventory.FreeBagSlots < Config.BagSlotsToGoSell
                              && Bot.Character.Inventory.Items
                              .Any(e => e.Price > 0 && !Config.ItemSellBlacklist.Contains(e.Name)
                                      && ((Config.SellGrayItems && e.ItemQuality == (int)WowItemQuality.Poor)
                                      || (Config.SellWhiteItems && e.ItemQuality == (int)WowItemQuality.Common)
                                      || (Config.SellGreenItems && e.ItemQuality == (int)WowItemQuality.Uncommon)
                                      || (Config.SellBlueItems && e.ItemQuality == (int)WowItemQuality.Rare)
                                      || (Config.SellPurpleItems && e.ItemQuality == (int)WowItemQuality.Epic)));

            IWowUnit vendorRepair = null;
            IWowUnit vendorSell = null;

            if (Mode != BotMode.None && Bot.Grinding.Profile?.NpcsOfInterest == null)
            {
                return false;
            }

            switch (Mode)
            {
                case BotMode.Grinding:
                    {
                        Npc repairNpcEntry = Bot.Grinding.Profile.NpcsOfInterest.FirstOrDefault(e => e.Type == NpcType.VendorRepair);

                        if (repairNpcEntry != null)
                        {
                            vendorRepair = Bot.GetClosestVendorByEntryId(repairNpcEntry.EntryId);
                        }

                        Npc sellNpcEntry = Bot.Grinding.Profile.NpcsOfInterest.FirstOrDefault(e => e.Type is NpcType.VendorRepair or NpcType.VendorSellBuy);

                        if (sellNpcEntry != null)
                        {
                            vendorSell = Bot.GetClosestVendorByEntryId(sellNpcEntry.EntryId);
                        }

                        break;
                    }
                case BotMode.None:
                    IsRepairNpcNear(out IWowUnit repairNpc);
                    vendorRepair = repairNpc;

                    IsVendorNpcNear(out IWowUnit sellNpc);
                    vendorSell = sellNpc;
                    break;

                case BotMode.Questing:
                    break;

                case BotMode.PvP:
                    break;

                case BotMode.Testing:
                    break;

                case BotMode.Jobs:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (needToRepair && vendorRepair != null)
            {
                Merchant = vendorRepair;
                return true;
            }

            if (needToSell && vendorSell != null)
            {
                Merchant = vendorSell;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the player needs to talk to a quest giver.
        /// </summary>
        /// <returns>True if the player needs to talk to a quest giver; otherwise, false.</returns>
        private bool NeedToTalkToQuestgiver()
        {
            if (Config.AutoTalkToNearQuestgivers)
            {
                if (Bot.Objects.Partymembers.Any())
                {
                    List<ulong> guids = new();

                    if (Bot.Objects.Partyleader != null && Bot.Player.DistanceTo(Bot.Objects.Partyleader) < 6.0f)
                    {
                        guids.Add(Bot.Objects.Partyleader.TargetGuid);
                    }

                    foreach (ulong guid in guids)
                    {
                        if (Bot.TryGetWowObjectByGuid(guid, out IWowUnit unit)
                            && Bot.Player.DistanceTo(unit) < 5.6f
                            && unit.IsQuestgiver
                            && Bot.Db.GetReaction(Bot.Player, unit) != WowUnitReaction.Hostile)
                        {
                            QuestGiverToTalkTo = unit;
                            return true;
                        }
                    }
                }
            }

            QuestGiverToTalkTo = null;
            return false;
        }

        /// <summary>
        /// Checks if the character needs to train secondary skills.
        /// </summary>
        /// <returns>True if the character needs to train secondary skills, otherwise false.</returns>
        private bool NeedToTrainSecondarySkills()
        {
            IWowUnit professionTrainer = null;
            Npc profileTrainer = null;

            if (Bot.Grinding.Profile != null)
            {
                profileTrainer = Bot.Grinding.Profile.NpcsOfInterest?.FirstOrDefault(e =>
                    e.Type == NpcType.ProfessionTrainer);
            }

            if (profileTrainer != null)
            {
                professionTrainer = profileTrainer.SubType switch
                {
                    NpcSubType.FishingTrainer when !Bot.Character.Skills.ContainsKey("Fishing") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.FirstAidTrainer when !Bot.Character.Skills.ContainsKey("First Aid") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.CookingTrainer when !Bot.Character.Skills.ContainsKey("Cooking") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    _ => null
                };
            }

            if (professionTrainer == null)
            {
                return false;
            }

            ProfessionTrainer = professionTrainer;
            return ProfessionTrainer != null; // todo: Config.LearnSecondarySkills
        }

        /// <summary>
        /// Determines if the player needs to train spells.
        /// </summary>
        /// <returns>Returns true if the player needs to train spells, otherwise false.</returns>
        private bool NeedToTrainSpells()
        {
            IWowUnit classTrainer = null;
            Npc profileTrainer = null;

            if (Bot.Grinding.Profile != null)
            {
                profileTrainer = Bot.Grinding.Profile.NpcsOfInterest?.FirstOrDefault(e =>
                    e.Type == NpcType.ClassTrainer && e.SubType == DecideClassTrainer(Bot.Player.Class));
            }

            if (profileTrainer != null)
            {
                classTrainer = Bot.GetClosestTrainerByEntryId(profileTrainer.EntryId);
            }

            if (classTrainer == null)
            {
                return false;
            }

            ClassTrainer = classTrainer;
            return Bot.Character.LastLevelTrained != 0 && Bot.Character.LastLevelTrained < Bot.Player.Level;
        }

        /// <summary>
        /// Runs to the corpse position and retrieves it if the distance between the player's position and the corpse position is greater than the Ghost Resurrect Threshold. Returns Success if the corpse is successfully retrieved, Ongoing if the player is still moving towards the corpse, and Failed if the corpse position cannot be read.
        /// </summary>
        private BtStatus RunToCorpseAndRetrieveIt()
        {
            if (!Bot.Memory.Read(Bot.Memory.Offsets.CorpsePosition, out Vector3 corpsePosition))
            {
                return BtStatus.Failed;
            }

            if (Bot.Player.Position.GetDistance(corpsePosition) > Config.GhostResurrectThreshold)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, corpsePosition);
                return BtStatus.Ongoing;
            }

            Bot.Wow.RetrieveCorpse();
            return BtStatus.Success;
        }

        /// <summary>
        /// Sets the console variables (CVars) for the game.
        /// </summary>
        private void SetCVars()
        {
            List<(string, string)> cvars = new()
            {
                ("maxfps", $"{Config.MaxFps}"),
                ("maxfpsbk", $"{Config.MaxFps}"),
                ("AutoInteract", "1"),
                ("AutoLootDefault", "0"),
            };

            if (Config.AutoSetUlowGfxSettings)
            {
                cvars.AddRange(new (string, string)[]
                {
                    ("alphalevel", "1"),
                    ("anisotropic", "0"),
                    ("basemip", "1"),
                    ("bitdepth", "16"),
                    ("characterAmbient", "1"),
                    ("detaildensity", "1"),
                    ("detailDoodadAlpha", "0"),
                    ("doodadanim", "0"),
                    ("environmentDetail", "0.5"),
                    ("extshadowquality", "0"),
                    ("farclip", "177"),
                    ("ffx", "0"),
                    ("fog", "0"),
                    ("fullalpha", "0"),
                    ("groundeffectdensity", "16"),
                    ("groundeffectdist", "1"),
                    ("gxcolorbits", "16"),
                    ("gxdepthbits", "16"),
                    ("horizonfarclip", "1305"),
                    ("hwPCF", "1"),
                    ("light", "0"),
                    ("lod", "0"),
                    ("loddist", "50"),
                    ("m2Faster", "1"),
                    ("mapshadows", "0"),
                    ("maxlights", "0"),
                    ("maxlod", "0"),
                    ("overridefarclip ", "0"),
                    ("particledensity", "0.3"),
                    ("pixelshader", "0"),
                    ("shadowlevel", "1"),
                    ("shadowlod", "0"),
                    ("showfootprintparticles", "0"),
                    ("showfootprints", "0"),
                    ("showshadow", "0"),
                    ("showwater", "0"),
                    ("skyclouddensity", "0"),
                    ("skycloudlod", "0"),
                    ("skyshow", "0"),
                    ("skysunglare", "0"),
                    ("smallcull", "1"),
                    ("specular", "0"),
                    ("textureloddist", "80"),
                    ("timingmethod", "1"),
                    ("unitdrawdist", "20"),
                    ("waterlod", "0"),
                    ("watermaxlod", "0"),
                    ("waterparticulates", "0"),
                    ("waterripples", "0"),
                    ("waterspecular", "0"),
                    ("waterwaves", "0"),
                });
            }

            StringBuilder sb = new();

            foreach ((string cvar, string value) in cvars)
            {
                sb.Append($"pcall(SetCVar,\"{cvar}\",\"{value}\");");
            }

            Bot.Wow.LuaDoString(sb.ToString());
        }

        /// <summary>
        /// Sets up the WoW interface by calling the Setup method of the Bot.Wow object. 
        /// Returns Success if the setup is successful, otherwise returns Failed.
        /// </summary>
        private BtStatus SetupWowInterface()
        {
            return Bot.Wow.Setup() ? BtStatus.Success : BtStatus.Failed;
        }

        /// <summary>
        /// Starts the WoW process.
        /// </summary>
        /// <returns>The status of the WoW process.</returns>
        private BtStatus StartWow()
        {
            if (File.Exists(Config.PathToWowExe))
            {
                AmeisenLogger.I.Log("StartWow", "Starting WoW Process");
                Process p = Bot.Memory.StartProcessNoActivate($"\"{Config.PathToWowExe}\" -windowed -d3d9", out IntPtr processHandle, out IntPtr mainThreadHandle);
                p.WaitForInputIdle();

                AmeisenLogger.I.Log("StartWow", $"Attaching XMemory to {p.ProcessName} ({p.Id})");

                if (Bot.Memory.Init(p, processHandle, mainThreadHandle))
                {
                    Bot.Memory.Offsets.Init(Bot.Memory.Process.MainModule.BaseAddress);

                    OnWoWStarted?.Invoke();

                    if (Config.SaveWowWindowPosition)
                    {
                        LoadWowWindowPosition();
                    }

                    return BtStatus.Success;
                }
                else
                {
                    AmeisenLogger.I.Log("StartWow", $"Attaching XMemory failed...");
                    p.Kill();
                    return BtStatus.Failed;
                }
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// Method used to update the in-game functionality.
        /// </summary>
        private void UpdateIngame()
        {
            if (FirstStart)
            {
                FirstStart = false;
                IngameSince = DateTime.UtcNow;
            }

            if (Bot.Wow.Events != null)
            {
                if (!Bot.Wow.Events.IsActive && DateTime.UtcNow - IngameSince > TimeSpan.FromSeconds(2))
                {
                    // need to wait for the Frame setup
                    Bot.Wow.Events.Start();
                }

                Bot.Wow.Events.Tick();
            }

            Bot.Movement.Execute();

            if (CharacterUpdateEvent.Run())
            {
                Bot.Character.UpdateAll();
            }

            if (!Bot.Player.IsDead)
            {
                DungeonDiedTimestamp = default;
            }

            // auto disable rendering when not in focus
            if (Config.AutoDisableRender && RenderSwitchEvent.Run())
            {
                IntPtr foregroundWindow = Bot.Memory.GetForegroundWindow();
                Bot.Wow.SetRenderState(foregroundWindow == Bot.Memory.Process.MainWindowHandle);
            }
        }
    }
}