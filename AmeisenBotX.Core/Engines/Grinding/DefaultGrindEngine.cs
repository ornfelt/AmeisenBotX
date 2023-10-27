using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Grinding.Objects;
using AmeisenBotX.Core.Engines.Grinding.Profiles;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Logic;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Grinding
{
    /// <summary>
    /// Constructor for the DefaultGrindEngine class.
    /// Initializes a new instance of the DefaultGrindEngine class with the specified bot and config.
    /// </summary>
    /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
    /// <param name="config">The AmeisenBotConfig object representing the bot configuration.</param>
    public class DefaultGrindEngine : IGrindingEngine
    {
        /// <summary>
        /// Constructor for the DefaultGrindEngine class.
        /// Initializes a new instance of the DefaultGrindEngine class with the specified bot and config.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        /// <param name="config">The AmeisenBotConfig object representing the bot configuration.</param>
        public DefaultGrindEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;

            RootSelector = new Selector
            (
                () => Profile == null,
                new Leaf(ReportNoProfile),
                new Selector
                    (
                        () => Bot.Character.LastLevelTrained == 0,
                        new Leaf(InitLastTrainingLevel),
                        new Selector
                            (
                                () => NeedToRepair(),
                                new Leaf(GoToNpcAndRepair),
                                new Selector
                                (
                                    () => NeedToSell(),
                                    new Leaf(GoToNpcAndSell),
                                    new Selector
                                    (
                                        () => NeedToDismount(),
                                        new Leaf(Dismount),
                                        new Selector
                                        (
                                            () => NeedToTrainSpells(),
                                            new Leaf(GoToNpcAndTrain),
                                            new Selector
                                            (
                                                () => NeedToLearnSecondarySkills(),
                                                new Leaf(GoToNpcAndLearnSecondarySkills),
                                                new Selector
                                                (
                                                    () => ThreatsNearby(),
                                                    new Leaf(FightTarget),
                                                    new Selector
                                                    (
                                                        () => TargetsNearby(),
                                                        new Selector
                                                        (
                                                            () => SelectTarget(),
                                                            new Leaf(FightTarget),
                                                            new Leaf(() => BtStatus.Failed)
                                                        ),
                                                        new Leaf(MoveToNextGrindNode)
                                                    )
                                                )
                                            )
                                        )
                                  )
                              )
                         )
                    )
            );

            GrindingTree = new Tree
            (
                RootSelector
            );
        }

        /// <summary>
        /// The counter for keeping track of the number of fighting targets.
        /// </summary>
        private static uint FightingTargetCounter = 0;

        /// <summary>
        /// Private static field that represents a list of blacklisted targets with data type ulong.
        /// </summary>
        private static List<ulong> BlackListedTargets = new List<ulong>();

        /// <summary>
        /// Gets or sets the Bot property of the AmeisenBotInterfaces class.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the configuration of the AmeisenBot.
        /// </summary>
        public AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets the grinding profile associated with the object.
        /// </summary>
        public IGrindingProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets the index of the current spot.
        /// </summary>
        private int CurrentSpotIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the program is going to the next spot.
        /// </summary>
        private bool GoingToNextSpot { get; set; }

        /// <summary>
        /// Gets the Tree object for grinding.
        /// </summary>
        private Tree GrindingTree { get; }

        /// <summary>
        /// Gets or sets the next grinding spot.
        /// </summary>
        private GrindingSpot NextSpot { get; set; } = new();

        /// <summary>
        /// Gets the root selector.
        /// </summary>
        private Selector RootSelector { get; }

        /// <summary>
        /// Executes the grinding operation on the grinding tree.
        /// </summary>
        public void Execute()
        {
            GrindingTree.Tick();
        }

        /// <summary>
        /// Load the specified grinding profile.
        /// </summary>
        /// <param name="profile">The grinding profile to be loaded.</param>
        public void LoadProfile(IGrindingProfile profile)
        {
            Profile = profile;
        }

        /// <summary>
        /// Determines whether the given WoW object is within the radius of the specified grind spot.
        /// </summary>
        /// <param name="wowObject">The WoW object to check.</param>
        /// <param name="grindSpot">The grind spot to compare with.</param>
        /// <returns>True if the WoW object is within the grind spot's radius; otherwise, false.</returns>
        private static bool ObjectWithinGrindSpotRadius(IWowObject wowObject, GrindingSpot grindSpot)
        {
            return wowObject.Position.GetDistance(grindSpot.Position) <= grindSpot.Radius;
        }

        /// <summary>
        /// Checks if the level of the provided unit is within the minimum and maximum level range of the grind spot.
        /// </summary>
        /// <param name="unit">The unit to be checked.</param>
        /// <param name="grindSpot">The grind spot object containing the level limits.</param>
        /// <returns>True if the unit's level is within the grind spot's level range, otherwise false.</returns>
        private static bool UnitWithinGrindSpotLvlLimit(IWowUnit unit, GrindingSpot grindSpot)
        {
            return unit.Level >= grindSpot.MinLevel && unit.Level <= grindSpot.MaxLevel;
        }

        /// <summary>
        /// Dismounts the character from their current mount.
        /// </summary>
        /// <returns>Returns a BtStatus indicating the success of the dismount operation.</returns>
        private BtStatus Dismount()
        {
            Bot.Wow.DismissCompanion("MOUNT");
            return BtStatus.Success;
        }

        /// <summary>
        /// This method is used to engage with a target in combat.
        /// It checks if there is a target, and if not, it returns a Failed status.
        /// It increments the FightingTargetCounter and checks if it has reached the limit of 1000.
        /// If it has, it adds the target to the BlackListedTargets list, clears the target, and returns a Failed status.
        /// It then executes the combat class's Execute method and returns a Success status.
        /// </summary>
        private BtStatus FightTarget()
        {
            if (Bot.Target == null)
            {
                return BtStatus.Failed;
            }

            FightingTargetCounter++;
            if (FightingTargetCounter > 1000)
            {
                AmeisenLogger.I.Log("AmeisenBot", "FightingTargetCounter > 1000 " + Bot.Target.Guid, LogLevel.Debug);
                BlackListedTargets.Add(Bot.Target.Guid);
                Bot.Wow.ClearTarget();
                return BtStatus.Failed;
            }

            Bot.CombatClass?.Execute();
            return BtStatus.Success;
        }

        /// <summary>
        /// Method to go to an NPC and learn secondary skills.
        /// </summary>
        /// <returns>
        /// The status of the operation (BtStatus).
        /// </returns>
        private BtStatus GoToNpcAndLearnSecondarySkills()
        {
            Npc profileTrainer = Profile.NpcsOfInterest?
                .Where(e => e.Type == NpcType.ProfessionTrainer)
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            IWowUnit professionTrainer = null;

            if (profileTrainer != null)
            {
                professionTrainer = profileTrainer.SubType switch
                {
                    NpcSubType.FishingTrainer when !Bot.Character.Skills.ContainsKey("Fishing") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.FirstAidTrainer when !Bot.Character.Skills.ContainsKey("First Aid") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.CookingTrainer when !Bot.Character.Skills.ContainsKey("Cooking") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    _ => null
                };
            }

            if (professionTrainer == null)
            {
                return BtStatus.Failed;
            }

            if (professionTrainer.Position.GetDistance(Bot.Player.Position) > 5.0f)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, professionTrainer.Position);
                return BtStatus.Ongoing;
            }

            if (professionTrainer.Position.GetDistance(Bot.Player.Position) < 5.0f)
            {
                Bot.Movement.StopMovement();
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// This method is responsible for finding the nearest vendor NPC with repair services and navigating the character to it for repairs.
        /// </summary>
        /// <returns>The status of the behavior tree node, indicating whether the action is ongoing or successful.</returns>
        private BtStatus GoToNpcAndRepair()
        {
            List<Vector3> repairNpcsPos = (
                from vendor in Profile.NpcsOfInterest
                where vendor.Type == NpcType.VendorRepair
                select vendor.Position).ToList();

            Vector3 repairNpc = repairNpcsPos.OrderBy(e => e.GetDistance(Bot.Player.Position)).First();
            if (repairNpc.GetDistance(Bot.Player.Position) > 5.0f)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, repairNpc);
                return BtStatus.Ongoing;
            }
            if (repairNpc.GetDistance(Bot.Player.Position) < 5.0f)
            {
                Bot.Movement.StopMovement();
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Goes to the nearest NPC that is a vendor for selling or buying items and initiates the selling process.
        /// </summary>
        /// <returns>The status of the behavior tree node.</returns>
        private BtStatus GoToNpcAndSell()
        {
            List<Npc> profileVendors = Profile.NpcsOfInterest;
            if (!profileVendors.Any())
            {
                return BtStatus.Failed;
            }

            Npc firstVendor = profileVendors
                .Where(e => e.Type is NpcType.VendorSellBuy or NpcType.VendorRepair)
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();
            if (firstVendor == null)
            {
                return BtStatus.Failed;
            }

            if (firstVendor.Position.GetDistance(Bot.Player.Position) > 5.0f)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, firstVendor.Position);
                return BtStatus.Ongoing;
            }
            if (firstVendor.Position.GetDistance(Bot.Player.Position) < 5.0f)
            {
                Bot.Movement.StopMovement();
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Method to go to the nearest NPC class trainer and train player's class.
        /// </summary>
        /// <returns>The status of the behavior tree node execution.</returns>
        private BtStatus GoToNpcAndTrain()
        {
            Npc firstTrainer = Profile.NpcsOfInterest?
                .Where(e => e.Type == NpcType.ClassTrainer && e.SubType == AmeisenBotLogic.DecideClassTrainer(Bot.Player.Class))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            if (firstTrainer == null)
            {
                return BtStatus.Failed;
            }

            if (firstTrainer.Position.GetDistance(Bot.Player.Position) > 5.0f)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, firstTrainer.Position);
                return BtStatus.Ongoing;
            }
            if (firstTrainer.Position.GetDistance(Bot.Player.Position) < 5.0f)
            {
                Bot.Movement.StopMovement();
            }

            return BtStatus.Success;
        }

        /// <summary>
        /// Initializes the last training level.
        /// </summary>
        /// <returns>The status of the initialization.</returns>
        private BtStatus InitLastTrainingLevel()
        {
            if (Bot.Character.LastLevelTrained != 0)
            {
                return BtStatus.Failed;
            }

            Bot.Character.LastLevelTrained = Bot.Player.Level;
            return BtStatus.Success;
        }

        /// <summary>
        /// Moves the bot to the next grind node based on the specified conditions.
        /// </summary>
        /// <returns>The status of the movement operation.</returns>
        private BtStatus MoveToNextGrindNode()
        {
            Bot.CombatClass?.OutOfCombatExecute();

            List<GrindingSpot> spots = Profile.Spots.Where(e =>
                Bot.Player.Level >= e.MinLevel && Bot.Player.Level <= e.MaxLevel)
                .ToList();

            if (spots.Count == 0)
            {
                spots.AddRange(Profile.Spots.Where(e =>
                    e.MinLevel >= Profile.Spots.Max(e => e.MinLevel)));
            }

            switch (Profile.RandomizeSpots)
            {
                case true when !GoingToNextSpot:
                    {
                        Random rnd = new();
                        NextSpot = spots[rnd.Next(0, spots.Count)];
                        GoingToNextSpot = true;
                        break;
                    }
                case false when !GoingToNextSpot:
                    {
                        ++CurrentSpotIndex;

                        if (CurrentSpotIndex >= spots.Count)
                        {
                            CurrentSpotIndex = 0;
                        }

                        NextSpot = spots[CurrentSpotIndex];
                        GoingToNextSpot = true;
                        break;
                    }
            }

            if (Bot.Player.Position.GetDistance(NextSpot.Position) < 5.0f)
            {
                AmeisenLogger.I.Log("AmeisenBot", "Moving towards nextspot, pos: " + NextSpot.Position.X + " " + NextSpot.Position.Y +
                    " " + NextSpot.Position.Z, LogLevel.Debug);
                GoingToNextSpot = false;
                NextSpot = new GrindingSpot();
                return BtStatus.Success;
            }
            if (Bot.Player.Position.GetDistance(NextSpot.Position) > 5.0f)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, NextSpot.Position);
                return BtStatus.Ongoing;
            }

            return BtStatus.Success;
        }

        ///<summary>
        ///Checks if the bot's player is in combat and currently mounted.
        ///</summary>
        private bool NeedToDismount()
        {
            return Bot.Player.IsInCombat && Bot.Player.IsMounted;
        }

        /// <summary>
        /// Determines if the character needs to learn secondary skills from a profession trainer.
        /// </summary>
        /// <returns>True if the character needs to learn secondary skills, false otherwise.</returns>
        private bool NeedToLearnSecondarySkills()
        {
            Npc profileTrainer = Profile.NpcsOfInterest?
                .Where(e => e.Type == NpcType.ProfessionTrainer)
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            IWowUnit professionTrainer = null;

            if (profileTrainer != null)
            {
                professionTrainer = profileTrainer.SubType switch
                {
                    NpcSubType.FishingTrainer when !Bot.Character.Skills.ContainsKey("Fishing") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.FirstAidTrainer when !Bot.Character.Skills.ContainsKey("First Aid") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.CookingTrainer when !Bot.Character.Skills.ContainsKey("Cooking") => Bot
                        .GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    _ => null
                };
            }

            return professionTrainer != null;
        }

        /// <summary>
        /// Checks if the bot's equipment needs to be repaired based on the item durability and repair threshold.
        /// </summary>
        private bool NeedToRepair()
        {
            return Bot.Character.Equipment.Items.Any(e => e.Value.MaxDurability > 0
                   && (e.Value.Durability / (double)e.Value.MaxDurability * 100.0) <= Config.ItemRepairThreshold);
        }

        /// <summary>
        /// Determines if the player needs to sell items based on the number of free bag slots.
        /// </summary>
        /// <returns>True if the player needs to sell items, false otherwise.</returns>
        private bool NeedToSell()
        {
            return false;
            return Bot.Character.Inventory.FreeBagSlots < Config.BagSlotsToGoSell;
        }

        /// <summary>
        /// Determines if the character needs to train spells.
        /// </summary>
        /// <returns>
        /// True if the character needs to train spells, otherwise false.
        /// </returns>
        private bool NeedToTrainSpells()
        {
            bool levelGreaterThenLastTrained = Bot.Character.LastLevelTrained != 0
                                               && Bot.Character.LastLevelTrained < Bot.Player.Level;

            bool hasMoney = Bot.Character.Money > 10;

            Npc trainer = Profile.NpcsOfInterest?
                .Where(e => e.Type == NpcType.ClassTrainer && e.SubType == AmeisenBotLogic.DecideClassTrainer(Bot.Player.Class))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            return trainer != null && levelGreaterThenLastTrained && hasMoney;
        }

        /// <summary>
        /// Reports that there is no profile and returns the status as Failed.
        /// </summary>
        private BtStatus ReportNoProfile()
        {
            //TODO: warn no profile
            return BtStatus.Failed;
        }

        /// <summary>
        /// Selects a target for the bot to attack.
        /// </summary>
        private bool SelectTarget()
        {
            if (Bot.Target != null)
            {
                return true;
            }

            GrindingSpot nearestGrindSpot = Profile.Spots
                .Where(e => e.Position.GetDistance(Bot.Player.Position) <= e.Radius)
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position)).FirstOrDefault();

            if (nearestGrindSpot == null)
            {
                return false;
            }

            IWowUnit possibleTarget = Bot.GetNearEnemiesOrNeutrals<IWowUnit>(nearestGrindSpot.Position, nearestGrindSpot.Radius)
                .Where(e => UnitWithinGrindSpotLvlLimit(e, nearestGrindSpot) && ObjectWithinGrindSpotRadius(e, nearestGrindSpot)
                && !BlackListedTargets.Contains(e.Guid))
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position))
                .FirstOrDefault();

            if (possibleTarget == null)
            {
                return false;
            }

            FightingTargetCounter = 0;
            Bot.Wow.ChangeTarget(possibleTarget.Guid);
            return true;
        }

        /// <summary>
        /// Determines if there are targets nearby within the specified grind spot radius.
        /// </summary>
        /// <returns>True if there are targets nearby; otherwise, false.</returns>
        private bool TargetsNearby()
        {
            GrindingSpot nearestGrindSpot = Profile.Spots
                .Where(e => e.Position.GetDistance(Bot.Player.Position) <= e.Radius)
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position))
                .FirstOrDefault();

            if (nearestGrindSpot == null)
            {
                return false;
            }

            IEnumerable<IWowUnit> nearUnits = Bot.GetNearEnemiesOrNeutrals<IWowUnit>(nearestGrindSpot.Position, nearestGrindSpot.Radius)
                .Where(e => UnitWithinGrindSpotLvlLimit(e, nearestGrindSpot)
                                && ObjectWithinGrindSpotRadius(e, nearestGrindSpot)
                                && e.Health > 10)
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position));

            return nearUnits.Any();
        }

        /// <summary>
        /// Checks if there are any threats nearby.
        /// </summary>
        private bool ThreatsNearby()
        {
            IEnumerable<IWowUnit> enemiesFightingMe = Bot.GetEnemiesInCombatWithMe<IWowUnit>(Bot.Player.Position, 40)
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position))
                .ToList();
            IEnumerable<IWowUnit> enemiesTargetingMe = Bot.GetEnemiesTargetingMe<IWowUnit>(Bot.Player.Position, 40)
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position))
                .ToList();
            IEnumerable<IWowUnit> enemiesAround = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 40)
                .OrderBy(e => e.Position.GetDistance2D(Bot.Player.Position))
                .ToList();

            if (enemiesFightingMe.Any())
            {
                AmeisenLogger.I.Log("AmeisenBot", "ThreatsNearby! (enemiesfightingme)", LogLevel.Debug);
                Bot.Wow.ChangeTarget(enemiesFightingMe.FirstOrDefault().Guid);
                return true;
            }
            if (enemiesTargetingMe.Any())
            {
                AmeisenLogger.I.Log("AmeisenBot", "ThreatsNearby! (enemiestargetingme)", LogLevel.Debug);
                Bot.Wow.ChangeTarget(enemiesTargetingMe.FirstOrDefault().Guid);
                return true;
            }
            if (enemiesAround.Any())
            {
                AmeisenLogger.I.Log("AmeisenBot", "ThreatsNearby! (enemiesAround)", LogLevel.Debug);
                Bot.Wow.ChangeTarget(enemiesAround.FirstOrDefault().Guid);
                return true;
            }

            return false;
        }
    }
}