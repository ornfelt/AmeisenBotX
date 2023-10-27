using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Tactic.Dungeon.PitOfSaron
{
    /// <summary>
    /// Represents a specific tactic implementation, named IckAndKrickTactic, that implements the ITactic interface.
    /// </summary>
    public class IckAndKrickTactic : ITactic
    {
        /// <summary>
        /// Initializes a new instance of the IckAndKrickTactic class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public IckAndKrickTactic(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            Configurables = new()
            {
                { "isOffTank", false },
            };
        }

        /// <summary>
        /// Gets the Vector3 representing the area with coordinates (823, 110, 509).
        /// </summary>
        public Vector3 Area { get; } = new(823, 110, 509);

        /// <summary>
        /// Gets the area radius.
        /// </summary>
        public float AreaRadius { get; } = 150.0f;

        /// <summary>
        /// Gets or sets the date and time when the chasing is activated.
        /// </summary>
        public DateTime ChasingActivated { get; private set; }

        /// <summary>
        /// Gets or sets the configurables dictionary.
        /// </summary>
        /// <value>
        /// The configurables dictionary.
        /// </value>
        public Dictionary<string, dynamic> Configurables { get; private set; }

        /// <summary>
        /// Gets the map ID for the Pit of Saron.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.PitOfSaron;

        /// <summary>
        /// Gets or sets the list of integer values for IckDisplayId.
        /// </summary>
        private static List<int> IckDisplayId { get; } = new List<int> { 30347 };

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces that represents the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Determines if the chasing is currently active by checking if the chasing activation time plus 14 seconds is greater than the current UTC time.
        /// </summary>
        private bool ChasingActive => (ChasingActivated + TimeSpan.FromSeconds(14)) > DateTime.UtcNow;

        /// <summary>
        /// Executes a tactic based on the role, melee status, and quest giver information.
        /// </summary>
        /// <param name="role">The role of the player in the game.</param>
        /// <param name="isMelee">A boolean indicating if the player is in melee range.</param>
        /// <param name="preventMovement">An out parameter that determines if movement should be prevented.</param>
        /// <param name="allowAttacking">An out parameter that determines if attacking should be allowed.</param>
        /// <returns>A boolean indicating if a tactic was executed.</returns>
        public bool ExecuteTactic(WowRole role, bool isMelee, out bool preventMovement, out bool allowAttacking)
        {
            preventMovement = false;
            allowAttacking = true;

            IWowUnit wowUnit = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, IckDisplayId, false);

            if (wowUnit != null)
            {
                if (wowUnit.CurrentlyCastingSpellId == 68987 || wowUnit.CurrentlyChannelingSpellId == 68987) // chasing
                {
                    ChasingActivated = DateTime.UtcNow;
                    return true;
                }
                else if (ChasingActive && wowUnit.TargetGuid == Bot.Wow.PlayerGuid && wowUnit.Position.GetDistance(Bot.Player.Position) < 7.0f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Flee, wowUnit.Position);

                    preventMovement = true;
                    allowAttacking = false;
                    return true;
                }

                IWowUnit unitOrb = Bot.Objects.All.OfType<IWowUnit>()
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault(e => e.DisplayId == 11686 && e.HasBuffById(69017) && e.Position.GetDistance(Bot.Player.Position) < 3.0f);

                if (unitOrb != null) // orbs
                {
                    Bot.Movement.SetMovementAction(MovementAction.Flee, unitOrb.Position);

                    preventMovement = true;
                    allowAttacking = false;
                    return true;
                }

                if (role == WowRole.Tank)
                {
                    if (wowUnit.TargetGuid == Bot.Wow.PlayerGuid)
                    {
                        Vector3 modifiedCenterPosition = BotUtils.MoveAhead(Area, BotMath.GetFacingAngle(Bot.Objects.CenterPartyPosition, Area), 8.0f);
                        float distanceToMid = Bot.Player.Position.GetDistance(modifiedCenterPosition);

                        if (distanceToMid > 5.0f && Bot.Player.Position.GetDistance(wowUnit.Position) < 3.5f)
                        {
                            // move the boss to mid
                            Bot.Movement.SetMovementAction(MovementAction.Move, modifiedCenterPosition);

                            preventMovement = true;
                            allowAttacking = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}