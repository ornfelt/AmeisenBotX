using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a World of Warcraft unit in version 3.3.5a.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowUnitDescriptor335a
    {
        ///<summary>
        ///Gets or sets the Charm value.
        ///</summary>
        public ulong Charm;
        /// <summary>
        /// Represents the summon value.
        /// </summary>
        public ulong Summon;
        /// <summary>
        /// Represents a unique identifier for a critter.
        /// </summary>
        public ulong Critter;
        /// <summary>
        /// Represents the identifier of an entity that is charmed by another entity.
        /// </summary>
        public ulong CharmedBy;
        /// <summary>
        /// Gets or sets the unique identifier of the entity that summoned the current object.
        /// </summary>
        public ulong SummonedBy;
        /// <summary>
        /// Gets or sets the value representing the user ID of the creator.
        /// </summary>
        public ulong CreatedBy;
        /// <summary>
        /// The target value of type ulong.
        /// </summary>
        public ulong Target;
        /// <summary>
        /// Represents a channel object.
        /// </summary>
        public ulong ChannelObject;
        /// <summary>
        /// Gets or sets the channel spell.
        /// </summary>
        public int ChannelSpell;
        /// <summary>
        /// Gets or sets the race of the entity.
        /// </summary>
        public byte Race;
        /// <summary>
        /// Represents a public byte class.
        /// </summary>
        public byte Class;
        /// <summary>
        /// Represents the gender of an individual.
        /// </summary>
        public byte Gender;
        /// <summary>
        /// Represents the power type stored as a byte value.
        /// </summary>
        public byte PowerType;
        /// <summary>
        /// Gets or sets the health value.
        /// </summary>
        public int Health;
        /// <summary>
        /// Gets or sets the Power1 property.
        /// </summary>
        public int Power1;
        /// <summary>
        /// Represents the exponentiation of a number to the power of 2.
        /// </summary>
        public int Power2;
        /// <summary>
        /// Gets or sets the value of Power3, representing the third power.
        /// </summary>
        public int Power3;
        /// <summary>
        /// Represents a public integer value named Power4.
        /// </summary>
        public int Power4;
        /// <summary>
        /// Represents the power of 5 for mathematical calculations.
        /// </summary>
        public int Power5;
        /// <summary>
        /// The Power6 property represents the value raised to the power of 6.
        /// </summary>
        public int Power6;
        /// <summary>
        /// Represents the Power7 value.
        /// </summary>
        public int Power7;
        /// <summary>
        /// Represents the maximum health value for an entity.
        /// </summary>
        public int MaxHealth;
        /// <summary>
        /// Gets or sets the maximum power value.
        /// </summary>
        public int MaxPower1;
        /// <summary>
        /// Gets or sets the maximum power of 2.
        /// </summary>
        public int MaxPower2;
        /// <summary>
        /// Represents the maximum power value 3.
        /// </summary>
        public int MaxPower3;
        /// <summary>
        /// The maximum power level for an entity.
        /// </summary>
        public int MaxPower4;
        /// <summary>
        /// The maximum power of 5 allowed.
        /// </summary>
        public int MaxPower5;
        /// <summary>
        /// Represents the maximum power value for version 6.
        /// </summary>
        public int MaxPower6;
        /// <summary>
        /// Represents the maximum power value for a given calculation, which is set to 7.
        /// </summary>
        public int MaxPower7;
        /// <summary>
        /// The array storing the power regeneration modifiers for different levels.
        /// </summary>
        public fixed float PowerRegenModifier[7];
        /// <summary>
        /// Represents the power regeneration interrupted modifier array.
        /// </summary>
        public fixed float PowerRegenInterruptedModifier[7];
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level;
        /// <summary>
        /// Gets or sets the faction template.
        /// </summary>
        public int FactionTemplate;
        /// <summary>
        /// Gets or sets the fixed array of three integers representing the virtual item slot IDs.
        /// </summary>
        public fixed int VirtualItemSlotId[3];
        /// <summary>
        /// Represents a BitVector32 object used to store a set of Boolean flags.
        /// </summary>
        public BitVector32 Flags1;
        /// <summary>
        /// Represents a 32-bit vector that can be used as a flag container.
        /// </summary>
        public BitVector32 Flags2;
        /// <summary>
        /// The AuraState property represents the current state of the Aura.
        /// </summary>
        public int AuraState;
        /// <summary>
        /// Represents the base attack time for a character at different levels.
        /// </summary>
        public fixed int BaseAttackTime[2];
        /// <summary>
        /// Represents the range attack time in milliseconds.
        /// </summary>
        public int RangeAttackTime;
        /// <summary>
        /// Gets or sets the bounding radius of the object.
        /// </summary>
        public float BoundingRadius;
        /// <summary>
        /// Represents the combat reach of a character or object in the game, measured in units.
        /// </summary>
        public float CombatReach;
        /// <summary>
        /// Gets or sets the display ID.
        /// </summary>
        public int DisplayId;
        /// <summary>
        /// Gets or sets the display ID of the native element.
        /// </summary>
        public int NativeDisplayId;
        /// <summary>
        /// Specifies the mount display ID.
        /// </summary>
        public int MountDisplayId;
        /// <summary>
        /// Gets or sets the minimum damage value.
        /// </summary>
        public float MinDamage;
        /// <summary>
        /// The maximum amount of damage that can be dealt.
        /// </summary>
        public float MaxDamage;
        ///<summary>
        ///Gets or sets the minimum damage inflicted by an offhand weapon.
        ///</summary>
        public float MinOffhandDamage;
        /// <summary>
        /// Gets or sets the maximum offhand damage.
        /// </summary>
        public float MaxOffhandDamage;
        /// <summary>
        /// Represents the number of bytes stored in the property Bytes1.
        /// </summary>
        public int Bytes1;
        /// <summary>
        /// Represents the pet number.
        /// </summary>
        public int PetNumber;
        /// <summary>
        /// Gets or sets the timestamp for the pet name.
        /// </summary>
        public int PetNameTimestamp;
        /// <summary>
        /// The experience level of the pet.
        /// </summary>
        public int PetExperience;
        /// <summary>
        /// Gets or sets the next level experience points required for the pet.
        /// </summary>
        public int PetNextLevelXp;
        /// <summary>
        /// Represents a compact representation of a collection of binary flags, as a 32-bit value.
        /// </summary>
        public BitVector32 DynamicFlags;
        /// <summary>
        /// The speed at which the object is being cast as a float value.
        /// </summary>
        public float CastSpeed;
        /// <summary>
        /// Represents the ID of the spell that created the object.
        /// </summary>
        public int CreatedBySpell;
        /// <summary>
        /// The flags representing various properties and states of an NPC.
        /// </summary>
        public BitVector32 NpcFlags;
        /// <summary>
        /// Represents the current emote state of the Non-Player Character (NPC).
        /// </summary>
        public int NpcEmoteState;
        /// <summary>
        /// Represents the statistic 0 value.
        /// </summary>
        public int Stat0;
        ///<summary>
        /// Gets or sets the value of Stat1.
        ///</summary>
        public int Stat1;
        /// <summary>
        /// Gets or sets the value of Stat2.
        /// </summary>
        public int Stat2;
        /// <summary>
        /// Represents the value of the Stat3 property.
        /// </summary>
        public int Stat3;
        /// <summary>
        /// Represents the value of Stat4.
        /// </summary>
        public int Stat4;
        /// <summary>
        /// Gets or sets the value of the PosStat0 property.
        /// </summary>
        public int PosStat0;
        /// <summary>
        /// Represents the position status 1.
        /// </summary>
        public int PosStat1;
        /// <summary>
        /// Represents the position statistic 2.
        /// </summary>
        public int PosStat2;
        /// <summary>
        /// The position status 3.
        /// </summary>
        public int PosStat3;
        /// <summary>
        /// Represents the position statistic 4.
        /// </summary>
        public int PosStat4;
        /// <summary>
        /// Represents a public integer variable named NegStat0.
        /// </summary>
        public int NegStat0;
        /// <summary>
        /// This is a public integer variable named NegStat1.
        /// </summary>
        public int NegStat1;
        /// <summary>
        /// Represents the integer variable for the negative statistic 2.
        /// </summary>
        public int NegStat2;
        /// <summary>
        /// The public integer NegStat3 represents a negative status 3.
        /// </summary>
        public int NegStat3;
        /// <summary>
        /// The integer representing the negative value of the statistical variable 4.
        /// </summary>
        public int NegStat4;
        /// <summary>
        /// Represents an array of resistances for different elements.
        /// </summary>
        public fixed int Resistances[7];
        /// <summary>
        /// Represents an array of positive resistance modifiers.
        /// The array has a fixed length of 7 and each element stores an integer value.
        /// </summary>
        public fixed int ResistanceModsBuffPositive[7];
        /// <summary>
        /// Represents an array of fixed integers that store the resistance modifications when they are negative.
        /// </summary>
        public fixed int ResistanceModsBuffNegative[7];
        /// <summary>
        /// Represents the base mana value of a character. 
        /// </summary>
        public int BaseMana;
        /// <summary>
        /// Represents the base health value of an object.
        /// </summary>
        public int BaseHealth;
        /// <summary>
        /// The number of bytes in the Bytes2 variable.
        /// </summary>
        public int Bytes2;
        /// <summary>
        /// Represents the attack power of a character.
        /// </summary>
        public int AttackPower;
        /// <summary>
        /// Gets or sets the array of attack power modifiers.
        /// </summary>
        public fixed short AttackPowerMods[2];
        /// <summary>
        /// Gets or sets the attack power multiplier.
        /// </summary>
        public float AttackPowerMultiplier;
        /// <summary>
        /// Gets or sets the numerical value representing the power of ranged attacks.
        /// </summary>
        public int RangedAttackPower;
        /// <summary>
        /// Array that represents the modified ranged attack power.
        /// the array contains 2 fixed short values.
        /// </summary>
        public fixed short RangedAttackPowerMods[2];
        /// <summary>
        /// The multiplier for the power of a ranged attack.
        /// </summary>
        public float RangedAttackPowerMultiplier;
        /// <summary>
        /// The minimum amount of damage that can be inflicted with ranged attacks.
        /// </summary>
        public float MinRangedDamage;
        /// <summary>
        /// Gets or sets the maximum ranged damage value.
        /// </summary>
        public float MaxRangedDamage;
        /// <summary>
        /// Gets or sets the power cost modifier for each day of the week.
        /// </summary>
        public fixed int PowerCostModifier[7];
        /// <summary>
        /// Represents an array that stores the power cost multiplier for each day of the week.
        /// </summary>
        public fixed float PowerCostMultiplier[7];
        /// <summary>
        /// The maximum health modifier value.
        /// </summary>
        public float MaxHealthModifier;
        /// <summary>
        /// Represents the hover height for an object. It is a public field and can be accessed directly.
        /// </summary>
        public float HoverHeight;
        /// <summary>
        /// Represents the padding value in WowUnits for a specific element.
        /// </summary>
        public int WowUnitPadding;

        /// <summary>
        /// The end offset value is a constant representing the integer value 568.
        /// </summary>
        public static readonly int EndOffset = 568;
    }
}