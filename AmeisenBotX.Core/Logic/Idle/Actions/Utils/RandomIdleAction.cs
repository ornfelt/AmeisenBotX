using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions.Utils
{
    /// <summary>
    /// Initializes a new instance of the RandomIdleAction class with the specified name and list of idle actions.
    /// </summary>
    /// <param name="name">The name of the random idle action.</param>
    /// <param name="actions">The list of idle actions.</param>
    public class RandomIdleAction : BasicIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the RandomIdleAction class with the specified list of idle actions.
        /// </summary>
        public RandomIdleAction(List<IIdleAction> actions)
                    : base(actions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RandomIdleAction class with the specified name and list of idle actions.
        /// </summary>
        /// <param name="name">The name of the random idle action.</param>
        /// <param name="actions">The list of idle actions.</param>
        public RandomIdleAction(string name, List<IIdleAction> actions)
                    : base(actions, name)
        {
        }

        /// <summary>
        /// Gets or sets the instance of the Random class used for generating random numbers.
        /// </summary>
        private Random Rnd { get; } = new();

        /// <summary>
        /// Enters the current state, selects and sets the active action based on the available idle actions.
        /// Returns true if an action is selected and set, otherwise returns false.
        /// </summary>
        public override bool Enter()
        {
            IEnumerable<IIdleAction> possibleActions = Actions.Where(e => e.Enter());
            int actionCount = possibleActions.Count();

            if (actionCount > 1)
            {
                SelectedAction = possibleActions.ElementAt(Rnd.Next(0, actionCount));
                return true;
            }
            else if (actionCount == 1)
            {
                SelectedAction = possibleActions.FirstOrDefault();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// If AutopilotOnly is true, adds a 🤖 emoji to the string.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}{Name}";
        }
    }
}