using System;

namespace AmeisenBotX.Wow.Objects.Flags
{
    /// <summary>
    /// Represents the flags for a World of Warcraft aura.
    /// </summary>
    [Flags]
    public enum WowAuraFlag
    {
        Passive = 0x10,
        Harmful = 0x20,
        Active = 0x80
    }
}