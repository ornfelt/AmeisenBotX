using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging.Enums;
using System;
using System.IO;
using System.Text;

namespace AmeisenBotX.Logging
{
    /// <summary>
    /// Represents a logger class that provides thread-safe logging functionality.
    /// </summary>
    public class AmeisenLogger
    {
        /// <summary>
        /// The padlock object used for thread synchronization.
        /// </summary>
        private static readonly object padlock = new();

        /// <summary>
        /// Represents a lock object used for synchronizing access to a StringBuilder.
        /// </summary>
        private static readonly object stringBuilderLock = new();

        /// <summary>
        /// Gets the instance of the AmeisenLogger class.
        /// </summary>
        private static AmeisenLogger instance;

        /// <summary>
        /// Initializes a new instance of the AmeisenLogger class.
        /// </summary>
        /// <param name="deleteOldLogs">Whether to delete old logs or not.</param>
        private AmeisenLogger(bool deleteOldLogs = false)
        {
            StringBuilder = new();
            Enabled = false;
            ActiveLogLevel = LogLevel.Debug;

            // default log path
            ChangeLogFolder(AppDomain.CurrentDomain.BaseDirectory + "log/", false);

            if (deleteOldLogs)
            {
                DeleteOldLogs();
            }

            LockedTimer logFileWriter = new(1000, LogFileWriterTick);
        }

        /// <summary>
        /// Event that is raised when a log message is emitted.
        /// </summary>
        /// <param name="level">The level of the log message.</param>
        /// <param name="message">The log message.</param>
        public event Action<LogLevel, string> OnLog;

        /// <summary>
        /// Event that is triggered whenever a raw log entry is logged.
        /// The event handler should handle three parameters: the log message as a string,
        /// an additional message as a string, and the log level as a LogLevel enum.
        /// </summary>
        public event Action<string, string, LogLevel> OnLogRaw;

        /// <summary>
        /// Gets the instance of the AmeisenLogger class. If the instance doesn't exist, it creates a new one and returns it.
        /// </summary>
        public static AmeisenLogger I
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new(true);
                    return instance;
                }
            }
        }

        /// <summary>
        /// Gets or sets the active log level.
        /// </summary>
        public LogLevel ActiveLogLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is enabled.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets the folder path where the log files are stored.
        /// </summary>
        public string LogFileFolder { get; private set; }

        /// <summary>
        /// Gets or sets the file path for logging purposes.
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets the private StringBuilder.
        /// </summary>
        private StringBuilder StringBuilder { get; }

        /// <summary>
        /// Changes the folder location for the log files.
        /// </summary>
        /// <param name="logFolderPath">The new folder path for the log files.</param>
        /// <param name="createFolder">Optional. Determines if the folder should be created if it doesn't exist. Default is true.</param>
        /// <param name="deleteOldLogs">Optional. Determines if old log files should be deleted. Default is true.</param>
        public void ChangeLogFolder(string logFolderPath, bool createFolder = true, bool deleteOldLogs = true)
        {
            LogFileFolder = logFolderPath;

            if (createFolder)
            {
                IOUtils.CreateDirectoryIfNotExists(logFolderPath);
            }

            LogFilePath = LogFileFolder + $"AmeisenBot.{DateTime.Now:dd.MM.yyyy}-{DateTime.Now:HH.mm}.txt";

            if (deleteOldLogs)
            {
                DeleteOldLogs();
            }
        }

        /// <summary>
        /// Deletes old log files from the specified log file folder.
        /// By default, it deletes files older than 1 day.
        /// </summary>
        /// <param name="daysToKeep">The number of days to keep log files. Defaults to 1.</param>
        public void DeleteOldLogs(int daysToKeep = 1)
        {
            if (Directory.Exists(LogFileFolder))
            {
                string[] files = Directory.GetFiles(LogFileFolder);

                foreach (string file in files)
                {
                    FileInfo fileInfo = new(file);

                    if (fileInfo.LastAccessTime < DateTime.Now.AddDays(daysToKeep * -1))
                    {
                        fileInfo.Delete();
                    }
                }
            }
        }

        /// <summary>
        ///     Logs a message with the specified tag and log level.
        /// </summary>
        /// <param name="tag">The tag associated with the log message.</param>
        /// <param name="log">The log message to be logged.</param>
        /// <param name="logLevel">The log level of the log message. The default value is LogLevel.Debug.</param>
        public void Log(string tag, string log, LogLevel logLevel = LogLevel.Debug) // [CallerFilePath] string callingClass = "", [CallerMemberName] string callingFunction = "", [CallerLineNumber] int callingCodeline = 0
        {
            if (Enabled && logLevel <= ActiveLogLevel)
            {
                lock (stringBuilderLock)
                {
                    OnLogRaw?.Invoke(tag, log, logLevel);

                    string line = $"[{DateTime.UtcNow.ToLongTimeString()}] {$"[{logLevel}]",-9} {$"[{tag}]",-24} {log}";

                    StringBuilder.AppendLine(line);
                    OnLog?.Invoke(logLevel, line);
                }
            }
        }

        /// <summary>
        /// Sets the Enabled property to true.
        /// </summary>
        public void Start()
        {
            Enabled = true;
        }

        /// <summary>
        /// Stops the process of writing to the log file.
        /// </summary>
        public void Stop()
        {
            if (Enabled)
            {
                Enabled = false;
                LogFileWriterTick();
            }
        }

        /// <summary>
        /// Writes the contents of the StringBuilder to the log file if the writer is enabled.
        /// </summary>
        private void LogFileWriterTick()
        {
            if (Enabled)
            {
                lock (stringBuilderLock)
                {
                    File.AppendAllText(LogFilePath, StringBuilder.ToString());
                    StringBuilder.Clear();
                }
            }
        }
    }
}