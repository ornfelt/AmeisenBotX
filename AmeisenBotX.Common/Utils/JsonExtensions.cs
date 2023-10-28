using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// Extension methods for JSON related operations, especially for JsonElement manipulation.
/// </summary>
namespace AmeisenBotX.Common.Utils
{
    /// <summary>
    /// Extension methods for JSON related operations, especially for JsonElement manipulation.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Deserializes a JsonElement to a specified type.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize to.</typeparam>
        /// <param name="element">The JsonElement to deserialize.</param>
        /// <returns>The deserialized object of type T.</returns>
        public static T To<T>(this JsonElement element)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }

        /// <summary>
        /// Convert a JsonElement to a dynamic dictionary.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>A dynamic dictionary representing the JSON structure.</returns>
        public static Dictionary<string, dynamic> ToDyn(this JsonElement element)
        {
            Dictionary<string, JsonElement> dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(element.GetRawText());
            Dictionary<string, dynamic> result = new();

            foreach (KeyValuePair<string, JsonElement> d in dict)
            {
                switch (d.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        result.Add(d.Key, d.Value.GetString());
                        break;

                    case JsonValueKind.Number:
                        result.Add(d.Key, d.Value.GetDouble());
                        break;

                    case JsonValueKind.True:
                        result.Add(d.Key, true);
                        break;

                    case JsonValueKind.False:
                        result.Add(d.Key, false);
                        break;

                    default:
                        result.Add(d.Key, d.Value.To<dynamic>());
                        break;
                }
            }

            return result;
        }
    }
}