using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Movement.Pathfinding.Objects;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    public class KillAndLootQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the KillAndLootQuestObjective class with the specified parameters.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance for performing bot actions.</param>
        /// <param name="npcIds">The list of NPC IDs to target for the quest objective.</param>
        /// <param name="collectOrKillAmount">The amount of NPCs to collect or kill for the quest objective.</param>
        /// <param name="questItemId">The quest item ID required for completing the quest objective.</param>
        /// <param name="areas">The list of search areas where the NPCs can be found.</param>
        public KillAndLootQuestObjective(AmeisenBotInterfaces bot, List<int> npcIds, int collectOrKillAmount, int questItemId, List<List<Vector3>> areas)
        {
            Bot = bot;
            NpcIds = npcIds;
            CollectOrKillAmount = collectOrKillAmount;
            QuestItemId = questItemId;
            SearchAreas = new SearchAreaEnsamble(areas);

            Bot.CombatLog.OnPartyKill += OnPartyKill;
        }

        /// <summary>
        /// Removes the event handler for handling party kills from the CombatLog.
        /// </summary>
        ~KillAndLootQuestObjective()
        {
            Bot.CombatLog.OnPartyKill -= OnPartyKill;
        }

        /// <summary>
        /// Checks if the progress is approximately equal to 100.
        /// </summary>
        public bool Finished => Math.Abs(Progress - 100.0f) < 0.00001;

        /// <summary>
        /// Gets the progress of the quest.
        /// </summary>
        /// <returns>The progress of the quest as a double value, ranging from 0.0 to 100.0.</returns>
        public double Progress
        {
            get
            {
                if (CollectOrKillAmount == 0)
                {
                    return 100.0;
                }

                int amount = Killed;
                if (CollectQuestItem)
                {
                    IWowInventoryItem inventoryItem =
                        Bot.Character.Inventory.Items.Find(item => item.Id == QuestItemId);

                    if (inventoryItem != null)
                    {
                        amount = inventoryItem.Count;
                    }
                    else
                    {
                        return 0.0;
                    }
                }

                return Math.Min(100.0 * ((float)amount) / ((float)CollectOrKillAmount), 100.0);
            }
        }

        /// <summary>
        /// Gets the locations of the vendors.
        /// </summary>
        public List<Vector3> VendorsLocation { get; }

        /// <summary>
        /// Gets the instance of the Bot class that implements the private AmeisenBotInterfaces interface.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the amount of CollectOrKill.
        /// </summary>
        private int CollectOrKillAmount { get; }

        /// <summary>
        /// Determines if a quest item has been collected.
        /// </summary>
        private bool CollectQuestItem => QuestItemId > 0;

        /// <summary>
        /// Gets or sets the current spot in three-dimensional space.
        /// </summary>
        private Vector3 CurrentSpot { get; set; }

        /// <summary>
        /// Gets or sets the instance of the IWowUnit interface.
        /// </summary>
        private IWowUnit IWowUnit { get; set; }

        /// <summary>
        /// Gets or sets the number of times the object has been killed.
        /// </summary>
        private int Killed { get; set; }

        /// <summary>
        /// Gets or sets the last time the unit was checked in Coordinated Universal Time (UTC).
        /// </summary>
        private DateTime LastUnitCheck { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the list of integer values representing NPC identifiers.
        /// </summary>
        private List<int> NpcIds { get; }

        /// <summary>
        /// Gets or sets the quest item ID.
        /// </summary>
        private int QuestItemId { get; }

        /// <summary>
        /// Gets the SearchAreaEnsamble property.
        /// </summary>
        private SearchAreaEnsamble SearchAreas { get; }

        /// <summary>
        /// Executes the logic for the current action. 
        /// Checks if the action is finished or if the bot player is currently casting, and returns if so. 
        /// Checks if the bot player is not in combat and if the specified time since the last unit check has passed. 
        /// Finds nearby non-dead enemy units that are attackable and hostile to the bot player, and orders them based on their distance to the bot player. 
        /// If there are enemies in the path, sets the nearest one as the target. 
        /// Changes the target to the specified unit if it exists. 
        /// Notifies the search areas of detour. 
        /// Attacks the target if it exists. 
        /// If the target does not exist or if the bot player is close to the current spot, has aborted the path, or is not currently in any movement action, sets the current spot to the next position dictated by the search areas and sets the movement action to move towards the current spot.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            if (!Bot.Player.IsInCombat && DateTime.UtcNow.Subtract(LastUnitCheck).TotalMilliseconds >= 1250.0)
            {
                LastUnitCheck = DateTime.UtcNow;
                IWowUnit = Bot.Objects.All
                    .OfType<IWowUnit>()
                    .Where(e => !e.IsDead && NpcIds.Contains(BotUtils.GuidToNpcId(e.Guid)) && !e.IsNotAttackable
                                && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly)
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .Take(3)
                    .OrderBy(e => Bot.Player.DistanceTo(e))
                    .FirstOrDefault();

                // Kill enemies in the path
                if (IWowUnit != null && Bot.Db.GetReaction(Bot.Player, IWowUnit) == WowUnitReaction.Hostile)
                {
                    IEnumerable<Vector3> path = Bot.PathfindingHandler.GetPath((int)Bot.Objects.MapId,
                    Bot.Player.Position, IWowUnit.Position);

                    if (path != null)
                    {
                        IEnumerable<IWowUnit> nearEnemies = Bot.GetEnemiesInPath<IWowUnit>(path, 10.0f);

                        if (nearEnemies.Any())
                        {
                            IWowUnit = nearEnemies.FirstOrDefault();
                        }
                    }
                }

                if (IWowUnit != null)
                {
                    Bot.Wow.ChangeTarget(IWowUnit.Guid);
                }
            }

            if (IWowUnit != null)
            {
                SearchAreas.NotifyDetour();
                Bot.CombatClass.AttackTarget();
            }
            else if (Bot.Player.Position.GetDistance(CurrentSpot) < 3.0f || SearchAreas.HasAbortedPath() || Bot.Movement.Status == MovementAction.None)
            {
                CurrentSpot = SearchAreas.GetNextPosition(Bot);
                Bot.Movement.SetMovementAction(MovementAction.Move, CurrentSpot);
            }
        }

        /// <summary>
        /// Event handler for when a party member kills an NPC.
        /// </summary>
        /// <param name="sourceGuid">The GUID of the party member who killed the NPC.</param>
        /// <param name="npcGuid">The GUID of the killed NPC.</param>
        public void OnPartyKill(ulong sourceGuid, ulong npcGuid)
        {
            IWowUnit wowUnit = Bot.GetWowObjectByGuid<IWowUnit>(npcGuid);

            if (wowUnit != null
                && (Bot.Player.Guid == sourceGuid || Bot.Objects.PartymemberGuids.Contains(sourceGuid))
                && NpcIds.Contains(BotUtils.GuidToNpcId(npcGuid)))
            {
                ++Killed;
            }
        }
    }
}