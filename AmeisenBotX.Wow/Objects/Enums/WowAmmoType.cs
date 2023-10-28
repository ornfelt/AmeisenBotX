/// <summary>
/// Represents the type of ammunition used in the World of Warcraft game.
/// </summary>
namespace AmeisenBotX.Wow.Objects.Enums
{
    /// <summary>
    /// Represents the type of ammunition used in the World of Warcraft game.
    /// </summary>
    /// <remarks>
    /// The possible values for this enumeration include:
    /// <list type="bullet">
    /// <item><see cref="None"/>: No ammunition is used.</item>
    /// <item><see cref="Bolts"/>: Bolts are used as ammunition.</item>
    /// <item><see cref="Arrows"/>: Arrows are used as ammunition.</item>
    /// <item><see cref="Bullets"/>: Bullets are used as ammunition.</item>
    /// <item><see cref="Thrown"/>: Thrown weapons are used as ammunition.</item>
    /// </list>
    /// </remarks>
    internal enum WowAmmoType
    {
        None = 0,
        Bolts = 1,
        Arrows = 2,
        Bullets = 3,
        Thrown = 4
    }
}