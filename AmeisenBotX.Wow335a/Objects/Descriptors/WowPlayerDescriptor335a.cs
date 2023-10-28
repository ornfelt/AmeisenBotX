using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a namespace for WoW player descriptors.
/// </summary>
namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a WoW player.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowPlayerDescriptor335a
    {
        /// <summary>
        /// Represents the arbiter responsible for overseeing and resolving duels.
        /// </summary>
        public ulong DuelArbiter;
        /// <summary>
        /// Represents an integer value used for flags.
        /// </summary>
        public int Flags;
        /// <summary>
        /// The ID of the guild.
        /// </summary>
        public int GuildId;
        /// <summary>
        /// The rank of the guild.
        /// </summary>
        public int GuildRank;
        /// <summary>
        /// Represents the first 4 bytes of the player's data.
        /// </summary>
        public fixed byte PlayerBytes0[4];
        ///<summary>
        ///Gets or sets the fixed array of bytes representing the player data.
        ///</summary>
        public fixed byte PlayerBytes1[4];
        /// <summary>
        /// Gets or sets the byte array representing the player bytes.
        /// </summary>
        public fixed byte PlayerBytes2[4];
        /// <summary>
        /// The number of teams participating in the duel.
        /// </summary>
        public int DuelTeam;
        /// <summary>
        /// Gets or sets the timestamp of the guild.
        /// </summary>
        public int GuildTimestamp;
        /// <summary>
        /// Represents a quest log entry.
        /// </summary>
        public QuestlogEntry QuestlogEntry1;
        /// <summary>
        /// The second quest log entry.
        /// </summary>
        public QuestlogEntry QuestlogEntry2;
        /// <summary>
        /// A public field representing the third quest log entry.
        /// </summary>
        public QuestlogEntry QuestlogEntry3;
        /// <summary>
        /// Represents the fourth entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry4;
        /// <summary>
        /// Represents the fifth entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry5;
        /// <summary>
        /// The questlog entry object for entry number 6.
        /// </summary>
        public QuestlogEntry QuestlogEntry6;
        /// <summary>
        /// Represents the seventh entry in a quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry7;
        /// <summary>
        /// Represents the eighth quest log entry.
        /// </summary>
        public QuestlogEntry QuestlogEntry8;
        /// <summary>
        /// Represents the ninth quest log entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry9;
        /// <summary>
        /// Represents a quest log entry for the 10th quest.
        /// </summary>
        public QuestlogEntry QuestlogEntry10;
        /// <summary>
        /// Represents a quest log entry.
        /// </summary>
        public QuestlogEntry QuestlogEntry11;
        /// <summary>
        /// Represents the 12th entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry12;
        /// <summary>
        /// Represents Questlog Entry 13.
        /// </summary>
        public QuestlogEntry QuestlogEntry13;
        /// <summary>
        /// Represents the 14th entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry14;
        /// <summary>
        /// Represents the 15th entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry15;
        /// <summary>
        /// Represents the 16th entry in a quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry16;
        /// <summary>
        /// Represents Questlog Entry number 17.
        /// </summary>
        public QuestlogEntry QuestlogEntry17;
        /// <summary>
        /// Holds data for Questlog Entry 18.
        /// </summary>
        public QuestlogEntry QuestlogEntry18;
        /// <summary>
        /// Represents the Questlog Entry 19.
        /// </summary>
        public QuestlogEntry QuestlogEntry19;
        /// <summary>
        /// Represents the 20th entry in the quest log.
        /// </summary>
        public QuestlogEntry QuestlogEntry20;
        ///<summary>
        /// Represents the 21st entry in the quest log.
        /// Use this variable to access and manipulate the quest log entry.
        ///</summary>
        public QuestlogEntry QuestlogEntry21;
        /// <summary>
        /// Represents a quest log entry with the ID of 22.
        /// </summary>
        public QuestlogEntry QuestlogEntry22;
        /// <summary>
        /// Represents Questlog Entry 23.
        /// </summary>
        public QuestlogEntry QuestlogEntry23;
        /// <summary>
        /// Represents the quest log entry with ID 24.
        /// </summary>
        public QuestlogEntry QuestlogEntry24;
        /// <summary>
        /// Represents a quest log entry with the ID 25.
        /// </summary>
        public QuestlogEntry QuestlogEntry25;
        /// <summary>
        /// A public visible item enchantment instance.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment1;
        /// <summary>
        /// Represents a visible enchantment for an item.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment2;
        /// <summary>
        /// The third visible item enchantment.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment3;
        /// <summary>
        /// Represents a visible item enchantment for the fourth slot.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment4;
        /// <summary>
        /// Represents the fifth visible item enchantment in a game.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment5;
        /// <summary>
        /// Represents the seventh visible item enchantment in the list of visible item enchantments.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment6;
        /// <summary>
        /// Represents the 7th visible item enchantment.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment7;
        /// <summary>
        /// Represents the eighth visible item enchantment.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment8;
        /// <summary>
        /// Represents a visible item enchantment.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment9;
        /// <summary>
        /// Public field representing the VisibleItemEnchantment object with a numerical suffix of 10.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment10;
        /// <summary>
        /// Represents a visible item enchantment.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment11;
        /// <summary>
        /// Represents a visible enchantment on an item.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment12;
        /// <summary>
        /// Represents the visible enchantment for a specific item.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment13;
        /// <summary>
        /// The visible item enchantment with the ID 14.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment14;
        /// <summary>
        /// Represents a visible item enchantment with an ID of 15.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment15;
        /// <summary>
        /// Represents a visible enchantment for an item.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment16;
        /// <summary>
        /// The visible item enchantment with an index of 17.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment17;
        /// <summary>
        /// Represents an enchantment for a visible item.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment18;
        /// <summary>
        /// Represents a visible enchantment applied to an item.
        /// </summary>
        public VisibleItemEnchantment VisibleItemEnchantment19;
        /// <summary>
        /// Represents the chosen title, stored as an integer.
        /// </summary>
        public int ChosenTitle;
        /// <summary>
        /// Represents the level of fake inebriation.
        /// </summary>
        public int FakeInebriation;
        /// <summary>
        /// Represents the player's pad in the WowPlayer class.
        /// </summary>
        public int WowPlayerPad;
        /// <summary>
        /// Represents an array of inventory slots with a fixed size of 23.
        /// </summary>
        public fixed ulong InventorySlots[23];
        /// <summary>
        /// Represents an array of 16 fixed-size slots in the backpack.
        /// Each slot is able to store an unsigned long value.
        /// </summary>
        public fixed ulong BackpackSlots[16];
        /// <summary>
        /// Represents an array of bank slots, each slot has a corresponding index.
        /// </summary>
        public fixed ulong BankSlots[28];
        /// <summary>
        /// Array representing the number of bank bag slots.
        /// </summary>
        public fixed ulong BankBagSlots[7];
        /// <summary>
        /// Array that represents the vendor's buy back slots, with a fixed size of 12.
        /// </summary>
        public fixed ulong VendorBuyBackSlots[12];
        /// <summary>
        /// Represents an array of 32 slots that hold keys in the keyring.
        /// Each slot can hold an unsigned long integer value.
        /// </summary>
        public fixed ulong KeyringSlots[32];
        /// <summary>
        /// Represents an array of token slots, each slot stores an unsigned long value.
        /// The maximum number of slots in the array is 32.
        /// </summary>
        public fixed ulong TokenSlots[32];
        /// <summary>
        /// Represents the Farsight value.
        /// </summary>
        public ulong Farsight;
        /// <summary>
        /// Represents the number of known titles as an unsigned long value.
        /// </summary>
        public ulong KnownTitles0;
        /// <summary>
        /// Represents a public field for storing a known title value as an unsigned long integer.
        /// </summary>
        public ulong KnownTitles1;
        /// <summary>
        /// The second known title stored as an unsigned long integer.
        /// </summary>
        public ulong KnownTitles2;
        /// <summary>
        /// Represents the known currencies as an unsigned long integer.
        /// </summary>
        public ulong KnownCurrencies;
        /// <summary>
        /// The experience points of the player.
        /// </summary>
        public int Xp;
        /// <summary>
        /// Gets or sets the next level experience points.
        /// </summary>
        public int NextLevelXp;
        /// <summary>
        /// Array of fixed length containing 768 short values representing skill information.
        /// </summary>
        public fixed short SkillInfos[768];
        /// <summary>
        /// Gets or sets the number of character points for CharacterPoints1.
        /// </summary>
        public int CharacterPoints1;
        /// <summary>
        /// The total number of character points for the second character.
        /// </summary>
        public int CharacterPoints2;
        /// <summary>
        /// Represents the number of creatures being tracked.
        /// </summary>
        public int TrackCreatures;
        /// <summary>
        /// Gets or sets the number of resources being tracked.
        /// </summary>
        public int TrackResources;
        /// <summary>
        /// The percentage of blocks in the code.
        /// </summary>
        public float BlockPercentage;
        /// <summary>
        /// Represents the percentage chance to dodge an incoming attack.
        /// </summary>
        public float DodgePercentage;
        /// <summary>
        /// The parry percentage of a character's defense.
        /// </summary>
        public float ParryPercentage;
        /// <summary>
        /// Gets or sets the expertise level of the object.
        /// </summary>
        public int Expertise;
        /// <summary>
        /// Represents the level of expertise in using the offhand weapon.
        /// </summary>
        public int OffhandExpertise;
        /// <summary>
        /// Represents the critical hit percentage for a particular entity.
        /// </summary>
        public float CritPercentage;
        /// <summary>
        /// The percentage of critical hits for ranged attacks.
        /// </summary>
        public float RangedCritPercentage;
        /// <summary>
        /// Gets or sets the offhand critical hit percentage.
        /// </summary>
        public float OffhandCritPercentage;
        /// <summary>
        /// Gets or sets the percentage of spell critical hits.
        /// </summary>
        public float SpellCritPercentage;
        /// <summary>
        /// Gets or sets the value indicating the shield block of the object.
        /// </summary>
        public int ShieldBlock;
        /// <summary>
        /// Gets or sets the percentage of block chance for a shield.
        /// </summary>
        public float ShieldBlockPercentage;
        /// <summary>
        /// Represents an array of 512 bytes indicating the explored zones.
        /// </summary>
        public fixed byte ExploredZones[512];
        /// <summary>
        /// Gets or sets the amount of rest state experience.
        /// </summary>
        public int RestStateExpirience;
        /// <summary>
        /// Gets or sets the value of FieldCoinage.
        /// </summary>
        public int FieldCoinage;
        /// <summary>
        /// Represents the array that stores the modified damage done values for different positions.
        /// This array has a fixed length of 7 elements.
        /// </summary>
        public fixed int ModDamageDonePos[7];
        /// <summary>
        /// Represents an array of mod damage values for negative damage types.
        /// </summary>
        public fixed int ModDamageDoneNeg[7];
        /// <summary>
        /// Gets or sets the modified damage done percentage for each day of the week.
        /// </summary>
        public fixed int ModDamageDonePercentage[7];
        /// <summary>
        /// Gets or sets the amount of modified healing done.
        /// </summary>
        public int ModHealingDone;
        /// <summary>
        /// Gets or sets the modification healing percentage.
        /// </summary>
        public float ModHealingPercentage;
        /// <summary>
        /// Gets or sets the percentage of modified healing done.
        /// </summary>
        public float ModHealingDonePercentage;
        /// <summary>
        /// Represents the modified target resistance value.
        /// </summary>
        public int ModTargetResistance;
        /// <summary>
        /// Gets or sets the value of the target's physical resistance modifier.
        /// </summary>
        public int ModTargetPhysicalResistance;
        /// <summary>
        /// The fixed byte array that stores the data for the player in four consecutive bytes.
        /// </summary>
        public fixed byte PlayerBytes3[4];
        /// <summary>
        /// Gets or sets the ID of the ammo.
        /// </summary>
        public int AmmoId;
        /// <summary>
        /// Represents the self-resurrection spell level.
        /// </summary>
        public int SelfResSpell;
        /// <summary>
        /// The number of PVP medals earned by the player.
        /// </summary>
        public int PvpMedals;
        /// <summary>
        /// Represents an array of fixed buyback prices.
        /// </summary>
        public fixed int BuybackPrices[12];
        /// <summary>
        /// Gets or sets the array of buyback timestamps with a fixed length of 12.
        /// </summary>
        public fixed int BuybackTimestamps[12];
        /// <summary>
        /// Gets or sets the number of kills for each player in a fixed-length array.
        /// </summary>
        public fixed short Kills[2];
        /// <summary>
        /// Represents the contribution made today.
        /// </summary>
        public int TodayContribution;
        /// <summary>
        /// The total contribution made yesterday.
        /// </summary>
        public int YesterdayContribution;
        /// <summary>
        /// Represents the number of lifetime honorable kills.
        /// </summary>
        public int LifetimeHonorableKills;
        /// <summary>
        /// Represents an array of 4 fixed bytes that stores player data.
        /// </summary>
        public fixed byte PlayerBytes4[4];
        /// <summary>
        /// Represents the index of the watched faction.
        /// </summary>
        public int WatchedFactionIndex;
        /// <summary>
        /// Represents an array of combat ratings with a fixed length of 25.
        /// </summary>
        public fixed int CombatRatings[25];
        /// <summary>
        /// Represents an array of arena team information with a fixed length of 21.
        /// </summary>
        public fixed int ArenaTeamInfo[21];
        /// <summary>
        /// Represents the amount of honor currency.
        /// </summary>
        public int HonorCurrency;
        /// <summary>
        /// Represents the amount of currency available in the arena.
        /// </summary>
        public int ArenaCurrency;
        /// <summary>
        /// Represents the maximum level value.
        /// </summary>
        public int MaxLevel;
        /// <summary>
        /// Represents an array of daily quests.
        /// </summary>
        public fixed int DailyQuests[25];
        /// <summary>
        /// Gets or sets the array of Rune regeneration rates.
        /// </summary>
        public fixed float RuneRegens[4];
        /// <summary>
        /// This array represents the cost of reagents for a specific operation. 
        /// It contains three fixed integers, where index 0 represents the cost of the first reagent, 
        /// index 1 represents the cost of the second reagent, and index 2 represents the cost of the third reagent.
        /// </summary>
        public fixed int NoReagentCosts[3];
        /// <summary>
        /// Represents an array of fixed-size integer slots used for storing glyph information.
        /// </summary>
        public fixed int GlyphSlots[6];
        /// <summary>
        /// The array that stores the glyphs used in the code.
        /// </summary>
        public fixed int Glyphs[6];
        /// <summary>
        /// Gets or sets the number of glyphs that are currently enabled.
        /// </summary>
        public int GlyphsEnabled;
        /// <summary>
        /// Gets or sets the spell power of the pet.
        /// </summary>
        public int PetSpellPower;

        /// <summary>
        /// Represents the end offset value, which is a constant integer.
        /// </summary>
        public static readonly int EndOffset = 4712;
    }
}