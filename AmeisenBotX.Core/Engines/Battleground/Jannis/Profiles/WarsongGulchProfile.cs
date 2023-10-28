using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a profile for the Warsong Gulch battleground.
/// </summary>
namespace AmeisenBotX.Core.Engines.Battleground.Jannis.Profiles
{
    /// <summary>
    /// Represents a profile for the Warsong Gulch battleground.
    /// </summary>
    public class WarsongGulchProfile : IBattlegroundProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarsongGulchProfile"/> class.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public WarsongGulchProfile(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            ActionEvent = new(TimeSpan.FromMilliseconds(500));
            LosCheckEvent = new(TimeSpan.FromMilliseconds(1000));

            JBgBlackboard = new(UpdateBattlegroundInfo);

            KillEnemyFlagCarrierSelector = new
            (
                (b) => b.EnemyTeamFlagCarrier != null,
                new Selector<CtfBlackboard>
                (
                    (b) => b.MyTeamHasFlag
                        || b.EnemyTeamFlagCarrier.Position.GetDistance(WsgDataset.EnemyBasePosition)
                         < Bot.Player.Position.GetDistance(WsgDataset.EnemyBasePosition),
                    new Leaf<CtfBlackboard>(KillEnemyFlagCarrier),
                    new Leaf<CtfBlackboard>(MoveToEnemyBaseAndGetFlag)
                ),
                new Selector<CtfBlackboard>
                (
                    (b) => b.EnemyTeamFlagPos.GetDistance2D(WsgDataset.EnemyBasePositionMapCoords)
                         < b.EnemyTeamFlagPos.GetDistance2D(WsgDataset.OwnBasePositionMapCoords),
                    new Leaf<CtfBlackboard>((b) => MoveToPosition(WsgDataset.EnemyBasePosition)),
                    new Leaf<CtfBlackboard>((b) => MoveToPosition(WsgDataset.OwnBasePosition))
                )
            );

            FlagSelector = new DualSelector<CtfBlackboard>
            (
                (b) => b.MyTeamHasFlag,
                (b) => b.EnemyTeamHasFlag,

                // no team has the flag
                new Leaf<CtfBlackboard>(MoveToEnemyBaseAndGetFlag),

                // only my team has the flag
                new Selector<CtfBlackboard>
                (
                    (b) => b.MyTeamFlagCarrier != null && b.MyTeamFlagCarrier.Guid == Bot.Wow.PlayerGuid,
                    // i'm the flag carrier
                    new Leaf<CtfBlackboard>((b) => MoveToPosition(WsgDataset.OwnBasePosition)),
                    new Selector<CtfBlackboard>
                    (
                        (b) => b.MyTeamFlagCarrier != null,
                        new Leaf<CtfBlackboard>((b) => MoveToPosition(b.MyTeamFlagCarrier.Position)),
                        new Leaf<CtfBlackboard>(DefendOwnBase)
                    )
                ),

                // only enemy team has the flag
                KillEnemyFlagCarrierSelector,

                // both teams have the flag
                new Selector<CtfBlackboard>
                (
                    (b) => b.MyTeamFlagCarrier != null && b.MyTeamFlagCarrier.Guid == Bot.Wow.PlayerGuid,
                    new Selector<CtfBlackboard>
                    (
                        (b) => Bot.Player.Position.GetDistance(WsgDataset.OwnBasePosition) < 128.0f,
                        new Leaf<CtfBlackboard>((b) => MoveToPosition(WsgDataset.FlagHidingSpot)),
                        new Leaf<CtfBlackboard>((b) => MoveToPosition(WsgDataset.OwnBasePosition))
                    ),
                    new Selector<CtfBlackboard>
                    (
                        (b) => b.MyTeamFlagCarrier != null
                            && b.MyTeamFlagCarrier.DistanceTo(Bot.Player) < 50.0f
                            && Bot.GetNearFriends<IWowPlayer>(b.MyTeamFlagCarrier.Position, 25.0f).Count() < 2,
                        // assist our flag carrier
                        new Leaf<CtfBlackboard>((b) => MoveToPosition(b.MyTeamFlagCarrier.Position)),
                        // go kill other flag carrier
                        KillEnemyFlagCarrierSelector
                    )
                )
            );

            MainSelector = new Selector<CtfBlackboard>
            (
                (b) => IsGateOpen(),
                 new Selector<CtfBlackboard>
                 (
                     (b) => IsFlagNear(),
                     new Leaf<CtfBlackboard>(UseNearestFlag),
                     new Selector<CtfBlackboard>
                     (
                         (b) => IsAnyBuffNearMeAndNoOneElseUsingIt(16.0f),
                         new Leaf<CtfBlackboard>(MoveToNearestBuff),
                         new Selector<CtfBlackboard>
                         (
                             (b) => DoWeOutnumberOurEnemies(b),
                             new Leaf<CtfBlackboard>(AttackNearWeakestEnemy),
                             FlagSelector
                         )
                     )
                 ),
                 new Leaf<CtfBlackboard>((b) => MoveToPosition(WsgDataset.GatePosition))
            );

            BehaviorTree = new
            (
                MainSelector,
                JBgBlackboard,
                TimeSpan.FromSeconds(1)
            );
        }

        /// <summary>
        /// Represents a dataset interface for Warsong Gulch battleground.
        /// </summary>
        private interface IWsgDataset
        {
            /// <summary>
            /// A static list of Buff Display IDs used for various purposes.
            /// </summary>
            static readonly List<int> BuffDisplayIds = new() { 5991, 5995, 5931 };

            /// <summary>
            /// Gets the position of the enemy base.
            /// </summary>
            Vector3 EnemyBasePosition { get; }

            /// <summary>
            /// Gets the map coordinates of the enemy base.
            /// </summary>
            Vector3 EnemyBasePositionMapCoords { get; }

            /// <summary>
            /// Gets the position of the enemy graveyard.
            /// </summary>
            Vector3 EnemyGraveyardPosition { get; }

            /// <summary>
            /// Gets the position of the flag hiding spot.
            /// </summary>
            Vector3 FlagHidingSpot { get; }

            /// <summary>
            /// Gets the position of the gate.
            /// </summary>
            Vector3 GatePosition { get; }

            /// <summary>
            /// Gets the position of the own base.
            /// </summary>
            Vector3 OwnBasePosition { get; }

            /// <summary>
            /// Gets the map coordinates of the own base.
            /// </summary>
            Vector3 OwnBasePositionMapCoords { get; }

            /// <summary>
            /// Gets the position of the own graveyard.
            /// </summary>
            Vector3 OwnGraveyardPosition { get; }
        }

        /// <summary>
        /// Gets the behavior tree used for decision-making in the Warsong Gulch battleground.
        /// </summary>
        public BehaviorTree<CtfBlackboard> BehaviorTree { get; }

        /// <summary>
        /// Gets the selector responsible for handling flag-related decisions in the battleground.
        /// </summary>
        public DualSelector<CtfBlackboard> FlagSelector { get; }

        /// <summary>
        /// Gets or sets the blackboard used to store and exchange information within the profile.
        /// </summary>
        public CtfBlackboard JBgBlackboard { get; set; }

        /// <summary>
        /// Gets the main selector for executing battleground-related behaviors.
        /// </summary>
        public Selector<CtfBlackboard> MainSelector { get; }

        /// <summary>
        /// Gets the event timer for triggering specific actions within the profile.
        /// </summary>
        private TimegatedEvent ActionEvent { get; }

        /// <summary>
        /// Gets the reference to the bot interface for interacting with the game world.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player character is in line of sight (LOS) of a target.
        /// </summary>
        private bool InLos { get; set; }

        /// <summary>
        /// Gets the selector responsible for making decisions related to killing the enemy flag carrier.
        /// </summary>
        private Selector<CtfBlackboard> KillEnemyFlagCarrierSelector { get; }

        /// <summary>
        /// Gets the event timer for checking line of sight (LOS) conditions.
        /// </summary>
        private TimegatedEvent LosCheckEvent { get; }

        /// <summary>
        /// Gets or sets the dataset containing positional information specific to Warsong Gulch battleground.
        /// </summary>
        private IWsgDataset WsgDataset { get; set; }

        /// <inheritdoc />
        public void Execute()
        {
            if (WsgDataset == null)
            {
                WsgDataset = Bot.Player.IsAlliance() ? new AllianceWsgDataset() : new HordeWsgDataset();
            }

            BehaviorTree.Tick();
        }

        /// <summary>
        /// Determines whether the player character is near their own flag carrier.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns><c>true</c> if the player character is near their own flag carrier; otherwise, <c>false</c>.</returns>
        private bool AmINearOwnFlagCarrier(CtfBlackboard blackboard)
        {
            return Bot.GetNearEnemies<IWowUnit>(blackboard.MyTeamFlagCarrier.Position, 48.0f).Any(e => !e.IsDead);
        }

        /// <summary>
        /// Determines whether the player character is one of the closest members to their own flag carrier.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <param name="memberCount">The number of closest members to consider.</param>
        /// <returns><c>true</c> if the player character is one of the closest members; otherwise, <c>false</c>.</returns>
        private bool AmIOneOfTheClosestToOwnFlagCarrier(CtfBlackboard blackboard, int memberCount)
        {
            if (memberCount <= 0 || blackboard.MyTeamFlagCarrier == null)
            {
                return false;
            }

            // check whether i'm part of the closest x (memberCount) members to the flag carrier
            int index = Bot.Objects.Partymembers.OfType<IWowPlayer>()
                            .Where(e => e.Guid != blackboard.MyTeamFlagCarrier.Guid)
                            .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                            .Select((player, id) => new { Player = player, Index = id })
                            .FirstOrDefault(_ => _.Player.Guid == Bot.Wow.PlayerGuid)?.Index ?? -1;

            return index > -1 && index <= memberCount;
        }

        /// <summary>
        /// Initiates an attack on the nearest enemy player with the lowest health.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus AttackNearWeakestEnemy(CtfBlackboard blackboard)
        {
            IWowPlayer weakestPlayer = Bot.GetNearEnemies<IWowPlayer>(Bot.Player.Position, 20.0f).OrderBy(e => e.Health).FirstOrDefault();

            if (weakestPlayer != null)
            {
                InitiateCombat(weakestPlayer);
                return BtStatus.Success;
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// Defends the player's own base by either moving to a target or initiating combat with nearby enemies.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus DefendOwnBase(CtfBlackboard blackboard)
        {
            if (!CommonRoutines.MoveToTarget(Bot, WsgDataset.OwnBasePosition, 16.0f))
            {
                IWowUnit nearEnemy = Bot.GetNearEnemies<IWowUnit>(WsgDataset.OwnBasePosition, 16.0f).FirstOrDefault();

                if (nearEnemy != null)
                {
                    InitiateCombat(nearEnemy);
                }
            }

            return BtStatus.Ongoing;
        }

        /// <summary>
        /// Determines whether the player's team outnumbers the enemy team within a certain radius.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns><c>true</c> if the player's team outnumbers the enemy team; otherwise, <c>false</c>.</returns>
        private bool DoWeOutnumberOurEnemies(CtfBlackboard blackboard)
        {
            if (blackboard.MyTeamFlagCarrier != null && blackboard.MyTeamFlagCarrier.Guid == Bot.Wow.PlayerGuid)
            {
                return false;
            }

            int friends = Bot.GetNearFriends<IWowPlayer>(Bot.Player.Position, 18.0f).Count();
            int enemies = Bot.GetNearEnemies<IWowPlayer>(Bot.Player.Position, 18.0f).Count();

            return enemies > 0 && friends >= enemies;
        }

        /// <summary>
        /// Determines whether there are enemies near the flag carrier of the player's team.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns><c>true</c> if there are enemies near the flag carrier; otherwise, <c>false</c>.</returns>
        private bool EnemiesNearFlagCarrier(CtfBlackboard blackboard)
        {
            return blackboard.MyTeamFlagCarrier.Position.GetDistance(Bot.Player.Position) < 32.0;
        }

        /// <summary>
        /// Attempts to flee from nearby enemies by moving away from the nearest enemy player.
        /// </summary>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus FleeFromComingEnemies()
        {
            IWowUnit nearestEnemy = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 48.0f).OrderBy(e => e.Position.GetDistance(Bot.Player.Position)).FirstOrDefault();

            if (nearestEnemy != null)
            {
                Bot.Movement.SetMovementAction(MovementAction.Flee, nearestEnemy.Position, nearestEnemy.Rotation);
                return BtStatus.Ongoing;
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Initiates combat with a specified unit, either by changing the target or moving into combat range.
        /// </summary>
        /// <param name="unit">The unit to initiate combat with.</param>
        private void InitiateCombat(IWowUnit unit)
        {
            if (Bot.Target == null)
            {
                Bot.Wow.ChangeTarget(unit.Guid);
                return;
            }

            MovementAction action = Bot.Player.DistanceTo(WsgDataset.OwnGraveyardPosition) < 24.0f
                || Bot.Player.DistanceTo(WsgDataset.EnemyGraveyardPosition) < 24.0f
                    ? MovementAction.DirectMove
                    : MovementAction.Chase;

            if (!CommonRoutines.MoveToTarget(Bot, Bot.Target.Position, Bot.CombatClass.IsMelee ? 3.0f : 28.0f, action))
            {
                Bot.CombatClass?.Execute();
            }
        }

        /// <summary>
        /// Determines whether there is any usable buff object near the player character, and no other party members are using it.
        /// </summary>
        /// <param name="distance">The maximum distance to search for a buff object.</param>
        /// <returns><c>true</c> if there is a usable buff object near the player character; otherwise, <c>false</c>.</returns>
        private bool IsAnyBuffNearMeAndNoOneElseUsingIt(float distance)
        {
            IWowGameobject buff = Bot.GetClosestGameObjectByDisplayId(Bot.Player.Position, IWsgDataset.BuffDisplayIds);
            return buff != null && buff.Position.GetDistance(Bot.Player.Position) < distance && !Bot.GetNearPartyMembers<IWowPlayer>(buff.Position, 8.0f).Any();
        }

        /// <summary>
        /// Determines whether a flag object is near the player character.
        /// </summary>
        /// <returns><c>true</c> if a flag object is near; otherwise, <c>false</c>.</returns>
        private bool IsFlagNear()
        {
            return JBgBlackboard.NearFlags != null && JBgBlackboard.NearFlags.Any(e => e.Position.GetDistance(Bot.Player.Position) < 8.0);
        }

        /// <summary>
        /// Determines whether the gate to the player's base is open.
        /// </summary>
        /// <returns><c>true</c> if the gate is open; otherwise, <c>false</c>.</returns>
        private bool IsGateOpen()
        {
            if (Bot.Player.IsAlliance())
            {
                IWowGameobject obj = Bot.Objects.All.OfType<IWowGameobject>()
                                    .FirstOrDefault(e => e.GameObjectType == WowGameObjectType.Door && e.DisplayId == 411);

                return obj == null || obj.Bytes0 == 0;
            }
            else
            {
                IWowGameobject obj = Bot.Objects.All.OfType<IWowGameobject>()
                                    .FirstOrDefault(e => e.GameObjectType == WowGameObjectType.Door && e.DisplayId == 850);

                return obj == null || obj.Bytes0 == 0;
            }
        }

        /// <summary>
        /// Attempts to kill the enemy flag carrier by initiating combat with them.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus KillEnemyFlagCarrier(CtfBlackboard blackboard)
        {
            if (JBgBlackboard.EnemyTeamFlagCarrier == null)
            {
                return BtStatus.Failed;
            }

            InitiateCombat(JBgBlackboard.EnemyTeamFlagCarrier);
            return BtStatus.Ongoing;
        }

        /// <summary>
        /// Moves the player character to the enemy base to retrieve the flag.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus MoveToEnemyBaseAndGetFlag(CtfBlackboard blackboard)
        {
            return !CommonRoutines.MoveToTarget(Bot, WsgDataset.EnemyBasePosition, 2.0f) && JBgBlackboard.NearFlags != null
                ? UseNearestFlag(blackboard)
                : BtStatus.Ongoing;
        }

        /// <summary>
        /// Moves the player character to the nearest buff object.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus MoveToNearestBuff(CtfBlackboard blackboard)
        {
            IWowGameobject buffObject = Bot.GetClosestGameObjectByDisplayId(Bot.Player.Position, IWsgDataset.BuffDisplayIds);

            return buffObject == null || !CommonRoutines.MoveToTarget(Bot, buffObject.Position, 3.0f) ? BtStatus.Failed : BtStatus.Ongoing;
        }

        /// <summary>
        /// Moves the player character to their own flag carrier to provide assistance.
        /// </summary>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus MoveToOwnFlagCarrierAndHelp()
        {
            if (JBgBlackboard.MyTeamFlagCarrier == null)
            {
                return BtStatus.Failed;
            }

            if (!CommonRoutines.MoveToTarget(Bot, JBgBlackboard.MyTeamFlagCarrier.Position, 5.0f))
            {
                IWowUnit nearEnemy = Bot.GetNearEnemies<IWowUnit>(JBgBlackboard.MyTeamFlagCarrier.Position, 32.0f).FirstOrDefault();

                if (nearEnemy != null)
                {
                    InitiateCombat(nearEnemy);
                    return BtStatus.Success;
                }

                return BtStatus.Failed;
            }

            return BtStatus.Ongoing;
        }

        /// <summary>
        /// Moves the player character to a specified position with optional minimum distance.
        /// </summary>
        /// <param name="position">The position to move to.</param>
        /// <param name="minDistance">The minimum distance to maintain from the position (default is 3.5).</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus MoveToPosition(Vector3 position, float minDistance = 3.5f)
        {
            double distance = Bot.Player.Position.GetDistance(position);

            if (distance > minDistance)
            {
                if (LosCheckEvent.Run())
                {
                    InLos = Bot.Wow.IsInLineOfSight(Bot.Player.Position, position, 2.0f);
                }

                float zDiff = position.Z - Bot.Player.Position.Z;

                MovementAction action = (zDiff < -4.0 && InLos)
                    || Bot.Player.DistanceTo(WsgDataset.OwnGraveyardPosition) < 24.0f
                    || Bot.Player.DistanceTo(WsgDataset.EnemyGraveyardPosition) < 24.0f
                        ? MovementAction.DirectMove
                        : MovementAction.Move;

                Bot.Movement.SetMovementAction(action, position);

                return BtStatus.Ongoing;
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Updates the information about the battleground state, including scores, flag positions, and flag carriers.
        /// </summary>
        private void UpdateBattlegroundInfo()
        {
            try
            {
                if (Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=\"{{\"_,stateA,textA,_,_,_,_,_,_,_,_,_=GetWorldStateUIInfo(2)_,stateH,textH,_,_,_,_,_,_,_,_,_=GetWorldStateUIInfo(3)flagXA,flagYA=GetBattlefieldFlagPosition(1)flagXH,flagYH=GetBattlefieldFlagPosition(2){{v:0}}={{v:0}}..\"\\\"allianceState\\\" : \\\"\"..stateA..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"allianceText\\\" : \\\"\"..textA..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"hordeState\\\" : \\\"\"..stateH..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"hordeText\\\" : \\\"\"..textH..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"allianceFlagX\\\" : \\\"\"..flagXA..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"allianceFlagY\\\" : \\\"\"..flagYA..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"hordeFlagX\\\" : \\\"\"..flagXH..\"\\\",\"{{v:0}}={{v:0}}..\"\\\"hordeFlagY\\\" : \\\"\"..flagYH..\"\\\"\"{{v:0}}={{v:0}}..\"}}\""), out string result))
                {
                    Dictionary<string, dynamic> bgState = JsonSerializer.Deserialize<JsonElement>(result, new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString }).ToDyn();

                    string[] splittedScoreH = ((string)bgState["hordeText"]).Split('/');
                    string[] splittedScoreA = ((string)bgState["allianceText"]).Split('/');

                    float allianceFlagX = float.Parse((string)bgState["allianceFlagX"], NumberStyles.Any) * 100.0f;
                    float allianceFlagY = float.Parse((string)bgState["allianceFlagY"], NumberStyles.Any) * 100.0f;

                    float hordeFlagX = float.Parse((string)bgState["hordeFlagX"], NumberStyles.Any) * 100.0f;
                    float hordeFlagY = float.Parse((string)bgState["hordeFlagY"], NumberStyles.Any) * 100.0f;

                    if (Bot.Player.IsAlliance())
                    {
                        JBgBlackboard.MyTeamScore = int.Parse(splittedScoreA[0]);
                        JBgBlackboard.MyTeamMaxScore = int.Parse(splittedScoreA[1]);

                        JBgBlackboard.EnemyTeamScore = int.Parse(splittedScoreH[0]);
                        JBgBlackboard.EnemyTeamMaxScore = int.Parse(splittedScoreH[1]);

                        JBgBlackboard.MyTeamHasFlag = int.Parse((string)bgState["allianceState"]) == 2;
                        JBgBlackboard.EnemyTeamHasFlag = int.Parse((string)bgState["hordeState"]) == 2;

                        JBgBlackboard.MyTeamFlagPos = new(allianceFlagX, allianceFlagY, 0.0f);
                        JBgBlackboard.EnemyTeamFlagPos = new(hordeFlagX, hordeFlagY, 0.0f);

                        JBgBlackboard.MyTeamFlagCarrier = Bot.Objects.All.OfType<IWowPlayer>().FirstOrDefault(e => e.HasBuffById(23333));
                        JBgBlackboard.EnemyTeamFlagCarrier = Bot.Objects.All.OfType<IWowPlayer>().FirstOrDefault(e => e.HasBuffById(23335));
                    }
                    else
                    {
                        JBgBlackboard.MyTeamScore = int.Parse(splittedScoreH[0]);
                        JBgBlackboard.MyTeamMaxScore = int.Parse(splittedScoreH[1]);

                        JBgBlackboard.EnemyTeamScore = int.Parse(splittedScoreA[0]);
                        JBgBlackboard.EnemyTeamMaxScore = int.Parse(splittedScoreA[1]);

                        JBgBlackboard.MyTeamHasFlag = int.Parse((string)bgState["hordeState"]) == 2;
                        JBgBlackboard.EnemyTeamHasFlag = int.Parse((string)bgState["allianceState"]) == 2;

                        JBgBlackboard.MyTeamFlagPos = new(hordeFlagX, hordeFlagY, 0.0f);
                        JBgBlackboard.EnemyTeamFlagPos = new(allianceFlagX, allianceFlagY, 0.0f);

                        JBgBlackboard.MyTeamFlagCarrier = Bot.Objects.All.OfType<IWowPlayer>().FirstOrDefault(e => e.HasBuffById(23335));
                        JBgBlackboard.EnemyTeamFlagCarrier = Bot.Objects.All.OfType<IWowPlayer>().FirstOrDefault(e => e.HasBuffById(23333));
                    }

                    JBgBlackboard.NearFlags = Bot.Objects.All.OfType<IWowGameobject>()
                                                 .Where(e => e.DisplayId == (int)WowGameObjectDisplayId.WsgAllianceFlag
                                                          || e.DisplayId == (int)WowGameObjectDisplayId.WsgHordeFlag);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Uses the nearest flag object if available and moves towards it.
        /// </summary>
        /// <param name="blackboard">The blackboard containing relevant information.</param>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus UseNearestFlag(CtfBlackboard blackboard)
        {
            IWowGameobject nearestFlag = JBgBlackboard.NearFlags.OrderBy(e => e.Position.GetDistance(Bot.Player.Position)).FirstOrDefault();

            if (nearestFlag != null)
            {
                if (!CommonRoutines.MoveToTarget(Bot, nearestFlag.Position, 4.0f) && ActionEvent.Run())
                {
                    Bot.Wow.InteractWithObject(nearestFlag);
                    return BtStatus.Success;
                }

                return BtStatus.Ongoing;
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// Represents a dataset specific to the Alliance faction in Warsong Gulch battleground.
        /// </summary>
        private class AllianceWsgDataset : IWsgDataset
        {
            /// <inheritdoc />
            public Vector3 EnemyBasePosition { get; } = new(916, 1434, 346);

            /// <inheritdoc />
            public Vector3 EnemyBasePositionMapCoords { get; } = new(53, 90, 0);

            /// <inheritdoc />
            public Vector3 EnemyGraveyardPosition { get; } = new(1415, 1555, 343);

            /// <inheritdoc />
            public Vector3 FlagHidingSpot { get; } = new(1519, 1467, 374);

            /// <inheritdoc />
            public Vector3 GatePosition { get; } = new(1494, 1457 + (float)((new Random().NextDouble() * 16.0f) - 8.0f), 343);

            /// <inheritdoc />
            public Vector3 OwnBasePosition { get; } = new(1539, 1481, 352);

            /// <inheritdoc />
            public Vector3 OwnBasePositionMapCoords { get; } = new(49, 15, 0);

            /// <inheritdoc />
            public Vector3 OwnGraveyardPosition { get; } = new(1029, 1387, 340);
        }

        /// <summary>
        /// Represents a dataset specific to the Horde faction in Warsong Gulch battleground.
        /// </summary>
        private class HordeWsgDataset : IWsgDataset
        {
            /// <inheritdoc />
            public Vector3 EnemyBasePosition { get; } = new(1539, 1481, 352);

            /// <inheritdoc />
            public Vector3 EnemyBasePositionMapCoords { get; } = new(49, 15, 0);

            /// <inheritdoc />
            public Vector3 EnemyGraveyardPosition { get; } = new(1029, 1387, 340);

            /// <inheritdoc />
            public Vector3 FlagHidingSpot { get; } = new(949, 1449, 367);

            /// <inheritdoc />
            public Vector3 GatePosition { get; private set; } = new(951, 1459 + (float)((new Random().NextDouble() * 16.0f) - 8.0f), 342);

            /// <inheritdoc />
            public Vector3 OwnBasePosition { get; } = new(916, 1434, 346);

            /// <inheritdoc />
            public Vector3 OwnBasePositionMapCoords { get; } = new(53, 90, 0);

            /// <inheritdoc />
            public Vector3 OwnGraveyardPosition { get; } = new(1415, 1555, 343);
        }
    }
}