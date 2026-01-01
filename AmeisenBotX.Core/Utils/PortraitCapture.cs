using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Core.Utils
{
    /// <summary>
    /// Utility class for capturing character portraits from the WoW UI.
    /// Creates a temporary UI frame to render the portrait texture, captures it from screen,
    /// and corrects for aspect ratio distortion when the window is stretched.
    /// </summary>
    public static partial class PortraitCapture
    {
        /// <summary>
        /// Default portrait output size in pixels.
        /// </summary>
        public const int PortraitSize = 128;

        /// <summary>
        /// Standard 4:3 aspect ratio that WoW 3.3.5a renders at internally.
        /// </summary>
        public const float NativeAspectRatio = 4.0f / 3.0f;

        /// <summary>
        /// Percentage of screen height to use for the portrait frame (0.0 - 1.0).
        /// 20% provides good quality while being unobtrusive.
        /// </summary>
        public const float ScreenPercentage = 0.20f;

        // Random frame name generator for stealth
        private static readonly Random Rng = new();
        private static string _frameName;

        /// <summary>
        /// Gets or generates a random frame name for stealth.
        /// The name is generated once per session.
        /// </summary>
        private static string FrameName => _frameName ??= GenerateRandomName();

        /// <summary>
        /// Generates a random alphanumeric name for the UI frame.
        /// </summary>
        private static string GenerateRandomName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int len = Rng.Next(8, 12);
            Span<char> name = stackalloc char[len];
            for (int i = 0; i < len; i++)
            {
                name[i] = chars[Rng.Next(chars.Length)];
            }

            return new string(name);
        }

        /// <summary>
        /// Optimized Lua script to create/update the portrait frame.
        /// Uses screen percentage for consistent sizing across window sizes.
        /// Frame uses a randomized name.
        /// </summary>
        /// <param name="unit">Unit ID to capture portrait of (player, target, party1, etc.)</param>
        /// <returns>Minified Lua script string.</returns>
        public static string GetCreateFrameLua(string unit = "player")
        {
            string n = FrameName;
            // Minified Lua with randomized frame name
            return $"local h=GetScreenHeight()*0.2;local f={n} or CreateFrame('Frame','{n}',UIParent);f:SetSize(h,h);f:SetPoint('TOPLEFT',0,0);local t=f.t or f:CreateTexture(nil,'BACKGROUND');t:SetAllPoints();t:SetTexCoord(0,1,0,1);t:SetBlendMode('DISABLE');f.t=t;SetPortraitTexture(t,'{unit}');f:Show()";
        }

        /// <summary>
        /// Lua script to hide the portrait frame after capture.
        /// </summary>
        public static string HideFrameLua => $"if {FrameName} then {FrameName}:Hide()end";

        /// <summary>
        /// Lua script to update the portrait to a different unit without recreating the frame.
        /// </summary>
        /// <param name="unit">Unit ID (player, target, party1, etc.)</param>
        public static string GetUpdatePortraitLua(string unit)
        {
            string n = FrameName;
            return $"if {n} and {n}.t then SetPortraitTexture({n}.t,'{unit}');{n}:Show()end";
        }

        /// <summary>
        /// Lua script to destroy the frame completely (cleanup).
        /// </summary>
        public static string DestroyFrameLua
        {
            get
            {
                string n = FrameName;
                return $"if {n} then {n}:Hide();{n}.t=nil;{n}=nil end";
            }
        }

        /// <summary>
        /// Resets the frame name, forcing a new random name on next use.
        /// Call this on session end or character change for extra stealth.
        /// </summary>
        public static void ResetFrameName() => _frameName = null;

        // Win32 API imports for window handling
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetClientRect(nint hWnd, out RECT lpRect);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ClientToScreen(nint hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X, Y;
        }

        /// <summary>
        /// Captures a portrait from the WoW window.
        /// Call GetCreateFrameLua() or GetUpdatePortraitLua() first to set up the frame.
        /// </summary>
        /// <param name="hwnd">WoW window handle.</param>
        /// <param name="outputSize">Desired output size (default 128x128).</param>
        /// <returns>Square bitmap of the portrait, or null on failure.</returns>
        public static Bitmap Capture(nint hwnd, int outputSize = PortraitSize)
        {
            if (hwnd == nint.Zero)
            {
                return null;
            }

            try
            {
                if (!GetClientRect(hwnd, out RECT clientRect))
                {
                    return null;
                }

                POINT topLeft = new() { X = 0, Y = 0 };
                if (!ClientToScreen(hwnd, ref topLeft))
                {
                    return null;
                }

                int clientWidth = clientRect.Right - clientRect.Left;
                int clientHeight = clientRect.Bottom - clientRect.Top;

                if (clientWidth <= 0 || clientHeight <= 0)
                {
                    return null;
                }

                // Calculate aspect ratio stretch factor
                float horizontalStretch = (float)clientWidth / clientHeight / NativeAspectRatio;

                // Frame is ScreenPercentage of height, stretched horizontally
                int captureHeight = Math.Max(16, (int)(clientHeight * ScreenPercentage));
                int captureWidth = Math.Max(16, Math.Min(clientWidth, (int)(captureHeight * horizontalStretch)));

                // Capture the frame area
                using Bitmap raw = new(captureWidth, captureHeight, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(raw))
                {
                    g.CopyFromScreen(topLeft.X, topLeft.Y, 0, 0, new Size(captureWidth, captureHeight));
                }

                // Resize to square output with high quality
                Bitmap portrait = new(outputSize, outputSize, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(portrait))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawImage(raw, 0, 0, outputSize, outputSize);
                }

                return portrait;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Full capture workflow: creates frame, waits for render, captures, and hides frame.
        /// </summary>
        /// <param name="hwnd">WoW window handle.</param>
        /// <param name="executeLua">Function to execute Lua in WoW (e.g., IWowInterface.LuaDoString).</param>
        /// <param name="unit">Unit to capture portrait of.</param>
        /// <param name="outputSize">Desired output size.</param>
        /// <param name="renderDelayMs">Delay after creating frame to allow render (default 100ms).</param>
        /// <returns>Square bitmap of the portrait, or null on failure.</returns>
        public static Bitmap CapturePortrait(
            nint hwnd,
            Func<string, bool> executeLua,
            string unit = "player",
            int outputSize = PortraitSize,
            int renderDelayMs = 100)
        {
            if (hwnd == nint.Zero || executeLua == null)
            {
                return null;
            }

            try
            {
                // Create/update the frame
                executeLua(GetCreateFrameLua(unit));

                // Wait for WoW to render the frame
                if (renderDelayMs > 0)
                {
                    System.Threading.Thread.Sleep(renderDelayMs);
                }

                // Capture
                Bitmap portrait = Capture(hwnd, outputSize);

                // Hide the frame
                executeLua(HideFrameLua);

                return portrait;
            }
            catch
            {
                return null;
            }
        }
    }
}
