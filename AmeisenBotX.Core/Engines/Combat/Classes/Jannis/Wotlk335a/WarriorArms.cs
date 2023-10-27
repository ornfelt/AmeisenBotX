using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Constructor for the WarriorArms class.
    /// </summary>
    public class WarriorArms : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the WarriorArms class.
        /// Initializes the WarriorArms object with the provided bot parameter.
        /// Adds a job to the MyAuraManager.Jobs list to keep the Warrior's Battle Shout aura active.
        /// Adds a job to the TargetAuraManager.Jobs list to keep the Hamstring aura active on the target if it is a player.
        /// Adds a job to the TargetAuraManager.Jobs list to keep the Rend aura active on the target if the player's Rage is above 75.
        /// Sets the interrupt spells for the InterruptManager with corresponding cast spell methods.
        /// Initializes the HeroicStrikeEvent with a delay of 2 seconds.
        /// </summary>
        public WarriorArms(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.BattleShout, () => TryCastSpell(Warrior335a.BattleShout, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.Hamstring, () => Bot.Target?.Type == WowObjectType.Player && TryCastSpell(Warrior335a.Hamstring, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.Rend, () => Bot.Target?.Type == WowObjectType.Player && Bot.Player.Rage > 75 && TryCastSpell(Warrior335a.Rend, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellWarrior(Warrior335a.IntimidatingShout, Warrior335a.BerserkerStance, x.Guid, true) },
                { 1, (x) => TryCastSpellWarrior(Warrior335a.IntimidatingShout, Warrior335a.BattleStance, x.Guid, true) }
            };

            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Gets the description of the CombatClass for the Arms Warrior spec, which is based on First-Come-First-Serve (FCFS) strategy.
        /// </summary>
        /// <returns>The description as a string.</returns>
        public override string Description => "FCFS based CombatClass for the Arms Warrior spec.";

        /// <summary>
        /// Gets or sets the display name for the Arms specialization of the Warrior class.
        /// </summary>
        public override string DisplayName2 => "Warrior Arms";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// Returns false, as this object does not handle movement.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is a melee character.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for this object.
        /// The default value is a BasicStrengthComparator initialized with the armor type Shield
        /// and the weapon types Sword, Mace, and Axe.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the role of the character in the World of Warcraft game.
        /// </summary>
        /// <value>The role is set to DPS (Damage Per Second).</value>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree for the character.
        /// </summary>
        /// <value>
        /// The talent tree object with three trees: Tree1, Tree2, and Tree3.
        /// Each tree contains multiple talents represented by a dictionary.
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 3, new(1, 3, 2) },
                { 4, new(1, 4, 2) },
                { 6, new(1, 6, 3) },
                { 7, new(1, 7, 2) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 3) },
                { 12, new(1, 12, 3) },
                { 13, new(1, 13, 5) },
                { 14, new(1, 14, 1) },
                { 17, new(1, 17, 2) },
                { 19, new(1, 19, 2) },
                { 21, new(1, 21, 1) },
                { 22, new(1, 22, 2) },
                { 24, new(1, 24, 1) },
                { 25, new(1, 25, 3) },
                { 26, new(1, 26, 2) },
                { 27, new(1, 27, 3) },
                { 28, new(1, 28, 1) },
                { 29, new(1, 29, 2) },
                { 30, new(1, 30, 5) },
                { 31, new(1, 31, 1) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 2, new(2, 2, 2) },
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 7, new(2, 7, 1) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether auto attacks should be used.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the implementation.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets whether the character can walk behind enemies.
        /// Returns false indicating that the character cannot walk behind enemies.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the wow class of the character which is Warrior.
        /// </summary>
        public override WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Gets or sets the version of World of Warcraft the code is targeting.
        /// Currently set to World of Warcraft: Wrath of the Lich King (3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the private TimegatedEvent property HeroicStrikeEvent.
        /// </summary>
        private TimegatedEvent HeroicStrikeEvent { get; }

        /// <summary>
        /// Executes the action for the bot.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target != null)
                {
                    double distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                    if (distanceToTarget > 3.0)
                    {
                        if (TryCastSpellWarrior(Warrior335a.Charge, Warrior335a.BattleStance, Bot.Wow.TargetGuid, true)
                            || TryCastSpellWarrior(Warrior335a.Intercept, Warrior335a.BerserkerStance, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if ((Bot.Target.HealthPercentage < 20 || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Sudden Death"))
                           && TryCastSpellWarrior(Warrior335a.Execute, Warrior335a.BattleStance, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if ((Bot.Objects.All.OfType<IWowUnit>().Where(e => Bot.Target.Position.GetDistance(e.Position) < 8).Count() > 2 && TryCastSpell(Warrior335a.Bladestorm, 0, true))
                            || TryCastSpellWarrior(Warrior335a.Overpower, Warrior335a.BattleStance, Bot.Wow.TargetGuid, true)
                            || TryCastSpellWarrior(Warrior335a.MortalStrike, Warrior335a.BattleStance, Bot.Wow.TargetGuid, true)
                            || (HeroicStrikeEvent.Run() && TryCastSpellWarrior(Warrior335a.HeroicStrike, Warrior335a.BattleStance, Bot.Wow.TargetGuid, true)))
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}