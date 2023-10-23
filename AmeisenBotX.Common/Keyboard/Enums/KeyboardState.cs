namespace AmeisenBotX.Common.Keyboard.Enums
{
    /// <summary>
    /// Represents the possible states of a keyboard key.
    /// </summary>
    internal enum KeyboardState
    {
        /// <summary>
        /// Indicates that a key has been pressed down.
        /// </summary>
        KeyDown = 0x0100,

        /// <summary>
        /// Indicates that a key has been released.
        /// </summary>
        KeyUp = 0x0101,

        /// <summary>
        /// Indicates that a system key (e.g., Alt) has been pressed down.
        /// </summary>
        SysKeyDown = 0x0104,

        /// <summary>
        /// Indicates that a system key (e.g., Alt) has been released.
        /// </summary>
        SysKeyUp = 0x0105
    }

}