using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Logic.Enums;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    /// <summary>
    /// Service for NPC interactions (Merchants, Trainers, Questgivers).
    /// Maintains "sticky" targets until interaction is complete.
    /// </summary>
    public class NpcService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;

        // Interaction Cooldowns
        private readonly Dictionary<ulong, DateTime> _interactionCooldowns = [];
        private const int InteractionCooldownSeconds = 10;

        // UI Visibility Throttling (reduce Lua spam)
        private readonly TimegatedEvent _uiCheckEvent = new(TimeSpan.FromMilliseconds(500));
        private bool _cachedGossipVisible;
        private bool _cachedQuestFrameVisible;

        // Target NPC references (sticky until interaction complete)
        public IWowUnit Merchant { get; private set; }
        public IWowUnit ClassTrainer { get; private set; }
        public IWowUnit ProfessionTrainer { get; private set; }
        public IWowUnit QuestGiverToTalkTo { get; private set; }

        // Target position (used when NPC not yet loaded but position known from profile)
        public Vector3 TargetNpcPosition { get; set; }

        /// <summary>
        /// Check if we need to repair or sell items. Found merchant stored in Merchant property.
        /// </summary>
        public bool NeedToRepairOrSell(BotMode mode, bool needsRepair, bool hasItemsToSell)
        {
            if (!needsRepair && !hasItemsToSell)
            {
                Merchant = null;
                return false;
            }

            // Stickiness: Keep using current merchant if valid
            if (Merchant != null)
            {
                IWowUnit currentMerchant = Bot.GetWowObjectByGuid<IWowUnit>(Merchant.Guid);
                if (currentMerchant != null && !currentMerchant.IsDead)
                {
                    bool merchantCanRepair = currentMerchant.IsRepairer;
                    if ((needsRepair && merchantCanRepair) || hasItemsToSell)
                    {
                        Merchant = currentMerchant;
                        TargetNpcPosition = currentMerchant.Position;
                        return true;
                    }
                }
                Merchant = null;
            }

            IWowUnit vendorRepair = null;
            IWowUnit vendorSell = null;
            TargetNpcPosition = Vector3.Zero;

            // No profile in None mode - use local search
            if (mode != BotMode.None && Bot.Grinding.Profile?.NpcsOfInterest == null)
            {
                return false;
            }

            // Find vendors from profile or local search
            switch (mode)
            {
                case BotMode.Grinding:
                    Npc repairEntry = Bot.Grinding.Profile.NpcsOfInterest.FirstOrDefault(e => e.Type == NpcType.VendorRepair);
                    if (repairEntry != null)
                    {
                        vendorRepair = Bot.GetClosestVendorByEntryId(repairEntry.EntryId);
                        if (vendorRepair == null)
                        {
                            TargetNpcPosition = repairEntry.Position;
                        }
                    }

                    Npc sellEntry = Bot.Grinding.Profile.NpcsOfInterest.FirstOrDefault(e => e.Type is NpcType.VendorRepair or NpcType.VendorSellBuy);
                    if (sellEntry != null)
                    {
                        vendorSell = Bot.GetClosestVendorByEntryId(sellEntry.EntryId);
                        if (vendorSell == null && TargetNpcPosition == Vector3.Zero)
                        {
                            TargetNpcPosition = sellEntry.Position;
                        }
                    }
                    break;

                case BotMode.None:
                    IsVendorNear(out vendorRepair, true);
                    IsVendorNear(out vendorSell, false);
                    break;
            }

            if (needsRepair)
            {
                if (vendorRepair != null)
                {
                    Merchant = vendorRepair;
                    TargetNpcPosition = vendorRepair.Position;
                    return true;
                }
                if (TargetNpcPosition != Vector3.Zero)
                {
                    Merchant = null;
                    return true;
                }
            }

            if (hasItemsToSell)
            {
                if (vendorSell != null)
                {
                    Merchant = vendorSell;
                    TargetNpcPosition = vendorSell.Position;
                    return true;
                }
                if (TargetNpcPosition != Vector3.Zero)
                {
                    Merchant = null;
                    return true;
                }
            }

            Merchant = null;
            return false;
        }

        /// <summary>
        /// Check if we need to train class spells.
        /// </summary>
        public bool NeedToTrainSpells(bool needsTrainSpells)
        {
            if (!needsTrainSpells)
            {
                ClassTrainer = null;
                return false;
            }

            // Stickiness
            if (ClassTrainer != null)
            {
                IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(ClassTrainer.Guid);
                if (unit != null && !unit.IsDead)
                {
                    ClassTrainer = unit;
                    TargetNpcPosition = unit.Position;
                    return true;
                }
                ClassTrainer = null;
            }

            IWowUnit classTrainer = null;
            Npc profileTrainer = null;
            TargetNpcPosition = Vector3.Zero;

            if (Bot.Grinding.Profile != null)
            {
                profileTrainer = Bot.Grinding.Profile.NpcsOfInterest?.FirstOrDefault(e =>
                    e.Type == NpcType.ClassTrainer && e.SubType == AmeisenBotLogic.DecideClassTrainer(Bot.Player.Class));
            }

            if (profileTrainer != null)
            {
                classTrainer = Bot.GetClosestTrainerByEntryId(profileTrainer.EntryId);
                if (classTrainer == null)
                {
                    TargetNpcPosition = profileTrainer.Position;
                }
            }

            if (classTrainer == null && TargetNpcPosition == Vector3.Zero)
            {
                return false;
            }

            ClassTrainer = classTrainer;
            TargetNpcPosition = classTrainer != null ? classTrainer.Position : TargetNpcPosition;
            return true;
        }

        /// <summary>
        /// Check if we need to train secondary skills (fishing, cooking, first aid).
        /// </summary>
        public bool NeedToTrainSecondarySkills()
        {
            // Stickiness
            if (ProfessionTrainer != null)
            {
                IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(ProfessionTrainer.Guid);
                if (unit != null && !unit.IsDead)
                {
                    ProfessionTrainer = unit;
                    TargetNpcPosition = unit.Position;
                    return true;
                }
                ProfessionTrainer = null;
            }

            IWowUnit professionTrainer = null;
            Npc profileTrainer = null;
            TargetNpcPosition = Vector3.Zero;

            if (Bot.Grinding.Profile != null)
            {
                profileTrainer = Bot.Grinding.Profile.NpcsOfInterest?.FirstOrDefault(e => e.Type == NpcType.ProfessionTrainer);
            }

            if (profileTrainer != null)
            {
                professionTrainer = profileTrainer.SubType switch
                {
                    NpcSubType.FishingTrainer when !Bot.Character.Skills.ContainsKey("Fishing") => Bot.GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.FirstAidTrainer when !Bot.Character.Skills.ContainsKey("First Aid") => Bot.GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    NpcSubType.CookingTrainer when !Bot.Character.Skills.ContainsKey("Cooking") => Bot.GetClosestTrainerByEntryId(profileTrainer.EntryId),
                    _ => null
                };

                if (professionTrainer == null)
                {
                    TargetNpcPosition = profileTrainer.Position;
                }
            }

            if (professionTrainer == null && TargetNpcPosition == Vector3.Zero)
            {
                return false;
            }

            ProfessionTrainer = professionTrainer;
            TargetNpcPosition = professionTrainer != null ? professionTrainer.Position : TargetNpcPosition;
            return true;
        }

        /// <summary>
        /// Check if we need to talk to a questgiver (auto-quest interaction).
        /// </summary>
        public bool NeedToTalkToQuestgiver()
        {
            // Cleanup old cooldowns occasionally
            if (_interactionCooldowns.Count > 50)
            {
                _interactionCooldowns.Clear();
            }

            // Throttled UI visibility check
            if (_uiCheckEvent.Run())
            {
                _cachedGossipVisible = Bot.Wow.UiIsVisible("GossipFrame");
                _cachedQuestFrameVisible = Bot.Wow.UiIsVisible("QuestFrame");
            }

            // Check if interaction successful (UI Open)
            if (_cachedGossipVisible || _cachedQuestFrameVisible)
            {
                if (QuestGiverToTalkTo != null)
                {
                    _interactionCooldowns[QuestGiverToTalkTo.Guid] = DateTime.Now.AddSeconds(InteractionCooldownSeconds);
                    QuestGiverToTalkTo = null; // Interaction done, stop trying
                }
                return false; // UI is open, we are done here
            }

            // Stickiness
            if (QuestGiverToTalkTo != null)
            {
                IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(QuestGiverToTalkTo.Guid);
                if (unit != null && Bot.Player.DistanceTo(unit) < 15.0f && Bot.Db.GetReaction(Bot.Player, unit) != WowUnitReaction.Hostile)
                {
                    // Check cooldown
                    if (_interactionCooldowns.TryGetValue(unit.Guid, out DateTime cd) && DateTime.Now < cd)
                    {
                        QuestGiverToTalkTo = null;
                        return false;
                    }

                    QuestGiverToTalkTo = unit;
                    return true;
                }
                QuestGiverToTalkTo = null;
            }

            if (Config.AutoTalkToNearQuestgivers && Bot.Objects.Partymembers.Any())
            {
                List<ulong> guids = [];
                if (Bot.Objects.Partyleader != null && Bot.Player.DistanceTo(Bot.Objects.Partyleader) < 40.0f)
                {
                    guids.Add(Bot.Objects.Partyleader.TargetGuid);
                }

                foreach (ulong guid in guids)
                {
                    if (Bot.TryGetWowObjectByGuid(guid, out IWowUnit unit)
                        && Bot.Player.DistanceTo(unit) < 100.0f
                        && Bot.Objects.Partyleader != null && Bot.Objects.Partyleader.DistanceTo(unit) < 15.0f
                        && unit.IsQuestgiver
                        && Bot.Db.GetReaction(Bot.Player, unit) != WowUnitReaction.Hostile)
                    {
                        // Check cooldown
                        if (_interactionCooldowns.TryGetValue(unit.Guid, out DateTime cd) && DateTime.Now < cd)
                        {
                            continue;
                        }

                        QuestGiverToTalkTo = unit;
                        return true;
                    }
                }
            }

            QuestGiverToTalkTo = null;
            return false;
        }

        private bool IsVendorNear(out IWowUnit vendor, bool repairOnly)
        {
            vendor = Bot.Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead
                    && (repairOnly ? e.IsRepairer : e.IsVendor)
                    && Bot.Db.GetReaction(Bot.Player, e) is not WowUnitReaction.Hostile
                    && e.DistanceTo(Bot.Player) < 50.0f)
                .OrderBy(e => e.DistanceTo(Bot.Player))
                .FirstOrDefault();
            return vendor != null;
        }

        public void Reset()
        {
            Merchant = null;
            ClassTrainer = null;
            ProfessionTrainer = null;
            QuestGiverToTalkTo = null;
            TargetNpcPosition = Vector3.Zero;
        }
    }
}
