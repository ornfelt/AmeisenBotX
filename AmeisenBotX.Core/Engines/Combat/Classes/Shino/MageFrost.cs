using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Logic.CombatClasses.Shino;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Shino
{
    /// <summary>
    /// Initializes a new instance of the MageFrost class.
    /// Adds jobs to the MyAuraManager to keep active auras, including Arcane Intellect, Frost Armor, Ice Armor, Mana Shield, and Ice Barrier.
    /// Sets the InterruptSpells property of the InterruptManager to include Counterspell.
    /// </summary>
    public class MageFrost : TemplateCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the MageFrost class.
        /// Adds jobs to the MyAuraManager to keep active auras, including Arcane Intellect, Frost Armor, Ice Armor, Mana Shield, and Ice Barrier.
        /// Sets the InterruptSpells property of the InterruptManager to include Counterspell.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for initialization.</param>
        public MageFrost(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ArcaneIntellect, () => TryCastSpell(Mage335a.ArcaneIntellect, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.FrostArmor, () => TryCastSpell(Mage335a.FrostArmor, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.IceArmor, () => TryCastSpell(Mage335a.IceArmor, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.ManaShield, () => TryCastSpell(Mage335a.ManaShield, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Mage335a.IceBarrier, () => TryCastSpell(Mage335a.IceBarrier, 0, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Mage335a.Counterspell, x.Guid, true) }
            };
        }

        /// <summary>
        /// Gets the description of the grinding and leveling combat class.
        /// </summary>
        public override string Description => "Grinding and Leveling CombatClass.";

        /// <summary>
        /// Gets or sets the display name of the Frostmage.
        /// </summary>
        public override string DisplayName2 => "Frostmage";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this object handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this character is not a melee character.
        /// </summary>
        public override bool IsMelee => false;

        ///<summary>
        /// Gets or sets the item comparator used for comparing items.
        ///</summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the role of the character in the World of Warcraft game.
        /// The role is set to Damage Per Second (DPS).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree for the object.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 3) },
            },
            Tree2 = new(),
            Tree3 = new()
            {
                { 1, new(3, 1, 3) },
                { 2, new(3, 2, 5) },
                { 3, new(3, 3, 3) },
                { 4, new(3, 4, 3) },
                { 5, new(3, 5, 2) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 3) },
                { 8, new(3, 8, 3) },
                { 9, new(3, 9, 1) },
                { 11, new(3, 11, 2) },
                { 12, new(3, 12, 3) },
                { 13, new(3, 13, 3) },
                { 14, new(3, 14, 1) },
                { 15, new(3, 15, 3) },
                { 17, new(3, 17, 2) },
                { 18, new(3, 18, 3) },
                { 19, new(3, 19, 2) },
                { 20, new(3, 20, 1) },
                { 21, new(3, 21, 5) },
                { 22, new(3, 22, 2) },
                { 23, new(3, 23, 2) },
                { 24, new(3, 24, 3) },
                { 25, new(3, 25, 1) },
                { 26, new(3, 26, 3) },
                { 27, new(3, 27, 5) },
                { 28, new(3, 28, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether this character should use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind an enemy.
        /// </summary>
        /// <returns>A boolean value indicating whether the player can walk behind an enemy. Always returns false.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass of the character,
        /// which is set to Mage in this case.
        /// </summary>
        public override WowClass WowClass => WowClass.Mage;

        /// <summary>
        /// Gets or sets the WowVersion property to the value of WowVersion.WotLK335a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the date and time of the last sheep.
        /// </summary>
        private DateTime LastSheep { get; set; } = DateTime.Now;

        /// <summary>
        /// Executes the mage's rotation logic to attack the target. 
        /// This method overrides the base Execute method.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (Bot.Player.IsCasting)
            {
                return;
            }

            if (SelectTarget(out IWowUnit target))
            {
                if (Bot.Player.ManaPercentage <= 25.0 && TryCastSpell(Mage335a.Evocation, 0, true))
                {
                    return;
                }

                if (CooldownManager.IsSpellOnCooldown(Mage335a.SummonWaterElemental) &&
                    CooldownManager.IsSpellOnCooldown(Mage335a.IcyVeins))
                {
                    TryCastSpell(Mage335a.ColdSnap, 0);
                }

                if (Bot.Character.SpellBook.IsSpellKnown(Mage335a.Freeze))
                {
                    TryCastAoeSpell(Mage335a.Freeze, target.Guid);
                }

                System.Collections.Generic.IEnumerable<IWowUnit> nearbyTargets = Bot.GetEnemiesInCombatWithParty<IWowUnit>(Bot.Player.Position, 64.0f);
                if (nearbyTargets.Count(e => e.Position.GetDistance(Bot.Player.Position) <= 9.0) == 1
                    && TryCastSpell(Mage335a.FrostNova, 0, true))
                {
                    return;
                }

                if (DateTime.Now.Subtract(LastSheep).TotalMilliseconds >= 3000.0)
                {
                    if (nearbyTargets.Count() > 1 && !nearbyTargets.Any(e => e.Auras.Any(aura => Bot.Db.GetSpellName(aura.SpellId) == Mage335a.Polymorph)))
                    {
                        IWowUnit targetInDistance = nearbyTargets
                            .Where(e => e.Guid != Bot.Wow.TargetGuid)
                            .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                            .FirstOrDefault();
                        Bot.Wow.ChangeTarget(targetInDistance.Guid);
                        if (TryCastSpell(Mage335a.Polymorph, targetInDistance.Guid, true))
                        {
                            Bot.Wow.ChangeTarget(target.Guid);
                            LastSheep = DateTime.Now;
                            return;
                        }
                    }
                }

                if (Bot.Target.Position.GetDistance(Bot.Player.Position) <= 4.0)
                {
                    // TODO: Logic to check if the target blink location is dangerous
                    if (!TryCastSpell(Mage335a.Blink, 0, true))
                    {
                    }
                    else
                    {
                        // TODO: Go away somehow if the enemy is freezed?
                        return;
                    }
                }

                if (Bot.Target.Position.GetDistance(Bot.Player.Position) <= 4.0
                    && Bot.Player.HealthPercentage <= 50.0
                    && CooldownManager.IsSpellOnCooldown(Mage335a.Blink) && TryCastSpell(Mage335a.IceBlock, 0, true))
                {
                    return;
                }

                if (TryCastSpell(Mage335a.SummonWaterElemental, target.Guid, true))
                {
                    return;
                }

                if (TryCastSpell(Mage335a.DeepFreeze, target.Guid, true))
                {
                    return;
                }

                TryCastSpell(Mage335a.IcyVeins, 0, true);
                TryCastSpell(Racials335a.Berserking, 0, true);

                if (TryCastSpell(Mage335a.FrostBolt, target.Guid, true))
                {
                    return;
                }

                TryCastSpell(Mage335a.Fireball, target.Guid, true);
            }
        }

        /// <summary>
        /// This method is used to destroy conjured items from the character's inventory based on the spell rank. 
        /// It first checks if the character knows the spell ConjureWater. If true, it goes through a series of if statements 
        /// to destroy conjured water items in the inventory based on the spell rank. It then checks if the character knows 
        /// the spell ConjureRefreshment and destroys Conjured Glacier Water if true. 
        /// If the character knows the spell ConjureFood, it goes through a series of if statements to destroy conjured 
        /// food items in the inventory based on the spell rank. It also checks if the character knows the spell ConjureRefreshment 
        /// and destroys Conjured Croissant if true. If the character knows the spell ConjureRefreshment, it destroys 
        /// Conjured Mana Pie if the character doesn't have it. It then calls the base OutOfCombatExecute method.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            if (Bot.Character.SpellBook.IsSpellKnown(Mage335a.ConjureWater))
            {
                Spell spell = Bot.Character.SpellBook.GetSpellByName(Mage335a.ConjureWater);
                spell.TryGetRank(out int spellRank);
                if (spellRank >= 2)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Water");
                }

                if (spellRank >= 3)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Fresh Water");
                }

                if (spellRank >= 4)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Spring Water");
                }

                if (spellRank >= 5)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Mineral Water");
                }

                if (spellRank >= 6)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Sparkling Water");
                }

                if (spellRank >= 7)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Crystal Water");
                }

                if (spellRank >= 8)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Mountain Spring Water");
                }

                if (Bot.Character.SpellBook.IsSpellKnown(Mage335a.ConjureRefreshment))
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Glacier Water");
                }

                if (
                    (spellRank == 1 && !Bot.Character.Inventory.HasItemByName("Conjured Water"))
                    || (spellRank == 2 && !Bot.Character.Inventory.HasItemByName("Conjured Fresh Water"))
                    || (spellRank == 3 && !Bot.Character.Inventory.HasItemByName("Conjured Spring Water"))
                    || (spellRank == 4 && !Bot.Character.Inventory.HasItemByName("Conjured Mineral Water"))
                    || (spellRank == 5 && !Bot.Character.Inventory.HasItemByName("Conjured Sparkling Water"))
                    || (spellRank == 6 && !Bot.Character.Inventory.HasItemByName("Conjured Crystal Water"))
                    || (spellRank == 7 && !Bot.Character.Inventory.HasItemByName("Conjured Mountain Spring Water"))
                    || (spellRank == 8 && !Bot.Character.Inventory.HasItemByName("Conjured Glacier Water"))
                    )
                {
                    TryCastSpell(Mage335a.ConjureWater, 0, true);
                    return;
                }
            }

            if (Bot.Character.SpellBook.IsSpellKnown(Mage335a.ConjureFood))
            {
                Spell spell = Bot.Character.SpellBook.GetSpellByName(Mage335a.ConjureFood);
                spell.TryGetRank(out int spellRank);
                if (spellRank >= 2)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Muffin");
                }

                if (spellRank >= 3)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Bread");
                }

                if (spellRank >= 4)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Rye");
                }

                if (spellRank >= 5)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Pumpernickel");
                }

                if (spellRank >= 6)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Sourdough");
                }

                if (spellRank >= 7)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Sweet Roll");
                }

                if (spellRank >= 8)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Cinnamon Roll");
                }

                if (Bot.Character.SpellBook.IsSpellKnown(Mage335a.ConjureRefreshment))
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Croissant");
                }

                if (
                    (spellRank == 1 && !Bot.Character.Inventory.HasItemByName("Conjured Muffin"))
                    || (spellRank == 2 && !Bot.Character.Inventory.HasItemByName("Conjured Bread"))
                    || (spellRank == 3 && !Bot.Character.Inventory.HasItemByName("Conjured Rye"))
                    || (spellRank == 4 && !Bot.Character.Inventory.HasItemByName("Conjured Pumpernickel"))
                    || (spellRank == 5 && !Bot.Character.Inventory.HasItemByName("Conjured Sourdough"))
                    || (spellRank == 6 && !Bot.Character.Inventory.HasItemByName("Conjured Sweet Roll"))
                    || (spellRank == 7 && !Bot.Character.Inventory.HasItemByName("Conjured Cinnamon Roll"))
                    || (spellRank == 8 && !Bot.Character.Inventory.HasItemByName("Conjured Croissant"))
                    )
                {
                    TryCastSpell(Mage335a.ConjureFood, 0, true);
                    return;
                }
            }

            if (Bot.Character.SpellBook.IsSpellKnown(Mage335a.ConjureRefreshment))
            {
                Spell spell = Bot.Character.SpellBook.GetSpellByName(Mage335a.ConjureRefreshment);
                spell.TryGetRank(out int spellRank);

                if (spellRank >= 2)
                {
                    Bot.Character.Inventory.DestroyItemByName("Conjured Mana Pie");
                }

                if (
                    (spellRank == 1 && !Bot.Character.Inventory.HasItemByName("Conjured Mana Pie"))
                    || (spellRank == 2 && !Bot.Character.Inventory.HasItemByName("Conjured Mana Strudel"))
                )
                {
                    TryCastSpell(Mage335a.ConjureRefreshment, 0, true);
                    return;
                }
            }

            base.OutOfCombatExecute();
        }

        /// <summary>
        /// Retrieves the opening spell for the bot's character.
        /// </summary>
        /// <returns>
        /// The opening spell. If the character has a Frostbolt spell, it will be returned; otherwise, the Fireball spell will be returned.
        /// </returns>
        protected override Spell GetOpeningSpell()
        {
            Spell spell = Bot.Character.SpellBook.GetSpellByName(Mage335a.FrostBolt);
            if (spell != null)
            {
                return spell;
            }
            return Bot.Character.SpellBook.GetSpellByName(Mage335a.Fireball);
        }
    }
}