using AmeisenBotX.Core.Engines.Jobs.Profiles;
using System.Collections.Generic;

/// <summary>
/// Represents a job engine that allows entering, executing, and resetting jobs.
/// </summary>
namespace AmeisenBotX.Core.Engines.Jobs
{
    /// <summary>
    /// Represents a job engine that allows entering, executing, and resetting jobs.
    /// </summary>
    public interface IJobEngine
    {
        /// <summary>
        /// Gets or sets the list of blacklisted nodes as positive integers.
        /// </summary>
        List<ulong> NodeBlacklist { get; set; }

        /// <summary>
        /// Gets or sets the job profile.
        /// </summary>
        IJobProfile Profile { get; set; }

        /// <summary>
        ///  Enters the specified location.
        /// </summary>
        void Enter();

        /// <summary>
        /// Executes the specified action.
        /// </summary>
        void Execute();

        /// <summary>
        /// Resets the state of the object to its default values.
        /// </summary>
        void Reset();
    }
}