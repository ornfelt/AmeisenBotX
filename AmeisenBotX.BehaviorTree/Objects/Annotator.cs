using AmeisenBotX.BehaviorTree.Enums;

/// <summary>
/// Namespace containing objects related to the BehaviorTree, including the Annotator node.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// BehaviorTree Node that executes a node before executing annother node. Use this to update
    /// stuff before executing a node.
    /// </summary>
    public class Annotator : INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotator"/> class.
        /// </summary>
        /// <param name="annotationNode">The node to be executed before the child node.</param>
        /// <param name="child">The main node to be executed after the annotation node.</param>
        public Annotator(INode annotationNode, INode child) : base()
        {
            AnnotationNode = annotationNode;
            Child = child;
        }

        /// <summary>
        /// Gets or sets the annotation node that gets executed prior to the child node.
        /// </summary>
        public INode AnnotationNode { get; set; }

        /// <summary>
        /// Gets or sets the main child node to execute.
        /// </summary>
        public INode Child { get; set; }

        /// <summary>
        /// Executes the annotation node, followed by the child node.
        /// </summary>
        /// <returns>The status of the child node execution.</returns>
        public BtStatus Execute()
        {
            AnnotationNode.Execute();
            return Child.Execute();
        }

        /// <summary>
        /// Retrieves the main child node to be executed.
        /// </summary>
        /// <returns>The child node.</returns>
        public INode GetNodeToExecute()
        {
            return Child;
        }
    }

    /// <summary>
    /// Represents a BehaviorTree node that executes an annotation node prior to executing another node, 
    /// with an associated blackboard of type <typeparamref name="T"/>.
    /// This can be used to perform setup or precondition checks before actual execution.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard.</typeparam>
    public class Annotator<T> : INode<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotator{T}"/> class.
        /// </summary>
        /// <param name="annotationNode">The node to be executed before the child node.</param>
        /// <param name="child">The main node to be executed after the annotation node.</param>
        public Annotator(INode<T> annotationNode, INode<T> child) : base()
        {
            AnnotationNode = annotationNode;
            Child = child;
        }

        /// <summary>
        /// Gets or sets the annotation node that gets executed prior to the child node.
        /// </summary>
        public INode<T> AnnotationNode { get; set; }

        /// <summary>
        /// Gets or sets the main child node to execute.
        /// </summary>
        public INode<T> Child { get; set; }

        /// <summary>
        /// Executes the annotation node using the provided blackboard, followed by the child node.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The status of the child node execution.</returns>
        public BtStatus Execute(T blackboard)
        {
            AnnotationNode.Execute(blackboard);
            return Child.Execute(blackboard);
        }

        /// <summary>
        /// Retrieves the main child node to be executed.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The child node.</returns>
        public INode<T> GetNodeToExecute(T blackboard)
        {
            return Child;
        }
    }

}