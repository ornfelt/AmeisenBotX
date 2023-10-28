using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Movement.Providers.Basic;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a movement provider for dungeon maps.
/// </summary>
namespace AmeisenBotX.Core.Engines.Movement.Providers.Special
{
    /// <summary>
    /// Represents a movement provider for dungeon maps.
    /// </summary>
    public class DungeonMovementProvider : IMovementProvider
    {
        /// <summary>
        /// Initializes a new instance of the DungeonMovementProvider class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance used for movement.</param>
        public DungeonMovementProvider(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            Providers = new()
            {
                { WowMapId.TempleOfTheJadeSerpent, TempleOfTheJadeSerpent },
            };

            MariMovementProvider = new StayAroundMovementProvider(() => (Bot.Target, MathF.PI * 0.75f, Bot.CombatClass == null || Bot.CombatClass.IsMelee ? Bot.Player.MeleeRangeTo(Bot.Target) : 7.5f));
        }

        /// <summary>
        /// Gets or sets the private property "Bot" of type AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Represents the private property for the StayAroundMovementProvider object that provides movement functionality for the Mari object.
        /// </summary>
        private StayAroundMovementProvider MariMovementProvider { get; }

        /// <summary>
        /// Gets or sets the dictionary of WowMapId and corresponding movement provider functions.
        /// </summary>
        private Dictionary<WowMapId, Func<IMovementProvider>> Providers { get; }

        /// <summary>
        /// Get the position and movement type of the object.
        /// </summary>
        public bool Get(out Vector3 position, out MovementAction type)
        {
            if (Providers.TryGetValue(Bot.Objects.MapId, out Func<IMovementProvider> getProvider))
            {
                IMovementProvider provider = getProvider();

                if (provider != null)
                {
                    return provider.Get(out position, out type);
                }
            }

            type = MovementAction.None;
            position = Vector3.Zero;
            return false;
        }

        /// <summary>
        /// Returns a movement provider for the Temple of the Jade Serpent dungeon. 
        /// If any enemy unit is currently casting or channeling the spell with ID 106055, returns the <see cref="MariMovementProvider"/>.
        /// Otherwise, returns null.
        /// </summary>
        private IMovementProvider TempleOfTheJadeSerpent()
        {
            if (Bot.Objects.All.OfType<IWowUnit>().Any(e => e.CurrentlyCastingSpellId == 106055 || e.CurrentlyChannelingSpellId == 106055))
            {
                // dodge wise mari hydrolance
                return MariMovementProvider;
            }

            return null;
        }
    }
}