using System.IO;
using System.Runtime.CompilerServices;

/// <summary>
/// Contains utility classes for the AmeisenBotX project.
/// </summary>
namespace AmeisenBotX.Common.Utils
{
    /// <summary>
    /// Utility class providing methods to assist with input/output operations.
    /// </summary>
    public static class IOUtils
    {
        /// <summary>
        /// Creates a folder if it does not exist.
        /// </summary>
        /// <param name="directory">Path to the directory.</param>
        /// <returns>Path of the directory.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CreateDirectoryIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        /// <summary>
        /// Creates a folder if it does not exist.
        /// </summary>
        /// <param name="directory">Path to the directory.</param>
        /// <returns>Path of the directory.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CreateDirectoryIfNotExists(params string[] directory)
        {
            string path = Path.Combine(directory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}