using AmeisenBotX.Common.Math;
using AmeisenBotX.Memory;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Flags;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using AmeisenBotX.Wow335a.Objects.Flags;
using AmeisenBotX.Wow335a.Objects.Raw;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AmeisenBotX.Wow335a.Objects
{
    [Serializable]
    public class WowUnit335a : WowObject335a, IWowUnit
    {
        /// <summary>
        /// Gets or sets the number of auras.
        /// </summary>
        public int AuraCount { get; set; }

        /// <summary>
        /// Gets or sets a collection of objects that implement the <see cref="IWowAura"/> interface.
        /// </summary>
        public IEnumerable<IWowAura> Auras { get; set; }

        /// <summary>
        /// Gets the WowClass of the RawWowUnit.
        /// </summary>
        public WowClass Class => (WowClass)RawWowUnit.Class;

        /// <summary>
        /// Gets the combat reach of the unit.
        /// </summary>
        public float CombatReach => RawWowUnit.CombatReach;

        /// <summary>
        /// Gets or sets the ID of the currently casting spell.
        /// </summary>
        public int CurrentlyCastingSpellId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the spell that is currently being channeled.
        /// </summary>
        public int CurrentlyChannelingSpellId { get; set; }

        /// <summary>
        /// Gets the display ID of the wow unit.
        /// </summary>
        public int DisplayId => RawWowUnit.DisplayId;

        /// <summary>
        /// Gets the energy of the RawWowUnit.
        /// </summary>
        public int Energy => RawWowUnit.Power4;

        /// <summary>
        /// Calculates the percentage of energy out of the maximum energy.
        /// </summary>
        public double EnergyPercentage => BotMath.Percentage(Energy, MaxEnergy);

        /// <summary>
        /// Gets the faction template of the WowUnit.
        /// </summary>
        public int FactionTemplate => RawWowUnit.FactionTemplate;

        /// <summary>
        /// Gets the gender of the WowUnit.
        /// </summary>
        public WowGender Gender => (WowGender)RawWowUnit.Gender;

        /// <summary>
        /// Gets or sets the health of the RawWowUnit.
        /// </summary>
        public int Health => RawWowUnit.Health;

        /// <summary>
        /// Calculates the health percentage of the bot.
        /// </summary>
        public double HealthPercentage => BotMath.Percentage(Health, MaxHealth);

        /// <summary>
        /// Gets or sets the value of Holy Power.
        /// </summary>
        public int HolyPower => 0;

        /// <summary>
        /// Gets or sets a value indicating whether the entity is in auto-attacking mode.
        /// </summary>
        public bool IsAutoAttacking { get; set; }

        /// <summary>
        /// Determines whether the unit is dead by checking if its health is 0 or the dynamic flags representing death are enabled,
        /// and also verifying that the unit does not have the "Feign Death" flag enabled in the secondary flags.
        /// </summary>
        public bool IsDead => (Health == 0 || UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.Dead]) && !UnitFlags2[(int)WowUnit2Flag.FeignDeath];

        /// <summary>
        /// Determines if the unit is lootable based on the value of the dynamic unit flags.
        /// </summary>
        public bool IsLootable => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.Lootable];

        /// <summary>
        /// Returns a boolean value indicating if the refer-a-friend flag is linked for the unit.
        /// </summary>
        public bool IsReferAFriendLinked => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.ReferAFriendLinked];

        /// <summary>
        /// Returns a boolean value indicating whether the unit has special information.
        /// </summary>
        public bool IsSpecialInfo => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.SpecialInfo];

        /// <summary>
        /// Returns a boolean value indicating whether the unit is tagged by me.
        /// </summary>
        public bool IsTaggedByMe => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.TaggedByMe];

        /// <summary>
        /// Gets a value indicating whether the unit is tagged by another entity.
        /// </summary>
        public bool IsTaggedByOther => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.TaggedByOther];

        /// <summary>
        /// Returns whether the unit is tapped by all threat list.
        /// </summary>
        public bool IsTappedByAllThreatList => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.IsTappedByAllThreatList];

        /// <summary>
        /// Determines if the unit is being tracked based on the dynamic unit flags.
        /// </summary>
        public bool IsTrackedUnit => UnitFlagsDynamic[(int)WowUnitDynamicFlags335a.TrackUnit];

        /// <summary>
        /// Gets the level of the RawWowUnit.
        /// </summary>
        public int Level => RawWowUnit.Level;

        /// <summary>
        /// Gets the mana of the RawWowUnit.
        /// </summary>
        public int Mana => RawWowUnit.Power1;

        /// <summary>
        /// Calculates the percentage of mana remaining.
        /// </summary>
        public double ManaPercentage => BotMath.Percentage(Mana, MaxMana);

        /// <summary>
        /// Gets the maximum energy of the WowUnit.
        /// </summary>
        public int MaxEnergy => RawWowUnit.MaxPower4;

        /// <summary>
        /// Gets the maximum health of the Raw Wow Unit.
        /// </summary>
        public int MaxHealth => RawWowUnit.MaxHealth;

        /// <summary>
        /// Gets the maximum holy power.
        /// </summary>
        public int MaxHolyPower => 0;

        /// <summary>
        /// Gets the maximum mana of the WoW unit.
        /// </summary>
        public int MaxMana => RawWowUnit.MaxPower1;

        /// <summary>
        /// Gets the maximum rage by dividing the maximum power of a RawWowUnit by 10.
        /// </summary>
        public int MaxRage => RawWowUnit.MaxPower2 / 10;

        /// <summary>
        /// Retrieves the maximum runic power by dividing the raw wow unit's maximum power by 10.
        /// </summary>
        public int MaxRunicPower => RawWowUnit.MaxPower7 / 10;

        /// <summary>
        /// Gets the maximum secondary resource based on the specified class.
        /// </summary>
        /// <returns>
        /// The maximum secondary resource.
        /// </returns>
        public int MaxSecondary => Class switch
        {
            WowClass.Warrior => MaxRage,
            WowClass.Rogue => MaxEnergy,
            WowClass.Deathknight => MaxRunicPower,
            _ => MaxMana,
        };

        /// <summary>
        /// Gets the BitVector32 containing the NPC flags of the RawWowUnit.
        /// </summary>
        public BitVector32 NpcFlags => RawWowUnit.NpcFlags;

        /// <summary>
        /// Gets the power type of the unit.
        /// </summary>
        public WowPowerType PowerType => (WowPowerType)RawWowUnit.PowerType;

        /// <summary>
        /// Gets the race of the WowUnit.
        /// </summary>
        public WowRace Race => (WowRace)RawWowUnit.Race;

        /// <summary>
        /// Gets the rage value by dividing the power2 of the RawWowUnit by 10.
        /// </summary>
        public int Rage => RawWowUnit.Power2 / 10;

        /// <summary>
        /// Calculates the rage percentage by calling the method Percentage from the BotMath class,
        /// using the Rage parameter and MaxRage as input.
        /// </summary>
        public double RagePercentage => BotMath.Percentage(Rage, MaxRage);

        /// <summary>
        /// Gets or sets the rotation value.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets the Runic Power by dividing the RawWowUnit's power by 10.
        /// </summary>
        public int RunicPower => RawWowUnit.Power7 / 10;

        /// <summary>
        /// Calculates and returns the percentage of Runic Power out of the maximum Runic Power.
        /// </summary>
        public double RunicPowerPercentage => BotMath.Percentage(RunicPower, MaxRunicPower);

        /// <summary>
        /// Gets the secondary resource value based on the class.
        /// </summary>
        /// <returns>The secondary resource value.</returns>
        public int Secondary => Class switch
        {
            WowClass.Warrior => Rage,
            WowClass.Rogue => Energy,
            WowClass.Deathknight => RunicPower,
            _ => Mana,
        };

        /// <summary>
        /// Returns the secondary resource percentage based on the current class.
        /// </summary>
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
        public ulong SummonedByGuid => RawWowUnit.SummonedBy;

        /// <summary>
        /// Gets the target Guid of the RawWowUnit.
        /// </summary>
        public ulong TargetGuid => RawWowUnit.Target;

        /// <summary>
        /// Gets or sets the BitVector32 that represents the unit flags.
        /// </summary>
        public BitVector32 UnitFlags => RawWowUnit.Flags1;

        /// <summary>
        /// Gets or sets the BitVector32 that represents the unit flags 2 of the World of Warcraft unit.
        /// </summary>
        public BitVector32 UnitFlags2 => RawWowUnit.Flags2;

        /// <summary>
        /// Gets or sets the dynamic flags of the unit as represented by a BitVector32.
        /// </summary>
        public BitVector32 UnitFlagsDynamic => RawWowUnit.DynamicFlags;

        /// <summary>
        /// Gets or sets the raw WowUnit descriptor.
        /// </summary>
        protected WowUnitDescriptor335a RawWowUnit { get; private set; }

        /// <summary>
        /// Retrieves the list of auras associated with a unit given its base address in memory.
        /// </summary>
        /// <param name="memory">The WowMemoryApi instance used to read data from memory.</param>
        /// <param name="unitBase">The base address of the unit in memory.</param>
        /// <param name="auraCount">The number of auras associated with the unit.</param>
        /// <returns>An IEnumerable of IWowAura representing the unit's auras.</returns>
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
                        auraCount = auraCount2;
                        return ReadAuraTable(memory, auraTable, auraCount2);
                    }
                    else
                    {
                        auraCount = 0;
                    }
                }
                else
                {
                    auraCount = auraCount1;
                    return ReadAuraTable(memory, IntPtr.Add(unitBase, (int)memory.Offsets.AuraTable1), auraCount1);
                }
            }
            else
            {
                auraCount = 0;
            }

            return Array.Empty<IWowAura>();
        }

        /// <summary>
        /// Calculates the aggression range to another WoW unit.
        /// </summary>
        /// <param name="other">The other WoW unit to calculate the aggression range to.</param>
        /// <returns>The aggression range to the other WoW unit.</returns>
        public float AggroRangeTo(IWowUnit other)
        {
            float range = 20.0f + (other.Level - Level);
            return MathF.Max(5.0f, MathF.Min(45.0f, range));
        }

        /// <summary>
        /// Checks if there is a buff with the given spell ID.
        /// </summary>
        /// <param name="spellId">The ID of the spell to check for.</param>
        /// <returns>True if a buff with the specified spell ID exists, false otherwise.</returns>
        public bool HasBuffById(int spellId)
        {
            return Auras != null && Auras.Any(e => e.SpellId == spellId);
        }

        /// <summary>
        /// Checks if a given WoW unit is within melee range.
        /// </summary>
        /// <param name="wowUnit">The WoW unit to check.</param>
        /// <returns>True if the WoW unit is within melee range, otherwise false.</returns>
        public bool IsInMeleeRange(IWowUnit wowUnit)
        {
            // TODO: figure out real way to use combat reach
            return wowUnit != null && Position.GetDistance(wowUnit.Position) < MeleeRangeTo(wowUnit);
        }

        /// <summary>
        /// Calculates the melee range to the specified WoW unit.
        /// </summary>
        /// <param name="wowUnit">The WoW unit to calculate the melee range to.</param>
        /// <returns>The calculated melee range.</returns>
        public float MeleeRangeTo(IWowUnit wowUnit)
        {
            return wowUnit != null ? (wowUnit.CombatReach + CombatReach) * 0.95f : 0.0f;
        }

        /// <summary>
        /// Reads the name of a WowUnit from the database entry. If successful, returns the name string. Otherwise, returns "unknown".
        /// </summary>
        public virtual string ReadName()
        {
            return GetDbEntry(Memory.Offsets.WowUnitDbEntryName, out IntPtr namePtr)
                && Memory.ReadString(namePtr, Encoding.UTF8, out string name) ? name : "unknown";
        }

        /// <summary>
        /// Reads the type of the WoW creature.
        /// </summary>
        /// <returns>
        /// The WoW creature type. If the type cannot be retrieved from the database entry, returns WowCreatureType.Unknown.
        /// </returns>
        public virtual WowCreatureType ReadType()
        {
            return GetDbEntry(Memory.Offsets.WowUnitDbEntryType, out WowCreatureType type) ? type : WowCreatureType.Unknown;
        }

        /// <summary>
        /// Converts the object to a string representation.
        /// </summary>
        public override string ToString()
        {
            return $"Unit: {Guid} lvl. {Level} Position: {Position} DisplayId: {DisplayId}";
        }

        ///<summary>
        /// Updates the object's properties based on the current state in memory.
        ///</summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowUnitDescriptor335a objPtr))
            {
                RawWowUnit = objPtr;
            }

            Auras = GetUnitAuras(Memory, BaseAddress, out int auraCount);
            AuraCount = auraCount;

            if (Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitPosition), out Vector3 position))
            {
                Position = position;
            }

            if (Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitPosition + 0x10), out float rotation))
            {
                Rotation = rotation;
            }

            if (Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitIsAutoAttacking), out int isAutoAttacking))
            {
                IsAutoAttacking = isAutoAttacking == 1;
            }

            if (Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.CurrentlyCastingSpellId), out int castingId))
            {
                CurrentlyCastingSpellId = castingId;
            }

            if (Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.CurrentlyChannelingSpellId), out int channelingId))
            {
                CurrentlyChannelingSpellId = channelingId;
            }
        }

        /// <summary>
        /// Reads the aura table from memory and returns a collection of WoW auras.
        /// </summary>
        /// <param name="memory">An instance of the IMemoryApi interface used for reading memory.</param>
        /// <param name="buffBase">The base address of the aura table in memory.</param>
        /// <param name="auraCount">The number of auras to read from the table.</param>
        /// <returns>A collection of WoW auras.</returns>
        private static unsafe IEnumerable<IWowAura> ReadAuraTable(IMemoryApi memory, IntPtr buffBase, int auraCount)
        {
            List<IWowAura> auras = new();

            if (auraCount > 40)
            {
                return auras;
            }

            for (int i = 0; i < auraCount; ++i)
            {
                if (memory.Read(buffBase + (sizeof(WowAura335a) * i), out WowAura335a rawWowAura) && rawWowAura.SpellId > 0)
                {
                    auras.Add(rawWowAura);
                }
            }

            return auras;
        }

        /// <summary>
        /// Retrieves a database entry of type T at the specified offset in memory.
        /// </summary>
        /// <typeparam name="T">The type of the database entry.</typeparam>
        /// <param name="entryOffset">The offset of the entry in memory.</param>
        /// <param name="ptr">The output parameter containing the retrieved entry.</param>
        /// <returns>True if the database entry is successfully retrieved, false otherwise.</returns>
        private bool GetDbEntry<T>(IntPtr entryOffset, out T ptr) where T : unmanaged
        {
            ptr = default;
            return Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowUnitDbEntry), out IntPtr dbEntry)
                && dbEntry != IntPtr.Zero
                && Memory.Read(IntPtr.Add(dbEntry, (int)entryOffset), out ptr);
        }
    }
}