using AmeisenBotX.Core.Engines.Movement.AI;

namespace AmeisenBotX.Core.Engines.AI
{
    public interface ICombatAi
    {
        /// <summary>
        /// Gets the current strategy predicted by the AI.
        /// </summary>
        AiCombatStrategy CurrentStrategy { get; }

        CombatStateAnalyzer Analyzer { get; } // For Visualization

        CombatActionClassifier ActionClassifier { get; } // Tactical Brain
    }
}
