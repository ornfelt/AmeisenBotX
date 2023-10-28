using AmeisenBotX.Common.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Manages the serialization and deserialization of objects that implement the <see cref="IStoreable"/> interface.
/// Allows for storing the state of objects in JSON format and subsequently loading their states from the JSON files.
/// </summary>
namespace AmeisenBotX.Common.Storage
{
    /// <summary>
    /// Manages the serialization and deserialization of objects that implement the <see cref="IStoreable"/> interface.
    /// Allows for storing the state of objects in JSON format and subsequently loading their states from the JSON files.
    /// </summary>
    public class StorageManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageManager"/> class.
        /// </summary>
        /// <param name="basePath">The directory path where JSON files will be saved and loaded from.</param>
        /// <param name="partsToRemove">Strings to be removed from the filename (typically to exclude certain namespace parts).</param>
        public StorageManager(string basePath, IEnumerable<string> partsToRemove = null)
        {
            BasePath = basePath;
            PartsToRemove = partsToRemove;

            Storeables = new();
        }

        /// <summary>
        /// The base directory path where JSON files will be saved and loaded from.
        /// </summary>
        private string BasePath { get; }

        /// <summary>
        /// Collection of strings to be removed from the file name when building the path.
        /// Typically used to exclude certain namespace parts.
        /// </summary>
        private IEnumerable<string> PartsToRemove { get; }

        /// <summary>
        /// List of objects that implement the <see cref="IStoreable"/> interface. These objects
        /// will have their states saved to or loaded from the JSON files.
        /// </summary>
        private List<IStoreable> Storeables { get; set; }

        /// <summary>
        /// Loads the state of the specified object from its corresponding JSON file.
        /// </summary>
        /// <param name="s">The object to load.</param>
        public void Load(IStoreable s)
        {
            if (!Storeables.Contains(s))
            {
                Register(s);
            }

            string fullPath = BuildPath(s);

            try
            {
                string parent = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(parent))
                {
                    return;
                }

                s.Load(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(fullPath), new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString }));
            }
            catch
            {
                // AmeisenLogger.I.Log("CombatClass", $"Failed to load {s.GetType().Name}
                // ({fullPath}):\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Loads the states of all registered objects from their respective JSON files.
        /// </summary>
        public void LoadAll()
        {
            foreach (IStoreable s in Storeables)
            {
                Load(s);
            }
        }

        /// <summary>
        /// Registers an object to be managed by the <see cref="StorageManager"/>.
        /// </summary>
        /// <param name="s">The object to register.</param>
        public void Register(IStoreable s)
        {
            Storeables.Add(s);
        }

        /// <summary>
        /// Saves the state of the specified object to a JSON file.
        /// </summary>
        /// <param name="s">The object to save.</param>
        public void Save(IStoreable s)
        {
            if (!Storeables.Contains(s))
            {
                Register(s);
            }

            string fullPath = BuildPath(s);

            try
            {
                Dictionary<string, object> data = s.Save();

                if (data == null)
                {
                    return;
                }

                IOUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(fullPath));
                File.WriteAllText(fullPath, JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true }));
            }
            catch
            {
                // AmeisenLogger.I.Log("CombatClass", $"Failed to save {s.GetType().Name}
                // ({fullPath}):\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Saves the states of all registered objects to their respective JSON files.
        /// </summary>
        public void SaveAll()
        {
            foreach (IStoreable s in Storeables)
            {
                Save(s);
            }
        }

        /// <summary>
        /// Constructs the full path for the JSON file corresponding to the specified <see cref="IStoreable"/> object.
        /// </summary>
        /// <param name="s">The <see cref="IStoreable"/> object for which the path is to be built.</param>
        /// <returns>The full path to the JSON file representing the state of the given object.</returns>
        private string BuildPath(IStoreable s)
        {
            string typePath = (s.GetType().FullName + ".json").ToLower();

            foreach (string rep in PartsToRemove)
            {
                typePath = typePath.Replace(rep.ToLower(), string.Empty);
            }

            return Path.Combine(BasePath, typePath);
        }
    }
}