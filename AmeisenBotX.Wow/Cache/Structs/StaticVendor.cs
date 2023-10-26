using AmeisenBotX.Common.Math;

namespace AmeisenBotX.Wow.Cache.Structs
{
    /// <summary>
    /// Represents a static vendor that implements the ILikeUnit interface.
    /// </summary>
    /// <remarks>
    /// This class contains properties and methods related to a static vendor in the game. 
    /// It implements the ILikeUnit interface to indicate that the vendor likes both Alliance and Horde factions.
    /// </remarks>
    public record StaticVendor : ILikeUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticVendor"/> class.
        /// </summary>
        /// <param name="entry">The vendor's entry.</param>
        /// <param name="mapId">The map ID where the vendor is located.</param>
        /// <param name="posX">The X coordinate of the vendor's position.</param>
        /// <param name="posY">The Y coordinate of the vendor's position.</param>
        /// <param name="posZ">The Z coordinate of the vendor's position.</param>
        /// <param name="ammo">Specifies whether the vendor sells ammunition.</param>
        /// <param name="food">Specifies whether the vendor sells food.</param>
        /// <param name="poison">Specifies whether the vendor sells poison.</param>
        /// <param name="reagent">Specifies whether the vendor sells reagents.</param>
        /// <param name="repairer">Specifies whether the vendor is a repairer.</param>
        /// <param name="likesHorde">Specifies whether the vendor prefers Horde faction.</param>
        /// <param name="likesAlliance">Specifies whether the vendor prefers Alliance faction.</param>
        public StaticVendor(int entry, int mapId, float posX, float posY, float posZ, bool ammo, bool food, bool poison, bool reagent, bool repairer, bool likesHorde, bool likesAlliance)
        {
            Entry = entry;
            MapId = mapId;
            Position = new Vector3(posX, posY, posZ);
            IsAmmoVendor = ammo;
            IsFoodVendor = food;
            IsPoisonVendor = poison;
            IsReagentVendor = reagent;
            IsRepairer = repairer;
            LikesHorde = likesHorde;
            LikesAlliance = likesAlliance;
        }

        /// <summary>
        /// Gets or sets the value of the Entry.
        /// </summary>
        public int Entry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is an ammo vendor.
        /// </summary>
        public bool IsAmmoVendor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is a food vendor.
        /// </summary>
        public bool IsFoodVendor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vendor is a poison vendor.
        /// </summary>
        public bool IsPoisonVendor { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the entity is a reagent vendor.
        /// </summary>
        public bool IsReagentVendor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person is a repairer.
        /// </summary>
        public bool IsRepairer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the individual likes the alliance.
        /// </summary>
        public bool LikesAlliance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person likes the Horde faction.
        /// </summary>
        public bool LikesHorde { get; set; }

        /// <summary>
        /// Gets or sets the map ID.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Gets or sets the position of the object in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }
    }
}