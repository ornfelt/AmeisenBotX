using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    [CombatClassMetadata("[WotLK335a] Priest Discipline", "Jannis")]
    public class PriestDiscipline : BasicCombatClass
    {
        public PriestDiscipline(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PriestWotlk.PowerWordFortitude, () => TryCastSpell(PriestWotlk.PowerWordFortitude, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PriestWotlk.InnerFire, () => TryCastSpell(PriestWotlk.InnerFire, 0, true)));

            SpellUsageHealDict = new Dictionary<int, string>()
            {
                { 0, PriestWotlk.FlashHeal },
                { 400, PriestWotlk.FlashHeal },
                { 3000, PriestWotlk.Penance },
                { 5000, PriestWotlk.GreaterHeal },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((PriestWotlk.PowerWordFortitude, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        public override string Description => "FCFS based CombatClass for the Discipline Priest spec.";

        public override string DisplayName2 => "Priest Discipline";

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator([WowArmorType.Shield], [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe]);

        public override WowRole Role => WowRole.Heal;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 4, new(1, 4, 3) },
                { 5, new(1, 5, 2) },
                { 7, new(1, 7, 3) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 3) },
                { 11, new(1, 11, 3) },
                { 14, new(1, 14, 5) },
                { 15, new(1, 15, 1) },
                { 16, new(1, 16, 2) },
                { 17, new(1, 17, 3) },
                { 18, new(1, 18, 3) },
                { 19, new(1, 19, 1) },
                { 20, new(1, 20, 3) },
                { 21, new(1, 21, 2) },
                { 22, new(1, 22, 3) },
                { 23, new(1, 23, 2) },
                { 24, new(1, 24, 3) },
                { 25, new(1, 25, 1) },
                { 26, new(1, 26, 2) },
                { 27, new(1, 27, 5) },
                { 28, new(1, 28, 1) },
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 4, new(2, 4, 5) },
                { 6, new(2, 6, 1) },
                { 8, new(2, 8, 3) },
            },
            Tree3 = [],
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "1.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Priest;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private Dictionary<int, string> SpellUsageHealDict { get; }

        public override void Execute()
        {
            base.Execute();

            if ((Bot.Objects.PartymemberGuids.Any() || Bot.Player.HealthPercentage < 75.0)
                && NeedToHealSomeone())
            {
                return;
            }

            if ((!Bot.Objects.PartymemberGuids.Any() || Bot.Player.ManaPercentage > 50) && TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PriestWotlk.ShadowWordPain)
                    && TryCastSpell(PriestWotlk.ShadowWordPain, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(PriestWotlk.Smite, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(PriestWotlk.HolyShock, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(PriestWotlk.Consecration, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealSomeone()
                || HandleDeadPartymembers(PriestWotlk.Resurrection))
            {
                return;
            }
        }

        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                IWowUnit target = unitsToHeal.FirstOrDefault();

                if (target == null)
                {
                    return false;
                }

                if (unitsToHeal.Count() > 3
                    && TryCastSpell(PriestWotlk.PrayerOfHealing, target.Guid, true))
                {
                    return true;
                }

                if (target.Guid != Bot.Wow.PlayerGuid
                    && target.HealthPercentage < 70
                    && Bot.Player.HealthPercentage < 70
                    && TryCastSpell(PriestWotlk.BindingHeal, target.Guid, true))
                {
                    return true;
                }

                if (Bot.Player.ManaPercentage < 50
                    && TryCastSpell(PriestWotlk.HymnOfHope, 0))
                {
                    return true;
                }

                if (Bot.Player.HealthPercentage < 20
                    && TryCastSpell(PriestWotlk.DesperatePrayer, 0))
                {
                    return true;
                }

                if ((target.HealthPercentage < 98 && target.HealthPercentage > 80
                        && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PriestWotlk.WeakenedSoul)
                        && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PriestWotlk.PowerWordShield)
                        && TryCastSpell(PriestWotlk.PowerWordShield, target.Guid, true))
                    || (target.HealthPercentage < 90 && target.HealthPercentage > 80
                        && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PriestWotlk.Renew)
                        && TryCastSpell(PriestWotlk.Renew, target.Guid, true)))
                {
                    return true;
                }

                double healthDifference = target.MaxHealth - target.Health;
                List<KeyValuePair<int, string>> spellsToTry = SpellUsageHealDict.Where(e => e.Key <= healthDifference).ToList();

                foreach (KeyValuePair<int, string> keyValuePair in spellsToTry.OrderByDescending(e => e.Value))
                {
                    if (TryCastSpell(keyValuePair.Value, target.Guid, true))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
