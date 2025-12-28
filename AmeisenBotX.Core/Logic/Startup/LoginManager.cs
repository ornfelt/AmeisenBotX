using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Shared.Lua;
using System;

namespace AmeisenBotX.Core.Logic.Startup
{
    /// <summary>
    /// Handles WoW login flow including:
    /// - CVar configuration
    /// - Automatic login with credentials
    /// - Character selection
    /// </summary>
    public class LoginManager
    {
        private readonly AmeisenBotConfig Config;
        private readonly IWowInterface Wow;

        private readonly TimegatedEvent LoginAttemptEvent;
        private bool _hasDoneFirstLogin = true;

        public LoginManager(AmeisenBotConfig config, IWowInterface wow)
        {
            Config = config;
            Wow = wow;
            LoginAttemptEvent = new(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Perform login attempt. Called repeatedly until successful.
        /// </summary>
        /// <param name="antiAfkAction">Action to run for anti-AFK during login screen.</param>
        public void PerformLogin(Action antiAfkAction)
        {
            Wow.SetWorldLoadedCheck(true);

            if (_hasDoneFirstLogin)
            {
                _hasDoneFirstLogin = false;
                SetCVars();
            }

            // Prevent inactivity logout
            antiAfkAction?.Invoke();

            if (LoginAttemptEvent.Run())
            {
                Wow.LuaDoString(LuaLogin.Get(Config.Username, Config.Password, Config.Realm, Config.CharacterSlot));
            }

            Wow.SetWorldLoadedCheck(false);
        }

        /// <summary>
        /// Reset first login flag (e.g., on disconnect).
        /// </summary>
        public void Reset()
        {
            _hasDoneFirstLogin = true;
        }

        private void SetCVars()
        {
            if (Config.AutoSetUlowGfxSettings)
            {
                // Use version-specific CVars from the interface implementation
                Wow.ApplyBotCVars(Config.MaxFps);
            }
        }
    }
}
