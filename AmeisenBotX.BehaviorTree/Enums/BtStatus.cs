namespace AmeisenBotX.BehaviorTree.Enums
{
    /// <summary>
    /// Represents the status of a behavior tree node.
    /// </summary>
    public enum BtStatus
    {
        /// <summary>
        /// Indicates that the behavior tree node successfully completed its operation.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates that the behavior tree node is still processing its operation.
        /// </summary>
        Ongoing,

        /// <summary>
        /// Indicates that the behavior tree node failed to complete its operation.
        /// </summary>
        Failed
    }
}