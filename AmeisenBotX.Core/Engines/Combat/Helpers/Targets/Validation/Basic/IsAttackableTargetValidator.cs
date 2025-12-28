using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Validates that a target is actually attackable and safe to engage.
    /// Filters out: critters, non-combat pets, tagged mobs, totems, dead units, and non-attackable flags.
    /// </summary>
    public class IsAttackableTargetValidator(AmeisenBotInterfaces bot) : ITargetValidator
    {
        private AmeisenBotInterfaces Bot { get; } = bot;

        public bool IsValid(IWowUnit unit)
        {
            // Must be hostile or neutral reaction
            WowUnitReaction reaction = Bot.Db.GetReaction(Bot.Player, unit);
            if (reaction is not WowUnitReaction.Hostile and not WowUnitReaction.Neutral)
            {
                return false;
            }

            // CRITICAL: Filter out critters and non-combat entities
            // This prevents wasting time on non-XP targets and looking suspicious
            WowCreatureType creatureType = unit.ReadType();
            if (creatureType is WowCreatureType.Critter
                or WowCreatureType.NonCombatPet
                or WowCreatureType.Totem)
            {
                return false;
            }

            // CRITICAL: Never attack mobs tagged by other players
            // This honors kill credits and avoids player reports
            if (unit.IsTaggedByOther)
            {
                return false;
            }

            // Safety checks: ensure unit is alive and attackable
            return !unit.IsDead && !unit.IsNotAttackable;
        }
    }
}
