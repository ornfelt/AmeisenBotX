﻿using System;

namespace AmeisenBotX.Core.OffsetLists
{
    public class OffsetList335a : IOffsetList
    {
        public IntPtr AccountName { get; } = new IntPtr(0xB6AA40);

        public IntPtr CharacterSlotSelected { get; } = new IntPtr(0xAC436C);

        public IntPtr ChatOpened { get; } = new IntPtr(0xD41660);

        public IntPtr ClickToMoveAction { get; } = new IntPtr(0xCA11D8 + 0x1C);

        public IntPtr ClickToMoveDistance { get; } = new IntPtr(0xCA11D8 + 0xC);

        public IntPtr ClickToMoveEnabled { get; } = new IntPtr(0x30);

        public IntPtr ClickToMoveGuid { get; } = new IntPtr(0xCA11D8 + 0x20);

        public IntPtr ClickToMovePointer { get; } = new IntPtr(0xBD08F4);

        public IntPtr ClickToMoveX { get; } = new IntPtr(0xCA11D8 + 0x8C);

        public IntPtr ClickToMoveY { get; } = new IntPtr(0xCA11D8 + 0x90);

        public IntPtr ClickToMoveZ { get; } = new IntPtr(0xCA11D8 + 0x94);

        public IntPtr ClientConnection { get; } = new IntPtr(0xC79CE0);

        public IntPtr ComboPoints { get; } = new IntPtr(0xBD0845);

        public IntPtr ContinentName { get; } = new IntPtr(0xCE06D0);

        public IntPtr CorpsePosition { get; } = new IntPtr(0xBD0A58);

        public IntPtr CurrentlyCastingSpellId { get; } = new IntPtr(0xA6C);

        public IntPtr CurrentlyChannelingSpellId { get; } = new IntPtr(0xA80);

        public IntPtr CurrentObjectManager { get; } = new IntPtr(0x2ED0);

        public IntPtr DescriptorEnergy { get; } = new IntPtr(0x6C);

        public IntPtr DescriptorExp { get; } = new IntPtr(0x1E3C);

        public IntPtr DescriptorFactionTemplate { get; } = new IntPtr(0xDC);

        public IntPtr DescriptorHealth { get; } = new IntPtr(0x60);

        public IntPtr DescriptorLevel { get; } = new IntPtr(0xD8);

        public IntPtr DescriptorMana { get; } = new IntPtr(0x64);

        public IntPtr DescriptorMaxEnergy { get; } = new IntPtr(0x8C);

        public IntPtr DescriptorMaxExp { get; } = new IntPtr(0x1E40);

        public IntPtr DescriptorMaxHealth { get; } = new IntPtr(0x80);

        public IntPtr DescriptorMaxMana { get; } = new IntPtr(0x84);

        public IntPtr DescriptorMaxRage { get; } = new IntPtr(0x88);

        public IntPtr DescriptorMaxRuneenergy { get; } = new IntPtr(0x88);

        public IntPtr DescriptorRage { get; } = new IntPtr(0x68);

        public IntPtr DescriptorRuneenergy { get; } = new IntPtr(0x68);

        public IntPtr DescriptorTargetGuid { get; } = new IntPtr(0x48);

        public IntPtr DescriptorUnitFlags { get; } = new IntPtr(0xEC);

        public IntPtr DescriptorUnitFlagsDynamic { get; } = new IntPtr(0x240);

        public IntPtr DescriptorInfoFlags { get; } = new IntPtr(0x5C);

        public IntPtr EndSceneOffset { get; } = new IntPtr(0xA8);

        public IntPtr EndSceneOffsetDevice { get; } = new IntPtr(0x397C);

        public IntPtr EndSceneStaticDevice { get; } = new IntPtr(0xC5DF88);

        public IntPtr ErrorMessage { get; } = new IntPtr(0xBCFB90);

        public IntPtr FirstObject { get; } = new IntPtr(0xAC);

        public IntPtr FunctionCastSpellById { get; } = new IntPtr(0x80DA40);

        public IntPtr FunctionGetActivePlayerObject { get; } = new IntPtr(0x4038F0);

        public IntPtr FunctionGetLocalizedText { get; } = new IntPtr(0x7225E0);

        public IntPtr FunctionGetUnitReaction { get; } = new IntPtr(0x7251C0);

        public IntPtr FunctionLuaDoString { get; } = new IntPtr(0x819210);

        public IntPtr FunctionSendMovementPacket { get; } = new IntPtr(0x7413F0);

        public IntPtr FunctionSetTarget { get; } = new IntPtr(0x524BF0);

        public IntPtr GameState { get; } = new IntPtr(0xB6A9E0);

        public IntPtr IsAutoAttacking { get; } = new IntPtr(0xA20);

        public IntPtr IsWorldLoaded { get; } = new IntPtr(0xBEBA40);

        public IntPtr LastTargetGuid { get; } = new IntPtr(0xBD07B8);

        public IntPtr MapId { get; } = new IntPtr(0xAB63BC);

        public IntPtr NameBase { get; } = new IntPtr(0x1C);

        public IntPtr NameMask { get; } = new IntPtr(0x24);

        public IntPtr NameStore { get; } = new IntPtr(0xC5D940);

        public IntPtr NameString { get; } = new IntPtr(0x20);

        public IntPtr NextObject { get; } = new IntPtr(0x3C);

        public IntPtr PartyLeader { get; } = new IntPtr(0xBD1968);

        public IntPtr PartyPlayer1 { get; } = new IntPtr(0xBD1948);

        public IntPtr PartyPlayer2 { get; } = new IntPtr(0xBD1950);

        public IntPtr PartyPlayer3 { get; } = new IntPtr(0xBD1958);

        public IntPtr PartyPlayer4 { get; } = new IntPtr(0xBD1960);

        public IntPtr PetGuid { get; } = new IntPtr(0xC234D0);

        public IntPtr PlayerBase { get; } = new IntPtr(0xD38AE4);

        public IntPtr PlayerGuid { get; } = new IntPtr(0xCA1238);

        public IntPtr PlayerName { get; } = new IntPtr(0xC79D18);

        public IntPtr PlayerRotation { get; } = new IntPtr(0x7A8);

        public IntPtr RaidGroupPlayer { get; } = new IntPtr(0x50);

        public IntPtr RaidGroupStart { get; } = new IntPtr(0xBF8258);

        public IntPtr RaidLeader { get; } = new IntPtr(0xBD1990);

        public IntPtr RealmName { get; } = new IntPtr(0xC79B9E);

        public IntPtr Runes { get; } = new IntPtr(0x824388);

        public IntPtr RuneType { get; } = new IntPtr(0x824304);

        public IntPtr RuneCooldown { get; } = new IntPtr(0x824364);

        public IntPtr TargetGuid { get; } = new IntPtr(0xBD07B0);

        public IntPtr TickCount { get; } = new IntPtr(0xB499A4);

        public IntPtr WowBuild { get; } = new IntPtr(0xA30BE6);

        public IntPtr WowDynobjectCasterGuid { get; } = new IntPtr(0x18);

        public IntPtr WowDynobjectFacing { get; } = new IntPtr(0x38);

        public IntPtr WowDynobjectPosition { get; } = new IntPtr(0x110);

        public IntPtr WowDynobjectRadius { get; } = new IntPtr(0x28);

        public IntPtr WowDynobjectSpellId { get; } = new IntPtr(0x24);

        public IntPtr WowGameobjectDisplayId { get; } = new IntPtr(0x20);

        public IntPtr WowGameobjectLevel { get; } = new IntPtr(0x58);

        public IntPtr WowGameobjectPosition { get; } = new IntPtr(0x1D8);

        public IntPtr WowGameobjectType { get; } = new IntPtr(0x54);

        public IntPtr WowObjectDescriptor { get; } = new IntPtr(0x8);

        public IntPtr WowObjectGuid { get; } = new IntPtr(0x30);

        public IntPtr WowObjectPosition { get; } = new IntPtr(0xE8);

        public IntPtr WowObjectType { get; } = new IntPtr(0x14);

        public IntPtr WowUnitPosition { get; } = new IntPtr(0x798);

        public IntPtr ZoneId { get; } = new IntPtr(0xAF4E48);
    }
}