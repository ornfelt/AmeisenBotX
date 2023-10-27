using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Bia10
{
    /// <summary>
    /// Initializes a new instance of the MageFrost class with the specified bot parameter.
    /// </summary>
    public class MageFrost : BasicCombatClassBia10
    {
        /// <summary>
        /// Initializes a new instance of the MageFrost class with the specified bot parameter.
        /// </summary>
        /// <param name="bot">The bot object implementing the AmeisenBotInterfaces.</param>
        public MageFrost(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.FrostArmor, () =>
                Bot.Player.ManaPercentage > 20.0
                && ValidateSpell(Mage335a.FrostArmor, true)
                && TryCastSpell(Mage335a.FrostArmor, Bot.Player.Guid)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ArcaneIntellect, () =>
                Bot.Player.ManaPercentage > 30.0
                && ValidateSpell(Mage335a.ArcaneIntellect, true)
                && TryCastSpell(Mage335a.ArcaneIntellect, Bot.Player.Guid)));
        }

        /// <summary>
        /// Gets the description of the CombatClass for the Frost Mage spec.
        /// </summary>
        public override string Description => "CombatClass for the Frost Mage spec.";

        /// <summary>
        /// Gets the display name for a Frost Mage.
        /// </summary>
        public override string DisplayName => "Frost Mage";

        /// <summary>
        /// Gets or sets a value indicating whether this instance handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is melee or not.
        /// </summary>
        /// <returns>False.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator used for comparing two items.
        /// </summary>
        /// <value>The item comparator.</value>
        public override IItemComparator ItemComparator { get; set; } =
                    new BasicIntellectComparator(null, new List<WowWeaponType>
                    {
                WowWeaponType.AxeTwoHand,
                WowWeaponType.MaceTwoHand,
                WowWeaponType.SwordTwoHand
                    });

        /// <summary>
        /// Gets or sets the role of the character as a damage dealer (DPS) in the game World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree object with three separate dictionaries for each tree.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new Dictionary<int, Talent>(),
            Tree2 = new Dictionary<int, Talent>(),
            Tree3 = new Dictionary<int, Talent>(),
        };

        /// This property indicates whether the entity is capable of using auto attacks.
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets or sets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// This property indicates that the player's character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property which represents the mage class.
        /// </summary>
        public override WowClass WowClass => WowClass.Mage;

        /// <summary>
        /// Executes the method by selecting a spell and casting it on a target.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            string spellName = SelectSpell(out ulong targetGuid);
            TryCastSpell(spellName, targetGuid);
        }

        /// <summary>
        /// Executes the action when out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();
        }

        /// <summary>
        /// Selects a spell based on various conditions and sets the targetGuid parameter to the target's GUID.
        /// </summary>
        /// <returns>
        /// The spell to be selected.
        /// </returns>
        private string SelectSpell(out ulong targetGuid)
        {
            if (IsInSpellRange(Bot.Target, Mage335a.FireBlast)
                && ValidateSpell(Mage335a.FireBlast, true)
                && Bot.Target.HealthPercentage > 10)
            {
                targetGuid = Bot.Target.Guid;
                return Mage335a.FireBlast;
            }
            if (IsInSpellRange(Bot.Target, Mage335a.Fireball)
                && ValidateSpell(Mage335a.Fireball, true)
                && !IsInSpellRange(Bot.Target, Mage335a.FrostBolt))
            {
                targetGuid = Bot.Target.Guid;
                return Mage335a.Fireball;
            }
            if (IsInSpellRange(Bot.Target, Mage335a.FrostBolt)
                && ValidateSpell(Mage335a.FrostBolt, true))
            {
                targetGuid = Bot.Target.Guid;
                return Mage335a.FrostBolt;
            }
            if (Bot.GetEnemiesOrNeutralsInCombatWithMe<IWowUnit>(Bot.Player.Position, 10).Count() >= 2
                || Bot.GetEnemiesOrNeutralsTargetingMe<IWowUnit>(Bot.Player.Position, 10).Count() >= 2
                && ValidateSpell(Mage335a.FrostNova, true))
            {
                targetGuid = 9999999;
                return Mage335a.FrostNova;
            }

            targetGuid = 9999999;
            return string.Empty;
        }
    }
}