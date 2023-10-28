using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Linq;

/// <summary>
/// Namespace for the DeathknightBlood specialization, which focuses on tanking and dealing damage.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Class for the DeathknightBlood specialization, which focuses on tanking and dealing damage.
    /// </summary>
    public class DeathknightBlood : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the DeathknightBlood class.
        /// Initializes and adds active aura jobs for Blood Presence and Horn of Winter to the MyAuraManager.
        /// Initializes and adds active aura jobs for Frost Fever and Blood Plague to the TargetAuraManager.
        /// Sets the interrupt spells for the InterruptManager.
        /// Sets the BloodBoilEvent to a TimeSpan of 2 seconds.
        /// </summary>
        public DeathknightBlood(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Deathknight335a.BloodPresence, () => TryCastSpellDk(Deathknight335a.BloodPresence, 0)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Deathknight335a.HornOfWinter, () => TryCastSpellDk(Deathknight335a.HornOfWinter, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Deathknight335a.FrostFever, () => TryCastSpellDk(Deathknight335a.IcyTouch, Bot.Wow.TargetGuid, false, false, false, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Deathknight335a.BloodPlague, () => TryCastSpellDk(Deathknight335a.PlagueStrike, Bot.Wow.TargetGuid, false, false, false, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellDk(Deathknight335a.MindFreeze, x.Guid, true) },
                { 1, (x) => TryCastSpellDk(Deathknight335a.Strangulate, x.Guid, false, true) }
            };

            BloodBoilEvent = new(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Blood Deathknight spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Blood Deathknight spec.";

        /// <summary>
        /// Gets or sets the display name for a Deathknight Blood.
        /// </summary>
        public override string DisplayName2 => "Deathknight Blood";

        /// This property indicates that the class does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this object is considered to be melee.
        /// </summary>
        /// <returns>True, indicating that this object is considered to be melee.</returns>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for comparing items.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the role of the character as a Dps (damage per second) in the game World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// Represents a collection of talent trees available for a character.
        ///
        /// The "Talents" property is an override property of the base class "TalentTree".
        /// It returns a new instance of "TalentTree" with three talent trees: "Tree1", "Tree2", and "Tree3".
        /// Each talent tree is initialized with a dictionary of key-value pairs, where the key represents the talent level
        /// and the value represents a new instance of the "Talent" class with specific attribute values.
        /// The "Tree1" talent tree contains 22 key-value pairs, "Tree2" contains 2 key-value pairs, and "Tree3" contains 2 key-value pairs.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 3) },
                { 3, new(1, 3, 5) },
                { 4, new(1, 4, 5) },
                { 5, new(1, 5, 2) },
                { 6, new(1, 6, 2) },
                { 7, new(1, 7, 1) },
                { 8, new(1, 8, 5) },
                { 9, new(1, 9, 3) },
                { 13, new(1, 13, 3) },
                { 14, new(1, 14, 3) },
                { 16, new(1, 16, 3) },
                { 17, new(1, 17, 2) },
                { 18, new(1, 18, 3) },
                { 19, new(1, 19, 1) },
                { 21, new(1, 21, 2) },
                { 23, new(1, 23, 1) },
                { 24, new(1, 24, 3) },
                { 25, new(1, 25, 1) },
                { 26, new(1, 26, 3) },
                { 27, new(1, 27, 5) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 3, new(2, 3, 5) },
            },
            Tree3 = new()
            {
                { 3, new(3, 3, 5) },
                { 4, new(3, 4, 2) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether this entity can use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        /// <returns>The version.</returns>
        public override string Version => "1.0";

        /// Gets or sets a value indicating whether the character can walk behind an enemy. Returns false as the character cannot walk behind an enemy.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass for a Death Knight.
        /// </summary>
        public override WowClass WowClass => WowClass.Deathknight;

        /// <summary>
        /// Gets or sets the version of World of Warcraft as Wrath of the Lich King (3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets the TimegatedEvent for BloodBoil.
        /// </summary>
        private TimegatedEvent BloodBoilEvent { get; }

        /// This method overrides the Execute method and executes a series of actions based on certain conditions. It checks if a target can be found using the TargetProviderDps, and if so, it performs several actions such as casting spells, checking target distance, and checking player health percentage. Additionally, it counts the number of near enemies and performs actions based on the result.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid
                    && TryCastSpellDk(Deathknight335a.DarkCommand, Bot.Wow.TargetGuid))
                {
                    return;
                }

                if (Bot.Target.Position.GetDistance(Bot.Player.Position) > 6.0
                    && TryCastSpellDk(Deathknight335a.DeathGrip, Bot.Wow.TargetGuid, false, false, true))
                {
                    return;
                }

                if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Deathknight335a.ChainsOfIce)
                    && Bot.Target.Position.GetDistance(Bot.Player.Position) > 2.0
                    && TryCastSpellDk(Deathknight335a.ChainsOfIce, Bot.Wow.TargetGuid, false, false, true))
                {
                    return;
                }

                if (TryCastSpellDk(Deathknight335a.EmpowerRuneWeapon, 0))
                {
                    return;
                }

                int nearEnemies = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 12.0f).Count();

                if ((Bot.Player.HealthPercentage < 70.0 && TryCastSpellDk(Deathknight335a.RuneTap, 0, false, false, true))
                    || (Bot.Player.HealthPercentage < 60.0 && (TryCastSpellDk(Deathknight335a.IceboundFortitude, 0, true) || TryCastSpellDk(Deathknight335a.AntiMagicShell, 0, true)))
                    || (Bot.Player.HealthPercentage < 50.0 && TryCastSpellDk(Deathknight335a.VampiricBlood, 0, false, false, true))
                    || (nearEnemies > 2 && (TryCastAoeSpellDk(Deathknight335a.DeathAndDecay, 0) || (BloodBoilEvent.Run() && TryCastSpellDk(Deathknight335a.BloodBoil, 0))))
                    || TryCastSpellDk(Deathknight335a.UnbreakableArmor, 0, false, false, true)
                    || TryCastSpellDk(Deathknight335a.DeathStrike, Bot.Wow.TargetGuid, false, false, true, true)
                    || TryCastSpellDk(Deathknight335a.HeartStrike, Bot.Wow.TargetGuid, false, false, true)
                    || TryCastSpellDk(Deathknight335a.DeathCoil, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }
    }
}