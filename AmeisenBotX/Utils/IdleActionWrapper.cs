/// <summary>
/// Contains utility classes and methods for the AmeisenBotX namespace.
/// </summary>
namespace AmeisenBotX.Utils
{
    /// <summary>
    /// Represents a wrapper class for the IdleAction feature.
    /// </summary>
    public class IdleActionWrapper
    {
        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the Name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the name of the object.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}