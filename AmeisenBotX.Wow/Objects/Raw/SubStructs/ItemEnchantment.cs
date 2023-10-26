using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow.Objects.Raw.SubStructs
{
    /// <summary>
    /// Represents an item enchantment.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ItemEnchantment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the duration of something.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the value of the C property.
        /// </summary>
        public short C { get; set; }

        /// <summary>
        /// Gets or sets the value of property D.
        /// </summary>
        public short D { get; set; }
    }
}