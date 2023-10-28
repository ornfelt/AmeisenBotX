using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Events;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Constants;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;

/// <summary>
/// Interface to the wow game. All functions that interact with the game should be reachable via
/// this interface.
/// </summary>
namespace AmeisenBotX.Wow
{
    /// <summary>
    /// Interface to the wow game. All functions that interact with the game should be reachable via
    /// this interface.
    /// </summary>
    public interface IWowInterface
    {
        /// <summary>
        /// Gets fired when the battleground state changes.
        /// </summary>
        event Action<string> OnBattlegroundStatus;

        /// <summary>
        /// Gets fired when a new static popup appears ingame.
        /// Format: {ID}:{POPUPTYPE};... 1:DELETE_ITEM;2:SAMPLE_POPUP;...
        /// </summary>
        event Action<string> OnStaticPopup;

        /// <summary>
        /// Use this to interact with the wow event system.
        /// </summary>
        IEventManager Events { get; }

        /// <summary>
        /// Used for the HookCall display in the bots main window. Use this to display cost
        /// intensive calls to the user. The name Hookcall originates from EndScene hook calls.
        /// </summary>
        int HookCallCount { get; }

        /// <summary>
        /// Get the status of the wow interface, true if its useable, false if not.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Shortcut to get the last targets guid.
        /// </summary>
        public ulong LastTargetGuid => ObjectProvider.LastTarget != null ? ObjectProvider.LastTarget.Guid : 0ul;

        /// <summary>
        /// Use this to interact with the wows memory.
        /// </summary>
        WowMemoryApi Memory { get; }

        /// <summary>
        /// Use this to interact with wowobjects, units, players and more.
        /// </summary>
        IObjectProvider ObjectProvider { get; }

        /// <summary>
        /// Shortcut to all wow objects.
        /// </summary>
        IEnumerable<IWowObject> Objects => ObjectProvider.All;

        /// <summary>
        /// Shortcut to get the current partyleaders guid.
        /// </summary>
        ulong PartyleaderGuid => ObjectProvider.Partyleader != null ? ObjectProvider.Partyleader.Guid : 0ul;

        /// <summary>
        /// Shortcut to get the current pets guid.
        /// </summary>
        public ulong PetGuid => ObjectProvider.Pet != null ? ObjectProvider.Pet.Guid : 0ul;

        /// <summary>
        /// Shortcut to get the current players guid.
        /// </summary>
        public ulong PlayerGuid => ObjectProvider.Player != null ? ObjectProvider.Player.Guid : 0ul;

        /// <summary>
        /// Shortcut to get the current targets guid.
        /// </summary>
        public ulong TargetGuid => ObjectProvider.Target != null ? ObjectProvider.Target.Guid : 0ul;

        /// <summary>
        /// Get the current version of wow.
        /// </summary>
        WowVersion WowVersion { get; }

        /// <summary>
        /// Abandons quests that are not present in the given enumerable.
        /// </summary>
        void AbandonQuestsNotIn(IEnumerable<string> enumerable);

        /// <summary>
        /// Accepts a battleground invitation.
        /// </summary>
        void AcceptBattlegroundInvite();

        /// <summary>
        /// Accepts a party invitation.
        /// </summary>
        void AcceptPartyInvite();

        /// <summary>
        /// Accepts a quest.
        /// </summary>
        void AcceptQuest();

        /// <summary>
        /// This method is used to accept quests.
        /// </summary>
        void AcceptQuests();

        /// <summary>
        /// Accepts resurrection for the current operation.
        /// </summary>
        void AcceptResurrect();

        /// <summary>
        /// Accepts a summon.
        /// </summary>
        void AcceptSummon();

        /// <summary>
        /// Calls the companion of the specified index and type.
        /// </summary>
        void CallCompanion(int index, string type);

        /// <summary>
        /// Cast a spell using the lua CastSpell function.
        /// </summary>
        /// <param name="spellName">Name of the spell to cast</param>
        /// <param name="castOnSelf">True if we should cast it on our own character</param>
        void CastSpell(string spellName, bool castOnSelf = false);

        /// <summary>
        /// Casts a spell by its unique identifier.
        /// </summary>
        void CastSpellById(int spellId);

        /// <summary>
        /// Changes the target of the specified GUID.
        /// </summary>
        void ChangeTarget(ulong guid);

        /// <summary>
        /// Clears the target.
        /// </summary>
        void ClearTarget();

        /// <summary>
        /// Triggers a click event on the terrain at the specified position.
        /// </summary>
        void ClickOnTerrain(Vector3 position);

        /// <summary>
        /// Clicks on the train button.
        /// </summary>
        void ClickOnTrainButton();

        /// <summary>
        /// Moves the object to the specified position using click-to-move mechanics.
        /// </summary>
        /// <param name="pos">The position to move the object to.</param>
        /// <param name="guid">The unique identifier of the object.</param>
        /// <param name="clickToMoveType">The type of click-to-move to perform (default value is Move).</param>
        /// <param name="turnSpeed">The speed at which the object turns (default value is 20.9f).</param>
        /// <param name="distance">The distance at which the object moves (default value is Move).</param>
        void ClickToMove(Vector3 pos, ulong guid, WowClickToMoveType clickToMoveType = WowClickToMoveType.Move, float turnSpeed = 20.9f, float distance = WowClickToMoveDistance.Move);

        /// <summary>
        /// Performs a click on the given ui element.
        /// </summary>
        /// <param name="elementName">UI element name, find it using ingame command "/fstack"</param>
        void ClickUiElement(string elementName);

        /// <summary>
        /// Confirms the loot roll.
        /// </summary>
        void CofirmLootRoll();

        /// <summary>
        /// Confirms if a check is ready or not.
        /// </summary>
        /// <param name="isReady">A boolean indicating if the check is ready.</param>
        void CofirmReadyCheck(bool isReady);

        /// <summary>
        /// Displays a static pop-up for user confirmation.
        /// </summary>
        void CofirmStaticPopup();

        /// <summary>
        /// Marks a quest as complete.
        /// </summary>
        void CompleteQuest();

        /// <summary>

        /// Deletes an item from the system based on its name.

        /// </summary>
        void DeleteItemByName(string name);

        /// <summary>
        /// This method dismisses the companion of the specified type.
        /// </summary>
        void DismissCompanion(string type);

        /// <summary>
        /// Dispose the wow interface making it realese and unhook all resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Equips an item with the specified itemName to the given slot. If slot is not provided, the item will be equipped to the default slot (-1).
        /// </summary>
        void EquipItem(string itemName, int slot = -1);

        /// <summary>
        /// Executes a Lua command with the provided command and variable combo and returns the result.
        /// </summary>
        /// <param name="commandVariableCombo">The command and variable combo to execute.</param>
        /// <param name="result">The resulting string returned from the execution.</param>
        /// <returns>Returns true if the execution is successful; otherwise, false.</returns>
        bool ExecuteLuaAndRead((string, string) commandVariableCombo, out string result);

        /// <summary>
        /// Updates the face position of a player using the provided player base, player position, and target position vectors.
        /// Optionally applies smoothing to the face movement if smooth flag is set to true.
        /// </summary>
        /// <param name="playerBase">The memory address of the player base.</param>
        /// <param name="playerPosition">The current position of the player.</param>
        /// <param name="position">The target position for the player's face.</param>
        /// <param name="smooth">Indicates whether to apply smoothing to the face movement. (default: false)</param>
        void FacePosition(IntPtr playerBase, Vector3 playerPosition, Vector3 position, bool smooth = false);

        /// <summary>
        /// Retrieves all completed quests as an IEnumerable of integers.
        /// </summary>
        IEnumerable<int> GetCompletedQuests();

        /// <summary>
        /// Retrieves the equipment items as a string.
        /// </summary>
        string GetEquipmentItems();

        /// <summary>
        /// Returns the number of unused bag slots.
        /// </summary>
        /// <returns>Free bag slot count</returns>
        int GetFreeBagSlotCount();

        /// <summary>
        /// Retrieves an array of gossip types.
        /// </summary>
        string[] GetGossipTypes();

        /// <summary>
        /// Retrieves the inventory items as a string.
        /// </summary>
        string GetInventoryItems();

        /// <summary>
        /// Retrieves an item by its name or link.
        /// </summary>
        /// <param name="itemLink">The name or link of the item to retrieve.</param>
        /// <returns>The item associated with the provided name or link.</returns>
        string GetItemByNameOrLink(string itemLink);

        /// <summary>
        /// Retrieves the loot roll item link associated with the specified roll ID.
        /// </summary>
        string GetLootRollItemLink(int rollId);

        /// <summary>
        /// Returns the current money in copper.
        /// </summary>
        /// <returns>Money in copper</returns>
        int GetMoney();

        /// <summary>
        /// Retrieves all the WowMount objects available.
        /// </summary>
        IEnumerable<WowMount> GetMounts();

        /// <summary>
        /// Retrieves the number of choices available in the quest log.
        /// </summary>
        bool GetNumQuestLogChoices(out int numChoices);

        /// <summary>
        /// Retrieves the item link associated with the choice item in the quest log at the given index.
        /// </summary>
        /// <param name="i">The index of the choice item in the quest log.</param>
        /// <param name="itemLink">The retrieved item link.</param>
        /// <returns>True if the item link was successfully retrieved; otherwise, false.</returns>
        bool GetQuestLogChoiceItemLink(int i, out string itemLink);

        /// <summary>
        /// Retrieves the quest log ID associated with the specified quest title.
        /// </summary>
        /// <param name="name">The title of the quest.</param>
        /// <param name="questLogId">The quest log ID to be returned if found.</param>
        /// <returns>True if the quest log ID is successfully obtained, otherwise false.</returns>
        bool GetQuestLogIdByTitle(string name, out int questLogId);

        /// <summary>
        /// Retrieves the reaction between two WowObjects based on their IntPtr values.
        /// </summary>
        WowUnitReaction GetReaction(IntPtr a, IntPtr b);

        /// <summary>
        /// Returns a dictionary containing the number of runes ready for each corresponding key.
        /// </summary>
        Dictionary<int, int> GetRunesReady();

        /// <summary>
        /// Retrieves a dictionary of skills where the key is a string and the value is a tuple of two integers.
        /// </summary>
        Dictionary<string, (int, int)> GetSkills();

        /// <summary>
        /// Retrieves the cooldown of a specific spell by its name.
        /// </summary>
        int GetSpellCooldown(string spellName);

        /// <summary>
        /// Returns the name of a spell based on its unique identifier.
        /// </summary>
        /// <param name="spellId">The unique identifier of the spell.</param>
        /// <returns>The name of the spell.</returns>
        string GetSpellNameById(int spellId);

        /// <summary>
        /// Retrieves a list of spells.
        /// </summary>
        string GetSpells();

        /// <summary>
        /// Retrieves the talents as a string.
        /// </summary>
        string GetTalents();

        ///<summary>
        /// Retrieves the casting information of a unit, specified by the provided target.
        ///</summary>
        ///<param name="target">The unit whose casting information needs to be retrieved.</param>
        ///<returns>
        /// A tuple containing the casting information, including the name and duration of the cast, as a string,
        /// and the unit's identifier (ID) as an integer.
        ///</returns>
        (string, int) GetUnitCastingInfo(WowLuaUnit target);

        ///<summary>Returns the number of unspent talent points.</summary>
        int GetUnspentTalentPoints();

        /// <summary>
        /// This method allows the user to interact with the specified World of Warcraft object.
        /// </summary>
        void InteractWithObject(IWowObject obj);

        /// <summary>
        /// Interacts with a World of Warcraft unit.
        /// </summary>
        /// <param name="unit">The unit to interact with.</param>
        void InteractWithUnit(IWowUnit unit);

        /// <summary>
        /// Gets the state of autoloot.
        /// </summary>
        /// <returns>True if it is enabled, false if not</returns>
        bool IsAutoLootEnabled();

        /// <summary>
        /// Returns a boolean value indicating if the click-to-move feature is active.
        /// </summary>
        bool IsClickToMoveActive();

        /// <summary>
        /// Checks if the current user is in a looking-for-group (LFG) group.
        /// </summary>
        bool IsInLfgGroup();

        /// <summary>
        /// Determines if there is a direct line of sight between point A and point B, taking into account a height adjustment.
        /// </summary>
        /// <param name="a">The position of point A.</param>
        /// <param name="b">The position of point B.</param>
        /// <param name="heightAdjust">An optional height adjustment value. Default is 1.5f.</param>
        /// <returns>True if there is a direct line of sight between the points, otherwise false.</returns>
        bool IsInLineOfSight(Vector3 a, Vector3 b, float heightAdjust = 1.5f);

        ///<summary>
        /// Checks if the rune with the specified ID is ready.
        ///</summary>
        bool IsRuneReady(int id);

        /// <summary>
        /// Leaves the current battleground and removes the player from the current session.
        /// </summary>
        void LeaveBattleground();

        /// <summary>
        /// This method loots everything available.
        /// </summary>
        void LootEverything();

        /// <summary>
        /// This method loots the money and quest items from the player's inventory.
        /// </summary>
        void LootMoneyAndQuestItems();

        /// <summary>
        /// Run lua code in wow using the LuaDosString() function
        /// </summary>
        /// <param name="lua">Code to run</param>
        /// <returns>Whether the code was executed or not</returns>
        bool LuaDoString(string lua);

        /// <summary>
        /// Adds a battleground to the Lua queue by its name.
        /// </summary>
        void LuaQueueBattlegroundByName(string bgName);

        /// <summary>
        /// This method is used to query the number of quests completed.
        /// </summary>
        void QueryQuestsCompleted();

        /// <summary>
        /// Repairs all items.
        /// </summary>
        void RepairAllItems();

        /// <summary>
        /// Repopulates the entity.
        /// </summary>
        void RepopMe();

        /// <summary>
        /// Retrieves the corpse of the deceased.
        /// </summary>
        void RetrieveCorpse();

        /// <summary>
        /// Method for rolling on loot in World of Warcraft.
        /// </summary>
        /// <param name="rollId">The ID of the roll.</param>
        /// <param name="need">The type of roll needed (e.g., need, greed, pass).</param>
        void RollOnLoot(int rollId, WowRollType need);

        /// <summary>
        /// Selects the active quest associated with the provided gossip identifier.
        /// </summary>
        void SelectGossipActiveQuest(int gossipId);

        /// <summary>
        /// Selects and displays the available quest associated with the given gossip ID.
        /// </summary>
        void SelectGossipAvailableQuest(int gossipId);

        /// <summary>
        /// Selects the gossip option with the specified index.
        /// </summary>
        void SelectGossipOption(int i);

        /// <summary>
        /// Selects a simple gossip option based on the provided index.
        /// </summary>
        void SelectGossipOptionSimple(int i);

        /// <summary>
        /// Selects a quest by its name or gossip Id and checks if it is available.
        /// </summary>
        /// <param name="name">The name of the quest.</param>
        /// <param name="gossipId">The gossip Id associated with the quest.</param>
        /// <param name="isAvailable">True if the quest is currently available; otherwise, false.</param>
        void SelectQuestByNameOrGossipId(string name, int gossipId, bool isAvailable);

        /// <summary>
        /// Selects a quest log entry based on the given quest log ID.
        /// </summary>
        void SelectQuestLogEntry(int questLogId);

        /// <summary>
        /// Selects the quest reward with the specified index.
        /// </summary>
        void SelectQuestReward(int i);

        /// <summary>
        /// Sends a chat message.
        /// </summary>
        /// <param name="msg">The message to be sent.</param>
        void SendChatMessage(string msg);

        /// <summary>
        /// Sets the facing angle of the player given by the provided player base pointer.
        /// </summary>
        /// <param name="playerBase">The base address of the player in memory.</param>
        /// <param name="angle">The new facing angle of the player.</param>
        /// <param name="smooth">Determines if the transition of the facing angle should be smooth or instant. By default, it is set to false.</param>
        void SetFacing(IntPtr playerBase, float angle, bool smooth = false);

        /// <summary>
        /// Sets the Looking for Group (LFG) role for the player.
        /// </summary>
        /// <param name="wowRole">The WoW role to set as the LFG role.</param>
        void SetLfgRole(WowRole wowRole);

        /// <summary>
        /// Sets the render state to the specified value.
        /// </summary>
        void SetRenderState(bool state);

        /// <summary>
        /// Init the wow interface.
        /// </summary>
        /// <returns>True if everything went well, false if not</returns>
        bool Setup();

        /// <summary>
        /// Use this to diable the is world loaded check that is used to prevent the execution of
        /// assembly code during loading screens. Used to disable the check in the login process as
        /// the world is not loaded in the main menu.
        /// </summary>
        /// <param name="enabled">Status of the check (true = on | false = off)</param>
        void SetWorldLoadedCheck(bool enabled);

        ///<summary>
        /// Starts the auto attack.
        ///</summary>
        void StartAutoAttack();

        /// <summary>
        /// Stops the casting process.
        /// </summary>
        void StopCasting();

        /// <summary>
        /// Stops the click-to-move functionality.
        /// </summary>
        void StopClickToMove();

        /// <summary>
        /// Poll this on a regular basis to keep the stuff up to date. Updates objects, gameinfo and more.
        /// </summary>
        void Tick();

        /// <summary>
        /// Checks if the UI elements with the specified names are visible.
        /// </summary>
        /// <param name="elementNames">The names of the UI elements to check.</param>
        /// <returns>True if all the UI elements are visible, otherwise false.</returns>
        bool UiIsVisible(params string[] elementNames);

        /// <summary>
        /// Uses an item from a specified bag and slot.
        /// </summary>
        /// <param name="bagId">The ID of the bag where the item is located.</param>
        /// <param name="bagSlot">The slot index of the item within the bag.</param>
        void UseContainerItem(int bagId, int bagSlot);

        /// <summary>
        /// Uses the inventory item equipped in the specified equipment slot.
        /// </summary>
        void UseInventoryItem(WowEquipmentSlot equipmentSlot);

        /// <summary>
        /// Uses an item with the given name.
        /// </summary>
        void UseItemByName(string name);
    }
}