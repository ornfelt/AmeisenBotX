using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using AmeisenBotX.Core.Engines.Quest.Quests.Grinder;
using System.Collections.Generic;

/// <summary>
/// Namespace for the Shino profile of the AmeisenBotX.Core.Engines.Quest.Profiles namespace.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Profiles.Shino
{
    /// <summary>
    /// Constructor for Horde1To60GrinderProfile, which sets up the list of quests to be completed.
    /// </summary>
    internal class Horde1To60GrinderProfile : IQuestProfile
    {
        /// <summary>
        /// Constructor for Horde1To60GrinderProfile, which sets up the list of quests to be completed.
        /// Each quest is added to the Quests queue, with the corresponding bot and level.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance to use for the quests</param>
        public Horde1To60GrinderProfile(AmeisenBotInterfaces bot)
        {
            Quests = new Queue<List<IBotQuest>>();
            Quests.Enqueue(new List<IBotQuest>() {
                new QDurotarGrindToLevel6(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QDurotarGrindToLevel9(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QDurotarGrindToLevel11(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTheBarrensGrindToLevel14(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTheBarrensGrindToLevel16(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTheBarrensGrindToLevel19(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QStonetalonGrindToLevel23(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QStonetalonGrindToLevel31(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QDesolaceGrindToLevel35(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QDesolaceGrindToLevel40(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTanarisGrindToLevel44(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QTanarisGrindToLevel49(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QUngoroGrindToLevel54(bot)
            });
            Quests.Enqueue(new List<IBotQuest>() {
                new QSilithusGrindToLevel60(bot)
            });
        }

        /// <summary>
        /// Gets or sets the queue of lists of IBotQuest objects.
        /// </summary>
        public Queue<List<IBotQuest>> Quests { get; }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"[1-60] Horde1To60GrinderProfile (Shino)";
        }
    }
}