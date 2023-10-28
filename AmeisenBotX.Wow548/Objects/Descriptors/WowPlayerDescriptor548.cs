using System.Runtime.InteropServices;

/// <summary>
/// Namespace for objects related to WoW player descriptors in version 5.4.8.
/// </summary>
namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a WoW player.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowPlayerDescriptor548
    {
        /// <summary>
        /// Represents a duel arbiter with a value type of ulong.
        /// </summary>
        public ulong DuelArbiter;
        /// <summary>
        /// Represents the player flags.
        /// </summary>
        public int PlayerFlags;
        /// <summary>
        /// Gets or sets the ID of the guild rank.
        /// </summary>
        public int GuildRankId;
        /// <summary>
        /// Represents the deletion date of a guild.
        /// </summary>
        public int GuildDeleteDate;
        /// <summary>
        /// Represents the level of the guild.
        /// </summary>
        public int GuildLevel;
        /// <summary>
        /// Gets or sets the identifier for the hair color.
        /// </summary>
        public int HairColorId;
        /// <summary>
        /// The RestState property represents the current rest state.
        /// </summary>
        public int RestState;
        /// <summary>
        /// Gets or sets the value indicating the arena faction.
        /// </summary>
        public int ArenaFaction;
        /// <summary>
        /// Gets or sets the number of the duel team.
        /// </summary>
        public int DuelTeam;
        /// <summary>
        /// Gets or sets the time stamp for the guild.
        /// </summary>
        public int GuildTimeStamp;
        /// <summary>
        /// Represents the quest log, which stores information about quests.
        /// The quest log has a fixed size of 750 integers.
        /// </summary>
        public fixed int QuestLog[750];
        /// <summary>
        /// Represents an array of fixed integers containing the visible items.
        /// The array has a length of 38.
        /// </summary>
        public fixed int VisibleItems[38];
        /// <summary>
        /// Represents the PlayerTitle property.
        /// </summary>
        public int PlayerTitle;
        /// <summary>
        /// Represents the level of fake inebriation.
        /// </summary>
        public int FakeInebriation;
        /// <summary>
        /// Gets or sets the realm of the virtual player.
        /// </summary>
        public int VirtualPlayerRealm;
        /// <summary>
        /// Gets or sets the current ID of the specification.
        /// </summary>
        public int CurrentSpecId;
        /// <summary>
        /// Represents the unique identifier for the taxi mount animation kit.
        /// </summary>
        public int TaxiMountAnimKitId;
        /// <summary>
        /// Gets or sets the current breed quality of the battle pet.
        /// </summary>
        public int CurrentBattlePetBreedQuality;
        /// <summary>
        /// The array representing the inventory slots.
        /// </summary>
        public fixed int InvSlots[46];
        /// <summary>
        /// Represents an array of 32 integers used to store packed slots.
        /// </summary>
        public fixed int PackSlots[32];
        /// <summary>
        /// The array of bank slots containing 56 fixed integers.
        /// </summary>
        public fixed int BankSlots[56];
        /// <summary>
        /// Gets or sets the array of vendor buyback slots, with a fixed size of 24.
        /// </summary>
        public fixed int VendorbuybackSlots[24];
        /// <summary>
        /// Represents a Farsight object.
        /// </summary>
        public ulong FarsightObject;
        /// <summary>
        /// Gets or sets an array of known titles with a length of 10.
        /// </summary>
        public fixed int KnownTitles[10];
        /// <summary>
        /// Represents a value in terms of coinage, typically used for currencies.
        /// </summary>
        public ulong Coinage;
        /// <summary>
        /// Represents the experience points of an entity.
        /// </summary>
        public int Xp;
        /// <summary>
        /// Represents the amount of experience points (XP) needed to reach the next level.
        /// </summary>
        public int NextLevelXp;
        /// <summary>
        /// Represents the array of skills, with a fixed size of 448 elements.
        /// </summary>
        public fixed int Skill[448];
        /// <summary>
        /// Array storing the skill line IDs for a maximum of 64 skills.
        /// </summary>
        public fixed int SkillLineids[64];
        /// <summary>
        /// Gets or sets the array of skill steps.
        /// </summary>
        public fixed int SkillSteps[64];
        /// <summary>
        /// An array to store the skill ranks.
        /// </summary>
        public fixed int SkillRanks[64];
        /// <summary>
        /// Represents an array of fixed 64 integers, storing the starting ranks for skills.
        /// </summary>
        public fixed int SkillStartingRanks[64];
        /// <summary>
        /// The maximum number of ranks for each skill. 
        /// This array has a fixed size of 64.
        /// </summary>
        public fixed int SkillMaxRanks[64];
        /// <summary>
        /// Represents an array of skill modifiers.
        /// The length of the array is fixed to 64.
        /// Each element in the array represents the modifier for a specific skill.
        /// </summary>
        public fixed int SkillModifiers[64];
        /// <summary>
        /// Represents an array of skill talents with a fixed length of 64.
        /// </summary>
        public fixed int SkillTalents[64];
        /// <summary>
        /// The total number of points assigned to the character.
        /// </summary>
        public int CharacterPoints;
        /// <summary>
        /// Gets or sets the maximum number of talent tiers.
        /// </summary>
        public int MaxTalentTiers;
        /// <summary>
        /// Gets or sets the bitmask value used to track creatures.
        /// </summary>
        public int TrackCreatureMask;
        /// <summary>
        /// Gets or sets the track resource mask.
        /// </summary>
        public int TrackResourceMask;
        ///<summary>
        ///Represents the level of expertise in the character's main hand combat ability.
        ///This member stores the value as an integer.
        ///</summary>
        public int MainhandExpertise;
        /// <summary>
        /// Represents the level of expertise in using the offhand for a specific character.
        /// </summary>
        public int OffhandExpertise;
        /// <summary>
        /// Gets or sets the expertise level in ranged combat.
        /// </summary>
        public int RangedExpertise;
        /// <summary>
        /// Represents the combat rating expertise of the object.
        /// </summary>
        public int CombatRatingExpertise;
        /// <summary>
        /// Gets or sets the block percentage.
        /// </summary>
        public int BlockPercentage;
        /// <summary>
        /// Represents the dodge percentage of a character.
        /// </summary>
        public int DodgePercentage;
        /// <summary>
        /// Represents the parry percentage of a character.
        /// </summary>
        public int ParryPercentage;
        /// <summary>
        /// Gets or sets the critical percentage of the object.
        /// </summary>
        public int CritPercentage;
        /// <summary>
        /// Gets or sets the ranged critical hit percentage.
        /// </summary>
        public int RangedCritPercentage;
        /// <summary>
        /// Represents the critical strike percentage for the offhand.
        /// </summary>
        public int OffhandCritPercentage;
        /// <summary>
        /// The SpellCritPercentage array represents the critical strike percentage of spells for different categories.
        /// Index 0 represents the critical strike percentage for category 0.
        /// Index 1 represents the critical strike percentage for category 1.
        /// Index 2 represents the critical strike percentage for category 2.
        /// Index 3 represents the critical strike percentage for category 3.
        /// Index 4 represents the critical strike percentage for category 4.
        /// Index 5 represents the critical strike percentage for category 5.
        /// Index 6 represents the critical strike percentage for category 6.
        /// </summary>
        public fixed int SpellCritPercentage[7];
        /// <summary>
        /// The ShieldBlock property represents the current shield block value.
        /// </summary>
        public int ShieldBlock;
        /// <summary>
        /// Gets or sets the percentage of critical hits blocked by the shield.
        /// </summary>
        public int ShieldBlockCritPercentage;
        /// <summary>
        /// Gets or sets the Mastery value.
        /// </summary>
        public int Mastery;
        /// <summary>
        /// Represents the Player versus Player (PvP) power damage.
        /// </summary>
        public int PvpPowerDamage;
        /// <summary>
        /// Gets or sets the PVP power for healing.
        /// </summary>
        public int PvpPowerHealing;
        /// <summary>
        /// Represents an array of explored zones.
        /// </summary>
        public fixed int ExploredZones[200];
        /// <summary>
        /// The bonus pool available for the rest state.
        /// </summary>
        public int RestStateBonusPool;
        /// <summary>
        /// The array that stores the values of modified damage done for 7 different positions.
        /// </summary>
        public fixed int ModDamageDonePos[7];
        ///<summary>
        /// Represents an array of negative modified damage values.
        /// The array has a fixed length of 7 elements.
        ///</summary>
        public fixed int ModDamageDoneNeg[7];
        /// <summary>
        /// Gets or sets the modified damage done percentage for each of the seven elements.
        /// </summary>
        public fixed int ModDamageDonePercent[7];
        ///<summary>
        /// Gets or sets the value representing the amount of positive healing done by a character.
        ///</summary>
        public int ModHealingDonePos;
        /// <summary>
        /// Represents the percentage of healing modification.
        /// </summary>
        public int ModHealingPercent;
        /// <summary>
        /// Gets or sets the percentage of healing done by the player.
        /// </summary>
        public int ModHealingDonePercent;
        /// <summary>
        /// Represents the percentage of periodic healing done.
        /// </summary>
        public int ModPeriodicHealingDonePercent;
        /// <summary>
        /// The array storing the weapon damage multipliers for different types of weapons.
        /// </summary>
        public fixed int WeaponDmgMultipliers[3];
        /// <summary>
        /// Represents the modified spell power percentage.
        /// </summary>
        public int ModSpellPowerPercent;
        /// <summary>
        /// Gets or sets the percentage value of the resilience modification.
        /// </summary>
        public int ModResiliencePercent;
        /// <summary>
        /// Gets or sets the value that overrides the spell power by a specified percentage.
        /// </summary>
        public int OverrideSpellPowerByAppercent;
        /// <summary>
        /// Gets or sets the override AP by spell power percent.
        /// </summary>
        public int OverrideApbySpellPowerPercent;
        /// <summary>
        /// The target resistance for modification.
        /// </summary>
        public int ModTargetResistance;
        /// <summary>
        /// Gets or sets the target's physical resistance modifier.
        /// </summary>
        public int ModTargetPhysicalResistance;
        /// <summary>
        /// Gets or sets the maximum rank achieved during the lifetime of an object.
        /// </summary>
        public int LifetimeMaxRank;
        /// <summary>
        /// Represents the self resurrection spell.
        /// </summary>
        public int SelfResSpell;
        /// <summary>
        /// Represents the number of player versus player medals earned.
        /// </summary>
        public int PvpMedals;
        /// <summary>
        /// Gets or sets the buyback price for each of the 12 months.
        /// </summary>
        public fixed int BuybackPrice[12];
        /// <summary>
        /// Array to store the buyback timestamps for 12 months.
        /// </summary>
        public fixed int BuybackTimestamp[12];
        /// <summary>
        /// Represents the number of honorable kills achieved yesterday.
        /// </summary>
        public int YesterdayHonorableKills;
        /// <summary>
        /// Gets or sets the total number of honorable kills during the lifetime of a player.
        /// </summary>
        public int LifetimeHonorableKills;
        /// <summary>
        /// Gets or sets the index of the watched faction.
        /// </summary>
        public int WatchedFactionIndex;
        /// <summary>
        /// The array of combat ratings.
        /// </summary>
        public fixed int CombatRatings[27];
        /// <summary>
        /// Gets or sets the player versus player information.
        /// </summary>
        /// <remarks>
        /// This array stores the player versus player information for the player in a fixed size of 24 integers.
        /// </remarks>
        public fixed int PvpInfo[24];
        /// <summary>
        /// Gets or sets the maximum level.
        /// </summary>
        public int MaxLevel;
        /// <summary>
        /// Gets or sets the array of Rune regeneration values.
        /// The array has a length of 4, where each index represents a specific Rune type.
        /// </summary>
        public fixed int RuneRegen[4];
        /// <summary>
        /// Represents an array of 4 integers for storing the no reagent cost mask.
        /// </summary>
        public fixed int NoReagentCostMask[4];
        /// <summary>
        /// Represents an array of glyph slots used for storing integers.
        /// </summary>
        public fixed int GlyphSlots[6];
        /// <summary>
        /// Gets or sets the collection of glyphs for the designated code.
        /// </summary>
        public fixed int Glyphs[6];
        /// <summary>
        /// Gets or sets the number of enabled glyph slots.
        /// </summary>
        public int GlyphSlotsEnabled;
        /// <summary>
        /// The spell power of the pet.
        /// </summary>
        public int PetSpellPower;
        /// <summary>
        /// Represents an array of fixed integers used for research purposes.
        /// </summary>
        public fixed int Researching[8];
        /// <summary>
        /// Represents the profession skill line for a character.
        /// </summary>
        public ulong ProfessionSkillLine;
        ///<summary>Gets or sets the modifier for UiHit.</summary>
        public int UiHitModifier;
        /// <summary>
        /// The spell hit modifier for the UI.
        /// </summary>
        public int UiSpellHitModifier;
        /// <summary>
        /// Gets or sets the time offset for the home realm. 
        /// </summary>
        public int HomeRealmTimeOffset;
        /// <summary>
        /// Represents the current modified haste value of a pet.
        /// </summary>
        public int ModPetHaste;
        /// <summary>
        /// Gets or sets the unique identifier of the summoned battle pet.
        /// </summary>
        public ulong SummonedBattlePetGuid;
        /// <summary>
        /// Gets or sets the identifier for overriding spells.
        /// </summary>
        public int OverrideSpellsId;
        /// <summary>
        /// Gets or sets the bonus faction ID for Looking for Group.
        /// </summary>
        public int LfgBonusFactionId;
        /// <summary>
        /// The identifier for the loot specialization.
        /// </summary>
        public int LootSpecId;
        /// <summary>
        /// Gets or sets the PvP type for the current zone.
        /// </summary>
        public int OverrideZonePvptype;
        /// <summary>
        /// Gets or sets the Item Level Delta.
        /// </summary>
        public int ItemLevelDelta;
    }
}