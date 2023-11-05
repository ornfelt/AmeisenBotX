using AmeisenBotX.Core.Engines.Battleground.Jannis.Profiles;
using AmeisenBotX.Wow.Objects.Enums;
using System.Diagnostics;

/// <summary>
/// Represents a universal battleground engine that can handle different battleground profiles.
/// </summary>
namespace AmeisenBotX.Core.Engines.Battleground.Jannis
{
    /// <summary>
    /// Represents a universal battleground engine that can handle different battleground profiles.
    /// </summary>
    public class UniversalBattlegroundEngine : IBattlegroundEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalBattlegroundEngine"/> class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public UniversalBattlegroundEngine(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <inheritdoc />
        public string Author => "Jannis";

        /// <inheritdoc />
        public string Description => "Working battlegrounds:\n - Warsong Gulch";

        /// <inheritdoc />
        public string Name => "Universal Battleground Engine";

        /// <summary>
        /// Gets or sets the currently active battleground profile.
        /// </summary>
        public IBattlegroundProfile Profile { get; set; }

        /// <summary>
        /// Provides access to various bot functionalities and interfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <inheritdoc />
        public void Enter()
        {

        }

        /// <inheritdoc />
        public void Execute()
        {
            if (Profile == null)
            {
                TryLoadProfile();
            }

            Bot.CombatClass?.OutOfCombatExecute();
            Profile?.Execute();
            if (Bot.Player.IsGhost)
            {
                Bot.Movement.StopMovement();
                //Debug.WriteLine("Stopping movement since player is dead!");
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            Profile = null;
        }

        /// <summary>
        /// Overrides the ToString method to provide a formatted string representation of the engine.
        /// </summary>
        /// <returns>A string containing the name and author of the engine.</returns>
        public override string ToString()
        {
            return $"{Name} ({Author})";
        }

        /// <summary>
        /// Attempts to load a specific battleground profile based on the current map.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a profile for the current map is successfully loaded; otherwise, <c>false</c>.
        /// </returns>
        private bool TryLoadProfile()
        {
            switch (Bot.Objects.MapId)
            {
                case WowMapId.WarsongGulch:
                    Profile = new WarsongGulchProfile(Bot);
                    return true;

                default:
                    Profile = null;
                    return false;
            }
        }
    }
}