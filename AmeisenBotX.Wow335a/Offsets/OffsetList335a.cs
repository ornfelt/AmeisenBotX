using AmeisenBotX.Wow.Offsets;
using System;

namespace AmeisenBotX.Wow335a.Offsets
{
    /// <summary>
    /// Represents a list of offsets used in the OffsetList335a class.
    /// </summary>
    public class OffsetList335a : IOffsetList
    {
        /// <summary>
        /// Gets the IntPtr representing the AuraCount1 property.
        /// The default value is set to new IntPtr(0xDD0).
        /// </summary>
        public IntPtr AuraCount1 { get; } = new(0xDD0);

        /// <summary>
        /// Gets the pointer to the AuraCount2 property which contains the value 0xC54.
        /// </summary>
        public IntPtr AuraCount2 { get; } = new(0xC54);

        /// <summary>
        /// Gets the pointer to the AuraTable1 with the default value of 0xC50.
        /// </summary>
        public IntPtr AuraTable1 { get; } = new(0xC50);

        /// <summary>
        /// Gets or sets the pointer to the AuraTable2 with the value 0xC58.
        /// </summary>
        public IntPtr AuraTable2 { get; } = new(0xC58);

        /// <summary>
        /// Gets the memory address of the BattlegroundFinished variable.
        /// </summary>
        public IntPtr BattlegroundFinished { get; } = new(0xBEA588);

        /// <summary>
        /// Gets the pointer to the BattlegroundStatus.
        /// </summary>
        public IntPtr BattlegroundStatus { get; } = new(0xBEA4D0);

        /// <summary>
        /// Gets or sets the pointer to the BreathTimer IntPtr.
        /// </summary>
        public IntPtr BreathTimer { get; } = new(0xBD0BA0);

        /// <summary>
        /// Gets or sets the camera offset pointer.
        /// </summary>
        public IntPtr CameraOffset { get; } = new(0x7E20);

        /// <summary>
        /// Gets or sets the pointer to the camera.
        /// </summary>
        public IntPtr CameraPointer { get; } = new(0xB7436C);

        /// <summary>
        /// Gets the memory address for the ClickToMoveAction pointer.
        /// </summary>
        public IntPtr ClickToMoveAction { get; } = new(0xCA11D8 + 0x1C);

        /// <summary>
        /// Gets the distance for click to move.
        /// </summary>
        public IntPtr ClickToMoveDistance { get; } = new(0xCA11D8 + 0xC);

        /// <summary>
        /// Gets or sets the pointer to the memory address that indicates whether the click-to-move feature is enabled.
        /// </summary>
        public IntPtr ClickToMoveEnabled { get; } = new(0x30);

        /// <summary>
        /// Gets the memory address of the ClickToMove Guid.
        /// </summary>
        public IntPtr ClickToMoveGuid { get; } = new(0xCA11D8 + 0x20);

        /// <summary>
        /// Gets or sets the pointer used for the ClickToMove action.
        /// </summary>
        public IntPtr ClickToMovePointer { get; } = new(0xBD08F4);

        /// <summary>
        /// Gets the memory address for the ClickToMoveTurnSpeed property which is used for setting the turn speed when issuing a click-to-move command.
        /// </summary>
        public IntPtr ClickToMoveTurnSpeed { get; } = new(0xCA11D8 + 0x4);

        /// <summary>
        /// Gets the memory address for the ClickToMoveX property.
        /// </summary>
        public IntPtr ClickToMoveX { get; } = new(0xCA11D8 + 0x8C);

        /// <summary>
        /// Gets or sets the client connection.
        /// </summary>
        public IntPtr ClientConnection { get; } = new(0xC79CE0);

        /// <summary>
        /// Gets or sets the climb angle of the object.
        /// </summary>
        public IntPtr ClimbAngle { get; } = new(0x858);

        /// <summary>
        /// Gets the IntPtr representing the CollisionM2C property.
        /// </summary>
        public IntPtr CollisionM2C { get; } = new(0x7A50CF);

        /// <summary>
        /// Gets the memory address for the CollisionM2S property.
        /// </summary>
        public IntPtr CollisionM2S { get; } = new(0x7A52EC);

        /// <summary>
        /// Gets the IntPtr representing the CollisionWMO property.
        /// </summary>
        public IntPtr CollisionWMO { get; } = new(0x7AE7EA);

        /// <summary>
        /// Gets the IntPtr representing the ComboPoints property which points to the memory address 0xBD084D.
        /// </summary>
        public IntPtr ComboPoints { get; } = new(0xBD084D);

        /// <summary>
        /// Gets the memory address of the position of the corpse.
        /// </summary>
        public IntPtr CorpsePosition { get; } = new(0xBD0A58);

        /// <summary>
        /// Gets the IntPtr representing the ID of the currently casting spell.
        /// </summary>
        public IntPtr CurrentlyCastingSpellId { get; } = new(0xA6C);

        /// <summary>
        /// Gets the IntPtr object representing the currently channeling spell ID,
        /// which is set to a new instance of IntPtr with the value 0xA80.
        /// </summary>
        public IntPtr CurrentlyChannelingSpellId { get; } = new(0xA80);

        /// <summary>
        /// Gets the current object manager.
        /// </summary>
        public IntPtr CurrentObjectManager { get; } = new(0x2ED0);

        /// <summary>
        /// Gets or sets the offset for the EndScene function in the underlying IntPtr object. 
        /// By default, the offset is set to 0xA8, but it is recommended to use 0xAC instead 
        /// to avoid potential crashes in the clear function.
        /// </summary>
        public IntPtr EndSceneOffset { get; } = new(0xA8); // maybe use 0xAC, clear function, leads to many crashes

        /// <summary>
        /// Gets or sets the device offset for the EndScene method.
        /// </summary>
        public IntPtr EndSceneOffsetDevice { get; } = new(0x397C);

        /// <summary>
        /// Gets the pointer to the end scene static device.
        /// </summary>
        public IntPtr EndSceneStaticDevice { get; } = new(0xC5DF88);

        /// <summary>
        /// Gets the pointer to the first object.
        /// </summary>
        public IntPtr FirstObject { get; } = new(0xAC);

        /// <summary>
        /// Represents the memory address of the function "FunctionGameobjectOnRightClick" 
        /// which is called when the user right-clicks on a GameObject in the game.
        /// </summary>
        /// <returns>
        /// The memory pointer (IntPtr) of the function "FunctionGameobjectOnRightClick".
        /// </returns>
        public IntPtr FunctionGameobjectOnRightClick { get; } = new(0x711140);

        ///<summary>
        /// Gets the pointer to the active player object.
        ///</summary>
        public IntPtr FunctionGetActivePlayerObject { get; } = new(0x4038F0);

        /// <summary>
        /// Gets the localized text by returning an IntPtr value.
        /// </summary>
        public IntPtr FunctionGetLocalizedText { get; } = new(0x7225E0);

        /// <summary>
        /// Gets the handle for the terrain click function.
        /// </summary>
        public IntPtr FunctionHandleTerrainClick { get; } = new(0x80C340);

        ///<summary>
        /// Represents the pointer to the IsOutdoors function.
        ///</summary>
        public IntPtr FunctionIsOutdoors { get; } = new(0x71B7F0);

        /// <summary>
        /// Gets or sets the function pointer for executing a Lua script as a string.
        /// </summary>
        public IntPtr FunctionLuaDoString { get; } = new(0x819210);

        /// <summary>
        /// Gets the function pointer for the player click-to-move functionality.
        /// </summary>
        public IntPtr FunctionPlayerClickToMove { get; } = new(0x727400);

        /// <summary>
        /// Gets the pointer to the FunctionPlayerClickToMoveStop function.
        /// </summary>
        public IntPtr FunctionPlayerClickToMoveStop { get; } = new(0x72B3A0);

        /// <summary>
        /// Gets the memory address of the native function "SetTarget".
        /// </summary>
        public IntPtr FunctionSetTarget { get; } = new(0x524BF0);

        /// <summary>
        /// Gets the pointer to the FunctionTraceline address.
        /// </summary>
        public IntPtr FunctionTraceline { get; } = new(0x7A3B70);

        /// <summary>
        /// Gets the pointer to the FunctionUnitGetReaction function.
        /// </summary>
        public IntPtr FunctionUnitGetReaction { get; } = new(0x7251C0);

        /// <summary>
        /// Gets the pointer to the function unit that is triggered when the right mouse button is clicked.
        /// </summary>
        public IntPtr FunctionUnitOnRightClick { get; } = new(0x731260);

        /// <summary>
        /// The property representing the IntPtr value for the FunctionUnitSetFacing.
        /// </summary>
        public IntPtr FunctionUnitSetFacing { get; } = new(0x72EA50);

        /// <summary>
        /// Gets the memory address of the FunctionWorldFrame.
        /// </summary>
        public IntPtr FunctionWorldFrame { get; } = new(0x4FA390);

        /// <summary>
        /// Gets the memory address of the FunctionWorldRender function.
        /// </summary>
        public IntPtr FunctionWorldRender { get; } = new(0x4F8EA0);

        /// <summary>
        /// Gets the memory address of the FunctionWorldRenderWorld property,
        /// which returns a pointer to the rendering function for the world.
        /// The memory address is initialized to 0x4FAF90.
        /// </summary>
        public IntPtr FunctionWorldRenderWorld { get; } = new(0x4FAF90);

        /// <summary>
        /// Represents a pointer to the game state.
        /// </summary>
        public IntPtr GameState { get; } = new(0xB6A9E0);

        /// <summary>
        /// Gets the pointer to the IsIngame property.
        /// </summary>
        public IntPtr IsIngame { get; } = new(0xBEBAA4);

        /// <summary>
        /// Gets the pointer to the loaded world.
        /// </summary>
        public IntPtr IsWorldLoaded { get; } = new(0xBEBA40);

        /// <summary>
        /// Gets the last target GUID.
        /// </summary>
        public IntPtr LastTargetGuid { get; } = new(0xBD07B8);

        /// <summary>
        /// The pointer to the Loot Window Open function in memory.
        /// </summary>
        public IntPtr LootWindowOpen { get; } = new(0xBFA8D8);

        /// <summary>
        /// Gets the mapped ID.
        /// </summary>
        public IntPtr MapId { get; } = new(0xADFBC4);

        /// <summary>
        /// Gets the name base <see cref="IntPtr"/> with the value 0x1C.
        /// </summary>
        public IntPtr NameBase { get; } = new(0x1C);

        /// <summary>
        /// Gets the name mask value.
        /// </summary>
        /// <returns>
        /// The name mask value.
        /// </returns>
        public IntPtr NameMask { get; } = new(0x24);

        /// <summary>
        /// Gets the pointer to the NameStore with the specified memory address.
        /// </summary>
        public IntPtr NameStore { get; } = new(0xC5D940);

        /// <summary>
        /// Gets or sets the name string as a pointer to an IntPtr object with the value 0x20.
        /// </summary>
        public IntPtr NameString { get; } = new(0x20);

        /// <summary>
        /// Gets the next object as an IntPtr with a default value of 0x3C.
        /// </summary>
        public IntPtr NextObject { get; } = new(0x3C);

        /// <summary>
        /// Gets the pointer to the party leader.
        /// </summary>
        public IntPtr PartyLeader { get; } = new(0xBD1968);

        /// <summary>
        /// Gets the IntPtr value representing the PartyPlayerGuids property.
        /// </summary>
        public IntPtr PartyPlayerGuids { get; } = new(0xBD1948);

        /// <summary>
        /// Gets the pointer to the PetGuid.
        /// </summary>
        public IntPtr PetGuid { get; } = new(0xC234D0);

        /// <summary>
        /// Gets the memory address of the player base.
        /// </summary>
        public IntPtr PlayerBase { get; } = new(0xD38AE4);

        /// <summary>
        /// Gets the player GUID.
        /// </summary>
        public IntPtr PlayerGuid { get; } = new(0xCA1238);

        /// <summary>
        /// Gets the pointer to the start address of the RAID group.
        /// </summary>
        public IntPtr RaidGroupStart { get; } = new(0xBEB568);

        /// <summary>
        /// Gets the memory address of the Raid Leader.
        /// </summary>
        public IntPtr RaidLeader { get; } = new(0xBD1990);

        /// <summary>
        /// Gets the render flags associated with the object.
        /// </summary>
        public IntPtr RenderFlags { get; } = new(0xCD774C);

        /// <summary>
        /// Gets or sets the pointer to the Runes object.
        /// </summary>
        public IntPtr Runes { get; } = new(0xC24388);

        /// <summary>
        /// Gets the pointer to the RuneType.
        /// </summary>
        public IntPtr RuneType { get; } = new(0xC24304);

        /// <summary>
        /// Gets the target GUID represented as an IntPtr.
        /// </summary>
        public IntPtr TargetGuid { get; } = new(0xBD07B0);

        /// <summary>
        /// Gets the tick count value.
        /// </summary>
        public IntPtr TickCount { get; } = new(0xB499A4);

        /// <summary>
        /// Gets the memory address of the WowDynobjectPosition.
        /// </summary>
        public IntPtr WowDynobjectPosition { get; } = new(0xE8);

        /// <summary>
        /// Gets the memory address of the WowGameobjectPosition property.
        /// </summary>
        public IntPtr WowGameobjectPosition { get; } = new(0x1D8);

        /// <summary>
        /// Gets or sets the pointer to the WowObjectDescriptor.
        /// </summary>
        public IntPtr WowObjectDescriptor { get; } = new(0x8);

        /// <summary>
        /// Gets the IntPtr representing the WowObjectType.
        /// </summary>
        public IntPtr WowObjectType { get; } = new(0x14);

        /// <summary>
        /// Gets the IntPtr representing the WowUnitDbEntry property.
        /// </summary>
        public IntPtr WowUnitDbEntry { get; } = new(0x964);

        /// <summary>
        /// Gets or sets the pointer to the WowUnitDbEntryName field.
        /// </summary>
        public IntPtr WowUnitDbEntryName { get; } = new(0x5C);

        /// <summary>
        /// Represents the WowUnitDbEntryType property.
        /// </summary>
        public IntPtr WowUnitDbEntryType { get; } = new(0x10);

        /// <summary>
        /// Gets the memory address of the "WowUnitIsAutoAttacking" property, which represents whether the WoW unit is auto-attacking.
        /// </summary>
        public IntPtr WowUnitIsAutoAttacking { get; } = new(0xA20);

        ///<summary>
        ///Gets the memory address of the WowUnitPosition.
        ///</summary>
        public IntPtr WowUnitPosition { get; } = new(0x798);

        /// <summary>
        /// Gets the pointer to the ZoneId.
        /// </summary>
        public IntPtr ZoneId { get; } = new(0xBD080C);

        /// <summary>
        /// Gets the IntPtr representing the ZoneSubText property, which holds the memory address 0xBD0784.
        /// </summary>
        public IntPtr ZoneSubText { get; } = new(0xBD0784);

        /// <summary>
        /// Gets or sets the memory pointer to the ZoneText data.
        /// </summary>
        public IntPtr ZoneText { get; } = new(0xBD0788);

        /// <summary>
        /// Initializes the method with the given main module base.
        /// </summary>
        public void Init(IntPtr mainModuleBase)
        {
            // unused, ASLR not enabled
        }
    }
}