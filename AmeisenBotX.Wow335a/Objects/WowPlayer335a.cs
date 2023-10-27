using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmeisenBotX.Wow335a.Objects
{
    /// <summary>
    /// Represents a player in the World of Warcraft 3.3.5a game version.
    /// </summary>
    [Serializable]
    public class WowPlayer335a : WowUnit335a, IWowPlayer
    {
        /// <summary>
        /// Represents an array of visible item enchantments.
        /// </summary>
        private VisibleItemEnchantment[] itemEnchantments;

        /// <summary>
        /// Private field representing an array of QuestlogEntry objects.
        /// </summary>
        private QuestlogEntry[] questlogEntries;

        /// <summary>
        /// Gets or sets the number of combo points.
        /// </summary>
        public int ComboPoints { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently flying.
        /// </summary>
        public bool IsFlying { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is a ghost.
        /// </summary>
        public bool IsGhost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a property that determines if the item is outdoors.
        /// </summary>
        public bool IsOutdoors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently swimming.
        /// </summary>
        public bool IsSwimming { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is located underwater.
        /// </summary>
        public bool IsUnderwater { get; set; }

        /// <summary>
        /// Gets the collection of visible item enchantments.
        /// </summary>
        /// <returns>
        /// An IEnumerable of VisibleItemEnchantment objects representing the item enchantments.
        /// </returns>
        public IEnumerable<VisibleItemEnchantment> ItemEnchantments => itemEnchantments;

        /// <summary>
        /// Gets the experience points required to reach the next level.
        /// </summary>
        public int NextLevelXp => RawWowPlayer.NextLevelXp;

        /// <summary>
        /// Gets the collection of quest log entries.
        /// </summary>
        /// <returns>An IEnumerable of QuestlogEntry objects.</returns>
        public IEnumerable<QuestlogEntry> QuestlogEntries => questlogEntries;

        /// <summary>
        /// Gets the experience points (XP) of the player.
        /// </summary>
        public int Xp => RawWowPlayer.Xp;

        /// <summary>
        /// Calculates the XP percentage based on the current XP and the required XP for the next level.
        /// </summary>
        public double XpPercentage => BotMath.Percentage(Xp, NextLevelXp);

        /// <summary>
        /// Gets or sets the raw WowPlayerDescriptor335a.
        /// </summary>
        protected WowPlayerDescriptor335a RawWowPlayer { get; private set; }

        /// <summary>
        /// Determines if the race is part of the Alliance faction in World of Warcraft.
        /// </summary>
        public bool IsAlliance()
        {
            return Race is WowRace.Draenei
                or WowRace.Human
                or WowRace.Dwarf
                or WowRace.Gnome
                or WowRace.Nightelf;
        }

        /// <summary>
        /// Determines whether the race is classified as a Horde race.
        /// </summary>
        public bool IsHorde()
        {
            return Race is WowRace.Undead
                or WowRace.Orc
                or WowRace.Bloodelf
                or WowRace.Tauren
                or WowRace.Troll;
        }

        /// <summary>
        /// Reads and returns the name from the memory. If the name is not found or cannot be read, it returns an empty string.
        /// </summary>
        public override string ReadName()
        {
            if (Memory.Read(IntPtr.Add(Memory.Offsets.NameStore, (int)Memory.Offsets.NameMask), out uint nameMask)
                && Memory.Read(IntPtr.Add(Memory.Offsets.NameStore, (int)Memory.Offsets.NameBase), out uint nameBase))
            {
                uint shortGuid = (uint)Guid & 0xfffffff;
                uint offset = 12 * (nameMask & shortGuid);

                if (Memory.Read(new(nameBase + offset + 8), out uint current)
                    && Memory.Read(new(nameBase + offset), out offset))
                {
                    if ((current & 0x1) == 0x1)
                    {
                        return string.Empty;
                    }

                    Memory.Read(new(current), out uint testGuid);

                    while (testGuid != shortGuid)
                    {
                        Memory.Read(new(current + offset + 4), out current);

                        if ((current & 0x1) == 0x1)
                        {
                            return string.Empty;
                        }

                        Memory.Read(new(current), out testGuid);
                    }

                    if (Memory.ReadString(new(current + (int)Memory.Offsets.NameString), Encoding.UTF8, out string name, 16))
                    {
                        return name;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Overrides the ToString method to return a string representation of the player's Guid and Level.
        /// </summary>
        public override string ToString()
        {
            return $"Player: {Guid} lvl. {Level}";
        }

        /// <summary>
        /// Overrides the Update method and updates various properties and fields of the WowPlayer object.
        /// This includes updating the questlog entries and visible item enchantments based on the values obtained from memory.
        /// Additionally, it checks various memory addresses to determine if the player is swimming, flying, underwater, or in ghost form.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset + WowUnitDescriptor335a.EndOffset, out WowPlayerDescriptor335a obj))
            {
                RawWowPlayer = obj;

                questlogEntries = new QuestlogEntry[]
                {
                    obj.QuestlogEntry1,
                    obj.QuestlogEntry2,
                    obj.QuestlogEntry3,
                    obj.QuestlogEntry4,
                    obj.QuestlogEntry5,
                    obj.QuestlogEntry6,
                    obj.QuestlogEntry7,
                    obj.QuestlogEntry8,
                    obj.QuestlogEntry9,
                    obj.QuestlogEntry10,
                    obj.QuestlogEntry11,
                    obj.QuestlogEntry12,
                    obj.QuestlogEntry13,
                    obj.QuestlogEntry14,
                    obj.QuestlogEntry15,
                    obj.QuestlogEntry16,
                    obj.QuestlogEntry17,
                    obj.QuestlogEntry18,
                    obj.QuestlogEntry19,
                    obj.QuestlogEntry20,
                    obj.QuestlogEntry21,
                    obj.QuestlogEntry22,
                    obj.QuestlogEntry23,
                    obj.QuestlogEntry24,
                    obj.QuestlogEntry25,
                };

                itemEnchantments = new VisibleItemEnchantment[]
                {
                    obj.VisibleItemEnchantment1,
                    obj.VisibleItemEnchantment2,
                    obj.VisibleItemEnchantment3,
                    obj.VisibleItemEnchantment4,
                    obj.VisibleItemEnchantment5,
                    obj.VisibleItemEnchantment6,
                    obj.VisibleItemEnchantment7,
                    obj.VisibleItemEnchantment8,
                    obj.VisibleItemEnchantment9,
                    obj.VisibleItemEnchantment10,
                    obj.VisibleItemEnchantment11,
                    obj.VisibleItemEnchantment12,
                    obj.VisibleItemEnchantment13,
                    obj.VisibleItemEnchantment14,
                    obj.VisibleItemEnchantment15,
                    obj.VisibleItemEnchantment16,
                    obj.VisibleItemEnchantment17,
                    obj.VisibleItemEnchantment18,
                    obj.VisibleItemEnchantment19,
                };
            }

            if (Memory.Read(IntPtr.Add(BaseAddress, 0xA30), out uint swimFlags))
            {
                IsSwimming = (swimFlags & 0x200000) != 0;
            }

            if (Memory.Read(IntPtr.Add(BaseAddress, 0xD8), out IntPtr flyFlagsPointer)
                && Memory.Read(IntPtr.Add(flyFlagsPointer, 0x44), out uint flyFlags))
            {
                IsFlying = (flyFlags & 0x2000000) != 0;
            }

            if (Memory.Read(Memory.Offsets.BreathTimer, out int breathTimer))
            {
                IsUnderwater = breathTimer > 0;
            }

            if (Memory.Read(Memory.Offsets.ComboPoints, out byte comboPoints))
            {
                ComboPoints = comboPoints;
            }

            IsGhost = HasBuffById(8326);
        }
    }
}