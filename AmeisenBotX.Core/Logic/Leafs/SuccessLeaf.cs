using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Leafs
{
    public class SuccessLeaf(Action action = null, string name = null) : INode
    {
        public string Name { get; } = name;

        private Action Action { get; } = action;

        public BtStatus Execute()
        {
            Action?.Invoke();
            return BtStatus.Success;
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}
