using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Bia10
{
    /// <summary>
    /// Constructor for the PriestShadow class. Initializes and adds jobs to the MyAuraManager and TargetAuraManager. These jobs keep track of and cast various spells based on certain conditions.
    /// </summary>
    public class PriestShadow : BasicCombatClassBia10
    {
        /// <summary>
        /// Constructor for the PriestShadow class. Initializes and adds jobs to the MyAuraManager and TargetAuraManager. These jobs keep track of and cast various spells based on certain conditions.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used in the constructor.</param>
        public PriestShadow(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.PowerWordFortitude, () =>
                Bot.Player.ManaPercentage > 60.0
                && ValidateSpell(Priest335a.PowerWordFortitude, true)
                && TryCastSpell(Priest335a.PowerWordFortitude, Bot.Player.Guid)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.PowerWordShield, () =>
                Bot.Player.Auras.All(e => Bot.Db.GetSpellName(e.SpellId) != "Weakened Soul")
                && Bot.Player.ManaPercentage > 60.0
                && ValidateSpell(Priest335a.PowerWordShield, true)
                && TryCastSpell(Priest335a.PowerWordShield, Bot.Player.Guid)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.InnerFire, () =>
                Bot.Player.ManaPercentage > 60.0
                && ValidateSpell(Priest335a.InnerFire, true)
                && TryCastSpell(Priest335a.InnerFire, Bot.Player.Guid)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.Renew, () =>
                Bot.Player.Auras.All(e => Bot.Db.GetSpellName(e.SpellId) != Priest335a.Renew)
                && Bot.Player.HealthPercentage < 85 && Bot.Player.ManaPercentage > 65.0
                && ValidateSpell(Priest335a.Renew, true)
                && TryCastSpell(Priest335a.Renew, Bot.Player.Guid)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Priest335a.ShadowWordPain, () =>
                Bot.Target?.HealthPercentage >= 5
                && ValidateSpell(Priest335a.ShadowWordPain, true)
                && TryCastSpell(Priest335a.ShadowWordPain, Bot.Wow.TargetGuid)));
        }

        /// <summary>
        /// Gets the description of the CombatClass for the Shadow Priest spec.
        /// </summary>
        public override string Description => "CombatClass for the Shadow Priest spec.";

        /// <summary>
        /// Gets the display name for a Shadow Priest.
        /// </summary>
        public override string DisplayName => "Shadow Priest";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> if this object does not handle movement; otherwise, <c>true</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this character is a melee character.
        /// </summary>
        /// <returns>Returns false indicating that this character is not a melee character.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the class.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } =
                    new BasicIntellectComparator(null, new List<WowWeaponType>
                    {
                WowWeaponType.AxeTwoHand,
                WowWeaponType.MaceTwoHand,
                WowWeaponType.SwordTwoHand
                    });

        /// <summary>
        /// Gets or sets the role of the WowPlayer character as DPS (damage per second).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent trees for the object.
        /// </summary>
        /// <value>The talent trees.</value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new Dictionary<int, Talent>(),
            Tree2 = new Dictionary<int, Talent>(),
            Tree3 = new Dictionary<int, Talent>(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code as a string.
        /// </summary>
        /// <returns>The version of the code.</returns>
        public override string Version => "1.0";

        /// This property indicates that the character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass for the Priest.
        /// </summary>
        public override WowClass WowClass => WowClass.Priest;

        /// <summary>
        /// Executes the code by selecting a spell and casting it on the target.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            string spellName = SelectSpell(out ulong targetGuid);
            TryCastSpell(spellName, targetGuid);
        }

        /// Executes the out of combat behavior, including handling dead party members by using the specified resurrection method.
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();
            HandleDeadPartyMembers(Priest335a.Resurrection);
        }

        /// <summary>
        /// Selects a spell based on specific conditions and returns the spell name. Also assigns the target's GUID to the 'targetGuid' parameter.
        /// </summary>
        /// <param name="targetGuid">The variable that will hold the target's GUID.</param>
        /// <returns>The name of the selected spell, or an empty string if no spell is selected.</returns>
        private string SelectSpell(out ulong targetGuid)
        {
            if (Bot.Player.HealthPercentage < DataConstants.HealSelfPercentage
                && ValidateSpell(Priest335a.LesserHeal, true))
            {
                targetGuid = Bot.Player.Guid;
                return Priest335a.LesserHeal;
            }
            if (IsInSpellRange(Bot.Target, Priest335a.MindBlast)
                && ValidateSpell(Priest335a.MindBlast, true))
            {
                targetGuid = Bot.Target.Guid;
                return Priest335a.MindBlast;
            }
            if (IsInSpellRange(Bot.Target, Priest335a.Smite)
                && ValidateSpell(Priest335a.Smite, true))
            {
                targetGuid = Bot.Target.Guid;
                return Priest335a.Smite;
            }

            targetGuid = 9999999;
            return string.Empty;
        }
    }
}