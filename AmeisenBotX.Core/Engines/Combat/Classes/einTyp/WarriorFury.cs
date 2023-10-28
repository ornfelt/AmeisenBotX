using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

/// <summary>
/// Provides classes related to different types of combat engines in the AmeisenBotX Core.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.einTyp
{
    /// <summary>
    /// Represents a class that implements the ICombatClass interface for the Warrior Fury specialization.
    /// </summary>
    public class WarriorFury : ICombatClass
    {
        /// <summary>
        /// Represents a reference to the <see cref="AmeisenBotInterfaces"/> interface object.
        /// </summary>
        private readonly AmeisenBotInterfaces Bot;
        /// <summary>
        /// This field represents an array of running emotes.
        /// </summary>
        private readonly string[] runningEmotes = { "/train", "/cackle", "/silly" };
        /// <summary>
        /// The readonly field that holds the Warrior's Fury spells.
        /// </summary>
        private readonly WarriorFurySpells spells;
        /// <summary>
        /// Array of standing emotes.
        /// </summary>
        private readonly string[] standingEmotes = { "/shimmy", "/dance", "/twiddle", "/highfive" };
        /// <summary>
        /// Gets or sets a value indicating whether a new route will be computed.
        /// </summary>
        private bool computeNewRoute = false;
        /// <summary>
        /// Represents the distance from the current object to the target.
        /// </summary>
        private double distanceToTarget = 0;

        /// <summary>
        /// The distance traveled.
        /// </summary>
        private double distanceTraveled = 0;

        /// <summary>
        /// Indicates whether there are multiple targets.
        /// </summary>
        private bool multipleTargets = false;
        /// <summary>
        /// Represents the current standing status.
        /// </summary>
        private bool standing = false;

        /// <summary>
        /// Constructor for the WarriorFury class.
        /// Initializes a new instance of the class and sets the bot and spells.
        /// </summary>
        /// <param name="bot">The bot object implementing the AmeisenBotInterfaces.</param>
        public WarriorFury(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            spells = new WarriorFurySpells(bot);
        }

        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        /// <returns>The name of the author, which is "einTyp".</returns>
        public string Author => "einTyp";

        /// <summary>
        /// Gets or sets the collection of blacklisted target display IDs.
        /// </summary>
        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of configureables.
        /// </summary>
        /// <value>
        /// The dictionary of configureables.
        /// </value>
        public Dictionary<string, dynamic> Configureables { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets or sets the description of the object.
        /// </summary>
        public string Description => "...";

        /// <summary>
        /// Gets the display name for the Fury Warrior class.
        /// </summary>
        public string DisplayName => "Fury Warrior";

        /// <summary>
        /// Gets a value indicating whether this code handles facing.
        /// </summary>
        public bool HandlesFacing => false;

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this object handles movement; otherwise, <c>false</c>.
        /// </value>
        public bool HandlesMovement => true;

        /// <summary>
        /// Gets or sets a value indicating whether the character is a melee fighter.
        /// </summary>
        public bool IsMelee => true;

        /// <summary>
        /// Gets the item comparator for the bot.
        /// </summary>
        public IItemComparator ItemComparator => new FuryItemComparator(Bot);

        /// <summary>
        /// Gets or sets the collection of priority target display IDs.
        /// </summary>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets the Role property which represents the World of Warcraft role assigned to a character,
        /// and returns the value "Dps" which stands for Damage per Second.
        /// </summary>
        public WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets a collection of talent trees.
        /// </summary>
        /// <value>
        /// The talent trees available.
        /// </value>
        public TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 2, new(1, 2, 2) },
                { 4, new(1, 4, 2) },
                { 6, new(1, 6, 3) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) }
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 9, new(2, 9, 5) },
                { 10, new(2, 10, 5) },
                { 11, new(2, 11, 2) },
                { 12, new(2, 12, 5) },
                { 13, new(2, 13, 3) },
                { 14, new(2, 14, 1) },
                { 17, new(2, 17, 5) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 1) },
                { 22, new(2, 22, 5) },
                { 24, new(2, 24, 1) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 5) },
                { 27, new(2, 27, 1) }
            },
            Tree3 = new()
        };

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind an enemy.
        /// </summary>
        /// <value>
        ///   <c>false</c> indicating that the player cannot walk behind an enemy; otherwise, <c>true</c> if the player can walk behind an enemy.
        /// </value>
        public bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the selected WoW class, which is currently set to Warrior.
        /// </summary>
        public WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Gets or sets a value indicating whether the person is currently dancing.
        /// </summary>
        private bool Dancing { get; set; }

        /// <summary>
        /// Gets or sets the last known position of the player in a three-dimensional space.
        /// </summary>
        private Vector3 LastPlayerPosition { get; set; }

        /// <summary>
        /// Gets or sets the last target position in 3D space.
        /// </summary>
        private Vector3 LastTargetPosition { get; set; }

        /// <summary>
        /// Attacks the target unit. If there is no target, the method returns. 
        /// If the target is within a distance of 3.0, the bot stops click-to-move, resets movement,
        /// and interacts with the target unit. Otherwise, the bot sets a movement action to move towards the target.
        /// </summary>
        public void AttackTarget()
        {
            IWowUnit target = Bot.Target;
            if (target == null)
            {
                return;
            }

            if (Bot.Player.Position.GetDistance(target.Position) <= 3.0)
            {
                Bot.Wow.StopClickToMove();
                Bot.Movement.Reset();
                Bot.Wow.InteractWithUnit(target);
            }
            else
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, target.Position);
            }
        }

        /// <summary>
        /// Executes the action specified in this method.
        /// </summary>
        public void Execute()
        {
            computeNewRoute = false;
            IWowUnit target = Bot.Target;
            if ((Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1)) || SearchNewTarget(ref target, false))
            {
                Dancing = false;
                bool targetDistanceChanged = false;
                if (!LastPlayerPosition.Equals(Bot.Player.Position))
                {
                    distanceTraveled = Bot.Player.Position.GetDistance(LastPlayerPosition);
                    LastPlayerPosition = new Vector3(Bot.Player.Position.X, Bot.Player.Position.Y, Bot.Player.Position.Z);
                    targetDistanceChanged = true;
                }

                if (!LastTargetPosition.Equals(target.Position))
                {
                    computeNewRoute = true;
                    LastTargetPosition = new Vector3(target.Position.X, target.Position.Y, target.Position.Z);
                    targetDistanceChanged = true;
                }

                if (targetDistanceChanged)
                {
                    distanceToTarget = LastPlayerPosition.GetDistance(LastTargetPosition);
                }

                HandleMovement(target);
                HandleAttacking(target);
            }
            else if (!Dancing)
            {
                if (distanceTraveled < 0.001)
                {
                    Bot.Wow.ClearTarget();
                    Bot.Wow.SendChatMessage(standingEmotes[new Random().Next(standingEmotes.Length)]);
                    Dancing = true;
                }
                else
                {
                    Bot.Wow.ClearTarget();
                    Bot.Wow.SendChatMessage(runningEmotes[new Random().Next(runningEmotes.Length)]);
                    Dancing = true;
                }
            }
        }

        /// <summary>
        /// Loads the specified objects into the Configureables property.
        /// </summary>
        /// <param name="objects">A dictionary containing the objects to load.</param>
        public void Load(Dictionary<string, JsonElement> objects)
        {
            Configureables = objects["Configureables"].ToDyn();
        }

        /// <summary>
        /// Executes actions when the character is out of combat.
        /// </summary>
        public void OutOfCombatExecute()
        {
            computeNewRoute = false;
            if (!LastPlayerPosition.Equals(Bot.Player.Position))
            {
                distanceTraveled = Bot.Player.Position.GetDistance(LastPlayerPosition);
                LastPlayerPosition = new Vector3(Bot.Player.Position.X, Bot.Player.Position.Y, Bot.Player.Position.Z);
            }

            if (distanceTraveled < 0.001)
            {
                ulong leaderGuid = Bot.Objects.Partyleader.Guid;
                IWowUnit target = Bot.Target;
                IWowUnit leader = null;
                if (leaderGuid != 0)
                {
                    leader = Bot.GetWowObjectByGuid<IWowUnit>(leaderGuid);
                }

                if (leaderGuid != 0 && leaderGuid != Bot.Wow.PlayerGuid && leader != null && !(leader.IsDead || leader.Health < 1))
                {
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, Bot.GetWowObjectByGuid<IWowUnit>(leaderGuid).Position);
                }
                else if ((Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1)) || SearchNewTarget(ref target, true))
                {
                    if (!LastTargetPosition.Equals(target.Position))
                    {
                        computeNewRoute = true;
                        LastTargetPosition = new Vector3(target.Position.X, target.Position.Y, target.Position.Z);
                        distanceToTarget = LastPlayerPosition.GetDistance(LastTargetPosition);
                    }

                    Dancing = false;
                    HandleMovement(target);
                    HandleAttacking(target);
                }
                else if (!Dancing || standing)
                {
                    standing = false;
                    Bot.Wow.ClearTarget();
                    Bot.Wow.SendChatMessage(standingEmotes[new Random().Next(standingEmotes.Length)]);
                    Dancing = true;
                }
            }
            else
            {
                if (!Dancing || !standing)
                {
                    standing = true;
                    Bot.Wow.ClearTarget();
                    Bot.Wow.SendChatMessage(runningEmotes[new Random().Next(runningEmotes.Length)]);
                    Dancing = true;
                }
            }
        }

        /// <summary>
        /// Saves the dictionary of configureables.
        /// </summary>
        public Dictionary<string, object> Save()
        {
            return new()
            {
                { "configureables", Configureables }
            };
        }

        /// <summary>
        /// Handles attacking a target in World of Warcraft.
        /// </summary>
        /// <param name="target">The target to be attacked.</param>
        private void HandleAttacking(IWowUnit target)
        {
            Bot.Wow.ChangeTarget(target.Guid);
            spells.CastNextSpell(distanceToTarget, target, multipleTargets);
            if (target.IsDead || target.Health < 1)
            {
                spells.ResetAfterTargetDeath();
            }
        }

        /// <summary>
        /// Handles movement for the provided target in the game.
        /// </summary>
        /// <param name="target">The target to move towards.</param>
        private void HandleMovement(IWowUnit target)
        {
            if (target == null)
            {
                return;
            }

            if (Bot.Movement.Status != Movement.Enums.MovementAction.None && distanceToTarget < 0.75f * (Bot.Player.CombatReach + target.CombatReach))
            {
                Bot.Movement.StopMovement();
            }

            if (computeNewRoute)
            {
                if (!BotMath.IsFacing(LastPlayerPosition, Bot.Player.Rotation, LastTargetPosition, 0.5f))
                {
                    Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, target.Position);
                }

                Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, target.Position, target.Rotation);
            }
        }

        /// <summary>
        /// Searches for a new target to attack.
        /// </summary>
        /// <param name="target">Reference to the current target.</param>
        /// <param name="grinding">Flag indicating if the player is grinding.</param>
        /// <returns>True if a new target is found, false otherwise.</returns>
        private bool SearchNewTarget(ref IWowUnit target, bool grinding)
        {
            if (Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1 || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem"))))
            {
                return false;
            }

            List<IWowUnit> wowUnits = Bot.Objects.All.OfType<IWowUnit>().Where(e => Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Neutral).ToList();
            bool newTargetFound = false;
            int targetHealth = (target == null || target.IsDead || target.Health < 1) ? 2147483647 : target.Health;
            bool inCombat = target != null && target.IsInCombat;
            int targetCount = 0;
            multipleTargets = false;
            foreach (IWowUnit unit in wowUnits)
            {
                if (IWowUnit.IsValid(unit) && unit != target && !(unit.IsDead || unit.Health < 1 || unit.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem"))))
                {
                    double tmpDistance = Bot.Player.Position.GetDistance(unit.Position);
                    if (tmpDistance < 100.0 || grinding)
                    {
                        if (tmpDistance < 6.0)
                        {
                            targetCount++;
                        }

                        if (((unit.IsInCombat && unit.Health < targetHealth) || (!inCombat && grinding && unit.Health < targetHealth)) && Bot.Wow.IsInLineOfSight(Bot.Player.Position, unit.Position))
                        {
                            target = unit;
                            targetHealth = unit.Health;
                            newTargetFound = true;
                            inCombat = unit.IsInCombat;
                        }
                    }
                }
            }

            if (target == null || !(target.IsDead || target.Health < 1 || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem"))))
            {
                Bot.Wow.ClearTarget();
                newTargetFound = false;
                target = null;
            }
            else if (targetCount > 1)
            {
                multipleTargets = true;
            }

            if (newTargetFound)
            {
                Bot.Wow.ChangeTarget(target.Guid);
                spells.ResetAfterTargetDeath();
            }

            return newTargetFound;
        }

        /// <summary>
        /// The WarriorFurySpells class is responsible for managing the fury spells of a warrior.
        /// </summary>
        private class WarriorFurySpells
        {
            /// <summary>
            /// The name of the Battle Shout ability.
            /// </summary>
            private static readonly string BattleShout = "Battle Shout";
            /// <summary>
            /// Represents the battle stance string constant.
            /// </summary>
            private static readonly string BattleStance = "Battle Stance";
            /// <summary>
            /// Represents the name of the ability "Berserker Rage".
            /// </summary>
            private static readonly string BerserkerRage = "Berserker Rage";
            /// <summary>
            /// The name of the berserker stance.
            /// </summary>
            private static readonly string BerserkerStance = "Berserker Stance";
            /// <summary>
            /// The name of the ability "Bloodthirst".
            /// </summary>
            private static readonly string Bloodthirst = "Bloodthirst";
            /// <summary>
            /// Represents a string constant named "Charge".
            /// </summary>
            private static readonly string Charge = "Charge";
            /// <summary>
            /// Represents the constant string "Death Wish" that is readonly and private.
            /// </summary>
            private static readonly string DeathWish = "Death Wish";
            /// <summary>
            /// The private static constant string representing the ability "Enraged Regeneration".
            /// </summary>
            private static readonly string EnragedRegeneration = "Enraged Regeneration";
            /// <summary>
            /// Represents the constant variable for "Execute".
            /// </summary>
            private static readonly string Execute = "Execute";
            /// <summary>
            /// A readonly string representing the value "Hamstring".
            /// </summary>
            private static readonly string Hamstring = "Hamstring";
            /// <summary>
            /// Represents the name of the Heroic Strike ability.
            /// </summary>
            private static readonly string HeroicStrike = "Heroic Strike";
            /// <summary>
            /// Represents the constant value for the heroic throw ability.
            /// </summary>
            private static readonly string HeroicThrow = "Heroic Throw";
            /// <summary>
            /// The constant string representing the intercept value.
            /// </summary>
            private static readonly string Intercept = "Intercept";
            /// <summary>
            /// The name of the constant string "Recklessness".
            /// </summary>
            private static readonly string Recklessness = "Recklessness";
            /// <summary>
            /// Represents the constant value for "Retaliation".
            /// </summary>
            private static readonly string Retaliation = "Retaliation";
            /// <summary>
            /// This field represents the string value "Shattering Throw".
            /// </summary>
            private static readonly string ShatteringThrow = "Shattering Throw";
            /// <summary>
            /// Private static readonly string variable Slam with value "Slam".
            /// </summary>
            private static readonly string Slam = "Slam";
            /// <summary>
            /// Represents a constant string named "Whirlwind".
            /// </summary>
            private static readonly string Whirlwind = "Whirlwind";

            /// <summary>
            /// Represents a reference to the <see cref="AmeisenBotInterfaces"/> interface object.
            /// </summary>
            /// <summary>
            /// Private readonly field that represents an instance of the AmeisenBotInterfaces class.
            /// </summary>
            private readonly AmeisenBotInterfaces Bot;

            /// <summary>
            /// Dictionary that stores the next action time for various skills.
            /// </summary>
            private readonly Dictionary<string, DateTime> nextActionTime = new()
            {
                { BattleShout, DateTime.Now },
                { BattleStance, DateTime.Now },
                { BerserkerStance, DateTime.Now },
                { BerserkerRage, DateTime.Now },
                { Slam, DateTime.Now },
                { Recklessness, DateTime.Now },
                { DeathWish, DateTime.Now },
                { Intercept, DateTime.Now },
                { ShatteringThrow, DateTime.Now },
                { HeroicThrow, DateTime.Now },
                { Charge, DateTime.Now },
                { Bloodthirst, DateTime.Now },
                { Hamstring, DateTime.Now },
                { Execute, DateTime.Now },
                { Whirlwind, DateTime.Now },
                { Retaliation, DateTime.Now },
                { EnragedRegeneration, DateTime.Now },
                { HeroicStrike, DateTime.Now }
            };

            /// <summary>
            /// Indicates whether a heal has been requested.
            /// </summary>
            private bool askedForHeal = false;
            /// <summary>
            /// Indicates whether help has been asked for.
            /// </summary>
            private bool askedForHelp = false;

            /// <summary>
            /// Initializes a new instance of the WarriorFurySpells class.
            /// </summary>
            /// <param name="bot">The AmeisenBotInterfaces instance.</param>
            public WarriorFurySpells(AmeisenBotInterfaces bot)
            {
                Bot = bot;
                Player = Bot.Player;
                IsInBerserkerStance = false;
                NextGCDSpell = DateTime.Now;
                NextStance = DateTime.Now;
                NextCast = DateTime.Now;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the character is in Berserker Stance.
            /// </summary>
            private bool IsInBerserkerStance { get; set; }

            /// <summary>
            /// Gets or sets the next cast date and time.
            /// </summary>
            private DateTime NextCast { get; set; }

            /// <summary>
            /// Gets or sets the date and time for the next GCDSpell.
            /// </summary>
            private DateTime NextGCDSpell { get; set; }

            /// <summary>
            /// Gets or sets the next stance date and time.
            /// </summary>
            private DateTime NextStance { get; set; }

            /// <summary>
            /// Gets or sets the Wow player.
            /// </summary>
            private IWowPlayer Player { get; set; }

            /// <summary>
            /// Casts the next spell based on the distance to the target, the target's health percentage, and the player's rage level.
            /// </summary>
            /// <param name="distanceToTarget">The distance to the target.</param>
            /// <param name="target">The target to cast the spell on.</param>
            /// <param name="multipleTargets">A boolean value indicating whether there are multiple targets.</param>
            public void CastNextSpell(double distanceToTarget, IWowUnit target, bool multipleTargets)
            {
                if (!IsReady(NextCast) || !IsReady(NextGCDSpell))
                {
                    return;
                }

                if (!Bot.Player.IsAutoAttacking)
                {
                    Bot.Wow.StartAutoAttack();
                }

                Player = Bot.Player;
                int rage = Player.Rage;
                bool lowHealth = Player.HealthPercentage <= 25;
                bool mediumHealth = !lowHealth && Player.HealthPercentage <= 75;
                if (!(lowHealth || mediumHealth))
                {
                    askedForHelp = false;
                    askedForHeal = false;
                }
                else if (lowHealth && !askedForHelp)
                {
                    Bot.Wow.SendChatMessage("/helpme");
                    askedForHelp = true;
                }
                else if (mediumHealth && !askedForHeal)
                {
                    Bot.Wow.SendChatMessage("/healme");
                    askedForHeal = true;
                }

                if (lowHealth && rage > 15 && IsReady(EnragedRegeneration))
                {
                    Bot.Wow.SendChatMessage("/s Oh shit");
                    CastSpell(EnragedRegeneration, ref rage, 15, 180, false);
                }

                if (!lowHealth && rage > 10 && IsReady(DeathWish))
                {
                    CastSpell(DeathWish, ref rage, 10, 120.6, true); // lasts 30 sec
                }
                else if (rage > 10 && IsReady(BattleShout))
                {
                    Bot.Wow.SendChatMessage("/roar");
                    CastSpell(BattleShout, ref rage, 10, 120, true); // lasts 2 min
                }
                else if (IsInBerserkerStance && rage > 10 && Player.HealthPercentage > 50 && IsReady(Recklessness))
                {
                    CastSpell(Recklessness, ref rage, 0, 201, true); // lasts 12 sec
                }
                else if (Player.Health < Player.MaxHealth && IsReady(BerserkerRage))
                {
                    CastSpell(BerserkerRage, ref rage, 0, 20.1, true); // lasts 10 sec
                }

                if (distanceToTarget < (26 + target.CombatReach))
                {
                    if (distanceToTarget < (21 + target.CombatReach))
                    {
                        if (distanceToTarget > (9 + target.CombatReach))
                        {
                            // -- run to the target! --
                            if (Player.IsInCombat)
                            {
                                if (IsInBerserkerStance)
                                {
                                    // intercept
                                    if (rage > 10 && IsReady(Intercept))
                                    {
                                        CastSpell(Intercept, ref rage, 10, 30, true);
                                    }
                                }
                                else
                                {
                                    // Berserker Stance
                                    if (IsReady(NextStance))
                                    {
                                        ChangeToStance(BerserkerStance, out rage);
                                    }
                                }
                            }
                            else
                            {
                                if (IsInBerserkerStance)
                                {
                                    // Battle Stance
                                    if (IsReady(NextStance))
                                    {
                                        ChangeToStance(BattleStance, out rage);
                                    }
                                }
                                else
                                {
                                    // charge
                                    if (IsReady(Charge))
                                    {
                                        Bot.Wow.SendChatMessage("/incoming");
                                        CastSpell(Charge, ref rage, 0, 15, false);
                                    }
                                }
                            }
                        }
                        else if (distanceToTarget <= 0.75f * (Player.CombatReach + target.CombatReach))
                        {
                            // -- close combat -- Berserker Stance
                            if (!IsInBerserkerStance && IsReady(NextStance))
                            {
                                if (IsReady(Retaliation))
                                {
                                    CastSpell(Retaliation, ref rage, 0, 300, false);
                                }

                                ChangeToStance(BerserkerStance, out rage);
                            }
                            else if (mediumHealth && rage > 20 && IsReady(Bloodthirst))
                            {
                                CastSpell(Bloodthirst, ref rage, 20, 4, true);
                            }
                            else
                            {
                                List<string> buffs = Bot.Player.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
                                if (buffs.Any(e => e.Contains("slam") || e.Contains("Slam")) && rage > 15)
                                {
                                    CastSpell(Slam, ref rage, 15, 0, false);
                                    NextCast = DateTime.Now.AddSeconds(1.5); // casting time
                                    NextGCDSpell = DateTime.Now.AddSeconds(3.0); // 1.5 sec gcd after the 1.5 sec casting time
                                }
                                else if (rage > 10 && IsReady(Hamstring))
                                {
                                    CastSpell(Hamstring, ref rage, 10, 15, true);
                                }
                                else if (target.HealthPercentage <= 20 && rage > 10)
                                {
                                    CastSpell(Execute, ref rage, 10, 0, true);
                                }
                                else if (((multipleTargets && rage > 25) || rage > 50) && IsReady(Whirlwind))
                                {
                                    CastSpell(Whirlwind, ref rage, 25, 10, true);
                                }
                                else if (rage > 12 && IsReady(HeroicStrike))
                                {
                                    CastSpell(HeroicStrike, ref rage, 12, 3.6, false);
                                }
                                else if (!Bot.Player.IsAutoAttacking)
                                {
                                    Bot.Wow.StartAutoAttack();
                                }
                            }
                        }
                    }
                    else
                    {
                        // shattering throw (in Battle Stance)
                        if (rage > 25 && IsReady(ShatteringThrow))
                        {
                            if (IsInBerserkerStance)
                            {
                                if (IsReady(NextStance))
                                {
                                    ChangeToStance(BattleStance, out rage);
                                }
                            }
                            else
                            {
                                CastSpell(ShatteringThrow, ref rage, 25, 301.5, false);
                                NextCast = DateTime.Now.AddSeconds(1.5); // casting time
                                NextGCDSpell = DateTime.Now.AddSeconds(3.0); // 1.5 sec gcd after the 1.5 sec casting time
                            }
                        }
                        else
                        {
                            CastSpell(HeroicThrow, ref rage, 0, 60, true);
                        }
                    }
                }
            }

            /// <summary>
            /// Resets the next action time for Hamstring and Heroic Strike after target's death.
            /// </summary>
            public void ResetAfterTargetDeath()
            {
                nextActionTime[Hamstring].AddSeconds(-15.0);
                nextActionTime[HeroicStrike].AddSeconds(-3.6);
            }

            /// <summary>
            /// Determines if the specified date and time for the next action has already passed.
            /// </summary>
            /// <param name="nextAction">The date and time for the next action.</param>
            /// <returns>True if the current date and time is later than the specified next action, otherwise false.</returns>
            private static bool IsReady(DateTime nextAction)
            {
                return DateTime.Now > nextAction;
            }

            /// <summary>
            /// Casts a specified spell, deducts rage points, updates cooldown time and triggers global cooldown if necessary.
            /// </summary>
            private void CastSpell(string spell, ref int rage, int rageCosts, double cooldown, bool gcd)
            {
                Bot.Wow.CastSpell(spell);
                rage -= rageCosts;
                if (cooldown > 0)
                {
                    nextActionTime[spell] = DateTime.Now.AddSeconds(cooldown);
                }

                if (gcd)
                {
                    NextGCDSpell = DateTime.Now.AddSeconds(1.5);
                }
            }

            /// <summary>
            /// Changes the current stance to the specified stance and updates the rage value.
            /// </summary>
            /// <param name="stance">The new stance to change to.</param>
            /// <param name="rage">The updated rage value.</param>
            private void ChangeToStance(string stance, out int rage)
            {
                Bot.Wow.CastSpell(stance);
                rage = UpdateRage();
                NextStance = DateTime.Now.AddSeconds(1);
                IsInBerserkerStance = stance == BerserkerStance;
            }

            /// <summary>
            /// Determines if a given spell is ready to be used.
            /// </summary>
            /// <param name="spell">The name of the spell to check.</param>
            /// <returns>True if the spell is ready, otherwise false.</returns>
            private bool IsReady(string spell)
            {
                bool result = true; // begin with neutral element of AND
                if (spell.Equals(Hamstring) || spell.Equals(BattleShout))
                {
                    // only use these spells in a certain interval
                    result &= !nextActionTime.TryGetValue(spell, out DateTime NextSpellAvailable) || IsReady(NextSpellAvailable);
                }

                result &= Bot.Wow.GetSpellCooldown(spell) <= 0 && Bot.Wow.GetUnitCastingInfo(WowLuaUnit.Player).Item2 <= 0;
                return result;
            }

            /// <summary>
            /// Updates the rage level of the player and returns it.
            /// </summary>
            private int UpdateRage()
            {
                Player = Bot.Player;
                return Player.Rage;
            }
        }
    }
}