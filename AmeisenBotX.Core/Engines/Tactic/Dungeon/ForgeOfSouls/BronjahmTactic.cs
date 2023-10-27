using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Tactic.Dungeon.ForgeOfSouls
{
    /// <summary>
    /// Initializes a new instance of the BronjahmTactic class.
    /// </summary>
    public class BronjahmTactic : ITactic
    {
        /// <summary>
        /// Initializes a new instance of the BronjahmTactic class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public BronjahmTactic(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            Configurables = new()
            {
                { "isOffTank", false },
            };
        }

        /// <summary>
        /// Gets the area represented by a Vector3 object. The area is calculated using the
        /// coordinates (5297, 2506, 686).
        /// </summary>
        public Vector3 Area { get; } = new(5297, 2506, 686);

        /// <summary>
        /// Represents the radius of the area.
        /// </summary>
        public float AreaRadius { get; } = 70.0f;

        /// <summary>
        /// Gets or sets the Bot interface for the AmeisenBot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the dictionary of configurable values.
        /// </summary>
        /// <value>
        /// The dictionary containing configurable values.  
        /// </value>
        public Dictionary<string, dynamic> Configurables { get; private set; }

        /// <summary>
        /// The MapId property represents the unique identifier of the map 'The Forge of Souls' in the 'World of Warcraft' game.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheForgeOfSouls;

        /// <summary>
        /// Gets the display ID for Bronjahm.
        /// </summary>
        private static List<int> BronjahmDisplayId { get; } = new List<int> { 30226 };

        /// <summary>
        /// Executes a tactic based on the provided parameters.
        /// </summary>
        /// <param name="role">The role of the player (Tank, DPS, or Heal).</param>
        /// <param name="isMelee">A boolean indicating whether the player is in melee range or not.</param>
        /// <param name="preventMovement">An out parameter indicating if movement should be prevented.</param>
        /// <param name="allowAttacking">An out parameter indicating if attacking is allowed.</param>
        /// <returns>A boolean value indicating if the tactic was executed successfully.</returns>
        public bool ExecuteTactic(WowRole role, bool isMelee, out bool preventMovement, out bool allowAttacking)
        {
            preventMovement = false;
            allowAttacking = true;

            IWowUnit wowUnit = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, BronjahmDisplayId, false);

            if (wowUnit != null)
            {
                if (wowUnit.CurrentlyCastingSpellId == 68872 || wowUnit.CurrentlyChannelingSpellId == 68872 || wowUnit.HasBuffById(68872)) // soulstorm
                {
                    if (Bot.Player.Position.GetDistance(Area) > 8.0)
                    {
                        Bot.Movement.SetMovementAction(MovementAction.Move, BotUtils.MoveAhead(Area, BotMath.GetFacingAngle(Bot.Player.Position, Area), -5.0f));

                        preventMovement = true;
                        allowAttacking = true;
                        return true;
                    }

                    // stay at the mid
                    return false;
                }

                if (role == WowRole.Tank)
                {
                    if (wowUnit.TargetGuid == Bot.Wow.PlayerGuid)
                    {
                        Vector3 modifiedCenterPosition = BotUtils.MoveAhead(Area, BotMath.GetFacingAngle(Bot.Objects.CenterPartyPosition, Area), 8.0f);
                        float distanceToMid = Bot.Player.Position.GetDistance(modifiedCenterPosition);

                        // flee from the corrupted souls target
                        bool needToFlee = wowUnit.CurrentlyChannelingSpellId == 68839
                            || Bot.Objects.All.OfType<IWowUnit>().Any(e => e.DisplayId == 30233 && e.IsInCombat);

                        if (needToFlee)
                        {
                            if (distanceToMid < 16.0f)
                            {
                                Bot.Movement.SetMovementAction(MovementAction.Flee, modifiedCenterPosition);

                                preventMovement = true;
                                allowAttacking = false;
                                return true;
                            }

                            // we cant run away further
                            preventMovement = true;
                            return false;
                        }

                        if (distanceToMid > 5.0f && Bot.Player.Position.GetDistance(wowUnit.Position) < 3.5)
                        {
                            // move the boss to mid
                            Bot.Movement.SetMovementAction(MovementAction.Move, modifiedCenterPosition);

                            preventMovement = true;
                            allowAttacking = false;
                            return true;
                        }
                    }
                }
                else if (role == WowRole.Dps || role == WowRole.Heal)
                {
                    float distanceToMid = Bot.Player.Position.GetDistance(Area);

                    if (!isMelee && distanceToMid < 20.0f)
                    {
                        // move to the outer ring of the arena
                        Bot.Movement.SetMovementAction(MovementAction.Move, BotUtils.MoveAhead(Area, BotMath.GetFacingAngle(Bot.Player.Position, Area), -22.0f));

                        preventMovement = true;
                        allowAttacking = false;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}