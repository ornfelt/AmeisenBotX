using AmeisenBotX.Common.Math;

/// <summary>
/// Contains structs related to caching data for static mailboxes in the game world.
/// </summary>
namespace AmeisenBotX.Wow.Cache.Structs
{
    /// <summary>
    /// Represents a static mailbox in the game world that can be liked by a player.
    /// Implements the ILikeUnit interface.
    /// </summary>
    public record StaticMailbox : ILikeUnit
    {
        /// <summary>
        /// Initializes a new instance of the StaticMailbox class.
        /// </summary>
        /// <param name="entry">The entry of the mailbox.</param>
        /// <param name="mapId">The map ID of the mailbox.</param>
        /// <param name="posX">The X position of the mailbox.</param>
        /// <param name="posY">The Y position of the mailbox.</param>
        /// <param name="posZ">The Z position of the mailbox.</param>
        /// <param name="likesHorde">A boolean representing if the mailbox likes Horde faction.</param>
        /// <param name="likesAlliance">A boolean representing if the mailbox likes Alliance faction.</param>
        public StaticMailbox(int entry, int mapId, float posX, float posY, float posZ, bool likesHorde, bool likesAlliance)
        {
            Entry = entry;
            MapId = mapId;
            Position = new Vector3(posX, posY, posZ);
            LikesHorde = likesHorde;
            LikesAlliance = likesAlliance;
        }

        /// <summary>
        /// Gets or sets the entry value.
        /// </summary>
        public int Entry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person likes being part of an alliance.
        /// </summary>
        public bool LikesAlliance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person likes Horde.
        /// </summary>
        public bool LikesHorde { get; set; }

        /// <summary>
        /// Gets or sets the map ID.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Gets or sets the position of the object in three-dimensional space.
        /// </summary>
        public Vector3 Position { get; set; }
    }
}