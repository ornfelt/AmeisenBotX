using AmeisenBotX.Wow.Objects;

/// <summary>
/// Represents an interface for validating the target.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation
{
    /// <summary>
    /// Represents an interface for validating the target.
    /// </summary>
    /// <param name="unit">The target unit to be validated.</param>
    /// <returns>True if the target is valid; otherwise, false.</returns>
    public interface ITargetValidator
    {
        /// <summary>
        /// Checks if the provided WoW unit is valid.
        /// </summary>
        bool IsValid(IWowUnit unit);
    }
}