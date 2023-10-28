using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Cache.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Flags;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a namespace that contains classes for implementing idle actions in the AmeisenBotX.Core.Logic.Idle.Actions namespace.
/// </summary>
namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Represents a class that implements the idle action for fishing.
    /// </summary>
    public class FishingIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the FishingIdleAction class.
        /// </summary>
        /// <param name="bot">The bot object.</param>
        public FishingIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the system is set to autopilot only mode.
        /// </summary>
        public bool AutopilotOnly => true;

        /// <summary>
        /// Gets or sets the interface for controlling the AmeisenBot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the start time of cooldown.
        /// </summary>
        public DateTime CooldownStart { get; set; }

        /// <summary>
        /// Gets or sets the current spot of the Vector3 object.
        /// </summary>
        public Vector3 CurrentSpot { get; set; }

        /// <summary>
        /// Gets or sets the duration of the specified TimeSpan.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets the maximum cooldown in milliseconds.
        /// </summary>
        public int MaxCooldown => 12 * 60 * 1000;

        /// <summary>
        /// Gets the maximum duration in milliseconds.
        /// </summary>
        public int MaxDuration { get; } = 20 * 60 * 1000;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 7 * 60 * 1000;

        /// <summary>
        /// Gets the minimum duration in milliseconds.
        /// </summary>
        public int MinDuration { get; } = 10 * 60 * 1000;

        /// <summary>
        /// Gets or sets the duration of the spot.
        /// </summary>
        public TimeSpan SpotDuration { get; set; }

        /// <summary>
        /// Gets or sets the date and time when a spot is selected.
        /// </summary>
        public DateTime SpotSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the process has started.
        /// </summary>
        public bool Started { get; set; }

        /// <summary>
        /// Gets the instance of the random number generator.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Checks if the player can enter the fishing mode by evaluating several conditions.
        /// Conditions include verifying if the player is swimming or flying, if they possess the fishing skill,
        /// if they have a fishing pole in their inventory or equipped, and if there are any fishing spots nearby.
        /// If all conditions are met, the function adds the mainhand and offhand item slots to skip and returns true.
        /// </summary>
        public bool Enter()
        {
            bool fishingPoleEquipped = IsFishingRodEquipped();
            bool status =  // we cant fish while swimming
                    !Bot.Player.IsSwimming && !Bot.Player.IsFlying
                    // do i have the fishing skill
                    && Bot.Character.Skills.Any(e => e.Key.Contains("fishing", StringComparison.OrdinalIgnoreCase))
                    // do i have a fishing pole in my inventory or equipped
                    && (Bot.Character.Inventory.Items.OfType<WowWeapon>().Any(e => e.WeaponType == WowWeaponType.FishingPole)
                        || IsFishingRodEquipped())
                    // do i know any fishing spot around here
                    && Bot.Db.TryGetPointsOfInterest(Bot.Objects.MapId, PoiType.FishingSpot, Bot.Player.Position, 256.0f, out IEnumerable<Vector3> pois);

            if (status)
            {
                Bot.Character.ItemSlotsToSkip.Add(WowEquipmentSlot.INVSLOT_MAINHAND);
                Bot.Character.ItemSlotsToSkip.Add(WowEquipmentSlot.INVSLOT_OFFHAND);
            }

            return status;
        }

        /// <summary>
        /// Executes the fishing action if conditions are met.
        /// </summary>
        public void Execute()
        {
            if ((CurrentSpot == default || SpotSelected + SpotDuration <= DateTime.UtcNow)
                && !Bot.Player.IsCasting
                && Bot.Db.TryGetPointsOfInterest(Bot.Objects.MapId, PoiType.FishingSpot, Bot.Player.Position, 256.0f, out IEnumerable<Vector3> pois))
            {
                CurrentSpot = pois.ElementAt(Rnd.Next(0, pois.Count() - 1));
                SpotSelected = DateTime.UtcNow;
                SpotDuration = TimeSpan.FromSeconds(new Random().Next(MinDuration, MaxDuration));
            }

            if (CurrentSpot != default)
            {
                if (Bot.Player.Position.GetDistance(CurrentSpot) > 3.5f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, CurrentSpot);
                    return;
                }
                else if (Bot.Wow.IsClickToMoveActive())
                {
                    Bot.Movement.StopMovement();
                    return;
                }

                if (!BotMath.IsFacing(Bot.Player.Position, Bot.Player.Rotation, CurrentSpot))
                {
                    Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, CurrentSpot, true);
                    return;
                }
            }

            if (!IsFishingRodEquipped())
            {
                IWowInventoryItem fishingRod = Bot.Character.Inventory.Items.OfType<WowWeapon>()
                    .FirstOrDefault(e => e.WeaponType == WowWeaponType.FishingPole);

                if (fishingRod != null)
                {
                    Bot.Wow.EquipItem(fishingRod.Name);
                }
            }

            IWowGameobject fishingBobber = Bot.Objects.All.OfType<IWowGameobject>()
                .FirstOrDefault(e => e.GameObjectType == WowGameObjectType.FishingBobber && e.CreatedBy == Bot.Wow.PlayerGuid);

            if (!Started)
            {
                Started = true;
                CooldownStart = DateTime.UtcNow;
                Duration = TimeSpan.FromSeconds(Rnd.Next(MinDuration, MaxDuration));
            }
            else if (CooldownStart + Duration <= DateTime.UtcNow)
            {
                Started = false;
                CooldownStart = default;
                Duration = default;
                CurrentSpot = default;
                return;
            }

            if (!Bot.Player.IsCasting || fishingBobber == null)
            {
                Bot.Wow.CastSpell("Fishing");
            }
            else if (fishingBobber.Flags[(int)WowGameObjectFlag.NoDespawn])
            {
                Bot.Wow.InteractWithObject(fishingBobber);
                Bot.Wow.LootEverything();
            }
        }

        /// <summary>
        /// Overrides the ToString method and returns a string representation of the object.
        /// The returned string includes the text "Go Fishing" and an optional emoji of a robot if the AutopilotOnly property is true.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Go Fishing";
        }

        /// <summary>
        /// Checks if a fishing rod is currently equipped by the character.
        /// </summary>
        private bool IsFishingRodEquipped()
        {
            return (Bot.Character.Equipment.Items[WowEquipmentSlot.INVSLOT_MAINHAND] != null
                && ((WowWeapon)Bot.Character.Equipment.Items[WowEquipmentSlot.INVSLOT_MAINHAND]).WeaponType == WowWeaponType.FishingPole);
        }
    }
}