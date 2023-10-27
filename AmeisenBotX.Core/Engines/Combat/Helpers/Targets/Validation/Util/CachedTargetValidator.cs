using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Util
{
    /// <summary>
    /// CachedTargetValidator class implements the ITargetValidator interface and provides a caching mechanism for target validators.
    /// </summary>
    public class CachedTargetValidator : ITargetValidator
    {
        /// <summary>
        /// Initializes a new instance of the CachedTargetValidator class.
        /// </summary>
        /// <param name="validator">The target validator to be cached.</param>
        /// <param name="maxCacheTime">The maximum time for caching the validator.</param>
        public CachedTargetValidator(ITargetValidator validator, TimeSpan maxCacheTime)
        {
            Validators = new() { validator };
            Cache = new();
            MaxCacheTime = maxCacheTime;
        }

        /// <summary>
        /// Initializes a new instance of the CachedTargetValidator class.
        /// </summary>
        /// <param name="validators">The collection of target validators.</param>
        /// <param name="maxCacheTime">The maximum cache time for the target validators.</param>
        public CachedTargetValidator(IEnumerable<ITargetValidator> validators, TimeSpan maxCacheTime)
        {
            Validators = new(validators);
            Cache = new();
            MaxCacheTime = maxCacheTime;
        }

        /// <summary>
        /// Gets the maximum cache time for the cache.
        /// </summary>
        public TimeSpan MaxCacheTime { get; }

        /// <summary>
        /// Gets or sets the list of target validators.
        /// </summary>
        /// <value>
        /// The list of target validators.
        /// </value>
        public List<ITargetValidator> Validators { get; }

        /// <summary>
        /// Gets or sets the cache, which is a dictionary storing values of type (DateTime, bool), 
        /// with keys of type ulong.
        /// </summary>
        private Dictionary<ulong, (DateTime, bool)> Cache { get; }

        /// <summary>
        /// Determines if the provided unit is valid by checking if it exists in the cache and if its cache entry is still within the maximum cache time. If it is valid, it returns the cached validation result. If not, it removes the unit from the cache and performs a full validation using all available validators.
        /// </summary>
        /// <param name="unit">The unit to be validated.</param>
        /// <returns>True if the unit is valid, false otherwise.</returns>
        public bool IsValid(IWowUnit unit)
        {
            if (Cache.ContainsKey(unit.Guid))
            {
                (DateTime, bool) cachedEntry = Cache[unit.Guid];

                if (DateTime.UtcNow - cachedEntry.Item1 < MaxCacheTime)
                {
                    return cachedEntry.Item2;
                }
                else
                {
                    Cache.Remove(unit.Guid);
                }
            }

            bool isValid = Validators.All(e => e.IsValid(unit));
            Cache.Add(unit.Guid, (DateTime.UtcNow, isValid));
            return isValid;
        }
    }
}