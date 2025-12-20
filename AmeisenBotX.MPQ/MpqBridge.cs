using AmeisenBotX.MPQ.Blp;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace AmeisenBotX.MPQ
{
    public unsafe partial class MpqBridge : IDisposable
    {
        private readonly string _wowFolder;
        private readonly string _cacheFolder;

        private readonly ConcurrentDictionary<string, Bitmap> _ramCache = new();
        private readonly ConcurrentDictionary<string, object> _loadingLocks = new();

        private readonly List<IntPtr> _openArchives = [];
        private readonly object _mpqLock = new();
        private bool _areMpqsInitialized = false;

        public MpqBridge(string wowFolder)
        {
            _wowFolder = wowFolder;
            _cacheFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"AmeisenBotX\cache"
            );

            if (!Directory.Exists(_cacheFolder))
                Directory.CreateDirectory(_cacheFolder);
        }

        public Bitmap GetIcon(string internalPath)
        {
            if (string.IsNullOrEmpty(internalPath)) return null;

            string normalizedPath = internalPath.Replace("/", "\\").TrimStart('\\');

            if (normalizedPath.EndsWith(".blp", StringComparison.OrdinalIgnoreCase))
                normalizedPath = normalizedPath.Substring(0, normalizedPath.Length - 4);

            string cacheKey = normalizedPath.ToLower();
            string pngPath = Path.Combine(_cacheFolder, normalizedPath + ".png");

            if (_ramCache.TryGetValue(cacheKey, out var memImage))
                return memImage;

            var lockObj = _loadingLocks.GetOrAdd(cacheKey, _ => new object());

            lock (lockObj)
            {
                if (_ramCache.TryGetValue(cacheKey, out memImage))
                    return memImage;

                if (File.Exists(pngPath))
                {
                    var diskImg = LoadFromDisk(pngPath);
                    if (diskImg != null)
                    {
                        _ramCache[cacheKey] = diskImg;
                        return diskImg;
                    }
                }

                EnsureMpqsInitialized();

                byte[] blpData = GetFileBytes(normalizedPath + ".blp");

                if (blpData != null && blpData.Length > 0)
                {
                    try
                    {
                        using var ms = new MemoryStream(blpData);
                        using var blpFile = new BlpFile(ms);

                        var bmp = blpFile.GetBitmap(0);

                        EnsureDirectoryExists(pngPath);
                        bmp.Save(pngPath, ImageFormat.Png);

                        _ramCache[cacheKey] = bmp;

                        return bmp;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] Failed to convert BLP {normalizedPath}: {ex.Message}");
                    }
                }
            }

            _loadingLocks.TryRemove(cacheKey, out _);

            return null;
        }

        public byte[] ReadFileBytes(string internalPath)
        {
            EnsureMpqsInitialized();
            return GetFileBytes(internalPath.Replace("/", "\\"));
        }

        private byte[] GetFileBytes(string searchPath)
        {
            lock (_mpqLock)
            {
                if (_openArchives.Count == 0) return null;

                foreach (var hMpq in _openArchives)
                {
                    if (StormLib.SFileOpenFileEx(hMpq, searchPath, 0, out IntPtr hFile))
                    {
                        try
                        {
                            uint fileSize = StormLib.SFileGetFileSize(hFile, out uint fileSizeHigh);

                            if (fileSize == 0xFFFFFFFF || fileSize == 0) continue;

                            byte[] buffer = new byte[fileSize];

                            if (StormLib.SFileReadFile(hFile, buffer, fileSize, out uint bytesRead, IntPtr.Zero))
                            {
                                return buffer;
                            }
                        }
                        finally
                        {
                            StormLib.SFileCloseFile(hFile);
                        }
                    }
                }
            }

            return null;
        }

        private void InitializeAllMpqs()
        {
            string dataPath = Path.Combine(_wowFolder, "Data");

            if (!Directory.Exists(dataPath)) return;

            var allMpqFiles = new List<string>();

            allMpqFiles.AddRange(Directory.GetFiles(dataPath, "*.mpq"));

            foreach (var subDir in Directory.GetDirectories(dataPath))
            {
                allMpqFiles.AddRange(Directory.GetFiles(subDir, "*.mpq"));
            }

            var sortedMpqs = allMpqFiles
                .Select(path => new { Path = path, Priority = CalculateMpqPriority(path) })
                .OrderByDescending(x => x.Priority)
                .Select(x => x.Path)
                .ToList();

            Console.WriteLine($"[MPQ] Opening {sortedMpqs.Count} archives...");

            foreach (var file in sortedMpqs)
            {
                if (StormLib.SFileOpenArchive(file, 0, 0x100, out IntPtr hMpq))
                {
                    _openArchives.Add(hMpq);
                }
            }
        }

        private Bitmap LoadFromDisk(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                using var ms = new MemoryStream(bytes);
                return new Bitmap(ms);
            }
            catch { return null; }
        }

        private void EnsureDirectoryExists(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        private int CalculateMpqPriority(string filePath)
        {
            string fileName = Path.GetFileName(filePath).ToLower();
            int score = 0;

            if (filePath.Contains("enUS") || filePath.Contains("deDE")) score += 10000;

            var match = MpqPatchRegex().Match(fileName);
            if (match.Success)
                score += 1000 + int.Parse(match.Groups[1].Value);
            else if (fileName.StartsWith("patch.mpq"))
                score += 900;

            if (fileName.StartsWith("art")) score += 500;
            if (fileName.StartsWith("lichking")) score += 400;
            if (fileName.StartsWith("expansion")) score += 300;
            if (fileName.StartsWith("common")) score += 100;

            return score;
        }

        private void EnsureMpqsInitialized()
        {
            if (_areMpqsInitialized) return;

            lock (_mpqLock)
            {
                if (_areMpqsInitialized) return;
                InitializeAllMpqs();
                _areMpqsInitialized = true;
            }
        }

        public void Dispose()
        {
            lock (_mpqLock)
            {
                foreach (var hMpq in _openArchives)
                {
                    StormLib.SFileCloseArchive(hMpq);
                }
                _openArchives.Clear();

                foreach (var bmp in _ramCache.Values)
                {
                    bmp.Dispose();
                }
                _ramCache.Clear();
            }
        }

        [GeneratedRegex(@"patch.*?(\d+)\.mpq")]
        private static partial Regex MpqPatchRegex();
    }
}
