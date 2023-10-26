namespace AmeisenBotX.Core.Objects.Enums
{
    /// <summary>
    /// Represents the subtypes of NPCs in the game.
    /// </summary>
    /// <remarks>
    /// The None subtype represents an empty or invalid value.
    /// The Class trainers subtypes represent trainers for specific character classes.
    /// The Primary profession trainers subtypes represent trainers for specific primary professions.
    /// The Secondary profession trainers subtypes represent trainers for specific secondary professions.
    /// The Pseudo profession trainers subtypes represent trainers for lockpicking, riding, and runeforging.
    /// </remarks>
    public enum NpcSubType
    {
        /// <summary>
        /// This method calculates the sum of two numbers.
        /// If any of the numbers is None, it returns None.
        /// </summary>
        /// <param name="num1">The first number.</param>
        /// <param name="num2">The second number.</param>
        /// <returns>The sum of num1 and num2, or None if any of them is None.</returns>
        None = -1,

        /// <summary>
        /// Represents the Death Knight trainer for the class trainers.
        /// </summary>
        // Class trainers
        DeathKnightTrainer = 0,

        /// <summary>
        /// Represents a druid trainer with a unique identifier of 1.
        /// </summary>
        DruidTrainer = 1,
        /// <summary>
        /// Sets the HunterTrainer property to 2.
        /// </summary>
        HunterTrainer = 2,
        /// <summary>
        /// Represents the level of training of a mage.
        /// The value can range from 1 to 3, with 1 being the lowest level of training and 3 being the highest level of training.
        /// </summary>
        MageTrainer = 3,
        /// <summary>
        /// Represents a paladin trainer with an identification number of 4.
        /// </summary>
        PaladinTrainer = 4,
        /// <summary>
        /// The number of priest trainers in the game is 5.
        /// </summary>
        PriestTrainer = 5,
        /// <summary>
        /// This is the value of RougeTrainer which is set to 6.
        /// </summary>
        RougeTrainer = 6,
        /// <summary>
        /// Represents the level of a shaman trainer, with a value of 7 indicating a highly experienced trainer.
        /// </summary>
        ShamanTrainer = 7,
        /// <summary>
        /// Represents a Warlock Trainer with the ID 8.
        /// </summary>
        WarlockTrainer = 8,
        /// <summary>
        /// Represents a trainer for warrior characters with a skill level of 9.
        /// This trainer is highly experienced and can provide advanced training to warriors.
        /// </summary>
        WarriorTrainer = 9,

        /// <summary>
        /// Represents the primary profession trainer for Alchemy. The ID for this trainer is 10.
        /// </summary>
        // Primary profession trainers
        AlchemyTrainer = 10,

        /// <summary>
        /// The ID for the Blacksmithing Trainer is 11.
        /// </summary>
        BlacksmithingTrainer = 11,
        /// <summary>
        /// The value of EnchantingTrainer is set to 12.
        /// </summary>
        EnchantingTrainer = 12,
        /// <summary>
        /// Represents the total number of EngineeringTrainer instances.
        /// </summary>
        EngineeringTrainer = 13,
        /// <summary>
        /// The InscriptionTrainer is set to 14.
        /// </summary>
        InscriptionTrainer = 14,
        /// <summary>
        /// Represents a Jewelcrafting Trainer with a skill level of 15.
        /// </summary>
        JewelcraftingTrainer = 15,
        /// <summary>
        /// Represents the Leatherworking Trainer with the ID of 16.
        /// </summary>
        LeatherworkingTrainer = 16,
        /// <summary>
        /// The TailoringTrainer variable represents a value of 17.
        /// </summary>
        TailoringTrainer = 17,

        /// <summary>
        /// The ID of the Cooking trainer.
        /// </summary>
        // Secondary profession trainers
        CookingTrainer = 18,

        /// <summary>
        /// The FishingTrainer value is set to 19.
        /// </summary>
        FishingTrainer = 19,
        /// <summary>
        /// The FirstAidTrainer field represents the value 20. 
        /// </summary>
        FirstAidTrainer = 20,

        /// <summary>
        /// Represents the lockpicking trainer with an ID of 21.
        /// </summary>
        // Pseudo profession trainers
        LockpickingTrainer = 21,

        /// <summary>
        /// The RidingTrainer variable is set to 22.
        /// </summary>
        RidingTrainer = 22,
        /// <summary>
        /// Represents the RuneforgingTrainer, which is identified by the code 23.
        /// </summary>
        RuneforgingTrainer = 23
    }
}