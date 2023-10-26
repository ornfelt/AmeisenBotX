using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow.Objects.Raw.SubStructs
{
    /// <summary>
    /// Represents a visible item enchantment.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VisibleItemEnchantment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the value of the property C.
        /// </summary>
        public short C { get; set; }

        /// <summary>
        /// Gets or sets the value of D.
        /// </summary>
        public short D { get; set; }
    }
}