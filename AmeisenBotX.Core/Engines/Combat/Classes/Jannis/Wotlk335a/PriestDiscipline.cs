using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a namespace for the Priest Discipline class in the Wotlk335a engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a Priest Discipline class that inherits from the BasicCombatClass.
    /// </summary>
    public class PriestDiscipline : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the PriestDiscipline class.
        /// Initializes the PriestDiscipline object, setting the bot parameter to the provided value.
        /// Adds jobs to the MyAuraManager to keep the PowerWordFortitude and InnerFire auras active.
        /// Initializes the SpellUsageHealDict dictionary with key-value pairs representing spell usage thresholds and corresponding spell names.
        /// Adds the PowerWordFortitude spell to the SpellsToKeepActiveOnParty list in the GroupAuraManager.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used as a parameter for the base constructor.</param>
        public PriestDiscipline(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.PowerWordFortitude, () => TryCastSpell(Priest335a.PowerWordFortitude, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.InnerFire, () => TryCastSpell(Priest335a.InnerFire, 0, true)));

            SpellUsageHealDict = new Dictionary<int, string>()
            {
                { 0, Priest335a.FlashHeal },
                { 400, Priest335a.FlashHeal },
                { 3000, Priest335a.Penance },
                { 5000, Priest335a.GreaterHeal },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Priest335a.PowerWordFortitude, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Discipline Priest spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Discipline Priest spec.";

        /// <summary>
        /// Gets or sets the display name of the Priest Discipline.
        /// </summary>
        public override string DisplayName2 => "Priest Discipline";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> indicating that this object does not handle movement.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is a melee type.
        /// It always returns false as the overridden behavior.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for this instance.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets the role of the character as a healer.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// <summary>
        /// Gets or sets the talent tree object with individual talents and their respective values.
        /// </summary>
        /// <value>
        /// The talent tree object.
        /// </value>
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
            Tree3 = new(),
        };

        /// <summary>
        /// Overrides the UseAutoAttacks property and returns false, indicating that auto attacks should not be used.
        /// </summary>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind enemy.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property of the current object to WowClass.Priest.
        /// </summary>
        public override WowClass WowClass => WowClass.Priest;

        /// <summary>
        /// Gets the World of Warcraft version, which is set to Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets a private dictionary that stores the usage and healing values of spells.
        /// The keys of the dictionary are integers representing the spell IDs, while the values are strings representing the amount of healing provided by each spell.
        /// </summary>
        private Dictionary<int, string> SpellUsageHealDict { get; }

        /// <summary>
        /// Executes the code block and performs various actions based on certain conditions.
        /// If the player's health is below 75% or there are party members that need healing,
        /// the method will return and not perform any further actions.
        /// If there are no party members or the player's mana percentage is above 50,
        /// the method will attempt to find a target using the TargetProviderDps and cast
        /// the ShadowWordPain spell on the target. If successful, the method will return.
        /// If casting ShadowWordPain is not possible, the method will attempt to cast
        /// the Smite spell on the target. If successful, the method will return.
        /// If casting Smite is not possible, the method will attempt to cast the HolyShock
        /// spell on the target. If successful, the method will return.
        /// If casting HolyShock is not possible, the method will attempt to cast the Consecration
        /// spell on the target. If successful, the method will return.
        /// </summary>
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
                if (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Priest335a.ShadowWordPain)
                    && TryCastSpell(Priest335a.ShadowWordPain, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(Priest335a.Smite, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(Priest335a.HolyShock, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(Priest335a.Consecration, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the action when the character is out of combat.
        /// Checks if there is a need to heal someone or handle dead party members.
        /// If either condition is met, returns without further execution.
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

        ///<summary>
        /// Checks if there is a need to heal someone.
        ///</summary>
        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                IWowUnit target = unitsToHeal.First();

                if (unitsToHeal.Count() > 3
                    && TryCastSpell(Priest335a.PrayerOfHealing, target.Guid, true))
                {
                    return true;
                }

                if (target.Guid != Bot.Wow.PlayerGuid
                    && target.HealthPercentage < 70
                    && Bot.Player.HealthPercentage < 70
                    && TryCastSpell(Priest335a.BindingHeal, target.Guid, true))
                {
                    return true;
                }

                if (Bot.Player.ManaPercentage < 50
                    && TryCastSpell(Priest335a.HymnOfHope, 0))
                {
                    return true;
                }

                if (Bot.Player.HealthPercentage < 20
                    && TryCastSpell(Priest335a.DesperatePrayer, 0))
                {
                    return true;
                }

                if ((target.HealthPercentage < 98 && target.HealthPercentage > 80
                        && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Priest335a.WeakenedSoul)
                        && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Priest335a.PowerWordShield)
                        && TryCastSpell(Priest335a.PowerWordShield, target.Guid, true))
                    || (target.HealthPercentage < 90 && target.HealthPercentage > 80
                        && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Priest335a.Renew)
                        && TryCastSpell(Priest335a.Renew, target.Guid, true)))
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