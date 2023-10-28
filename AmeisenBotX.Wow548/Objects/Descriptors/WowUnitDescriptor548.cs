using System.Collections.Specialized;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a namespace for objects related to World of Warcraft unit descriptors with ID 548.
/// </summary>
namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a World of Warcraft unit with ID 548.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowUnitDescriptor548
    {
        /// <summary>
        /// Represents the charm value as an unsigned 64-bit integer.
        /// </summary>
        public ulong Charm;
        /// <summary>
        /// Represents the summon property of an object, of type ulong.
        /// </summary>
        public ulong Summon;
        /// <summary>
        /// Represents a critter with a unique identifier stored as an unsigned long integer.
        /// </summary>
        public ulong Critter;
        /// <summary>
        /// Represents the ID of the entity that charmed this instance.
        /// </summary>
        public ulong CharmedBy;
        /// <summary>
        /// Represents the unique identifier for the entity that summoned this instance.
        /// </summary>
        public ulong SummonedBy;
        /// <summary>
        /// The field that represents the ID of the user who created this object.
        /// </summary>
        public ulong CreatedBy;
        /// <summary>
        /// Represents the DemonCreator field, which is a ulong value.
        /// </summary>
        public ulong DemonCreator;
        /// <summary>
        /// Gets or sets the target value as an unsigned long integer.
        /// </summary>
        public ulong Target;
        /// <summary>
        /// The unique identifier of the battle pet companion.
        /// </summary>
        public ulong BattlePetCompanionGuid;
        /// <summary>
        /// Represents a channel object of type ulong.
        /// </summary>
        public ulong ChannelObject;
        /// <summary>
        /// Represents the spell used by the channel, specified as an integer value.
        /// </summary>
        public int ChannelSpell;
        /// <summary>
        /// Gets or sets the Home Realm that summoned the object.
        /// </summary>
        public int SummonedByHomeRealm;
        /// <summary>
        /// Represents the race of a character as a byte value.
        /// </summary>
        public byte Race;
        /// <summary>
        /// Represents a public byte  Class.
        /// </summary>
        public byte Class;
        /// <summary>
        /// Represents the power type of an entity.
        /// </summary>
        public byte PowerType;
        /// <summary>
        /// Represents the gender of an individual.
        /// </summary>
        public byte Gender;
        /// <summary>
        /// Gets or sets the display power.
        /// </summary>
        public int DisplayPower;
        /// <summary>
        /// Gets or sets the value representing the overridden display power identifier.
        /// </summary>
        public int OverrideDisplayPowerId;
        /// <summary>
        /// Represents the health value of an entity.
        /// </summary>
        public int Health;
        /// <summary>
        /// Represents the Power1 variable which stores an integer value. 
        /// </summary>
        public int Power1;
        /// <summary>
        /// The power of 2.
        /// </summary>
        public int Power2;
        /// <summary>
        /// Gets or sets the power raised to the third exponent.
        /// </summary>
        public int Power3;
        /// <summary>
        /// Gets or sets the value of the <see cref="Power4"/> property.
        /// </summary>
        public int Power4;
        /// <summary>
        /// Represents the power of 5.
        /// </summary>
        public int Power5;
        /// <summary>
        /// Gets or sets the maximum health value.
        /// </summary>
        public int MaxHealth;
        /// <summary>
        /// Gets or sets the maximum power level 1.
        /// </summary>
        public int MaxPower1;
        /// <summary>
        /// Gets or sets the maximum power of 2.
        /// </summary>
        public int MaxPower2;
        /// <summary>
        /// Gets or sets the maximum power of 3.
        /// </summary>
        public int MaxPower3;
        /// <summary>
        /// Represents the maximum power of 4.
        /// </summary>
        public int MaxPower4;
        /// <summary>
        /// Gets or sets the maximum power level of 5.
        /// </summary>
        public int MaxPower5;
        /// <summary>
        /// Gets or sets the flat power regeneration modifier for each slot.
        /// </summary>
        public fixed int PowerRegenFlatModifier[5];
        /// <summary>
        /// Represents an array of fixed integers that represent the flat modifier for power regeneration when interrupted.
        /// The array has a length of 5.
        /// </summary>
        public fixed int PowerRegenInterruptedFlatModifier[5];
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level;
        /// <summary>
        /// Gets or sets the effective level.
        /// </summary>
        public int EffectiveLevel;
        /// <summary>
        /// Represents a faction template.
        /// </summary>
        public int FactionTemplate;
        /// <summary>
        /// Gets or sets the VirtualItemId array.
        /// </summary>
        public fixed int VirtualItemId[3];
        /// <summary>
        /// The first set of flags for manipulating and storing boolean values efficiently.
        /// </summary>
        public BitVector32 Flags1;
        /// <summary>
        /// Represents a 32-bit vector that contains a collection of Boolean flags.
        /// </summary>
        public BitVector32 Flags2;
        /// <summary>
        /// The AuraState property represents the current state of the Aura.
        /// </summary>
        public int AuraState;
        /// <summary>
        /// Represents the base time duration for an attack round, measured in milliseconds.
        /// </summary>
        public ulong AttackRoundBaseTime;
        /// <summary>
        /// The base time it takes to complete a ranged attack round.
        /// </summary>
        public int RangedAttackRoundBaseTime;
        /// <summary>
        /// Gets or sets the bounding radius of an object.
        /// </summary>
        public int BoundingRadius;
        /// <summary>
        /// Represents the combat reach of an entity in the game.
        /// The combat reach is a measure of the distance that an entity can reach with its attacks.
        /// </summary>
        public float CombatReach;
        /// <summary>
        /// Gets or sets the display ID.
        /// </summary>
        public int DisplayId;
        /// <summary>
        /// Represents the native display ID.
        /// </summary>
        public int NativeDisplayId;
        /// <summary>
        /// The display ID of the mount.
        /// </summary>
        public int MountDisplayId;
        /// <summary>
        /// Gets or sets the minimum damage value.
        /// </summary>
        public int MinDamage;
        /// <summary>
        /// Gets or sets the maximum damage value.
        /// </summary>
        public int MaxDamage;
        /// <summary>
        /// Gets or sets the minimum amount of damage dealt by the off-hand weapon.
        /// </summary>
        public int MinOffHandDamage;
        /// <summary>
        /// Gets or sets the maximum damage that can be dealt with the off-hand weapon.
        /// </summary>
        public int MaxOffHandDamage;
        /// <summary>
        /// Represents the animation tier for a particular object.
        /// </summary>
        public int AnimTier;
        /// <summary>
        /// The variable to store the pet's number.
        /// </summary>
        public int PetNumber;
        /// <summary>
        /// Gets or sets the timestamp for the pet name.
        /// </summary>
        public int PetNameTimestamp;
        /// <summary>
        /// Represents the amount of experience a pet has gained.
        /// </summary>
        public int PetExperience;
        /// <summary>
        /// The next level experience required for the pet.
        /// </summary>
        public int PetNextLevelExperience;
        /// <summary>
        /// Gets or sets the modified casting speed value.
        /// </summary>
        public int ModCastingSpeed;
        /// <summary>
        /// Represents the modifier for spell haste.
        /// </summary>
        public int ModSpellHaste;
        /// <summary>
        /// The ModHaste property represents the modulus of haste. 
        /// It is an integer value indicating the haste modifier.
        /// </summary>
        public int ModHaste;
        /// <summary>
        /// Gets or sets the modified ranged haste value.
        /// </summary>
        public int ModRangedHaste;
        /// <summary>
        /// Gets or sets the value of the ModHasteRegen property.
        /// </summary>
        public int ModHasteRegen;
        /// <summary>
        /// Gets or sets the spell that created this object.
        /// </summary>
        public int CreatedBySpell;
        /// <summary>
        /// Represents a BitVector32 for storing NPC flags.
        /// </summary>
        public BitVector32 NpcFlags1;
        /// <summary>
        /// Represents a BitVector32 that contains flags related to NPC (Non-Player Character) actions and behaviors.
        /// </summary>
        public BitVector32 NpcFlags2;
        /// <summary>
        /// Represents the emotive state of an NPC.
        /// </summary>
        public int NpcEmotestate;
        ///<summary>
        /// Represents an array of fixed length storing integer values for statistics.
        ///</summary>
        public fixed int Stats[5];
        /// <summary>
        /// Represents the positional buff statistics.
        /// </summary>
        public fixed int StatPosBuff[5];
        /// <summary>
        /// Represents an array of negative stat buffs.
        /// </summary>
        public fixed int StatNegBuff[5];
        /// <summary>
        /// Gets or sets the resistances for various elements.
        /// The array contains 7 elements representing different resistances.
        /// </summary>
        public fixed int Resistances[7];
        /// <summary>
        /// Array of positive resistance buff modifiers.
        /// </summary>
        public fixed int ResistanceBuffModsPositive[7];
        /// <summary>
        /// An array representing negative resistance buff modifiers for different attributes. 
        /// The index of the array corresponds to the attribute being modified.
        /// </summary>
        public fixed int ResistanceBuffModsNegative[7];
        /// <summary>
        /// Represents the base mana value.
        /// </summary>
        public int BaseMana;
        /// <summary>
        /// Gets or sets the base health value.
        /// </summary>
        public int BaseHealth;
        /// <summary>
        /// Gets or sets the shapeshift form.
        /// </summary>
        public int ShapeshiftForm;
        /// <summary>
        /// Gets or sets the attack power of the character. It represents the strength of their attacks.
        /// </summary>
        public int AttackPower;
        /// <summary>
        /// The positive attack power modifier.
        /// </summary>
        public int AttackPowerModPos;
        /// <summary>
        /// The negative attack power modifier.
        /// </summary>
        public int AttackPowerModNeg;
        /// <summary>
        /// Gets or sets the attack power multiplier.
        /// </summary>
        public int AttackPowerMultiplier;
        /// <summary>
        /// Gets or sets the ranged attack power.
        /// </summary>
        public int RangedAttackPower;
        ///<summary>
        /// The positive modification value for the ranged attack power.
        ///</summary>
        public int RangedAttackPowerModPos;
        /// <summary>
        /// Represents the negative ranged attack power modifier.
        /// </summary>
        public int RangedAttackPowerModNeg;
        ///<summary>
        ///The multiplier for calculating the attack power of ranged attacks.
        ///</summary>
        public int RangedAttackPowerMultiplier;
        /// <summary>
        /// Represents the minimum ranged damage value.
        /// </summary>
        public int MinRangedDamage;
        /// <summary>
        /// Gets or sets the maximum ranged damage value.
        /// </summary>
        public int MaxRangedDamage;
        /// <summary>
        /// Gets or sets the power cost modifier for each day of the week.
        /// Index 0 represents Sunday and index 6 represents Saturday.
        /// </summary>
        public fixed int PowerCostModifier[7];
        /// <summary>
        /// Represents an array containing power cost multipliers for 7 different powers.
        /// </summary>
        public fixed int PowerCostMultiplier[7];
        /// <summary>
        /// Represents the maximum health modifier.
        /// </summary>
        public int MaxHealthModifier;
        /// <summary>
        /// Gets or sets the hover height value.
        /// </summary>
        public int HoverHeight;
        /// <summary>
        /// Gets or sets the minimum item level.
        /// </summary>
        public int MinItemLevel;
        /// <summary>
        /// Represents the maximum item level.
        /// </summary>
        public int MaxItemLevel;
        /// <summary>
        /// The level of the wild battle pet.
        /// </summary>
        public int WildBattlePetLevel;
        /// <summary>
        /// Represents the timestamp of the battle pet companion name.
        /// </summary>
        public int BattlePetCompanionNameTimestamp;
        /// <summary>
        /// Gets or sets the ID of the interaction spell.
        /// </summary>
        public int InteractSpellId;
    }
}