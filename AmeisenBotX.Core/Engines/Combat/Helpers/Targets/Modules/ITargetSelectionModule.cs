using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules
{
    /// <summary>
    /// Modular target selection plugin for context-aware target prioritization.
    /// Modules are evaluated dynamically based on combat context (solo/group/dungeon/PvP).
    /// </summary>
    public interface ITargetSelectionModule
    {
        /// <summary>
        /// Module name for logging and identification.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Check if this module should be active in the current combat context.
        /// Called before GetPriorityBonus to avoid unnecessary calculations.
        /// </summary>
        /// <param name="bot">Bot interfaces for context checking.</param>
        /// <returns>True if module applies to current context, false otherwise.</returns>
        bool IsActive(AmeisenBotInterfaces bot);

        /// <summary>
        /// Calculate priority bonus for a target.
        /// Higher values = higher priority. Negative values discourage targeting.
        /// </summary>
        /// <param name="target">Unit to evaluate.</param>
        /// <param name="bot">Bot interfaces for context.</param>
        /// <returns>Priority bonus score (typically 0-100 range, -1000 for forbidden targets).</returns>
        float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot);
    }
}
