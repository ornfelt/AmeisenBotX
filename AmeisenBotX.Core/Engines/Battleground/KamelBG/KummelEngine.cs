using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Battleground.KamelBG
{
    /// <summary>
    /// Represents an implementation of the <see cref="IBattlegroundEngine"/> interface for the Kummel battleground.
    /// </summary>
    internal class KummelEngine : IBattlegroundEngine
    {
        /// <summary>
        /// Represents the own team flag value.
        /// </summary>
        private const int OWN_TEAM_FLAG = 2;

        /// <summary>
        /// Represents the flag state when picked up.
        /// </summary>
        private const int PICKED_UP = 1;

        /// <summary>
        /// Represents the coordinates of the allied exit point.
        /// </summary>
        private Vector3 ausgangAlly = new(1055, 1395, 340);

        /// <summary>
        /// Represents the coordinates of the Horde exit point.
        /// </summary>
        private Vector3 ausgangHord = new(1051, 1398, 340);

        /// <summary>
        /// Represents the coordinates of the allied base.
        /// </summary>
        private Vector3 baseAlly = new(1539, 1481, 352);

        /// <summary>
        /// Represents the coordinates of the Horde base.
        /// </summary>
        private Vector3 baseHord = new(916, 1434, 346);

        /// <summary>
        /// Represents the flag carrier unit.
        /// </summary>
        private IWowUnit FlagCarrier;

        /// <summary>
        /// Represents the flag object.
        /// </summary>
        private IWowObject FlagObject;

        /// <summary>
        /// Represents the flag state.
        /// </summary>
        private int FlagState = 0;

        /// <summary>
        /// Represents a flag indicating whether the character has a flag.
        /// </summary>
        private bool hasFlag = false;

        /// <summary>
        /// Represents a flag indicating whether there has been a state change.
        /// </summary>
        private bool hasStateChanged = true;

        /// <summary>
        /// Represents the starting position.
        /// </summary>
        private Vector3 startPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="KummelEngine"/> class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public KummelEngine(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            bot.Wow.Events.Subscribe("CHAT_MSG_BG_SYSTEM_ALLIANCE", OnFlagAlliance);
            bot.Wow.Events.Subscribe("CHAT_MSG_BG_SYSTEM_HORDE", OnFlagAlliance);
            bot.Wow.Events.Subscribe("CHAT_MSG_BG_SYSTEM_NEUTRAL", OnFlagAlliance);
        }

        /// <inheritdoc />
        public string Author => "Kamel";

        /// <inheritdoc />
        public string Description => "...";

        /// <inheritdoc />
        public string Name => "Kummel Engine";

        /// <summary>
        /// Gets the AmeisenBotInterfaces instance.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <inheritdoc />
        public void Enter()
        {
        }

        /// <inheritdoc />
        public void Execute()
        {
            Bot.CombatClass?.OutOfCombatExecute();
            if (Bot.Player.IsCasting)
            {
                return;
            }
            // set new state
            if (hasStateChanged)
            {
                hasStateChanged = false;
                //hasFlag = Bot.NewBot.GetBuffs(WowLuaUnit.Player).Any(e => e.Contains("flag") || e.Contains("Flag"));
                hasFlag = Bot.Player.Auras != null && Bot.Player.Auras.Any(e => e.SpellId == 23333 || e.SpellId == 23335);
                FlagCarrier = hasFlag ? Bot.Player : GetFlagCarrier();
                if (FlagCarrier == null)
                {
                    FlagObject = GetFlagObject();
                    if (FlagObject == null)
                    {
                        hasStateChanged = true;
                    }
                    else
                    {
                        FlagState = 0;
                        Bot.Wow.SendChatMessage("/y The flag just lies around! Let's take it!");
                    }
                }
                else
                {
                    FlagState = PICKED_UP;
                    if (hasFlag || Bot.Db.GetReaction(Bot.Player, FlagCarrier) == WowUnitReaction.Friendly || Bot.Db.GetReaction(Bot.Player, FlagCarrier) == WowUnitReaction.Neutral)
                    {
                        FlagState |= OWN_TEAM_FLAG;
                        Bot.Wow.SendChatMessage("/y We got it!");
                    }
                    else
                    {
                        Bot.Wow.SendChatMessage("/y They've got the flag!");
                    }
                }
            }

            // reaction
            if ((FlagState & PICKED_UP) > 0)
            {
                if ((FlagState & OWN_TEAM_FLAG) > 0)
                {
                    // own team has flag
                    if (hasFlag)
                    {
                        IWowObject ownFlag = GetFlagObject();
                        if (ownFlag != null)
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, ownFlag.Position);
                            if (Bot.Player.Position.GetDistance(ownFlag.Position) < 3.5f)
                            {
                                Bot.Wow.InteractWithObject(FlagObject);
                            }
                        }
                        else
                        {
                            IWowUnit enemyFlagCarrier = GetFlagCarrier();
                            if (enemyFlagCarrier != null)
                            {
                                Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, enemyFlagCarrier.Position);
                            }
                            else if (startPosition != default)
                            {
                                Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, ausgangAlly);
                            }
                        }
                    }
                    else if (FlagCarrier != null)
                    {
                        FlagCarrier = GetFlagCarrier();
                        if (FlagCarrier != null)
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, FlagCarrier.Position);
                        }
                        else
                        {
                            Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, baseHord);
                        }
                    }
                }
                else
                {
                    // enemy team has flag
                    FlagCarrier = GetFlagCarrier();
                    if (FlagCarrier != null)
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, FlagCarrier.Position);
                    }
                    else
                    {
                        Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, baseHord);
                    }
                }
            }
            else if (FlagObject != null)
            {
                // flag lies on the ground
                Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, FlagObject.Position);
                if (Bot.Player.Position.GetDistance(FlagObject.Position) < 3.5f) // limit the executions
                {
                    Bot.Wow.InteractWithObject(FlagObject);
                }
            }
            else if (startPosition != default)
            {
                Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, ausgangHord);
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
        }

        /// <summary>
        /// Retrieves the flag carrier unit.
        /// </summary>
        /// <returns>The flag carrier unit or null if none found.</returns>
        private IWowUnit GetFlagCarrier()
        {
            List<IWowUnit> flagCarrierList = Bot.Objects.All.OfType<IWowUnit>().Where(e => e.Guid != Bot.Wow.PlayerGuid && e.Auras != null && e.Auras.Any(en => Bot.Db.GetSpellName(en.SpellId).Contains("Flag") || Bot.Db.GetSpellName(en.SpellId).Contains("flag"))).ToList();
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
        /// Retrieves the flag object based on the current flag state.
        /// </summary>
        /// <returns>The flag object or null if none found.</returns>
        private IWowObject GetFlagObject()
        {
            WowGameObjectDisplayId targetFlag = hasFlag
                ? (Bot.Player.IsHorde() ? WowGameObjectDisplayId.WsgHordeFlag : WowGameObjectDisplayId.WsgAllianceFlag)
                : (Bot.Player.IsHorde() ? WowGameObjectDisplayId.WsgAllianceFlag : WowGameObjectDisplayId.WsgHordeFlag);

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
        /// Event handler for flag-related chat messages.
        /// </summary>
        /// <param name="timestamp">The timestamp of the chat message.</param>
        /// <param name="args">The list of arguments from the chat message.</param>
        private void OnFlagAlliance(long timestamp, List<string> args)
        {
            hasStateChanged = true;
            if (startPosition == default)
            {
                startPosition = Bot.Player.Position;
            }
        }
    }
}