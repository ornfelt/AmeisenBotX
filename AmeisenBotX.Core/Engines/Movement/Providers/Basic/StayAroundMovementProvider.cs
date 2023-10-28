﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;

/// <summary>
/// Namespace for the basic movement providers used in the AmeisenBotX.Core.Engines.Movement.Providers.Basic namespace.
/// </summary>
namespace AmeisenBotX.Core.Engines.Movement.Providers.Basic
{
    /// <summary>
    /// Implementation of the IMovementProvider interface for the StayAroundMovementProvider class.
    /// </summary>
    public class StayAroundMovementProvider : IMovementProvider
    {
        /// <summary>
        /// Initializes a new instance of the StayAroundMovementProvider class.
        /// </summary>
        /// <param name="getUnit">A function that returns a tuple containing an IWowUnit object, as well as two float values.</param>
        public StayAroundMovementProvider(Func<(IWowUnit, float, float)> getUnit)
        {
            GetUnit = getUnit;
        }

        /// <summary>
        /// Gets the function that returns a tuple containing an instance of IWowUnit, and two float values.
        /// </summary>
        public Func<(IWowUnit, float, float)> GetUnit { get; }

        /// <summary>
        /// Retrieves the position and movement action type of the unit.
        /// </summary>
        /// <param name="position">The position of the unit.</param>
        /// <param name="type">The movement action type of the unit.</param>
        /// <returns>True if the unit is valid and the position and movement action type are successfully retrieved, otherwise returns false.</returns>
        public bool Get(out Vector3 position, out MovementAction type)
        {
            (IWowUnit unit, float angle, float distance) = GetUnit();

            if (IWowUnit.IsValid(unit))
            {
                type = MovementAction.Move;
                position = BotMath.CalculatePositionAround(unit.Position, unit.Rotation, angle, distance);
                return true;
            }

            type = MovementAction.None;
            position = Vector3.Zero;
            return false;
        }
    }
}