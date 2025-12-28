using System;

namespace AmeisenBotX.Core.Logic
{
    /// <summary>
    /// Centralized timing configuration for all bot operations.
    /// Uses consistent jitter patterns for humanization while maintaining predictability.
    /// </summary>
    public static class TimingConfig
    {
        // ============================================
        // UPDATE INTERVALS (base + random jitter)
        // ============================================

        /// <summary>
        /// Combat state update interval (party combat detection).
        /// ~80-120ms for responsive combat entry.
        /// </summary>
        public static TimeSpan CombatStateUpdate => TimeSpan.FromMilliseconds(80 + Random.Shared.Next(40));

        /// <summary>
        /// Loot check interval (scanning for lootable units).
        /// ~400-600ms for moderate responsiveness.
        /// </summary>
        public static TimeSpan LootCheck => TimeSpan.FromMilliseconds(400 + Random.Shared.Next(200));

        /// <summary>
        /// Gobject/gathering check interval.
        /// ~400-600ms for moderate responsiveness.
        /// </summary>
        public static TimeSpan GobjectCheck => TimeSpan.FromMilliseconds(400 + Random.Shared.Next(200));

        /// <summary>
        /// Eat/drink action interval.
        /// ~200-300ms for action execution.
        /// </summary>
        public static TimeSpan EatAction => TimeSpan.FromMilliseconds(200 + Random.Shared.Next(100));

        /// <summary>
        /// Loot try interval (how often to retry interacting with corpse).
        /// ~400-600ms between attempts.
        /// </summary>
        public static TimeSpan LootTryInterval => TimeSpan.FromMilliseconds(400 + Random.Shared.Next(200));

        /// <summary>
        /// Inventory update throttle (BAG_UPDATE event response).
        /// ~800-1200ms to prevent spam.
        /// </summary>
        public static TimeSpan InventoryUpdateThrottle => TimeSpan.FromMilliseconds(800 + Random.Shared.Next(400));

        /// <summary>
        /// Training state update throttle.
        /// ~1600-2400ms for infrequent checks.
        /// </summary>
        public static TimeSpan TrainUpdateThrottle => TimeSpan.FromMilliseconds(1600 + Random.Shared.Next(800));

        /// <summary>
        /// Line of sight check interval.
        /// ~900-1100ms for expensive raycasts.
        /// </summary>
        public static TimeSpan LineOfSightCheck => TimeSpan.FromMilliseconds(900 + Random.Shared.Next(200));

        /// <summary>
        /// AoE avoidance check interval.
        /// ~400-600ms for responsive escape.
        /// </summary>
        public static TimeSpan AoECheck => TimeSpan.FromMilliseconds(400 + Random.Shared.Next(200));

        /// <summary>
        /// Spread from allies check interval.
        /// ~1800-2200ms for positioning optimization.
        /// </summary>
        public static TimeSpan SpreadCheck => TimeSpan.FromMilliseconds(1800 + Random.Shared.Next(400));

        /// <summary>
        /// Path validation interval.
        /// ~200-300ms for movement validation.
        /// </summary>
        public static TimeSpan PathValidation => TimeSpan.FromMilliseconds(200 + Random.Shared.Next(100));

        /// <summary>
        /// Character update interval (stats, equipment).
        /// ~5000ms for infrequent full updates.
        /// </summary>
        public static TimeSpan CharacterUpdate => TimeSpan.FromMilliseconds(5000);

        // ============================================
        // TIMEOUTS (maximum allowed durations)
        // ============================================

        /// <summary>
        /// Maximum time to attempt looting a single unit.
        /// </summary>
        public static TimeSpan LootTimeout => TimeSpan.FromSeconds(25);

        /// <summary>
        /// Maximum time spent on a single gobject before blacklisting.
        /// </summary>
        public static TimeSpan GobjectTimeout => TimeSpan.FromSeconds(15);

        /// <summary>
        /// Maximum time loot window can be open before force-closing.
        /// </summary>
        public static TimeSpan LootWindowStuckTimeout => TimeSpan.FromSeconds(3);

        /// <summary>
        /// Stuck detection threshold - if no movement in this time, considered stuck.
        /// </summary>
        public static TimeSpan StuckDetectionThreshold => TimeSpan.FromSeconds(2);

        /// <summary>
        /// Movement blocked duration after stuck detection.
        /// </summary>
        public static TimeSpan StuckBlockDuration => TimeSpan.FromSeconds(5);

        /// <summary>
        /// Blacklist duration for failed gobjects.
        /// </summary>
        public static TimeSpan GobjectBlacklistDuration => TimeSpan.FromMinutes(5);

        /// <summary>
        /// Blacklist duration for failed paths.
        /// </summary>
        public static TimeSpan PathBlacklistDuration => TimeSpan.FromMinutes(5);

        /// <summary>
        /// Maximum time to wait for WoW window to appear after launch.
        /// </summary>
        public static TimeSpan WowWindowWaitTimeout => TimeSpan.FromSeconds(30);

        /// <summary>
        /// Maximum time to wait for WoW login screen to load.
        /// </summary>
        public static TimeSpan WowLoginWaitTimeout => TimeSpan.FromSeconds(60);

        // ============================================
        // HYSTERESIS (minimum state durations)
        // ============================================

        /// <summary>
        /// Minimum time to stay in looting state before transitioning.
        /// </summary>
        public static TimeSpan MinLootingDuration => TimeSpan.FromSeconds(1.5);

        /// <summary>
        /// Minimum time to stay in eating state before transitioning.
        /// </summary>
        public static TimeSpan MinEatingDuration => TimeSpan.FromSeconds(3.0);

        /// <summary>
        /// Minimum time to stay in gathering state before transitioning.
        /// </summary>
        public static TimeSpan MinGatheringDuration => TimeSpan.FromSeconds(2.0);

        /// <summary>
        /// Eat block duration after aborting eat (prevents immediate re-eat).
        /// </summary>
        public static TimeSpan EatBlockDuration => TimeSpan.FromSeconds(25 + Random.Shared.Next(10));

        // ============================================
        // DISTANCES (interaction/movement thresholds)
        // ============================================

        /// <summary>
        /// WoW's actual loot range.
        /// </summary>
        public const float LootRange = 4.0f;

        /// <summary>
        /// Radius around group center for gathering.
        /// </summary>
        public const float GroupCollectRadius = 30f;

        /// <summary>
        /// Radius around player for solo gathering.
        /// </summary>
        public const float SoloCollectRadius = 45f;

        /// <summary>
        /// Distance at which another player is considered competing for a node.
        /// </summary>
        public const float CompetingRadius = 8f;

        /// <summary>
        /// Distance to escape from AoE effects.
        /// </summary>
        public const float AoEEscapeDistance = 8f;

        /// <summary>
        /// Minimum distance to spread from allies in combat.
        /// </summary>
        public const float SpreadMinDistance = 6f;

        // ============================================
        // PARTY MOVEMENT HYSTERESIS
        // ============================================

        /// <summary>
        /// Leader distance that triggers "party moving" state.
        /// </summary>
        public const float PartyMovingLeaderDistanceStart = 20f;

        /// <summary>
        /// Leader distance that resets "party moving" state (must be < Start for hysteresis).
        /// </summary>
        public const float PartyMovingLeaderDistanceStop = 10f;

        /// <summary>
        /// Party center distance that triggers "party moving" state.
        /// </summary>
        public const float PartyMovingCenterDistanceStart = 25f;

        /// <summary>
        /// Party center distance that resets "party moving" state.
        /// </summary>
        public const float PartyMovingCenterDistanceStop = 15f;
    }
}
