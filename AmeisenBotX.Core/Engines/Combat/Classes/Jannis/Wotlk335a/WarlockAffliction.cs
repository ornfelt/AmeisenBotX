using AmeisenBotX.Core.Engines.Combat.Helpers;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a class for a Warlock specializing in Affliction spells.
    /// </summary>
    public class WarlockAffliction : BasicCombatClass
    {
        ///<summary>
        /// Initializes a new instance of the WarlockAffliction class.
        ///</summary>
        ///<param name="bot">The AmeisenBotInterfaces to use for the WarlockAffliction class.</param>
        public WarlockAffliction(AmeisenBotInterfaces bot) : base(bot)
        {
            PetManager = new PetManager
            (
                Bot,
                TimeSpan.FromSeconds(1),
                null,
                () => (Bot.Character.SpellBook.IsSpellKnown(Warlock335a.SummonFelhunter)
                       && Bot.Character.Inventory.HasItemByName("Soul Shard")
                       && TryCastSpell(Warlock335a.SummonFelhunter, 0))
                   || (Bot.Character.SpellBook.IsSpellKnown(Warlock335a.SummonImp)
                       && TryCastSpell(Warlock335a.SummonImp, 0)),
                () => (Bot.Character.SpellBook.IsSpellKnown(Warlock335a.SummonFelhunter)
                       && Bot.Character.Inventory.HasItemByName("Soul Shard")
                       && TryCastSpell(Warlock335a.SummonFelhunter, 0))
                   || (Bot.Character.SpellBook.IsSpellKnown(Warlock335a.SummonImp)
                       && TryCastSpell(Warlock335a.SummonImp, 0))
            );

            MyAuraManager.Jobs.Add(new KeepBestActiveAuraJob(bot.Db, new List<(string, Func<bool>)>()
            {
                (Warlock335a.FelArmor, () => TryCastSpell(Warlock335a.FelArmor, 0, true)),
                (Warlock335a.DemonArmor, () => TryCastSpell(Warlock335a.DemonArmor, 0, true)),
                (Warlock335a.DemonSkin, () => TryCastSpell(Warlock335a.DemonSkin, 0, true)),
            }));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.Corruption, () => Bot.Target != null && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warlock335a.SeedOfCorruption) && TryCastSpell(Warlock335a.Corruption, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.CurseOfAgony, () => TryCastSpell(Warlock335a.CurseOfAgony, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.UnstableAffliction, () => TryCastSpell(Warlock335a.UnstableAffliction, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.Haunt, () => TryCastSpell(Warlock335a.Haunt, Bot.Wow.TargetGuid, true)));
        }

        /// <summary>
        /// Represents a First-Come-First-Serve (FCFS) based CombatClass for the Affliction Warlock spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Affliction Warlock spec.";

        /// <summary>
        /// Gets or sets the display name for a Warlock Affliction.
        /// </summary>
        public override string DisplayName2 => "Warlock Affliction";

        /// <summary>
        /// Indicates that this method does not handle movement.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this instance is a melee weapon.
        /// </summary>
        /// <returns><c>false</c> since this instance is not a melee weapon.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for this class.
        /// The comparator is used to compare items based on their properties.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the instance of the PetManager class.
        /// </summary>
        public PetManager PetManager { get; private set; }

        /// <summary>
        /// Gets the role of the character as a DPS (Damage Per Second) role in the World of Warcraft game.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// The talent tree for the character.
        /// </summary>
        /// <returns>
        /// The character's talent tree.
        /// </returns>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 2) },
                { 2, new(1, 2, 3) },
                { 3, new(1, 3, 5) },
                { 7, new(1, 7, 2) },
                { 9, new(1, 9, 3) },
                { 12, new(1, 12, 2) },
                { 13, new(1, 13, 3) },
                { 14, new(1, 14, 5) },
                { 15, new(1, 15, 1) },
                { 17, new(1, 17, 2) },
                { 18, new(1, 18, 5) },
                { 19, new(1, 19, 3) },
                { 20, new(1, 20, 5) },
                { 23, new(1, 23, 3) },
                { 24, new(1, 24, 3) },
                { 25, new(1, 25, 1) },
                { 26, new(1, 26, 1) },
                { 27, new(1, 27, 5) },
                { 28, new(1, 28, 1) },
            },
            Tree2 = new(),
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 2, new(3, 2, 5) },
                { 8, new(3, 8, 5) },
                { 9, new(3, 9, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character should use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Returns false indicating that the player cannot walk behind the enemy.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass for a specific instance of the Warlock class.
        /// </summary>
        public override WowClass WowClass => WowClass.Warlock;

        /// <summary>
        /// Gets or sets the World of Warcraft version to Wrath of the Lich King, patch 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the last date and time of a fear attempt.
        /// </summary>
        private DateTime LastFearAttempt { get; set; }

        /// <summary>
        /// Executes the specified logic for the Warlock character.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (PetManager.Tick()) { return; }

                if ((Bot.Player.ManaPercentage < 75.0 && Bot.Player.HealthPercentage > 60.0 && TryCastSpell(Warlock335a.LifeTap, 0))
                    || (Bot.Player.HealthPercentage < 80.0 && TryCastSpell(Warlock335a.DeathCoil, Bot.Wow.TargetGuid, true)))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if (Bot.Target.GetType() == typeof(IWowPlayer))
                    {
                        if (DateTime.UtcNow - LastFearAttempt > TimeSpan.FromSeconds(5)
                            && ((Bot.Player.Position.GetDistance(Bot.Target.Position) < 6.0f && TryCastSpell(Warlock335a.HowlOfTerror, 0, true))
                            || (Bot.Player.Position.GetDistance(Bot.Target.Position) < 12.0f && TryCastSpell(Warlock335a.Fear, Bot.Wow.TargetGuid, true))))
                        {
                            LastFearAttempt = DateTime.UtcNow;
                            return;
                        }
                    }

                    if (Bot.Character.Inventory.Items.Count(e => e.Name.Equals("Soul Shard", StringComparison.OrdinalIgnoreCase)) < 5
                        && Bot.Target.HealthPercentage < 25.0
                        && TryCastSpell(Warlock335a.DrainSoul, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }

                if (Bot.GetNearEnemies<IWowUnit>(Bot.Target.Position, 16.0f).Count() > 2
                    && ((!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warlock335a.SeedOfCorruption)
                        && TryCastSpell(Warlock335a.SeedOfCorruption, Bot.Wow.TargetGuid, true))
                    || TryCastAoeSpell(Warlock335a.RainOfFire, Bot.Wow.TargetGuid, true)))
                {
                    return;
                }

                if (TryCastSpell(Warlock335a.ShadowBolt, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Overrides the OutOfCombatExecute method and performs additional logic.
        /// Checks if the PetManager Tick method returns true and returns if it does.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (PetManager.Tick())
            {
                return;
            }
        }
    }
}