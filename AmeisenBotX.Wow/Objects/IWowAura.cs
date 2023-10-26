namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents an aura in the World of Warcraft game.
    /// </summary>
    public interface IWowAura
    {
        ///<summary>
        /// Gets or sets the creator of the item.
        ///</summary>
        public ulong Creator { get; }

        /// <summary>
        /// Gets the flags associated with the object.
        /// </summary>
        public byte Flags { get; }

        /// <summary>
        /// Gets the level value.
        /// </summary>
        public byte Level { get; }

        /// <summary>
        /// Gets or sets the spell ID.
        /// </summary>
        public int SpellId { get; }

        /// <summary>
        /// Gets the number of elements in the stack.
        /// </summary>
        public byte StackCount { get; }
    }
}