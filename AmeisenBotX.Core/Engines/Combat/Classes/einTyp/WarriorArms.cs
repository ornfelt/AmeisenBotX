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

namespace AmeisenBotX.Core.Engines.Combat.Classes.einTyp
{
    /// <summary>
    /// Represents a combat class for a warrior specialization that focuses on using arm spells.
    /// </summary>
    public class WarriorArms : ICombatClass
    {
        /// <summary>
        /// Represents a readonly instance of the AmeisenBotInterfaces class.
        /// </summary>
        private readonly AmeisenBotInterfaces Bot;
        /// <summary>
        /// This is an array of strings containing different running emotes.
        /// The emotes included are "/fart", "/burp", and "/moo".
        /// </summary>
        private readonly string[] runningEmotes = { "/fart", "/burp", "/moo" };
        /// <summary>
        /// The collection of arm spells for the warrior.
        /// </summary>
        private readonly WarriorArmSpells spells;
        /// <summary>
        /// Array of standing emotes.
        /// </summary>
        private readonly string[] standingEmotes = { "/chug", "/pick", "/whistle", "/violin" };
        /// <summary>
        /// A flag indicating whether a new route should be computed.
        /// </summary>
        private bool computeNewRoute = false;
        /// <summary>
        /// Represents the distance to the target.
        /// </summary>
        private double distanceToTarget = 0;
        /// <summary>
        /// Represents the distance traveled by an object.
        /// </summary>
        private double distanceTraveled = 0;
        /// <summary>
        /// Represents a boolean value indicating whether there are multiple targets. 
        /// The default value is false.
        /// </summary>
        private bool multipleTargets = false;
        /// <summary>
        /// Represents the current standing status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if standing; otherwise, <c>false</c>.
        /// </value>
        private bool standing = false;

        /// <summary>
        /// Initializes a new instance of the WarriorArms class with the specified bot.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces bot to assign.</param>
        public WarriorArms(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            spells = new WarriorArmSpells(bot);
        }

        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        /// <returns>
        /// The author of the code, which is always "einTyp".
        /// </returns>
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
        /// Gets or sets the Description of an object.
        /// </summary>
        public string Description => "...";

        /// <summary>
        /// Gets the display name of the Arms Warrior.
        /// </summary>
        public string DisplayName => "Arms Warrior";

        /// <summary>
        /// Gets a value indicating whether this handles facing.
        /// </summary>
        public bool HandlesFacing => false;

        /// <summary>
        /// Gets or sets a value indicating whether this code handles movement.
        /// </summary>
        public bool HandlesMovement => true;

        /// <summary>
        /// Gets the value indicating whether this character is a melee character.
        /// </summary>
        public bool IsMelee => true;

        /// <summary>
        /// Returns a new ArmsItemComparator object based on the Bot property.
        /// </summary>
        public IItemComparator ItemComparator => new ArmsItemComparator(Bot);

        /// <summary>
        /// Gets or sets the display IDs of the priority targets.
        /// </summary>
        /// <returns>An enumerable collection of integers representing the priority target display IDs.</returns>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets the role of the player as a Damage Per Second (DPS) role in World of Warcraft.
        /// </summary>
        public WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the TalentTree object that represents the talents of the character.
        /// </summary>
        public TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 3, new(1, 3, 2) },
                { 4, new(1, 4, 2) },
                { 6, new(1, 6, 3) },
                { 7, new(1, 7, 2) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 3) },
                { 12, new(1, 12, 3) },
                { 13, new(1, 13, 5) },
                { 14, new(1, 14, 1) },
                { 17, new(1, 17, 2) },
                { 21, new(1, 21, 1) },
                { 22, new(1, 22, 2) },
                { 24, new(1, 24, 1) },
                { 25, new(1, 25, 3) },
                { 26, new(1, 26, 2) },
                { 27, new(1, 27, 3) },
                { 28, new(1, 28, 1) },
                { 29, new(1, 29, 2) },
                { 30, new(1, 30, 5) },
                { 31, new(1, 31, 1) }
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 2, new(2, 2, 2) },
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 11, new(2, 11, 1) }
            },
            Tree3 = new()
        };

        /// <summary> 
        /// Gets the version of the software. 
        /// </summary>
        public string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player character can walk behind an enemy.
        /// </summary>
        /// <value>
        ///   <c>false</c> if the player character cannot walk behind an enemy; otherwise, <c>true</c>.
        /// </value>
        public bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the current WowClass which is Warrior.
        /// </summary>
        public WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Gets or sets a value indicating whether the person is dancing.
        /// </summary>
        private bool Dancing { get; set; }

        /// <summary>Gets or sets the last known position of the player in 3D space.</summary>
        private Vector3 LastPlayerPosition { get; set; }

        /// <summary>
        /// Gets or sets the last target position in 3D space.
        /// </summary>
        private Vector3 LastTargetPosition { get; set; }

        /// <summary>
        /// Attacks the target by either interacting with it or moving towards its position.
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
        /// Executes the code to handle movement and attacking for the current target.
        /// </summary>
        public void Execute()
        {
            computeNewRoute = false;
            IWowUnit target = Bot.Target;
            if ((Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1)) || SearchNewTarget(ref target, false))
            {
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
        }

        /// <summary>
        /// Loads the objects from the specified dictionary into the Configureables.
        /// </summary>
        public void Load(Dictionary<string, JsonElement> objects)
        {
            Configureables = objects["Configureables"].ToDyn();
        }

        /// <summary>
        /// Executes the actions to be performed while out of combat.
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
        /// Saves the list of configureables.
        /// </summary>
        /// <returns>A dictionary containing the saved configureables.</returns>
        public Dictionary<string, object> Save()
        {
            return new()
            {
                { "configureables", Configureables }
            };
        }

        /// <summary>
        /// Handles attacking the specified target.
        /// </summary>
        /// <param name="target">The target to attack.</param>
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
        /// Handles the movement of the character toward a target.
        /// If the target is null, the function returns.
        /// If the character is currently performing a movement action and the distance to the target is less than 0.75 times the sum of the character's combat reach and the target's combat reach, the movement is stopped.
        /// If computeNewRoute is true, the function checks if the character is facing the last player position and the last target position within a tolerance of 0.5.
        /// If the character is not facing the positions, the character is faced towards the target's position.
        /// Finally, the movement action is set to "Move" with the target's position and rotation.
        /// </summary>
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

        ///<summary>
        /// Searches for a new target for the bot to attack.
        ///</summary>
        ///<param name="target">Reference to the current target being attacked.</param>
        ///<param name="grinding">Specifies if the bot is grinding mobs.</param>
        ///<returns>Returns true if a new target is found, false otherwise.</returns>
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

            if (target == null || target.IsDead || target.Health < 1 || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem")))
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
        /// Represents a class that contains warrior arm spells.
        /// </summary>
        private class WarriorArmSpells
        {
            /// <summary>
            /// The constant string variable representing the battle shout.
            /// </summary>
            private static readonly string BattleShout = "Battle Shout";
            /// <summary>
            /// Represents the name of a battle stance.
            /// </summary>
            private static readonly string BattleStance = "Battle Stance";
            /// <summary>
            /// The constant variable representing Berserker Rage.
            /// </summary>
            private static readonly string BerserkerRage = "Berserker Rage";
            /// <summary>
            /// The name of the Berserker Stance.
            /// </summary>
            private static readonly string BerserkerStance = "Berserker Stance";
            /// <summary>
            /// A constant string representing the name "Bladestorm".
            /// </summary>
            private static readonly string Bladestorm = "Bladestorm";
            /// <summary>
            /// Represents the constant value "Bloodrage".
            /// </summary>
            private static readonly string Bloodrage = "Bloodrage";
            /// <summary>
            /// The constant string representing "Bloodthirst".
            /// </summary>
            private static readonly string Bloodthirst = "Bloodthirst";
            /// <summary>
            /// The constant string value for "Charge".
            /// </summary>
            private static readonly string Charge = "Charge";
            /// <summary>
            /// The constant string representing "Death Wish".
            /// </summary>
            private static readonly string DeathWish = "Death Wish";
            /// <summary>
            /// The name of the Enraged Regeneration ability.
            /// </summary>
            private static readonly string EnragedRegeneration = "Enraged Regeneration";
            /// <summary>
            /// The constant string value "Execute".
            /// </summary>
            private static readonly string Execute = "Execute";
            /// <summary>
            /// A constant string representing "Hamstring"
            /// </summary>
            private static readonly string Hamstring = "Hamstring";
            /// <summary>
            /// The name of the Heroic Strike ability.
            /// </summary>
            private static readonly string HeroicStrike = "Heroic Strike";
            /// <summary>
            /// The name of the Heroic Throw.
            /// </summary>
            private static readonly string HeroicThrow = "Heroic Throw";
            /// <summary>
            /// The constant string representing the intercept value for a specific operation.
            /// </summary>
            private static readonly string Intercept = "Intercept";
            /// <summary>
            /// The name of the intimidating shout ability.
            /// </summary>
            private static readonly string IntimidatingShout = "Intimidating Shout";
            /// <summary>
            /// The name of the Mortal Strike ability.
            /// </summary>
            private static readonly string MortalStrike = "Mortal Strike";
            /// <summary>
            /// Constant string representing "Recklessness".
            /// </summary>
            private static readonly string Recklessness = "Recklessness";
            /// <summary>
            /// Represents a read-only string variable named "Rend".
            /// </summary>
            private static readonly string Rend = "Rend";
            /// <summary>
            /// Constant string representing the retaliation value.
            /// </summary>
            private static readonly string Retaliation = "Retaliation";
            /// <summary>
            /// The name of the ability "Shattering Throw".
            /// </summary>
            private static readonly string ShatteringThrow = "Shattering Throw";
            /// <summary>
            /// Represents a constant string value "Slam".
            /// </summary>
            private static readonly string Slam = "Slam";
            /// <summary>
            /// Represents a constant string value "Whirlwind".
            /// </summary>
            private static readonly string Whirlwind = "Whirlwind";

            /// <summary>
            /// Represents a readonly instance of the AmeisenBotInterfaces class.
            /// </summary>
            /// <summary>
            /// Represents a private readonly field for the AmeisenBotInterfaces Bot.
            /// </summary>
            private readonly AmeisenBotInterfaces Bot;

            /// <summary>
            /// Dictionary that stores the next action time for each ability.
            /// Keys are ability names and values are the current system's date and time.
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
                { IntimidatingShout, DateTime.Now },
                { Bloodrage, DateTime.Now },
                { Bladestorm, DateTime.Now },
                { Rend, DateTime.Now },
                { MortalStrike, DateTime.Now },
                { HeroicStrike, DateTime.Now }
            };

            /// <summary>
            /// Indicates whether the heal was requested or not.
            /// </summary>
            private bool askedForHeal = false;
            /// <summary>
            /// Indicates whether help was requested or not.
            /// </summary>
            private bool askedForHelp = false;

            /// <summary>
            /// Initializes a new instance of the WarriorArmSpells class.
            /// </summary>
            /// <param name="bot">The AmeisenBotInterfaces object used to interact with the bot.</param>
            public WarriorArmSpells(AmeisenBotInterfaces bot)
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
            /// Gets or sets the value of the next cast date and time.
            /// </summary>
            private DateTime NextCast { get; set; }

            /// <summary>
            /// Gets or sets the date and time for the next GCD spell.
            /// </summary>
            private DateTime NextGCDSpell { get; set; }

            /// <summary>
            /// Gets or sets the next stance as a DateTime value.
            /// </summary>
            private DateTime NextStance { get; set; }

            /// <summary>
            /// Gets or sets the private property representing the WoW player.
            /// </summary>
            private IWowPlayer Player { get; set; }

            /// <summary>
            /// Casts the next spell based on the distance to the target, the target itself, and the option to target multiple enemies.
            /// </summary>
            /// <param name="distanceToTarget">The distance to the target.</param>
            /// <param name="target">The target to cast the spell on.</param>
            /// <param name="multipleTargets">Indicates whether there are multiple targets.</param>
            public void CastNextSpell(double distanceToTarget, IWowUnit target, bool multipleTargets)
            {
                if (!IsReady(NextCast))
                {
                    return;
                }

                if (!Bot.Player.IsAutoAttacking)
                {
                    Bot.Wow.StartAutoAttack();
                }

                Player = Bot.Player;
                int rage = Player.Rage;
                bool isGCDReady = IsReady(NextGCDSpell);
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

                // -- buffs --
                if (lowHealth && rage > 15 && IsReady(EnragedRegeneration))
                {
                    if (IsReady(EnragedRegeneration))
                    {
                        CastSpell(EnragedRegeneration, ref rage, 15, 180, false);
                    }
                }
                else if (rage < 20 && !lowHealth && !mediumHealth && IsReady(Bloodrage))
                {
                    CastSpell(Bloodrage, ref rage, 0, 40.2, false);
                }

                if (isGCDReady)
                {
                    // Berserker Rage
                    if (Player.Health < Player.MaxHealth && IsReady(BerserkerRage))
                    {
                        CastSpell(BerserkerRage, ref rage, 0, 20.1, true); // lasts 10 sec
                    }
                }

                if (multipleTargets)
                {
                    if (rage > 25 && IsReady(IntimidatingShout))
                    {
                        CastSpell(IntimidatingShout, ref rage, 25, 120, false);
                    }
                    else if (IsReady(Retaliation))
                    {
                        CastSpell(Retaliation, ref rage, 0, 300, false);
                    }
                }

                if (distanceToTarget < (29 + target.CombatReach))
                {
                    if (distanceToTarget < (24 + target.CombatReach))
                    {
                        if (distanceToTarget > (9 + target.CombatReach))
                        {
                            // -- run to the target! --
                            if (Player.IsInCombat)
                            {
                                if (isGCDReady)
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
                                        CastSpell(Charge, ref rage, 0, 15, false);
                                    }
                                }
                            }
                        }
                        else if (distanceToTarget <= 0.75f * (Player.CombatReach + target.CombatReach))
                        {
                            // -- close combat -- Battle Stance
                            if (IsInBerserkerStance && IsReady(NextStance))
                            {
                                ChangeToStance(BattleStance, out rage);
                            }
                            else if (isGCDReady)
                            {
                                List<string> buffs = Bot.Player.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
                                if (buffs.Any(e => e.Contains("slam") || e.Contains("Slam")) && rage > 15)
                                {
                                    CastSpell(Slam, ref rage, 15, 0, false);
                                    NextCast = DateTime.Now.AddSeconds(1.5);
                                    NextGCDSpell = DateTime.Now.AddSeconds(3.0);
                                }
                                else if (rage > 10 && IsReady(Rend))
                                {
                                    CastSpell(Rend, ref rage, 10, 15, true);
                                }
                                else if (rage > 10 && IsReady(Hamstring))
                                {
                                    CastSpell(Hamstring, ref rage, 10, 15, true);
                                }
                                else if (target.HealthPercentage <= 20 && rage > 10)
                                {
                                    CastSpell(Execute, ref rage, 10, 0, true);
                                }
                                else if (multipleTargets && rage > 25 && IsReady(Bladestorm))
                                {
                                    CastSpell(Bladestorm, ref rage, 25, 90, true);
                                }
                                else if (rage > 30 && IsReady(MortalStrike))
                                {
                                    CastSpell(MortalStrike, ref rage, 30, 6, true);
                                }
                                else if (rage > 12 && IsReady(HeroicStrike))
                                {
                                    CastSpell(HeroicStrike, ref rage, 12, 3.6, false);
                                }
                            }
                            else if (target.HealthPercentage > 20 && rage > 12 && IsReady(HeroicStrike))
                            {
                                CastSpell(HeroicStrike, ref rage, 12, 3.6, false);
                            }
                            else
                            {
                                if (!Bot.Player.IsAutoAttacking)
                                {
                                    Bot.Wow.StartAutoAttack();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isGCDReady)
                        {
                            // -- distant attacks --
                            if (isGCDReady)
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
                }
            }

            /// <summary>
            /// Resets the next action time for certain abilities after the target's death.
            /// </summary>
            public void ResetAfterTargetDeath()
            {
                nextActionTime[Hamstring].AddSeconds(-15.0);
                nextActionTime[Rend].AddSeconds(-15.0);
                nextActionTime[HeroicStrike].AddSeconds(-3.6);
            }

            /// <summary>
            /// Checks if the given <paramref name="nextAction"/> has already passed.
            /// </summary>
            /// <param name="nextAction">The DateTime value to be checked.</param>
            /// <returns>True if the current time is greater than the <paramref name="nextAction"/> value, otherwise false.</returns>
            private static bool IsReady(DateTime nextAction)
            {
                return DateTime.Now > nextAction;
            }

            /// <summary>
            /// Casts a spell on the Bot character in the game.
            /// </summary>
            /// <param name="spell">The name of the spell to be cast.</param>
            /// <param name="rage">The amount of rage the character has, which is updated by subtracting the rage cost of the spell.</param>
            /// <param name="rageCosts">The amount of rage that the spell costs to be cast.</param>
            /// <param name="cooldown">The cooldown duration of the spell, in seconds.</param>
            /// <param name="gcd">A boolean value indicating whether the spell triggers global cooldown (gcd) or not.</param>
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
            /// Changes the stance of the bot to the specified stance and updates the rage value.
            /// Sets the NextStance to 1 second after the current time and checks if the bot is in BerserkerStance.
            /// </summary>
            private void ChangeToStance(string stance, out int rage)
            {
                Bot.Wow.CastSpell(stance);
                rage = UpdateRage();
                NextStance = DateTime.Now.AddSeconds(1);
                IsInBerserkerStance = stance == BerserkerStance;
            }

            /// <summary>
            /// Determines if the given spell is ready to be used.
            /// </summary>
            /// <param name="spell">The spell to check for readiness.</param>
            /// <returns>True if the spell is ready; otherwise, false.</returns>
            private bool IsReady(string spell)
            {
                return !nextActionTime.TryGetValue(spell, out DateTime NextSpellAvailable) || IsReady(NextSpellAvailable);
            }

            /// <summary>
            /// Updates the rage value of the player.
            /// </summary>
            /// <returns>The updated rage value.</returns>
            private int UpdateRage()
            {
                Player = Bot.Player;
                return Player.Rage;
            }
        }
    }
}