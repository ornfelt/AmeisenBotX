using AmeisenBotX.Wow.Objects;
using System.Linq;

/// <summary>
/// The Bot interface used for validating threat targets.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    ///summary>
    /// The Bot interface used for validating threat targets.
    ///</summary>
    public class IsThreatTargetValidator : ITargetValidator
    {
        ///<summary>
        ///Creates a new instance of the IsThreatTargetValidator class.
        ///</summary>
        ///<param name="bot">The AmeisenBotInterfaces instance to be used for validating threat targets.</param>
        public IsThreatTargetValidator(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets the Bot interface.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Determines whether the specified unit is considered valid based on the following conditions:
        /// - The unit is tagged by me or my group
        /// - The unit has no target
        /// - The unit is targeting me, my group members, or pets
        /// - My group members or pets are targeting the unit
        /// </summary>
        /// <param name="unit">The unit to be validated</param>
        /// <returns>True if the unit is considered valid, otherwise false</returns>
        public bool IsValid(IWowUnit unit)
        {
            // is tagged by me or my group
            return (unit.IsTaggedByMe || !unit.IsTaggedByOther)
                // has no target
                && (unit.TargetGuid == 0
                    // unit is targeting me, group or pets
                    || (unit.TargetGuid == Bot.Player.Guid || Bot.Objects.PartymemberGuids.Contains(unit.TargetGuid) || Bot.Objects.PartyPetGuids.Contains(unit.TargetGuid)
                    // group or pets are targeting the unit
                    || (Bot.Objects.Partymembers.Any(e => e.TargetGuid == unit.Guid) || Bot.Objects.PartyPets.Any(e => e.TargetGuid == unit.Guid))));
        }
    }
}