using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Dps;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    public class DeathknightBlood : ICombatClass
    {
        /// <summary>
        /// Initializes a new instance of the DeathknightBlood class with the provided bot.
        /// Creates a TargetManager with a SimpleDpsTargetSelectionLogic for the bot and a TimeSpan of 250 milliseconds.
        /// </summary>
        public DeathknightBlood(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            TargetProvider = new TargetManager(new SimpleDpsTargetSelectionLogic(bot), TimeSpan.FromMilliseconds(250));//Heal/Tank/DPS
        }

        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author => "Kamel";

        /// <summary>
        /// Gets or sets the collection of blacklisted target display IDs.
        /// </summary>
        /// <returns>An IEnumerable of integers representing blacklisted target display IDs.</returns>
        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of configureables.
        /// </summary>
        /// <value>
        /// The dictionary containing strings as keys and dynamic values.
        /// </value>
        public Dictionary<string, dynamic> Configureables { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Blood Deathknight spec.
        /// </summary>
        public string Description => "FCFS based CombatClass for the Blood Deathknight spec.";

        /// <summary>
        /// Gets the display name of the Blood Deathknight which is currently a work-in-progress.
        /// </summary>
        public string DisplayName => "[WIP] Blood Deathknight";

        /// <summary>
        /// Gets a value indicating whether this code handles facing.
        /// </summary>
        public bool HandlesFacing => false;

        /// <summary>
        /// Gets a value indicating whether this code handles movement.
        /// </summary>
        /// <returns>False.</returns>
        public bool HandlesMovement => false;

        /// <summary>
        /// Returns whether the character uses melee attacks.
        /// </summary>
        public bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for comparing two items.
        /// </summary>
        public IItemComparator ItemComparator => null;

        /// <summary>
        /// Gets or sets the display IDs of the priority targets.
        /// </summary>
        /// <returns>An enumerable collection of integer values representing the display IDs.</returns>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets the role of the character as DPS (Damage Per Second).
        /// </summary>
        public WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree for the talent system.
        /// </summary>
        public TalentTree Talents { get; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the target is in the line of sight.
        /// </summary>
        public bool TargetInLineOfSight { get; set; }

        /// <summary>
        /// Gets or sets the target provider for the code.
        /// </summary>
        public ITargetProvider TargetProvider { get; internal set; }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>The version number as a string.</returns>
        public string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind the enemy.
        /// </summary>
        public bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets the WowClass property which represents the WowClass of type Deathknight.
        /// </summary>
        public WowClass WowClass => WowClass.Deathknight;

        /// <summary>
        /// Gets or sets the Bot object that implements the AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Attacks the current target.
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
        /// Executes the attack sequence.
        /// </summary>
        public void Execute()
        {
            ulong targetGuid = Bot.Wow.TargetGuid;
            IWowUnit target = Bot.Objects.All.OfType<IWowUnit>().FirstOrDefault(t => t.Guid == targetGuid);
            if (target != null)
            {
                // make sure we're auto attacking
                if (!Bot.Objects.Player.IsAutoAttacking)
                {
                    Bot.Wow.StartAutoAttack();
                }

                HandleAttacking(target);
            }
        }

        /// <summary>
        /// Loads the specified dictionary of objects and assigns the value associated with the key "Configureables" to the Configureables property, converting it to a dynamic type.
        /// </summary>
        public void Load(Dictionary<string, JsonElement> objects)
        {
            Configureables = objects["Configureables"].ToDyn();
        }

        /// <summary>
        /// This method is used to execute code when the system is out of combat.
        /// </summary>
        public void OutOfCombatExecute()
        {
        }

        /// <summary>
        /// Saves the configureables into a dictionary and returns it.
        /// </summary>
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
            if (TargetProvider.Get(out IEnumerable<IWowUnit> targetToTarget))
            {
                ulong guid = targetToTarget.First().Guid;

                if (Bot.Objects.Player.TargetGuid != guid)
                {
                    Bot.Wow.ChangeTarget(guid);
                }
            }

            if (Bot.Objects.Target == null
                || Bot.Objects.Target.IsDead
                || !IWowUnit.IsValid(Bot.Objects.Target))
            {
                return;
            }

            double playerRunePower = Bot.Objects.Player.RunicPower;
            double distanceToTarget = Bot.Objects.Player.Position.GetDistance(target.Position);
            double targetHealthPercent = (target.Health / (double)target.MaxHealth) * 100;
            double playerHealthPercent = (Bot.Objects.Player.Health / (double)Bot.Objects.Player.MaxHealth) * 100.0;
            (string, int) targetCastingInfo = Bot.Wow.GetUnitCastingInfo(WowLuaUnit.Target);
            //List<string> myBuffs = Bot.NewBot.GetBuffs(WowLuaUnit.Player.ToString());
            //myBuffs.Any(e => e.Equals("Chains of Ice"))

            if (Bot.Wow.GetSpellCooldown("Death Grip") <= 0 && distanceToTarget <= 30)
            {
                Bot.Wow.CastSpell("Death Grip");
                return;
            }
            if (target.IsFleeing && distanceToTarget <= 30)
            {
                Bot.Wow.CastSpell("Chains of Ice");
                return;
            }

            if (Bot.Wow.GetSpellCooldown("Army of the Dead") <= 0 &&
                IsOneOfAllRunesReady())
            {
                Bot.Wow.CastSpell("Army of the Dead");
                return;
            }

            List<IWowUnit> unitsNearPlayer = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(e => e.Position.GetDistance(Bot.Objects.Player.Position) <= 10)
                .ToList();

            if (unitsNearPlayer.Count > 2 &&
                Bot.Wow.GetSpellCooldown("Blood Boil") <= 0 &&
                Bot.Wow.IsRuneReady(0) ||
                Bot.Wow.IsRuneReady(1))
            {
                Bot.Wow.CastSpell("Blood Boil");
                return;
            }

            List<IWowUnit> unitsNearTarget = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(e => e.Position.GetDistance(target.Position) <= 30)
                .ToList();

            if (unitsNearTarget.Count > 2 &&
                Bot.Wow.GetSpellCooldown("Death and Decay") <= 0 &&
                IsOneOfAllRunesReady())
            {
                Bot.Wow.CastSpell("Death and Decay");
                Bot.Wow.ClickOnTerrain(target.Position);
                return;
            }

            if (Bot.Wow.GetSpellCooldown("Icy Touch") <= 0 &&
                Bot.Wow.IsRuneReady(2) ||
                Bot.Wow.IsRuneReady(3))
            {
                Bot.Wow.CastSpell("Icy Touch");
                return;
            }
        }

        /// <summary>
        /// Checks if at least one of all the runes is ready.
        /// </summary>
        private bool IsOneOfAllRunesReady()
        {
            return Bot.Wow.IsRuneReady(0)
                       || Bot.Wow.IsRuneReady(1)
                       && Bot.Wow.IsRuneReady(2)
                       || Bot.Wow.IsRuneReady(3)
                       && Bot.Wow.IsRuneReady(4)
                       || Bot.Wow.IsRuneReady(5);
        }
    }
}