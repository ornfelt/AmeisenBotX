using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Leafs
{
    public class InteractWithUnitLeaf : MoveToLeaf
    {
        /// <summary>
        /// Initializes a new instance of the InteractWithUnitLeaf class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="getUnit">The function to retrieve the WowUnit object.</param>
        /// <param name="child">The child node.</param>
        /// <param name="maxDistance">The maximum distance for the interaction.</param>
        /// <param name="interactInterval">The interval in milliseconds for the interaction event.</param>
        public InteractWithUnitLeaf(AmeisenBotInterfaces bot, Func<IWowUnit> getUnit, INode child = null, float maxDistance = 3.2f, int interactInterval = 1500) : base(bot, getUnit, null, maxDistance)
        {
            Child = child;
            InteractionEvent = new(TimeSpan.FromMilliseconds(interactInterval));
        }

        /// <summary>
        /// Gets the time-gated interaction event.
        /// </summary>
        private TimegatedEvent InteractionEvent { get; }

        /// <summary>
        /// Executes the behavior tree node.
        /// </summary>
        /// <returns>The status of the execution.</returns>
        public override BtStatus Execute()
        {
            BtStatus status = base.Execute();

            if (!InteractionEvent.Run())
            {
                return BtStatus.Ongoing;
            }

            if (status == BtStatus.Success)
            {
                Bot.Wow.InteractWithUnit(GetUnit());
            }

            return Child?.Execute() ?? BtStatus.Success;
        }
    }
}