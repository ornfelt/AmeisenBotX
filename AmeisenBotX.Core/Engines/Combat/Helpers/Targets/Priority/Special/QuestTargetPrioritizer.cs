using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Special
{
    /// <summary>
    /// Prioritizes units that are quest objective targets.
    /// This makes the bot recognize quest mobs like the WoW UI does.
    /// </summary>
    public class QuestTargetPrioritizer(AmeisenBotInterfaces bot) : ITargetPrioritizer
    {
        private readonly AmeisenBotInterfaces Bot = bot;

        public bool HasPriority(IWowUnit unit)
        {
            return Bot.IsQuestTarget(unit);
        }
    }
}
