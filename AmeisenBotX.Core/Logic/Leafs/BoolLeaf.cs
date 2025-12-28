using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Leafs
{
    public class BoolLeaf(Func<bool> action = null, string name = null) : INode
    {
        public string Name { get; } = name;

        private Func<bool> Action { get; } = action;

        public BtStatus Execute()
        {
            return Action?.Invoke() == true ? BtStatus.Success : BtStatus.Failed;
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}
