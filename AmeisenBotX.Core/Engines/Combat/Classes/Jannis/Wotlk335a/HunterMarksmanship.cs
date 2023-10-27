using AmeisenBotX.Core.Engines.Combat.Helpers;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Movement.Enums;
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
    /// Initializes a new instance of the HunterMarksmanship class with the provided bot.
    /// </summary>
    public class HunterMarksmanship : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the HunterMarksmanship class with the provided bot.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance to use for the hunter.</param>
        public HunterMarksmanship(AmeisenBotInterfaces bot) : base(bot)
        {
            PetManager = new PetManager
            (
                Bot,
                TimeSpan.FromSeconds(15),
                () => TryCastSpell(Hunter335a.MendPet, 0, true),
                () => TryCastSpell(Hunter335a.CallPet, 0),
                () => TryCastSpell(Hunter335a.RevivePet, 0)
            );

            MyAuraManager.Jobs.Add(new KeepBestActiveAuraJob(bot.Db, new List<(string, Func<bool>)>()
            {
                (Hunter335a.AspectOfTheViper, () => Bot.Player.ManaPercentage < 25.0 && TryCastSpell(Hunter335a.AspectOfTheViper, 0, true)),
                (Hunter335a.AspectOfTheDragonhawk, () => (!bot.Character.SpellBook.IsSpellKnown(Hunter335a.AspectOfTheViper) || Bot.Player.ManaPercentage > 80.0) && TryCastSpell(Hunter335a.AspectOfTheDragonhawk, 0, true)),
                (Hunter335a.AspectOfTheHawk, () => TryCastSpell(Hunter335a.AspectOfTheHawk, 0, true))
            }));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Hunter335a.HuntersMark, () => TryCastSpell(Hunter335a.HuntersMark, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Hunter335a.SerpentSting, () => TryCastSpell(Hunter335a.SerpentSting, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Hunter335a.SilencingShot, x.Guid, true) }
            };
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Marksmanship Hunter spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Marksmanship Hunter spec.";

        /// <summary>
        /// Gets or sets the display name for the Hunter Marksmanship.
        /// </summary>
        public override string DisplayName2 => "Hunter Marksmanship";

        /// <summary>
        /// Indicates that this method does not handle movement.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is not a melee character.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for comparing items. The default value is a BasicIntellectComparator
        /// with a single WowArmorType.Shield.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the role of the World of Warcraft character as a Dps (Damage per Second).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree for a character.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },
                { 3, new(1, 3, 2) },
            },
            Tree2 = new()
            {
                { 2, new(2, 2, 3) },
                { 3, new(2, 3, 5) },
                { 4, new(2, 4, 3) },
                { 6, new(2, 6, 5) },
                { 7, new(2, 7, 1) },
                { 8, new(2, 8, 3) },
                { 9, new(2, 9, 1) },
                { 11, new(2, 11, 3) },
                { 14, new(2, 14, 1) },
                { 15, new(2, 15, 3) },
                { 16, new(2, 16, 2) },
                { 17, new(2, 17, 3) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 1) },
                { 20, new(2, 20, 3) },
                { 21, new(2, 21, 5) },
                { 23, new(2, 23, 3) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 5) },
                { 27, new(2, 27, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 7, new(3, 7, 2) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        /// <value>The version number.</value>
        public override string Version => "1.0";

        /// This property indicates that the enemy cannot be walked behind.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the <see cref="WowClass"/> property to WowClass.Hunter.
        /// </summary>
        public override WowClass WowClass => WowClass.Hunter;

        /// <summary>
        /// Gets or sets the version of the World of Warcraft as WotLK (Wrath of the Lich King) 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the private PetManager.
        /// </summary>
        private PetManager PetManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is ready to disengage.
        /// </summary>
        private bool ReadyToDisengage { get; set; } = false;

        /// Gets or sets a value indicating whether the target should be slowed down when possible. Default value is false.
        private bool SlowTargetWhenPossible { get; set; } = false;

        /// This method is used to execute the logic for the Hunter335a class. It first calls the base Execute() method. It then checks if the target can be found using the TargetProviderDps and if the PetManager Tick() method returns true. It then retrieves the target unit based on its GUID and calculates the distance to the target. If the target is within a certain distance, various actions are performed such as setting a movement action to flee, using spells like FeignDeath, FrostTrap, Deterrence, RaptorStrike, and MongooseBite. If the target is further away, other actions are performed such as using Disengage, KillShot, KillCommand, RapidFire, MultiShot, ChimeraShot, AimedShot, ArcaneShot, and SteadyShot based on certain conditions.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (PetManager.Tick()) { return; }

                IWowUnit target = (IWowUnit)Bot.Objects.All.FirstOrDefault(e => e != null && e.Guid == Bot.Wow.TargetGuid);

                if (target != null)
                {
                    double distanceToTarget = target.Position.GetDistance(Bot.Player.Position);

                    // make some distance
                    if ((Bot.Target.Type == WowObjectType.Player && Bot.Wow.TargetGuid != 0 && distanceToTarget < 10.0)
                        || (Bot.Target.Type == WowObjectType.Unit && Bot.Wow.TargetGuid != 0 && distanceToTarget < 3.0))
                    {
                        Bot.Movement.SetMovementAction(MovementAction.Flee, Bot.Target.Position, Bot.Target.Rotation);
                    }

                    if (Bot.Player.HealthPercentage < 15
                        && TryCastSpell(Hunter335a.FeignDeath, 0))
                    {
                        return;
                    }

                    if (distanceToTarget < 5.0)
                    {
                        if (ReadyToDisengage
                            && TryCastSpell(Hunter335a.Disengage, 0, true))
                        {
                            ReadyToDisengage = false;
                            return;
                        }

                        if (TryCastSpell(Hunter335a.FrostTrap, 0, true))
                        {
                            ReadyToDisengage = true;
                            SlowTargetWhenPossible = true;
                            return;
                        }

                        if (Bot.Player.HealthPercentage < 30
                            && TryCastSpell(Hunter335a.Deterrence, 0, true))
                        {
                            return;
                        }

                        if (TryCastSpell(Hunter335a.RaptorStrike, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Hunter335a.MongooseBite, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (SlowTargetWhenPossible
                            && TryCastSpell(Hunter335a.Disengage, 0, true))
                        {
                            SlowTargetWhenPossible = false;
                            return;
                        }

                        if (target.HealthPercentage < 20
                            && TryCastSpell(Hunter335a.KillShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        TryCastSpell(Hunter335a.KillCommand, Bot.Wow.TargetGuid, true);
                        TryCastSpell(Hunter335a.RapidFire, Bot.Wow.TargetGuid);

                        if (Bot.GetNearEnemies<IWowUnit>(Bot.Target.Position, 16.0f).Count() > 2
                            && TryCastSpell(Hunter335a.MultiShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if ((Bot.Objects.All.OfType<IWowUnit>().Where(e => target.Position.GetDistance(e.Position) < 16).Count() > 2 && TryCastSpell(Hunter335a.MultiShot, Bot.Wow.TargetGuid, true))
                            || TryCastSpell(Hunter335a.ChimeraShot, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Hunter335a.AimedShot, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Hunter335a.ArcaneShot, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Hunter335a.SteadyShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the code when the character is out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            ReadyToDisengage = false;
            SlowTargetWhenPossible = false;

            base.OutOfCombatExecute();

            if (PetManager.Tick())
            {
                return;
            }
        }
    }
}