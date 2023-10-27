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
    /// Represents a Paladin Protection class that implements the ICombatClass interface.
    /// </summary>
    public class PaladinProtection : ICombatClass
    {
        /// <summary>
        /// The readonly field for the AmeisenBotInterfaces class represents a instance of the AmeisenBotInterfaces class, and it cannot be modified once initialized.
        /// </summary>
        private readonly AmeisenBotInterfaces Bot;
        /// <summary>
        /// The array of running emotes.
        /// </summary>
        private readonly string[] runningEmotes = { "/question", "/talk" };
        /// <summary>
        /// Array containing a list of standing emotes.
        /// </summary>
        private readonly string[] standingEmotes = { "/bow" };
        /// <summary>
        /// Determines whether a new route needs to be computed. 
        /// </summary>
        private bool computeNewRoute = false;
        /// <summary>
        /// Represents the distance from the current object to the target. 
        /// </summary>
        private double distanceToTarget = 0;
        /// <summary>
        /// Represents a flag indicating whether there are multiple targets.
        /// </summary>
        private bool multipleTargets = false;
        /// <summary>
        /// Represents the standing status.
        /// </summary>
        private bool standing = false;

        /// <summary>
        /// Initializes a new instance of the PaladinProtection class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used to configure and control the bot.</param>
        public PaladinProtection(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets the author of the code, which is "einTyp".
        /// </summary>
        public string Author => "einTyp";

        /// <summary>
        /// Gets or sets a collection of blacklisted target display IDs.
        /// </summary>
        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of configurable values, where the keys are strings and the values can be of any type.
        /// </summary>
        /// <value>A dictionary containing configurable values.</value>
        public Dictionary<string, dynamic> Configureables { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description => "...";

        /// <summary>
        /// Gets the display name for the Protection Paladin.
        /// </summary>
        public string DisplayName => "Protection Paladin";

        /// <summary>
        /// Gets a value indicating whether this code handles facing, which is always false.
        /// </summary>
        public bool HandlesFacing => false;

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        public bool HandlesMovement => true;

        /// <summary>
        /// Determines if the character's attack is melee.
        /// </summary>
        public bool IsMelee => true;

        /// <summary>
        /// Gets the item comparator for tanks.
        /// </summary>
        public IItemComparator ItemComparator => new TankItemComparator();

        /// <summary>
        /// Gets or sets the display IDs of the priority targets.
        /// </summary>
        /// <returns>An enumerable of integers representing the display IDs of the priority targets.</returns>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets the role of the Wow character, which is set to Tank.
        /// </summary>
        public WowRole Role => WowRole.Tank;

        /// <summary>
        /// Represents the talent tree configuration.
        /// The TalentTree property contains three trees.
        /// 
        /// Tree1: Contains specific talent combinations based on key values.
        /// Example: Key 2 corresponds to Talent(1, 2, 5).
        /// 
        /// Tree2: Contains a larger set of talent combinations based on key values.
        /// Example: Key 1 corresponds to Talent(2, 1, 5).
        /// 
        /// Tree3: Contains a limited set of talent combinations based on key values.
        /// Example: Key 1 corresponds to Talent(3, 1, 5).
        /// </summary>
        public TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 4, new(1, 4, 5) },
                { 5, new(1, 5, 2) }
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 5) },
                { 2, new(2, 2, 5) },
                { 3, new(2, 3, 3) },
                { 4, new(2, 4, 2) },
                { 5, new(2, 5, 5) },
                { 6, new(2, 6, 1) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 5) },
                { 9, new(2, 9, 2) },
                { 12, new(2, 12, 1) },
                { 14, new(2, 14, 2) },
                { 16, new(2, 16, 2) },
                { 17, new(2, 17, 1) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 3) },
                { 22, new(2, 22, 1) },
                { 23, new(2, 23, 2) },
                { 24, new(2, 24, 3) }
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 2, new(3, 2, 5) }
            }
        };

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public string Version => "1.0";

        /// <summary>
        /// Determines whether the ability to walk behind an enemy is false.
        /// </summary>
        public bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass for the Paladin.
        /// </summary>
        public WowClass WowClass => WowClass.Paladin;

        /// <summary>
        /// Gets or sets a value indicating whether the person is currently dancing.
        /// </summary>
        private bool Dancing { get; set; }

        /// <summary>
        /// Gets or sets the GCD (Greatest Common Divisor) time.
        /// </summary>
        private double GCDTime { get; set; }

        /// <summary>
        /// Gets or sets the last date and time an Avenger was recorded.
        /// </summary>
        private DateTime LastAvenger { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last consecration.
        /// </summary>
        private DateTime LastConsecration { get; set; }

        /// <summary>
        /// Gets or sets the time when the Divine Shield was last activated or deactivated.
        /// </summary>
        private DateTime LastDivineShield { get; set; }

        /// <summary>
        /// Gets or sets the last date and time the GCD (Greatest Common Divisor) was calculated.
        /// </summary>
        private DateTime LastGCD { get; set; }

        /// <summary>
        /// Gets or sets the last time a hammer was used.
        /// </summary>
        private DateTime LastHammer { get; set; }

        /// <summary>
        /// Gets or sets the last time the Holy Shield was activated.
        /// </summary>
        private DateTime LastHolyShield { get; set; }

        /// <summary>
        /// Gets or sets the last position of the player.
        /// </summary>
        private Vector3 LastPlayerPosition { get; set; }

        /// <summary>
        /// Gets or sets the last protection date and time.
        /// </summary>
        private DateTime LastProtection { get; set; }

        /// <summary>
        /// Gets or sets the last sacrifice date and time.
        /// </summary>
        private DateTime LastSacrifice { get; set; }

        /// <summary>
        /// Gets or sets the last known target position in the world space.
        /// </summary>
        private Vector3 LastTargetPosition { get; set; }

        /// <summary>
        /// Gets or sets the last recorded date and time of wisdom.
        /// </summary>
        private DateTime LastWisdom { get; set; }

        /// <summary>
        /// Attacks the current target. If there is no target, does nothing. If the target is within 3.0 distance, stops click-to-move, resets movement, and interacts with the target. If the target is further away, sets movement action to move towards the target's position.
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
        /// Executes the action of the bot. Sets the computeNewRoute flag to false. Retrieves the target unit from the bot. If the target is valid (not null, alive, and has health above 1) or 
        /// a new target is found by searching, it proceeds with the following actions. 
        /// Checks if the player's position has changed since the last execution. If it has, updates the LastPlayerPosition and sets the targetDistanceChanged flag to true. 
        /// Checks if the target's position has changed since the last execution. If it has, updates the LastTargetPosition, sets the computeNewRoute flag to true, and sets the targetDistanceChanged flag to true. 
        /// If the targetDistanceChanged flag is true, updates the distanceToTarget based on the last player and target positions. 
        /// Handles the movement of the bot towards the target. 
        /// Handles the attacking action towards the target. 
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
        /// Loads the given dictionary of objects and assigns the value of the "Configureables" key to the Configureables property.
        /// </summary>
        public void Load(Dictionary<string, JsonElement> objects)
        {
            Configureables = objects["Configureables"].ToDyn();
        }

        /// <summary>
        /// Executes the out of combat behavior for the bot.
        /// </summary>
        public void OutOfCombatExecute()
        {
            double distanceTraveled = Bot.Player.Position.GetDistance(LastPlayerPosition);
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
        /// Saves the configureable items from the dictionary and returns a new dictionary containing the configureable items.
        /// </summary>
        /// <returns>A new dictionary containing the configureable items.</returns>
        public Dictionary<string, object> Save()
        {
            return new()
            {
                { "configureables", Configureables }
            };
        }

        /// <summary>
        /// Handles the attacking behavior of the character.
        /// </summary>
        /// <param name="target">The target to attack.</param>
        private void HandleAttacking(IWowUnit target)
        {
            bool gcdWaiting = IsGCD();
            Bot.Wow.ChangeTarget(target.Guid);
            bool targetAimed = true;
            double playerMana = Bot.Player.Mana;
            double targetHealthPercent = target.HealthPercentage;
            double playerHealthPercent = Bot.Player.HealthPercentage;
            List<string> buffs = Bot.Player.Auras.Select(e => Bot.Db.GetSpellName(e.SpellId)).ToList();

            // buffs
            if (!buffs.Any(e => e.Contains("evotion")))
            {
                Bot.Wow.CastSpell("Devotion Aura");
            }

            if (!gcdWaiting && !buffs.Any(e => e.Contains("ury")))
            {
                Bot.Wow.CastSpell("Righteous Fury");
                SetGCD(1.5);
                return;
            }

            if (!buffs.Any(e => e.Contains("ighteousness")))
            {
                Bot.Wow.CastSpell("Seal of Righteousness");
            }

            if (!gcdWaiting && playerHealthPercent > 50 && DateTime.Now.Subtract(LastSacrifice).TotalSeconds > 120)
            {
                Bot.Wow.CastSpell("Divine Sacrifice");
                LastSacrifice = DateTime.Now;
                SetGCD(1.5);
                return;
            }

            // distance attack
            if (!gcdWaiting && distanceToTarget > (10 + target.CombatReach) && distanceToTarget < (30 + target.CombatReach))
            {
                if (DateTime.Now.Subtract(LastAvenger).TotalSeconds > 30 && playerMana >= 1027)
                {
                    Bot.Wow.CastSpell("Avenger's Shield");
                    LastAvenger = DateTime.Now;
                    Bot.Wow.SendChatMessage("/s and i'm like.. bam!");
                    playerMana -= 1027;
                    SetGCD(1.5);
                    return;
                }
            }
            else
            {
                // close combat
                if (!gcdWaiting && distanceToTarget <= 0.75f * (Bot.Player.CombatReach + target.CombatReach))
                {
                    if (multipleTargets && DateTime.Now.Subtract(LastConsecration).TotalSeconds > 8 && playerMana >= 869)
                    {
                        Bot.Wow.CastSpell("Consecration");
                        LastConsecration = DateTime.Now;
                        Bot.Wow.SendChatMessage("/s MOVE BITCH!!!!!11");
                        playerMana -= 869;
                        SetGCD(1.5);
                        return;
                    }

                    if (DateTime.Now.Subtract(LastHammer).TotalSeconds > 60 && playerMana >= 117)
                    {
                        Bot.Wow.CastSpell("Hammer of Justice");
                        LastHammer = DateTime.Now;
                        Bot.Wow.SendChatMessage("/s STOP! hammertime!");
                        playerMana -= 117;
                        SetGCD(1.5);
                        return;
                    }
                }
            }

            // support members
            int lowHealth = 2147483647;
            IWowUnit lowMember = null;
            foreach (ulong memberGuid in Bot.Objects.PartymemberGuids)
            {
                IWowUnit member = Bot.GetWowObjectByGuid<IWowUnit>(memberGuid);
                if (member != null && member.Health < lowHealth)
                {
                    lowHealth = member.Health;
                    lowMember = member;
                }
            }

            if (lowMember != null)
            {
                if (!gcdWaiting && (lowMember.IsDazed || lowMember.IsConfused || lowMember.IsFleeing || lowMember.IsSilenced))
                {
                    if (playerMana >= 276)
                    {
                        Bot.Wow.ChangeTarget(lowMember.Guid);
                        targetAimed = false;
                        Bot.Wow.CastSpell("Blessing of Sanctuary");
                        playerMana -= 276;
                        SetGCD(1.5);
                        return;
                    }

                    if (playerMana >= 236)
                    {
                        Bot.Wow.ChangeTarget(lowMember.Guid);
                        targetAimed = false;
                        Bot.Wow.CastSpell("Hand of Freedom");
                        playerMana -= 236;
                        SetGCD(1.5);
                        return;
                    }
                }

                if (lowMember.HealthPercentage > 1)
                {
                    if (!gcdWaiting && DateTime.Now.Subtract(LastDivineShield).TotalSeconds > 240 && lowMember.HealthPercentage < 20 && playerMana >= 117)
                    {
                        Bot.Wow.ChangeTarget(lowMember.Guid);
                        targetAimed = false;
                        Bot.Wow.CastSpell("Divine Shield");
                        LastDivineShield = DateTime.Now;
                        playerMana -= 117;
                        SetGCD(1.5);
                        return;
                    }
                    else if (lowMember.HealthPercentage < 50 && DateTime.Now.Subtract(LastProtection).TotalSeconds > 120 && playerMana >= 117)
                    {
                        Bot.Wow.ChangeTarget(lowMember.Guid);
                        targetAimed = false;
                        Bot.Wow.CastSpell("Divine Protection");
                        LastProtection = DateTime.Now;
                        playerMana -= 117;
                    }
                }
            }

            // self-casts
            if (!gcdWaiting && DateTime.Now.Subtract(LastHolyShield).TotalSeconds > 8 && playerMana >= 395)
            {
                Bot.Wow.ClearTarget();
                targetAimed = false;
                Bot.Wow.CastSpell("Holy Shield");
                LastHolyShield = DateTime.Now;
                playerMana -= 395;
                SetGCD(1.5);
                return;
            }

            if (!gcdWaiting && DateTime.Now.Subtract(LastWisdom).TotalSeconds > 600 && playerMana >= 197)
            {
                Bot.Wow.ClearTarget();
                targetAimed = false;
                Bot.Wow.CastSpell("Blessing of Wisdom");
                LastWisdom = DateTime.Now;
                playerMana -= 197;
                SetGCD(1.5);
                return;
            }

            // back to attack
            if (!targetAimed)
            {
                Bot.Wow.ChangeTarget(target.Guid);
            }

            if (!Bot.Player.IsAutoAttacking)
            {
                Bot.Wow.StartAutoAttack();
            }
        }

        /// <summary>
        /// Handles the movement of the character towards a specified target.
        /// If the target is null, the method returns without performing any action.
        /// If the bot's movement status is not 'None' and the character is within a certain distance from the target,
        /// it stops the character's movement.
        /// If the computeNewRoute flag is true and the character is not facing the last player position,
        /// it rotates the character to face the target position.
        /// Sets the movement action of the bot to 'Move' with the specified target position and rotation.
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

        /// <summary>
        /// Checks if the current time subtracted from the value of LastGCD is less than the specified GCDTime.
        /// Returns true if it is, false otherwise.
        /// </summary>
        private bool IsGCD()
        {
            return DateTime.Now.Subtract(LastGCD).TotalSeconds < GCDTime;
        }

        ///<summary>
        ///Searches for a new target to attack.
        ///</summary>
        ///<param name="target">The current target being attacked.</param>
        ///<param name="grinding">A boolean value indicating if the player is grinding.</param>
        ///<returns>Returns true if a new target is found, otherwise returns false.</returns>
        private bool SearchNewTarget(ref IWowUnit target, bool grinding)
        {
            if (Bot.Wow.TargetGuid != 0 && target != null && !(target.IsDead || target.Health < 1 || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem"))))
            {
                return false;
            }

            List<IWowUnit> wowUnits = Bot.Objects.All.OfType<IWowUnit>().Where(e => Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Neutral).ToList();
            bool newTargetFound = false;
            int areaToLookAt = grinding ? 100 : 50;
            bool inCombat = target != null && !target.IsDead && target.Health >= 1 && target.IsInCombat;
            int targetHealth = (target == null || target.IsDead || target.Health < 1) ? 2147483647 : target.Health;
            ulong memberGuid = (target == null || target.IsDead || target.Health < 1) ? 0 : target.TargetGuid;
            IWowUnit member = (target == null || target.IsDead || target.Health < 1) ? null : Bot.GetWowObjectByGuid<IWowUnit>(memberGuid);
            int memberHealth = member == null ? 2147483647 : member.Health;
            int targetCount = 0;
            multipleTargets = false;
            foreach (IWowUnit unit in wowUnits)
            {
                if (IWowUnit.IsValid(unit) && unit != target && !(unit.IsDead || unit.Health < 1 || unit.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId).Contains("Spirit of Redem"))))
                {
                    double tmpDistance = Bot.Player.Position.GetDistance(unit.Position);
                    if (tmpDistance < areaToLookAt)
                    {
                        int compHealth = 2147483647;
                        if (tmpDistance < 6.0)
                        {
                            targetCount++;
                        }

                        if (unit.IsInCombat)
                        {
                            member = Bot.GetWowObjectByGuid<IWowUnit>(unit.TargetGuid);
                            if (member != null)
                            {
                                compHealth = member.Health;
                            }
                        }

                        if (((unit.IsInCombat && (compHealth < memberHealth || (compHealth == memberHealth && targetHealth < unit.Health))) || (!inCombat && grinding && (target == null || target.IsDead) && unit.Health < targetHealth)) && Bot.Wow.IsInLineOfSight(Bot.Player.Position, unit.Position))
                        {
                            target = unit;
                            newTargetFound = true;
                            inCombat = unit.IsInCombat;
                            memberHealth = compHealth;
                            targetHealth = unit.Health;
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
            else if (newTargetFound)
            {
                Bot.Wow.ChangeTarget(target.Guid);
            }

            if (targetCount > 1)
            {
                multipleTargets = true;
            }

            return newTargetFound;
        }

        /// <summary>
        /// Sets the value of GCDTime to the given gcdInSec and updates LastGCD with the current date and time.
        /// </summary>
        private void SetGCD(double gcdInSec)
        {
            GCDTime = gcdInSec;
            LastGCD = DateTime.Now;
        }
    }
}