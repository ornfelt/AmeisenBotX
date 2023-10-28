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

/// <summary>
/// Namespace for combat classes specific to a Warlock specializing in Destruction abilities.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a combat class for a Warlock specializing in Destruction abilities.
    /// </summary>
    public class WarlockDestruction : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the WarlockDestruction class.
        /// </summary>
        /// <param name="bot">The Bot instance to use.</param>
        public WarlockDestruction(AmeisenBotInterfaces bot) : base(bot)
        {
            PetManager = new PetManager
            (
                Bot,
                TimeSpan.FromSeconds(1),
                null,
                () => Bot.Character.SpellBook.IsSpellKnown(Warlock335a.SummonImp) && TryCastSpell(Warlock335a.SummonImp, 0, true),
                () => Bot.Character.SpellBook.IsSpellKnown(Warlock335a.SummonImp) && TryCastSpell(Warlock335a.SummonImp, 0, true)
            );

            MyAuraManager.Jobs.Add(new KeepBestActiveAuraJob(bot.Db, new List<(string, Func<bool>)>()
            {
                (Warlock335a.FelArmor, () => TryCastSpell(Warlock335a.FelArmor, 0, true)),
                (Warlock335a.DemonArmor, () => TryCastSpell(Warlock335a.DemonArmor, 0, true)),
                (Warlock335a.DemonSkin, () => TryCastSpell(Warlock335a.DemonSkin, 0, true)),
            }));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.Corruption, () => Bot.Target != null && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warlock335a.SeedOfCorruption) && TryCastSpell(Warlock335a.Corruption, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.CurseOfTheElements, () => TryCastSpell(Warlock335a.CurseOfTheElements, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warlock335a.Immolate, () => TryCastSpell(Warlock335a.Immolate, Bot.Wow.TargetGuid, true)));
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Destruction Warlock spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Destruction Warlock spec.";

        /// <summary>
        /// Gets the display name for a Warlock Destruction.
        /// </summary>
        public override string DisplayName2 => "Warlock Destruction";

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> because this object does not handle movement.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is a melee entity.
        /// In this case, the value is always false.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the specific implementation.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the PetManager instance.
        /// </summary>
        public PetManager PetManager { get; private set; }

        /// <summary>
        /// Gets or sets the role of the Wow character as a DPS (Damage Per Second) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree for the override.
        /// </summary>
        /// <value>
        /// The talent tree with the specified talents.
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 2, new(2, 2, 3) },
                { 3, new(2, 3, 3) },
                { 4, new(2, 4, 1) },
                { 7, new(2, 7, 3) },
                { 9, new(2, 9, 1) },
                { 10, new(2, 10, 1) },
                { 11, new(2, 11, 3) },
                { 12, new(2, 12, 3) },
            },
            Tree3 = new()
            {
                { 2, new(3, 2, 5) },
                { 3, new(3, 3, 2) },
                { 5, new(3, 5, 3) },
                { 6, new(3, 6, 2) },
                { 8, new(3, 8, 5) },
                { 9, new(3, 9, 2) },
                { 10, new(3, 10, 1) },
                { 12, new(3, 12, 3) },
                { 13, new(3, 13, 3) },
                { 14, new(3, 14, 1) },
                { 16, new(3, 16, 5) },
                { 17, new(3, 17, 1) },
                { 19, new(3, 19, 3) },
                { 20, new(3, 20, 5) },
                { 22, new(3, 22, 3) },
                { 24, new(3, 24, 3) },
                { 25, new(3, 25, 5) },
                { 26, new(3, 26, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the entity can use auto-attacks.
        /// </summary>
        /// <value>
        /// <c>false</c> if the entity cannot use auto-attacks; otherwise, <c>true</c>.
        /// </value>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets or sets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind an enemy.
        /// </summary>
        /// <value>
        ///   <c>false</c> indicating that the player cannot walk behind an enemy; otherwise, <c>true</c>.
        /// </value>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the wow class for this instance, which is a Warlock.
        /// </summary>
        public override WowClass WowClass => WowClass.Warlock;

        /// <summary>
        /// Gets or sets the version of World of Warcraft.
        /// </summary>
        /// <value>
        /// The version of World of Warcraft: Wrath of the Lich King (3.3.5a).
        /// </value>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the last datetime of the fear attempt.
        /// </summary>
        private DateTime LastFearAttempt { get; set; }

        /// <summary>
        /// Executes the specified code logic.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (PetManager.Tick()) { return; }

                if ((Bot.Player.ManaPercentage < 75.0 && Bot.Player.HealthPercentage > 60.0 && TryCastSpell(Warlock335a.LifeTap, 0))
                    || (Bot.Player.HealthPercentage < 80.0 && TryCastSpell(Warlock335a.DeathCoil, Bot.Wow.TargetGuid, true))
                    || (Bot.Player.HealthPercentage < 50.0 && TryCastSpell(Warlock335a.DrainLife, Bot.Wow.TargetGuid, true)))
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

                if (TryCastSpell(Warlock335a.ChaosBolt, Bot.Wow.TargetGuid, true)
                    || TryCastSpell(Warlock335a.Incinerate, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the OutOfCombatExecute method. It first calls the base implementation of the method. 
        /// Then it checks if the Tick method of the PetManager class returns true and returns if it does. 
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