using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Constructor for the PaladinRetribution class. Initializes the PaladinRetribution object with the provided bot interface.
    /// </summary>
    public class PaladinRetribution : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the PaladinRetribution class. Initializes the PaladinRetribution object with the provided bot interface.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for communication with the bot.</param>
        public PaladinRetribution(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.BlessingOfMight, () => TryCastSpell(Paladin335a.BlessingOfMight, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.RetributionAura, () => TryCastSpell(Paladin335a.RetributionAura, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.SealOfVengeance, () => TryCastSpell(Paladin335a.SealOfVengeance, 0, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Paladin335a.HammerOfJustice, x.Guid, true) }
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Paladin335a.BlessingOfMight, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Retribution Paladin spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Retribution Paladin spec.";

        /// <summary>
        /// Gets or sets the display name for a retribution Paladin.
        /// </summary>
        public override string DisplayName2 => "Paladin Retribution";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement. 
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets or sets a value indicating whether the character is a melee fighter.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for the class. Overrides the base class implementation. 
        /// The item comparator is set to a BasicStrengthComparator with a shield armor type and axe, mace, and sword weapon types.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Axe, WowWeaponType.Mace, WowWeaponType.Sword });

        /// <summary>
        /// Gets or sets the role of a character in the World of Warcraft game, specifically the character is a DPS (damage per second) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// Represents a collection of talent trees.
        /// Each talent tree is identified by a unique string key (e.g. Tree1, Tree2, etc.).
        /// The talent tree consists of a dictionary where the key is an integer representing the talent index,
        /// and the value is an instance of the Talent class.
        /// The Talent class represents a specific talent within the talent tree,
        /// and contains properties for the talent's index, row, column, and tier.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 4, new(1, 4, 5) },
                { 6, new(1, 6, 1) },
            },
            Tree2 = new()
            {
                { 2, new(2, 2, 5) },
            },
            Tree3 = new()
            {
                { 2, new(3, 2, 5) },
                { 3, new(3, 3, 2) },
                { 4, new(3, 4, 3) },
                { 5, new(3, 5, 2) },
                { 7, new(3, 7, 5) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 2) },
                { 11, new(3, 11, 3) },
                { 12, new(3, 12, 3) },
                { 13, new(3, 13, 3) },
                { 14, new(3, 14, 1) },
                { 15, new(3, 15, 3) },
                { 17, new(3, 17, 2) },
                { 18, new(3, 18, 1) },
                { 19, new(3, 19, 3) },
                { 20, new(3, 20, 3) },
                { 21, new(3, 21, 2) },
                { 22, new(3, 22, 3) },
                { 23, new(3, 23, 1) },
                { 24, new(3, 24, 3) },
                { 25, new(3, 25, 3) },
                { 26, new(3, 26, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code as a string.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets whether the character can walk behind an enemy.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class of type Paladin.
        /// </summary>
        public override WowClass WowClass => WowClass.Paladin;

        /// <summary>
        /// Gets or sets the version of World of Warcraft as Wrath of the Lich King (3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Executes the specified action for the Paladin335a bot.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if ((Bot.Player.HealthPercentage < 20.0
                        && TryCastSpell(Paladin335a.LayOnHands, Bot.Wow.PlayerGuid))
                    || (Bot.Player.HealthPercentage < 60.0
                        && TryCastSpell(Paladin335a.HolyLight, Bot.Wow.PlayerGuid, true)))
                {
                    return;
                }

                if (((Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.SealOfVengeance) || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.SealOfWisdom))
                        && TryCastSpell(Paladin335a.JudgementOfLight, Bot.Wow.TargetGuid, true))
                    || TryCastSpell(Paladin335a.AvengingWrath, 0, true)
                    || (Bot.Player.ManaPercentage < 80.0
                        && TryCastSpell(Paladin335a.DivinePlea, 0, true)))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if ((Bot.Player.HealthPercentage < 20.0
                            && TryCastSpell(Paladin335a.HammerOfWrath, Bot.Wow.TargetGuid, true))
                        || TryCastSpell(Paladin335a.CrusaderStrike, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Paladin335a.DivineStorm, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Paladin335a.Consecration, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Paladin335a.Exorcism, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Paladin335a.HolyWrath, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the OutOfCombatExecute method by calling the base implementation.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();
        }
    }
}