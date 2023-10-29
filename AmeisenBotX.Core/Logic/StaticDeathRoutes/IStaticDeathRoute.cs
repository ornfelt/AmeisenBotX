﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects.Enums;

/// <summary>
/// Provides classes and interfaces for managing static death routes in AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Core.Logic.StaticDeathRoutes
{
    /// <summary>
    /// Represents an interface for defining a static death route in a game world.
    /// </summary>
    public interface IStaticDeathRoute
    {
        /// <summary>
        /// Get the next point from the path.
        /// </summary>
        /// <param name="playerPosition">Current position</param>
        /// <returns>Next point</returns>
        Vector3 GetNextPoint(Vector3 playerPosition);

        /// <summary>
        /// Call this once when you start using the static route.
        /// </summary>
        /// <param name="playerPosition">Current position</param>
        void Init(Vector3 playerPosition);

        /// <summary>
        /// Returns whether the route can be used based othe the map id, and start / end.
        /// </summary>
        /// <param name="mapId">Current map id</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <returns>True when the path is useable, false if not</returns>
        bool IsUseable(WowMapId mapId, Vector3 start, Vector3 end);
    }
}