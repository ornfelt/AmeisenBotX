using AmeisenBotX.Wow.Combatlog.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Represents a namespace for combat log objects in the AmeisenBotX.Wow.Combatlog.Objects namespace.
/// </summary>
namespace AmeisenBotX.Wow.Combatlog.Objects
{
    /// <summary>
    /// Represents a basic combat log entry.
    /// </summary>
    /// <remarks>
    /// This class is used for storing information about a combat log entry.
    /// It contains properties for various attributes of the combat log entry, such as args, destination GUID, 
    /// destination name, flags, source GUID, source name, subtype, target flags, timestamp, and type.
    /// </remarks>
    [Serializable]
    public record BasicCombatlogEntry
    {
        /// <summary>
        /// Gets or sets the list of string arguments.
        /// </summary>
        public List<string> Args { get; set; }

        /// <summary>
        /// Gets or sets the destination globally unique identifier (GUID).
        /// </summary>
        public ulong DestinationGuid { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination.
        /// </summary>
        public string DestinationName { get; set; }

        /// <summary>
        /// Gets or sets the flags of the object.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the SourceGuid property, which represents the globally unique identifier (GUID) 
        /// for the source. It is a 64-bit unsigned integer value.
        /// </summary>
        public ulong SourceGuid { get; set; }

        /// <summary>
        /// Gets or sets the source name.
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the combat log entry.
        /// </summary>
        public CombatlogEntrySubtype Subtype { get; set; }

        /// <summary>
        /// Gets or sets the target flags.
        /// </summary>
        public int TargetFlags { get; set; }

        /// <summary>
        /// The timestamp of the object.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the combat log entry type.
        /// </summary>
        public CombatlogEntryType Type { get; set; }

        /// <summary>
        /// Parses a list of event arguments and populates a BasicCombatlogEntry object with the parsed values.
        /// Returns true if the parsing is successful, otherwise returns false.
        /// </summary>
        /// <param name="fields">The fields object that maps indices of event arguments.</param>
        /// <param name="eventArgs">The list of event arguments to parse.</param>
        /// <param name="basicCombatLogEntry">The output BasicCombatlogEntry object that will be populated with the parsed values.</param>
        /// <returns>True if the parsing is successful, otherwise false.</returns>
        public static bool TryParse(ICombatlogFields fields, List<string> eventArgs, out BasicCombatlogEntry basicCombatLogEntry)
        {
            basicCombatLogEntry = new BasicCombatlogEntry();

            if (eventArgs != null && eventArgs.Count < 8)
            {
                return false;
            }

            basicCombatLogEntry.Args = eventArgs;

            if (double.TryParse(eventArgs[fields.Timestamp].Replace(".", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out double millis))
            {
                basicCombatLogEntry.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)millis).LocalDateTime;
            }
            else
            {
                return false;
            }

            string[] splitted = eventArgs[fields.Type]
                .Replace("SPELL_BUILDING", "SPELLBUILDING")
                .Replace("SPELL_PERIODIC", "SPELLPERIODIC")
                .Split(new char[] { '_' }, 2);

            if (splitted.Length < 2)
            {
                return false;
            }

            if (Enum.TryParse(splitted[0], out CombatlogEntryType type)
                && Enum.TryParse(splitted[1], out CombatlogEntrySubtype subtype))
            {
                basicCombatLogEntry.Type = type;
                basicCombatLogEntry.Subtype = subtype;
            }
            else
            {
                return false;
            }

            if (ulong.TryParse(eventArgs[fields.Source].Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out ulong sourceGuid))
            {
                basicCombatLogEntry.SourceGuid = sourceGuid;
            }
            else
            {
                return false;
            }

            basicCombatLogEntry.SourceName = eventArgs[fields.SourceName];

            if (int.TryParse(eventArgs[fields.Flags], out int flags))
            {
                basicCombatLogEntry.Flags = flags;
            }
            else
            {
                return false;
            }

            if (ulong.TryParse(eventArgs[fields.DestinationGuid].Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out ulong destGuid))
            {
                basicCombatLogEntry.DestinationGuid = destGuid;
            }
            else
            {
                return false;
            }

            basicCombatLogEntry.DestinationName = eventArgs[fields.DestinationName];

            if (int.TryParse(eventArgs[fields.TargetFlags], out int targetFlags))
            {
                basicCombatLogEntry.TargetFlags = targetFlags;
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Overrides the default ToString method and returns a string representation of the object,
        /// including the Type, Subtype, SourceName, and DestinationName.
        /// </summary>
        public override string ToString()
        {
            return $"{Type}_{Subtype} {SourceName} -> {DestinationName}";
        }
    }
}