using AmeisenBotX.Wow.Objects.Enums;

/// <summary>
/// This namespace contains classes and interfaces related to World of Warcraft objects for the AmeisenBotX project.
/// </summary>
namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a dynamic object in the World of Warcraft game.
    /// </summary>
    public interface IWowDynobject : IWowObject
    {
        /// <summary>
        /// Gets or sets the Caster property which is of type ulong.
        /// </summary>
        ulong Caster { get; }

        /// <summary>
        /// Gets the radius of the circle.
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// Gets the spell ID.
        /// </summary>
        int SpellId { get; }

        /// <summary>
        /// Gets or sets the type of the WowObject.
        /// </summary>
        public new WowObjectType Type => WowObjectType.DynamicObject;
    }
}