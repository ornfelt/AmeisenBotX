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
    public class RogueAssassination : ICombatClass
    {
        /// <summary>
        /// This is a private readonly field of type AmeisenBotInterfaces, representing the bot.
        /// </summary>
        private readonly AmeisenBotInterfaces Bot;
        /// <summary>
        /// Represents a boolean value indicating whether the target has moved.
        /// </summary>
        private readonly bool hasTargetMoved = false;
        /// <summary>
        /// Holds the list of spells available to the Rogue Assassin.
        /// </summary>
        private readonly RogueAssassinSpells spells;
        /// <summary>
        /// An array of string containing one element "/bored".
        /// </summary>
        private readonly string[] standingEmotes = { "/bored" };
        /// <summary>
        /// Represents a boolean value indicating whether a new route should be computed or not.
        /// </summary>
        private bool computeNewRoute = false;

        /// <summary>
        /// Represents the distance to the target located behind.
        /// </summary>
        private double distanceToBehindTarget = 0;

        /// <summary>
        /// Represents the distance from the current object to the target.
        /// </summary>
        private double distanceToTarget = 0;

        /// <summary>
        /// Private field representing the distance traveled.
        /// </summary>
        private double distanceTraveled = 0;
        /// <summary>
        /// Determines if the character is attacking from behind or not.
        /// </summary>
        private bool isAttackingFromBehind = false;

        /// <summary>
        /// This private boolean variable represents if the current state is sneaky or not.
        /// </summary>
        private bool isSneaky = false;

        /// <summary>
        /// Represents the current standing status.
        /// </summary>
        private bool standing = false;

        /// <summary>
        /// Represents a boolean value indicating whether the object was in stealth mode.
        /// </summary>
        private bool wasInStealth = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RogueAssassination"/> class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance to which the RogueAssassination belongs.</param>
        public RogueAssassination(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            spells = new RogueAssassinSpells(bot);
        }

        /// <summary>
        /// Gets the author of the code, which is "einTyp".
        /// </summary>
        public string Author => "einTyp";

        /// <summary>
        /// Gets or sets the collection of blacklisted target display IDs.
        /// </summary>
        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of configurable items.
        /// </summary>
        public Dictionary<string, dynamic> Configureables { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description for the current instance.
        /// </summary>
        /// <returns>The description for the current instance.</returns>
        public string Description => "...";

        /// <summary>
        /// Gets or sets the display name for an Assasination Rogue.
        /// </summary>
        public string DisplayName => "Assasination Rogue";

        /// <summary>
        /// Specifies that the HandlesFacing property always returns false.
        /// </summary>
        public bool HandlesFacing => false;

        /// <summary>
        /// Gets a value indicating whether this instance handles movement.
        /// </summary>
        /// <returns>True.</returns>
        public bool HandlesMovement => true;

        /// <summary>
        /// Determines if the character uses melee combat.
        /// </summary>
        public bool IsMelee => true;

        /// <summary>
        /// Gets the ItemComparator which compares items for the Assassination scenario.
        /// </summary>
        public IItemComparator ItemComparator => new AssassinationItemComparator();

        /// <summary>
        /// Gets or sets the collection of priority target display IDs.
        /// </summary>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the role of the character as a Damage Per Second (DPS) role.
        /// </summary>
        public WowRole Role => WowRole.Dps;

        /// <summary>
        /// The TalentTree property represents the talent tree for a character.
        /// It is a collection of three trees: Tree1, Tree2, and Tree3.
        /// Each tree is a dictionary where the key represents the level at which the talent can be unlocked,
        /// and the value is an instance of the Talent class that contains the talent's details.
        /// </summary>
        public TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 3, new(1, 3, 5) },
                { 4, new(1, 4, 3) },
                { 5, new(1, 5, 2) },
                { 6, new(1, 6, 3) },
                { 9, new(1, 9, 5) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 5) },
                { 13, new(1, 13, 1) },
                { 16, new(1, 16, 5) },
                { 17, new(1, 17, 2) },
                { 19, new(1, 19, 1) },
                { 21, new(1, 21, 3) },
                { 22, new(1, 22, 3) },
                { 23, new(1, 23, 3) },
                { 24, new(1, 24, 1) },
                { 26, new(1, 26, 5) },
                { 27, new(1, 27, 1) }
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 6, new(2, 6, 5) },
                { 9, new(2, 9, 5) },
                { 12, new(2, 12, 3) }
            },
            Tree3 = new()
            {
                { 3, new(3, 3, 2) }
            }
        };

        /// <summary>
        /// Gets the version number which is set as "1.0".
        /// </summary>
        public string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind the enemy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the player can walk behind the enemy; otherwise, <c>false</c>.
        /// </value>
        public bool WalkBehindEnemy => false;

        ///<summary>Returns the instance of the <see cref="WowClass"/> representing the Rogue class.</summary>
        public WowClass WowClass => WowClass.Rogue;

        /// <summary>
        /// Gets or sets a value indicating whether the person is dancing or not.
        /// </summary>
        private bool Dancing { get; set; }

        /// <summary>
        /// Gets or sets the position of the last location behind the target in a 3D vector.
        /// </summary>
        private Vector3 LastBehindTargetPosition { get; set; }

        /// <summary>
        /// Gets or sets the last known position of the player.
        /// </summary>
        private Vector3 LastPlayerPosition { get; set; }

        /// <summary>
        /// Gets or sets the last recorded position of the target as a Vector3 object.
        /// </summary>
        private Vector3 LastTargetPosition { get; set; }

        /// <summary>
        /// Gets or sets the last target rotation value.
        /// </summary>
        private float LastTargetRotation { get; set; }

        /// <summary>
        /// Attacks the target if it is within a 3.0 distance from the player's position.
        /// If the target is not within range, the player's movement is set to move towards the target.
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
        /// Executes the logic for the current bot behavior.
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

                if (LastTargetRotation != target.Rotation)
                {
                    computeNewRoute = true;
                    LastTargetRotation = target.Rotation;
                }

                if (!LastTargetPosition.Equals(target.Position))
                {
                    computeNewRoute = true;
                    LastTargetPosition = new Vector3(target.Position.X, target.Position.Y, target.Position.Z);
                    LastBehindTargetPosition = BotMath.CalculatePositionBehind(target.Position, target.Rotation, 3f);
                    targetDistanceChanged = true;
                }

                if (targetDistanceChanged)
                {
                    distanceToTarget = LastPlayerPosition.GetDistance(LastTargetPosition);
                    distanceToBehindTarget = LastPlayerPosition.GetDistance(LastBehindTargetPosition);
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
                    Dancing = true;
                }
            }
        }

        ///<summary>
        ///Loads the Configureables from the provided dictionary of JSON elements.
        ///</summary>
        public void Load(Dictionary<string, JsonElement> objects)
        {
            Configureables = objects["Configureables"].ToDyn();
        }

        /// <summary>
        /// Executes actions when the character is out of combat.
        /// Updates the route, checks for stealth buffs, casts stealth if necessary, and resets spells after target death.
        /// Calculates the distance traveled by the character.
        /// Updates the last player position.
        /// Searches for a new target if the current target is not valid.
        /// Updates the last target position and behind target position.
        /// Handles movement and attacking if a target is found.
        /// Handles dancing and standing emotes if no target is found.
        /// Clears the target and sets dancing to true if the character is moving.
        /// Clears the target and sets dancing to true if the character is standing still.
        /// </summary>
        public void OutOfCombatExecute()
        {
            computeNewRoute = false;
            List<string> buffs = Bot.Player.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
            if (!buffs.Any(e => e.Contains("tealth")))
            {
                Bot.Wow.CastSpell("Stealth");
                spells.ResetAfterTargetDeath();
            }

            if (!LastPlayerPosition.Equals(Bot.Player.Position))
            {
                distanceTraveled = Bot.Player.Position.GetDistance(LastPlayerPosition);
                LastPlayerPosition = new Vector3(Bot.Player.Position.X, Bot.Player.Position.Y, Bot.Player.Position.Z);
            }

            if (distanceTraveled < 0.001)
            {
                ulong leaderGuid = Bot.Objects.Partyleader.Guid;
                IWowUnit target = Bot.Target;
                if ((Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1)) || SearchNewTarget(ref target, true))
                {
                    if (!LastTargetPosition.Equals(target.Position))
                    {
                        computeNewRoute = true;
                        LastTargetPosition = new Vector3(target.Position.X, target.Position.Y, target.Position.Z);
                        LastBehindTargetPosition = BotMath.CalculatePositionBehind(target.Position, target.Rotation, 5f);
                        distanceToTarget = LastPlayerPosition.GetDistance(LastTargetPosition);
                        distanceToBehindTarget = LastPlayerPosition.GetDistance(LastBehindTargetPosition);
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
                    Dancing = true;
                }
            }
        }

        /// <summary>
        /// Saves the configureables in a dictionary and returns it.
        /// </summary>
        /// <returns>A dictionary containing the configureables.</returns>
        public Dictionary<string, object> Save()
        {
            return new()
            {
                { "configureables", Configureables }
            };
        }

        /// <summary>
        /// Handles attacking the specified target by changing the bot's target to the target's GUID, casting the next spell on the target based on the distance to the target, and resetting the spells after the target's death if the target is dead or has health less than 1.
        /// </summary>
        private void HandleAttacking(IWowUnit target)
        {
            Bot.Wow.ChangeTarget(target.Guid);
            spells.CastNextSpell(distanceToTarget, target);
            if (target.IsDead || target.Health < 1)
            {
                spells.ResetAfterTargetDeath();
            }
        }

        /// <summary>
        /// Handles movement of the character towards the specified target.
        /// </summary>
        /// <param name="target">The target to move towards.</param>
        private void HandleMovement(IWowUnit target)
        {
            if (target == null)
            {
                return;
            }

            if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("tealth")))
            {
                if (!wasInStealth || hasTargetMoved)
                {
                    isSneaky = true;
                }

                wasInStealth = true;
            }
            else
            {
                isSneaky = false;
                wasInStealth = false;
            }

            if (isAttackingFromBehind)
            {
                if (Bot.Movement.Status != Movement.Enums.MovementAction.None && distanceToTarget < 0.75f * (Bot.Player.CombatReach + target.CombatReach))
                {
                    Bot.Movement.StopMovement();
                }

                if (Bot.Player.IsInCombat)
                {
                    isAttackingFromBehind = false;
                }
            }

            if (computeNewRoute)
            {
                if (!isAttackingFromBehind && isSneaky && distanceToBehindTarget > 0.75f * (Bot.Player.CombatReach + target.CombatReach))
                {
                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, LastBehindTargetPosition);
                }
                else
                {
                    isAttackingFromBehind = true;
                    if (!BotMath.IsFacing(LastPlayerPosition, Bot.Player.Rotation, LastTargetPosition, 0.5f))
                    {
                        Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, target.Position);
                    }

                    Bot.Movement.SetMovementAction(Movement.Enums.MovementAction.Move, LastTargetPosition, LastTargetRotation);
                }
            }
        }

        /// <summary>
        /// Searches for a new target for the Bot to attack.
        /// </summary>
        /// <param name="target">A reference to the current target being attacked.</param>
        /// <param name="grinding">A boolean indicating whether the Bot is currently grinding.</param>
        /// <returns>True if a new target is found, false otherwise.</returns>
        private bool SearchNewTarget(ref IWowUnit target, bool grinding)
        {
            List<string> buffs = Bot.Player.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
            if ((Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1 || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem")))) || (buffs.Any(e => e.Contains("tealth")) && Bot.Player.HealthPercentage <= 20))
            {
                return false;
            }

            List<IWowUnit> wowUnits = Bot.Objects.All.OfType<IWowUnit>().Where(e => Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Neutral).ToList();
            bool newTargetFound = false;
            int targetHealth = (target == null || target.IsDead || target.Health < 1) ? 0 : target.Health;
            bool inCombat = target != null && target.IsInCombat;
            int targetCount = 0;
            foreach (IWowUnit unit in wowUnits)
            {
                if (IWowUnit.IsValid(unit) && unit != target && !(unit.IsDead || unit.Health < 1 || unit.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem"))))
                {
                    double tmpDistance = Bot.Player.Position.GetDistance(unit.Position);
                    if ((isSneaky && tmpDistance < 100.0) || isSneaky && tmpDistance < 50.0)
                    {
                        if (tmpDistance < 6.0)
                        {
                            targetCount++;
                        }

                        if (((unit.IsInCombat && unit.Health > targetHealth) || (!inCombat && grinding && unit.Health > targetHealth)) && Bot.Wow.IsInLineOfSight(Bot.Player.Position, unit.Position))
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

            if (newTargetFound)
            {
                Bot.Wow.ChangeTarget(target.Guid);
                spells.ResetAfterTargetDeath();
            }

            return newTargetFound;
        }

        private class RogueAssassinSpells
        {
            /// <summary>
            /// Represents the name of the ambush.
            /// </summary>
            private static readonly string Ambush = "Ambush";
            /// <summary>
            /// Represents the string "Cold Blood" which is a constant and cannot be modified.
            /// </summary>
            private static readonly string ColdBlood = "Cold Blood";
            /// <summary>
            /// The constant string representing "Deadly Throw".
            /// </summary>
            private static readonly string DeadlyThrow = "Deadly Throw";
            /// <summary>
            /// Represents the name of the Envenom ability.
            /// </summary>
            private static readonly string Envenom = "Envenom";
            /// <summary>
            /// The constant string that represents "Eviscerate".
            /// </summary>
            private static readonly string Eviscerate = "Eviscerate";
            /// <summary>
            /// This is a private static readonly string variable called Garrote.
            /// It is initialized with the value "Garrote".
            /// </summary>
            private static readonly string Garrote = "Garrote";
            /// <summary>
            /// Represents the constant string "Hunger For Blood".
            /// </summary>
            private static readonly string HungerForBlood = "Hunger For Blood";
            /// <summary>
            /// Represents the constant value for the action "Kick".
            /// </summary>
            private static readonly string Kick = "Kick";
            /// <summary>
            /// Private static readonly string variable named Mutilate with value "Mutilate".
            /// </summary>
            private static readonly string Mutilate = "Mutilate";
            /// <summary>
            /// Represents a constant string value that signifies overkill.
            /// </summary>
            private static readonly string Overkill = "Overkill";
            /// <summary>
            /// Represents the constant string "Rupture".
            /// </summary>
            private static readonly string Rupture = "Rupture";
            /// <summary>
            /// The constant string value representing "Sinister Strike".
            /// </summary>
            private static readonly string SinisterStrike = "Sinister Strike";
            /// <summary>
            /// The constant variable representing "Slice and Dice".
            /// </summary>
            private static readonly string SliceAndDice = "Slice and Dice";
            /// <summary>
            /// Represents a constant string value for the sprint.
            /// </summary>
            private static readonly string Sprint = "Sprint";
            /// <summary>
            /// Represents the constant string "Stealth".
            /// </summary>
            private static readonly string Stealth = "Stealth";
            /// <summary>
            /// Represents the constant value for the throw attack.
            /// </summary>
            private static readonly string ThrowAttack = "Throw";
            /// <summary>
            /// Represents the constant string "Vanish".
            /// </summary>
            private static readonly string Vanish = "Vanish";

            /// <summary>
            /// This is a private readonly field of type AmeisenBotInterfaces, representing the bot.
            /// </summary>
            /// <summary>
            /// Represents a read-only instance of the interface AmeisenBotInterfaces.
            /// </summary>
            private readonly AmeisenBotInterfaces Bot;

            /// <summary>
            /// Dictionary that stores the next action time for each ability.
            /// </summary>
            private readonly Dictionary<string, DateTime> nextActionTime = new()
            {
                { Garrote, DateTime.Now },
                { Ambush, DateTime.Now },
                { HungerForBlood, DateTime.Now },
                { SliceAndDice, DateTime.Now },
                { Mutilate, DateTime.Now },
                { Envenom, DateTime.Now },
                { Vanish, DateTime.Now },
                { Overkill, DateTime.Now },
                { ColdBlood, DateTime.Now },
                { Stealth, DateTime.Now },
                { Sprint, DateTime.Now },
                { SinisterStrike, DateTime.Now },
                { Rupture, DateTime.Now },
                { Eviscerate, DateTime.Now },
                { DeadlyThrow, DateTime.Now },
                { Kick, DateTime.Now }
            };

            /// <summary>
            /// Represents a boolean value indicating whether the heal was requested or not.
            /// </summary>
            private bool askedForHeal = false;

            /// <summary>
            /// Represents a flag indicating whether help has been requested.
            /// </summary>
            private bool askedForHelp = false;

            /// <summary>
            /// Private variable to keep track of the combo count.
            /// </summary>
            private int comboCnt = 0;

            ///<summary>
            ///Constructor for RogueAssassinSpells class.
            ///Initializes a new instance of the class with the given bot and sets the bot and player properties.
            ///Sets the NextGCDSpell and NextCast to the current date and time.
            ///</summary>
            public RogueAssassinSpells(AmeisenBotInterfaces bot)
            {
                Bot = bot;
                Player = Bot.Player;
                NextGCDSpell = DateTime.Now;
                NextCast = DateTime.Now;
            }

            /// <summary>
            /// Gets or sets the next casting date and time.
            /// </summary>
            private DateTime NextCast { get; set; }

            /// <summary>
            /// Gets or sets the next time the GCDSpell property will be computed.
            /// </summary>
            private DateTime NextGCDSpell { get; set; }

            /// <summary>
            /// Gets or sets the private property representing the WoW player.
            /// </summary>
            private IWowPlayer Player { get; set; }

            /// <summary>
            /// Casts the next spell based on the distance to the target and the current player's health and energy levels.
            /// </summary>
            /// <param name="distanceToTarget">The distance from the player to the target.</param>
            /// <param name="target">The target unit.</param>
            public void CastNextSpell(double distanceToTarget, IWowUnit target)
            {
                if (!IsReady(NextCast) || !IsReady(NextGCDSpell))
                {
                    return;
                }

                if (!Bot.Player.IsAutoAttacking && !IsInStealth())
                {
                    Bot.Wow.StartAutoAttack();
                }

                Player = Bot.Player;
                int energy = Player.Energy;
                bool lowHealth = Player.HealthPercentage <= 20;
                bool mediumHealth = !lowHealth && Player.HealthPercentage <= 50;
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

                // -- stealth --
                if (!IsInStealth())
                {
                    if (!Player.IsInCombat)
                    {
                        CastSpell(Stealth, ref energy, 0, 1, false);
                    }
                    else if (lowHealth)
                    {
                        if (IsReady(Vanish))
                        {
                            CastSpell(Vanish, ref energy, 0, 180, false);
                            Bot.Wow.ClearTarget();
                            return;
                        }
                    }
                }

                // combat
                if (distanceToTarget < (29 + target.CombatReach))
                {
                    // in range
                    if (energy > 15 && IsReady(HungerForBlood) && IsTargetBleeding() && !IsInStealth())
                    {
                        CastSpell(HungerForBlood, ref energy, 15, 60, true);
                    }

                    if (distanceToTarget < (24 + target.CombatReach))
                    {
                        if (distanceToTarget > (9 + target.CombatReach))
                        {
                            // 9 < distance < 24 run?
                            if (energy > 15 && IsReady(Sprint) && IsTargetBleeding())
                            {
                                CastSpell(Sprint, ref energy, 15, 180, true);
                            }
                        }
                        else if (distanceToTarget <= 0.75f * (Player.CombatReach + target.CombatReach))
                        {
                            // distance <= 9 close combat
                            if (IsInStealth())
                            {
                                if (energy > 50 && IsReady(Garrote) && !IsTargetBleeding())
                                {
                                    CastSpell(Garrote, ref energy, 50, 3, true);
                                    comboCnt++;
                                }
                                else if (energy > 60)
                                {
                                    CastSpell(Ambush, ref energy, 60, 0, true);
                                    comboCnt += 2;
                                }
                            }
                            else
                            {
                                if (Bot.Wow.GetUnitCastingInfo(WowLuaUnit.Target).Item2 > 0 && energy > 25 && IsReady(Kick))
                                {
                                    CastSpell(Kick, ref energy, 25, 10, true);
                                }
                                else if (comboCnt > 4 && energy > 35 && IsTargetPoisoned())
                                {
                                    if (IsReady(ColdBlood))
                                    {
                                        CastSpell(ColdBlood, ref energy, 0, 180, false);
                                    }

                                    CastSpell(Envenom, ref energy, 35, 0, true);
                                    comboCnt -= 5;
                                }
                                else if (comboCnt > 4 && energy > 35)
                                {
                                    if (IsReady(ColdBlood))
                                    {
                                        CastSpell(ColdBlood, ref energy, 0, 180, false);
                                    }

                                    CastSpell(Eviscerate, ref energy, 35, 0, true);
                                    comboCnt -= 5;
                                }
                                else if (comboCnt > 0 && energy > 25 && IsReady(SliceAndDice))
                                {
                                    int comboCntUsed = Math.Min(5, comboCnt);
                                    CastSpell(SliceAndDice, ref energy, 25, 6 + (3 * comboCntUsed), true);
                                    comboCnt -= comboCntUsed;
                                }
                                else if (energy > 60 && IsTargetPoisoned())
                                {
                                    CastSpell(Mutilate, ref energy, 60, 0, true);
                                    comboCnt += 2;
                                }
                                else if (energy > 45)
                                {
                                    CastSpell(SinisterStrike, ref energy, 45, 0, true);
                                    comboCnt++;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 24 <= distance < 29 distance attacks
                        if (Player.IsInCombat)
                        {
                            if (comboCnt > 4 && energy > 35 && IsReady(DeadlyThrow))
                            {
                                CastSpell(DeadlyThrow, ref energy, 35, 1.5, true);
                                comboCnt -= 5;
                            }
                            else
                            {
                                CastSpell(ThrowAttack, ref energy, 0, 2.1, false);
                                NextCast = DateTime.Now.AddSeconds(0.5); // casting time
                                NextGCDSpell = DateTime.Now.AddSeconds(2.0); // 1.5 sec gcd after the casting time
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Resets the combo count and updates the next action time for HungerForBlood and Garrote after the target's death.
            /// </summary>
            public void ResetAfterTargetDeath()
            {
                comboCnt = 0;
                nextActionTime[HungerForBlood] = DateTime.Now;
                nextActionTime[Garrote] = DateTime.Now;
            }

            /// <summary>
            /// Returns true if the current time is greater than the provided nextAction time.
            /// </summary>
            private static bool IsReady(DateTime nextAction)
            {
                return DateTime.Now > nextAction;
            }

            /// <summary>
            /// Casts a spell with the given parameters.
            /// </summary>
            /// <param name="spell">The name of the spell to cast.</param>
            /// <param name="rage">The current rage value. This is updated after casting the spell.</param>
            /// <param name="rageCosts">The amount of rage that the spell costs.</param>
            /// <param name="cooldown">The cooldown time in seconds for the spell. If greater than 0, updates the next action time for the spell.</param>
            /// <param name="gcd">A flag indicating whether the spell is subject to the global cooldown. If true, updates the next global cooldown time.</param>
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
            /// Checks if the player is currently in stealth mode by evaluating their active buffs.
            /// </summary>
            private bool IsInStealth()
            {
                List<string> buffs = Bot.Player.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
                return buffs.Any(e => e.Contains("tealth"));
            }

            /// <summary>
            /// Checks if the given spell is ready to be cast.
            /// </summary>
            /// <param name="spell">The spell to be checked.</param>
            /// <returns>True if the spell is ready to be cast, otherwise false.</returns>
            private bool IsReady(string spell)
            {
                bool result = true; // begin with neutral element of AND
                if (spell.Equals(HungerForBlood) || spell.Equals(SliceAndDice) || spell.Equals(Garrote))
                {
                    // only use these spells in a certain interval
                    result &= !nextActionTime.TryGetValue(spell, out DateTime NextSpellAvailable) || IsReady(NextSpellAvailable);
                }

                result &= Bot.Wow.GetSpellCooldown(spell) <= 0 && Bot.Wow.GetUnitCastingInfo(WowLuaUnit.Player).Item2 <= 0;
                return result;
            }

            ///<summary>
            ///Checks if the target is currently affected by any bleeding debuffs.
            ///</summary>
            private bool IsTargetBleeding()
            {
                List<string> buffs = Bot.Target.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
                return buffs.Any(e => e.Contains("acerate") || e.Contains("Bleed") || e.Contains("bleed") || e.Contains("Rip") || e.Contains("rip")
                 || e.Contains("Rake") || e.Contains("rake") || e.Contains("iercing") || e.Contains("arrote") || e.Contains("emorrhage") || e.Contains("upture") || e.Contains("Wounds") || e.Contains("wounds"));
            }

            /// <summary>
            /// Checks if the target is poisoned by searching for aura names that contain "Poison" or "poison".
            /// </summary>
            private bool IsTargetPoisoned()
            {
                List<string> buffs = Bot.Target.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();
                return buffs.Any(e => e.Contains("Poison") || e.Contains("poison"));
            }
        }
    }
}