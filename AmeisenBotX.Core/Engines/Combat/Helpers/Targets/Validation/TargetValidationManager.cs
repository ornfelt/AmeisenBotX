using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation
{
    public class TargetValidationManager : ITargetValidator
    {
        /// <summary>
        /// Initializes a new instance of the TargetValidationManager class with the provided ITargetValidator.
        /// </summary>
        public TargetValidationManager(ITargetValidator validator)
        {
            Validators = new() { validator };
            BlacklistTargetValidator = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetValidationManager"/> class.
        /// </summary>
        /// <param name="validators">The collection of target validators to be used.</param>
        public TargetValidationManager(IEnumerable<ITargetValidator> validators)
        {
            Validators = new(validators);
            BlacklistTargetValidator = new();
        }

        /// <summary>
        /// Gets or sets the validator for blacklisted display IDs in the target.
        /// </summary>
        public DisplayIdBlacklistTargetValidator BlacklistTargetValidator { get; }

        /// <summary>
        /// Gets or sets the collection of target validators.
        /// </summary>
        /// <value>
        /// The collection of target validators.
        /// </value>
        public List<ITargetValidator> Validators { get; }

        /// <summary>
        /// Adds a target validator to the collection of validators.
        /// </summary>
        public void Add(ITargetValidator validator)
        {
            Validators.Add(validator);
        }

        /// <summary>
        /// Checks if the provided IWowUnit is valid by verifying if it is on the blacklist and passing all other validators.
        /// </summary>
        public bool IsValid(IWowUnit unit)
        {
            // is unit on blacklist
            return BlacklistTargetValidator.IsValid(unit)
                // run all other validators
                && Validators.All(e => e.IsValid(unit));
        }
    }
}