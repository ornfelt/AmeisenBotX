using System;

namespace AmeisenBotX.Wow.Offsets
{
    ///<summary>
    ///Represents an interface for accessing various game offsets in memory.
    ///</summary>
    ///<remarks>
    ///This interface provides properties for accessing specific memory offsets related to game objects, player characters, spells, and other aspects of the game. It also includes an initialization method for setting the main module base address.
    ///</remarks>
    public interface IOffsetList
    {
        /// <summary>
        /// Gets the value of AuraCount1 property which holds a pointer to an integer.
        /// </summary>
        IntPtr AuraCount1 { get; }

        /// <summary>
        /// Gets the number of auras in the IntPtr AuraCount2.
        /// </summary>
        IntPtr AuraCount2 { get; }

        /// <summary>
        /// Gets or sets the IntPtr representing AuraTable1.
        /// </summary>
        IntPtr AuraTable1 { get; }

        /// <summary>
        /// Gets or sets the value of the AuraTable2 property.
        /// </summary>
        IntPtr AuraTable2 { get; }

        /// <summary>
        /// Gets or sets the pointer to the finished battleground.
        /// </summary>
        IntPtr BattlegroundFinished { get; }

        /// <summary>
        /// Gets the status of the battlefield as an IntPtr.
        /// </summary>
        IntPtr BattlegroundStatus { get; }

        /// <summary>
        /// Gets or sets the handle to a memory location that contains a timer value.
        /// </summary>
        IntPtr BreathTimer { get; }

        /// <summary>
        /// Gets the offset value of the camera.
        /// </summary>
        IntPtr CameraOffset { get; }

        /// <summary>
        /// Gets or sets the pointer to the camera.
        /// </summary>
        IntPtr CameraPointer { get; }

        /// <summary>
        /// Gets or sets the click to move action as an IntPtr.
        /// </summary>
        IntPtr ClickToMoveAction { get; }

        /// <summary>
        /// Gets or sets the distance for clicking to move.
        /// </summary>
        IntPtr ClickToMoveDistance { get; }

        /// <summary>
        /// Gets the value of the ClickToMoveGuid property.
        /// </summary>
        IntPtr ClickToMoveGuid { get; }

        /// <summary>
        /// Gets or sets the turn speed for the click-to-move feature.
        /// </summary>
        IntPtr ClickToMoveTurnSpeed { get; }

        /// <summary>
        /// Gets or sets the X coordinate for the click-to-move interaction.
        /// </summary>
        IntPtr ClickToMoveX { get; }

        /// <summary>
        /// Gets the IntPtr representing the client connection.
        /// </summary>
        IntPtr ClientConnection { get; }

        /// <summary>
        /// Gets or sets the climb angle of the object.
        /// </summary>
        IntPtr ClimbAngle { get; }

        /// <summary>
        /// Gets or sets a pointer to the CollisionM2C object.
        /// </summary>
        IntPtr CollisionM2C { get; }

        /// <summary>
        /// Gets the pointer to the collision model converted to a scalar value.
        /// </summary>
        IntPtr CollisionM2S { get; }

        /// <summary>
        /// Gets or sets the collision World Model Object.
        /// </summary>
        IntPtr CollisionWMO { get; }

        /// <summary>
        /// Gets the combo points as an IntPtr.
        /// </summary>
        IntPtr ComboPoints { get; }

        /// <summary>
        /// Gets the position of the corpse as an IntPtr.
        /// </summary>
        IntPtr CorpsePosition { get; }

        /// <summary>
        /// Gets the ID of the spell currently being casted.
        /// </summary>
        IntPtr CurrentlyCastingSpellId { get; }

        /// <summary>
        /// Gets or sets the ID of the spell that is currently being channeled.
        /// </summary>
        IntPtr CurrentlyChannelingSpellId { get; }

        /// <summary>
        /// Gets the current object manager of type IntPtr.
        /// </summary>
        IntPtr CurrentObjectManager { get; }

        /// <summary>
        /// Gets the offset of the EndScene function in the IntPtr type.
        /// </summary>
        IntPtr EndSceneOffset { get; }

        /// <summary>
        /// Gets the offset of the EndScene function in the device.
        /// </summary>
        IntPtr EndSceneOffsetDevice { get; }

        /// <summary>
        /// Gets the end scene static device as an IntPtr.
        /// </summary>
        IntPtr EndSceneStaticDevice { get; }

        /// <summary>
        /// Gets or sets the first object as an IntPtr.
        /// </summary>
        IntPtr FirstObject { get; }

        /// <summary>
        /// Retrieves the IntPtr handle of the function that is called when a GameObject is right-clicked.
        /// </summary>
        IntPtr FunctionGameobjectOnRightClick { get; }

        /// <summary>
        /// Gets the active player's object as an IntPtr.
        /// </summary>
        IntPtr FunctionGetActivePlayerObject { get; }

        ///<summary>
        /// Gets the localized text as an IntPtr.
        ///</summary>
        IntPtr FunctionGetLocalizedText { get; }

        /// <summary>
        /// Gets the function handle for terrain click.
        /// </summary>
        IntPtr FunctionHandleTerrainClick { get; }

        /// <summary>
        /// Gets the IntPtr representing the FunctionIsOutdoors property.
        /// </summary>
        IntPtr FunctionIsOutdoors { get; }

        /// <summary>
        /// Gets the function pointer to the LuaDoString function.
        /// </summary>
        IntPtr FunctionLuaDoString { get; }

        /// <summary>
        /// Gets the function pointer for the PlayerClickToMove function.
        /// </summary>
        IntPtr FunctionPlayerClickToMove { get; }

        /// <summary>
        /// Stops the player from moving when they click.
        /// </summary>
        IntPtr FunctionPlayerClickToMoveStop { get; }

        /// <summary>
        /// Gets or sets the target value for the function.
        /// </summary>
        IntPtr FunctionSetTarget { get; }

        /// <summary>
        /// Gets the function traceline as an IntPtr.
        /// </summary>
        IntPtr FunctionTraceline { get; }

        /// <summary>
        /// Get the reaction of the function unit as an IntPtr.
        /// </summary>
        IntPtr FunctionUnitGetReaction { get; }

        /// <summary>
        /// Gets or sets the function unit for right-click actions.
        /// </summary>
        IntPtr FunctionUnitOnRightClick { get; }

        /// <summary>
        /// Gets or sets the facing of the Function Unit.
        /// </summary>
        IntPtr FunctionUnitSetFacing { get; }

        /// <summary>
        /// Gets the IntPtr that represents the world frame function.
        /// </summary>
        IntPtr FunctionWorldFrame { get; }

        /// <summary>
        /// Gets the function world render of type IntPtr.
        /// </summary>
        IntPtr FunctionWorldRender { get; }

        /// <summary>
        /// Gets the function to render the world in the world.
        /// </summary>
        IntPtr FunctionWorldRenderWorld { get; }

        /// <summary>
        /// Gets or sets the value representing the game state as a pointer to an unmanaged memory location.
        /// </summary>
        IntPtr GameState { get; }

        /// <summary>
        /// Gets a pointer to the current game instance.
        /// </summary>
        IntPtr IsIngame { get; }

        /// <summary>
        /// Gets a value indicating whether the world is loaded.
        /// </summary>
        IntPtr IsWorldLoaded { get; }

        /// <summary>
        /// Gets the last target GUID.
        /// </summary>
        IntPtr LastTargetGuid { get; }

        /// <summary>
        /// Gets or sets the handle to the open loot window.
        /// </summary>
        IntPtr LootWindowOpen { get; }

        /// <summary>
        /// Gets the map identifier as an IntPtr.
        /// </summary>
        IntPtr MapId { get; }

        /// <summary>
        /// Gets or sets the name base as an IntPtr.
        /// </summary>
        IntPtr NameBase { get; }

        /// <summary>
        /// Gets or sets the name mask represented as an IntPtr.
        /// </summary>
        IntPtr NameMask { get; }

        /// <summary>
        /// Gets or sets the name store as an IntPtr.
        /// </summary>
        IntPtr NameStore { get; }

        /// <summary>
        /// Gets or sets the name of the string as an IntPtr.
        /// </summary>
        IntPtr NameString { get; }

        /// <summary>
        /// Gets the pointer to the next object.
        /// </summary>
        IntPtr NextObject { get; }

        /// <summary>
        /// Gets or sets the pointer to the party leader.
        /// </summary>
        IntPtr PartyLeader { get; }

        /// <summary>
        /// Gets the IntPtr representing the Party Player Guids.
        /// </summary>
        IntPtr PartyPlayerGuids { get; }

        /// <summary>
        /// Gets or sets the pet Guid as an IntPtr.
        /// </summary>
        IntPtr PetGuid { get; }

        /// <summary>
        /// Gets the memory address of the player base.
        /// </summary>
        IntPtr PlayerBase { get; }

        /// <summary>
        /// Gets or sets the unique identifier of the player.
        /// </summary>
        IntPtr PlayerGuid { get; }

        /// <summary>
        /// Gets or sets the starting point of the RAID group.
        /// </summary>
        IntPtr RaidGroupStart { get; }

        /// <summary>
        /// Gets the pointer to the Raid Leader.
        /// </summary>
        IntPtr RaidLeader { get; }

        /// <summary>
        /// Gets or sets a pointer to the render flags.
        /// </summary>
        IntPtr RenderFlags { get; }

        /// <summary>
        /// Gets the IntPtr representation of the Runes property.
        /// </summary>
        IntPtr Runes { get; }

        /// <summary>
        /// Gets the RuneType of the IntPtr.
        /// </summary>
        IntPtr RuneType { get; }

        /// <summary>
        /// Gets the pointer to the target GUID.
        /// </summary>
        IntPtr TargetGuid { get; }

        /// <summary>
        /// Gets the number of milliseconds elapsed since the system started.
        /// </summary>
        IntPtr TickCount { get; }

        ///<summary>Gets or sets the position of the wow dynamic object.</summary>
        IntPtr WowDynobjectPosition { get; }

        /// <summary>
        /// Gets the position of the WowGameobject as an IntPtr.
        /// </summary>
        IntPtr WowGameobjectPosition { get; }

        /// <summary>
        /// Gets or sets the pointer to the wow object descriptor.
        /// </summary>
        IntPtr WowObjectDescriptor { get; }

        /// <summary>
        /// Gets the type of the Wow object as an IntPtr.
        /// </summary>
        IntPtr WowObjectType { get; }

        /// <summary>
        /// Gets or sets the entry for WowUnitDb as an IntPtr.
        /// </summary>
        IntPtr WowUnitDbEntry { get; }

        /// <summary>
        /// Gets or sets the name of the WowUnitDbEntry as an IntPtr.
        /// </summary>
        IntPtr WowUnitDbEntryName { get; }

        /// <summary>
        /// Gets or sets the type of the WowUnitDbEntry as an IntPtr.
        /// </summary>
        IntPtr WowUnitDbEntryType { get; }

        /// <summary>
        /// Gets or sets a pointer to the WowUnitIsAutoAttacking property.
        /// </summary>
        IntPtr WowUnitIsAutoAttacking { get; }

        /// <summary>
        /// Gets or sets the position of WowUnit as an IntPtr.
        /// </summary>
        IntPtr WowUnitPosition { get; }

        /// <summary>
        /// Gets or sets the identifier of the zone represented by the IntPtr.
        /// </summary>
        IntPtr ZoneId { get; }

        /// <summary>
        /// Gets or sets the subtext of the Zone.
        /// </summary>
        IntPtr ZoneSubText { get; }

        /// <summary>
        /// Gets or sets the zone text as an IntPtr.
        /// </summary>
        IntPtr ZoneText { get; }

        /// <summary>
        /// Initializes the function with the specified main module base address.
        /// </summary>
        void Init(IntPtr mainModuleBase);
    }
}