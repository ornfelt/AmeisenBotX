using AmeisenBotX.Core.Engines.AI;
using AmeisenBotX.Core.Engines.Combat.Classes.Jannis;

namespace AmeisenBotX.Core.Engines.Combat.Classes
{
    /// <summary>
    /// Base class for all AI-powered combat classes.
    /// Provides infrastructure for spell categorization and tactical AI decision making.
    /// </summary>
    public abstract class BasicAiCombatClass : BasicCombatClass
    {
        public BasicAiCombatClass(AmeisenBotInterfaces bot) : base(bot)
        {
        }

        #region AI Tools

        /// <summary>
        /// Registers a spell with a specific AI category manually.
        /// Useful if the automatic classification is missing something or needs override.
        /// </summary>
        protected void RegisterSpellCategory(string spellName, AiSpellCategory category)
        {
            AiSpellClassifier.Register(spellName, category);
        }

        /// <summary>
        /// Gets the category of a spell.
        /// </summary>
        protected AiSpellCategory GetSpellCategory(string spellName)
        {
            return AiSpellClassifier.Classify(spellName);
        }

        /// <summary>
        /// Checks if the spell matches the requested AI category.
        /// </summary>
        protected bool IsSpellCategory(string spellName, AiSpellCategory category)
        {
            return GetSpellCategory(spellName) == category;
        }

        /// <summary>
        /// Example AI Query: "Should I heal myself now?"
        /// (This will later connect to the Neural Network Action Classifier)
        /// </summary>
        protected bool AiSuggests(AiSpellCategory category)
        {
            // 1. Ask the Tactical Brain
            if (Bot.CombatAi?.ActionClassifier != null && Bot.CombatAi.Analyzer != null)
            {
                // We pass the strategic context (Analyzer snapshot) + Bot (for cooldowns)
                // Note: We need to modify PredictAction to accept Bot too, or handle it inside
                // Ideally: 
                // var suggestion = bot.CombatAi.ActionClassifier.PredictAction(bot.CombatAi.Analyzer.LastSnapshot);

                // If suggestion is Unknown, fall back to Heuristic
                // if (suggestion != AiSpellCategory.Unknown) return suggestion == category;
            }

            // Fallback: Heuristic Logic based on Strategy
            switch (AiStrategy)
            {
                case Movement.AI.AiCombatStrategy.Survival:
                    return category == AiSpellCategory.HealSelf || category == AiSpellCategory.DefensiveCooldown || category == AiSpellCategory.Utility;

                case Movement.AI.AiCombatStrategy.Burst:
                    return category == AiSpellCategory.BurstCooldown || category == AiSpellCategory.Damage;

                case Movement.AI.AiCombatStrategy.Flee:
                    // Recommend CC (to peel) or Utility (Speed boosts) or Defensive Cooldowns
                    return category == AiSpellCategory.CrowdControl
                        || category == AiSpellCategory.Utility
                        || category == AiSpellCategory.DefensiveCooldown;

                default:
                    return true; // If standard, allow everything
            }
        }

        #endregion
    }
}
