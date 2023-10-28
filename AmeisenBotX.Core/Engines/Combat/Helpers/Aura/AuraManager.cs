using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;

/// <summary>
/// Represents a namespace for managing auras in the bot.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Aura
{
    /// <summary>
    /// Represents a class for managing auras in the bot.
    /// </summary>
    public class AuraManager
    {
        /// <summary>
        /// Initializes a new instance of the AuraManager class.
        /// </summary>
        /// <param name="bot">The bot instance that will use the AuraManager.</param>
        public AuraManager(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Jobs = new();
        }

        ///<summary>
        ///Gets or sets the Bot for the AmeisenBotInterfaces.
        ///</summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the jobs associated with the Aura.
        /// </summary>
        public List<IAuraJob> Jobs { get; set; }

        /// <summary>
        /// Executes a tick on a collection of WoW auras.
        /// Iterates through each aura job and runs it on the given auras.
        /// Returns true if any of the jobs returns true, indicating a successful tick.
        /// Returns false if none of the jobs return true.
        /// </summary>
        /// <param name="auras">The collection of WoW auras to be ticked.</param>
        /// <returns>True if any of the jobs returns true, otherwise false.</returns>
        public bool Tick(IEnumerable<IWowAura> auras)
        {
            foreach (IAuraJob job in Jobs)
            {
                if (job.Run(auras))
                {
                    return true;
                }
            }

            return false;
        }
    }
}