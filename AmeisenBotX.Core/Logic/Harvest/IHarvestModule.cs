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
        /// Check if a GameObject can be harvested by this module.
        /// Only called if module is loaded.
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
