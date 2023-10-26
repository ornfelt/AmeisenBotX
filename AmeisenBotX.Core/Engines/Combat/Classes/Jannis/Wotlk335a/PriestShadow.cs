using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class PriestShadow : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the PriestShadow class.
        /// Adds jobs to the MyAuraManager and TargetAuraManager to keep certain auras active.
        /// Also adds a spell to be kept active on the party members through the GroupAuraManager.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for interaction with the bot.</param>
        public PriestShadow(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.Shadowform, () => TryCastSpell(Priest335a.Shadowform, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.PowerWordFortitude, () => TryCastSpell(Priest335a.PowerWordFortitude, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.VampiricEmbrace, () => TryCastSpell(Priest335a.VampiricEmbrace, Bot.Wow.PlayerGuid, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.VampiricTouch, () => TryCastSpell(Priest335a.VampiricTouch, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.DevouringPlague, () => TryCastSpell(Priest335a.DevouringPlague, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.ShadowWordPain, () => TryCastSpell(Priest335a.ShadowWordPain, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.MindBlast, () => TryCastSpell(Priest335a.MindBlast, Bot.Wow.TargetGuid, true)));

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Priest335a.PowerWordFortitude, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Shadow Priest spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Shadow Priest spec.";

        /// <summary>
        /// Gets or sets the display name of the Priest Shadow.
        /// </summary>
        public override string DisplayName2 => "Priest Shadow";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this object handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => false;

        ///<summary>
        /// Gets or sets a value indicating whether the character is a melee or ranged combatant.
        ///</summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the current object. The item comparator is set to a new instance of BasicIntellectComparator with specified armor and weapon types.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the role of the player character as a damage dealer (DPS) in the game World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the TalentTree property.
        /// </summary>
        /// <remarks>
        /// The TalentTree property represents the available talent trees for an object.
        /// </remarks>
        /// <value>
        /// The TalentTree property is a dictionary that contains the different talent trees.
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 4, new(1, 4, 3) },
                { 5, new(1, 5, 2) },
                { 7, new(1, 7, 3) },
            },
            Tree2 = new(),
            Tree3 = new()
            {
                { 1, new(3, 1, 3) },
                { 2, new(3, 2, 2) },
                { 3, new(3, 3, 5) },
                { 5, new(3, 5, 2) },
                { 6, new(3, 6, 3) },
                { 8, new(3, 8, 5) },
                { 9, new(3, 9, 1) },
                { 10, new(3, 10, 2) },
                { 11, new(3, 11, 2) },
                { 12, new(3, 12, 3) },
                { 14, new(3, 14, 1) },
                { 16, new(3, 16, 3) },
                { 17, new(3, 17, 2) },
                { 18, new(3, 18, 3) },
                { 19, new(3, 19, 1) },
                { 20, new(3, 20, 5) },
                { 21, new(3, 21, 2) },
                { 22, new(3, 22, 3) },
                { 24, new(3, 24, 1) },
                { 25, new(3, 25, 3) },
                { 26, new(3, 26, 5) },
                { 27, new(3, 27, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character can use auto attacks.
        /// </summary>
        /// <returns><c>true</c> if the character can use auto attacks; otherwise, <c>false</c>.</returns>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets or sets the version of the code.
        /// </summary>
        public override string Version => "1.1";

        /// <summary>
        /// Gets or sets a value indicating whether the player is able to walk behind an enemy.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass of the character as Priest.
        /// </summary>
        public override WowClass WowClass => WowClass.Priest;

        /// <summary>
        /// Gets or sets the World of Warcraft version to WotLK335a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Method that executes a series of conditional spell casts based on the current state of the player character.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Player.ManaPercentage < 90
                    && TryCastSpell(Priest335a.Shadowfiend, Bot.Wow.TargetGuid))
                {
                    return;
                }

                if (Bot.Player.ManaPercentage < 30
                    && TryCastSpell(Priest335a.HymnOfHope, 0))
                {
                    return;
                }

                if (Bot.Player.HealthPercentage < 70
                    && TryCastSpell(Priest335a.FlashHeal, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (!Bot.Player.IsCasting
                    && TryCastSpell(Priest335a.MindFlay, Bot.Wow.TargetGuid, true))
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
        /// Executes the OutOfCombatExecute method.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HandleDeadPartymembers(Priest335a.Resurrection))
            {
                return;
            }
        }
    }
}