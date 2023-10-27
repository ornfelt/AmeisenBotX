using System;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    public class SheathWeaponIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the SheathWeaponIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public SheathWeaponIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot mode is exclusive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the autopilot mode is exclusive; otherwise, <c>false</c>.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces object that represents the bot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        ///<summary>
        /// Gets or sets the cooldown time for the specified operation.
        ///</summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value in milliseconds.
        /// </summary>
        public int MaxCooldown => 49 * 1000;

        /// <summary>
        /// Gets or sets the maximum duration.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 16 * 1000;

        /// <summary>
        /// Gets the minimum duration, which is always 0.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Method to enter a specific action.
        /// </summary>
        public bool Enter()
        {
            return true;
        }

        /// <summary>
        /// Executes the "ToggleSheath()" Lua function to toggle the sheath state of the bot.
        /// </summary>
        public void Execute()
        {
            Bot.Wow.LuaDoString("ToggleSheath()");
        }

        /// <summary>
        /// Overrides the ToString() method and returns the string representation of the object.
        /// If AutopilotOnly is true, adds a robot emoji before the string.
        /// The string value is "Sheath Weapon".
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Sheath Weapon";
        }
    }
}