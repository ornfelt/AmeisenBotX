namespace AmeisenBotX.Wow.Objects.Enums
{
    /// <summary>
    /// All WoW class specializations for WotLK 3.3.5.
    /// Used for spec-specific item evaluation and stat weights.
    /// </summary>
    public enum WowSpecialization
    {
        None = 0,

        // Death Knight
        DeathknightBlood,
        DeathknightFrost,
        DeathknightUnholy,

        // Druid
        DruidBalance,
        DruidFeralCat,
        DruidFeralBear,
        DruidRestoration,

        // Hunter
        HunterBeastmastery,
        HunterMarksmanship,
        HunterSurvival,

        // Mage
        MageArcane,
        MageFire,
        MageFrost,

        // Paladin
        PaladinHoly,
        PaladinProtection,
        PaladinRetribution,

        // Priest
        PriestDiscipline,
        PriestHoly,
        PriestShadow,

        // Rogue
        RogueAssassination,
        RogueCombat,
        RogueSubtlety,

        // Shaman
        ShamanElemental,
        ShamanEnhancement,
        ShamanRestoration,

        // Warlock
        WarlockAffliction,
        WarlockDemonology,
        WarlockDestruction,

        // Warrior
        WarriorArms,
        WarriorFury,
        WarriorProtection
    }
}
