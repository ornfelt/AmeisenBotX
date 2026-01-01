using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Dps;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    [CombatClassMetadata("[WIP] Blood Deathknight", "Lukas")]
    public class DeathknightBlood(AmeisenBotInterfaces bot) : ICombatClass
    {
        public string Author => "Kamel";

        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        public Dictionary<string, dynamic> Configureables { get; set; } = [];

        public string Description => "FCFS based CombatClass for the Blood Deathknight spec.";

        public string DisplayName => "[WIP] Blood Deathknight";

        public bool HandlesFacing => false;

        public bool HandlesMovement => false;

        public bool IsMelee => true;

        public IItemComparator ItemComparator => null;

        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        public WowRole Role => WowRole.Dps;

        public TalentTree Talents { get; } = null;

        public bool TargetInLineOfSight { get; set; }

        public ITargetProvider TargetProvider { get; internal set; } = new TargetManager(new SimpleDpsTargetSelectionLogic(bot), TimeSpan.FromMilliseconds(250));//Heal/Tank/DPS

        public string Version => "1.0";

        public bool WalkBehindEnemy => false;

        public WowClass WowClass => WowClass.Deathknight;

        public WowSpecialization Specialization => WowSpecialization.DeathknightBlood;

        private AmeisenBotInterfaces Bot { get; } = bot;
        private DateTime LastTargetSwitch = DateTime.MinValue;

        public void AttackTarget()
        {
            IWowUnit target = Bot.Target;
            if (target == null)
            {
                return;
            }

            if (Bot.Player.Position.GetDistance(target.Position) <= 3.0)
            {
                Bot.Wow.StopClickToMove();
                Bot.Movement.Reset();
                Bot.Wow.InteractWithUnit(target);
            }
            else
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, target.Position);
            }
        }

        public void Execute()
        {
            ulong targetGuid = Bot.Wow.TargetGuid;
            IWowUnit target = Bot.Objects.All.OfType<IWowUnit>().FirstOrDefault(t => t.Guid == targetGuid);
            if (target != null)
            {
                // make sure we're auto attacking
                if (!Bot.Objects.Player.IsAutoAttacking)
                {
                    Bot.Wow.StartAutoAttack();
                }

                HandleAttacking(target);
            }
        }

        public void Load(Dictionary<string, JsonElement> objects)
        {
            if (objects.TryGetValue("Configureables", out JsonElement configElement))
            {
                Configureables = configElement.ToDyn();
            }
        }

        public void OutOfCombatExecute()
        {
        }

        public Dictionary<string, object> Save()
        {
            return new()
            {
                { "configureables", Configureables }
            };
        }

        private void HandleAttacking(IWowUnit target)
        {
            if (TargetProvider.Get(out IEnumerable<IWowUnit> targetToTarget))
            {
                IWowUnit firstTarget = targetToTarget.FirstOrDefault();

                if (firstTarget != null && Bot.Objects.Player.TargetGuid != firstTarget.Guid)
                {
                    // Stickiness check
                    if (!IWowUnit.IsValidAlive(Bot.Target) || (DateTime.UtcNow - LastTargetSwitch).TotalSeconds > 2)
                    {
                        Bot.Wow.ChangeTarget(firstTarget.Guid);
                        LastTargetSwitch = DateTime.UtcNow;
                    }
                }
            }

            if (Bot.Objects.Target == null
                || Bot.Objects.Target.IsDead
                || !IWowUnit.IsValid(Bot.Objects.Target))
            {
                return;
            }

            double playerRunePower = Bot.Objects.Player.RunicPower;
            double distanceToTarget = Bot.Objects.Player.Position.GetDistance(target.Position);
            double targetHealthPercent = target.Health / (double)target.MaxHealth * 100;
            double playerHealthPercent = Bot.Objects.Player.Health / (double)Bot.Objects.Player.MaxHealth * 100.0;
            (string, int) targetCastingInfo = Bot.Wow.GetUnitCastingInfo(WowLuaUnit.Target);
            //List<string> myBuffs = Bot.NewBot.GetBuffs(WowLuaUnit.Player.ToString());
            //myBuffs.Any(e => e.Equals("Chains of Ice"))

            if (Bot.Wow.GetSpellCooldown(DeathknightWotlk.DeathGrip) <= 0 && distanceToTarget <= 30)
            {
                Bot.Wow.CastSpell(DeathknightWotlk.DeathGrip);
                return;
            }
            if (target.IsFleeing && distanceToTarget <= 30)
            {
                Bot.Wow.CastSpell(DeathknightWotlk.ChainsOfIce);
                return;
            }

            if (Bot.Wow.GetSpellCooldown(DeathknightWotlk.ArmyOfTheDead) <= 0 &&
                IsOneOfAllRunesReady())
            {
                Bot.Wow.CastSpell(DeathknightWotlk.ArmyOfTheDead);
                return;
            }

            List<IWowUnit> unitsNearPlayer = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(e => e.Position.GetDistance(Bot.Objects.Player.Position) <= 10)
                .ToList();

            if ((unitsNearPlayer.Count > 2 &&
                Bot.Wow.GetSpellCooldown(DeathknightWotlk.BloodBoil) <= 0 &&
                Bot.Wow.IsRuneReady(0)) ||
                Bot.Wow.IsRuneReady(1))
            {
                Bot.Wow.CastSpell(DeathknightWotlk.BloodBoil);
                return;
            }

            List<IWowUnit> unitsNearTarget = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(e => e.Position.GetDistance(target.Position) <= 30)
                .ToList();

            if (unitsNearTarget.Count > 2 &&
                Bot.Wow.GetSpellCooldown(DeathknightWotlk.DeathAndDecay) <= 0 &&
                IsOneOfAllRunesReady())
            {
                Bot.Wow.CastSpell(DeathknightWotlk.DeathAndDecay);
                Bot.Wow.ClickOnTerrain(target.Position);
                return;
            }

            if ((Bot.Wow.GetSpellCooldown(DeathknightWotlk.IcyTouch) <= 0 &&
                Bot.Wow.IsRuneReady(2)) ||
                Bot.Wow.IsRuneReady(3))
            {
                Bot.Wow.CastSpell(DeathknightWotlk.IcyTouch);
                return;
            }

            // Plague Strike (Unholy Rune)
            if (Bot.Wow.GetSpellCooldown(DeathknightWotlk.PlagueStrike) <= 0 &&
               (Bot.Wow.IsRuneReady(4) || Bot.Wow.IsRuneReady(5)))
            {
                Bot.Wow.CastSpell(DeathknightWotlk.PlagueStrike);
                return;
            }

            // Heart Strike (Blood Rune) - Primary Damage
            if (Bot.Wow.GetSpellCooldown(DeathknightWotlk.HeartStrike) <= 0 &&
               (Bot.Wow.IsRuneReady(0) || Bot.Wow.IsRuneReady(1)))
            {
                Bot.Wow.CastSpell(DeathknightWotlk.HeartStrike);
                return;
            }

            // Death Strike (Frost + Unholy) - Survival/Damage
            if (Bot.Wow.GetSpellCooldown(DeathknightWotlk.DeathStrike) <= 0 &&
               (Bot.Wow.IsRuneReady(2) || Bot.Wow.IsRuneReady(3)) && // Frost
               (Bot.Wow.IsRuneReady(4) || Bot.Wow.IsRuneReady(5)))   // Unholy
            {
                Bot.Wow.CastSpell(DeathknightWotlk.DeathStrike);
                return;
            }

            // Rune Strike (Dump Runic Power)
            if (playerRunePower >= 20 && Bot.Wow.GetSpellCooldown(DeathknightWotlk.RuneStrike) <= 0)
            {
                Bot.Wow.CastSpell(DeathknightWotlk.RuneStrike);
                return;
            }

            // Death Coil (Range Dump)
            if (playerRunePower >= 40 && Bot.Wow.GetSpellCooldown(DeathknightWotlk.DeathCoil) <= 0)
            {
                Bot.Wow.CastSpell(DeathknightWotlk.DeathCoil);
                return;
            }
        }

        private bool IsOneOfAllRunesReady()
        {
            return (Bot.Wow.IsRuneReady(0) || Bot.Wow.IsRuneReady(1))
                && (Bot.Wow.IsRuneReady(2) || Bot.Wow.IsRuneReady(3))
                && (Bot.Wow.IsRuneReady(4) || Bot.Wow.IsRuneReady(5));
        }
    }
}
