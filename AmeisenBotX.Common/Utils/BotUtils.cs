using AmeisenBotX.Common.Math;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/// <summary>
/// Contains utility classes and methods for the AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Common.Utils
{
    /// <summary>
    /// Utility class providing various helper methods for bot operations.
    /// </summary>
    public static class BotUtils
    {
        /// <summary>
        /// Constant value representing the WM_KEYDOWN Windows message.
        /// </summary>
        private const uint WM_KEYDOWN = 0x100;
        /// <summary>
        /// Constant value representing the WM_KEYUP Windows message.
        /// </summary>
        private const uint WM_KEYUP = 0x101;

        /// <summary>
        /// Convert a big number to the known wow format (K, M)
        /// </summary>
        /// <param name="value">Big number</param>
        /// <returns>Shortened number as string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BigValueToString(float value)
        {
            if (value >= 100000000)
            {
                return $"{(int)value / 1000000}M";
            }
            else if (value >= 100000)
            {
                return $"{(int)value / 1000}K";
            }

            return $"{value}";
        }

        /// <summary>
        /// Convert a byte array to string without a delimiter.
        /// Example: [0x00, 0x12, 0xab, 0xe4] -&gt; 0012abe4
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <returns>Byte array as a string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ByteArrayToString(byte[] bytes)
        {
            return bytes != null ? BitConverter.ToString(bytes).Replace("-", " ", StringComparison.OrdinalIgnoreCase) : "null";
        }

        /// <summary>
        /// Generate a random string fast using Path.GetRandomFileName()
        /// </summary>
        /// <returns>Random string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FastRandomString()
        {
            return Path.GetRandomFileName().Replace(".", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Generate a random string fast using Path.GetRandomFileName() without numbers
        /// </summary>
        /// <returns>Random string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FastRandomStringOnlyLetters()
        {
            return new string(FastRandomString().Where(c => c is not '-' and (< '0' or > '9')).ToArray());
        }

        /// <summary>
        /// Convert a wow guid to a wow npc id.
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>NpcId</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GuidToNpcId(ulong guid)
        {
            return (int)((guid >> 24) & 0x0000000000FFFFFF);
        }

        /// <summary>
        /// Move a position x meters ahead of the given target position.
        /// </summary>
        /// <param name="origin">Start position</param>
        /// <param name="targetPosition">Target position</param>
        /// <param name="offset">Meters to move over the target position</param>
        /// <returns>Final position</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MoveAhead(Vector3 origin, Vector3 targetPosition, float offset)
        {
            return MoveAhead(targetPosition, BotMath.GetFacingAngle(origin, targetPosition), offset);
        }

        /// <summary>
        /// Move a position x meters ahead with the given angle.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="rotation"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MoveAhead(Vector3 targetPosition, float rotation, float offset)
        {
            return BotMath.CalculatePositionAround(targetPosition, rotation, 0.0f, offset);
        }

        /// <summary>
        /// Replace the variables in a LUA string to make them less obvious.
        ///
        /// Mark the variable places using this format: "{v:0}=1" ({v:0} will be replaced with a
        /// random string) Name the next {v:1} the next {v:2} and so on.
        ///
        /// The first variable will always be the return variable name!
        /// </summary>
        /// <param name="input">LUA string</param>
        /// <returns>(LUA string, return variable name)</returns>
        public static (string, string) ObfuscateLua(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return (string.Empty, string.Empty);
            }

            string returnValueName = string.Empty;

            for (int i = 0; ; ++i)
            {
                string symbol = $"{{v:{i}}}";

                if (input.Contains(symbol, StringComparison.OrdinalIgnoreCase))
                {
                    string newValueName = FastRandomStringOnlyLetters();
                    input = input.Replace(symbol, newValueName, StringComparison.OrdinalIgnoreCase);

                    if (i == 0)
                    {
                        returnValueName = newValueName;
                    }
                }
                else
                {
                    break;
                }
            }

            return (input, returnValueName);
        }

        /// <summary>
        /// Send a key to the game using SendMessage()
        /// </summary>
        /// <param name="windowHandle">Windows handle</param>
        /// <param name="key">Key to send</param>
        /// <param name="minDelay">Minimun delay to release the key</param>
        /// <param name="maxDelay">Maximum delay to release the key</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SendKey(IntPtr windowHandle, IntPtr key, int minDelay = 20, int maxDelay = 40)
        {
            SendMessage(windowHandle, WM_KEYDOWN, key, IntPtr.Zero);
            Task.Delay(new Random().Next(minDelay, maxDelay)).Wait();
            SendMessage(windowHandle, WM_KEYUP, key, IntPtr.Zero);
        }

        /// <summary>
        /// Sends a Windows message to a specified window handle.
        /// </summary>
        /// <param name="windowHandle">The handle of the window to send the message to.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="param">Additional message-specific information.</param>
        /// <param name="parameter">Additional message-specific information.</param>
        /// <returns>The result of the message processing, which is dependent on the message sent.</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr windowHandle, uint msg, IntPtr param, IntPtr parameter);
    }
}