using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Leafs
{
    public class MoveToLeaf(AmeisenBotInterfaces bot, Func<IWowObject> getObj, INode child = null, float maxDistance = 3.2f, string name = null) : INode
    {
        public string Name { get; } = name;

        protected AmeisenBotInterfaces Bot { get; } = bot;

        protected INode Child { get; set; } = child;

        protected Func<IWowObject> GetObj { get; } = getObj;

        protected float MaxDistance { get; } = maxDistance;

        protected bool NeedToStopMoving { get; set; }

        private Vector3 _lastPosition;
        private DateTime _lastMoveTime;
        private DateTime _stuckStartTime;

        public virtual BtStatus Execute()
        {
            IWowObject unit = GetObj();

            if (unit == null)
            {
                return BtStatus.Failed;
            }

            if (Bot.Player.DistanceTo(unit) > MaxDistance)
            {
                // Stuck Detection Logic
                if (_stuckStartTime == default)
                {
                    _stuckStartTime = DateTime.UtcNow;
                    _lastPosition = Bot.Player.Position;
                }

                if (DateTime.UtcNow - _lastMoveTime > TimeSpan.FromSeconds(1))
                {
                    if (_lastPosition != Vector3.Zero && Bot.Player.Position.GetDistance(_lastPosition) < 0.5f)
                    {
                        if (DateTime.UtcNow - _stuckStartTime > TimeSpan.FromSeconds(3))
                        {
                            // We are stuck (haven't moved 0.5yd in 3 seconds)
                            Bot.Movement.StopMovement();
                            return BtStatus.Failed;
                        }
                    }
                    else
                    {
                        // We moved, reset stuck timer
                        _stuckStartTime = DateTime.UtcNow;
                        _lastPosition = Bot.Player.Position;
                    }

                    _lastMoveTime = DateTime.UtcNow;
                }

                Bot.Movement.SetMovementAction(MovementAction.Move, unit.Position);
                NeedToStopMoving = true;
                return BtStatus.Ongoing;
            }

            // Reset stuck tracking if we reached destination or switched target
            _stuckStartTime = default;
            _lastMoveTime = default;
            _lastPosition = Vector3.Zero;

            if (NeedToStopMoving)
            {
                NeedToStopMoving = false;
                Bot.Movement.StopMovement();
            }

            return Child?.Execute() ?? BtStatus.Success;
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}
