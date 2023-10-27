using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a priest specialization that focuses on healing and support abilities.
    /// </summary>
    public class PriestHoly : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the PriestHoly class.
        /// Initializes the PriestHoly object with the specified bot.
        /// </summary>
        /// <param name="bot">The bot object.</param>
        public PriestHoly(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.PowerWordFortitude, () => TryCastSpell(Priest335a.PowerWordFortitude, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.InnerFire, () => TryCastSpell(Priest335a.InnerFire, 0, true)));

            SpellUsageHealDict = new Dictionary<int, string>()
            {
                { 0, Priest335a.LesserHeal },
                { 100, Priest335a.FlashHeal },
                { 5000, Priest335a.GreaterHeal },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Priest335a.PowerWordFortitude, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description for the FCFS based CombatClass of the Holy Priest spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Holy Priest spec.";

        /// <summary>
        /// Gets or sets the display name of the Priest Holy.
        /// </summary>
        public override string DisplayName2 => "Priest Holy";

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is a melee character.
        /// </summary>
        /// <returns>False always.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the current object.
        /// The item comparator is set to a new instance of the BasicSpiritComparator class,
        /// which includes a list of WowArmorType.Shield and a list of WowWeaponType.Sword,
        /// WowWeaponType.Mace, and WowWeaponType.Axe.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets the role of the Wow character which is Heal.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// <summary>
        /// The TalentTree property overrides the base class's TalentTree property.
        /// It represents the talent trees of a character and holds the talent tree objects.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 4, new(1, 4, 3) },
                { 5, new(1, 5, 2) },
                { 7, new(1, 7, 3) },
                { 8, new(1, 8, 1) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 2) },
                { 2, new(2, 2, 3) },
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 8, new(2, 8, 3) },
                { 9, new(2, 9, 2) },
                { 10, new(2, 10, 3) },
                { 12, new(2, 12, 2) },
                { 13, new(2, 13, 1) },
                { 14, new(2, 14, 5) },
                { 15, new(2, 15, 2) },
                { 16, new(2, 16, 5) },
                { 17, new(2, 17, 3) },
                { 22, new(2, 22, 3) },
                { 23, new(2, 23, 3) },
                { 24, new(2, 24, 1) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 5) },
                { 27, new(2, 27, 1) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether the object uses auto attacks.
        /// </summary>
        /// <returns>False, as the object does not use auto attacks.</returns>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets or sets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind enemy.
        /// </summary>
        /// <returns>Always returns <c>false</c> indicating that the character cannot walk behind enemy.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class of the character, which is set to Priest.
        /// </summary>
        public override WowClass WowClass => WowClass.Priest;

        /// <summary>
        /// Gets or sets the World of Warcraft version to WotLK 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets the dictionary of spell usage for healing, where the key represents the spell identifier
        /// and the value represents the corresponding healing spell name.
        /// </summary>
        private Dictionary<int, string> SpellUsageHealDict { get; }

        /// <summary>
        /// Executes the logic for the Priest335a class.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if ((Bot.Objects.PartymemberGuids.Any() || Bot.Player.HealthPercentage < 75.0)
                && NeedToHealSomeone())
            {
                return;
            }

            if (Bot.Player.ManaPercentage < 20
                && TryCastSpell(Priest335a.HymnOfHope, 0))
            {
                return;
            }

            if ((!Bot.Objects.PartymemberGuids.Any() || Bot.Player.ManaPercentage > 50) && TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Priest335a.ShadowWordPain)
                    && TryCastSpell(Priest335a.ShadowWordPain, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(Priest335a.Smite, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the code when the character is out of combat. 
        /// Checks if someone needs healing or if there are dead party members to resurrect and returns.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealSomeone()
                || HandleDeadPartymembers(Priest335a.Resurrection))
            {
                return;
            }
        }

        /// <summary>
        /// Determines if there is a need to heal someone.
        /// </summary>
        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                IWowUnit target = unitsToHeal.First();

                if (unitsToHeal.Count() > 3
                    && target.HealthPercentage > 80.0
                    && TryCastSpell(Priest335a.PrayerOfHealing, target.Guid, true))
                {
                    return true;
                }

                if (target.HealthPercentage < 25.0
                    && TryCastSpell(Priest335a.GuardianSpirit, target.Guid, true))
                {
                    return true;
                }

                if (target.Guid != Bot.Wow.PlayerGuid
                    && target.HealthPercentage < 70.0
                    && Bot.Player.HealthPercentage < 70.0
                    && TryCastSpell(Priest335a.BindingHeal, target.Guid, true))
                {
                    return true;
                }

                if (target.HealthPercentage < 90.0
                    && target.HealthPercentage > 75.0
                    && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Priest335a.Renew)
                    && TryCastSpell(Priest335a.Renew, target.Guid, true))
                {
                    return true;
                }

                double healthDifference = target.MaxHealth - target.Health;
                List<KeyValuePair<int, string>> spellsToTry = SpellUsageHealDict.Where(e => e.Key <= healthDifference).OrderByDescending(e => e.Key).ToList();

                foreach (KeyValuePair<int, string> keyValuePair in spellsToTry)
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