using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Bia10
{
    public class WarrirorFurry : BasicCombatClassBia10
    {
        /// <summary>
        /// Initializes a new instance of the WarriorFurry class with the specified AmeisenBotInterfaces object as a parameter. This constructor adds two KeepActiveAuraJob objects to the MyAuraManager.Jobs list. The first job keeps the BattleShout aura active as long as the following conditions are met: all of the player's auras do not have the spell ID for BattleShout, the player has more than 10.0 Rage points, the ValidateSpell method returns true for BattleShout, and the TryCastSpell method successfully casts BattleShout on the player's GUID. The second job keeps the Rend aura active as long as the following conditions are met: the target's health percentage is equal to or greater than 10, the ValidateSpell method returns true for Rend, and the TryCastSpell method successfully casts Rend on the target's GUID. Additionally, the constructor adds a third KeepActiveAuraJob object to the TargetAuraManager.Jobs list. This job keeps the ThunderClap aura active as long as the following conditions are met: none of the target's auras have the spell ID for ThunderClap, all of the player's auras have the spell ID for BattleShout, the target's health percentage is equal to or greater than 10, the player has more than 30 Rage points, the ValidateSpell method returns true for ThunderClap, and the TryCastSpell method successfully casts ThunderClap on the target's GUID.
        /// </summary>
        public WarrirorFurry(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.BattleShout, () =>
                Bot.Player.Auras.All(e => Bot.Db.GetSpellName(e.SpellId) != Warrior335a.BattleShout)
                && Bot.Player.Rage > 10.0
                && ValidateSpell(Warrior335a.BattleShout, true)
                && TryCastSpell(Warrior335a.BattleShout, Bot.Player.Guid)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.Rend, () =>
                Bot.Target?.HealthPercentage >= 10
                && ValidateSpell(Warrior335a.Rend, true)
                && TryCastSpell(Warrior335a.Rend, Bot.Wow.TargetGuid)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.ThunderClap, () =>
                Bot.Target.Auras.All(e => Bot.Db.GetSpellName(e.SpellId) != Warrior335a.ThunderClap)
                && Bot.Player.Auras.All(e => Bot.Db.GetSpellName(e.SpellId) == Warrior335a.BattleShout)
                && Bot.Target?.HealthPercentage >= 10 && Bot.Player.Rage > 30
                && ValidateSpell(Warrior335a.ThunderClap, true)
                && TryCastSpell(Warrior335a.ThunderClap, Bot.Wow.TargetGuid)));
        }

        /// <summary>
        /// Gets the description of the CombatClass for the Warrior Furry spec.
        /// </summary>
        public override string Description => "CombatClass for the Warrior Furry spec.";

        /// <summary>
        /// Gets or sets the display name of the warrior furry.
        /// </summary>
        public override string DisplayName => "Warrior Furry";

        /// This property indicates that the class does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this instance is a melee attack.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items based on their strength. 
        /// The default comparator is set to BasicStrengthComparator with null faction filter and an empty list of weapon types.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } =
                    new BasicStrengthComparator(null, new List<WowWeaponType>());

        /// <summary>
        /// Gets or sets the role of the character, which is set to Dps.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree with three different trees represented as dictionaries.
        /// </summary>
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
        /// Gets the version number as a string.
        /// </summary>
        public override string Version => "1.0";

        /// This property indicates that the character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property, representing the playable class of a warrior in the game.
        /// </summary>
        public override WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Executes the spell casting action by selecting a spell and casting it on a target.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            string spellName = SelectSpell(out ulong targetGuid);
            TryCastSpell(spellName, targetGuid);
        }

        /// <summary>
        /// This method is called when the character is out of combat and is used to execute any actions or logic specific to this state. It calls the base class' OutOfCombatExecute method.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();
        }

        /// <summary>
        /// Selects a spell based on certain conditions and sets the targetGuid to the Guid of the target.
        /// </summary>
        /// <param name="targetGuid">The Guid of the target.</param>
        /// <returns>The selected spell.</returns>
        private string SelectSpell(out ulong targetGuid)
        {
            // todo: bot doesn't understand the condition
            if (Bot.Player.Rage < 15 && IsInSpellRange(Bot.Target, Warrior335a.VictoryRush)
                && ValidateSpell(Warrior335a.VictoryRush, true))
            {
                targetGuid = Bot.Target.Guid;
                return Warrior335a.VictoryRush;
            }
            if (IsInSpellRange(Bot.Target, Warrior335a.HeroicStrike)
                && ValidateSpell(Warrior335a.HeroicStrike, true))
            {
                targetGuid = Bot.Target.Guid;
                return Warrior335a.HeroicStrike;
            }
            if (IsInSpellRange(Bot.Target, Warrior335a.Charge)
                && ValidateSpell(Warrior335a.Charge, true))
            {
                targetGuid = Bot.Target.Guid;
                return Warrior335a.Charge;
            }

            targetGuid = 9999999;
            return string.Empty;
        }
    }
}