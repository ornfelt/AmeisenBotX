using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

/// <summary>
/// The AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a namespace contains classes related to combat functionality for the AmeisenBotX bot, specifically for the Wotlk335a version.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a DeathknightFrost class that extends BasicCombatClass. This class is responsible for managing various AuraManager and InterruptManager jobs for the bot object.
    /// </summary>
    public class DeathknightFrost : BasicCombatClass
    {
        /// Constructor for a DeathknightFrost object that takes in a bot object and initializes various AuraManager and InterruptManager jobs.
        public DeathknightFrost(AmeisenBotInterfaces bot) : base(bot)
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
        }

        /// <summary>
        /// Gets the description for the FCFS based CombatClass for the Frost Deathknight spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Frost Deathknight spec.";

        /// <summary>
        /// Gets or sets the display name for a Deathknight Frost.
        /// </summary>
        public override string DisplayName2 => "Deathknight Frost";

        /// <summary>
        /// Gets or sets a value indicating whether this instance handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is a melee character.
        /// </summary>
        /// <returns>True if the character is a melee character; otherwise, false.</returns>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items. 
        /// The default value is a BasicStrengthComparator that only compares items of type WowArmorType.Shield.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the role of the player character in World of Warcraft, which is Damage per Second (DPS).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// Returns the TalentTree instance for the code, which consists of three trees (Tree1, Tree2, Tree3) with 
        /// different talent configurations. Each tree is represented by a dictionary, where the key is the talent node 
        /// index and the value is an instance of the Talent class, which contains three talents (talent1, talent2, talent3) 
        /// represented by integers.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 3) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 2, new(2, 2, 2) },
                { 5, new(2, 5, 2) },
                { 6, new(2, 6, 3) },
                { 7, new(2, 7, 5) },
                { 9, new(2, 9, 3) },
                { 10, new(2, 10, 5) },
                { 11, new(2, 11, 2) },
                { 12, new(2, 12, 2) },
                { 14, new(2, 14, 3) },
                { 16, new(2, 16, 1) },
                { 17, new(2, 17, 2) },
                { 18, new(2, 18, 3) },
                { 22, new(2, 22, 3) },
                { 23, new(2, 23, 3) },
                { 24, new(2, 24, 1) },
                { 26, new(2, 26, 1) },
                { 27, new(2, 27, 3) },
                { 28, new(2, 28, 5) },
                { 29, new(2, 29, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 2, new(3, 2, 3) },
                { 4, new(3, 4, 2) },
                { 7, new(3, 7, 3) },
                { 9, new(3, 9, 5) },
            },
        };

        /// The property determines whether the object can use auto attacks. It returns true.
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// This property indicates that the character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass of a Death Knight.
        /// </summary>
        public override WowClass WowClass => WowClass.Deathknight;

        /// <summary>
        /// Gets the World of Warcraft version as WotLK 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// This method executes a series of actions for a Death Knight character in World of Warcraft. It first calls the base Execute method. Then, it checks if a target can be found using the TargetProviderDps, and if so, it attempts to cast the Dark Command spell on the target. If that fails or the target has the Chains of Ice aura, it attempts to cast the Chains of Ice spell on the target if the target is not in close proximity to the player. If the target has the Chains of Ice aura, it also attempts to cast the Chains of Ice spell on the target. Then, it tries to cast the Empower Rune Weapon spell with a rune count of 0. After that, it evaluates several conditions for different spells to cast based on the player's health percentage, available resources, and the target's status.
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

                if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Deathknight335a.ChainsOfIce)
                    && Bot.Target.Position.GetDistance(Bot.Player.Position) > 2.0
                    && TryCastSpellDk(Deathknight335a.ChainsOfIce, Bot.Wow.TargetGuid, false, false, true))
                {
                    return;
                }

                if (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Deathknight335a.ChainsOfIce)
                    && TryCastSpellDk(Deathknight335a.ChainsOfIce, Bot.Wow.TargetGuid, false, false, true))
                {
                    return;
                }

                if (TryCastSpellDk(Deathknight335a.EmpowerRuneWeapon, 0))
                {
                    return;
                }

                if ((Bot.Player.HealthPercentage < 60
                        && TryCastSpellDk(Deathknight335a.IceboundFortitude, 0, true))
                    || TryCastSpellDk(Deathknight335a.UnbreakableArmor, 0, false, false, true)
                    || TryCastSpellDk(Deathknight335a.Obliterate, Bot.Wow.TargetGuid, false, false, true, true)
                    || TryCastSpellDk(Deathknight335a.BloodStrike, Bot.Wow.TargetGuid, false, true)
                    || TryCastSpellDk(Deathknight335a.DeathCoil, Bot.Wow.TargetGuid, true)
                    || (Bot.Player.RunicPower > 60
                        && TryCastSpellDk(Deathknight335a.RuneStrike, Bot.Wow.TargetGuid)))
                {
                    return;
                }
            }
        }
    }
}