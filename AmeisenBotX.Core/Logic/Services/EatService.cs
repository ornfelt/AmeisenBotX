using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    /// <summary>
    /// Service for eating/drinking logic.
    /// Manages food inventory and health/mana thresholds.
    /// </summary>
    public class EatService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;

        // Food cache
        private IEnumerable<IWowInventoryItem> Food { get; set; } = [];

        // State
        public bool ShouldEat { get; private set; }

        /// <summary>Reason for current decision - for debugging.</summary>
        public string LastReason { get; private set; } = "None";

        // Throttled events
        private readonly TimegatedEvent EatBlockEvent = new(TimingConfig.EatBlockDuration);
        private readonly TimegatedEvent EatEvent = new(TimingConfig.EatAction);
        private readonly TimegatedEvent UpdateFoodEvent = new(TimingConfig.InventoryUpdateThrottle);

        public void CheckEatState()
        {
            // CRITICAL: Stop eating if party enters combat
            if (Bot.Player.IsInCombat || IsPartyInCombat())
            {
                CancelEatingIfActive();
                ShouldEat = false;
                LastReason = "Combat - eating cancelled";
                return;
            }

            // Respect block cooldown
            if (!EatBlockEvent.Ready)
            {
                LastReason = "Block cooldown active";
                return;
            }

            // Don't try to eat while mounted
            if (Bot.Player.IsMounted)
            {
                ShouldEat = false;
                LastReason = "Mounted";
                return;
            }

            // Abort if party moving away
            if (Config.EatDrinkAbortFollowParty &&
                Bot.Objects.PartymemberGuids.Any() &&
                Bot.Player.DistanceTo(Bot.Objects.CenterPartyPosition) > Config.EatDrinkAbortFollowPartyDistance)
            {
                EatBlockEvent.Run();
                ShouldEat = false;
                LastReason = "Party moving away";
                return;
            }

            // Check if actively eating/drinking
            bool activeEating = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Food");
            bool activeDrinking = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Drink");

            // Continue if not yet at target
            if ((activeEating && Bot.Player.HealthPercentage < Config.EatUntilPercent) ||
                (activeDrinking && Bot.Player.MaxMana > 0 && Bot.Player.ManaPercentage < Config.DrinkUntilPercent))
            {
                ShouldEat = true;
                LastReason = $"In progress: HP={Bot.Player.HealthPercentage:F0}% MP={Bot.Player.ManaPercentage:F0}%";
                return;
            }

            // Update food cache periodically
            if (UpdateFoodEvent.Run())
            {
                Food = Bot.Character.Inventory.Items
                    .Where(e => e.RequiredLevel <= Bot.Player.Level)
                    .OrderByDescending(e => e.ItemLevel);
            }

            // Check if we need to start eating/drinking
            bool needsFood = Bot.Player.HealthPercentage < Config.EatStartPercent &&
                (Food.Any(e => Enum.IsDefined(typeof(WowFood), e.Id)) ||
                 Food.Any(e => Enum.IsDefined(typeof(WowRefreshment), e.Id)));

            bool needsDrink = Bot.Player.MaxMana > 0 &&
                Bot.Player.ManaPercentage < Config.DrinkStartPercent &&
                (Food.Any(e => Enum.IsDefined(typeof(WowWater), e.Id)) ||
                 Food.Any(e => Enum.IsDefined(typeof(WowRefreshment), e.Id)));

            ShouldEat = needsFood || needsDrink;

            // Track reason
            LastReason = needsFood && needsDrink
                ? "Need food and drink"
                : needsFood
                    ? $"Low HP ({Bot.Player.HealthPercentage:F0}%)"
                    : needsDrink ? $"Low MP ({Bot.Player.ManaPercentage:F0}%)" : "No need";
        }

        /// <summary>
        /// Execute the eating behavior (use item). Returns BtStatus.
        /// </summary>
        public BtStatus ExecuteEat()
        {
            if (Bot.Player.IsInCombat)
            {
                return BtStatus.Failed;
            }

            if (Bot.Player.IsCasting)
            {
                return BtStatus.Ongoing;
            }

            // If already eating/drinking, checks in CheckEatState will keep ShouldEat true until full.
            // We just need to wait here if buffs are present.
            bool activeEating = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Food");
            bool activeDrinking = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Drink");

            if (activeEating || activeDrinking)
            {
                return BtStatus.Success; // Successfully "eating"
            }

            IWowInventoryItem bestFood = Food?.FirstOrDefault();
            if (bestFood != null)
            {
                if (EatEvent.Run())
                {
                    Bot.Wow.UseItemByName(bestFood.Name);
                    return BtStatus.Success;
                }
                return BtStatus.Ongoing;
            }
            return BtStatus.Failed;
        }

        private bool IsPartyInCombat()
        {
            return Bot.Objects.Partymembers.Any(e => e.IsInCombat && e.DistanceTo(Bot.Player) < Config.SupportRange);
        }

        private void CancelEatingIfActive()
        {
            bool isEating = Bot.Player.Auras.Any(a => Bot.Db.GetSpellName(a.SpellId) == "Food");
            bool isDrinking = Bot.Player.Auras.Any(a => Bot.Db.GetSpellName(a.SpellId) == "Drink");

            if (isEating || isDrinking)
            {
                Bot.Wow.LuaDoString("CancelUnitBuff('player', 'Food'); CancelUnitBuff('player', 'Drink');");
                EatBlockEvent.Run(); // Reset cooldown
            }
        }

        public void Reset()
        {
            ShouldEat = false;
            Food = [];
        }
    }
}
