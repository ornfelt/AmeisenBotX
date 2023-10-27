using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class DruidFeralCat : BasicCombatClass
    {
        /// This constructor initializes a DruidFeralCat object and sets up various jobs and spell configurations for the bot. It takes in a parameter 'bot' of type AmeisenBotInterfaces to provide access to necessary functionalities.
        public DruidFeralCat(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MarkOfTheWild, () => TryCastSpell(Druid335a.MarkOfTheWild, Bot.Wow.PlayerGuid, true, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.CatForm, () => TryCastSpell(Druid335a.CatForm, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.SavageRoar, () => TryCastSpellRogue(Druid335a.SavageRoar, Bot.Wow.TargetGuid, true, true, 1)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.Rip, () => Bot.Player.ComboPoints == 5 && TryCastSpellRogue(Druid335a.Rip, Bot.Wow.TargetGuid, true, true, 5)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.Rake, () => TryCastSpell(Druid335a.Rake, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MangleCat, () => TryCastSpell(Druid335a.MangleCat, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Druid335a.FaerieFire, x.Guid, true) },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Druid335a.MarkOfTheWild, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Feral (Cat) Druid spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Feral (Cat) Druid spec.";

        /// <summary>
        /// Gets the display name for a Druid Feral Cat.
        /// </summary>
        public override string DisplayName2 => "Druid Feral Cat";

        /// This property indicates that the current code does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is a melee fighter.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for the code, initialized with a BasicAgilityComparator object that compares items with shield armor type and sword, mace, and axe weapon types.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicAgilityComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the role of the character in the World of Warcraft game, which is a damage-per-second (DPS) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Overrides the Talents property and initializes it with a new instance of the TalentTree class.
        /// The Talents property consists of three trees: Tree1, Tree2, and Tree3.
        /// Tree1 is empty.
        /// 
        /// Tree2 consists of multiple key-value pairs. Each key represents a specific talent node and each value represents the attributes of that talent node.
        /// The attributes include the tree ID, the talent ID, and the number of points invested in the talent.
        /// The talent nodes in Tree2 are as follows:
        /// 1. Talent ID: 2, Tree ID: 1, Points: 5
        /// 2. Talent ID: 2, Tree ID: 2, Points: 5
        /// 3. Talent ID: 2, Tree ID: 4, Points: 2
        /// 4. Talent ID: 2, Tree ID: 6, Points: 2
        /// 5. Talent ID: 2, Tree ID: 7, Points: 1
        /// 6. Talent ID: 2, Tree ID: 8, Points: 3
        /// 7. Talent ID: 2, Tree ID: 9, Points: 2
        /// 8. Talent ID: 2, Tree ID: 10, Points: 3
        /// 9. Talent ID: 2, Tree ID: 11, Points: 2
        /// 10. Talent ID: 2, Tree ID: 12, Points: 2
        /// 11. Talent ID: 2, Tree ID: 14, Points: 1
        /// 12. Talent ID: 2, Tree ID: 17, Points: 5
        /// 13. Talent ID: 2, Tree ID: 18, Points: 3
        /// 14. Talent ID: 2, Tree ID: 19, Points: 1
        /// 15. Talent ID: 2, Tree ID: 20, Points: 2
        /// 16. Talent ID: 2, Tree ID: 23, Points: 3
        /// 17. Talent ID: 2, Tree ID: 25, Points: 3
        /// 18. Talent ID: 2, Tree ID: 26, Points: 1
        /// 19. Talent ID: 2, Tree ID: 28, Points: 5
        /// 20. Talent ID: 2, Tree ID: 29, Points: 1
        /// 21. Talent ID: 2, Tree ID: 30, Points: 1
        /// 
        /// Tree3 consists of multiple key-value pairs. Each key represents a specific talent node and each value represents the attributes of that talent node.
        /// The attributes include the tree ID, the talent ID, and the number of points invested in the talent.
        /// The talent nodes in Tree3 are as follows:
        /// 1. Talent ID: 3, Tree ID: 1, Points: 2
        /// 2. Talent ID: 3, Tree ID: 3, Points: 5
        /// 3. Talent ID: 3, Tree ID: 4, Points: 5
        /// 4. Talent ID: 3, Tree ID: 6, Points: 3
        /// 5. Talent ID: 3, Tree ID: 8, Points: 1
        /// 6. Talent ID: 3, Tree ID: 9, Points: 2
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 1, new(2, 1, 5) },
                { 2, new(2, 2, 5) },
                { 4, new(2, 4, 2) },
                { 6, new(2, 6, 2) },
                { 7, new(2, 7, 1) },
                { 8, new(2, 8, 3) },
                { 9, new(2, 9, 2) },
                { 10, new(2, 10, 3) },
                { 11, new(2, 11, 2) },
                { 12, new(2, 12, 2) },
                { 14, new(2, 14, 1) },
                { 17, new(2, 17, 5) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 1) },
                { 20, new(2, 20, 2) },
                { 23, new(2, 23, 3) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 1) },
                { 28, new(2, 28, 5) },
                { 29, new(2, 29, 1) },
                { 30, new(2, 30, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 3, new(3, 3, 5) },
                { 4, new(3, 4, 5) },
                { 6, new(3, 6, 3) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 2) },
            },
        };

        /// This property specifies that auto attacks can be used in this context.
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        /// <value>The version string.</value>
        public override string Version => "1.0";

        /// Determines if the player character can walk behind enemy characters. Returns true.
        public override bool WalkBehindEnemy => true;

        /// <summary>
        /// Gets or sets the <see cref="WowClass"/> of the character as a Druid.
        /// </summary>
        public override WowClass WowClass => WowClass.Druid;

        /// <summary>
        /// Gets or sets the version of World of Warcraft to WotLK 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Executes a series of actions based on the current state of the player and target.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                double distanceToTarget = Bot.Player.Position.GetDistance(Bot.Target.Position);

                if (distanceToTarget > 9.0
                    && TryCastSpell(Druid335a.FeralChargeBear, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (distanceToTarget > 8.0
                    && TryCastSpell(Druid335a.Dash, 0))
                {
                    return;
                }

                if (Bot.Player.HealthPercentage < 40
                    && TryCastSpell(Druid335a.SurvivalInstincts, 0, true))
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

                if ((Bot.Player.EnergyPercentage > 70
                        && TryCastSpell(Druid335a.Berserk, 0))
                    || (Bot.Player.Energy < 30
                        && TryCastSpell(Druid335a.TigersFury, 0))
                    || (Bot.Player.HealthPercentage < 70
                        && TryCastSpell(Druid335a.Barkskin, 0, true))
                    || (Bot.Player.HealthPercentage < 35
                        && TryCastSpell(Druid335a.SurvivalInstincts, 0, true))
                    || (Bot.Player.ComboPoints == 5
                        && TryCastSpellRogue(Druid335a.FerociousBite, Bot.Wow.TargetGuid, true, true, 5))
                    || TryCastSpell(Druid335a.Shred, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the out of combat behavior, including healing if necessary.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealMySelf())
            {
                return;
            }
        }

        /// Determines if the player needs to heal themselves based on their current health percentage and available spells.
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