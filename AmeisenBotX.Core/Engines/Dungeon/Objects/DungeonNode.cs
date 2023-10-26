using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;

namespace AmeisenBotX.Core.Engines.Dungeon.Objects
{
    public class DungeonNode
    {
        /// <summary>
        /// Constructor for creating a new dungeon node.
        /// </summary>
        /// <param name="position">The position of the dungeon node.</param>
        /// <param name="type">The type of the dungeon node (optional, defaults to DungeonNodeType.Normal).</param>
        /// <param name="extra">Extra information about the dungeon node (optional).</param>
        public DungeonNode(Vector3 position, DungeonNodeType type = DungeonNodeType.Normal, string extra = "")
        {
            Position = position;
            Type = type;
            Extra = extra;
        }

        /// <summary>
        /// Gets the value of the public string Extra property.
        /// </summary>
        public string Extra { get; }

        /// <summary>
        /// Gets the position of the object in 3D space.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the type of the dungeon node.
        /// </summary>
        public DungeonNodeType Type { get; }
    }
}