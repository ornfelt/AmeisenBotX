using AmeisenBotX.Wow.Objects;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Helpers
{
    /// <summary>
    /// Helper to determine if the party is currently moving, with hysteresis to prevent rapid state flipping.
    /// Used by decision modules to pause looting/gathering when catching up to the group is priority.
    /// </summary>
    public class PartyMovementMonitor
    {
        private readonly AmeisenBotInterfaces Bot;

        private bool _isPartyMovingLatch = false;

        public PartyMovementMonitor(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Check if party is considered "moving away" or "running".
        /// Returns true if we should prioritize catching up over stationary tasks (loot/gather).
        /// </summary>
        public bool IsPartyMoving()
        {
            if (!Bot.Objects.Partymembers.Any())
            {
                return false; // Solo player is never "left behind" by party
            }

            IWowUnit leader = Bot.Objects.Partyleader ?? Bot.Objects.Partymembers.FirstOrDefault();
            if (leader == null)
            {
                return false;
            }

            float distanceToLeader = leader.DistanceTo(Bot.Player);
            float partyDistance = Bot.Objects.CenterPartyPosition.GetDistance(Bot.Player.Position);

            // Assume leader is "running" away if they are far enough and not casting/staying still
            // We don't have IsMoving directly on IWowUnit apparently, instead we rely on distance urgency
            bool leaderRunning = !leader.IsCasting && distanceToLeader > 25f;

            // Hysteresis logic
            if (_isPartyMovingLatch)
            {
                // We are currently in "Party Moving" mode
                // Switch back to "Stationary" only if we are CLOSE enough to catch up
                if (distanceToLeader < TimingConfig.PartyMovingLeaderDistanceStop &&
                    partyDistance < TimingConfig.PartyMovingCenterDistanceStop)
                {
                    _isPartyMovingLatch = false;
                }
            }
            else
            {
                // We are in "Stationary" mode
                // Switch to "Party Moving" if leader gets too far
                if ((leaderRunning && distanceToLeader > TimingConfig.PartyMovingLeaderDistanceStart) ||
                    partyDistance > TimingConfig.PartyMovingCenterDistanceStart)
                {
                    _isPartyMovingLatch = true;
                }
            }

            return _isPartyMovingLatch;
        }

        public void Reset()
        {
            _isPartyMovingLatch = false;
        }
    }
}
