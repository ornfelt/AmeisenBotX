namespace AmeisenBotX.Utils
{
    /// <summary>
    /// Class representing an Idle Action Wrapper.
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