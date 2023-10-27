using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Initializes a new instance of the RogueAssassination class with the specified AmeisenBotInterfaces object.
    /// </summary>
    public class RogueAssassination : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the RogueAssassination class with the specified AmeisenBotInterfaces object.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public RogueAssassination(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Rogue335a.SliceAndDice, () => TryCastSpellRogue(Rogue335a.SliceAndDice, 0, true, true, 1)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Rogue335a.ColdBlood, () => TryCastSpellRogue(Rogue335a.ColdBlood, 0, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellRogue(Rogue335a.Kick, x.Guid, true) }
            };
        }

        /// <summary>
        /// This property represents the description of the FCFS (First Come, First Serve) based CombatClass
        /// for the Assasination Rogue specialization.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Assasination Rogue spec.";

        /// <summary>
        /// Gets or sets the display name of the character, indicating that it is a Work In Progress and 
        /// the character is a Rogue specialized in the Assasination talent tree.
        /// </summary>
        public override string DisplayName2 => "[WIP] Rogue Assasination";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> because this object does not handle movement.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets or sets a value indicating whether the object is a melee weapon.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the ItemComparator used for comparing items.
        /// The default value is a BasicAgilityComparator with a WowArmorType of Shield.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicAgilityComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets the role of the Wow character as Dps (damage per second).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree.
        /// </summary>
        /// <value>
        /// The talent tree.
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 3, new(1, 3, 2) },
                { 4, new(1, 4, 5) },
                { 5, new(1, 5, 2) },
                { 6, new(1, 6, 3) },
                { 9, new(1, 9, 5) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 5) },
                { 13, new(1, 13, 1) },
                { 16, new(1, 16, 5) },
                { 17, new(1, 17, 2) },
                { 19, new(1, 19, 1) },
                { 21, new(1, 21, 3) },
                { 22, new(1, 22, 3) },
                { 23, new(1, 23, 3) },
                { 24, new(1, 24, 1) },
                { 26, new(1, 26, 5) },
                { 27, new(1, 27, 1) },
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 6, new(2, 6, 5) },
                { 9, new(2, 9, 3) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 3, new(3, 3, 2) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether this character is able to use auto attacks.
        /// </summary>
        /// <value>
        /// <c>true</c> if this character can use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => true;

        /// <summary>Gets the version of the implementation.</summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind an enemy.
        /// </summary>
        /// <returns>True if the character can walk behind an enemy, otherwise false.</returns>
        public override bool WalkBehindEnemy => true;

        /// <summary>
        /// Gets or sets the WoW class of the character as a Rogue.
        /// </summary>
        public override WowClass WowClass => WowClass.Rogue;

        /// <summary>
        /// Gets the World of Warcraft version as WotLK 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Executes the rogue's combat rotation.
        /// If the target provider returns a valid target, the rogue will prioritize using defensive abilities
        /// such as Cloak of Shadows when the player's health is below 20%.
        /// If the player is not within melee range of the target, the rogue will attempt to use Sprint to close the distance.
        /// The rogue will then attempt to use Eviscerate or Mutilate as their primary damage abilities.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if ((Bot.Player.HealthPercentage < 20
                        && TryCastSpellRogue(Rogue335a.CloakOfShadows, 0, true)))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if ((Bot.Target.Position.GetDistance(Bot.Player.Position) > 16
                            && TryCastSpellRogue(Rogue335a.Sprint, 0, true)))
                    {
                        return;
                    }
                }

                if (TryCastSpellRogue(Rogue335a.Eviscerate, Bot.Wow.TargetGuid, true, true, 5)
                    || TryCastSpellRogue(Rogue335a.Mutilate, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }
    }
}