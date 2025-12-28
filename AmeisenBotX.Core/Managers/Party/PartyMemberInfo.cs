using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Core.Managers.Party
{
    /// <summary>
    /// Information about a party member including detected role and capabilities.
    /// </summary>
    public class PartyMemberInfo
    {
        public ulong Guid { get; set; }
        public string Name { get; set; }
        public WowClass Class { get; set; }
        public WowRole Role { get; set; }

        /// <summary>
        /// Detected specialization based on auras and behavior.
        /// </summary>
        public WowSpecialization Spec { get; set; }

        /// <summary>
        /// True if this player is classified as a caster (mage, warlock, ele shaman, etc.)
        /// </summary>
        public bool IsCaster { get; set; }

        /// <summary>
        /// True if this player is detected as a healer (has healing spec active)
        /// </summary>
        public bool IsHealer { get; set; }

        /// <summary>
        /// True if this player is detected as a tank (has tank spec/stance active)
        /// </summary>
        public bool IsTank { get; set; }

        /// <summary>
        /// Live reference to the player object (may be null if out of range)
        /// </summary>
        public IWowPlayer Player { get; set; }

        /// <summary>
        /// Last time this member's info was updated
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
