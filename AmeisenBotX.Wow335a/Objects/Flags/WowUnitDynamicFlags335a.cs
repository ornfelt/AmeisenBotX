using System;

/// <summary>
/// Represents the dynamic flags for a World of Warcraft unit in patch 3.3.5a.
/// </summary>
namespace AmeisenBotX.Wow335a.Objects.Flags
{
    /// <summary>
    /// Represents the dynamic flags for a World of Warcraft unit in patch 3.3.5a.
    /// </summary>
    [Flags]
    public enum WowUnitDynamicFlags335a
    {
        None = 0x0,
        Lootable = 0x1,
        TrackUnit = 0x2,
        TaggedByOther = 0x4,
        TaggedByMe = 0x8,
        SpecialInfo = 0x10,
        Dead = 0x20,
        ReferAFriendLinked = 0x40,
        IsTappedByAllThreatList = 0x80,
    }
}