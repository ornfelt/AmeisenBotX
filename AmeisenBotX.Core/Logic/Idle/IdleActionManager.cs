using AmeisenBotX.Core.Logic.Idle.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle
{
    /// <summary>
    /// Initializes a new instance of the IdleActionManager class with the specified AmeisenBotConfig and idleActions.
    /// </summary>
    public class IdleActionManager
    {
        /// <summary>
        /// Initializes a new instance of the IdleActionManager class with the specified AmeisenBotConfig and idleActions.
        /// </summary>
        /// <param name="config">The AmeisenBotConfig object that contains the configuration settings for the AmeisenBot.</param>
        /// <param name="idleActions">An IEnumerable collection of IIdleAction objects representing the idle actions available.</param>
        public IdleActionManager(AmeisenBotConfig config, IEnumerable<IIdleAction> idleActions)
        {
            Config = config;
            IdleActions = idleActions;

            Rnd = new();
            LastActions = new();
        }

        /// <summary>
        /// Gets or sets the cooldown duration for an action.
        /// </summary>
        public TimeSpan Cooldown { get; private set; }

        /// <summary>
        /// Gets or sets the DateTime value representing the execution time until which the code will continue to execute.
        /// </summary>
        public DateTime ExecuteUntil { get; private set; }

        /// <summary>
        /// Gets or sets the collection of idle actions.
        /// </summary>
        /// <returns>An IEnumerable of type IIdleAction.</returns>
        public IEnumerable<IIdleAction> IdleActions { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last action executed.
        /// </summary>
        public DateTime LastActionExecuted { get; private set; }

        ///<summary> 
        /// Gets or sets the list of the last actions performed, represented as a collection of key-value pairs.
        /// The key represents the date and time of the action, and the value represents the specific action performed.
        ///</summary>
        public List<KeyValuePair<DateTime, IIdleAction>> LastActions { get; private set; }

        /// <summary>
        /// Gets or sets the configuration settings for the AmeisenBot.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets the current idle action.
        /// </summary>
        private IIdleAction CurrentAction { get; set; }

        /// <summary>
        /// Gets the maximum action cooldown.
        /// </summary>
        private int MaxActionCooldown { get; } = 28 * 1000;

        /// <summary>
        /// Gets the minimum action cooldown in milliseconds.
        /// </summary>
        private int MinActionCooldown { get; } = 12 * 1000;

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Resets the ExecuteUntil to its default value and sets the LastActionExecuted to the current UTC time.
        /// </summary>
        public void Reset()
        {
            ExecuteUntil = default;
            LastActionExecuted = DateTime.UtcNow;
        }

        /// <summary>
        /// Executes a tick of the game loop.
        /// </summary>
        /// <param name="autopilotEnabled">A flag indicating whether autopilot is enabled.</param>
        /// <returns>True if the tick was successfully executed, false otherwise.</returns>
        public bool Tick(bool autopilotEnabled)
        {
            if (ExecuteUntil > DateTime.UtcNow && CurrentAction != null)
            {
                CurrentAction.Execute();
                return true;
            }

            // cleanup old events
            LastActions.RemoveAll(e => e.Key < e.Value.Cooldown);

            if (LastActionExecuted + Cooldown <= DateTime.UtcNow)
            {
                IEnumerable<IIdleAction> filteredActions = IdleActions.Where
                (
                    e => Config.IdleActionsEnabled.TryGetValue(e.ToString(), out bool b) && b
                      && (!e.AutopilotOnly || autopilotEnabled)
                      && DateTime.UtcNow > e.Cooldown
                );

                if (filteredActions.Any())
                {
                    CurrentAction = filteredActions.ElementAtOrDefault(Rnd.Next(0, filteredActions.Count()));

                    if (CurrentAction != null && CurrentAction.Enter())
                    {
                        LastActionExecuted = DateTime.UtcNow;

                        Cooldown = TimeSpan.FromMilliseconds(Rnd.Next(MinActionCooldown, MaxActionCooldown));
                        CurrentAction.Cooldown = DateTime.UtcNow + TimeSpan.FromMilliseconds(Rnd.Next(CurrentAction.MinCooldown, CurrentAction.MaxCooldown));
                        ExecuteUntil = LastActionExecuted + TimeSpan.FromMilliseconds(Rnd.Next(CurrentAction.MinDuration, CurrentAction.MaxDuration));

                        LastActions.Add(new(LastActionExecuted, CurrentAction));

                        CurrentAction.Execute();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}