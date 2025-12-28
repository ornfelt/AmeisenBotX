using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    public class Leaf(Func<BtStatus> behaviorTreeAction, string name = null) : INode
    {
        public Func<BtStatus> BehaviorTreeAction { get; set; } = behaviorTreeAction;

        public string Name { get; } = name;

        public BtStatus Execute()
        {
            return BehaviorTreeAction();
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }

    public class Leaf<T>(Func<T, BtStatus> behaviorTreeAction, string name = null) : INode<T>
    {
        public Func<T, BtStatus> BehaviorTreeAction { get; set; } = behaviorTreeAction;

        public string Name { get; } = name;

        public BtStatus Execute(T blackboard)
        {
            return BehaviorTreeAction(blackboard);
        }

        public INode<T> GetNodeToExecute(T blackboard)
        {
            return this;
        }
    }
}
