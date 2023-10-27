using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Tactic.Dungeon.ForgeOfSouls
{
    public class DevourerOfSoulsTactic : ITactic
    {
        /// <summary>
        /// Initializes a new instance of the DevourerOfSoulsTactic class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        public DevourerOfSoulsTactic(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            Configurables = new()
            {
                { "isOffTank", false },
            };
        }

        /// <summary>
        /// Gets the area represented by a Vector3 object with X, Y, and Z coordinates of 5662, 2507, and 709, respectively.
        /// </summary>
        public Vector3 Area { get; } = new(5662, 2507, 709);

        /// <summary>
        /// Gets the area radius.
        /// </summary>
        public float AreaRadius { get; } = 120.0f;

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces instance.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the dictionary of configurable values.
        /// </summary>
        public Dictionary<string, dynamic> Configurables { get; private set; }

        /// <summary>
        /// Gets the map ID for the Forge of Souls.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheForgeOfSouls;

        /// <summary>
        /// Gets the display IDs for the Devourer of Souls.
        /// </summary>
        private static List<int> DevourerOfSoulsDisplayId { get; } = new() { 30148, 30149, 30150 };

        /// <summary>
        /// Executes a tactic based on the given role and melee status, updating the preventMovement and allowAttacking variables.
        /// </summary>
        /// <param name="role">The role of the player.</param>
        /// <param name="isMelee">True if the player is a melee character, false otherwise.</param>
        /// <param name="preventMovement">A variable indicating if movement should be prevented.</param>
        /// <param name="allowAttacking">A variable indicating if attacking is allowed.</param>
        /// <returns>True if a tactic was executed, false otherwise.</returns>
        public bool ExecuteTactic(WowRole role, bool isMelee, out bool preventMovement, out bool allowAttacking)
        {
            preventMovement = false;
            allowAttacking = true;

            IWowUnit wowUnit = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, DevourerOfSoulsDisplayId, false);

            if (wowUnit != null)
            {
                if (wowUnit.DisplayId == 30150)
                {
                    // make sure we avoid the lazer we only care about being on the reight side of
                    // him because the lazer spins clockwise
                    float angleDiff = BotMath.GetAngleDiff(wowUnit.Position, wowUnit.Rotation, Bot.Player.Position);

                    if (angleDiff < 0.5f)
                    {
                        Bot.Movement.SetMovementAction(MovementAction.Move, BotMath.CalculatePositionAround(wowUnit.Position, wowUnit.Rotation, MathF.PI, isMelee ? 5.0f : 22.0f));

                        preventMovement = true;
                        allowAttacking = false;
                        return true;
                    }
                }

                if (role == WowRole.Tank)
                {
                    Vector3 modifiedCenterPosition = BotUtils.MoveAhead(Area, BotMath.GetFacingAngle(Bot.Objects.CenterPartyPosition, Area), 8.0f);
                    float distanceToMid = Bot.Player.Position.GetDistance(modifiedCenterPosition);

                    if (wowUnit.TargetGuid == Bot.Wow.PlayerGuid)
                    {
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
            }

            return false;
        }
    }
}