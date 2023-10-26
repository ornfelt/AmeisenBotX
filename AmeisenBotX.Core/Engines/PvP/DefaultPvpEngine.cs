using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;

namespace AmeisenBotX.Core.Engines.PvP
{
    public class DefaultPvpEngine : IPvpEngine
    {
        /// <summary>
        /// Constructor for DefaultPvpEngine class.
        /// Initializes a new instance of the class with the provided bot and config parameters.
        /// </summary>
        public DefaultPvpEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;

            INode mainNode = new Waterfall
            (
                new Leaf(() => BtStatus.Ongoing),
                (() => QueueStatus == 0, new Leaf(QueueForBattlegrounds))
            // (() => QueueStatus == 2, new Leaf(() => { Bot.Wow.AcceptBattlegroundInvite(); return
            // BtStatus.Success; }))
            );

            Bt = new(mainNode);
        }

        /// <summary>
        /// Gets or sets the Bot object that implements the AmeisenBotInterfaces interface.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// This property represents a private Tree object named Bt.
        /// </summary>
        private Tree Bt { get; }

        /// <summary>
        /// Gets the AmeisenBotConfig object.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets the status of the queue.
        /// </summary>
        private int QueueStatus { get; set; }

        /// <summary>
        /// Executes the necessary actions for the PvpQueue, including updating the status 
        /// and executing the Tick method of the Bt object.
        /// </summary>
        public void Execute()
        {
            UpdatePvpQueueStatus();
            Bt.Tick();
        }

        /// <summary>
        /// Queues the player for the Battlegrounds.
        /// </summary>
        private BtStatus QueueForBattlegrounds()
        {
            // TODO: fix this function Bot.Wow.LuaQueueBattlegroundByName("Warsong Gulch");

            Bot.Wow.ClickUiElement("BattlegroundType2");
            Bot.Wow.ClickUiElement("PVPBattlegroundFrameJoinButton");

            return BtStatus.Success;
        }

        /// <summary>
        /// Updates the player versus player (PvP) queue status by reading the value
        /// from the memory and assigning it to the QueueStatus property.
        /// </summary>
        private void UpdatePvpQueueStatus()
        {
            if (Bot.Memory.Read(Bot.Memory.Offsets.BattlegroundStatus, out int q))
            {
                QueueStatus = q;
            }
        }
    }
}