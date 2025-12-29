using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Logic.Enums;
using AmeisenBotX.Core.Logic.Leafs;
using AmeisenBotX.Core.Logic.Leafs.Combat;
using AmeisenBotX.Core.Logic.Leafs.Movement;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Core.Logic.Services;
using AmeisenBotX.Core.Logic.Startup;
using AmeisenBotX.Core.Logic.StaticDeathRoutes;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
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
using System.Threading;

namespace AmeisenBotX.Core.Logic
{
    public class AmeisenBotLogic : IAmeisenBotLogic
    {
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
            LoginAttemptEvent = new(TimeSpan.FromMilliseconds(500));
            RenderSwitchEvent = new(TimeSpan.FromMilliseconds(1000));
            _inventoryUpdateThrottle = new(TimeSpan.FromMilliseconds(800 + Random.Shared.Next(400)));
            _trainUpdateThrottle = new(TimeSpan.FromMilliseconds(1600 + Random.Shared.Next(800)));

            // Initialize Services
            LootService = new LootService(bot, config);
            GatherService = new GatherService(bot, config);
            CombatService = new CombatService(bot, config);
            EatService = new EatService(bot, config);
            FirstAidService = new FirstAidService(bot, config);
            FollowService = new FollowService(bot, config);
            NpcService = new NpcService(bot, config);
            EquipUpgrades = new EquipUpgradesRoutine(Bot, Config);
            Launcher = new WowLauncher(config, bot);
            Launcher.OnWoWStarted += () => OnWoWStarted?.Invoke();
            LoginMgr = new LoginManager(config, Bot.Wow);

            SubscribeToEvents();

            // --- Behavior Tree Construction ---

            INode openworldGhostNode = new Selector
            (
                () => CanUseStaticPaths(),
                new SuccessLeaf(() => Bot.Movement.DirectMove(StaticRoute.GetNextPoint(Bot.Player.Position)), "Ghost.StaticPath"),
                new Leaf(RunToCorpseAndRetrieveIt, "Ghost.RunToCorpse")
            );

            // Fallback: simple movement or auto attack
            INode autoAttackFallback = new Annotator(
                new Leaf(() => { Bot.Movement.Execute(); return BtStatus.Success; }, "Combat.Movement"),
                new Leaf(() => { Bot.Wow.StartAutoAttack(); return BtStatus.Success; }, "Combat.AutoAttack")
            );

            // Decisions: Check Services to determine state

            // Combat Node
            INode combatNode = new Selector(
                () => Bot.CombatClass == null, // If no combat class, skip
                autoAttackFallback,
                new Annotator(
                    new UsePotionsLeaf(Bot, Config),
                    new Annotator(
                        new CombatMovementLeaf(Bot),
                        new SuccessLeaf(() => Bot.CombatClass.Execute(), "Combat.Execute")
                    )
                )
            );

            // Check Combat Condition Wrapper
            Func<bool> checkCombat = () =>
            {
                CombatService.CheckCombatState();
                return CombatService.ShouldFight;
            };

            // Needs Repair/Sell
            Func<bool> checkRepairSell = () =>
            {
                UpdateInventoryState(); // ensure state is fresh-ish
                return NpcService.NeedToRepairOrSell(Mode, NeedsRepair, ItemsToSell.Count > 0);
            };

            INode interactWithMerchantNode = new Selector(
                () => NpcService.Merchant == null,
                new Leaf(() => MoveToPosition(NpcService.TargetNpcPosition), name: "OpenWorld.MoveToMerchant"),
                new Annotator(
                    new InteractWithUnitLeaf(Bot, () => NpcService.Merchant, name: "OpenWorld.Merchant"),
                    new Leaf(() => { SpeakToMerchantRoutine.Run(Bot, NpcService.Merchant, Config, ItemsToSell); return BtStatus.Success; }, "OpenWorld.Sell")
                )
            );

            // Class Trainer
            Func<bool> checkClassTrainer = () =>
            {
                UpdateTrainState();
                return NpcService.NeedToTrainSpells(NeedsTrainSpells);
            };

            INode interactWithClassTrainerNode = new Selector(
                () => NpcService.ClassTrainer == null,
                new Leaf(() => MoveToPosition(NpcService.TargetNpcPosition), name: "OpenWorld.MoveToClassTrainer"),
                new InteractWithUnitLeaf(Bot, () => NpcService.ClassTrainer, new Leaf(() => { TrainAllSpellsRoutine.Run(Bot); return BtStatus.Success; }, "Train"), name: "OpenWorld.ClassTrainer")
            );

            // Profession Trainer
            Func<bool> checkProfTrainer = () => NpcService.NeedToTrainSecondarySkills();

            INode interactWithProfessionTrainerNode = new Selector(
                () => NpcService.ProfessionTrainer == null,
                new Leaf(() => MoveToPosition(NpcService.TargetNpcPosition), name: "OpenWorld.MoveToProfessionTrainer"),
                new InteractWithUnitLeaf(Bot, () => NpcService.ProfessionTrainer, new Leaf(() => { TrainAllSpellsRoutine.Run(Bot); return BtStatus.Success; }, "Train"), name: "OpenWorld.ProfessionTrainer")
            );


            // First Aid
            Func<bool> checkFirstAid = () => { FirstAidService.CheckState(); return FirstAidService.ShouldBandage; };
            INode firstAidNode = new Leaf(() => FirstAidService.Execute(), "FirstAid");

            // Eat/Drink
            Func<bool> checkEat = () => { EatService.CheckEatState(); return EatService.ShouldEat; };
            INode eatNode = new Leaf(() => EatService.ExecuteEat(), "Eat");

            // Gather
            INode collectGobjectsNode = new Leaf(
                () => GatherService.Execute(),
                "OpenWorld.Gather"
            );

            // Loot
            Func<bool> checkLoot = () => LootService.ScanForLoot();
            INode lootNode = new TimeLimit(TimeSpan.FromSeconds(30), new Leaf(() => LootService.ExecuteLoot(), "Loot"));


            // --- Main Logical Blocks ---

            INode jobsNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Jobs.Execute(), "Jobs.Execute"),
                (() => Bot.Player.IsDead, new Leaf(Dead, "Jobs.Dead")),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (() => !Bot.Player.IsMounted && checkCombat(), combatNode),
                (checkRepairSell, interactWithMerchantNode),
                (checkFirstAid, firstAidNode),
                (checkEat, eatNode)
            );

            INode grindingNode = new StickySelector
            (
                TimeSpan.FromSeconds(30),
                new SuccessLeaf(() => Bot.Grinding.Execute(), "Grind.Execute"),
                (() => Bot.Player.IsDead, new Leaf(Dead, "Grind.Dead")),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (checkCombat, combatNode),
                (checkFirstAid, firstAidNode),
                (checkEat, eatNode),
                (checkRepairSell, interactWithMerchantNode),
                (checkClassTrainer, interactWithClassTrainerNode),
                (checkProfTrainer, interactWithProfessionTrainerNode),
                (checkLoot, lootNode)
            );

            INode questingNode = new Waterfall
           (
               new SuccessLeaf(() => Bot.Quest.Execute(), "Quest.Execute"),
               (() => Bot.Player.IsDead, new Leaf(Dead, "Quest.Dead")),
               (() => Bot.Player.IsGhost, openworldGhostNode),
               (checkCombat, combatNode),
               (checkFirstAid, firstAidNode),
               (checkEat, eatNode),
               (checkRepairSell, interactWithMerchantNode),
               (checkLoot, lootNode)
           );

            INode pvpNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Pvp.Execute(), "PvP.Execute"),
                (() => Bot.Player.IsDead, new Leaf(Dead, "PvP.Dead")),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (checkCombat, combatNode),
                (checkFirstAid, firstAidNode),
                (checkEat, eatNode),
                (checkRepairSell, interactWithMerchantNode),
                (checkLoot, lootNode)
            );

            INode testingNode = new Waterfall
            (
                new SuccessLeaf(() => Bot.Test.Execute(), "Test.Execute"),
                (() => Bot.Player.IsDead, new Leaf(Dead, "Test.Dead")),
                (() => Bot.Player.IsGhost, openworldGhostNode)
            );

            INode openworldNode = new StickySelector
            (
                TimeSpan.FromSeconds(30), // Max 30 seconds sticky before re-evaluation
                                          // do idle stuff as fallback
                new SuccessLeaf(() => Bot.CombatClass?.OutOfCombatExecute(), "OpenWorld.OutOfCombat"),
                // handle main open world states - ordered by priority
                (() => Bot.Player.IsDead, new Leaf(Dead, "OpenWorld.Dead")),
                (() => Bot.Player.IsGhost, openworldGhostNode),
                (checkCombat, combatNode),
                (checkLoot, lootNode),
                (checkLoot, lootNode), // Loot has priority over gather to finish jobs
                (() => GatherService.HasValidTarget(), collectGobjectsNode),
                (checkFirstAid, firstAidNode),
                (checkEat, eatNode),
                (() => NpcService.NeedToTalkToQuestgiver(), new InteractWithUnitLeaf(Bot, () => NpcService.QuestGiverToTalkTo, name: "OpenWorld.Questgiver")),
                (checkClassTrainer, interactWithClassTrainerNode),
                (checkProfTrainer, interactWithProfessionTrainerNode),
                (checkRepairSell, interactWithMerchantNode),
                (() => FollowService.ShouldFollow(), new FollowLeaf(Bot)),
                (() => Bot.Character.Professions.HasPendingJob, new BoolLeaf(Bot.Character.Professions.Tick, "OpenWorld.Professions")),
                (() => Config.IdleActions, new Cooldown(TimeSpan.FromSeconds(3), new SuccessLeaf(() => Bot.IdleActions.Tick(Config.Autopilot), "OpenWorld.IdleActions")))
            );

            // SPECIAL ENVIRONMENTS -----------------------------

            INode battlegroundNode = new Waterfall
            (
                new SuccessLeaf(() => { Bot.Battleground.Execute(); }, "BG.Execute"),
                // leave battleground once it is finished
                (IsBattlegroundFinished, new SuccessLeaf(() => { Bot.Wow.LeaveBattleground(); Bot.Battleground.Reset(); }, "BG.Leave")),
                // only handle dead state here, ghost should only be a problem on AV as the
                // graveyard might get lost while we are a ghost
                (() => Bot.Player.IsDead, new Leaf(Dead, "BG.Dead")),
                (checkCombat, combatNode),
                (checkFirstAid, firstAidNode),
                (checkEat, eatNode)
            );

            INode dungeonNode = new Waterfall
            (
                new Selector
                (
                    () => Config.DungeonUsePartyMode,
                    // just follow when we use party mode in dungeon
                    openworldNode,
                    new SuccessLeaf(() => Bot.Dungeon.Execute(), "Dungeon.Execute")
                ),
                (() => Bot.Player.IsDead, new Leaf(DeadDungeon, "Dungeon.Dead")),
                    (
                        checkCombat,
                        new Selector
                        (
                            NeedToFollowTactic,
                            new SuccessLeaf(name: "Dungeon.FollowTactic"),
                            combatNode
                        )
                    ),
                    (checkLoot, lootNode),
                    (checkFirstAid, firstAidNode),
                    (checkEat, eatNode)
                );

            INode raidNode = new Waterfall
            (
                new Selector
                (
                    () => Config.DungeonUsePartyMode,
                    // just follow when we use party mode in raid
                    new FollowLeaf(Bot),
                    new SuccessLeaf(() => Bot.Dungeon.Execute(), "Raid.Execute")
                ),
                (
                    checkCombat,
                    new Selector
                    (
                        NeedToFollowTactic,
                        new SuccessLeaf(name: "Raid.FollowTactic"),
                        combatNode
                    )
                ),
                (checkLoot, lootNode),
                (checkFirstAid, firstAidNode),
                (checkEat, eatNode)
            );

            INode mainLogicNode = new Annotator
            (
                new SuccessLeaf(() => Bot.Wow.Tick(), "Core.WowTick"),
                new Selector
                (
                    () => Bot.Objects.IsWorldLoaded && Bot.Player != null && Bot.Objects != null,
                    new Annotator
                    (
                        new SuccessLeaf(UpdateIngame, "Core.UpdateIngame"),
                        new Waterfall
                        (
                            // open world auto behavior as fallback
                            openworldNode,
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
                    }, "Core.LoadingScreen")
                )
            );

            // ROOT NODE
            Tree = new
            (
                new Waterfall
                (
                    // run the anti afk and main logic if wow is running and we are logged in
                    new Annotator
                    (
                        new SuccessLeaf(AntiAfk, "Core.AntiAfk"),
                        mainLogicNode
                    ),
                    // accept tos and eula, start wow
                    (
                        () => Bot.Memory.Process == null || Bot.Memory.Process.HasExited,
                        new Sequence
                        (
                            new Leaf(Launcher.CheckTosAndEula, "Startup.CheckTos"),
                            new Leaf(Launcher.ChangeRealmlist, "Startup.Realmlist"),
                            new Leaf(Launcher.StartWow, "Startup.Launch")
                        )
                    ),
                    // setup interface and login
                    (() => !Bot.Wow.IsReady, new Leaf(() => Bot.Wow.Setup() ? BtStatus.Success : BtStatus.Failed, "Startup.SetupInterface")),
                    (NeedToLogin, new SuccessLeaf(() => LoginMgr.PerformLogin(AntiAfk), "Startup.Login"))
                )
            );

            // Expose tree and config for debugging
            Bot.BehaviorTree = Tree;
            Bot.Config = Config;
        }

        public event Action OnWoWStarted;

        // Services
        public LootService LootService { get; }
        public GatherService GatherService { get; }
        public CombatService CombatService { get; }
        public FirstAidService FirstAidService { get; }
        public EatService EatService { get; }
        public FollowService FollowService { get; }
        public NpcService NpcService { get; }

        private EquipUpgradesRoutine EquipUpgrades { get; }
        private WowLauncher Launcher { get; }
        private LoginManager LoginMgr { get; }

        public BotMode Mode { get; private set; }

        // Event Throttles
        private TimegatedEvent AntiAfkEvent { get; }
        private TimegatedEvent CharacterUpdateEvent { get; }
        private TimegatedEvent LoginAttemptEvent { get; }
        private TimegatedEvent RenderSwitchEvent { get; }
        private readonly TimegatedEvent _inventoryUpdateThrottle;
        private readonly TimegatedEvent _trainUpdateThrottle;

        private Tree Tree;

        private AmeisenBotInterfaces Bot { get; }
        private AmeisenBotConfig Config { get; }
        private Random Random { get; }

        // State Tracking
        private bool FirstLogin { get; set; }
        private bool FirstStart { get; set; }
        private DateTime DungeonDiedTimestamp { get; set; }
        private DateTime IngameSince { get; set; }

        // Inventory/Training State (Managed here now)
        public List<Managers.Character.Inventory.Objects.IWowInventoryItem> ItemsToSell { get; private set; } = [];
        public bool NeedsRepair { get; private set; }
        public bool NeedsTrainSpells { get; private set; }

        public string State => (Tree.OngoingNode != null ? Tree.OngoingNode.ToString() : Tree.RootNode?.ToString()).Split('.')[^1];

        // Static Death Routes
        public IStaticDeathRoute StaticRoute { get; private set; }
        public bool SearchedStaticRoutes { get; set; }
        private readonly List<IStaticDeathRoute> StaticDeathRoutes =
        [
            new ForgeOfSoulsDeathRoute(),
            new PitOfSaronDeathRoute()
        ];

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
                _ => throw new NotImplementedException(),
            };
        }

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

        public void Tick()
        {
            Tree.Tick();
        }

        private void SubscribeToEvents()
        {
            Bot.Wow.Events.Subscribe("PLAYER_ENTERING_WORLD", (_, _) =>
           {
               // Apply CVars again once ingame to ensure they stick
               if (Config.AutoSetUlowGfxSettings)
               {
                   Bot.Wow.ApplyBotCVars(Config.MaxFps);
               }

               // CRITICAL: Update character data including Skills for harvest modules
               Bot.Character.UpdateAll();

               UpdateInventoryState();
               UpdateTrainState();
               Reset();
           });
        }

        public void Reset()
        {
            LootService.Reset();
            GatherService.Reset();
            CombatService.Reset();
            FirstAidService.Reset();
            EatService.Reset();
            // FollowService has no state
            NpcService.Reset();
        }

        private void UpdateInventoryState()
        {
            if (!_inventoryUpdateThrottle.Ready)
            {
                return;
            }

            // Logic from old DecisionManager
            List<IWowInventoryItem> itemsToSell =
            [
                .. Bot.Character.Inventory.Items.Where(e =>
                    e.Price > 0 && !Config.ItemSellBlacklist.Contains(e.Name) &&
                    (
                        (Config.SellGrayItems && e.ItemQuality == (int)WowItemQuality.Poor) ||
                        (Config.SellWhiteItems && e.ItemQuality == (int)WowItemQuality.Common) ||
                        (Config.SellGreenItems && e.ItemQuality == (int)WowItemQuality.Uncommon) ||
                        (Config.SellBlueItems && e.ItemQuality == (int)WowItemQuality.Rare) ||
                        (Config.SellPurpleItems && e.ItemQuality == (int)WowItemQuality.Epic)
                    )
                ),
            ];

            ItemsToSell = itemsToSell;

            // Repair check
            int durability = 0;
            int maxDurability = 0;
            foreach (IWowInventoryItem item in Bot.Character.Equipment.Items.Values)
            {
                durability += item.Durability;
                maxDurability += item.MaxDurability;
            }

            float durabilityPercent = maxDurability > 0 ? (float)durability / maxDurability * 100f : 100f;
            NeedsRepair = durabilityPercent < Config.ItemRepairThreshold;

            _inventoryUpdateThrottle.Run();
        }

        private void UpdateTrainState()
        {
            if (!_trainUpdateThrottle.Ready)
            {
                return;
            }

            NeedsTrainSpells = Bot.Character.LastLevelTrained != 0 && Bot.Character.LastLevelTrained < Bot.Player.Level;
            _trainUpdateThrottle.Run();
        }

        private void AntiAfk()
        {
            if (AntiAfkEvent.Run())
            {
                Bot.Memory.Write(Bot.Memory.Offsets.TickCount, Environment.TickCount);
                AntiAfkEvent.Timegate = TimeSpan.FromMilliseconds(Random.Next(300, 2300));
            }
        }

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

        private BtStatus DeadDungeon()
        {
            if (!CombatService.ArePartymembersInFight)
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

            if ((!CombatService.ArePartymembersInFight && DateTime.UtcNow - DungeonDiedTimestamp > TimeSpan.FromSeconds(30))
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

        private bool IsBattlegroundFinished()
        {
            return Bot.Memory.Read(Bot.Memory.Offsets.BattlegroundFinished, out int bgFinished)
                && bgFinished == 1;
        }

        private void LoadWowWindowPosition()
        {
            if (Config.SaveWowWindowPosition && !Config.AutoPositionWow)
            {
                if (Bot.Memory.Process.MainWindowHandle != nint.Zero && Config.WowWindowRect != new Rect() { Left = -1, Top = -1, Right = -1, Bottom = -1 })
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

        private BtStatus MoveToPosition(Vector3 position, MovementAction movementAction = MovementAction.Move, float rotation = 0f)
        {
            if (position != Vector3.Zero && Bot.Player.DistanceTo(position) > 3.0f)
            {
                Bot.Movement.SetMovementAction(movementAction, position, rotation);
                return BtStatus.Ongoing;
            }

            return BtStatus.Success;
        }

        private bool NeedToFollowTactic()
        {
            return Bot.Tactic.Execute() && !Bot.Tactic.AllowAttacking;
        }

        private bool NeedToLogin()
        {
            return Bot.Memory.Read(Bot.Memory.Offsets.IsIngame, out int isIngame) && isIngame == 0;
        }

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

        private void UpdateIngame()
        {
            if (FirstStart)
            {
                FirstStart = false;
                IngameSince = DateTime.UtcNow;
                EquipUpgrades.Update();
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
                nint foregroundWindow = Bot.Memory.GetForegroundWindow();
                Bot.Wow.SetRenderState(foregroundWindow == Bot.Memory.Process.MainWindowHandle);
            }


        }
    }
}