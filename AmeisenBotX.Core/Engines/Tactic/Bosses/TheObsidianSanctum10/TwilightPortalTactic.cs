using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Tactic.Bosses.TheObsidianSanctum10
{
    /// <summary>
    /// Initializes a new instance of the TwilightPortalTactic class.
    /// </summary>
    public class TwilightPortalTactic : ITactic
    {
        /// <summary>
        /// Initializes a new instance of the TwilightPortalTactic class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used by the tactic.</param>
        public TwilightPortalTactic(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            PortalClickEvent = new(TimeSpan.FromSeconds(1));

            Configurables = new()
            {
                { "isOffTank", false },
            };
        }

        /// <summary>
        /// Gets the area represented as a 3D vector with the values (3243, 541, 59).
        /// </summary>
        public Vector3 Area { get; } = new(3243, 541, 59);

        ///<summary>Returns the area of a circle with a fixed radius of 1024.0 units.</summary>
        public float AreaRadius { get; } = 1024.0f;

        /// <summary>
        /// Gets or sets the dictionary of configurable elements.
        /// </summary>
        /// <value>
        /// The dictionary of configurable elements.
        /// </value>
        public Dictionary<string, dynamic> Configurables { get; private set; }

        /// <summary>
        /// Gets the Map ID for The Obsidian Sanctum.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheObsidianSanctum;

        /// <summary>
        /// Gets the list of Dragon Display IDs.
        /// </summary>
        private static List<int> DragonDisplayId { get; } = new() { 27421, 27039 };

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces that represents the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Finds the nearest portal game object with a display ID of 1327 that is within 80 units of the player's position.
        /// </summary>
        private IWowGameobject NearestPortal => Bot.Objects.All.OfType<IWowGameobject>().FirstOrDefault(e => e.DisplayId == 1327 && e.Position.GetDistance(Bot.Player.Position) < 80.0);

        /// <summary>
        /// Gets or sets the TimegatedEvent for the PortalClickEvent.
        /// </summary>
        private TimegatedEvent PortalClickEvent { get; }

        /// <summary>
        /// Executes a tactic based on the specified role and melee status.
        /// </summary>
        /// <param name="role">The role of the character.</param>
        /// <param name="isMelee">Indicates whether the character is melee or not.</param>
        /// <param name="preventMovement">Outputs a value indicating whether movement should be prevented.</param>
        /// <param name="allowAttacking">Outputs a value indicating whether attacking should be allowed.</param>
        /// <returns>True if a tactic is executed, otherwise false.</returns>
        public bool ExecuteTactic(WowRole role, bool isMelee, out bool preventMovement, out bool allowAttacking)
        {
            if (role == WowRole.Dps)
            {
                IWowUnit wowUnit = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, DragonDisplayId, false);
                IWowGameobject portal = NearestPortal;

                if (wowUnit != null)
                {
                    if (portal != null && Bot.Player.HealthPercentage > 80.0)
                    {
                        preventMovement = true;
                        allowAttacking = false;

                        UsePortal(portal);
                        return true;
                    }
                }
                else if (portal != null && Bot.Player.HealthPercentage < 25.0)
                {
                    preventMovement = true;
                    allowAttacking = false;

                    UsePortal(portal);
                    return true;
                }
            }

            preventMovement = false;
            allowAttacking = true;
            return false;
        }

        /// <summary>
        /// Uses a portal in the game.
        /// If the player is not within range of the portal by 3.0 units, the bot moves towards the portal.
        /// If the portal click event is successfully executed, the bot interacts with the portal.
        /// </summary>
        private void UsePortal(IWowGameobject portal)
        {
            if (!Bot.Player.IsInRange(portal, 3.0f))
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, portal.Position);
            }
            else if (PortalClickEvent.Run())
            {
                Bot.Wow.InteractWithObject(portal);
            }
        }
    }
}