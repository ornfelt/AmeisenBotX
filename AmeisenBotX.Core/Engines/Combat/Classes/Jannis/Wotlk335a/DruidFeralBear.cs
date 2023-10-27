using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Initializes a new instance of the DruidFeralBear class and sets up the necessary aura and spell management for the bot.
    /// </summary>
    public class DruidFeralBear : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the DruidFeralBear class and sets up the necessary aura and spell management for the bot.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance to use for aura and spell management.</param>
        public DruidFeralBear(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MarkOfTheWild, () => TryCastSpell(Druid335a.MarkOfTheWild, Bot.Wow.PlayerGuid, true, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.DireBearForm, () => TryCastSpell(Druid335a.DireBearForm, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MangleBear, () => TryCastSpell(Druid335a.MangleBear, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Druid335a.Bash, x.Guid, true) },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Druid335a.MarkOfTheWild, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// This method overrides the Description property and returns a string representing the FCFS based CombatClass for the Feral (Bear) Druid spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Feral (Bear) Druid spec.";

        /// <summary>
        /// The display name for a Feral Bear Druid.
        /// </summary>
        public override string DisplayName2 => "Druid Feral Bear";

        /// This property indicates that the class does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the code is for a melee action.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicArmorComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the role of the character as a tank in World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Tank;

        /// Initializes a new instance of the TalentTree class with predefined values for each Tree. 
        /// Tree1 is empty, Tree2 contains 28 Talent objects with associated identifiers and corresponding attributes, 
        /// and Tree3 contains 4 Talent objects with associated identifiers and corresponding attributes.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 1, new(2, 1, 5) },
                { 3, new(2, 3, 1) },
                { 4, new(2, 4, 2) },
                { 5, new(2, 5, 3) },
                { 6, new(2, 6, 2) },
                { 7, new(2, 7, 1) },
                { 8, new(2, 8, 3) },
                { 10, new(2, 10, 3) },
                { 11, new(2, 11, 2) },
                { 12, new(2, 12, 2) },
                { 13, new(2, 13, 1) },
                { 14, new(2, 14, 1) },
                { 16, new(2, 16, 3) },
                { 17, new(2, 17, 5) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 1) },
                { 20, new(2, 20, 2) },
                { 22, new(2, 22, 3) },
                { 24, new(2, 24, 3) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 1) },
                { 27, new(2, 27, 3) },
                { 28, new(2, 28, 5) },
                { 29, new(2, 29, 1) },
                { 30, new(2, 30, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 3, new(3, 3, 3) },
                { 4, new(3, 4, 5) },
                { 8, new(3, 8, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// This property indicates that the character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class for Druid.
        /// </summary>
        public override WowClass WowClass => WowClass.Druid;

        /// <summary>
        /// Gets or sets the World of Warcraft version as Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// This method is responsible for executing a series of actions for the Druid class. It first calls the base.Execute() method. If the target is found using the TargetProviderDps, it calculates the distance to the target. If the distance is greater than 9.0, it tries to cast the FeralChargeBear spell on the target. If the player's health percentage is less than 40, it tries to cast the SurvivalInstincts spell. If the target's target is not the player, it tries to cast the Growl spell. It then tries to cast the Berserk spell. If the player needs to heal themselves, it returns immediately. The method then calculates the number of near enemies within a 10 unit range. If the player's health percentage is above 80 and the Enrage spell can be cast, it tries to cast it. Otherwise, if the player's health percentage is below 70, it tries to cast the Barkskin spell. If the player's health percentage is below 75, it tries to cast the FrenziedRegeneration spell. If there are more than 2 near enemies, it tries to cast the ChallengingRoar spell. It then tries to cast the Lacerate spell on the target or the Swipe spell if there are more than 2 near enemies. Finally, it tries to cast the MangleBear spell on the target.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                double distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                if (distanceToTarget > 9.0
                    && TryCastSpell(Druid335a.FeralChargeBear, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (Bot.Player.HealthPercentage < 40
                    && TryCastSpell(Druid335a.SurvivalInstincts, 0, true))
                {
                    return;
                }

                if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid
                    && TryCastSpell(Druid335a.Growl, 0, true))
                {
                    return;
                }

                if (TryCastSpell(Druid335a.Berserk, 0))
                {
                    return;
                }

                if (NeedToHealMySelf())
                {
                    return;
                }

                int nearEnemies = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 10).Count();

                if ((Bot.Player.HealthPercentage > 80
                        && TryCastSpell(Druid335a.Enrage, 0, true))
                    || (Bot.Player.HealthPercentage < 70
                        && TryCastSpell(Druid335a.Barkskin, 0, true))
                    || (Bot.Player.HealthPercentage < 75
                        && TryCastSpell(Druid335a.FrenziedRegeneration, 0, true))
                    || (nearEnemies > 2 && TryCastSpell(Druid335a.ChallengingRoar, 0, true))
                    || TryCastSpell(Druid335a.Lacerate, Bot.Wow.TargetGuid, true)
                    || (nearEnemies > 2 && TryCastSpell(Druid335a.Swipe, 0, true))
                    || TryCastSpell(Druid335a.MangleBear, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the code when the character is out of combat.
        /// Checks if the character needs to heal themselves and returns if true.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealMySelf())
            {
                return;
            }
        }

        /// Checks if the player needs to heal themselves. Returns true if a healing spell should be casted, false otherwise.
        private bool NeedToHealMySelf()
        {
            if (Bot.Player.HealthPercentage < 60
                && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Rejuvenation)
                && TryCastSpell(Druid335a.Rejuvenation, 0, true))
            {
                return true;
            }

            if (Bot.Player.HealthPercentage < 40
                && TryCastSpell(Druid335a.HealingTouch, 0, true))
            {
                return true;
            }

            return false;
        }
    }
}