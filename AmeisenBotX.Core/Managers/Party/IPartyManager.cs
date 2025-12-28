using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Party
{
    /// <summary>
    /// Interface for party member management and role classification.
    /// </summary>
    public interface IPartyManager
    {
        /// <summary>
        /// All classified party members.
        /// </summary>
        IReadOnlyList<PartyMemberInfo> Members { get; }

        /// <summary>
        /// Get all party members with the specified role.
        /// </summary>
        IEnumerable<PartyMemberInfo> GetByRole(WowRole role);

        /// <summary>
        /// Get all healers in the party.
        /// </summary>
        IEnumerable<PartyMemberInfo> GetHealers();

        /// <summary>
        /// Get all tanks in the party.
        /// </summary>
        IEnumerable<PartyMemberInfo> GetTanks();

        /// <summary>
        /// Get all DPS in the party.
        /// </summary>
        IEnumerable<PartyMemberInfo> GetDps();

        /// <summary>
        /// Get all casters (mages, warlocks, etc.) in the party.
        /// </summary>
        IEnumerable<PartyMemberInfo> GetCasters();

        /// <summary>
        /// Get info for a specific party member by GUID.
        /// </summary>
        PartyMemberInfo GetMember(ulong guid);

        /// <summary>
        /// Check if a player is classified as a healer.
        /// </summary>
        bool IsHealer(ulong guid);

        /// <summary>
        /// Check if a player is classified as a caster.
        /// </summary>
        bool IsCaster(ulong guid);

        /// <summary>
        /// Check if a player is classified as a tank.
        /// </summary>
        bool IsTank(ulong guid);

        /// <summary>
        /// Update party member classifications. Call regularly.
        /// </summary>
        void Update();

        /// <summary>
        /// Force immediate update, bypassing rate limiting.
        /// </summary>
        void ForceUpdate();

        /// <summary>
        /// Called when INSPECT_TALENT_READY event fires.
        /// </summary>
        void OnInspectReady();
    }
}
