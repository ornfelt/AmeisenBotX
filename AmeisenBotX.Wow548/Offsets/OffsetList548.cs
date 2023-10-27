using AmeisenBotX.Wow.Offsets;

namespace AmeisenBotX.Wow548.Offsets
{
    /// <summary>
    /// Represents a list of offsets with two properties: AuraCount1 and AuraCount2. AuraCount1 is an IntPtr representing the memory address 0x1218, while AuraCount2 is initialized with the hexadecimal value 0xE14.
    /// </summary>
    public class OffsetList548 : IOffsetList
    {
        /// <summary>
        /// Gets the AuraCount1 property value, which is an IntPtr representing the memory address 0x1218.
        /// </summary>
        public IntPtr AuraCount1 { get; } = new(0x1218);

        /// <summary>
        /// Gets the IntPtr value of AuraCount2, which is initialized with the hexadecimal value 0xE14.
        /// </summary>
        public IntPtr AuraCount2 { get; } = new(0xE14);

        /// <summary>
        /// Gets the IntPtr representing AuraTable1.
        /// </summary>
        public IntPtr AuraTable1 { get; } = new(0xE18);

        /// <summary>
        /// Gets the IntPtr value representing the AuraTable2 property.
        /// </summary>
        public IntPtr AuraTable2 { get; } = new(0xE1C);

        /// <summary>
        /// Gets or sets the IntPtr representing the finishing state of the battleground.
        /// </summary>
        public IntPtr BattlegroundFinished { get; private set; }

        /// <summary>
        /// Gets or sets the status of the battleground as an IntPtr.
        /// </summary>
        public IntPtr BattlegroundStatus { get; private set; }

        /// <summary>
        /// Gets or sets the breath timer.
        /// </summary>
        public IntPtr BreathTimer { get; } = new(0x0);

        /// <summary>
        /// Gets the offset for the camera.
        /// </summary>
        public IntPtr CameraOffset { get; } = new(0x8208);

        /// <summary>
        /// Gets or sets the pointer to the camera.
        /// </summary>
        public IntPtr CameraPointer { get; private set; }

        /// <summary>
        /// Gets or sets the action represented as an IntPtr for clicking to initiate movement.
        /// </summary>
        public IntPtr ClickToMoveAction { get; private set; }

        /// <summary>
        /// Gets or sets the distance that represents the click-to-move functionality.
        /// </summary>
        public IntPtr ClickToMoveDistance { get; private set; }

        /// <summary>
        /// Gets or sets the GUID for the ClickToMove movement.
        /// </summary>
        public IntPtr ClickToMoveGuid { get; private set; }

        /// <summary>
        /// Gets or sets the turn speed for the ClickToMove functionality.
        /// </summary>
        public IntPtr ClickToMoveTurnSpeed { get; private set; }

        /// <summary>
        /// Gets or sets the pointer to the ClickToMoveX property. 
        /// </summary>
        public IntPtr ClickToMoveX { get; private set; }

        /// <summary>
        /// Gets or sets the client connection pointer used for communication.
        /// </summary>
        public IntPtr ClientConnection { get; private set; }

        /// <summary>
        /// Gets or sets the climb angle.
        /// </summary>
        public IntPtr ClimbAngle { get; } = new(0x0);

        /// <summary>
        /// Gets the pointer to CollisionM2C.
        /// </summary>
        public IntPtr CollisionM2C { get; } = new(0x0);

        /// <summary>
        /// Gets the IntPtr representing the CollisionM2S, which is initialized with the value 0x0.
        /// </summary>
        public IntPtr CollisionM2S { get; } = new(0x0);

        /// <summary>
        /// Gets or sets the collision WMO pointer.
        /// </summary>
        public IntPtr CollisionWMO { get; } = new(0x0);

        /// <summary>
        /// Gets or sets the pointer to the ComboPoints property.
        /// </summary>
        public IntPtr ComboPoints { get; private set; }

        /// <summary>
        /// Gets or sets the position of the corpse as a pointer.
        /// </summary>
        public IntPtr CorpsePosition { get; private set; }

        /// <summary>
        /// Gets or sets the currently casting spell ID.
        /// </summary>
        public IntPtr CurrentlyCastingSpellId { get; } = new(0xCB8);

        /// <summary>
        /// Gets the pointer to the currently channeling spell ID.
        /// </summary>
        public IntPtr CurrentlyChannelingSpellId { get; } = new(0xCD0);

        /// <summary>
        /// Gets the pointer to the current object manager with the specified address.
        /// </summary>
        public IntPtr CurrentObjectManager { get; } = new(0x462C);

        /// <summary>
        /// Gets the offset for the EndScene method pointer in the IntPtr data type.
        /// </summary>
        public IntPtr EndSceneOffset { get; } = new(0xA8);

        /// <summary>
        /// Gets or sets the memory offset for the EndScene function device.
        /// </summary>
        public IntPtr EndSceneOffsetDevice { get; } = new(0x2820);

        /// <summary>
        /// Gets or sets the pointer to the end scene static device.
        /// </summary>
        public IntPtr EndSceneStaticDevice { get; private set; }

        /// <summary>
        /// Gets the IntPtr of the first object, initialized with value 0xCC.
        /// </summary>
        public IntPtr FirstObject { get; } = new(0xCC);

        /// <summary>
        /// Gets the IntPtr representing the FunctionGameobjectOnRightClick property, which has a default value of 0x0.
        /// </summary>
        public IntPtr FunctionGameobjectOnRightClick { get; } = new(0x0);

        /// <summary>
        /// Gets the pointer to the active player object.
        /// </summary>
        public IntPtr FunctionGetActivePlayerObject { get; private set; }

        /// <summary>
        /// Gets or sets the IntPtr for retrieving localized text.
        /// </summary>
        public IntPtr FunctionGetLocalizedText { get; private set; }

        /// <summary>
        /// Gets or sets the function handle for terrain click events.
        /// </summary>
        public IntPtr FunctionHandleTerrainClick { get; private set; }

        /// <summary>
        /// Gets or sets the function pointer representing the IntPtr for determining whether the current context is outdoors.
        /// </summary>
        public IntPtr FunctionIsOutdoors { get; private set; }

        /// <summary>
        /// Gets or sets the pointer to the FunctionLuaDoString method.
        /// </summary>
        public IntPtr FunctionLuaDoString { get; private set; }

        ///<summary>Gets or sets the function pointer to the FunctionPlayerClickToMove.</summary>
        public IntPtr FunctionPlayerClickToMove { get; private set; }

        /// <summary>
        /// The pointer to the FunctionPlayerClickToMoveStop property.
        /// This pointer is unused.
        /// </summary>
        public IntPtr FunctionPlayerClickToMoveStop { get; } = new(0x0); // unused

        /// <summary>
        /// Gets or sets the function pointer to set the target.
        /// </summary>
        public IntPtr FunctionSetTarget { get; private set; }

        /// <summary>
        /// Gets or sets the pointer to the FunctionTraceline property.
        /// </summary>
        public IntPtr FunctionTraceline { get; private set; }

        /// <summary>
        /// Gets the pointer to the function unit reaction.
        /// </summary>
        public IntPtr FunctionUnitGetReaction { get; private set; }

        /// <summary>
        /// Gets or sets the function unit pointer for handling right click events.
        /// </summary>
        public IntPtr FunctionUnitOnRightClick { get; private set; }

        /// <summary>
        /// Gets or sets the memory address of the FunctionUnit's facing direction.
        /// </summary>
        public IntPtr FunctionUnitSetFacing { get; private set; }

        /// <summary>
        /// Gets or sets the IntPtr value for the FunctionUnitSetFacingSmooth property.
        /// </summary>
        public IntPtr FunctionUnitSetFacingSmooth { get; private set; }

        /// <summary>
        /// Gets the IntPtr representing the function world frame.
        /// </summary>
        public IntPtr FunctionWorldFrame { get; } = new(0x0);

        /// <summary>
        /// Gets the IntPtr for FunctionWorldRender.
        /// </summary>
        public IntPtr FunctionWorldRender { get; } = new(0x0);

        /// <summary>
        /// Gets the pointer to the function WorldRenderWorld.
        /// </summary>
        public IntPtr FunctionWorldRenderWorld { get; } = new(0x0);

        /// <summary>
        /// Gets or sets the handle to the current game state as an IntPtr.
        /// </summary>
        public IntPtr GameState { get; private set; }

        /// <summary>
        /// Gets or sets the pointer to indicate if the game is in progress.
        /// </summary>
        public IntPtr IsIngame { get; private set; }

        /// <summary>
        /// Gets or sets the pointer to the loaded world.
        /// </summary>
        public IntPtr IsWorldLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the last Target GUID value.
        /// </summary>
        public IntPtr LastTargetGuid { get; private set; }

        /// <summary>
        /// Gets or sets the IntPtr value representing the LootWindowOpen field.
        /// </summary>
        public IntPtr LootWindowOpen { get; private set; }

        /// <summary>
        /// Gets or sets the unique identifier used for mapping.
        /// </summary>
        public IntPtr MapId { get; private set; }

        /// <summary>
        /// Gets or sets the base address of the name.
        /// </summary>
        public IntPtr NameBase { get; } = new(0x18);

        /// <summary>
        /// Gets or sets the name mask of the object. The name mask is represented
        /// by the IntPtr data type and is initialized to 0x24 by default.
        /// </summary>
        public IntPtr NameMask { get; } = new(0x24);

        /// <summary>
        /// Gets or sets the IntPtr value representing the NameStore.
        /// </summary>
        public IntPtr NameStore { get; private set; }

        /// <summary>
        /// Gets the IntPtr object that represents the NameString with a value of 0x21.
        /// </summary>
        public IntPtr NameString { get; } = new(0x21);

        /// <summary>
        /// Gets the next object with a pointer to the memory location specified by 0x34.
        /// </summary>
        public IntPtr NextObject { get; } = new(0x34);

        /// <summary>
        /// Gets or sets the party leader's pointer to memory address.
        /// </summary>
        public IntPtr PartyLeader { get; set; }

        /// <summary>
        /// IntPtr representing the PartyPlayerGuids property.
        /// This property is currently unused.
        /// </summary>
        public IntPtr PartyPlayerGuids { get; } = new(0x0);  // unused

        /// <summary>
        /// Gets or sets the unique identifier for the pet.
        /// </summary>
        public IntPtr PetGuid { get; private set; }

        /// <summary>
        /// Gets or sets the memory address of the player base.
        /// </summary>
        public IntPtr PlayerBase { get; private set; }

        /// <summary>
        /// Gets the GUID of the player.
        /// </summary>
        public IntPtr PlayerGuid { get; private set; }

        /// <summary>
        /// Gets or sets the starting address of the RAID group.
        /// This property is unused.
        /// </summary>
        public IntPtr RaidGroupStart { get; } = new(0x0); // unused

        /// <summary>
        /// The IntPtr property representing the Raid Leader which is currently unused.
        /// </summary>
        public IntPtr RaidLeader { get; } = new(0x0);  // unused

        /// <summary>
        /// Gets the pointer to the rendering flags with the default value of 0x0.
        /// </summary>
        public IntPtr RenderFlags { get; } = new(0x0);

        /// <summary>
        /// Gets the IntPtr representing the Runes property.
        /// </summary>
        public IntPtr Runes { get; } = new(0x0);

        /// <summary>
        /// Gets the IntPtr representing the RuneType.
        /// </summary>
        public IntPtr RuneType { get; } = new(0x0);

        /// <summary>
        /// Gets or sets the target GUID.
        /// </summary>
        public IntPtr TargetGuid { get; private set; }

        /// <summary>
        /// Gets or sets the tick count in milliseconds.
        /// </summary>
        public IntPtr TickCount { get; private set; }

        /// <summary>
        /// Gets the position of the WowDynobject.
        /// </summary>
        public IntPtr WowDynobjectPosition { get; } = new(0x1F4);

        /// <summary>
        /// Gets the memory address of the position of the WoW game object.
        /// </summary>
        public IntPtr WowGameobjectPosition { get; } = new(0x1F4);

        /// <summary>
        /// Gets or sets the pointer to the WowObjectDescriptor.
        /// </summary>
        public IntPtr WowObjectDescriptor { get; } = new(0x4);

        /// <summary>
        /// Gets or sets the object type for the Wow Object.
        /// </summary>
        public IntPtr WowObjectType { get; } = new(0xC);

        /// <summary>
        /// Gets or sets the memory address for the WowPlayerIsSitting property.
        /// </summary>
        public IntPtr WowPlayerIsSitting { get; } = new(0x3BF8);

        /// <summary>
        /// Gets the handle of the WowUnitCanInterrupt property.
        /// </summary>
        public IntPtr WowUnitCanInterrupt { get; } = new(0xC64);

        /// <summary>
        /// Gets the pointer to the WowUnitDbEntry with the value 0x9B4.
        /// </summary>
        public IntPtr WowUnitDbEntry { get; } = new(0x9B4);

        /// <summary>
        /// Gets or sets the pointer to the WowUnitDbEntryName field.
        /// </summary>
        public IntPtr WowUnitDbEntryName { get; } = new(0x6C);

        /// <summary>
        /// Gets or sets the entry type for WowUnitDb, represented as a pointer to an integer.
        /// </summary>
        public IntPtr WowUnitDbEntryType { get; } = new(0x18);

        /// <summary>
        /// Gets the memory address of the WowUnitIsAutoAttacking property.
        /// </summary>
        public IntPtr WowUnitIsAutoAttacking { get; } = new(0x14EC);

        /// <summary>
        /// Gets or sets the memory address of the World of Warcraft unit position.
        /// </summary>
        public IntPtr WowUnitPosition { get; } = new(0x838);

        /// <summary>
        /// Gets or sets the zone identifier.
        /// </summary>
        public IntPtr ZoneId { get; private set; }

        /// <summary>
        /// Gets the subtext zone as a pointer to an IntPtr with the value 0x0.
        /// </summary>
        public IntPtr ZoneSubText { get; } = new(0x0);

        /// <summary>
        /// Gets or sets the zone text.
        /// </summary>
        public IntPtr ZoneText { get; } = new(0x0);

        /// <summary>
        /// Initializes the IntPtr values for various game data offsets based on the mainModuleBase.
        /// </summary>
        /// <param name="mainModuleBase">The base address of the main module.</param>
        public void Init(IntPtr mainModuleBase)
        {
            // need to add base to offsets because ASLR is enabled
            BattlegroundFinished = IntPtr.Add(mainModuleBase, 0xDC3050);
            BattlegroundStatus = IntPtr.Add(mainModuleBase, 0xB6335C);
            CameraPointer = IntPtr.Add(mainModuleBase, 0xD64E5C);

            IntPtr clickToMoveBase = IntPtr.Add(mainModuleBase, 0xD0F390);
            ClickToMoveAction = IntPtr.Add(clickToMoveBase, 0x1C);
            ClickToMoveDistance = IntPtr.Add(clickToMoveBase, 0xC);
            ClickToMoveGuid = IntPtr.Add(clickToMoveBase, 0x20);
            ClickToMoveTurnSpeed = IntPtr.Add(clickToMoveBase, 0x4);
            ClickToMoveX = IntPtr.Add(clickToMoveBase, 0x8C);

            ClientConnection = IntPtr.Add(mainModuleBase, 0xEC4628);
            ComboPoints = IntPtr.Add(mainModuleBase, 0xD65BF9);
            CorpsePosition = IntPtr.Add(mainModuleBase, 0xD65ED8);
            EndSceneStaticDevice = IntPtr.Add(mainModuleBase, 0xBB2FB8);
            FunctionGetActivePlayerObject = IntPtr.Add(mainModuleBase, 0x4F84);
            FunctionGetLocalizedText = IntPtr.Add(mainModuleBase, 0x414267);
            FunctionHandleTerrainClick = IntPtr.Add(mainModuleBase, 0x38F129);
            FunctionIsOutdoors = IntPtr.Add(mainModuleBase, 0x4142AC);
            FunctionLuaDoString = IntPtr.Add(mainModuleBase, 0x4FD12);
            FunctionPlayerClickToMove = IntPtr.Add(mainModuleBase, 0x41FB57);
            FunctionSetTarget = IntPtr.Add(mainModuleBase, 0x8CE510);
            FunctionTraceline = IntPtr.Add(mainModuleBase, 0x5EEF7B);
            FunctionUnitOnRightClick = IntPtr.Add(mainModuleBase, 0x8D0268);
            FunctionUnitSetFacing = IntPtr.Add(mainModuleBase, 0x41ADE7);
            FunctionUnitSetFacingSmooth = IntPtr.Add(mainModuleBase, 0x41A41F);
            FunctionUnitGetReaction = IntPtr.Add(mainModuleBase, 0x4153C3);
            GameState = IntPtr.Add(mainModuleBase, 0xD65B16);
            IsIngame = IntPtr.Add(mainModuleBase, 0xB935C0);
            IsWorldLoaded = IntPtr.Add(mainModuleBase, 0xAE1A18);
            LastTargetGuid = IntPtr.Add(mainModuleBase, 0xD65B48);
            LootWindowOpen = IntPtr.Add(mainModuleBase, 0xDD3D44);
            MapId = IntPtr.Add(mainModuleBase, 0xADF5E8);
            NameStore = IntPtr.Add(mainModuleBase, 0xC86840);
            PetGuid = IntPtr.Add(mainModuleBase, 0xDD4A00);
            PlayerBase = IntPtr.Add(mainModuleBase, 0xCFF49C);
            PlayerGuid = IntPtr.Add(mainModuleBase, 0xC95E60);
            PartyLeader = IntPtr.Add(mainModuleBase, 0xDC28EC);
            TargetGuid = IntPtr.Add(mainModuleBase, 0xD65B40);
            TickCount = IntPtr.Add(mainModuleBase, 0xBB2C74);
            ZoneId = IntPtr.Add(mainModuleBase, 0xB595B4);
            // RealmName = 0xEC480E
        }
    }
}