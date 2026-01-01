using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    [CombatClassMetadata("[WotLK335a] Warrior Fury", "Jannis")]
    public class WarriorFury : BaseWarriorCombatClass
    {
        public WarriorFury(AmeisenBotInterfaces bot) : base(bot)
        {
            // Fury-specific setup (base class handles common auras, interrupts, HeroicStrikeEvent)
        }

        public override string Description => "FCFS based CombatClass for the Fury Warrior spec.";

        public override string DisplayName2 => "Warrior Fury";

        public override bool HandlesMovement => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator([WowArmorType.Shield], [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe]);

        public override WowRole Role => WowRole.Dps;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 3, new(1, 3, 2) },
                { 5, new(1, 5, 2) },
                { 6, new(1, 6, 3) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 3) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 6, new(2, 6, 3) },
                { 10, new(2, 10, 5) },
                { 13, new(2, 13, 3) },
                { 14, new(2, 14, 1) },
                { 16, new(2, 16, 1) },
                { 17, new(2, 17, 5) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 1) },
                { 20, new(2, 20, 2) },
                { 22, new(2, 22, 5) },
                { 23, new(2, 23, 1) },
                { 24, new(2, 24, 1) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 5) },
                { 27, new(2, 27, 1) },
            },
            Tree3 = [],
        };

        public override string Version => "1.0";

        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target != null)
                {
                    double distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                    // Use base class helper for CC break
                    if (TryBreakCrowdControl())
                    {
                        return;
                    }

                    if (distanceToTarget > 4.0)
                    {
                        // Use base class helper for gap closing
                        if (TryCloseGap())
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (HeroicStrikeEvent.Ready && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Recklessness))
                        {
                            if ((Bot.Player.Rage > 50 && Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 8.0f).Count() > 2 && TryCastSpellWarrior(WarriorWotlk.Cleave, WarriorWotlk.BerserkerStance, 0, true))
                                || (Bot.Player.Rage > 50 && TryCastSpellWarrior(WarriorWotlk.HeroicStrike, WarriorWotlk.BerserkerStance, Bot.Wow.TargetGuid, true)))
                            {
                                HeroicStrikeEvent.Run();
                                return;
                            }
                        }

                        if (TryCastSpellWarrior(WarriorWotlk.Bloodthirst, WarriorWotlk.BerserkerStance, Bot.Wow.TargetGuid, true)
                            || TryCastSpellWarrior(WarriorWotlk.Whirlwind, WarriorWotlk.BerserkerStance, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        // dont prevent BT or WW with GCD
                        if (CooldownManager.GetSpellCooldown(WarriorWotlk.Bloodthirst) <= 1200
                            || CooldownManager.GetSpellCooldown(WarriorWotlk.Whirlwind) <= 1200)
                        {
                            return;
                        }

                        if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == $"{WarriorWotlk.Slam}!")
                           && TryCastSpell(WarriorWotlk.Slam, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if (TryCastSpell(WarriorWotlk.BerserkerRage, 0))
                        {
                            return;
                        }

                        if (TryCastSpell(WarriorWotlk.Bloodrage, Bot.Wow.TargetGuid, true, Bot.Player.Health))
                        {
                            return;
                        }

                        if (TryCastSpell(WarriorWotlk.Recklessness, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if (TryCastSpell(WarriorWotlk.DeathWish, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if (Bot.Player.Rage > 25
                           && Bot.Target.HealthPercentage < 20
                           && TryCastSpellWarrior(WarriorWotlk.Execute, WarriorWotlk.BerserkerStance, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
