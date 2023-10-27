using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// Initializes a new instance of the ObjectManager548 class.
    /// </summary>
    /// <param name="memory">The WowMemoryApi object to use for memory operations.</param>
    public class ObjectManager548 : ObjectManager<WowObject548, WowUnit548, WowPlayer548, WowGameobject548, WowDynobject548, WowItem548, WowCorpse548, WowContainer548>
    {
        /// <summary>
        /// Initializes a new instance of the ObjectManager548 class.
        /// </summary>
        /// <param name="memory">The WowMemoryApi object to use for memory operations.</param>
        public ObjectManager548(WowMemoryApi memory)
                    : base(memory)
        {
        }

        /// <summary>
        /// Reads party information and updates party-related properties.
        /// </summary>
        protected override void ReadParty()
        {
            if (ReadPartyPointer(out IntPtr party)
                && Memory.Read(IntPtr.Add(party, 0xC4), out int count) && count > 0)
            {
                PartymemberGuids = ReadPartymemberGuids(party);
                Partymembers = wowObjects.OfType<IWowUnit>().Where(e => PartymemberGuids.Contains(e.Guid));

                Vector3 pos = new();

                foreach (Vector3 vec in Partymembers.Select(e => e.Position))
                {
                    pos += vec;
                }

                CenterPartyPosition = pos / Partymembers.Count();

                PartyPetGuids = PartyPets.Select(e => e.Guid);
                PartyPets = wowObjects.OfType<IWowUnit>().Where(e => PartymemberGuids.Contains(e.SummonedByGuid));
            }
        }

        /// <summary>
        /// Reads the GUIDs of party members.
        /// </summary>
        /// <param name="party">A pointer to the party.</param>
        /// <returns>An enumerable collection of party member GUIDs.</returns>
        private IEnumerable<ulong> ReadPartymemberGuids(IntPtr party)
        {
            List<ulong> partymemberGuids = new();

            for (int i = 0; i < 40; i++)
            {
                if (Memory.Read(IntPtr.Add(party, i * 4), out IntPtr player) && player != IntPtr.Zero
                    && Memory.Read(IntPtr.Add(player, 0x10), out ulong guid) && guid > 0)
                {
                    partymemberGuids.Add(guid);

                    if (Memory.Read(IntPtr.Add(player, 0x4), out int status) && status == 2)
                    {
                        PartyleaderGuid = guid;
                    }
                }
            }

            return partymemberGuids.Where(e => e != 0 && e != PlayerGuid).Distinct();
        }

        /// <summary>
        /// Reads the party pointer from memory and assigns it to the out parameter party.
        /// Returns true if the party pointer is successfully read and is not null, false otherwise.
        /// </summary>
        private bool ReadPartyPointer(out IntPtr party)
        {
            return Memory.Read(Memory.Offsets.PartyLeader, out party) && party != IntPtr.Zero;
        }
    }
}