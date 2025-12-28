using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Leafs.Movement
{
    public class FollowLeaf(AmeisenBotInterfaces bot) : INode
    {
        public string Name { get; } = "Movement.Follow";

        private AmeisenBotInterfaces Bot { get; } = bot;
        private AmeisenBotConfig Config => Bot.Config;

        // Formation state - persists to avoid constant jitter
        private Vector3 _formationOffset = Vector3.Zero;
        private DateTime _nextOffsetChange = DateTime.MinValue;
        private int _assignedSlot = -1;

        // Formation angles (radians) relative to leader facing: beside left, beside right, back-left, back-right
        private static readonly float[] FormationAngles = { -1.2f, 1.2f, -2.4f, 2.4f };
        private static readonly float[] FormationDistances = { 2.0f, 2.0f, 3.0f, 3.0f };

        public BtStatus Execute()
        {
            if (Bot.Player.IsDead || Bot.Player.IsGhost || Bot.Player.IsInCombat || Config.Autopilot)
            {
                Bot.Movement.StopMovement();
                return BtStatus.Failed;
            }

            if (GetUnitToFollow(out IWowUnit unit))
            {
                // Calculate formation position
                Vector3 targetPos = GetFormationPosition(unit);
                float distance = Bot.Player.DistanceTo(targetPos);

                // Smart Mounting Logic
                if (!Bot.Player.IsMounted
                    && !Bot.Player.IsInCombat
                    && Bot.Player.IsOutdoors
                    && (unit.IsMounted || distance > 40.0f))
                {
                    if (!Bot.Player.IsCasting)
                    {
                        IEnumerable<WowMount> mounts = Bot.Character.Mounts;

                        if (Config.UseOnlySpecificMounts && !string.IsNullOrEmpty(Config.Mounts))
                        {
                            IEnumerable<string> allowedMounts = Config.Mounts.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                            mounts = mounts.Where(m => allowedMounts.Any(a => a.Equals(m.Name, StringComparison.OrdinalIgnoreCase)));
                        }

                        if (mounts != null && mounts.Any())
                        {
                            WowMount mount = mounts.ElementAt(Random.Shared.Next(mounts.Count()));
                            Bot.Wow.CallCompanion(mount.Index, "MOUNT");
                            return BtStatus.Ongoing;
                        }
                    }
                    else
                    {
                        return BtStatus.Ongoing;
                    }
                }

                if (distance > Config.MaxFollowDistance)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Follow, targetPos);
                    return BtStatus.Ongoing;
                }
                else if (distance > Config.MinFollowDistance)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Follow, targetPos);
                    return BtStatus.Ongoing;
                }
                else
                {
                    Bot.Movement.StopMovement();
                    return BtStatus.Success;
                }
            }

            return BtStatus.Failed;
        }

        /// <summary>
        /// Calculates formation position relative to leader for natural group movement.
        /// Bots walk beside/behind leader, not in single file.
        /// </summary>
        private Vector3 GetFormationPosition(IWowUnit leader)
        {
            // MULTI-FLOOR CHECK: If height difference is large, skip formation and follow directly
            // This allows pathfinding to find stairs/ramps instead of trying to reach an invalid position
            float heightDiff = MathF.Abs(Bot.Player.Position.Z - leader.Position.Z);
            if (heightDiff > 5.0f)
            {
                // Different floor - follow leader directly, let navmesh find the path
                return leader.Position;
            }

            DateTime now = DateTime.UtcNow;

            // Assign formation slot based on party position (stable)
            if (_assignedSlot < 0)
            {
                int partyIndex = GetPartyIndex();
                _assignedSlot = partyIndex % FormationAngles.Length;
            }

            // Update offset slowly (every 2-5 minutes) for organic movement
            if (now > _nextOffsetChange)
            {
                float baseAngle = FormationAngles[_assignedSlot];
                float baseDist = FormationDistances[_assignedSlot];

                // Small natural variation: ±0.3 rad angle, ±0.5m distance
                float angleJitter = (float)(Random.Shared.NextDouble() - 0.5) * 0.3f;
                float distJitter = (float)(Random.Shared.NextDouble() - 0.5) * 0.5f;

                float finalAngle = baseAngle + angleJitter;
                float finalDist = baseDist + distJitter;

                // Calculate offset relative to leader's facing (behind and to sides)
                _formationOffset = new Vector3(
                    MathF.Cos(finalAngle) * finalDist,
                    MathF.Sin(finalAngle) * finalDist,
                    0
                );

                // Change offset every 2-5 minutes for natural drift
                _nextOffsetChange = now.AddSeconds(120 + Random.Shared.Next(0, 180));
            }

            // Rotate offset based on leader's facing direction
            float leaderRot = leader.Rotation;
            float cos = MathF.Cos(leaderRot);
            float sin = MathF.Sin(leaderRot);

            Vector3 rotatedOffset = new(
                (_formationOffset.X * cos) - (_formationOffset.Y * sin),
                (_formationOffset.X * sin) + (_formationOffset.Y * cos),
                0
            );

            return leader.Position + rotatedOffset;
        }

        /// <summary>
        /// Gets this bot's index in the party for formation slot assignment.
        /// </summary>
        private int GetPartyIndex()
        {
            List<IWowUnit> partyMembers = Bot.Objects.Partymembers.ToList();
            for (int i = 0; i < partyMembers.Count; i++)
            {
                if (partyMembers[i].Guid == Bot.Player.Guid)
                {
                    return i;
                }
            }
            return 0;
        }

        private bool GetUnitToFollow(out IWowUnit playerToFollow)
        {
            IEnumerable<IWowPlayer> wowPlayers = Bot.Objects.All.OfType<IWowPlayer>().Where(e => !e.IsDead);

            if (wowPlayers.Any())
            {
                IWowUnit[] playersToTry =
                [
                    Config.FollowSpecificCharacter ? wowPlayers.FirstOrDefault(p => Bot.Db.GetUnitName(p, out string name) && name.Equals(Config.SpecificCharacterToFollow, StringComparison.OrdinalIgnoreCase)) : null,
                    Config.FollowGroupLeader ? Bot.Objects.Partyleader : null,
                    Config.FollowGroupMembers ? Bot.Objects.Partymembers.FirstOrDefault() : null
                ];

                foreach (IWowUnit unit in playersToTry)
                {
                    if (unit != null)
                    {
                        playerToFollow = unit;
                        return true;
                    }
                }
            }

            playerToFollow = null;
            return false;
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}
