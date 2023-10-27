using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Cache.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Represents an action that checks emails while the system is idle.
    /// </summary>
    public class CheckMailsIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the CheckMailsIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public CheckMailsIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot is enabled for this instance.
        /// </summary>
        public bool AutopilotOnly => true;

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown period in milliseconds.
        /// </summary>
        public int MaxCooldown => 7 * 60 * 1000;

        /// <summary>
        /// Gets the maximum duration in milliseconds, which is calculated as 2 minutes (2 * 60 seconds) multiplied by 1000 milliseconds.
        /// </summary>
        public int MaxDuration => 2 * 60 * 1000;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 6 * 60 * 1000;

        /// <summary>
        /// Gets the minimum duration in milliseconds.
        /// </summary>
        public int MinDuration => 1 * 60 * 1000;

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces object that represents the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the mails have been checked.
        /// </summary>
        private bool CheckedMails { get; set; }

        /// <summary>
        /// Gets or sets the current position of the mailbox in 3D space.
        /// </summary>
        private Vector3 CurrentMailbox { get; set; }

        /// <summary>
        /// Gets or sets the time when the mailbox was last checked.
        /// </summary>
        private DateTime MailboxCheckTime { get; set; }

        /// <summary>
        /// Gets or sets the origin position in a three-dimensional space.
        /// </summary>
        private Vector3 OriginPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object has returned to its origin.
        /// </summary>
        private bool ReturnedToOrigin { get; set; }

        /// <summary>
        /// Gets a private instance of the Random class.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Sets the necessary variables for the bot to enter a mailbox.
        /// If a mailbox is found within a certain radius of the player's position, the current mailbox is set and the method returns true.
        /// Otherwise, returns false.
        /// </summary>
        public bool Enter()
        {
            CheckedMails = false;
            MailboxCheckTime = default;
            OriginPosition = Bot.Player.Position;

            if (Bot.Db.TryGetPointsOfInterest(Bot.Objects.MapId, PoiType.Mailbox, Bot.Player.Position, 256.0f, out IEnumerable<Vector3> mailboxes))
            {
                CurrentMailbox = mailboxes.OrderBy(e => e.GetDistance(Bot.Player.Position)).First();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the mailbox interaction logic.
        /// If CheckedMails is false, it checks if the current mailbox is within a certain distance. If it is, it stops movement
        /// and interacts with the mailbox game object, looting all items in the mailbox by executing a Lua script.
        /// Sets CheckedMails to true and schedules the next mailbox check time.
        /// 
        /// If CheckedMails is true and ReturnedToOrigin is false, it checks if the current mailbox is within a certain distance
        /// from the origin position. If it is, it stops movement and sets ReturnedToOrigin to true.
        /// </summary>
        public void Execute()
        {
            if (!CheckedMails)
            {
                if (CurrentMailbox.GetDistance(Bot.Player.Position) > 3.5f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, CurrentMailbox);
                }
                else
                {
                    Bot.Movement.StopMovement();

                    IWowGameobject mailbox = Bot.Objects.All.OfType<IWowGameobject>()
                        .FirstOrDefault(e => e.GameObjectType == WowGameObjectType.Mailbox && e.Position.GetDistance(CurrentMailbox) < 1.0f);

                    if (mailbox != null)
                    {
                        Bot.Wow.InteractWithObject(mailbox);
                        Bot.Wow.LuaDoString("for i=1,GetInboxNumItems()do AutoLootMailItem(i)end");
                    }

                    CheckedMails = true;
                    MailboxCheckTime = DateTime.UtcNow + TimeSpan.FromSeconds(Rnd.Next(7, 16));
                }
            }
            else if (!ReturnedToOrigin && MailboxCheckTime < DateTime.UtcNow)
            {
                if (CurrentMailbox.GetDistance(OriginPosition) > 8.0f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, OriginPosition);
                }
                else
                {
                    Bot.Movement.StopMovement();
                    ReturnedToOrigin = true;
                }
            }
        }

        /// <summary>
        /// Overrides the ToString() method and returns the string representation of the object, including a robot emoji if AutopilotOnly is true,
        /// followed by the phrase "Check Mails".
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Check Mails";
        }
    }
}