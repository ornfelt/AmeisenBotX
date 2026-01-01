using AmeisenBotX.Core.Engines.Autopilot.Quest;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.MPQ;

namespace AmeisenBotX.Core.Engines.Autopilot
{
    public class AutopilotEngine : IAutopilotEngine
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;
        private readonly DbcBridge Dbc;
        
        private readonly AutopilotManager Manager;
        public QuestPulseEngine QuestPulse { get; }
        public string State => $"{Manager?.CurrentTaskName ?? "Not Initialized"} - {Manager?.DetailedStatus ?? ""}";

        public AutopilotEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config, DbcBridge dbc)
        {
            Bot = bot;
            Config = config;
            Dbc = dbc;

            QuestPulse = new QuestPulseEngine(bot, dbc);
            Manager = new AutopilotManager(bot, config, QuestPulse);
            
            AmeisenLogger.I.Log("Autopilot", "Initialized AutopilotEngine", LogLevel.Debug);
        }

        public void Execute()
        {
            // 1. Analyze Environment (Quests, Mobs, Loot)
            QuestPulse.Update();

            // 2. Decide High-Level Goal
            Manager.UpdateDecision();

            // 3. Execute Behavior
            Manager.ExecuteCurrentTask();
        }
    }
}
