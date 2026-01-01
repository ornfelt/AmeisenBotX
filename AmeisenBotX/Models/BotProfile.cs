using System;
using System.IO;
using System.Text.Json;

namespace AmeisenBotX.Models
{
    /// <summary>
    /// Cached character stats for display on the profile card.
    /// </summary>
    public class ProfileStats
    {
        public string CharacterName { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string Realm { get; set; }
        public string Zone { get; set; }
        public DateTime LastPlayed { get; set; }
        public string Faction { get; set; } // "Alliance" or "Horde"

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Gets a human-readable relative time string (e.g., "2h ago", "Yesterday").
        /// </summary>
        public string GetLastPlayedRelative()
        {
            if (LastPlayed == default)
            {
                return null;
            }

            TimeSpan diff = DateTime.Now - LastPlayed;

            if (diff.TotalMinutes < 1)
            {
                return "Just now";
            }

            if (diff.TotalMinutes < 60)
            {
                return $"{(int)diff.TotalMinutes}m ago";
            }

            return diff.TotalHours < 24
                ? $"{(int)diff.TotalHours}h ago"
                : diff.TotalDays < 2
                ? "Yesterday"
                : diff.TotalDays < 7
                ? $"{(int)diff.TotalDays}d ago"
                : diff.TotalDays < 30 ? $"{(int)(diff.TotalDays / 7)}w ago" : LastPlayed.ToString("MMM d");
        }

        /// <summary>
        /// Loads stats from the profile folder's stats.json file.
        /// </summary>
        public static ProfileStats Load(string profileFolder)
        {
            try
            {
                string statsPath = Path.Combine(profileFolder, "stats.json");
                if (File.Exists(statsPath))
                {
                    string json = File.ReadAllText(statsPath);
                    return JsonSerializer.Deserialize<ProfileStats>(json);
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Saves stats to the profile folder's stats.json file.
        /// </summary>
        public void Save(string profileFolder)
        {
            try
            {
                string statsPath = Path.Combine(profileFolder, "stats.json");
                File.WriteAllText(statsPath, JsonSerializer.Serialize(this, JsonOptions));
            }
            catch { }
        }
    }

    /// <summary>
    /// Represents a bot profile with its configuration and portrait.
    /// </summary>
    public class BotProfile
    {
        /// <summary>
        /// Display name of the profile (folder name).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full path to the config.json file.
        /// </summary>
        public string ConfigPath { get; set; }

        /// <summary>
        /// Full path to the portrait.png file.
        /// </summary>
        public string PortraitPath { get; set; }

        /// <summary>
        /// Cached character stats (name, level, class, realm).
        /// </summary>
        public ProfileStats Stats { get; set; }

        /// <summary>
        /// Whether a portrait image exists for this profile.
        /// </summary>
        public bool HasPortrait => !string.IsNullOrEmpty(PortraitPath) && File.Exists(PortraitPath);

        /// <summary>
        /// Whether cached stats exist for this profile.
        /// </summary>
        public bool HasStats => Stats != null && !string.IsNullOrEmpty(Stats.CharacterName);

        /// <summary>
        /// Whether this is the special "New Config" placeholder.
        /// </summary>
        public bool IsNewConfig { get; set; }

        /// <summary>
        /// Creates a BotProfile from a profile directory path.
        /// </summary>
        public static BotProfile FromDirectory(string directoryPath)
        {
            string name = Path.GetFileName(directoryPath);
            string configPath = Path.Combine(directoryPath, "config.json");
            string portraitPath = Path.Combine(directoryPath, "portrait.png");

            return new BotProfile
            {
                Name = name,
                ConfigPath = configPath,
                PortraitPath = portraitPath,
                Stats = ProfileStats.Load(directoryPath),
                IsNewConfig = false
            };
        }

        /// <summary>
        /// Creates the special "New Config" placeholder profile.
        /// </summary>
        public static BotProfile CreateNewConfigPlaceholder()
        {
            return new BotProfile
            {
                Name = "New Bot",
                IsNewConfig = true
            };
        }
    }
}
