using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Dungeons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Special
{
    public class DungeonTargetValidator : ITargetValidator
    {
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

        private AmeisenBotInterfaces Bot { get; }

        private Dictionary<WowMapId, Func<IWowUnit, bool>> Validations { get; }

        public bool IsValid(IWowUnit unit)
        {
            if (Validations.TryGetValue(Bot.Objects.MapId, out Func<IWowUnit, bool> isInvalid))
            {
                return !isInvalid(unit);
            }

            // no entry found, skip validation
            return true;
        }

        private bool DrakTharonKeepIsNovosChanneling(IWowUnit unit)
        {
            return unit.CurrentlyChannelingSpellId == DrakTharonKeep.NovosChannelingSpellId;
        }

        private bool HallsOfReflectionIsTheLichKing(IWowUnit unit)
        {
            return Bot.Db.GetUnitName(unit, out string name) && name == HallsOfReflection.TheLichKing;
        }

        private bool TempleOfTheJadeSerpent(IWowUnit unit)
        {
            // Note: Cataclysm+ dungeon - spell IDs kept for reference
            return unit.Auras.Any(e => e.SpellId is 113315 or 106062);
        }

        private bool ThroneOfTidesIsLadyNazjarChanneling(IWowUnit unit)
        {
            // Note: Cataclysm dungeon - spell ID kept for reference
            return unit.CurrentlyChannelingSpellId == 75683;
        }
    }
}

