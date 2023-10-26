namespace AmeisenBotX.Common.Keyboard.Enums
{
    /// <summary>
    /// Represents the possible states of a keyboard key.
    /// </summary>
    internal enum KeyboardState
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SysKeyDown = 0x0104,
        SysKeyUp = 0x0105
    }
}