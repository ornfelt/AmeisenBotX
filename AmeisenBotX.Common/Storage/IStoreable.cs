using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// Defines a contract for objects that can be serialized to and deserialized from a JSON representation.
/// Classes or structs that implement this interface should be able to convert their internal state 
/// to a dictionary representation for saving and should also be able to populate their internal state 
/// from a dictionary representation when loading.
/// </summary>
namespace AmeisenBotX.Common.Storage
{
    /// <summary>
    /// Defines a contract for objects that can be serialized to and deserialized from a JSON representation.
    /// Classes or structs that implement this interface should be able to convert their internal state 
    /// to a dictionary representation for saving and should also be able to populate their internal state 
    /// from a dictionary representation when loading.
    /// </summary>
    public interface IStoreable
    {
        /// <summary>
        /// Loads the values from a JSON representation into a dictionary. 
        /// Convert the JsonElement values to the desired object type using the To<T>() or ToDyn() extension methods.
        /// </summary>
        /// <param name="objects">A dictionary where each key-value pair represents a field or property and its corresponding JsonElement.</param>
        void Load(Dictionary<string, JsonElement> objects);

        /// <summary>
        /// Returns a dictionary of objects that need to be saved.
        /// The dictionary should contain field or property names as keys, and their current values as values.
        /// </summary>
        /// <returns>A dictionary containing objects to be saved.</returns>
        Dictionary<string, object> Save();
    }
}