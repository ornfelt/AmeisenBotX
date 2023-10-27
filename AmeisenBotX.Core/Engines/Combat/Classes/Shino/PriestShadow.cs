using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Logic.CombatClasses.Shino;
using AmeisenBotX.Core.Managers.Character;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Shino
{
    /// <summary>
    /// Represents a shadow specialization of the Priest class.
    /// </summary>
    public class PriestShadow : TemplateCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the PriestShadow class.
        /// Adds a KeepActiveAuraJob for the Priest335a.Shadowform spell, ensuring it stays active.
        /// Adds a KeepActiveAuraJob for the Priest335a.PowerWordFortitude spell, ensuring it stays active.
        /// Adds a KeepActiveAuraJob for the Priest335a.VampiricEmbrace spell, ensuring it stays active.
        /// Adds a KeepActiveAuraJob for the Priest335a.VampiricTouch spell, ensuring it stays active on the target.
        /// Adds a KeepActiveAuraJob for the Priest335a.DevouringPlague spell, ensuring it stays active on the target.
        /// Adds a KeepActiveAuraJob for the Priest335a.ShadowWordPain spell, ensuring it stays active on the target.
        /// Adds a KeepActiveAuraJob for the Priest335a.MindBlast spell, ensuring it stays active on the target.
        /// Adds a delegate to the SpellsToKeepActiveOnParty dictionary to keep Priest335a.PowerWordFortitude active on party members.
        /// </summary>
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
        /// Gets or sets the display name for a Priest Shadow.
        /// </summary>
        public override string DisplayName2 => "Priest Shadow";

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// This property is set to false.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this entity is melee.
        /// </summary>
        /// <returns>False, indicating that the entity is not melee.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the character's items.
        /// </summary>
        public override IItemComparator ItemComparator
        {
            get =>
                new SimpleItemComparator((DefaultCharacterManager)Bot.Character, new Dictionary<string, double>()
                {
                    { WowStatType.INTELLECT, 2.5 },
                    { WowStatType.SPELL_POWER, 2.5 },
                    { WowStatType.ARMOR, 2.0 },
                    { WowStatType.MP5, 2.0 },
                    { WowStatType.HASTE, 2.0 },
                });
            set { }
        }

        /// <summary>
        /// Gets the role of the character as a DPS (Damage Per Second) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        ///<summary>
        ///An override of the <see cref="TalentTree"/> property.
        ///It returns a new <see cref="TalentTree"/> object with the following configuration:
        ///- Tree1 has the following talents:
        ///   - Talent at position 2 with values (1, 2, 5)
        ///   - Talent at position 4 with values (1, 4, 3)
        ///   - Talent at position 5 with values (1, 5, 2)
        ///   - Talent at position 7 with values (1, 7, 3)
        ///- Tree2 is empty
        ///- Tree3 has the following talents:
        ///   - Talent at position 1 with values (3, 1, 3)
        ///   - Talent at position 2 with values (3, 2, 2)
        ///   - Talent at position 3 with values (3, 3, 5)
        ///   - Talent at position 5 with values (3, 5, 2)
        ///   - Talent at position 6 with values (3, 6, 3)
        ///   - Talent at position 8 with values (3, 8, 5)
        ///   - Talent at position 9 with values (3, 9, 1)
        ///   - Talent at position 10 with values (3, 10, 2)
        ///   - Talent at position 11 with values (3, 11, 2)
        ///   - Talent at position 12 with values (3, 12, 3)
        ///   - Talent at position 14 with values (3, 14, 1)
        ///   - Talent at position 16 with values (3, 16, 3)
        ///   - Talent at position 17 with values (3, 17, 2)
        ///   - Talent at position 18 with values (3, 18, 3)
        ///   - Talent at position 19 with values (3, 19, 1)
        ///   - Talent at position 20 with values (3, 20, 5)
        ///   - Talent at position 21 with values (3, 21, 2)
        ///   - Talent at position 22 with values (3, 22, 3)
        ///   - Talent at position 24 with values (3, 24, 1)
        ///   - Talent at position 25 with values (3, 25, 3)
        ///   - Talent at position 26 with values (3, 26, 5)
        ///   - Talent at position 27 with values (3, 27, 1)
        ///</summary>
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
        /// Gets or sets a value indicating whether this character should use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this character should use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        /// <returns>The version of the code as a string.</returns>
        public override string Version => "1.2";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind the enemy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character cannot walk behind the enemy; otherwise, <c>false</c>.
        /// </value>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the wow class for the current character as a Priest.
        /// </summary>
        public override WowClass WowClass => WowClass.Priest;

        /// <summary>
        /// Gets or sets the World of Warcraft version, which is WotLK335a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Executes the specified code block, performing a series of actions based on the current state of the bot and the player.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (Bot.Target == null)
            {
                return;
            }

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

            if (Bot.Player.ManaPercentage >= 50
                && TryCastSpell(Racials335a.Berserking, Bot.Wow.TargetGuid))
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

        /// <summary>
        /// Returns the opening spell for the Priest335a class.
        /// If the spell "ShadowWordPain" is available, it will be returned.
        /// Otherwise, the spell "Smite" will be returned.
        /// </summary>
        protected override Spell GetOpeningSpell()
        {
            Spell spell = Bot.Character.SpellBook.GetSpellByName(Priest335a.ShadowWordPain);
            if (spell != null)
            {
                return spell;
            }
            return Bot.Character.SpellBook.GetSpellByName(Priest335a.Smite);
        }
    }
}