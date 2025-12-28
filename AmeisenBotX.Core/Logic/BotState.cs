using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Logic
{
    /// <summary>
    /// Represents the possible high-level states of the bot.
    /// States have priority ordering - higher values = higher priority.
    /// </summary>
    public enum BotStateId
    {
        /// <summary>No active task, fallback/idle behavior.</summary>
        Idle = 0,

        /// <summary>Following party leader or specified character.</summary>
        Following = 10,

        /// <summary>Performing idle actions (emotes, looking around).</summary>
        IdleActions = 15,

        /// <summary>Gathering resources (mining, herbalism).</summary>
        Gathering = 20,

        /// <summary>Looting corpses.</summary>
        Looting = 25,

        /// <summary>Eating/drinking to restore health/mana.</summary>
        Eating = 30,

        /// <summary>Interacting with vendor for repair/sell.</summary>
        RepairingOrSelling = 35,

        /// <summary>Training spells at class trainer.</summary>
        Training = 40,

        /// <summary>Talking to questgiver.</summary>
        QuestInteraction = 45,

        /// <summary>Actively engaged in combat.</summary>
        InCombat = 80,

        /// <summary>Player is dead, awaiting release.</summary>
        Dead = 90,

        /// <summary>Player is a ghost, running to corpse.</summary>
        Ghost = 95
    }

    /// <summary>
    /// Centralized state machine for bot behavior.
    /// Provides deterministic state transitions with hysteresis to prevent oscillation.
    /// </summary>
    public class BotState
    {
        /// <summary>
        /// Current active state of the bot.
        /// </summary>
        public BotStateId CurrentState { get; private set; } = BotStateId.Idle;

        /// <summary>
        /// When the current state was entered.
        /// </summary>
        public DateTime StateEnteredAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// How long we've been in the current state.
        /// </summary>
        public TimeSpan TimeInCurrentState => DateTime.UtcNow - StateEnteredAt;

        /// <summary>
        /// Previous state (for debugging/recovery).
        /// </summary>
        public BotStateId PreviousState { get; private set; } = BotStateId.Idle;

        /// <summary>
        /// Last state transition reason (for debugging).
        /// </summary>
        public string LastTransitionReason { get; private set; } = "Initial";

        /// <summary>
        /// Minimum duration a state must be held before it can be exited (hysteresis).
        /// Prevents rapid oscillation between states.
        /// </summary>
        private static readonly Dictionary<BotStateId, TimeSpan> MinStateDurations = new()
        {
            { BotStateId.Looting, TimeSpan.FromSeconds(1.5) },
            { BotStateId.Eating, TimeSpan.FromSeconds(3.0) },
            { BotStateId.Gathering, TimeSpan.FromSeconds(2.0) },
            { BotStateId.RepairingOrSelling, TimeSpan.FromSeconds(2.0) },
            { BotStateId.Training, TimeSpan.FromSeconds(2.0) },
            { BotStateId.QuestInteraction, TimeSpan.FromSeconds(1.5) },
            { BotStateId.Following, TimeSpan.FromSeconds(0.5) },
        };

        /// <summary>
        /// Maximum duration a state can be held before timing out.
        /// Prevents getting stuck in states forever.
        /// </summary>
        private static readonly Dictionary<BotStateId, TimeSpan> MaxStateDurations = new()
        {
            { BotStateId.Looting, TimeSpan.FromSeconds(30) },
            { BotStateId.Eating, TimeSpan.FromSeconds(45) },
            { BotStateId.Gathering, TimeSpan.FromSeconds(20) },
            { BotStateId.RepairingOrSelling, TimeSpan.FromSeconds(60) },
            { BotStateId.Training, TimeSpan.FromSeconds(30) },
            { BotStateId.QuestInteraction, TimeSpan.FromSeconds(30) },
        };

        /// <summary>
        /// States that can interrupt ANY other state (emergency states).
        /// </summary>
        private static readonly HashSet<BotStateId> EmergencyStates =
        [
            BotStateId.Dead,
            BotStateId.Ghost,
            BotStateId.InCombat
        ];

        /// <summary>
        /// Attempts to transition to a new state. Returns true if transition succeeded.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        /// <param name="reason">Reason for the transition (for logging).</param>
        /// <param name="force">If true, ignore hysteresis and force transition.</param>
        /// <returns>True if transition occurred, false if blocked by hysteresis.</returns>
        public bool TryTransition(BotStateId newState, string reason = null, bool force = false)
        {
            // Same state = no-op, always "succeeds"
            if (newState == CurrentState)
            {
                return true;
            }

            // Emergency states always win
            if (EmergencyStates.Contains(newState))
            {
                ExecuteTransition(newState, reason ?? "Emergency state triggered");
                return true;
            }

            // Check if we're in an emergency state that should block lower-priority transitions
            if (EmergencyStates.Contains(CurrentState) && !force)
            {
                // Only allow transition out of emergency states via specific rules
                // Dead -> Ghost is allowed
                // Ghost -> Idle is allowed (after resurrection)
                // InCombat -> anything higher or equal priority is allowed, or lower when combat ends

                if (CurrentState == BotStateId.Dead && newState != BotStateId.Ghost)
                {
                    return false;
                }
                if (CurrentState == BotStateId.Ghost && newState != BotStateId.Idle && (int)newState < (int)BotStateId.Ghost)
                {
                    return false;
                }
                // InCombat can transition to anything if combat actually ended
            }

            // Check hysteresis - are we allowed to leave current state yet?
            if (!force && MinStateDurations.TryGetValue(CurrentState, out TimeSpan minDuration))
            {
                if (TimeInCurrentState < minDuration)
                {
                    // Still in hysteresis window, block transition
                    AmeisenLogger.I.Log("BotState",
                        $"Transition to {newState} blocked by hysteresis ({TimeInCurrentState.TotalSeconds:F1}s < {minDuration.TotalSeconds:F1}s)",
                        LogLevel.Verbose);
                    return false;
                }
            }

            // Check if incoming state is higher priority (can interrupt)
            // Lower priority states can only take over after completing current action
            if (!force && (int)newState < (int)CurrentState)
            {
                // Lower priority state trying to take over - only allow if current state is Idle
                if (CurrentState is not BotStateId.Idle and not BotStateId.Following and not BotStateId.IdleActions)
                {
                    return false;
                }
            }

            ExecuteTransition(newState, reason ?? $"Transition from {CurrentState}");
            return true;
        }

        /// <summary>
        /// Check if current state has exceeded its maximum duration (timeout).
        /// </summary>
        public bool IsCurrentStateTimedOut()
        {
            return MaxStateDurations.TryGetValue(CurrentState, out TimeSpan maxDuration) && TimeInCurrentState > maxDuration;
        }

        /// <summary>
        /// Forces exit from current state back to Idle. Used for timeout/error recovery.
        /// </summary>
        public void ForceReset(string reason = "Forced reset")
        {
            ExecuteTransition(BotStateId.Idle, reason);
        }

        /// <summary>
        /// Checks if we can potentially transition to the given state (without actually doing it).
        /// Useful for decision-making logic.
        /// </summary>
        public bool CanTransitionTo(BotStateId targetState)
        {
            if (targetState == CurrentState)
            {
                return true;
            }

            if (EmergencyStates.Contains(targetState))
            {
                return true;
            }

            if (EmergencyStates.Contains(CurrentState))
            {
                // Can only leave emergency states under specific conditions
                if (CurrentState == BotStateId.InCombat)
                {
                    return true; // Combat can end anytime
                }

                return (CurrentState == BotStateId.Dead && targetState == BotStateId.Ghost) || (CurrentState == BotStateId.Ghost && targetState == BotStateId.Idle);
            }

            // Check hysteresis
            if (MinStateDurations.TryGetValue(CurrentState, out TimeSpan minDuration))
            {
                if (TimeInCurrentState < minDuration)
                {
                    return false;
                }
            }

            // Check priority
            if ((int)targetState < (int)CurrentState)
            {
                if (CurrentState is not BotStateId.Idle and not BotStateId.Following and not BotStateId.IdleActions)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the bot is in a state where it should not be interrupted for low-priority tasks.
        /// </summary>
        public bool IsBusy()
        {
            return CurrentState switch
            {
                BotStateId.InCombat => true,
                BotStateId.Dead => true,
                BotStateId.Ghost => true,
                BotStateId.Eating => true, // Don't interrupt eating
                BotStateId.Looting => true, // Don't interrupt looting
                BotStateId.Gathering => true, // Don't interrupt gathering
                _ => false
            };
        }

        /// <summary>
        /// Returns true if the bot is in combat-related states.
        /// </summary>
        public bool IsInCombatState()
        {
            return CurrentState == BotStateId.InCombat;
        }

        /// <summary>
        /// Returns true if the bot is dead or a ghost.
        /// </summary>
        public bool IsDeadOrGhost()
        {
            return CurrentState is BotStateId.Dead or BotStateId.Ghost;
        }

        private void ExecuteTransition(BotStateId newState, string reason)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
            StateEnteredAt = DateTime.UtcNow;
            LastTransitionReason = reason;

            AmeisenLogger.I.Log("BotState",
                $"State: {PreviousState} -> {newState} ({reason})",
                LogLevel.Debug);
        }

        /// <summary>
        /// Check for timeout and auto-reset if needed. Call this in main update loop.
        /// </summary>
        public void Update()
        {
            if (IsCurrentStateTimedOut())
            {
                AmeisenLogger.I.Log("BotState",
                    $"State {CurrentState} timed out after {TimeInCurrentState.TotalSeconds:F1}s",
                    LogLevel.Warning);
                ForceReset($"Timeout in {CurrentState}");
            }
        }

        public override string ToString()
        {
            return $"{CurrentState} ({TimeInCurrentState.TotalSeconds:F1}s)";
        }
    }
}
