/// <summary>
/// Represents a blackboard used for sharing data between different parts of a behavior tree.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Interfaces
{
    /// <summary>
    /// Represents a blackboard used for sharing data between different parts of a behavior tree.
    /// </summary>
    public interface IBlackboard
    {
        /// <summary>
        /// Updates the state or values within the blackboard.
        /// </summary>
        void Update();
    }
}