using AmeisenBotX.Common.Keyboard.Enums;
using AmeisenBotX.Common.Keyboard.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Common.Keyboard
{
    /// <summary>
    /// Provides a mechanism to globally intercept keyboard input and manage custom hotkey combinations.
    /// </summary>
    /// <remarks>
    /// The KeyboardHook class enables applications to define and listen to global hotkeys 
    /// (keyboard shortcuts) regardless of the current foreground application.
    /// </remarks>
    public class KeyboardHook
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHook"/> class.
        /// </summary>
        public KeyboardHook()
        {
            Hotkeys = new();
            KeyboardProc = LowLevelKeyboardCallback;
        }

        /// <summary>
        /// Represents the method that will handle the low-level keyboard input events.
        /// </summary>
        private delegate int LowLevelKeyboardProc(int nCode, IntPtr wParam, ref LowLevelKeyboardInput lParam);

        /// <summary>
        /// Gets the pointer to the hook set up to intercept keyboard events.
        /// </summary>
        private IntPtr HookPtr { get; set; }

        /// <summary>
        /// Gets the list of hotkey combinations and their associated actions.
        /// </summary>
        private List<(KeyCode, KeyCode, Action)> Hotkeys { get; }

        /// <summary>
        /// Gets the callback method that will be invoked upon a low-level keyboard input event.
        /// </summary>
        private LowLevelKeyboardProc KeyboardProc { get; }

        /// <summary>
        /// Registers a hotkey combination and associates it with a callback action.
        /// </summary>
        /// <param name="key">The primary key of the hotkey combination.</param>
        /// <param name="modifier">The modifier key (e.g., CTRL, ALT) to be used in combination with the primary key.</param>
        /// <param name="callback">The action to be invoked when the hotkey combination is pressed.</param>
        public void AddHotkey(KeyCode key, KeyCode modifier, Action callback)
        {
            Hotkeys.Add((key, modifier, callback));
        }

        /// <summary>
        /// Registers a hotkey and associates it with a callback action.
        /// </summary>
        /// <param name="key">The primary key of the hotkey.</param>
        /// <param name="callback">The action to be invoked when the hotkey is pressed.</param>
        public void AddHotkey(KeyCode key, Action callback)
        {
            Hotkeys.Add((key, KeyCode.None, callback));
        }

        /// <summary>
        /// Clears all registered hotkeys.
        /// </summary>
        public void Clear()
        {
            Hotkeys.Clear();
        }

        /// <summary>
        /// Disables the keyboard hook, stopping the interception of keyboard input.
        /// </summary>
        public void Disable()
        {
            if (HookPtr != IntPtr.Zero)
            {
                UnhookWindowsHookEx(HookPtr);
            }
        }

        /// <summary>
        /// Enables the keyboard hook, allowing for the interception of keyboard input.
        /// </summary>
        public void Enable()
        {
            if (HookPtr == IntPtr.Zero)
            {
                HookPtr = SetWindowsHookEx
                (
                    13,
                    KeyboardProc,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0
                );
            }
        }

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain.
        /// </summary>
        /// <param name="hHook">Handle to the current hook. Not used in this context, can be set to IntPtr.Zero.</param>
        /// <param name="nCode">Specifies the hook code passed to the current hook procedure.</param>
        /// <param name="wParam">Specifies the wParam value passed to the current hook procedure.</param>
        /// <param name="lParam">Specifies the lParam value passed to the current hook procedure.</param>
        /// <returns>This value is returned by the next hook procedure in the chain.</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern int CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, ref LowLevelKeyboardInput lParam);

        /// <summary>
        /// Retrieves the status of the specified virtual key.
        /// </summary>
        /// <param name="nVirtKey">Specifies a virtual key code.</param>
        /// <returns>If the high-order bit is 1, the key is down; otherwise, it is up.</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern short GetKeyState(KeyCode nVirtKey);

        /// <summary>
        /// Retrieves the module handle for the specified module.
        /// </summary>
        /// <param name="lpModuleName">The name of the loaded module (either a .dll or .exe file).</param>
        /// <returns>A handle to the specified module.</returns>
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">Specifies the type of hook procedure to be installed.</param>
        /// <param name="lpfn">Pointer to the hook procedure.</param>
        /// <param name="hMod">Handle to the DLL containing the hook procedure pointed to by the lpfn parameter.</param>
        /// <param name="dwThreadId">Specifies the identifier of the thread with which the hook procedure is to be associated.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hHook">Handle to the hook to be removed.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern int UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// Handles low-level keyboard input events, checking for hotkey combinations and invoking the associated actions.
        /// </summary>
        private int LowLevelKeyboardCallback(int nCode, IntPtr wParam, ref LowLevelKeyboardInput lParam)
        {
            int wParamValue = wParam.ToInt32();

            if (Enum.IsDefined(typeof(KeyboardState), wParamValue))
            {
                if ((KeyboardState)wParamValue is KeyboardState.KeyDown or KeyboardState.SysKeyDown)
                {
                    foreach ((KeyCode key, KeyCode mod, Action callback) in Hotkeys)
                    {
                        if (lParam.VirtualCode == key && (mod == KeyCode.None || (GetKeyState(mod) & 0x8000) > 0))
                        {
                            callback?.Invoke();
                        }
                    }
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, ref lParam);
        }
    }
}