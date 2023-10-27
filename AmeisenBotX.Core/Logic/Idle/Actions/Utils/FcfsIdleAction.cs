using System.Collections.Generic;

namespace AmeisenBotX.Core.Logic.Idle.Actions.Utils
{
    /// <summary>
    /// Initializes a new instance of the FcfsIdleAction class with the specified
    /// name and list of idle actions, and initializes the base class with the actions
    /// and name.
    /// </summary>
    public class FcfsIdleAction : BasicIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the FcfsIdleAction class with the specified list of idle actions.
        /// </summary>
        public FcfsIdleAction(List<IIdleAction> actions)
                    : base(actions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FcfsIdleAction class with the specified
        /// name and list of idle actions, and initializes the base class with the actions
        /// and name.
        /// </summary>
        public FcfsIdleAction(string name, List<IIdleAction> actions)
                    : base(actions, name)
        {
        }

        /// <summary>
        /// Executes the enter method for each action in the Actions list until a successful enter method is found.
        /// Sets the SelectedAction property to the action that had a successful enter method and returns true.
        /// If no successful enter method is found, returns false.
        /// </summary>
        public override bool Enter()
        {
            foreach (IIdleAction action in Actions)
            {
                if (action.Enter())
                {
                    SelectedAction = action;
                    return true;
                }
            }

            return false;
        }
    }
}