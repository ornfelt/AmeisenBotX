using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using AmeisenBotX.Core.Engines.Quest.Quests.Durotar.RazorHill;
using AmeisenBotX.Core.Engines.Quest.Quests.Durotar.ValleyOfStrength;
using AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.Crossroads;
using AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.OutpostBridge;
using AmeisenBotX.Core.Engines.Quest.Quests.TheBarrens.OutpostStonetalon;
using System.Collections.Generic;

/// <summary>
/// The namespace containing quest profiles for the Shino engine in AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Profiles.Shino
{
    /// <summary>
    /// Represents a quest profile for a Horde character leveling from level 1 to level 80.
    /// </summary>
    internal class X5Horde1To80Profile : IQuestProfile
    {
        /// <summary>
        /// Initializes a new instance of the X5Horde1To80Profile class with the given bot.
        /// The constructor sets up a series of quests in a queue for the bot to complete as it progresses from level 1 to level 80.
        /// Each quest is added to a separate list within the queue, with each list representing a group of related quests.
        /// The bot will complete the quests in the order they are enqueued.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot to assign the quests to.</param>
        public X5Horde1To80Profile(AmeisenBotInterfaces bot)
        {
            Quests = new Queue<List<IBotQuest>>();
            Quests.Enqueue(new List<IBotQuest>() {
                new QYourPlaceInTheWorld(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QCuttingTeeth(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QStingOfTheScorpid(bot),
                new QVileFamiliars(bot),
                new QGalgarCactusAppleSurprise(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QSarkoth(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QSarkoth2(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QAPeonBurden(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QVanquishTheBetrayers(bot),
                new QCarryYourWeight(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QEncroachment(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QConscriptOfTheHorde(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QCrossroadsConscription(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QDisruptTheAttacks(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTheDisruptionEnds(bot),
                new QSuppliesForTheCrossroads(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QPlainstriderMenace(bot),
                new QRaptorThieves(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QSouthseaFreebooters(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTheZhevra(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QProwlersOfTheBarrens(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QHarpyRaiders(bot),
                new QCentaurBracers(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QKolkarLeaders(bot),
                new QHarpyLieutenants(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QSerenaBloodfeather(bot),
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QLetterToJinZil(bot),
            });
        }

        /// <summary>
        /// Gets or sets the collection of quests for the bot, organized in a queue.
        /// Each quest is represented as a list of <see cref="IBotQuest"/> objects.
        /// </summary>
        public Queue<List<IBotQuest>> Quests { get; }

        /// <summary>
        /// Returns the string representation of the X5Horde1To80Profile object in the format "[1-80] X5Horde1To80Profile (Shino)".
        /// </summary>
        public override string ToString()
        {
            return $"[1-80] X5Horde1To80Profile (Shino)";
        }
    }
}