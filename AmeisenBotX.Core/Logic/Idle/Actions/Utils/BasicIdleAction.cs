using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains utility classes for idle actions in the AmeisenBotX.Core.Logic.Idle.Actions namespace.
/// </summary>
namespace AmeisenBotX.Core.Logic.Idle.Actions.Utils
{
    /// <summary>
    /// Represents a basic idle action that can be performed.
    /// </summary>
    public abstract class BasicIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicIdleAction"/> class.
        /// </summary>
        /// <param name="actions">The list of idle actions.</param>
        /// <param name="name">The optional name of the basic idle action.</param>
        public BasicIdleAction(List<IIdleAction> actions, string name = "")
        {
            Name = name;
            Actions = actions;
            SelectedAction = Actions.ElementAt(new Random().Next(0, Actions.Count));
        }

        /// <summary>
        /// Gets a value indicating whether the AutopilotOnly property is true.
        /// </summary>
        public bool AutopilotOnly => Actions.Any(e => e.AutopilotOnly);

        /// <summary>
        /// Gets or sets the cooldown DateTime value.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown of the selected action.
        /// </summary>
        public int MaxCooldown => SelectedAction.MaxCooldown;

        /// <summary>
        /// Gets the maximum duration of the selected action.
        /// </summary>
        public int MaxDuration => SelectedAction.MaxDuration;

        /// <summary>
        /// Gets the minimum cooldown of the selected action.
        /// </summary>
        public int MinCooldown => SelectedAction.MinCooldown;

        /// <summary>
        /// Gets the minimum duration of the selected action.
        /// </summary>
        public int MinDuration => SelectedAction.MinDuration;

        /// <summary>
        /// Gets the value of the Name property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the list of idle actions.
        /// </summary>
        protected List<IIdleAction> Actions { get; }

        /// <summary>
        /// Gets or sets the currently selected idle action.
        /// </summary>
        protected IIdleAction SelectedAction { get; set; }

        /// <summary>
        /// Method to enter a certain action.
        /// </summary>
        /// <returns>
        /// Returns a boolean value indicating if the action was successfully entered.
        /// </returns>
        public abstract bool Enter();

        /// <summary>
        /// Executes the selected action.
        /// </summary>
        public virtual void Execute()
        {
            if (SelectedAction != null)
            {
                SelectedAction.Execute();
            }
        }

        /// <summary>
        /// Overrides the base ToString() method to return a string representation of the object's name with an optional autopilot indicator.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}{Name}";
        }
    }
}