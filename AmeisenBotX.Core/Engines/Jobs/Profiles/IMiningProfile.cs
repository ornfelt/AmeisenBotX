using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Jobs.Profiles
{
    /// <summary>
    /// Represents a mining profile for a character.
    /// </summary>
    public interface IMiningProfile : IJobProfile
    {
        /// <summary>
        /// Gets a value indicating whether the path is a circle.
        /// </summary>
        bool IsCirclePath { get; }

        /// <summary>
        /// Gets the list of mailbox nodes.
        /// </summary>
        List<Vector3> MailboxNodes { get; }

        /// <summary>
        /// Gets the list of WowOreId ore types.
        /// </summary>
        List<WowOreId> OreTypes { get; }

        /// <summary>
        /// Gets the list of Vector3 representing a path.
        /// </summary>
        List<Vector3> Path { get; }
    }
}