using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    public class SitByCampfireIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the SitByCampfireIdleAction class.
        /// </summary>
        /// <param name="bot">The bot to perform the idle action.</param>
        public SitByCampfireIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot is set to only.
        /// </summary>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets or sets the Bot property.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time for a certain action.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value in milliseconds.
        /// </summary>
        public int MaxCooldown => 11 * 60 * 1000;

        /// <summary>
        /// Gets the maximum duration in milliseconds.
        /// </summary>
        public int MaxDuration => 2 * 60 * 1000;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 5 * 60 * 1000;

        /// <summary>
        /// Gets the minimum duration in milliseconds.
        /// </summary>
        public int MinDuration => 1 * 60 * 1000;

        /// <summary>
        /// Gets or sets a value indicating whether a campfire has been placed.
        /// </summary>
        private bool PlacedCampfire { get; set; }

        /// <summary>
        /// Gets the instance of the Random class.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has sat down.
        /// </summary>
        private bool SatDown { get; set; }

        /// <summary>
        /// Enters the specific behavior.
        /// </summary>
        /// <returns>True if the "Basic Campfire" spell is known, otherwise false.</returns>
        public bool Enter()
        {
            PlacedCampfire = false;
            SatDown = false;

            return Bot.Character.SpellBook.IsSpellKnown("Basic Campfire");
        }

        /// <summary>
        /// Executes the action of sitting or sleeping near a cooking campfire.
        /// If the character has already placed the campfire and sat down, this method returns without performing any action.
        /// If the character is near a cooking campfire and has not yet sat down, this method makes the character face the campfire, sits or sleeps depending on a random chance, and sets the SatDown flag to true.
        /// If the character has not yet placed the campfire, this method casts the "Basic Campfire" spell and sets the PlacedCampfire flag to true.
        /// </summary>
        public void Execute()
        {
            if (PlacedCampfire && SatDown)
            {
                return;
            }

            IWowGameobject nearCampfire = Bot.Objects.All.OfType<IWowGameobject>()
                .FirstOrDefault(e => e.DisplayId == (int)WowGameObjectDisplayId.CookingCampfire
                                  && Bot.Objects.PartymemberGuids.Contains(e.CreatedBy));

            if (nearCampfire != null && !SatDown)
            {
                Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, nearCampfire.Position, true);
                Bot.Wow.SendChatMessage(Rnd.Next(0, 2) == 1 ? "/sit" : "/sleep");
                SatDown = true;
            }
            else if (!PlacedCampfire)
            {
                Bot.Wow.CastSpell("Basic Campfire");
                PlacedCampfire = true;
            }
        }

        /// <summary>
        /// Converts the object into a string representation.
        /// </summary>
        /// <returns>The string representation of the object. If AutopilotOnly is true, appends "(🤖)" to indicate autopilot mode.</returns>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Place Campfire";
        }
    }
}