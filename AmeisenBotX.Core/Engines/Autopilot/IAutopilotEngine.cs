using AmeisenBotX.Core.Engines.Autopilot.Quest;

namespace AmeisenBotX.Core.Engines.Autopilot
{
    public interface IAutopilotEngine
    {
        QuestPulseEngine QuestPulse { get; }
        void Execute();
    }
}
