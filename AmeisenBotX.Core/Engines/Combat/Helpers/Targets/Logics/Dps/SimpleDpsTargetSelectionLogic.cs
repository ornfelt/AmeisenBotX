using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Basic;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Dps
{
    /// <summary>
    /// "Pitbull" Target Selection Logic.
    /// Design constraints:
    /// 1. Zero-Oscillation: Once locked, never switches until dead.
    /// 2. Command-Driven: Autopilot/User sets initial target -> We lock it.
    /// 3. Self-Defense: Auto-locks threats if idle.
    /// </summary>
    public class SimpleDpsTargetSelectionLogic : BasicTargetSelectionLogic
    {
        private ulong _lockedTargetGuid = 0;

        public SimpleDpsTargetSelectionLogic(AmeisenBotInterfaces bot) : base(bot)
        {
            // Base constructor logic (Validators/Prioritizers) is largely bypassed by the Pitbull logic below,
            // but we keep the initialization to satisfy inheritance and potential fallbacks if needed.
            TargetValidator.Add(new IsAttackableTargetValidator(bot));
            TargetValidator.Add(new IsThreatTargetValidator(bot));
        }

        public override bool SelectTarget(out IEnumerable<IWowUnit> possibleTargets)
        {
            possibleTargets = null;

            // --- PHASE 1: COMMAND & SYNC ---
            // If the Player (User or Autopilot) has explicitly selected a target, update our lock to match.
            // This allows Autopilot to "Lead" the engagement.
            if (Bot.Player.TargetGuid != 0)
            {
                // Only update lock if the new target is different allowing manual override
                if (Bot.Player.TargetGuid != _lockedTargetGuid)
                {
                    _lockedTargetGuid = Bot.Player.TargetGuid;
                }
            }

            // --- PHASE 2: LOCK MAINTENANCE (THE PITBULL) ---
            if (_lockedTargetGuid != 0)
            {
                var lockedUnit = Bot.Objects.All.OfType<IWowUnit>().FirstOrDefault(u => u.Guid == _lockedTargetGuid);

                // Check Validity: Exists + Alive + Attackable
                if (IsUnitValid(lockedUnit))
                {
                    // LOCKED: Return ONLY this target. Stop thinking.
                    possibleTargets = new List<IWowUnit> { lockedUnit };
                    return true;
                }

                // Lock is broken (Dead/Vanished/Invalid) -> Release.
                _lockedTargetGuid = 0;
            }

            // --- PHASE 3: SELF DEFENSE (AGGRO) ---
            // If we are unlocked, check if anyone is actively trying to kill us/party.
            var threat = Bot.Objects.All.OfType<IWowUnit>()
                .Where(u => IsUnitValid(u) 
                            && u.IsInCombat 
                            && (u.TargetGuid == Bot.Player.Guid || Bot.Objects.PartymemberGuids.Contains(u.TargetGuid)))
                .OrderBy(u => u.DistanceTo(Bot.Player))
                .FirstOrDefault();

            if (threat != null)
            {
                // New threat acquired -> Lock it.
                _lockedTargetGuid = threat.Guid;
                possibleTargets = new List<IWowUnit> { threat };
                return true;
            }

            // --- PHASE 4: OPPORTUNITY (HOSTILES) ---
            // If idle and safe, scan for standard hostile mobs nearby to grind/clear.
            // NOTE: We do NOT auto-acquire Neutrals here to avoid "Genocide Mode". 
            // Neutrals are handled via Phase 1 (Autopilot selects them for quests).
            var target = Bot.Objects.All.OfType<IWowUnit>()
                .Where(u => IsUnitValid(u) 
                            && Bot.IsHostileReaction(u)) // Red mobs only
                .OrderBy(u => u.DistanceTo(Bot.Player))
                .FirstOrDefault();

            if (target != null && target.DistanceTo(Bot.Player) <= 40f) // Hard range limit for auto-pull
            {
                _lockedTargetGuid = target.Guid;
                possibleTargets = new List<IWowUnit> { target };
                return true;
            }

            return false;
        }

        /// <summary>
        /// Centralized validity check for "Pitbull" logic.
        /// </summary>
        private bool IsUnitValid(IWowUnit u)
        {
            return u != null 
                && IWowUnit.IsValid(u)
                && !u.IsDead 
                && !u.IsNotAttackable 
                && !u.IsNotSelectable;
        }
    }
}
