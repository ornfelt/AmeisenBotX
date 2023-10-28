using AmeisenBotX.BehaviorTree.Enums;
using System;

/// <summary>
/// Namespace containing objects related to the behavior tree of the AmeisenBotX.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Special selector that executes nodeNone when input is 0|0, nodeA when input is 1|0, nodeB
    /// when input is 0|1 and nodeBoth when input is 1|1.
    /// </summary>
    public class DualSelector : IComposite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualSelector"/> class.
        /// </summary>
        /// <param name="conditionA">First condition to evaluate.</param>
        /// <param name="conditionB">Second condition to evaluate.</param>
        /// <param name="nodeNone">Node to execute when both conditions are false.</param>
        /// <param name="nodeA">Node to execute when the first condition is true and the second is false.</param>
        /// <param name="nodeB">Node to execute when the first condition is false and the second is true.</param>
        /// <param name="nodeBoth">Node to execute when both conditions are true.</param>
        public DualSelector(Func<bool> conditionA, Func<bool> conditionB, INode nodeNone, INode nodeA, INode nodeB, INode nodeBoth) : base()
        {
            ConditionA = conditionA;
            ConditionB = conditionB;
            Children = new INode[] { nodeNone, nodeA, nodeB, nodeBoth };
        }

        /// <summary>
        /// Gets the array of child nodes.
        /// </summary>
        public INode[] Children { get; }

        /// <summary>
        /// Gets the first condition to evaluate.
        /// </summary>
        public Func<bool> ConditionA { get; }

        /// <summary>
        /// Gets the second condition to evaluate.
        /// </summary>
        public Func<bool> ConditionB { get; }

        /// <summary>
        /// Executes the appropriate child node based on the evaluation of the conditions.
        /// </summary>
        /// <returns>The status of the executed node.</returns>
        public BtStatus Execute()
        {
            return GetNodeToExecute().Execute();
        }

        /// <summary>
        /// Determines the appropriate child node to execute based on the evaluation of the conditions.
        /// </summary>
        /// <returns>The chosen child node.</returns>
        public INode GetNodeToExecute()
        {
            if (ConditionA() && ConditionB())
            {
                return Children[3];
            }
            else if (ConditionA() && !ConditionB())
            {
                return Children[1];
            }
            else if (!ConditionA() && ConditionB())
            {
                return Children[2];
            }
            else
            {
                return Children[0];
            }
        }
    }

    /// <summary>
    /// A specialized selector with an associated blackboard that chooses a child node to execute 
    /// based on two binary conditions. Executes 'nodeNone' when both conditions are false, 'nodeA' 
    /// when the first is true and the second is false, 'nodeB' when the first is false and the 
    /// second is true, and 'nodeBoth' when both conditions are true.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard.</typeparam>
    public class DualSelector<T> : IComposite<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualSelector{T}"/> class.
        /// </summary>
        /// <param name="conditionA">First condition to evaluate, with the blackboard as an argument.</param>
        /// <param name="conditionB">Second condition to evaluate, with the blackboard as an argument.</param>
        /// <param name="nodeNone">Node to execute when both conditions are false.</param>
        /// <param name="nodeA">Node to execute when the first condition is true and the second is false.</param>
        /// <param name="nodeB">Node to execute when the first condition is false and the second is true.</param>
        /// <param name="nodeBoth">Node to execute when both conditions are true.</param>
        public DualSelector(Func<T, bool> conditionA, Func<T, bool> conditionB, INode<T> nodeNone, INode<T> nodeA, INode<T> nodeB, INode<T> nodeBoth) : base()
        {
            ConditionA = conditionA;
            ConditionB = conditionB;
            Children = new INode<T>[] { nodeNone, nodeA, nodeB, nodeBoth };
        }

        /// <summary>
        /// Gets the array of child nodes.
        /// </summary>
        public INode<T>[] Children { get; }

        /// <summary>
        /// Gets the first condition to evaluate, with the blackboard as an argument.
        /// </summary>
        public Func<T, bool> ConditionA { get; }

        /// <summary>
        /// Gets the second condition to evaluate, with the blackboard as an argument.
        /// </summary>
        public Func<T, bool> ConditionB { get; }

        /// <summary>
        /// Executes the appropriate child node based on the evaluation of the conditions using the provided blackboard.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The status of the executed node.</returns>
        public BtStatus Execute(T blackboard)
        {
            return GetNodeToExecute(blackboard).Execute(blackboard);
        }

        /// <summary>
        /// Determines the appropriate child node to execute based on the evaluation of the conditions using the provided blackboard.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The chosen child node.</returns>
        public INode<T> GetNodeToExecute(T blackboard)
        {
            if (ConditionA(blackboard) && ConditionB(blackboard))
            {
                return Children[3];
            }
            else if (ConditionA(blackboard) && !ConditionB(blackboard))
            {
                return Children[1];
            }
            else if (!ConditionA(blackboard) && ConditionB(blackboard))
            {
                return Children[2];
            }
            else
            {
                return Children[0];
            }
        }
    }
}