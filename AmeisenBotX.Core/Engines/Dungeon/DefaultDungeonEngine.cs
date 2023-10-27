using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Core.Engines.Dungeon.Profiles;
using AmeisenBotX.Core.Engines.Dungeon.Profiles.Classic;
using AmeisenBotX.Core.Engines.Dungeon.Profiles.TBC;
using AmeisenBotX.Core.Engines.Dungeon.Profiles.WotLK;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Dungeon
{
    /// <summary>
    /// Constructs a DefaultDungeonEngine object with the provided bot and config.
    /// </summary>
    /// <param name="bot">The AmeisenBotInterfaces object to use for the dungeon engine.</param>
    /// <param name="config">The AmeisenBotConfig object to use for the dungeon engine.</param>
    public class DefaultDungeonEngine : IDungeonEngine
    {
        /// <summary>
        /// Constructs a DefaultDungeonEngine object with the provided bot and config.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object to use for the dungeon engine.</param>
        /// <param name="config">The AmeisenBotConfig object to use for the dungeon engine.</param>
        public DefaultDungeonEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;

            CurrentNodes = new();
            ExitDungeonEvent = new(TimeSpan.FromMilliseconds(1000));
            InteractionEvent = new(TimeSpan.FromMilliseconds(1000));

            RootSelector = new
            (
                () => Progress == 100.0,
                new Leaf(ExitDungeon),
                new Selector
                (
                    () => IDied,
                    new Sequence
                    (
                        new Leaf(() => MoveToPosition(DeathPosition)),
                        new Leaf(() =>
                        {
                            IDied = false;
                            return BtStatus.Success;
                        })
                    ),
                    new Selector
                    (
                        () => Bot.Objects.Partyleader == null || Bot.Objects.Partyleader.Guid == Bot.Wow.PlayerGuid || !Bot.Objects.PartymemberGuids.Any(),
                        new Selector
                        (
                            () => AreAllPlayersPresent(20.0f, 14.0f),
                            new Selector
                            (
                                () => Bot.Objects.Partymembers.Any(e => e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Food") || e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Drink")),
                                new Leaf(() => { return BtStatus.Success; }),
                                new Leaf(() => FollowNodePath())
                            ),
                            new Leaf(() => { return BtStatus.Success; })
                        ),
                        new Selector
                        (
                            () => Bot.Objects.Partyleader != null,
                            new Leaf(() => MoveToPosition(Bot.Objects.Partyleader.Position + LeaderFollowOffset, 0.0f, MovementAction.Follow)),
                            new Leaf(() => { return BtStatus.Success; })
                        )
                    )
                )
            );

            BehaviorTree = new
            (
                RootSelector
            );
        }

        ///<inheritdoc cref="IDungeonEngine.Nodes"/>
        public List<DungeonNode> Nodes => CurrentNodes?.ToList();

        ///<inheritdoc cref="IDungeonEngine.Profile"/>
        public IDungeonProfile Profile { get; private set; }

        /// <summary>
        /// Gets or sets the BehaviorTree property.
        /// </summary>
        private Tree BehaviorTree { get; }

        /// <summary>
        /// Gets the Bot object.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the private AmeisenBotConfig property.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets the current nodes in the dungeon.
        /// </summary>
        private Queue<DungeonNode> CurrentNodes { get; set; }

        /// <summary>
        /// Gets or sets the position where the object was last declared dead.
        /// </summary>
        private Vector3 DeathPosition { get; set; }

        /// <summary>
        /// Represents the time-gated event for exiting the dungeon.
        /// </summary>
        private TimegatedEvent ExitDungeonEvent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has died.
        /// </summary>
        private bool IDied { get; set; }

        /// <summary>
        /// Gets or sets the time-gated InteractionEvent.
        /// </summary>
        private TimegatedEvent InteractionEvent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the program is currently waiting for a group.
        /// </summary>
        private bool IsWaitingForGroup { get; set; }

        /// <summary>
        /// Gets or sets the offset for the follower from the leader's position.
        /// </summary>
        private Vector3 LeaderFollowOffset { get; set; }

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        private double Progress { get; set; }

        /// <summary>
        /// Gets the root selector for the code.
        /// </summary>
        private Selector RootSelector { get; }

        ///<inheritdoc cref="IDungeonEngine.Execute"/>
        public void Execute()
        {
            if (Profile != null)
            {
                if (Bot.Objects.MapId != Profile.MapId)
                {
                    Profile = null;
                }

                BehaviorTree.Tick();
            }
            else
            {
                Random rnd = new();

                LeaderFollowOffset = new()
                {
                    X = ((float)rnd.NextDouble() * (5.0f * 2)) - 5.0f,
                    Y = ((float)rnd.NextDouble() * (5.0f * 2)) - 5.0f,
                    Z = 0f
                };

                LoadProfile(TryGetProfileByMapId(Bot.Objects.MapId));
            }
        }

        ///<inheritdoc cref="IDungeonEngine.OnDeath"/>
        public void OnDeath()
        {
            IDied = true;
            DeathPosition = Bot.Player.Position;
        }

        ///<inheritdoc cref="IDungeonEngine.TryGetProfileByMapId(WowMapId)"/>
        public IDungeonProfile TryGetProfileByMapId(WowMapId mapId)
        {
            return mapId switch
            {
                WowMapId.RagefireChasm => new RagefireChasmProfile(),
                WowMapId.WailingCaverns => new WailingCavernsProfile(),
                WowMapId.Deadmines => new DeadminesProfile(),
                WowMapId.ShadowfangKeep => new ShadowfangKeepProfile(),
                WowMapId.StormwindStockade => new StockadeProfile(),

                WowMapId.HellfireRamparts => new HellfireRampartsProfile(),
                WowMapId.TheBloodFurnace => new TheBloodFurnaceProfile(),
                WowMapId.TheSlavePens => new TheSlavePensProfile(),
                WowMapId.TheUnderbog => new TheUnderbogProfile(),
                WowMapId.TheSteamvault => new TheSteamvaultProfile(),

                WowMapId.UtgardeKeep => new UtgardeKeepProfile(),
                WowMapId.AzjolNerub => new AzjolNerubProfile(),
                WowMapId.HallsOfLighting => new HallsOfLightningProfile(),
                WowMapId.TheForgeOfSouls => new ForgeOfSoulsProfile(),
                WowMapId.PitOfSaron => new PitOfSaronProfile(),

                _ => null
            };
        }

        /// <summary>
        /// Checks if all players are present within a certain distance and if it's time to start running.
        /// Returns true if all players are present or if the bot is waiting for the group to catch up, false otherwise.
        /// </summary>
        /// <param name="distance">The distance within which to check for nearby players.</param>
        /// <param name="distanceToStartRunning">The distance at which the bot should start running.</param>
        /// <returns>True if all players are present or if the bot is waiting for the group to catch up, false otherwise.</returns>
        private bool AreAllPlayersPresent(float distance, float distanceToStartRunning)
        {
            if (!Bot.Objects.Partymembers.Any())
            {
                return true;
            }

            if (IsWaitingForGroup)
            {
                distance = distanceToStartRunning;
            }

            int nearPlayers = Bot.GetNearPartyMembers<IWowPlayer>(Bot.Player.Position, distance).Count(e => !e.IsDead);

            if (nearPlayers >= Bot.Objects.Partymembers.Count() - 1)
            {
                IsWaitingForGroup = false;
                return true;
            }
            else
            {
                IsWaitingForGroup = true;
                return false;
            }
        }

        /// <summary>
        /// Function to exit the current dungeon.
        /// If the ExitDungeonEvent returns true, checks if the bot is in a LFG group.
        /// If it is, uses a Lua string to teleport using the LFGTeleport function.
        /// If it is not, moves the bot to the position of the first node in the current profile.
        /// Returns BtStatus.Success.
        /// </summary>
        private BtStatus ExitDungeon()
        {
            if (ExitDungeonEvent.Run())
            {
                if (Bot.Wow.IsInLfgGroup())
                {
                    Bot.Wow.LuaDoString("LFGTeleport(true);");
                }
                else
                {
                    MoveToPosition(Profile.Nodes.First().Position);
                }
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Follows the path defined by the CurrentNodes and performs actions based on the type of each node.
        /// If the player is currently casting, returns BtStatus.Ongoing.
        /// If the player is within range of the current node and the node is of type Use or Door, interacts with the nearest
        /// game object that meets the criteria and returns BtStatus.Ongoing.
        /// If the node is of type Jump, the character jumps.
        /// If the node is of type Collect and the character does not have the item specified in the Extra field of the node,
        /// interacts with the nearest game object that meets the criteria and loots everything. If the character's inventory is
        /// full, deletes the most worthless item according to predetermined criteria. Returns BtStatus.Ongoing.
        /// If none of the above conditions are met, moves the character to the position of the current node with a specified
        /// distance threshold and updates the CurrentNodes queue accordingly. Returns the status of the movement.
        /// If the CurrentNodes queue is empty, moves the character to the dungeon exit position with a specified distance
        /// threshold and returns the status of the movement.
        /// </summary>
        /// <returns>The status of the behavior tree node.</returns>
        private BtStatus FollowNodePath()
        {
            if (CurrentNodes.Any())
            {
                if (Bot.Player.IsCasting)
                {
                    return BtStatus.Ongoing;
                }

                DungeonNode node = CurrentNodes.Peek();

                if (node.Position.GetDistance(Bot.Player.Position) < 4.0f)
                {
                    if (node.Type == DungeonNodeType.Use
                        || node.Type == DungeonNodeType.Door)
                    {
                        IWowGameobject nearestGameobject = Bot.Objects.All.OfType<IWowGameobject>()
                            .OrderBy(e => e.Position.GetDistance(node.Position))
                            .FirstOrDefault();

                        if (nearestGameobject.Position.GetDistance(node.Position) < 5.0f && nearestGameobject != null && nearestGameobject.Bytes0 != 0)
                        {
                            if (InteractionEvent.Run())
                            {
                                Bot.Movement.Reset();
                                Bot.Wow.StopClickToMove();

                                Bot.Wow.InteractWithObject(nearestGameobject);
                            }

                            return BtStatus.Ongoing;
                        }
                    }
                    else if (node.Type == DungeonNodeType.Jump)
                    {
                        Bot.Character.Jump();
                    }
                    else if (node.Type == DungeonNodeType.Collect)
                    {
                        if (!Bot.Character.Inventory.HasItemByName(node.Extra))
                        {
                            IWowGameobject nearestGameobject = Bot.Objects.All.OfType<IWowGameobject>()
                                .OrderBy(e => e.Position.GetDistance(node.Position))
                                .FirstOrDefault();

                            if (nearestGameobject.Position.GetDistance(node.Position) < 5.0f)
                            {
                                if (Bot.Character.Inventory.FreeBagSlots == 0)
                                {
                                    // delete the most worthless item
                                    IWowInventoryItem itemToDelete = Bot.Character.Inventory.Items
                                        .Where(e => !Config.ItemSellBlacklist.Contains(e.Name))
                                        .OrderBy(e => e.ItemQuality).ThenBy(e => e.Price)
                                        .FirstOrDefault();

                                    if (itemToDelete != null)
                                    {
                                        Bot.Wow.DeleteItemByName(itemToDelete.Name);
                                    }
                                }

                                if (nearestGameobject != null && nearestGameobject.Bytes0 != 0)
                                {
                                    if (InteractionEvent.Run())
                                    {
                                        Bot.Movement.Reset();
                                        Bot.Wow.StopClickToMove();

                                        Bot.Wow.InteractWithObject(nearestGameobject);
                                        Bot.Wow.LootEverything();
                                    }

                                    return BtStatus.Ongoing;
                                }
                            }

                            return BtStatus.Ongoing;
                        }
                    }
                }

                BtStatus status = MoveToPosition(node.Position, 3.0f);

                if (status == BtStatus.Success)
                {
                    CurrentNodes.Dequeue();
                }

                return status;
            }
            else
            {
                return MoveToPosition(Profile.DungeonExit, 2.5f);
            }
        }

        /// <summary>
        /// Loads the specified dungeon profile.
        /// </summary>
        /// <param name="profile">The dungeon profile to load.</param>
        private void LoadProfile(IDungeonProfile profile)
        {
            Profile = profile;

            if (Profile != null)
            {
                DungeonNode closestNode = profile.Nodes.OrderBy(e => e.Position.GetDistance(Bot.Player.Position)).FirstOrDefault();
                int closestNodeIndex = profile.Nodes.IndexOf(closestNode);

                for (int i = closestNodeIndex; i < profile.Nodes.Count; ++i)
                {
                    CurrentNodes.Enqueue(profile.Nodes[i]);
                }

                if (Bot.CombatClass != null)
                {
                    Bot.CombatClass.PriorityTargetDisplayIds = profile.PriorityUnits;
                }
            }
        }

        /// <summary>
        /// Moves the bot to a specified position.
        /// </summary>
        /// <param name="position">The position to move to.</param>
        /// <param name="minDistance">The minimum distance required before considering the movement complete.</param>
        /// <param name="movementAction">The type of movement action to perform.</param>
        /// <returns>The status of the movement.</returns>
        private BtStatus MoveToPosition(Vector3 position, float minDistance = 2.5f, MovementAction movementAction = MovementAction.Move)
        {
            float distance = Bot.Player.Position.GetDistance(position);

            if (distance > minDistance)
            {
                Bot.Movement.SetMovementAction(movementAction, position);
                return BtStatus.Ongoing;
            }
            else
            {
                return BtStatus.Success;
            }
        }
    }
}