namespace AmeisenBotX.Bridge;

/// <summary>
/// Simple file-based logger for debugging the injected bridge.
/// </summary>
public static class BridgeLogger
{
    private static StreamWriter? _writer;
    private static readonly object _lock = new();

    public static void Init(string botName)
    {
        try
        {
            // Path: AppData\Roaming\AmeisenBotX\profiles\{BOTNAME}\log\AmeisenBot.Implant.{Date}.txt
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Sanitize botName for path
            string safeName = string.Join("_", botName.Split(Path.GetInvalidFileNameChars()));

            string logDir = Path.Combine(appData, "AmeisenBotX", "profiles", safeName, "log");
            Directory.CreateDirectory(logDir);

            string fileName = $"AmeisenBot.Implant.{DateTime.Now:dd.MM.yyyy-HH.mm}.txt";
            string fullPath = Path.Combine(logDir, fileName);

            _writer = new StreamWriter(fullPath, true) { AutoFlush = true };
            Log($"Bridge Logger Initialized for {botName}");
        }
        catch
        {
            // Fallback to local directory if AppData fails
            try
            {
                string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bridge_Fallback.log");
                _writer = new StreamWriter(fallbackPath, true) { AutoFlush = true };
                Log($"[Fallback] Bridge Logger Initialized (AppData failed). BotName: {botName}");
            }
            catch { /* Total failure */ }
        }
    }

    public static void Log(string message)
    {
        if (_writer == null)
        {
            return;
        }

        lock (_lock)
        {
            try
            {
                _writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
            catch { }
        }
    }
}
