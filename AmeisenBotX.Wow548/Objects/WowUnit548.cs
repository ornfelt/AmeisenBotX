using AmeisenBotX.Common.Math;
using AmeisenBotX.Memory;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Flags;
using AmeisenBotX.Wow335a.Objects.Raw;
using AmeisenBotX.Wow548.Objects.Descriptors;
using AmeisenBotX.Wow548.Objects.Flags;
using System.Collections.Specialized;
using System.Text;

namespace AmeisenBotX.Wow548.Objects
{
    [Serializable]
    public unsafe class WowUnit548 : WowObject548, IWowUnit
    {
        /// <summary>
        /// Gets or sets the movement flags, which are represented by an unsigned integer value that can be nullable.
        /// </summary>
        protected uint? MovementFlags;
        /// <summary>
        /// Private field representing a table of World of Warcraft auras that implement the IWowAura interface.
        /// </summary>
        private IEnumerable<IWowAura>? AuraTable;

        /// <summary>
        /// Gets or sets the unit descriptor for WowUnitDescriptor548.
        /// </summary>
        private WowUnitDescriptor548? UnitDescriptor;

        /// <summary>
        /// Gets the count of Aura from the AuraTable.
        /// </summary>
        public int AuraCount => ReadAuraTable().Count();

        /// <summary>
        /// Gets all the Auras by reading the Aura table.
        /// </summary>
        public IEnumerable<IWowAura> Auras => ReadAuraTable();

        /// <summary>
        /// Gets the WoW class of the character.
        /// </summary>
        public WowClass Class => (WowClass)GetUnitDescriptor().Class;

        /// <summary>
        /// Gets the combat reach of the unit. 
        /// </summary>
        public float CombatReach => GetUnitDescriptor().CombatReach;

        /// <summary>
        /// Retrieves the ID of the currently casting spell from memory and returns it as an integer. If the memory read is successful, the value is returned; otherwise, the default value of 0 is returned.
        /// </summary>
        public int CurrentlyCastingSpellId => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.CurrentlyCastingSpellId), out int castingId) ? castingId : 0;

        /// <summary>
        /// Gets the ID of the currently channeling spell by reading the memory at the specified address and returning the result. If the memory read operation fails, returns 0.
        /// </summary>
        public int CurrentlyChannelingSpellId => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.CurrentlyChannelingSpellId), out int channelingId) ? channelingId : 0;

        /// <summary>
        /// Gets the display ID of the unit.
        /// </summary>
        public int DisplayId => GetUnitDescriptor().DisplayId;

        /// <summary>
        /// Gets the energy value from the power of the unit descriptor.
        /// </summary>
        public int Energy => GetUnitDescriptor().Power4;

        /// <summary>
        /// Calculates and returns the energy percentage of the bot's current energy level.
        /// </summary>
        public double EnergyPercentage => BotMath.Percentage(Energy, MaxEnergy);

        /// <summary>
        /// This property retrieves the faction template for the unit descriptor.
        /// </summary>
        public int FactionTemplate => GetUnitDescriptor().FactionTemplate;

        /// <summary>
        /// Gets the gender of the WowGender object by calling the GetUnitDescriptor() method and 
        /// casting the result to WowGender enumeration. 
        /// </summary>
        public WowGender Gender => (WowGender)GetUnitDescriptor().Gender;

        /// <summary>
        /// Gets the health of the unit based on the unit's descriptor.
        /// </summary>
        public int Health => GetUnitDescriptor().Health;

        /// <summary>
        /// Calculates and returns the health percentage of the bot.
        /// </summary>
        public double HealthPercentage => BotMath.Percentage(Health, MaxHealth);

        /// <summary>
        /// Gets the Holy Power of the unit's descriptor property.
        /// </summary>
        public int HolyPower => GetUnitDescriptor().Power2;

        /// <summary>
        /// Determines if the unit is currently auto-attacking.
        /// </summary>
        public bool IsAutoAttacking => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitIsAutoAttacking), out int isAutoAttacking) && isAutoAttacking == 1;

        /// <summary>
        /// Determines if the unit is dead by checking the health and unit flags.
        /// Returns true if the health is zero or the dead flag is set, and false if the feign death flag is set.
        /// </summary>
        public bool IsDead => (Health == 0 || UnitFlagsDynamic[(int)WowUnitDynamicFlags548.Dead]) && !UnitFlags2[(int)WowUnit2Flag.FeignDeath];

        /// <summary>
        /// Determines whether the entity is flying by checking if the movement flags contain the flying flag bit.
        /// </summary>
        public bool IsFlying => (GetMovementFlags() & 0x1000000) != 0;

        /// <summary>
        /// Determines if the unit is lootable based on the value of the WowUnitDynamicFlags548.Lootable flag.
        /// </summary>
        public bool IsLootable => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.Lootable];

        /// <summary>
        /// Gets a value indicating whether the unit is linked as a refer-a-friend.
        /// </summary>
        public bool IsReferAFriendLinked => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.ReferAFriendLinked];

        /// <summary>
        /// Checks if the unit has special information based on the specified dynamic flag.
        /// </summary>
        public bool IsSpecialInfo => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.SpecialInfo];

        /// <summary>
        /// Determines if the entity is currently swimming.
        /// </summary>
        public bool IsSwimming => (GetMovementFlags() & 0x100000) != 0;

        /// <summary>
        /// Gets a value indicating whether the unit is tagged by the current entity.
        /// </summary>
        public bool IsTaggedByMe => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.TaggedByMe];

        /// <summary>
        /// Returns a boolean value indicating whether the unit is tagged by any other unit.
        /// </summary>
        public bool IsTaggedByOther => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.TaggedByOther];

        /// <summary>
        /// Gets a value indicating whether the unit is tapped by all threat list.
        /// </summary>
        public bool IsTappedByAllThreatList => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.IsTappedByAllThreatList];

        /// <summary>
        /// Gets a boolean value indicating if the unit is being tracked.
        /// </summary>
        public bool IsTrackedUnit => UnitFlagsDynamic[(int)WowUnitDynamicFlags548.TrackUnit];

        /// <summary>
        /// Gets the level of the unit based on its descriptor.
        /// </summary>
        public int Level => GetUnitDescriptor().Level;

        /// <summary>
        /// Gets the amount of mana for the unit.
        /// </summary>
        public int Mana => GetUnitDescriptor().Power1;

        /// <summary>
        /// Calculates the percentage of mana remaining.
        /// </summary>
        public double ManaPercentage => BotMath.Percentage(Mana, MaxMana);

        /// <summary>
        /// Returns the maximum energy level of the unit.
        /// </summary>
        public int MaxEnergy => GetUnitDescriptor().MaxPower4;

        /// <summary>
        /// Gets the maximum health of the unit.
        /// </summary>
        public int MaxHealth => GetUnitDescriptor().MaxHealth;

        /// <summary>
        /// Gets the maximum holy power of the unit descriptor.
        /// </summary>
        public int MaxHolyPower => GetUnitDescriptor().MaxPower2;

        /// <summary>
        /// Gets the maximum mana of the unit.
        /// </summary>
        public int MaxMana => GetUnitDescriptor().MaxPower1;

        /// <summary>
        /// Returns the maximum rage of the unit, which is calculated by dividing the MaxPower1 value
        /// obtained from the unit descriptor by 10.
        /// </summary>
        public int MaxRage => GetUnitDescriptor().MaxPower1 / 10;

        /// <summary>
        /// Gets the maximum runic power.
        /// </summary>
        public int MaxRunicPower => 0;

        /// <summary>
        /// Returns the maximum amount of secondary resource based on the class switch.
        /// </summary>
        public int MaxSecondary => Class switch
        {
            WowClass.Warrior => MaxRage,
            WowClass.Rogue => MaxEnergy,
            WowClass.Deathknight => MaxRunicPower,
            _ => MaxMana,
        };

        /// <summary>
        /// Gets the BitVector32 representing the NPC flags from the unit descriptor.
        /// </summary>
        public BitVector32 NpcFlags => GetUnitDescriptor().NpcFlags1;

        /// <summary>
        /// Returns the position of the WowUnit using the Memory API. If the read operation
        /// is successful, it returns the position value obtained from the Memory API. Otherwise,
        /// it returns a Vector3.Zero value.
        /// </summary>
        public new Vector3 Position => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitPosition), out Vector3 position) ? position : Vector3.Zero;

        /// <summary>
        /// Gets the power type of the unit.
        /// </summary>
        public WowPowerType PowerType => (WowPowerType)GetUnitDescriptor().PowerType;

        /// <summary>
        /// Returns the race of the WowRace object based on the race obtained from the unit descriptor.
        /// </summary>
        public WowRace Race => (WowRace)GetUnitDescriptor().Race;

        /// <summary>
        /// Gets the rage value by dividing the power1 value by 10.
        /// </summary>
        public int Rage => GetUnitDescriptor().Power1 / 10;

        /// <summary>
        /// Calculates the rage percentage based on the current rage and maximum rage.
        /// </summary>
        public double RagePercentage => BotMath.Percentage(Rage, MaxRage);

        /// <summary>
        /// Gets the rotation of the WowUnit based on the memory read from the specified address.
        /// If the memory read is successful, returns the rotation value, otherwise returns 0.0f.
        /// </summary>
        public float Rotation => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitPosition + 0x10), out float rotation) ? rotation : 0.0f;

        /// <summary>
        /// Gets the amount of Runic Power.
        /// </summary>
        public int RunicPower => 0;

        /// <summary>
        /// Calculates the percentage of Runic Power relative to the maximum Runic Power.
        /// </summary>
        public double RunicPowerPercentage => BotMath.Percentage(RunicPower, MaxRunicPower);

        /// <summary>
        /// Returns the secondary resource value based on the class.
        /// </summary>
        /// <returns>The secondary resource value.</returns>
        public int Secondary => Class switch
        {
            WowClass.Warrior => Rage,
            WowClass.Rogue => Energy,
            WowClass.Deathknight => RunicPower,
            _ => Mana,
        };

        ///<summary>
        ///Returns the secondary resource percentage based on the class.
        ///</summary>
        public double SecondaryPercentage => Class switch
        {
            WowClass.Warrior => RagePercentage,
            WowClass.Rogue => EnergyPercentage,
            WowClass.Deathknight => RunicPowerPercentage,
            _ => ManaPercentage,
        };

        /// <summary>
        /// Gets the GUID of the unit that summoned this unit.
        /// </summary>
        public ulong SummonedByGuid => GetUnitDescriptor().SummonedBy;

        /// <summary>
        /// Gets the target GUID of a unit.
        /// </summary>
        public ulong TargetGuid => GetUnitDescriptor().Target;

        /// <summary>
        /// Gets the unit flags from the unit descriptor.
        /// </summary>
        public BitVector32 UnitFlags => GetUnitDescriptor().Flags1;

        /// <summary>
        /// Gets the UnitFlags2 value from the UnitDescriptor.
        /// </summary>
        public BitVector32 UnitFlags2 => GetUnitDescriptor().Flags2;

        /// <summary>
        /// Retrieves the unit auras from the specified memory and unit base address.
        /// </summary>
        /// <param name="memory">An instance of the WowMemoryApi class used for memory operations.</param>
        /// <param name="unitBase">The base address of the unit in memory.</param>
        /// <param name="auraCount">An integer variable to store the total count of auras.</param>
        /// <returns>An IEnumerable collection of IWowAura objects representing the unit auras.</returns>
        public static IEnumerable<IWowAura> GetUnitAuras(WowMemoryApi memory, IntPtr unitBase, out int auraCount)
        {
            if (memory.Read(IntPtr.Add(unitBase, (int)memory.Offsets.AuraCount1), out int auraCount1))
            {
                if (auraCount1 == -1)
                {
                    if (memory.Read(IntPtr.Add(unitBase, (int)memory.Offsets.AuraCount2), out int auraCount2)
                        && auraCount2 > 0
                        && memory.Read(IntPtr.Add(unitBase, (int)memory.Offsets.AuraTable2), out IntPtr auraTable))
                    {
                        IEnumerable<IWowAura> auras = ReadAuraTable(memory, auraTable, auraCount2);
                        auraCount = auras.Count();
                        return auras;
                    }
                }
                else
                {
                    IEnumerable<IWowAura> auras = ReadAuraTable(memory, IntPtr.Add(unitBase, (int)memory.Offsets.AuraTable1), auraCount1);
                    auraCount = auras.Count();
                    return auras;
                }
            }

            auraCount = 0;
            return Array.Empty<IWowAura>();
        }

        /// <summary>
        /// Calculates the aggro range to another World of Warcraft unit.
        /// The aggro range is determined by adding the other unit's level to the current unit's level.
        /// The resulting range is restricted to a minimum of 5.0f and a maximum of 45.0f.
        /// </summary>
        /// <param name="other">The unit to calculate the aggro range to.</param>
        /// <returns>The calculated aggro range.</returns>
        public float AggroRangeTo(IWowUnit other)
        {
            float range = 20.0f + (other.Level - Level);
            return MathF.Max(5.0f, MathF.Min(45.0f, range));
        }

        /// <summary>
        /// Checks if there is a buff with the given spell id in the Auras collection.
        /// </summary>
        /// <param name="spellId">The id of the spell to check for.</param>
        /// <returns>True if a buff with the specified spell id exists, false otherwise.</returns>
        public bool HasBuffById(int spellId)
        {
            return Auras != null && Auras.Any(e => e.SpellId == spellId);
        }

        /// <summary>
        /// Checks if the provided <paramref name="wowUnit"/> is within melee range.
        /// </summary>
        /// <param name="wowUnit">The <see cref="IWowUnit"/> instance to be checked.</param>
        /// <returns>True if the <paramref name="wowUnit"/> is within melee range, otherwise false.</returns>
        public bool IsInMeleeRange(IWowUnit wowUnit)
        {
            // TODO: figure out real way to use combat reach
            return wowUnit != null && Position.GetDistance(wowUnit.Position) < MeleeRangeTo(wowUnit);
        }

        /// <summary>
        /// Calculates the melee range to the specified WoW unit by adding the combat reach of the unit and the combat reach of the current object,
        /// and then multiplying the result by 0.95.
        /// If the specified WoW unit is null, returns 0.0.
        /// </summary>
        public float MeleeRangeTo(IWowUnit wowUnit)
        {
            return wowUnit != null ? (wowUnit.CombatReach + CombatReach) * 0.95f : 0.0f;
        }

        /// <summary>
        /// Reads the name of the WoW unit from the database entry stored in memory.
        /// </summary>
        /// <returns>The name of the WoW unit if it can be successfully read from memory, otherwise returns "unknown".</returns>
        public virtual string ReadName()
        {
            return GetDbEntry(Memory.Offsets.WowUnitDbEntryName, out IntPtr namePtr)
                && Memory.ReadString(namePtr, Encoding.UTF8, out string name) ? name : "unknown";
        }

        /// <summary>
        /// Reads the type of the Wow creature.
        /// </summary>
        /// <returns>The type of the Wow creature. Returns WowCreatureType.Unknown if the type cannot be found.</returns>
        public virtual WowCreatureType ReadType()
        {
            return GetDbEntry(Memory.Offsets.WowUnitDbEntryType, out WowCreatureType type) ? type : WowCreatureType.Unknown;
        }

        /// <summary>
        /// Overrides the ToString method to provide a string representation of the Unit's details, including its Guid, Level, Position, and DisplayId.
        /// </summary>
        public override string ToString()
        {
            return $"Unit: {Guid} lvl. {Level} Position: {Position} DisplayId: {DisplayId}";
        }

        /// <summary>
        /// Overrides the Update method to include a call to the base class Update method.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Retrieves the movement flags from memory, if available. Returns 0 if not found.
        /// </summary>
        protected uint GetMovementFlags()
        {
            return MovementFlags ??= Memory.Read(IntPtr.Add(BaseAddress, 0xec), out IntPtr movementFlagsPtr)
                && Memory.Read(IntPtr.Add(movementFlagsPtr, 0x38), out uint movementFlags) ? movementFlags : 0;
        }

        /// <summary>
        /// Retrieves the WowUnitDescriptor548 for the current unit.
        /// If the UnitDescriptor is null, it reads the memory at DescriptorAddress + sizeof(WowObjectDescriptor548) 
        /// to obtain the WowUnitDescriptor548 object. If successful, it assigns UnitDescriptor to the obtained object.
        /// If the memory read fails, it initializes a new WowUnitDescriptor548 object and assigns it to UnitDescriptor.
        /// </summary>
        protected WowUnitDescriptor548 GetUnitDescriptor()
        {
            return UnitDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowUnitDescriptor548 objPtr) ? objPtr : new();
        }

        /// <summary>
        /// Reads a table of WoW auras from memory and returns an IEnumerable of IWowAura objects.
        /// The method uses the provided IMemoryApi to read memory at the specified buffBase address with a count of auraCount.
        /// It creates a new List to store the auras.
        /// It iterates through the table and for each iteration, it reads memory at the appropriate address to retrieve a WowAura548 object.
        /// If the memory read is successful and the SpellId of the retrieved WowAura548 object is greater than 0, it adds the object to the List of auras.
        /// Finally, it returns the list of auras.
        /// </summary>
        private static unsafe IEnumerable<IWowAura> ReadAuraTable(IMemoryApi memory, IntPtr buffBase, int auraCount)
        {
            List<IWowAura> auras = new();

            for (int i = 0; i < auraCount; ++i)
            {
                if (memory.Read(buffBase + (sizeof(WowAura548) * i), out WowAura548 rawWowAura) && rawWowAura.SpellId > 0)
                {
                    auras.Add(rawWowAura);
                }
            }

            return auras;
        }

        /// <summary>
        /// Retrieves a database entry of type T at the specified entry offset.
        /// </summary>
        /// <typeparam name="T">The type of the database entry to retrieve.</typeparam>
        /// <param name="entryOffset">The offset of the entry to retrieve.</param>
        /// <param name="ptr">The output parameter that will hold the retrieved entry.</param>
        /// <returns>True if the retrieval is successful, otherwise false.</returns>
        private bool GetDbEntry<T>(IntPtr entryOffset, out T ptr) where T : unmanaged
        {
            ptr = default;
            return Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitDbEntry), out IntPtr dbEntry)
                && dbEntry != IntPtr.Zero
                && Memory.Read(IntPtr.Add(dbEntry, (int)entryOffset), out ptr);
        }

        /// <summary>
        /// Reads the aura table and returns an enumerable collection of <see cref="IWowAura"/> objects.
        /// If the aura table is null, it is initialized by calling the <see cref="GetUnitAuras"/> method and assigning the result to the aura table.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="IWowAura"/> objects.</returns>
        private IEnumerable<IWowAura> ReadAuraTable()
        {
            return AuraTable ??= GetUnitAuras(Memory, BaseAddress, out _);
        }
    }
}