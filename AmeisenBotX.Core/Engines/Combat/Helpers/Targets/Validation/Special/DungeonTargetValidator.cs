using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Special
{
    public class DungeonTargetValidator : ITargetValidator
    {
        ///<summary>
        /// Constructor for DungeonTargetValidator class.
        ///</summary>
        public DungeonTargetValidator(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            // add per map validation functions here, lambda should return true if the unit is
            // invalid, false if its valid
            Validations = new()
            {
                { WowMapId.HallsOfReflection, HallsOfReflectionIsTheLichKing },
                { WowMapId.DrakTharonKeep, DrakTharonKeepIsNovosChanneling },
                { WowMapId.ThroneOfTides, ThroneOfTidesIsLadyNazjarChanneling },
                { WowMapId.TempleOfTheJadeSerpent, TempleOfTheJadeSerpent }
            };
        }

        /// <summary>
        /// Gets or sets the Bot instance of type AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the dictionary of validations for each WowMapId.
        /// </summary>
        private Dictionary<WowMapId, Func<IWowUnit, bool>> Validations { get; }

        /// <summary>
        /// Checks if the provided unit is valid based on the validations stored in Validations dictionary.
        /// Returns true if no validation entry is found, otherwise returns the opposite of the validation result.
        /// </summary>
        /// <param name="unit">The unit to be validated.</param>
        /// <returns>True if the unit is valid or if no validation entry is found, otherwise returns false.</returns>
        public bool IsValid(IWowUnit unit)
        {
            if (Validations.TryGetValue(Bot.Objects.MapId, out Func<IWowUnit, bool> isInvalid))
            {
                return !isInvalid(unit);
            }

            // no entry found, skip validation
            return true;
        }

        /// <summary>
        /// Determines if the specified unit is currently channeling Novos' Channel spell in the Drak'Tharon Keep.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <returns>True if the unit is currently channeling Novos' Channel spell, otherwise false.</returns>
        private bool DrakTharonKeepIsNovosChanneling(IWowUnit unit)
        {
            return unit.CurrentlyChannelingSpellId == 47346;
        }

        /// <summary>
        /// Checks if the provided unit is The Lich King in the Halls of Reflection.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <returns>True if the unit is The Lich King in the Halls of Reflection, false otherwise.</returns>
        private bool HallsOfReflectionIsTheLichKing(IWowUnit unit)
        {
            return Bot.Db.GetUnitName(unit, out string name) && name == "The Lich King";
        }

        /// <summary>
        /// Checks if the unit has either the Peril and Strafe or Wise Mari auras.
        /// </summary>
        private bool TempleOfTheJadeSerpent(IWowUnit unit)
        {
            return unit.Auras.Any(e => e.SpellId == 113315 || e.SpellId == 106062); // Peril and Strafe || Wise Mari
        }

        /// <summary>
        /// Determines if the specified unit is currently channeling the spell "Lady Nazjar" in the Throne of Tides.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <returns>True if the unit is currently channeling the spell "Lady Nazjar", false otherwise.</returns>
        private bool ThroneOfTidesIsLadyNazjarChanneling(IWowUnit unit)
        {
            return unit.CurrentlyChannelingSpellId == 75683;
        }
    }
}