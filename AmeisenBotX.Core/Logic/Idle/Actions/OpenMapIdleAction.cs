using System;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    public class OpenMapIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the OpenMapIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces to associate with this OpenMapIdleAction.</param>
        public OpenMapIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot is set to "Only" mode.
        /// </summary>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets or sets the Bot property of the AmeisenBotInterfaces class.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time for a specific action.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Represents the maximum cooldown in milliseconds.
        /// </summary>
        public int MaxCooldown => 59 * 1000;

        /// <summary>
        /// Gets the maximum duration of a value, which is 0 in this case.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown time in milliseconds.
        /// </summary>
        public int MinCooldown => 43 * 1000;

        /// <summary>
        /// Gets the minimum duration value.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Method for entering a certain action.
        /// </summary>
        /// <returns>True if the action is successfully entered, otherwise false.</returns>
        public bool Enter()
        {
            return true;
        }

        /// <summary>
        /// Executes code to open the map and make the windows small and transparent. If the WorldMapFrame is shown, it will hide it and set the alpha to 1.0. If it is not shown, it will show it and click the WorldMapFrameSizeDownButton if it is shown. It will then set the alpha of the WorldMapFrame to 0.1.
        /// </summary>
        public void Execute()
        {
            // open map and make the windows small and transparent
            Bot.Wow.LuaDoString(@"
                if WorldMapFrame:IsShown() then
                    WorldMapFrame:Hide()
                    WorldMapFrame:SetAlpha(1.0)
                else WorldMapFrame:Show()
                    if WorldMapFrameSizeDownButton and WorldMapFrameSizeDownButton:IsShown() then
                        WorldMapFrameSizeDownButton:Click()
                    end

                    WorldMapFrame:SetAlpha(0.1)
                end");
        }

        /// <summary>
        /// Returns a string representation of the object, indicating whether autopilot mode is enabled and if it is the string is preceded by the "🤖" emoji.
        /// The string returned represents an "Open Map" action.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Open Map";
        }
    }
}