using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Engines.Test
{
    /// <summary>
    /// Represents a test engine that uses default settings and functions. This class implements the ITestEngine interface.
    /// </summary>
    public class DefaultTestEngine : ITestEngine
    {
        /// <summary>
        /// The ID of the trainer entry.
        /// </summary>
        private const int trainerEntryId = 3173;

        /// <summary>
        /// Represents a private field that stores an instance of the interface IWowUnit, which represents a trainer.
        /// </summary>
        private IWowUnit trainer;

        /// <summary>
        /// Initializes a new instance of the DefaultTestEngine class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance that represents the bot.</param>
        /// <param name="config">The AmeisenBotConfig instance that represents the bot's configuration.</param>
        public DefaultTestEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;

            RootSelector = new Selector
            (
                () => trainer != null,
                new Selector
                (
                    () => Bot.Wow.UiIsVisible("GossipFrame"),
                    new Selector
                    (
                        () => SelectedTraining(),
                        new Leaf(TrainAll),
                        new Leaf(Fail)
                    ),
                    new Leaf(OpenTrainer)
                ),
                new Leaf(GetTrainer)
            );

            TestTree = new Tree
            (
                RootSelector
            );
        }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces used by the bot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the configuration for the AmeisenBot.
        /// </summary>
        public AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets the root selector.
        /// </summary>
        private Selector RootSelector { get; }

        /// <summary>
        /// Gets the TestTree.
        /// </summary>
        private Tree TestTree { get; }

        /// <summary>
        /// Executes the Execute method, which calls the Tick method of the TestTree object.
        /// </summary>
        public void Execute()
        {
            TestTree.Tick();
        }

        /// <summary>
        /// Checks if the trainer has gossip and selects the gossip option for training.
        /// </summary>
        /// <returns>
        /// Returns true if the trainer has gossip and the gossip option for training is selected; otherwise, returns false.
        /// </returns>
        public bool SelectedTraining()
        {
            if (!trainer.IsGossip)
            {
                return false;
            }

            // gossip 1 train skills gossip 2 unlearn talents quest gossip from trainer??

            string[] gossipTypes = Bot.Wow.GetGossipTypes();

            for (int i = 0; i < gossipTypes.Length; ++i)
            {
                if (!gossipTypes[i].Equals("trainer", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // +1 is due to implicit conversion between lua array (indexed at 1 not 0) and c# array
                Bot.Wow.SelectGossipOptionSimple(i + 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the status "Failed" for Bluetooth communication.
        /// </summary>
        private static BtStatus Fail()
        {
            return BtStatus.Failed;
        }

        /// <summary>
        /// Method to return a success BtStatus.
        /// </summary>
        private static BtStatus Success()
        {
            return BtStatus.Success;
        }

        /// <summary>
        /// Retrieves the closest trainer based on the trainer entry ID and updates the "trainer" variable.
        /// </summary>
        /// <returns>The status of the operation. Returns BtStatus.Success if the trainer is found and BtStatus.Failed otherwise.</returns>
        private BtStatus GetTrainer()
        {
            if (Bot.GetClosestTrainerByEntryId(trainerEntryId) == null)
            {
                return BtStatus.Failed;
            }

            trainer = Bot.GetClosestTrainerByEntryId(trainerEntryId);
            return BtStatus.Success;
        }

        /// <summary>
        /// Method to open the trainer for interacting with units.
        /// </summary>
        /// <returns>Returns the status of the operation. If the operation fails or encounters an error, it returns BtStatus.Failed. If the operation is successful, it returns BtStatus.Success.</returns>
        private BtStatus OpenTrainer()
        {
            if (Bot == null || trainer == null)
            {
                return BtStatus.Failed;
            }

            if (Bot.Wow.TargetGuid != trainer.Guid)
            {
                Bot.Wow.ChangeTarget(trainer.Guid);
            }

            if (!BotMath.IsFacing(Bot.Objects.Player.Position, Bot.Objects.Player.Rotation, trainer.Position, 0.5f))
            {
                Bot.Wow.FacePosition(Bot.Objects.Player.BaseAddress, Bot.Player.Position, trainer.Position);
            }

            if (Bot.Wow.UiIsVisible("GossipFrame"))
            {
                return BtStatus.Success;
            }

            Bot.Wow.InteractWithUnit(trainer);
            return BtStatus.Success;
        }

        /// <summary>
        /// Trains all the bots by clicking on the train button in the game.
        /// </summary>
        /// <returns>Returns the status of the training process.</returns>
        private BtStatus TrainAll()
        {
            Bot.Wow.ClickOnTrainButton();

            return BtStatus.Success;
        }
    }
}