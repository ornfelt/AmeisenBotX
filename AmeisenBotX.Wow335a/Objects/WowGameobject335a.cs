using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;
using System.Collections.Specialized;
using System.Globalization;

namespace AmeisenBotX.Wow335a.Objects
{
    [Serializable]
    public class WowGameobject335a : WowObject335a, IWowGameobject
    {
        /// <summary>
        /// Gets or sets the value of Bytes0.
        /// </summary>
        public byte Bytes0 { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created the object.
        /// </summary>
        public ulong CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the DisplayId value.
        /// </summary>
        public int DisplayId { get; set; }

        /// <summary>
        /// Gets or sets the faction of the code. 
        /// </summary>
        public int Faction { get; set; }

        /// <summary>
        /// Gets or sets the BitVector32 flags.
        /// </summary>
        public BitVector32 Flags { get; set; }

        /// <summary>
        /// Gets or sets the type of WowGameObject.
        /// </summary>
        public WowGameObjectType GameObjectType { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Converts the object to its string representation.
        /// </summary>
        public override string ToString()
        {
            return $"GameObject: [{EntryId}] ({(Enum.IsDefined(typeof(WowGameObjectDisplayId), DisplayId) ? ((WowGameObjectDisplayId)DisplayId).ToString() : DisplayId.ToString(CultureInfo.InvariantCulture))}:{DisplayId})";
        }

        /// <summary>
        /// Updates the game object information based on the current memory state.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowGameobjectDescriptor335a objPtr)
                && Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowGameobjectPosition), out Vector3 position))
            {
                GameObjectType = (WowGameObjectType)objPtr.GameobjectBytes1;
                CreatedBy = objPtr.CreatedBy;
                Bytes0 = objPtr.GameobjectBytes0;
                DisplayId = objPtr.DisplayId;
                Faction = objPtr.Faction;
                Flags = new(objPtr.Flags);
                Level = objPtr.Level;
                Position = position;
            }
        }
    }
}