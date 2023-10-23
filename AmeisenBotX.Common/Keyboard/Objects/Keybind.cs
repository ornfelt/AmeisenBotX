namespace AmeisenBotX.Common.Keyboard.Objects
{
    /// <summary>
    /// Represents a combination of a key and its associated modifier for binding purposes.
    /// </summary>
    public struct Keybind
    {
        /// <summary>
        /// Gets or sets the primary key for the binding.
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets the modifier key associated with the primary key.
        /// </summary>
        /// <remarks>
        /// Modifiers typically include keys like Alt, Ctrl, and Shift.
        /// </remarks>
        public int Mod { get; set; }
    }
}