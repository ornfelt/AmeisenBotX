using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Initializes a new instance of the MageFire class with the provided AmeisenBotInterfaces object as a parameter. 
    /// It sets up the necessary jobs for the MyAuraManager and TargetAuraManager to keep certain auras active. 
    /// It also initializes the InterruptManager with the InterruptSpells dictionary, mapping the Counterspell spell ID to the TryCastSpell method. 
    /// Additionally, it adds the ArcaneIntellect spell to the SpellsToKeepActiveOnParty list in the GroupAuraManager.
    /// </summary>
    public class MageFire : BasicCombatClass
    {
        /// Initializes a new instance of the MageFire class with the provided AmeisenBotInterfaces object as a parameter. It sets up the necessary jobs for the MyAuraManager and TargetAuraManager to keep certain auras active. It also initializes the InterruptManager with the InterruptSpells dictionary, mapping the Counterspell spell ID to the TryCastSpell method. Additionally, it adds the ArcaneIntellect spell to the SpellsToKeepActiveOnParty list in the GroupAuraManager.
        public MageFire(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ArcaneIntellect, () => TryCastSpell(Mage335a.ArcaneIntellect, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.MoltenArmor, () => TryCastSpell(Mage335a.MoltenArmor, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ManaShield, () => TryCastSpell(Mage335a.ManaShield, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.Scorch, () => TryCastSpell(Mage335a.Scorch, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.LivingBomb, () => TryCastSpell(Mage335a.LivingBomb, Bot.Wow.TargetGuid, true)));

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
        /// Gets the description for the FCFS based CombatClass of the Fire Mage spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Fire Mage spec.";

        /// <summary>
        /// Gets or sets the display name for the Mage Fire.
        /// </summary>
        public override string DisplayName2 => "Mage Fire";

        /// This property indicates that the class does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is melee or not.
        /// </summary>
        /// <returns>False as the character is not melee.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the current object, which compares items based on their intellect value. The default value is a BasicIntellectComparator that compares items with the WowArmorType.Shield and the WowWeaponType.Sword, WowWeaponType.Mace, and WowWeaponType.Axe.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the role of the Wow character as a DPS (Damage Per Second) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// Represents the talent tree for a character. It includes three trees: Tree1, Tree2, and Tree3.
        ///
        /// Tree1 contains the following talents:
        /// - Talent 1: Requires 1 point and has a rank of 1. It has a cost of 2 points.
        /// - Talent 2: Requires 1 point and has a rank of 2. It has a cost of 3 points.
        /// - Talent 6: Requires 1 point and has a rank of 6. It has a cost of 5 points.
        /// - Talent 8: Requires 1 point and has a rank of 8. It has a cost of 3 points.
        /// - Talent 9: Requires 1 point and has a rank of 9. It has a cost of 1 point.
        /// - Talent 10: Requires 1 point and has a rank of 10. It has a cost of 1 point.
        /// - Talent 14: Requires 1 point and has a rank of 14. It has a cost of 3 points.
        ///
        /// Tree2 contains the following talents:
        /// - Talent 3: Requires 2 points and has a rank of 3. It has a cost of 5 points.
        /// - Talent 4: Requires 2 points and has a rank of 4. It has a cost of 5 points.
        /// - Talent 6: Requires 2 points and has a rank of 6. It has a cost of 3 points.
        /// - Talent 7: Requires 2 points and has a rank of 7. It has a cost of 2 points.
        /// - Talent 9: Requires 2 points and has a rank of 9. It has a cost of 1 point.
        /// - Talent 10: Requires 2 points and has a rank of 10. It has a cost of 2 points.
        /// - Talent 11: Requires 2 points and has a rank of 11. It has a cost of 3 points.
        /// - Talent 13: Requires 2 points and has a rank of 13. It has a cost of 3 points.
        /// - Talent 14: Requires 2 points and has a rank of 14. It has a cost of 3 points.
        /// - Talent 15: Requires 2 points and has a rank of 15. It has a cost of 3 points.
        /// - Talent 18: Requires 2 points and has a rank of 18. It has a cost of 5 points.
        /// - Talent 19: Requires 2 points and has a rank of 19. It has a cost of 3 points.
        /// - Talent 20: Requires 2 points and has a rank of 20. It has a cost of 1 point.
        /// - Talent 21: Requires 2 points and has a rank of 21. It has a cost of 2 points.
        /// - Talent 23: Requires 2 points and has a rank of 23. It has a cost of 3 points.
        /// - Talent 27: Requires 2 points and has a rank of 27. It has a cost of 5 points.
        /// - Talent 28: Requires 2 points and has a rank of 28. It has a cost of 1 point.
        ///
        /// Tree3 is initially empty, representing no talents.
        ///
        /// The Talents property is set to a new instance of the TalentTree class, initialized with the specified talents in each tree.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 2) },
                { 2, new(1, 2, 3) },
                { 6, new(1, 6, 5) },
                { 8, new(1, 8, 3) },
                { 9, new(1, 9, 1) },
                { 10, new(1, 10, 1) },
                { 14, new(1, 14, 3) },
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 4, new(2, 4, 5) },
                { 6, new(2, 6, 3) },
                { 7, new(2, 7, 2) },
                { 9, new(2, 9, 1) },
                { 10, new(2, 10, 2) },
                { 11, new(2, 11, 3) },
                { 13, new(2, 13, 3) },
                { 14, new(2, 14, 3) },
                { 15, new(2, 15, 3) },
                { 18, new(2, 18, 5) },
                { 19, new(2, 19, 3) },
                { 20, new(2, 20, 1) },
                { 21, new(2, 21, 2) },
                { 23, new(2, 23, 3) },
                { 27, new(2, 27, 5) },
                { 28, new(2, 28, 1) },
            },
            Tree3 = new(),
        };

        /// This property indicates that the UseAutoAttacks property is set to false, meaning auto attacks will not be used.
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version number as a string.
        /// </summary>
        public override string Version => "1.0";

        /// The WalkBehindEnemy property is overridden and set to false.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the character's class as a Mage.
        /// </summary>
        public override WowClass WowClass => WowClass.Mage;

        /// <summary>
        /// Gets or sets the version of World of Warcraft as Wrath of the Lich King (3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// This method is used to execute a series of actions performed by the bot. It first calls the base Execute method. It then checks if the target can be found using the TargetProviderDps. If a target is found, it further checks if the bot's current target is not null. If these conditions are met, it attempts to cast various spells based on different conditions. If any of these spell casts are successful, the method returns.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target != null)
                {
                    if (TryCastSpell(Mage335a.MirrorImage, Bot.Wow.TargetGuid, true)
                        || (Bot.Player.HealthPercentage < 16 && TryCastSpell(Mage335a.IceBlock, 0, true))
                        || (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).ToLower() == Mage335a.Hotstreak.ToLower()) && TryCastSpell(Mage335a.Pyroblast, Bot.Wow.TargetGuid, true))
                        || (Bot.Player.ManaPercentage < 40 && TryCastSpell(Mage335a.Evocation, Bot.Wow.TargetGuid, true))
                        || TryCastSpell(Mage335a.Fireball, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }
    }
}