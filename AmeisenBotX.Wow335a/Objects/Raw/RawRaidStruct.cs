using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// A struct that represents a collection of IntPtr values for raid players.
/// </summary>
namespace AmeisenBotX.Wow335a.Objects.Raw
{
    /// <summary>
    /// A struct that represents a collection of IntPtr values for raid players.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawRaidStruct
    {
        /// <summary>
        /// Gets or sets the IntPtr handle for RaidPlayer1.
        /// </summary>
        public IntPtr RaidPlayer1 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr for RaidPlayer10.
        /// </summary>
        public IntPtr RaidPlayer10 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr for the RaidPlayer11.
        /// </summary>
        public IntPtr RaidPlayer11 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer12 handle.
        /// </summary>
        public IntPtr RaidPlayer12 { get; set; }

        /// <summary>
        /// Gets or sets the memory address of the RaidPlayer13 object.
        /// </summary>
        public IntPtr RaidPlayer13 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer14 IntPtr.
        /// </summary>
        public IntPtr RaidPlayer14 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer15 attribute as an IntPtr data type.
        /// </summary>
        public IntPtr RaidPlayer15 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer16 property which holds a pointer to an IntPtr value.
        /// </summary>
        public IntPtr RaidPlayer16 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr for RaidPlayer17.
        /// </summary>
        public IntPtr RaidPlayer17 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer18 handle.
        /// </summary>
        public IntPtr RaidPlayer18 { get; set; }

        /// <summary>
        /// Gets or sets the 19th player in the RaidPlayer array as an IntPtr value.
        /// </summary>
        public IntPtr RaidPlayer19 { get; set; }

        /// <summary>
        /// Gets or sets the RAID player 2 pointer.
        /// </summary>
        public IntPtr RaidPlayer2 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer20 pointer.
        /// </summary>
        public IntPtr RaidPlayer20 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer21 value as an IntPtr.
        /// </summary>
        public IntPtr RaidPlayer21 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr value representing the RaidPlayer22.
        /// </summary>
        public IntPtr RaidPlayer22 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr representing the RaidPlayer23 property.
        /// </summary>
        public IntPtr RaidPlayer23 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer24 pointer.
        /// </summary>
        public IntPtr RaidPlayer24 { get; set; }

        /// <summary>
        /// Gets or sets the RAID player 25 pointer.
        /// </summary>
        public IntPtr RaidPlayer25 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer26 property which represents a pointer to a memory location.
        /// </summary>
        public IntPtr RaidPlayer26 { get; set; }

        /// <summary>
        /// Gets or sets the handle to the RaidPlayer27 instance.
        /// </summary>
        public IntPtr RaidPlayer27 { get; set; }

        /// <summary>
        /// Gets or sets the handle to the RaidPlayer28 object.
        /// </summary>
        public IntPtr RaidPlayer28 { get; set; }

        /// <summary>
        /// Gets or sets the player 29 in the raid.
        /// </summary>
        public IntPtr RaidPlayer29 { get; set; }

        /// <summary>
        /// Gets or sets the handle to the RaidPlayer3.
        /// </summary>
        public IntPtr RaidPlayer3 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr value for the RaidPlayer30 property.
        /// </summary>
        public IntPtr RaidPlayer30 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer31 value as a pointer to an integer.
        /// </summary>
        public IntPtr RaidPlayer31 { get; set; }

        /// <summary>
        /// Gets or sets the 32-bit RAID player pointer.
        /// </summary>
        public IntPtr RaidPlayer32 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer33 property, which is an IntPtr type.
        /// </summary>
        public IntPtr RaidPlayer33 { get; set; }

        /// <summary>
        /// Gets or sets the RAidPlayer34 property of type IntPtr.
        /// </summary>
        public IntPtr RaidPlayer34 { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the RaidPlayer35 instance.
        /// </summary>
        public IntPtr RaidPlayer35 { get; set; }

        /// <summary>
        /// Gets or sets the handle to the RaidPlayer36 object.
        /// </summary>
        public IntPtr RaidPlayer36 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer37 IntPtr.
        /// </summary>
        public IntPtr RaidPlayer37 { get; set; }

        /// <summary>
        /// Gets or sets the 38th RAID player's handle.
        /// </summary>
        public IntPtr RaidPlayer38 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr used to identify the RaidPlayer39.
        /// </summary>
        public IntPtr RaidPlayer39 { get; set; }

        /// <summary>
        /// Gets or sets the handle to the RAID player 4.
        /// </summary>
        public IntPtr RaidPlayer4 { get; set; }

        /// <summary>
        /// Gets or sets the RaidPlayer40 value as an IntPtr.
        /// </summary>
        public IntPtr RaidPlayer40 { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the RaidPlayer5 object.
        /// </summary>
        public IntPtr RaidPlayer5 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr representation of the RaidPlayer6 property.
        /// </summary>
        public IntPtr RaidPlayer6 { get; set; }

        /// <summary>
        /// Gets or sets the player's RAID 7 handle.
        /// </summary>
        public IntPtr RaidPlayer7 { get; set; }

        /// <summary>
        /// Gets or sets the IntPtr representing the RaidPlayer8.
        /// </summary>
        public IntPtr RaidPlayer8 { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the RaidPlayer9 object.
        /// </summary>
        public IntPtr RaidPlayer9 { get; set; }

        /// <summary>
        /// Retrieves a collection of pointers to raid players.
        /// </summary>
        /// <returns>An IEnumerable of IntPtr representing pointers to raid player objects.</returns>
        public IEnumerable<IntPtr> GetPointers()
        {
            return new List<IntPtr>()
            {
                RaidPlayer1,
                RaidPlayer2,
                RaidPlayer3,
                RaidPlayer4,
                RaidPlayer5,
                RaidPlayer6,
                RaidPlayer7,
                RaidPlayer8,
                RaidPlayer9,
                RaidPlayer10,
                RaidPlayer11,
                RaidPlayer12,
                RaidPlayer13,
                RaidPlayer14,
                RaidPlayer15,
                RaidPlayer16,
                RaidPlayer17,
                RaidPlayer18,
                RaidPlayer19,
                RaidPlayer20,
                RaidPlayer21,
                RaidPlayer22,
                RaidPlayer23,
                RaidPlayer24,
                RaidPlayer25,
                RaidPlayer26,
                RaidPlayer27,
                RaidPlayer28,
                RaidPlayer29,
                RaidPlayer30,
                RaidPlayer31,
                RaidPlayer32,
                RaidPlayer33,
                RaidPlayer34,
                RaidPlayer35,
                RaidPlayer36,
                RaidPlayer37,
                RaidPlayer38,
                RaidPlayer39,
                RaidPlayer40
            };
        }
    }
}