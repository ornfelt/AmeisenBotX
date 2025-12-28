using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    /// <summary>
    /// Service for follow decisions.
    /// Determines if a follow target is configured and available.
    /// </summary>
    public class FollowService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;

        public bool ShouldFollow()
        {
            // Simple checks based on configuration
            return (Config.FollowGroupLeader && Bot.Objects.Partyleader != null) || (Config.FollowGroupMembers && Bot.Objects.Partymembers.Any()) || (Config.FollowSpecificCharacter && !string.IsNullOrEmpty(Config.SpecificCharacterToFollow));
        }
    }
}
