using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Leafs
{
    public class InteractWithGobjectLeaf : MoveToLeaf
    {
        public InteractWithGobjectLeaf(AmeisenBotInterfaces bot, Func<IWowGameobject> getObject, INode child = null, float maxDistance = 3.2f, int interactInterval = 1500, string name = null) : base(bot, getObject, null, maxDistance, name)
        {
            Child = child;
            InteractionEvent = new(TimeSpan.FromMilliseconds(interactInterval));
        }

        private TimegatedEvent InteractionEvent { get; }

        public override BtStatus Execute()
        {
            BtStatus status = base.Execute();

            if (!InteractionEvent.Run())
            {
                return BtStatus.Ongoing;
            }

            if (status == BtStatus.Success)
            {
                // Prevent double-clicking / interrupting self
                if (!Bot.Player.IsCasting)
                {
                    Bot.Wow.InteractWithObject(GetObj());
                }
            }

            return Child?.Execute() ?? BtStatus.Success;
        }
    }
}
