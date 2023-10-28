using AmeisenBotX.Common.Utils;
using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// Represents a basic configurable class that can be loaded from and saved to a JSON representation.
/// </summary>
namespace AmeisenBotX.Common.Storage
{
    /// <summary>
    /// Represents a basic configurable class that can be loaded from and saved to a JSON representation.
    /// </summary>
    public abstract class SimpleConfigurable : IStoreable
    {
        /// <summary>
        /// Gets or sets a dictionary of configurable items. 
        /// The keys represent the name or identifier of the configurable item, and the values represent their current settings or values.
        /// </summary>
        public Dictionary<string, dynamic> Configurables { get; protected set; } = new();

        /// <summary>
        /// Loads configurable items from a provided dictionary representation that is typically sourced from JSON.
        /// If an item from the input exists in the current <see cref="Configurables"/> collection, its value will be updated.
        /// If the item doesn't exist, it will be added to the <see cref="Configurables"/> collection.
        /// </summary>
        /// <param name="objects">A dictionary where each key-value pair represents a configurable item and its corresponding JsonElement.</param>
        public virtual void Load(Dictionary<string, JsonElement> objects)
        {
            if (objects.ContainsKey("Configurables"))
            {
                foreach (KeyValuePair<string, dynamic> x in objects["Configurables"].ToDyn())
                {
                    if (Configurables.ContainsKey(x.Key))
                    {
                        Configurables[x.Key] = x.Value;
                    }
                    else
                    {
                        Configurables.Add(x.Key, x.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current state of the <see cref="Configurables"/> collection into a dictionary representation 
        /// suitable for conversion to JSON or similar formats.
        /// </summary>
        /// <returns>A dictionary containing the current state of the <see cref="Configurables"/> collection.</returns>
        public virtual Dictionary<string, object> Save()
        {
            return new()
            {
                { "Configurables", Configurables }
            };
        }
    }
}