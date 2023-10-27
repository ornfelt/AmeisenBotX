using AmeisenBotX.Common.Utils;
using System;

namespace AmeisenBotX.Core.Engines.Combat.Helpers
{
    /// <summary>
    /// Initializes a new instance of the PetManager class with the specified parameters.
    /// </summary>
    public class PetManager
    {
        /// <summary>
        /// Initializes a new instance of the PetManager class with the specified parameters.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object for communication with the bot.</param>
        /// <param name="healPetCooldown">The TimeSpan representing the cooldown time for healing the pet.</param>
        /// <param name="castMendPetFunction">The function used for casting the Mend Pet spell.</param>
        /// <param name="castCallPetFunction">The function used for casting the Call Pet spell.</param>
        /// <param name="castRevivePetFunction">The function used for casting the Revive Pet spell.</param>
        public PetManager(AmeisenBotInterfaces bot, TimeSpan healPetCooldown, Func<bool> castMendPetFunction, Func<bool> castCallPetFunction, Func<bool> castRevivePetFunction)
        {
            Bot = bot;
            HealPetCooldown = healPetCooldown;
            CastMendPet = castMendPetFunction;
            CastCallPet = castCallPetFunction;
            CastRevivePet = castRevivePetFunction;

            CallPetEvent = new(TimeSpan.FromSeconds(8));
        }

        /// <summary>
        /// Gets or sets the Bot object that implements the AmeisenBotInterfaces interface.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; set; }

        /// <summary>
        /// Gets or sets the function for casting a call to pet.
        /// </summary>
        public Func<bool> CastCallPet { get; set; }

        /// <summary>
        /// Gets or sets the function used to cast the Mend Pet ability.
        /// </summary>
        public Func<bool> CastMendPet { get; set; }

        /// <summary>
        /// Gets or sets the function that is responsible for casting a revive pet action.
        /// </summary>
        public Func<bool> CastRevivePet { get; set; }

        /// <summary>
        /// Gets or sets the cooldown for healing the pet. 
        /// The cooldown duration is represented as a TimeSpan.
        /// </summary>
        public TimeSpan HealPetCooldown { get; set; }

        /// <summary>
        /// Gets or sets the last time the pet's mending ability was used.
        /// </summary>
        public DateTime LastMendPetUsed { get; private set; }

        /// <summary>
        /// Gets the private timegated event for calling a pet.
        /// </summary>
        private TimegatedEvent CallPetEvent { get; }

        /// <summary>
        ///  Gets or sets a boolean value indicating if the revive toggle should be called.
        /// </summary>
        private bool CallReviveToggle { get; set; }

        /// <summary>
        /// Gets or sets the last time the object was mounted.
        /// </summary>
        private DateTime LastTimeMounted { get; set; }

        /// <summary>
        /// This method is responsible for handling the tick logic for summoning and managing pets.
        /// It checks if the player is mounted and updates the LastTimeMounted variable.
        /// Returns false if the player is mounted.
        /// It also checks the LastTimeMounted to ensure that at least 1 second has passed since dismounting before performing any actions.
        /// Returns false if less than 1 second has passed.
        /// If a pet exists, it checks if the CastCallPet event is not null and either calls it if the pet is not summoned or dead, or calls the CastRevivePet event if applicable.
        /// Returns true if either action was performed successfully.
        /// If the pet does not exist, it checks if the CastCallPet event is not null and if the CallPetEvent is successful and the player is not casting. If CallReviveToggle is enabled, it calls the CastRevivePet event, otherwise it calls the CastCallPet event.
        /// Finally, if none of the above conditions are met, it returns false.
        /// </summary>
        public bool Tick()
        {
            if (Bot.Player.IsMounted)
            {
                // dont summon pets while on mount, they despawn when mounted
                LastTimeMounted = DateTime.UtcNow;
                return false;
            }

            if (LastTimeMounted + TimeSpan.FromSeconds(1) > DateTime.UtcNow)
            {
                // only do stuff 1sec after we dismounted pets need a few ms to spawn
                return false;
            }

            if (Bot.Objects.Pet != null)
            {
                if (CastCallPet != null
                    && ((Bot.Objects.Pet.Guid == 0 && CastCallPet.Invoke())
                    || (CastRevivePet != null
                        && Bot.Objects.Pet != null
                        && (Bot.Objects.Pet.Health == 0 || Bot.Objects.Pet.IsDead) && CastRevivePet())))
                {
                    return true;
                }

                if (Bot.Objects.Pet == null || Bot.Objects.Pet.Health == 0 || Bot.Objects.Pet.IsDead)
                {
                    return true;
                }

                if (CastMendPet != null
                    && DateTime.UtcNow - LastMendPetUsed > HealPetCooldown
                    && Bot.Objects.Pet.HealthPercentage < 80.0
                    && CastMendPet.Invoke())
                {
                    LastMendPetUsed = DateTime.UtcNow;
                    return true;
                }
            }
            else if (CastCallPet != null && CallPetEvent.Run() && !Bot.Player.IsCasting)
            {
                if (CallReviveToggle)
                {
                    CastRevivePet();
                }
                else
                {
                    CastCallPet.Invoke();
                }

                CallReviveToggle = !CallReviveToggle;
            }

            return false;
        }
    }
}