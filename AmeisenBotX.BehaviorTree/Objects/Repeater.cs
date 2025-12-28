using AmeisenBotX.BehaviorTree.Enums;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Decorator that repeats child execution a specified number of times or until failure.
    /// </summary>
    public class Repeater(int maxIterations, INode child) : INode
    {
        public string Name { get; } = null;

        public INode Child { get; } = child;

        public int MaxIterations { get; } = maxIterations;

        public int CurrentIteration { get; private set; }

        public BtStatus Execute()
        {
            if (CurrentIteration >= MaxIterations)
            {
                CurrentIteration = 0;
                return BtStatus.Success;
            }

            BtStatus result = Child.Execute();

            switch (result)
            {
                case BtStatus.Success:
                    CurrentIteration++;
                    return CurrentIteration >= MaxIterations ? BtStatus.Success : BtStatus.Ongoing;

                case BtStatus.Failed:
                    CurrentIteration = 0;
                    return BtStatus.Failed;

                default:
                    return BtStatus.Ongoing;
            }
        }

        public INode GetNodeToExecute() => this;

        /// <summary>
        /// Resets the iteration counter.
        /// </summary>
        public void Reset() => CurrentIteration = 0;
    }

    /// <summary>
    /// Blackboard-aware repeater decorator.
    /// </summary>
    public class Repeater<T>(int maxIterations, INode<T> child) : INode<T>
    {
        public string Name { get; } = null;

        public INode<T> Child { get; } = child;

        public int MaxIterations { get; } = maxIterations;

        public int CurrentIteration { get; private set; }

        public BtStatus Execute(T blackboard)
        {
            if (CurrentIteration >= MaxIterations)
            {
                CurrentIteration = 0;
                return BtStatus.Success;
            }

            BtStatus result = Child.Execute(blackboard);

            switch (result)
            {
                case BtStatus.Success:
                    CurrentIteration++;
                    return CurrentIteration >= MaxIterations ? BtStatus.Success : BtStatus.Ongoing;

                case BtStatus.Failed:
                    CurrentIteration = 0;
                    return BtStatus.Failed;

                default:
                    return BtStatus.Ongoing;
            }
        }

        public INode<T> GetNodeToExecute(T blackboard) => this;

        public void Reset() => CurrentIteration = 0;
    }
}

