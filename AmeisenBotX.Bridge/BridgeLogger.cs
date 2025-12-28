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
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Sanitize botName for path
            var safeName = string.Join("_", botName.Split(Path.GetInvalidFileNameChars()));

            var logDir = Path.Combine(appData, "AmeisenBotX", "profiles", safeName, "log");
            Directory.CreateDirectory(logDir);

            var fileName = $"AmeisenBot.Implant.{DateTime.Now:dd.MM.yyyy-HH.mm}.txt";
            var fullPath = Path.Combine(logDir, fileName);

            _writer = new StreamWriter(fullPath, true) { AutoFlush = true };
            Log($"Bridge Logger Initialized for {botName}");
        }
        catch
        {
            // Fallback to local directory if AppData fails
            try
            {
                var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bridge_Fallback.log");
                _writer = new StreamWriter(fallbackPath, true) { AutoFlush = true };
                Log($"[Fallback] Bridge Logger Initialized (AppData failed). BotName: {botName}");
            }
            catch { /* Total failure */ }
        }
    }

    public static void Log(string message)
    {
        if (_writer == null) return;

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
