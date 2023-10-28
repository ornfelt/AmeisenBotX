using System;

/// <summary>
/// Represents flags for the WowUnit2Flag enum.
/// </summary>
namespace AmeisenBotX.Wow.Objects.Flags
{
    /// <summary>
    /// Represents flags for the WowUnit2 enum.
    /// </summary>
    [Flags]
    public enum WowUnit2Flag
    {
        FeignDeath = 0x1,
        NoModel = 0x2,
        Unknown1 = 0x4,
        Unknown2 = 0x8,
        Unknown3 = 0x10,
        Unknown4 = 0x20,
        ForceAutoRunForward = 0x40,
        Unknown5 = 0x80,
        Unknown6 = 0x400,
        Unknown7 = 0x800,
        Unknown8 = 0x1000,
    }
}