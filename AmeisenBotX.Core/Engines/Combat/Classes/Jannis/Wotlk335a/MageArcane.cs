using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Linq;

/// <summary>
/// Contains classes related to the combat engine for the World of Warcraft character 'Jannis' in the Wrath of the Lich King 3.3.5a version, developed by AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// The MageArcane class extends the BasicCombatClass, providing functionality for managing auras, dispelling buffs, and interrupting spells.
    /// </summary>
    public class MageArcane : BasicCombatClass
    {
        /// Initializes a new instance of the MageArcane class with the provided bot parameter. It adds jobs to the MyAuraManager.Jobs list to keep certain auras active. It also sets the TargetAuraManager.DispellBuffs delegate to check if there are unit stealable buffs on the target and attempt to cast spellSteal if true. The InterruptManager.InterruptSpells dictionary is initialized with Counterspell as the key and a lambda expression as the value, which calls the TryCastSpell method with the Counterspell spell and the target's GUID. Finally, it adds a spell to the GroupAuraManager.SpellsToKeepActiveOnParty list, using ArcaneIntellect as the spell and a lambda expression that calls TryCastSpell with the spellName and guid parameters, and setting the last argument to true.
        public MageArcane(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ArcaneIntellect, () => TryCastSpell(Mage335a.ArcaneIntellect, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.MageArmor, () => TryCastSpell(Mage335a.MageArmor, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ManaShield, () => TryCastSpell(Mage335a.ManaShield, 0, true)));

            // TargetAuraManager.DispellBuffs = () =>
            // Bot.NewBot.LuaHasUnitStealableBuffs(WowLuaUnit.Target) && TryCastSpell(spellSteal,
            // Bot.NewBot.TargetGuid, true);

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Mage335a.Counterspell, x.Guid, true) }
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Mage335a.ArcaneIntellect, (spellName, guid) => TryCastSpell(spellName, guid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Arcane Mage spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Arcane Mage spec.";

        /// <summary>
        /// Gets or sets the display name for the Mage Arcane.
        /// </summary>
        public override string DisplayName2 => "Mage Arcane";

        /// <summary>
        /// Gets or sets a value indicating whether this class handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this class handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is not a melee entity.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for comparing items in the inventory.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// Gets or sets the last time the spellsteal check was performed.
        public DateTime LastSpellstealCheck { get; private set; }

        /// <summary>
        /// Gets or sets the role of the character as a Damage Per Second (DPS) role in World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// Initializes a new instance of the TalentTree class with predefined talent values for each tree.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 2) },
                { 2, new(1, 2, 3) },
                { 3, new(1, 3, 5) },
                { 6, new(1, 6, 5) },
                { 8, new(1, 8, 2) },
                { 9, new(1, 9, 1) },
                { 10, new(1, 10, 1) },
                { 13, new(1, 13, 2) },
                { 14, new(1, 14, 3) },
                { 16, new(1, 16, 1) },
                { 17, new(1, 17, 5) },
                { 19, new(1, 19, 3) },
                { 20, new(1, 20, 2) },
                { 23, new(1, 23, 3) },
                { 24, new(1, 24, 1) },
                { 25, new(1, 25, 5) },
                { 27, new(1, 27, 5) },
                { 28, new(1, 28, 3) },
                { 29, new(1, 29, 2) },
                { 30, new(1, 30, 1) },
            },
            Tree2 = new()
            {
                { 2, new(2, 2, 3) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 3, new(3, 3, 3) },
                { 5, new(3, 5, 2) },
                { 6, new(3, 6, 3) },
                { 9, new(3, 9, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character can use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public override string Version => "1.0";

        /// Overrides the WalkBehindEnemy property to always return false.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the class of the character in the World of Warcraft game.
        /// </summary>
        /// <value>The class of the character is set to Mage.</value>
        public override WowClass WowClass => WowClass.Mage;

        /// <summary>
        /// Gets the World of Warcraft version as Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// This method executes specific actions based on certain conditions. It first calls the base Execute method. If it is able to find a target, it checks for various conditions and tries to cast different spells accordingly. If any of the conditions are met and a spell is successfully cast, the method returns.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target != null)
                {
                    if ((Bot.Player.HealthPercentage < 16.0 && TryCastSpell(Mage335a.IceBlock, 0))
                        || (Bot.Player.ManaPercentage < 40.0 && TryCastSpell(Mage335a.Evocation, 0, true))
                        || TryCastSpell(Mage335a.MirrorImage, Bot.Wow.TargetGuid, true)
                        || (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Mage335a.MissileBarrage) && TryCastSpell(Mage335a.ArcaneMissiles, Bot.Wow.TargetGuid, true))
                        || TryCastSpell(Mage335a.ArcaneBarrage, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Mage335a.ArcaneBlast, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Mage335a.Fireball, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }
    }
}