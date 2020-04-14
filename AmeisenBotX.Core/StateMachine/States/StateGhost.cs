﻿using AmeisenBotX.Core.Common;
using AmeisenBotX.Core.Data.Enums;
using AmeisenBotX.Core.Data.Objects.WowObject;
using AmeisenBotX.Core.Movement.Enums;
using AmeisenBotX.Pathfinding.Objects;
using System.Linq;

namespace AmeisenBotX.Core.Statemachine.States
{
    public class StateGhost : BasicState
    {
        public StateGhost(AmeisenBotStateMachine stateMachine, AmeisenBotConfig config, WowInterface wowInterface) : base(stateMachine, config, wowInterface)
        {
        }

        public Vector3 CorpsePosition { get; private set; }

        public bool NeedToEnterPortal { get; private set; }

        public override void Enter()
        {
            // WowUnit spiritHealer = WowInterface.ObjectManager.WowObjects.OfType<WowUnit>().FirstOrDefault(e => e.Name.ToUpper().Contains("SPIRIT HEALER"));
            //
            // if (spiritHealer != null)
            // {
            //     WowInterface.HookManager.UnitOnRightClick(spiritHealer);
            // }
        }

        public override void Execute()
        {
            if (WowInterface.ObjectManager.Player.Health > 1)
            {
                StateMachine.SetState(BotState.Idle);
            }

            WowGameobject nearestPortal = WowInterface.ObjectManager.WowObjects
                .OfType<WowGameobject>()
                .Where(e => e.DisplayId == (int)GameobjectDisplayId.DungeonPortalNormal
                            || e.DisplayId == (int)GameobjectDisplayId.DungeonPortalHeroic)
                .FirstOrDefault(e => e.Position.GetDistance(WowInterface.ObjectManager.Player.Position) < 40);

            if (nearestPortal != null)
            {
                NeedToEnterPortal = true;
                CorpsePosition = nearestPortal.Position;
            }

            if (StateMachine.IsDungeonMap(StateMachine.MapIDiedOn))
            {
                CorpsePosition = WowInterface.DungeonEngine.DungeonProfile.WorldEntry;
                NeedToEnterPortal = true;
            }
            else if (StateMachine.IsBattlegroundMap(WowInterface.ObjectManager.MapId))
            {
                // just wait for the mass ress
                return;
            }
            else if (!NeedToEnterPortal)
            {
                WowInterface.XMemory.ReadStruct(WowInterface.OffsetList.CorpsePosition, out Vector3 corpsePosition);
                CorpsePosition = corpsePosition;
            }

            if (WowInterface.ObjectManager.Player.Position.GetDistance(CorpsePosition) > 8)
            {
                WowInterface.MovementEngine.SetState(MovementEngineState.Moving, CorpsePosition);
                WowInterface.MovementEngine.Execute();
            }
            else
            {
                if (NeedToEnterPortal)
                {
                    // move into portal
                    CorpsePosition = BotUtils.MoveAhead(BotMath.GetFacingAngle(WowInterface.ObjectManager.Player.Position, CorpsePosition), CorpsePosition, 4);
                    WowInterface.MovementEngine.SetState(MovementEngineState.Moving, CorpsePosition);
                    WowInterface.MovementEngine.Execute();
                }
                else
                {
                    WowInterface.HookManager.RetrieveCorpse();
                }
            }
        }

        public override void Exit()
        {
            CorpsePosition = default;
            NeedToEnterPortal = false;
        }
    }
}