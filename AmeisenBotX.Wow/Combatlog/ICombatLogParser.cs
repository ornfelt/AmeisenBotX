using System;
using System.Collections.Generic;

namespace AmeisenBotX.Wow.Combatlog
{
    /// <summary>
    /// Represents an interface for parsing combat log data.
    /// </summary>
    public interface ICombatlogParser
    {
        /// <summary>
        /// Event that is triggered when damage is dealt.
        /// </summary>
        event Action<ulong, ulong, int, int, int> OnDamage;

        /// <summary>
        /// Event that is triggered when a heal action occurs.
        /// The event parameters include the source entity's current health, 
        /// the target entity's current health, and the amount of healing done.
        /// </summary>
        event Action<ulong, ulong, int, int, int> OnHeal;

        /// <summary>
        /// Represents an event that is triggered when a party kill occurs.
        /// The event handler takes in two parameters, both of type ulong,
        /// which represent the IDs of the victim and the killer.
        /// </summary>
        event Action<ulong, ulong> OnPartyKill;

        /// <summary>
        /// Event that is triggered when a unit dies.
        /// </summary>
        event Action<ulong> OnUnitDied;

        /// <summary>
        /// Parses the given long timestamp along with a list of string arguments.
        /// </summary>
        void Parse(long timestamp, List<string> args);
    }
}