using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Core.Engines.Movement.Providers.Basic
{
    /// <summary>
    /// Gets or sets the AmeisenBotInterfaces object representing the bot.
    /// </summary>
    public class SimpleCombatMovementProvider : IMovementProvider
    {
        /// <summary>
        /// Initializes a new instance of the SimpleCombatMovementProvider class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object to be used by the SimpleCombatMovementProvider.</param>
        public SimpleCombatMovementProvider(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces object representing the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Returns a boolean value indicating if the bot should perform a movement action and assigns the position and type of movement if true.
        /// </summary>
        /// <param name="position">The position to move to if a movement action is required.</param>
        /// <param name="type">The type of movement action to perform if a movement action is required.</param>
        /// <returns>Returns true if a movement action is required and assigns the position and type of movement, otherwise returns false and assigns default values.</returns>
        public bool Get(out Vector3 position, out MovementAction type)
        {
            if (Bot.CombatClass != null
                && !Bot.CombatClass.HandlesMovement
                && IWowUnit.IsValidAliveInCombat(Bot.Player)
                && IWowUnit.IsValidAlive(Bot.Target)
                && !Bot.Player.IsGhost)
            {
                float distance = Bot.Player.DistanceTo(Bot.Target);

                switch (Bot.CombatClass.Role)
                {
                    case WowRole.Dps:
                        if (Bot.CombatClass.IsMelee)
                        {
                            if (distance > Bot.Player.MeleeRangeTo(Bot.Target))
                            {
                                position = Bot.Target.Position;
                                type = MovementAction.Chase;
                                return true;
                            }
                        }
                        else
                        {
                            if (distance > 26.5f + Bot.Target.CombatReach)
                            {
                                position = Bot.Target.Position;
                                type = MovementAction.Chase;
                                return true;
                            }
                        }
                        break;

                    case WowRole.Heal:
                        if (distance > 26.5f + Bot.Target.CombatReach)
                        {
                            position = Bot.Target.Position;
                            type = MovementAction.Chase;
                            return true;
                        }
                        break;

                    case WowRole.Tank:
                        if (distance > Bot.Player.MeleeRangeTo(Bot.Target))
                        {
                            position = Bot.Target.Position;
                            type = MovementAction.Chase;
                            return true;
                        }
                        break;
                }
            }

            type = MovementAction.None;
            position = Vector3.Zero;
            return false;
        }
    }
}