using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using AmeisenBotX.Wow548.Objects.Descriptors;
using System.Text;

namespace AmeisenBotX.Wow548.Objects
{
    [Serializable]
    public unsafe class WowPlayer548 : WowUnit548, IWowPlayer
    {
        /// <summary>
        /// Gets or sets the player descriptor for the WowPlayer.
        /// </summary>
        protected WowPlayerDescriptor548? PlayerDescriptor;

        /// <summary>
        /// Private field representing a collection of visible item enchantments.
        /// </summary>
        private IEnumerable<VisibleItemEnchantment> itemEnchantments;

        /// <summary>
        /// Private field representing a collection of questlog entries.
        /// </summary>
        private IEnumerable<QuestlogEntry> questlogEntries;

        /// <summary>
        /// Gets the value of the combo points by reading it from the memory. Returns the combo points value if successfully read, otherwise returns 0.
        /// </summary>
        public int ComboPoints => Memory.Read(Memory.Offsets.ComboPoints, out byte comboPoints) ? comboPoints : 0;

        /// <summary>
        /// Checks if the entity is a ghost by checking if it has the buff with ID 8326.
        /// </summary>
        public bool IsGhost => HasBuffById(8326);

        /// <summary>
        /// Gets or sets a value that indicates whether the current object is outdoors or not.
        /// </summary>
        public bool IsOutdoors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is underwater.
        /// </summary>
        public bool IsUnderwater { get; set; }

        /// <summary>
        /// Gets the collection of visible item enchantments.
        /// </summary>
        /// <returns>
        /// An enumerable collection of VisibleItemEnchantment objects.
        /// </returns>
        public IEnumerable<VisibleItemEnchantment> ItemEnchantments => itemEnchantments;

        /// <summary>
        /// Gets the next level experience points for the player.
        /// </summary>
        public int NextLevelXp => GetPlayerDescriptor().NextLevelXp;

        /// <summary>
        /// Gets the collection of questlog entries.
        /// </summary>
        public IEnumerable<QuestlogEntry> QuestlogEntries => questlogEntries;

        /// <summary>
        /// Gets the experience points (XP) of the player.
        /// </summary>
        public int Xp => GetPlayerDescriptor().Xp;

        /// <summary>
        /// Calculates the percentage of experience points (Xp) relative to the next level experience points (NextLevelXp).
        /// </summary>
        public double XpPercentage => BotMath.Percentage(Xp, NextLevelXp);

        /// <summary>
        /// Determines if the race belongs to the Alliance faction in World of Warcraft.
        /// </summary>
        public bool IsAlliance()
        {
            return Race is WowRace.Draenei
                or WowRace.Human
                or WowRace.Dwarf
                or WowRace.Gnome
                or WowRace.Nightelf
                or WowRace.Worgen
                or WowRace.PandarenA;
        }

        /// <summary>
        /// Determines if the race belongs to the Horde faction.
        /// </summary>
        public bool IsHorde()
        {
            return Race is WowRace.Undead
                or WowRace.Orc
                or WowRace.Bloodelf
                or WowRace.Tauren
                or WowRace.Troll
                or WowRace.Goblin
                or WowRace.PandarenH;
        }

        /// <summary>
        /// Reads the name from the memory store.
        /// </summary>
        /// <returns>The name read from the memory store. If no name is available, returns an empty string.</returns>
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
        /// Converts the current instance of the Player class to its equivalent string representation.
        /// </summary>
        /// <returns>A string that represents the Player object, including the player's unique identifier and level.</returns>
        public override string ToString()
        {
            return $"Player: {Guid} lvl. {Level}";
        }

        /// <summary>
        /// Updates the object's properties and behavior.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Gets the player descriptor by reading the memory at the specified address.
        /// </summary>
        /// <returns>The player descriptor object.</returns>
        protected WowPlayerDescriptor548 GetPlayerDescriptor()
        {
            return PlayerDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548) + sizeof(WowUnitDescriptor548), out WowPlayerDescriptor548 objPtr) ? objPtr : new();
        }
    }
}