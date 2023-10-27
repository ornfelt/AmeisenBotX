using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    ///<summary>
    /// Initializes a new instance of the PaladinProtection class.
    /// Adds jobs to the MyAuraManager to keep certain auras active.
    /// Sets up the InterruptManager with the HammerOfJustice spell as an interrupt.
    /// Adds a spell to keep active on the party using the GroupAuraManager.
    /// </summary>
    public class PaladinProtection : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the PaladinProtection class.
        /// Adds jobs to the MyAuraManager to keep certain auras active.
        /// Sets up the InterruptManager with the HammerOfJustice spell as an interrupt.
        /// Adds a spell to keep active on the party using the GroupAuraManager.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public PaladinProtection(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.DevotionAura, () => TryCastSpell(Paladin335a.DevotionAura, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.BlessingOfKings, () => TryCastSpell(Paladin335a.BlessingOfKings, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.SealOfVengeance, () => TryCastSpell(Paladin335a.SealOfVengeance, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.RighteousFury, () => TryCastSpell(Paladin335a.RighteousFury, 0, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Paladin335a.HammerOfJustice, x.Guid, true) }
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Paladin335a.BlessingOfKings, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Protection Paladin spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Protection Paladin spec.";

        /// <summary>
        /// Gets the display name for a Paladin Protection.
        /// </summary>
        public override string DisplayName2 => "Paladin Protection";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> if this object does not handle movement; otherwise, <c>true</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is a melee weapon.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicArmorComparator(null, new() { WowWeaponType.SwordTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.AxeTwoHand });

        /// <summary>
        /// Gets or sets the role of the WoW character as a Tank.
        /// </summary>
        public override WowRole Role => WowRole.Tank;

        /// Initializes a new instance of the TalentTree class, with the following configuration:
        /// 
        /// Tree1: An empty tree
        /// Tree2: Contains the following talents:
        /// - Talent at index 2 with values (2, 2, 5)
        /// - Talent at index 5 with values (2, 5, 5)
        /// - Talent at index 6 with values (2, 6, 1)
        /// - Talent at index 7 with values (2, 7, 3)
        /// - Talent at index 8 with values (2, 8, 5)
        /// - Talent at index 9 with values (2, 9, 2)
        /// - Talent at index 11 with values (2, 11, 3)
        /// - Talent at index 12 with values (2, 12, 1)
        /// - Talent at index 14 with values (2, 14, 2)
        /// - Talent at index 15 with values (2, 15, 3)
        /// - Talent at index 16 with values (2, 16, 1)
        /// - Talent at index 17 with values (2, 17, 1)
        /// - Talent at index 18 with values (2, 18, 3)
        /// - Talent at index 19 with values (2, 19, 3)
        /// - Talent at index 20 with values (2, 20, 3)
        /// - Talent at index 21 with values (2, 21, 3)
        /// - Talent at index 22 with values (2, 22, 1)
        /// - Talent at index 23 with values (2, 23, 2)
        /// - Talent at index 24 with values (2, 24, 3)
        /// - Talent at index 25 with values (2, 25, 2)
        /// - Talent at index 26 with values (2, 26, 1)
        /// Tree3: Contains the following talents:
        /// - Talent at index 1 with values (3, 1, 5)
        /// - Talent at index 3 with values (3, 3, 2)
        /// - Talent at index 4 with values (3, 4, 3)
        /// - Talent at index 7 with values (3, 7, 5)
        /// - Talent at index 12 with values (3, 12, 3)
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 2, new(2, 2, 5) },
                { 5, new(2, 5, 5) },
                { 6, new(2, 6, 1) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 5) },
                { 9, new(2, 9, 2) },
                { 11, new(2, 11, 3) },
                { 12, new(2, 12, 1) },
                { 14, new(2, 14, 2) },
                { 15, new(2, 15, 3) },
                { 16, new(2, 16, 1) },
                { 17, new(2, 17, 1) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 3) },
                { 20, new(2, 20, 3) },
                { 21, new(2, 21, 3) },
                { 22, new(2, 22, 1) },
                { 23, new(2, 23, 2) },
                { 24, new(2, 24, 3) },
                { 25, new(2, 25, 2) },
                { 26, new(2, 26, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 3, new(3, 3, 2) },
                { 4, new(3, 4, 3) },
                { 7, new(3, 7, 5) },
                { 12, new(3, 12, 3) },
            },
        };

        /// <summary>
        /// Specifies that auto attacks should be used.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        /// <returns>The version of the code.</returns>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind enemy.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property to Paladin.
        /// </summary>
        public override WowClass WowClass => WowClass.Paladin;

        /// <summary>
        /// Gets or sets the World of Warcraft version as Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets a value indicating whether the 9-second spell should be used.
        /// </summary>
        private bool Use9SecSpell { get; set; }

        ///<summary>
        /// This method is responsible for executing the paladin's combat rotation. 
        /// It first calls the base Execute method from the parent class. 
        /// 
        /// If the target is found, it checks the player's health percentage and tries to use the Lay On Hands spell if the health is below 10%. 
        /// If that fails, it checks for a health percentage below 20% and tries to use the Flash of Light spell. 
        /// If that also fails, it checks for a health percentage below 35% and tries to use the Holy Light spell. 
        /// 
        /// Then, it tries to cast the Sacred Shield spell or the Divine Plea spell. 
        /// 
        /// If there is a target, it checks if the target is not targeting the player and tries to use the Hand of Reckoning spell. 
        /// 
        /// Then, it checks for the Avenger's Shield spell or the Hammer of Wrath spell if the target's health is below 20%. 
        /// 
        /// If the Use9SecSpell flag is true, it tries to use the Judgement of Light spell if the player has either the Seal of Vengeance or the Seal of Wisdom buff. 
        /// If that fails, it tries to use the Consecration spell or the Holy Shield spell. 
        /// If none of those conditions are met, it tries to use the Shield of the Righteousness spell or the Hammer of the Righteous spell. 
        /// 
        /// The flag Use9SecSpell is toggled based on whether the Shield of the Righteousness spell or the Hammer of the Righteous spell is used. 
        ///</summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderTank, out _))
            {
                if (Bot.Player.HealthPercentage < 10.0
                    && TryCastSpell(Paladin335a.LayOnHands, 0, true))
                {
                    return;
                }

                if (Bot.Player.HealthPercentage < 20.0
                    && TryCastSpell(Paladin335a.FlashOfLight, 0, true))
                {
                    return;
                }
                else if (Bot.Player.HealthPercentage < 35.0
                    && TryCastSpell(Paladin335a.HolyLight, 0, true))
                {
                    return;
                }

                if (TryCastSpell(Paladin335a.SacredShield, 0, true)
                    || TryCastSpell(Paladin335a.DivinePlea, 0, true))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid
                        && TryCastSpell(Paladin335a.HandOfReckoning, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (TryCastSpell(Paladin335a.AvengersShield, Bot.Wow.TargetGuid, true)
                        || (Bot.Target.HealthPercentage < 20.0 && TryCastSpell(Paladin335a.HammerOfWrath, Bot.Wow.TargetGuid, true)))
                    {
                        return;
                    }

                    if (Use9SecSpell
                        && (((Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.SealOfVengeance) || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.SealOfWisdom))
                                && TryCastSpell(Paladin335a.JudgementOfLight, Bot.Wow.TargetGuid, true))
                            || TryCastSpell(Paladin335a.Consecration, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Paladin335a.HolyShield, Bot.Wow.TargetGuid, true)))
                    {
                        Use9SecSpell = false;
                        return;
                    }
                    else if (TryCastSpell(Paladin335a.ShieldOfTheRighteousness, Bot.Wow.TargetGuid, true)
                             || TryCastSpell(Paladin335a.HammerOfTheRighteous, Bot.Wow.TargetGuid, true))
                    {
                        Use9SecSpell = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Use9SecSpell variable to true and calls the base class's OutOfCombatExecute method.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            Use9SecSpell = true;

            base.OutOfCombatExecute();
        }
    }
}