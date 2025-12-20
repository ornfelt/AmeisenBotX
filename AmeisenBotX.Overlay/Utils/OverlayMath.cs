using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects.Raw;
using System;
using System.Drawing;
using Rect = AmeisenBotX.Memory.Win32.Rect;

namespace AmeisenBotX.Overlay.Utils
{
    public static class OverlayMath
    {
        public const float DEG_TO_RAD = MathF.PI / 180.0f;

        /// <summary>
        /// Transform world coordinates to screen coordinates.
        /// </summary>
        /// <param name="clientRect">Window size</param>
        /// <param name="cameraInfo">Game camera info</param>
        /// <param name="position">World position</param>
        /// <param name="screenCoordinates">Screen Position</param>
        /// <returns>True when coordinates are on the window, false if not</returns>
        public static bool WorldToScreen(Rect clientRect, RawCameraInfo cameraInfo, Vector3 position, out Point screenCoordinates)
        {
            Vector3 diff = position - cameraInfo.Pos;
            Vector3 view = diff * cameraInfo.ViewMatrix.Inverse();

            // Object is behind the camera
            if (view.X <= 0.0f)
            {
                screenCoordinates = default;
                return false;
            }

            float windowWidth = clientRect.Right - clientRect.Left;
            float windowHeight = clientRect.Bottom - clientRect.Top;

            float screenX = windowWidth / 2.0f;
            float screenY = windowHeight / 2.0f;
            float aspect = windowWidth / windowHeight;

            // Try to use camera FOV if it looks valid, otherwise use the original heuristic
            float fovHorizontal;
            float fovVertical;

            if (cameraInfo.Fov > 0.5f && cameraInfo.Fov < 2.5f)
            {
                // FOV looks like it's in radians (typical range 0.5-2.0 rad = ~30-115 degrees)
                fovVertical = cameraInfo.Fov;
                fovHorizontal = 2.0f * MathF.Atan(MathF.Tan(fovVertical / 2.0f) * aspect);
            }
            else
            {
                // Fallback to original heuristic (works "half decent")
                fovHorizontal = (aspect * (aspect >= 1.6f ? 55.0f : 44.0f)) * DEG_TO_RAD;
                fovVertical = (aspect * 35.0f) * DEG_TO_RAD;
            }

            // Calculate projection factors
            float tmpX = screenX / MathF.Tan(fovHorizontal / 2.0f);
            float tmpY = screenY / MathF.Tan(fovVertical / 2.0f);

            // Project to screen
            float projectedX = screenX + (-view.Y * tmpX / view.X);
            float projectedY = screenY + (-view.Z * tmpY / view.X);

            screenCoordinates = new()
            {
                X = (int)projectedX,
                Y = (int)projectedY
            };

            return screenCoordinates.X > 0
                && screenCoordinates.Y > 0
                && screenCoordinates.X < windowWidth
                && screenCoordinates.Y < windowHeight;
        }
    }
}