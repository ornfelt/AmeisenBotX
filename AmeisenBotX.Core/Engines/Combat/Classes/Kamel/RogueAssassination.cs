using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    internal class RogueAssassination : BasicKamelClass
    {
        /// <summary>
        /// Initializes a new instance of the RogueAssassination class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public RogueAssassination(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets the name of the author.
        /// </summary>
        /// <returns>The name of the author.</returns>
        public override string Author => "Lukas";

        /// <summary>
        /// Gets or sets a dictionary with string keys and dynamic values.
        /// </summary>
        /// <value>
        /// The dictionary with string keys and dynamic values.
        /// </value>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the class, which is "Rogue Assassination".
        /// </summary>
        public override string Description => "Rogue Assassination";

        /// <summary>
        /// Gets the display name for the Rogue Assassination specialization.
        /// </summary>
        public override string DisplayName => "Rogue Assassination";

        /// <summary>
        /// Gets or sets a value indicating whether this object
        /// handles movement.
        /// </summary>
        /// <value>
        /// <c>false</c> if this object does not handle movement; otherwise, <c>true</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this character is a melee character.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for this object.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicAgilityComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets the role of the Wow object which is Dps.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree for the character.
        /// </summary>
        /// <value>
        /// The talent tree for the character.
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
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version string.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind the enemy.
        /// </summary>
        /// <returns>True if the player can walk behind the enemy, otherwise false.</returns>
        public override bool WalkBehindEnemy => true;

        /// <summary>
        /// Gets or sets the WowClass property, representing the specific class of a character in the game World of Warcraft. 
        /// In this case, it is set to WowClass.Rogue, indicating that the character is a rogue.
        /// </summary>
        public override WowClass WowClass => WowClass.Rogue;

        /// <summary>
        /// Executes the CC attack by calling the StartAttack method.
        /// </summary>
        public override void ExecuteCC()
        {
            StartAttack();
        }

        /// <summary>
        /// Executes the OutOfCombatExecute method.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            Targetselection();
            StartAttack();
        }

        /// <summary>
        /// Starts the attack if the target guid is not 0. If the target's reaction is friendly, clear the target. If the player is in melee range of the target and is not already auto-attacking, start auto-attack.
        /// </summary>
        private void StartAttack()
        {
            if (Bot.Wow.TargetGuid != 0)
            {
                ChangeTargetToAttack();

                if (Bot.Db.GetReaction(Bot.Player, Bot.Target) == WowUnitReaction.Friendly)
                {
                    Bot.Wow.ClearTarget();
                    return;
                }

                if (Bot.Player.IsInMeleeRange(Bot.Target))
                {
                    if (!Bot.Player.IsAutoAttacking && AutoAttackEvent.Run())
                    {
                        Bot.Wow.StartAutoAttack();
                    }
                }
            }
        }
    }
}