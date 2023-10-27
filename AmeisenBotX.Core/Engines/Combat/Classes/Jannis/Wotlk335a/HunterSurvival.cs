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
    /// Constructor for the HunterSurvival class that initializes a new instance with the provided bot.
    /// </summary>
    public class HunterSurvival : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the HunterSurvival class that initializes a new instance with the provided bot.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces bot to initialize with.</param>
        public HunterSurvival(AmeisenBotInterfaces bot) : base(bot)
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
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Hunter335a.BlackArrow, () => TryCastSpell(Hunter335a.BlackArrow, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Hunter335a.WyvernSting, x.Guid, true) }
            };
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Survival Hunter spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Survival Hunter spec.";

        /// <summary>
        /// Gets or sets the display name for Hunter Survival.
        /// </summary>
        public override string DisplayName2 => "Hunter Survival";

        /// <summary>
        /// Gets a value indicating whether this implementation handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> since this implementation does not handle movement.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is a melee character.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items in terms of their intellect, applying a basic intellect comparison logic. The default comparator is initialized with a list containing only the WowArmorType.Shield type.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the role of the character as a damage per second (DPS) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// This code defines an override for the `Talents` property in the base class. The property is of type `TalentTree` and is initialized with a new instance of `TalentTree`. The `TalentTree` instance is populated with data for three different trees: `Tree1`, `Tree2`, and `Tree3`. Each tree is assigned a collection of key-value pairs, where the keys are integers and the values are instances of a class with three integer parameters.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 4, new(2, 4, 3) },
                { 6, new(2, 6, 5) },
                { 7, new(2, 7, 1) },
                { 9, new(2, 9, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 2) },
                { 8, new(3, 8, 5) },
                { 12, new(3, 12, 3) },
                { 13, new(3, 13, 3) },
                { 14, new(3, 14, 3) },
                { 15, new(3, 15, 3) },
                { 17, new(3, 17, 5) },
                { 18, new(3, 18, 2) },
                { 19, new(3, 19, 3) },
                { 20, new(3, 20, 1) },
                { 21, new(3, 21, 3) },
                { 22, new(3, 22, 4) },
                { 23, new(3, 23, 3) },
                { 25, new(3, 25, 1) },
                { 26, new(3, 26, 3) },
                { 27, new(3, 27, 3) },
                { 28, new(3, 28, 1) },
            },
        };

        /// This property indicates that the object should use auto attacks. The value is set to true.
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version number of the object.
        /// </summary>
        public override string Version => "1.0";

        /// This property represents whether or not the character can walk behind an enemy. It is set to false, indicating that the character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass value, which represents the class of a hunter in World of Warcraft.
        /// </summary>
        public override WowClass WowClass => WowClass.Hunter;

        /// <summary>
        /// Gets or sets the version of World of Warcraft to WotLK335a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the private instance of the PetManager class.
        /// </summary>
        private PetManager PetManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the code is ready to disengage.
        /// </summary>
        private bool ReadyToDisengage { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to target slowly when possible.
        /// </summary>
        private bool SlowTargetWhenPossible { get; set; } = false;

        /// This method is responsible for executing the logic of the DPS (Damage Per Second) strategy for the Hunter335a class. It first calls the base.Execute() method to execute the base logic, then it checks if a target is found using the TryFindTarget() method from the TargetProviderDps class. If a target is found, it performs a series of actions based on the distance to the target and the player's health percentage. The logic includes making distance from the target, using defensive spells like Feign Death and Deterrence, and using offensive spells like Raptor Strike, Mongoose Bite, KillShot, KillCommand, RapidFire, MultiShot, ExplosiveShot, AimedShot, and SteadyShot.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (PetManager.Tick()) { return; }

                if (Bot.Target != null)
                {
                    double distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                    // make some distance
                    if ((Bot.Target.Type == WowObjectType.Player && Bot.Wow.TargetGuid != 0 && distanceToTarget < 10.0)
                        || (Bot.Target.Type == WowObjectType.Unit && Bot.Wow.TargetGuid != 0 && distanceToTarget < 3.0))
                    {
                        Bot.Movement.SetMovementAction(MovementAction.Flee, Bot.Target.Position, Bot.Target.Rotation);
                    }

                    if (Bot.Player.HealthPercentage < 15.0
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

                        if (Bot.Player.HealthPercentage < 30.0
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
                            && TryCastSpell(Hunter335a.ConcussiveShot, Bot.Wow.TargetGuid, true))
                        {
                            SlowTargetWhenPossible = false;
                            return;
                        }

                        if (Bot.Target.HealthPercentage < 20.0
                            && TryCastSpell(Hunter335a.KillShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        TryCastSpell(Hunter335a.KillCommand, Bot.Wow.TargetGuid, true);
                        TryCastSpell(Hunter335a.RapidFire, 0);

                        if (Bot.GetNearEnemies<IWowUnit>(Bot.Target.Position, 16.0f).Count() > 2
                            && TryCastSpell(Hunter335a.MultiShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if ((Bot.Objects.All.OfType<IWowUnit>().Where(e => Bot.Target.Position.GetDistance(e.Position) < 16.0).Count() > 2 && TryCastSpell(Hunter335a.MultiShot, Bot.Wow.TargetGuid, true))
                            || TryCastSpell(Hunter335a.ExplosiveShot, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Hunter335a.AimedShot, Bot.Wow.TargetGuid, true)
                            || TryCastSpell(Hunter335a.SteadyShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the code when the entity is out of combat.
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