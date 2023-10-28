using AmeisenBotX.Core.Engines.Jobs.Enums;

/// <summary>
/// Represents an interface for a job profile.
/// </summary>
namespace AmeisenBotX.Core.Engines.Jobs.Profiles
{
    /// <summary>
    /// Represents an interface for a job profile.
    /// </summary>
    public interface IJobProfile
    {
        /// <summary>
        /// Gets the JobType of the Job.
        /// </summary>
        JobType JobType { get; }
    }
}