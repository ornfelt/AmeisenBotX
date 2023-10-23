using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Battleground.einTyp
{
    /// <summary>
    /// Represents an engine responsible for the battleground behavior.
    /// </summary>
    public class RunBoyRunEngine : IBattlegroundEngine
    {
        /// <summary>
        /// Provides access to various bot functionalities and interfaces.
        /// </summary>
        private readonly AmeisenBotInterfaces Bot;

        /// <summary>
        /// The base position for the Alliance team.
        /// </summary>
        private Vector3 baseAlly = new(1539, 1481, 352);

        /// <summary>
        /// The base position for the Horde team.
        /// </summary>
        private Vector3 baseHord = new(916, 1434, 346);

        /// <summary>
        /// Represents the enemy's flag object.
        /// </summary>
        private IWowObject enemyFlag;

        /// <summary>
        /// Unique identifier for the enemy's flag carrier.
        /// </summary>
        private ulong EnemyFlagCarrierGuid;

        /// <summary>
        /// Indicates whether the enemy team possesses the flag.
        /// </summary>
        private bool enemyTeamHasFlag = false;

        /// <summary>
        /// Indicates whether the bot has the flag.
        /// </summary>
        private bool hasFlag = false;

        /// <summary>
        /// Indicates whether the bot's state has changed.
        /// </summary>
        private bool hasStateChanged = true;

        /// <summary>
        /// Indicates whether the bot's faction is Horde.
        /// </summary>
        private bool isHorde = false;

        /// <summary>
        /// Represents the player's own flag object.
        /// </summary>
        private IWowObject ownFlag;

        /// <summary>
        /// Indicates whether the bot's team possesses the flag.
        /// </summary>
        private bool ownTeamHasFlag = false;

        /// <summary>
        /// Unique identifier for the bot's team flag carrier.
        /// </summary>
        private ulong TeamFlagCarrierGuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunBoyRunEngine"/> class.
        /// </summary>
        /// <param name="bot">The bot interface providing various bot capabilities.</param>
        public RunBoyRunEngine(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            if (bot.Wow.Events != null)
            {
                bot.Wow.Events.Subscribe("CHAT_MSG_BG_SYSTEM_ALLIANCE", OnFlagAlliance);
                bot.Wow.Events.Subscribe("CHAT_MSG_BG_SYSTEM_HORDE", OnFlagAlliance);
                bot.Wow.Events.Subscribe("CHAT_MSG_BG_SYSTEM_NEUTRAL", OnFlagAlliance);
                bot.Wow.Events.Subscribe("UPDATE_BATTLEFIELD_SCORE", OnFlagAlliance);
            }
        }

        /// <inheritdoc/>
        public string Author => "einTyp";

        /// <inheritdoc/>
        public string Description => "...";

        /// <inheritdoc/>
        public string Name => "RunBoyRunEngine";

        /// <inheritdoc/>
        public void Enter()
        {
            isHorde = Bot.Player.IsHorde();
        }

        /// <inheritdoc/>
        public void Execute()
        {
            if (!IsGateOpen())
            {
                Bot.CombatClass?.OutOfCombatExecute();
                return;
            }

            // --- set new state ---
            if (hasStateChanged)
            {
                hasStateChanged = false;
                hasFlag = Bot.Player.Auras != null && Bot.Player.Auras.Any(e => e.SpellId == 23333 || e.SpellId == 23335);
                IWowUnit teamFlagCarrier = GetTeamFlagCarrier();
                ownTeamHasFlag = teamFlagCarrier != null;
                if (ownTeamHasFlag)
                {
                    TeamFlagCarrierGuid = teamFlagCarrier.Guid;
                }

                IWowUnit enemyFlagCarrier = GetEnemyFlagCarrier();
                enemyTeamHasFlag = enemyFlagCarrier != null;
                if (enemyTeamHasFlag)
                {
                    EnemyFlagCarrierGuid = enemyFlagCarrier.Guid;
                }
            }

            // --- reaction ---
            if (hasFlag)
            {
                // you've got the flag!
                IWowObject tmpFlag = GetOwnFlagObject();
                ownFlag = tmpFlag ?? ownFlag;
                if (ownFlag != null)
                {
                    // own flag lies around
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, ownFlag.Position);
                    if (IsAtPosition(ownFlag.Position))
                    {
                        // own flag reached, save it!
                        Bot.Wow.InteractWithObject(ownFlag);
                        hasStateChanged = true;
                    }
                }
                else
                {
                    // bring it outside!
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseHord : baseAlly);
                }
            }
            else if (ownTeamHasFlag && enemyTeamHasFlag)
            {
                // team mate and enemy got the flag
                if (Bot.CombatClass.Role == WowRole.Dps)
                {
                    // run to the enemy
                    IWowUnit enemyFlagCarrier = Bot.GetWowObjectByGuid<IWowUnit>(EnemyFlagCarrierGuid);
                    if (enemyFlagCarrier != null)
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move,
                            BotUtils.MoveAhead(enemyFlagCarrier.Position, enemyFlagCarrier.Rotation, ((float)Bot.Player.Position.GetDistance2D(enemyFlagCarrier.Position)) / 2f));
                        if (IsInCombatReach(enemyFlagCarrier.Position))
                        {
                            Bot.Wow.ChangeTarget(enemyFlagCarrier.Guid);
                        }

                        if (IsEnemyClose())
                        {
                            // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                            return;
                        }
                    }
                    else
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseAlly : baseHord);
                    }

                    Bot.CombatClass?.OutOfCombatExecute();
                }
                else
                {
                    // run to the own flag carrier
                    IWowUnit teamFlagCarrier = Bot.GetWowObjectByGuid<IWowUnit>(TeamFlagCarrierGuid);
                    if (teamFlagCarrier != null)
                    {
                        if (Bot.CombatClass.Role == WowRole.Dps)
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, BotMath.CalculatePositionBehind(teamFlagCarrier.Position, teamFlagCarrier.Rotation, 1f));
                        }
                        else if (Bot.CombatClass.Role == WowRole.Tank)
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, BotUtils.MoveAhead(teamFlagCarrier.Position, teamFlagCarrier.Rotation, 2f));
                        }
                        else if (Bot.CombatClass.Role == WowRole.Heal)
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, teamFlagCarrier.Position);
                        }

                        if (IsEnemyClose())
                        {
                            // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                            return;
                        }
                    }
                    else
                    {
                        // run to the enemy
                        IWowUnit enemyFlagCarrier = Bot.GetWowObjectByGuid<IWowUnit>(EnemyFlagCarrierGuid);
                        if (enemyFlagCarrier != null)
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move,
                                BotUtils.MoveAhead(enemyFlagCarrier.Position, enemyFlagCarrier.Rotation, ((float)Bot.Player.Position.GetDistance2D(enemyFlagCarrier.Position)) / 2f));
                            if (Bot.CombatClass.Role != WowRole.Heal && IsInCombatReach(enemyFlagCarrier.Position))
                            {
                                Bot.Wow.ChangeTarget(enemyFlagCarrier.Guid);
                            }

                            if (IsEnemyClose())
                            {
                                // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                                return;
                            }
                        }
                        else
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseHord : baseAlly);
                        }

                        Bot.CombatClass?.OutOfCombatExecute();
                    }
                }
            }
            else if (ownTeamHasFlag)
            {
                // a team mate got the flag
                IWowUnit teamFlagCarrier = Bot.GetWowObjectByGuid<IWowUnit>(TeamFlagCarrierGuid);
                if (teamFlagCarrier != null)
                {
                    if (Bot.CombatClass.Role == WowRole.Dps)
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, BotMath.CalculatePositionBehind(teamFlagCarrier.Position, teamFlagCarrier.Rotation, 1f));
                    }
                    else if (Bot.CombatClass.Role == WowRole.Tank)
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, BotUtils.MoveAhead(teamFlagCarrier.Position, teamFlagCarrier.Rotation, 2f));
                    }
                    else if (Bot.CombatClass.Role == WowRole.Heal)
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, teamFlagCarrier.Position);
                    }

                    if (IsEnemyClose())
                    {
                        // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                        return;
                    }
                }
                else
                {
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseHord : baseAlly);
                }

                if (Bot.CombatClass.Role == WowRole.Dps)
                {
                    if (IsEnemyClose())
                    {
                        // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                        return;
                    }
                }
                Bot.CombatClass?.OutOfCombatExecute();
            }
            else if (enemyTeamHasFlag)
            {
                // the enemy got the flag
                if (Bot.CombatClass.Role == WowRole.Tank)
                {
                    IWowObject tmpFlag = GetEnemyFlagObject();
                    enemyFlag = tmpFlag ?? enemyFlag;
                    if (enemyFlag != null)
                    {
                        // flag lies around
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, enemyFlag.Position);
                        if (IsAtPosition(enemyFlag.Position))
                        {
                            // flag reached, save it!
                            hasStateChanged = true;
                            Bot.Wow.InteractWithObject(enemyFlag);
                        }
                    }
                    else
                    {
                        // go outside!
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseAlly : baseHord);
                        Bot.CombatClass?.OutOfCombatExecute();
                    }
                }
                else
                {
                    IWowUnit enemyFlagCarrier = Bot.GetWowObjectByGuid<IWowUnit>(EnemyFlagCarrierGuid);
                    if (enemyFlagCarrier != null)
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move,
                            BotUtils.MoveAhead(enemyFlagCarrier.Position, enemyFlagCarrier.Rotation, ((float)Bot.Player.Position.GetDistance2D(enemyFlagCarrier.Position)) / 2f));
                        if (Bot.CombatClass.Role != WowRole.Heal && IsInCombatReach(enemyFlagCarrier.Position))
                        {
                            Bot.Wow.ChangeTarget(enemyFlagCarrier.Guid);
                        }

                        if (IsEnemyClose())
                        {
                            // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                            return;
                        }
                    }
                    Bot.CombatClass?.OutOfCombatExecute();
                }
            }
            else
            {
                // go and get the enemy flag!!!
                IWowObject tmpFlag = GetEnemyFlagObject();
                enemyFlag = tmpFlag ?? enemyFlag;
                if (enemyFlag != null)
                {
                    // flag lies around
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, enemyFlag.Position);
                    if (IsAtPosition(enemyFlag.Position))
                    {
                        // flag reached, save it!
                        hasStateChanged = true;
                        Bot.Wow.InteractWithObject(enemyFlag);
                    }
                }
                else
                {
                    // go outside!
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseAlly : baseHord);
                    Bot.CombatClass?.OutOfCombatExecute();
                }
            }
            if (Bot.Movement.Status == Movement.Enums.MovementAction.None)
            {
                hasStateChanged = true;
                Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, isHorde ? baseAlly : baseHord);
                if (IsEnemyClose())
                {
                    // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                    return;
                }
                Bot.CombatClass?.OutOfCombatExecute();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
        }

        /// <summary>
        /// Retrieves the enemy flag carrier.
        /// </summary>
        /// <returns>The enemy unit carrying the flag or null if none found.</returns>
        private IWowUnit GetEnemyFlagCarrier()
        {
            List<IWowUnit> flagCarrierList = Bot.Objects.All.OfType<IWowUnit>().Where(e =>
            Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly
            && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Neutral
            && !e.IsDead && e.Guid != Bot.Wow.PlayerGuid
            && e.Auras != null && e.Auras.Any(en =>
            Bot.Db.GetSpellName(en.SpellId).Contains("Flag") || Bot.Db.GetSpellName(en.SpellId).Contains("flag")))
            .ToList();

            if (flagCarrierList.Count > 0)
            {
                return flagCarrierList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the enemy flag game object based on player's faction.
        /// </summary>
        /// <returns>The enemy flag game object or null if not found.</returns>
        private IWowObject GetEnemyFlagObject()
        {
            WowGameObjectDisplayId targetFlag = Bot.Player.IsHorde()
                ? WowGameObjectDisplayId.WsgAllianceFlag : WowGameObjectDisplayId.WsgHordeFlag;

            List<IWowGameobject> flagObjectList = Bot.Objects.All
                .OfType<IWowGameobject>() // only WowGameobjects
                .Where(x => Enum.IsDefined(typeof(WowGameObjectDisplayId), x.DisplayId)
                         && targetFlag == (WowGameObjectDisplayId)x.DisplayId).ToList();

            if (flagObjectList.Count > 0)
            {
                return flagObjectList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the bot's team flag game object based on player's faction.
        /// </summary>
        /// <returns>The team's flag game object or null if not found.</returns>
        private IWowObject GetOwnFlagObject()
        {
            WowGameObjectDisplayId targetFlag = Bot.Player.IsHorde()
                ? WowGameObjectDisplayId.WsgHordeFlag : WowGameObjectDisplayId.WsgAllianceFlag;

            List<IWowGameobject> flagObjectList = Bot.Objects.All
                .OfType<IWowGameobject>() // only WowGameobjects
                .Where(x => Enum.IsDefined(typeof(WowGameObjectDisplayId), x.DisplayId)
                         && targetFlag == (WowGameObjectDisplayId)x.DisplayId).ToList();

            if (flagObjectList.Count > 0)
            {
                return flagObjectList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the bot's team flag carrier.
        /// </summary>
        /// <returns>The friendly unit carrying the flag or null if none found.</returns>
        private IWowUnit GetTeamFlagCarrier()
        {
            List<IWowUnit> flagCarrierList = Bot.Objects.All.OfType<IWowUnit>().Where(e => (Bot.Db.GetReaction(Bot.Player, e) == WowUnitReaction.Friendly || Bot.Db.GetReaction(Bot.Player, e) == WowUnitReaction.Neutral) && !e.IsDead && e.Guid != Bot.Wow.PlayerGuid && e.Auras != null && e.Auras.Any(en => Bot.Db.GetSpellName(en.SpellId).Contains("Flag") || Bot.Db.GetSpellName(en.SpellId).Contains("flag"))).ToList();
            if (flagCarrierList.Count > 0)
            {
                return flagCarrierList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the bot is close to a specific position.
        /// </summary>
        /// <param name="position">The position to check against.</param>
        /// <returns>True if the bot is close to the given position, otherwise false.</returns>
        private bool IsAtPosition(Vector3 position)
        {
            return Bot.Player.Position.GetDistance(position) < (Bot.Player.CombatReach * 0.75f);
        }

        /// <summary>
        /// Checks if an enemy is in close proximity to the bot.
        /// </summary>
        /// <returns>True if an enemy is close, otherwise false.</returns>
        private bool IsEnemyClose()
        {
            return Bot.Objects.All.OfType<IWowUnit>() != null && Bot.Objects.All.OfType<IWowUnit>().Any(e => Bot.Player.Position.GetDistance(e.Position) < 49 && !e.IsDead && !(e.Health < 1) && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Neutral);
        }

        /// <summary>
        /// Determines if the gate is open based on the bot's faction.
        /// </summary>
        /// <returns>True if the gate is open, otherwise false.</returns>
        private bool IsGateOpen()
        {
            if (Bot.Player.IsAlliance())
            {
                IWowGameobject obj = Bot.Objects.All.OfType<IWowGameobject>()
                                    .Where(e => e.GameObjectType == WowGameObjectType.Door && e.DisplayId == 411)
                                    .FirstOrDefault();

                return obj == null || obj.Bytes0 == 0;
            }
            else
            {
                IWowGameobject obj = Bot.Objects.All.OfType<IWowGameobject>()
                                    .Where(e => e.GameObjectType == WowGameObjectType.Door && e.DisplayId == 850)
                                    .FirstOrDefault();

                return obj == null || obj.Bytes0 == 0;
            }
        }

        /// <summary>
        /// Checks if a given position is within the bot's combat reach.
        /// </summary>
        /// <param name="position">The position to check against.</param>
        /// <returns>True if the position is within combat reach, otherwise false.</returns>
        private bool IsInCombatReach(Vector3 position)
        {
            return Bot.Player.Position.GetDistance(position) < 50;
        }

        /// <summary>
        /// Handles events related to the Alliance's flag.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event.</param>
        /// <param name="args">The arguments associated with the event.</param>
        private void OnFlagAlliance(long timestamp, List<string> args)
        {
            hasStateChanged = true;
        }
    }
}