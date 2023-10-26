using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Raw
{
    ///<summary>
    ///Represents a structure containing the party member guids.
    ///</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawPartyGuids
    {
        /// <summary>
        /// Gets or sets the unique identifier for the first party member.
        /// </summary>
        public ulong PartymemberGuid1 { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the party member.
        /// </summary>
        public ulong PartymemberGuid2 { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the third party member.
        /// </summary>
        public ulong PartymemberGuid3 { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the fourth party member.
        /// </summary>
        public ulong PartymemberGuid4 { get; set; }

        /// <summary>
        /// Converts the PartymemberGuid fields to an array of ulong values.
        /// </summary>
        /// <returns>An array of ulong values containing the PartymemberGuid fields.</returns>
        public ulong[] AsArray()
        {
            return new ulong[]
            {
                PartymemberGuid1,
                PartymemberGuid2,
                PartymemberGuid3,
                PartymemberGuid4,
            };
        }
    }
}