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
    /// Constructor for the HunterBeastmastery class.
    /// </summary>
    public class HunterBeastmastery : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the HunterBeastmastery class.
        /// Initializes a new instance of the class and sets the PetManager property.
        /// Also adds jobs to the MyAuraManager and TargetAuraManager properties.
        /// Sets the interrupt spells for the InterruptManager.
        /// Adds configurable values to the Configurables dictionary.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public HunterBeastmastery(AmeisenBotInterfaces bot) : base(bot)
        {
            PetManager = new
            (
                Bot,
                TimeSpan.FromSeconds(5),
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
                { 0, (x) => TryCastSpell(Hunter335a.ScatterShot, x.Guid, true) },
                { 1, (x) => TryCastSpell(Hunter335a.Intimidation, x.Guid, true) }
            };

            Configurables.TryAdd("KitingStartDistanceUnit", 10.0f);
            Configurables.TryAdd("KitingEndDistanceUnit", 12.0f);
            Configurables.TryAdd("SteadyShotMinDistanceUnit", 12.0f);
            Configurables.TryAdd("ChaseDistanceUnit", 20.0f);

            Configurables.TryAdd("KitingStartDistancePlayer", 8.0f);
            Configurables.TryAdd("KitingEndDistancePlayer", 22.0f);
            Configurables.TryAdd("SteadyShotMinDistancePlayer", 22.0f);
            Configurables.TryAdd("ChaseDistancePlayer", 24.0f);

            Configurables.TryAdd("FleeActionCooldown", 400);
        }

        /// <summary>
        /// Returns the description of the FCFS based CombatClass for the Beastmastery Hunter spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Beastmastery Hunter spec.";

        /// <summary>
        /// Gets or sets the display name for the Hunter Beastmastery class.
        /// </summary>
        public override string DisplayName2 => "Hunter Beastmastery";

        /// <summary>
        /// Gets a value indicating whether this code handles movement.
        /// </summary>
        /// <value><c>true</c> if movement is handled; otherwise, <c>false</c>.</value>
        public override bool HandlesMovement => true;

        /// <summary>
        /// Gets a boolean value indicating whether the character is a melee character or not.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets or sets the role of this character as a DPS (Damage per Second) class in the World of Warcraft game.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// Initializes a new instance of the TalentTree class and sets the values for each talent in the TalentTree. The values are set for Tree1, Tree2, and Tree3.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },
                { 2, new(1, 2, 1) },
                { 3, new(1, 3, 2) },
                { 6, new(1, 6, 2) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 5) },
                { 11, new(1, 11, 5) },
                { 12, new(1, 12, 1) },
                { 13, new(1, 13, 1) },
                { 14, new(1, 14, 2) },
                { 15, new(1, 15, 2) },
                { 16, new(1, 16, 5) },
                { 17, new(1, 17, 3) },
                { 18, new(1, 18, 1) },
                { 21, new(1, 21, 5) },
                { 22, new(1, 22, 3) },
                { 23, new(1, 23, 1) },
                { 24, new(1, 24, 3) },
                { 25, new(1, 25, 5) },
                { 26, new(1, 26, 1) },
            },
            Tree2 = new()
            {
                { 2, new(1, 2, 1) },
                { 3, new(1, 3, 5) },
                { 4, new(1, 4, 3) },
                { 6, new(1, 6, 5) },
                { 8, new(1, 8, 3) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether auto attacks are enabled.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// Determines if the character should not walk behind an enemy.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property as a Hunter.
        /// </summary>
        public override WowClass WowClass => WowClass.Hunter;

        /// <summary>
        /// Gets or sets the World of Warcraft version to Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// Gets or sets the date and time of the last action performed.
        private DateTime LastAction { get; set; }

        /// <summary>
        /// Gets or sets the PetManager instance, which manages the pets.
        /// </summary>
        private PetManager PetManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the system is ready to disengage or not.
        /// </summary>
        private bool ReadyToDisengage { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the entity is running away.
        /// </summary>
        private bool RunningAway { get; set; } = false;

        /// This method is responsible for executing the hunter's behavior. It first calls the base Execute method. If it successfully finds a target, it then proceeds to evaluate various conditions and cast appropriate spells or take actions accordingly. If the player's health percentage drops below 15.0%, the method will attempt to cast Feign Death. If the player is within a certain distance of the target, it will check if it is ready to disengage and cast Disengage if possible. It will also attempt to cast Frost Trap and Deterrence under certain conditions. If the player is not within the close range but still within a certain distance of the target, it will perform a series of spell casts including Raptor Strike and Mongoose Bite. If the player is beyond this distance but still within another specified range, it will attempt to cast Concussive Shot and Kill Shot under certain conditions, followed by Kill Command, Beastial Wrath, Rapid Fire, and MultiShot if applicable. If the player is further away from the target, it will continue to cast Steady Shot or move towards the target's position. If none of these conditions are met, the method will check if the player should run away and take appropriate actions. If the player is not prevented from movement, it will attempt to run away if a certain cooldown has passed and if it is within a specific range from the target's position. If the cooldown has not passed, the method will reset the movement.
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (PetManager.Tick()) { return; }

                if (Bot.Target != null)
                {
                    float distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                    if (Bot.Player.HealthPercentage < 15.0
                        && TryCastSpell(Hunter335a.FeignDeath, 0))
                    {
                        return;
                    }

                    if (distanceToTarget < (Bot.Target.IsPlayer() ? Configurables["KitingStartDistancePlayer"] : Configurables["KitingStartDistanceUnit"]))
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
                            return;
                        }

                        if (Bot.Player.HealthPercentage < 30.0
                            && TryCastSpell(Hunter335a.Deterrence, 0, true))
                        {
                            return;
                        }

                        TryCastSpell(Hunter335a.RaptorStrike, Bot.Wow.TargetGuid, true);
                        TryCastSpell(Hunter335a.MongooseBite, Bot.Wow.TargetGuid, true);
                    }
                    else if (distanceToTarget < (Bot.Target.IsPlayer() ? Configurables["KitingEndDistancePlayer"] : Configurables["KitingEndDistanceUnit"]))
                    {
                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Hunter335a.ConcussiveShot)
                            && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frost Trap Aura")
                            && TryCastSpell(Hunter335a.ConcussiveShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage < 20.0
                            && TryCastSpell(Hunter335a.KillShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        TryCastSpell(Hunter335a.KillCommand, Bot.Wow.TargetGuid, true);
                        TryCastSpell(Hunter335a.BeastialWrath, Bot.Wow.TargetGuid, true);
                        TryCastSpell(Hunter335a.RapidFire, 0);

                        if (Bot.GetNearEnemies<IWowUnit>(Bot.Target.Position, 16.0f).Count() > 2
                            && TryCastSpell(Hunter335a.MultiShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        if (TryCastSpell(Hunter335a.ArcaneShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        // only cast when we are far away and disengage is ready
                        if (distanceToTarget > (Bot.Target.IsPlayer() ? Configurables["SteadyShotMinDistancePlayer"] : Configurables["SteadyShotMinDistanceUnit"])
                            && TryCastSpell(Hunter335a.SteadyShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }
                    else if (!Bot.Tactic.PreventMovement && distanceToTarget > (Bot.Target.IsPlayer() ? Configurables["ChaseDistancePlayer"] : Configurables["ChaseDistanceUnit"]))
                    {
                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Hunter335a.ConcussiveShot)
                            && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frost Trap Aura")
                            && TryCastSpell(Hunter335a.ConcussiveShot, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }

                        // move to position
                        Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position, Bot.Target.Rotation);
                        return;
                    }

                    // nothing to do, run away
                    if (!Bot.Tactic.PreventMovement)
                    {
                        if (DateTime.UtcNow - TimeSpan.FromMilliseconds(Configurables["FleeActionCooldown"]) > LastSpellCast)
                        {
                            if (RunningAway)
                            {
                                if (distanceToTarget < (Bot.Target.IsPlayer() ? Configurables["KitingEndDistancePlayer"] : Configurables["KitingEndDistanceUnit"]))
                                {
                                    Bot.Movement.SetMovementAction(MovementAction.Flee, Bot.Target.Position, Bot.Target.Rotation);
                                }
                                else
                                {
                                    RunningAway = false;
                                }
                            }
                            else if (distanceToTarget < (Bot.Target.IsPlayer() ? Configurables["KitingStartDistancePlayer"] : Configurables["KitingStartDistanceUnit"]))
                            {
                                RunningAway = true;
                            }
                        }
                        else
                        {
                            Bot.Movement.Reset();
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

            base.OutOfCombatExecute();

            if (PetManager.Tick())
            {
                return;
            }
        }
    }
}