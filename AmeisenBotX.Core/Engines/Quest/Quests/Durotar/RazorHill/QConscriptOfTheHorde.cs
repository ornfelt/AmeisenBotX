using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Quests.Durotar.RazorHill
{
    /// <summary>
    /// Initializes a new instance of the QConscriptOfTheHorde class.
    /// </summary>
    internal class QConscriptOfTheHorde : BotQuest
    {
        /// <summary>
        /// Initializes a new instance of the QConscriptOfTheHorde class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object to use for bot functionality.</param>
        public QConscriptOfTheHorde(AmeisenBotInterfaces bot)
                    : base(bot, 840, "Conscript of the Horde", 10, 1,
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3336 }), new Vector3(271.80f, -4650.83f, 11.79f)),
                        () => (bot.GetClosestQuestGiverByNpcId(bot.Player.Position, new List<int> { 3337 }), new Vector3(303.43f, -3686.16f, 27.15f)),
                        null)
        { }
    }
}