using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Logic.Harvest
{
    /// <summary>
    /// Modular harvest plugin for specific GameObject types.
    /// Modules are loaded dynamically based on character skills.
    /// </summary>
    public interface IHarvestModule
    {
        /// <summary>
        /// Module name for logging and identification.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Check if this module should be loaded for the current character.
        /// Called once during initialization.
        /// </summary>
        /// <returns>True if module applies to this character, false otherwise.</returns>
        bool ShouldLoad(AmeisenBotInterfaces bot);

        /// <summary>
        /// Fast type check - does this object match this module's target type?
        /// Called in Stage 1 filtering. Should be CHEAP (no skill lookups).
        /// Examples: IsOre, IsHerb, IsSparkling, GameObjectType == Chest
        /// </summary>
        /// <param name="gobject">GameObject to check.</param>
        /// <returns>True if object type matches this module.</returns>
        bool Matches(IWowGameobject gobject);

        /// <summary>
        /// Full harvest check - can we actually harvest this object?
        /// Called in Stage 2 after Matches() passes. Can be more expensive.
        /// Checks skills, inventory space, etc.
        /// NOTE: IsUsable is checked globally in HarvestManager, not here.
        /// </summary>
        /// <param name="gobject">GameObject to evaluate.</param>
        /// <returns>True if this module can harvest the object.</returns>
        bool CanHarvest(IWowGameobject gobject);

        /// <summary>
        /// Get priority score for a GameObject (higher = more important).
        /// Used for prioritization when multiple harvestable objects available.
        /// </summary>
        /// <param name="gobject">GameObject to score.</param>
        /// <returns>Priority score (0-1000 range recommended).</returns>
        int GetPriority(IWowGameobject gobject);
    }
}
