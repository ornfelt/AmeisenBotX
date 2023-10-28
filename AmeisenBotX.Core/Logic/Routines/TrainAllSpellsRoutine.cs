/// <summary>
/// Provides logic for various routines used in the AmeisenBotX bot.
/// </summary>
namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Executes the Run method to perform actions on the bot and config. 
    /// Note: This can fail for numerous reasons such as insufficient funds or bugs with the NPC or trainer frame.
    /// Please ensure a stable trainer frame is open and an unlimited cash supply is available before executing this method.
    /// </summary>
    public static class TrainAllSpellsRoutine
    {
        /// <summary>
        /// Executes the Run method to perform actions on the bot and config. 
        /// Note: This can fail for numerous reasons such as insufficient funds or bugs with the NPC or trainer frame.
        /// Please ensure a stable trainer frame is open and an unlimited cash supply is available before executing this method.
        /// </summary>
        public static void Run(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            // this can fail for myriad of reasons like not having enough money to buy service, or
            // npc getting rekt/trainerFrame bugging out this basically assumes unlimited cash
            // supply and stable trainer frame open while executing
            bot.Wow.ClickOnTrainButton();
            bot.Wow.ClearTarget();
        }
    }
}