using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Flags;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;
using System.Linq;
using AmeisenBotX.Core.Engines.Autopilot.Quest;

namespace AmeisenBotX.Core.Engines.Autopilot.Services
{
    public class QuestDiscoveryService
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly QuestPulseEngine QuestPulse;

        public QuestDiscoveryService(AmeisenBotInterfaces bot, QuestPulseEngine questPulse)
        {
            Bot = bot;
            QuestPulse = questPulse;
        }

        public List<IWowUnit> GetNearbyQuestGivers(float radius = 100f)
        {
            if (Bot.Player == null) return new List<IWowUnit>();

            return Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(u => u.Position.GetDistance(Bot.Player.Position) <= radius)
                // Must have the Questgiver NpcFlag set
                .Where(u => u.NpcFlags[(int)WowUnitNpcFlag.Questgiver])
                // Exclude class trainers - they only have class-specific quests which are rarely essential
                // The turn-in ignore list will handle any edge cases where we interact with wrong NPCs
                .Where(u => !u.NpcFlags[(int)WowUnitNpcFlag.ClassTrainer])
                // QuestGiverStatus offset (0xBC) is wrong for 3.3.5a
                // We rely on NpcFlag + interaction result filtering instead
                .ToList();
        }

        public List<IWowUnit> GetAvailableQuestGivers(float radius = 100f)
        {
            // For now, any questgiver that isn't already a turn-in candidate
            // may have available quests. We refine this via interaction.
            var questGivers = GetNearbyQuestGivers(radius);
            var completedQuestIds = QuestPulse.ActiveQuests
                .Where(q => q.IsComplete)
                .Select(q => q.Id)
                .ToList();

            // Simple filter: if they are a questgiver, they are a candidate.
            return questGivers;
        }
    }
}
