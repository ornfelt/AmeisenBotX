using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Combat.Classes.Jannis;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.CombatClasses.Shino
{
    /// <summary>
    /// Gets the author name.
    /// </summary>
    public abstract class TemplateCombatClass : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the TemplateCombatClass class with the specified AmeisenBotInterfaces instance.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance to use.</param>
        public TemplateCombatClass(AmeisenBotInterfaces bot) : base(bot)
        {
            //this line cause a bug because it run out of index
            //Bot.EventHookManager.Subscribe("UI_ERROR_MESSAGE", (t, a) => OnUIErrorMessage(a[0]));
        }

        /// <summary>
        /// Gets the author name.
        /// </summary>
        public new string Author { get; } = "Shino";

        /// <summary>
        /// Gets or sets the date and time of the last failed opener operation.
        /// </summary>
        private DateTime LastFailedOpener { get; set; } = DateTime.Now;

        /// <summary>
        /// Attack the target. If there is no target, return.
        /// If the target is attackable, cast the opening spell and stop movement.
        /// If the target is not attackable, move towards the target if within range or if no movement action is currently active.
        /// </summary>
        public override void AttackTarget()
        {
            IWowUnit target = Bot.Target;
            if (target == null)
            {
                return;
            }

            if (IsTargetAttackable(target))
            {
                Spell openingSpell = GetOpeningSpell();
                Bot.Wow.StopClickToMove();
                Bot.Movement.StopMovement();
                Bot.Movement.Reset();
                TryCastSpell(openingSpell.Name, target.Guid, openingSpell.Costs > 0);
            }
            else if (Bot.Player.Position.GetDistance(target.Position) < 3.5f || Bot.Movement.Status == MovementAction.None)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, target.Position);
            }
        }

        /// <summary>
        /// Checks if a target is attackable.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the target is attackable, false otherwise.</returns>
        public bool IsTargetAttackable(IWowUnit target)
        {
            Spell openingSpell = GetOpeningSpell();
            float posOffset = 0.5f;
            Vector3 currentPos = Bot.Player.Position;
            Vector3 posXLeft = Bot.Player.Position;
            posXLeft.X -= posOffset;
            Vector3 posXRight = Bot.Player.Position;
            posXRight.X += posOffset;
            Vector3 posYRight = Bot.Player.Position;
            posYRight.Y += posOffset;
            Vector3 posYLeft = Bot.Player.Position;
            posYLeft.Y -= posOffset;

            return IsInRange(openingSpell, target)
                    && DateTime.Now.Subtract(LastFailedOpener).TotalSeconds > 3
                    && Bot.Wow.IsInLineOfSight(currentPos, target.Position)
                    && Bot.Wow.IsInLineOfSight(posXLeft, target.Position)
                    && Bot.Wow.IsInLineOfSight(posXRight, target.Position)
                    && Bot.Wow.IsInLineOfSight(posYRight, target.Position)
                    && Bot.Wow.IsInLineOfSight(posYLeft, target.Position);
        }

        /// <summary>
        /// Updates the LastFailedOpener property with the current timestamp if the provided message is "target not in line of sight" (case insensitive).
        /// </summary>
        public void OnUIErrorMessage(string message)
        {
            if (string.Equals(message, "target not in line of sight", StringComparison.InvariantCultureIgnoreCase))
            {
                LastFailedOpener = DateTime.Now;
            }
        }

        /// <summary>
        /// Overrides the default ToString() method and returns a string representation of the object.
        /// The string includes the WowClass, Role, DisplayName, and Author properties enclosed in square brackets, with the DisplayName 
        /// followed by the Author's name in parentheses.
        /// </summary>
        public override string ToString()
        {
            return $"[{WowClass}] [{Role}] {DisplayName} ({Author})";
        }

        /// <summary>
        /// Retrieves the opening spell that must be implemented by subclasses.
        /// </summary>
        protected abstract Spell GetOpeningSpell();

        /// <summary>
        /// Selects a target for the bot to attack.
        /// </summary>
        /// <param name="target">The selected target.</param>
        /// <returns>True if a target is successfully selected, otherwise false.</returns>
        protected bool SelectTarget(out IWowUnit target)
        {
            IWowUnit currentTarget = Bot.Target;
            IEnumerable<IWowUnit> nearAttackingEnemies = Bot
                .GetEnemiesInCombatWithParty<IWowUnit>(Bot.Player.Position, 64.0f)
                .Where(e => !e.IsDead && !e.IsNotAttackable)
                .OrderBy(e => e.Auras.All(aura => Bot.Db.GetSpellName(aura.SpellId) != Mage335a.Polymorph));

            if (currentTarget != null && currentTarget.Guid != 0
               && (currentTarget.IsDead
                   || currentTarget.IsNotAttackable
                   || (currentTarget.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Mage335a.Polymorph) &&
                       nearAttackingEnemies.Where(e => e.Auras.All(aura => Bot.Db.GetSpellName(aura.SpellId) != Mage335a.Polymorph)).Any(e => e.Guid != currentTarget.Guid))
                   || (!currentTarget.IsInCombat && nearAttackingEnemies.Any())
                   || !IWowUnit.IsValid(Bot.Target)
                   || Bot.Db.GetReaction(Bot.Player, currentTarget) == WowUnitReaction.Friendly))
            {
                currentTarget = null;
                target = null;
            }

            if (currentTarget != null)
            {
                target = currentTarget;
                return true;
            }

            if (nearAttackingEnemies.Any())
            {
                IWowUnit potTarget = nearAttackingEnemies.FirstOrDefault();
                target = potTarget;
                Bot.Wow.ChangeTarget(potTarget.Guid);
                return true;
            }

            target = null;
            return false;
        }
    }
}