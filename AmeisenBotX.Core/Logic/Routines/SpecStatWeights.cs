using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Comprehensive stat weights for all WotLK 3.3.5 class/spec combinations.
    /// Higher weight = more valuable for that spec.
    /// Based on EJ/SimCraft values for WotLK.
    /// </summary>
    public static class SpecStatWeights
    {
        /// <summary>
        /// Gets stat weights for a spec enum.
        /// Returns null if spec not found.
        /// </summary>
        public static Dictionary<string, double> GetWeights(WowSpecialization spec)
        {
            return StatWeightsBySpec.TryGetValue(spec, out Dictionary<string, double> weights) ? weights : null;
        }

        /// <summary>
        /// Stats in order of general importance (for display).
        /// </summary>
        public static readonly string[] AllStats =
        [
            "Strength", "Agility", "Stamina", "Intellect", "Spirit",
            "AttackPower", "SpellPower", "Crit", "Hit", "Haste",
            "Expertise", "ArmorPen", "Defense", "Dodge", "Parry", "Block",
            "Mp5", "SpellPen"
        ];

        private static readonly Dictionary<WowSpecialization, Dictionary<string, double>> StatWeightsBySpec = new()
        {
            // ================ DEATH KNIGHT ================
            [WowSpecialization.DeathknightBlood] = new() // DPS (WotLK Blood is DPS)
            {
                { "Strength", 2.28 }, { "ArmorPen", 1.52 }, { "Crit", 1.18 },
                { "Expertise", 1.35 }, { "Hit", 1.13 }, { "Haste", 0.98 },
                { "Agility", 0.68 }, { "AttackPower", 1.0 }, { "Stamina", 0.2 }
            },
            [WowSpecialization.DeathknightFrost] = new() // DW Frost DPS
            {
                { "Strength", 2.14 }, { "Hit", 1.68 }, { "Expertise", 1.45 },
                { "ArmorPen", 1.38 }, { "Crit", 1.12 }, { "Haste", 1.05 },
                { "Agility", 0.72 }, { "AttackPower", 1.0 }, { "Stamina", 0.2 }
            },
            [WowSpecialization.DeathknightUnholy] = new() // Unholy DPS
            {
                { "Strength", 2.35 }, { "Hit", 1.52 }, { "Crit", 1.28 },
                { "Haste", 1.15 }, { "Expertise", 1.08 }, { "ArmorPen", 0.95 },
                { "Agility", 0.65 }, { "AttackPower", 1.0 }, { "Stamina", 0.2 }
            },

            // ================ DRUID ================
            [WowSpecialization.DruidBalance] = new() // Moonkin
            {
                { "SpellPower", 1.0 }, { "Haste", 0.78 }, { "Crit", 0.68 },
                { "Hit", 0.92 }, { "Intellect", 0.45 }, { "Spirit", 0.38 },
                { "Stamina", 0.15 }
            },
            [WowSpecialization.DruidFeralCat] = new() // Cat DPS
            {
                { "Agility", 1.52 }, { "Strength", 1.12 }, { "ArmorPen", 1.85 },
                { "Crit", 0.82 }, { "Expertise", 1.08 }, { "Hit", 0.95 },
                { "Haste", 0.58 }, { "AttackPower", 0.5 }, { "Stamina", 0.2 }
            },
            [WowSpecialization.DruidFeralBear] = new() // Bear Tank
            {
                { "Stamina", 1.8 }, { "Agility", 1.65 }, { "Defense", 0.85 },
                { "Dodge", 1.25 }, { "Expertise", 0.72 }, { "Hit", 0.58 },
                { "Strength", 0.45 }, { "AttackPower", 0.25 }
            },
            [WowSpecialization.DruidRestoration] = new() // Resto Healer
            {
                { "SpellPower", 0.85 }, { "Haste", 1.05 }, { "Intellect", 0.68 },
                { "Spirit", 0.78 }, { "Crit", 0.52 }, { "Mp5", 0.72 },
                { "Stamina", 0.2 }
            },

            // ================ HUNTER ================
            [WowSpecialization.HunterBeastmastery] = new()
            {
                { "Agility", 1.78 }, { "Hit", 1.35 }, { "ArmorPen", 1.42 },
                { "Crit", 1.08 }, { "Haste", 0.68 }, { "Intellect", 0.28 },
                { "AttackPower", 0.5 }, { "Stamina", 0.15 }
            },
            [WowSpecialization.HunterMarksmanship] = new()
            {
                { "Agility", 1.92 }, { "ArmorPen", 1.68 }, { "Hit", 1.42 },
                { "Crit", 1.15 }, { "Haste", 0.72 }, { "Intellect", 0.25 },
                { "AttackPower", 0.5 }, { "Stamina", 0.15 }
            },
            [WowSpecialization.HunterSurvival] = new()
            {
                { "Agility", 2.05 }, { "Hit", 1.52 }, { "Crit", 1.28 },
                { "ArmorPen", 1.15 }, { "Haste", 0.85 }, { "Intellect", 0.32 },
                { "AttackPower", 0.5 }, { "Stamina", 0.15 }
            },

            // ================ MAGE ================
            [WowSpecialization.MageArcane] = new()
            {
                { "SpellPower", 1.0 }, { "Haste", 0.92 }, { "Crit", 0.78 },
                { "Hit", 1.05 }, { "Intellect", 0.58 }, { "Spirit", 0.25 },
                { "Stamina", 0.1 }
            },
            [WowSpecialization.MageFire] = new()
            {
                { "SpellPower", 1.0 }, { "Crit", 0.88 }, { "Haste", 0.82 },
                { "Hit", 1.02 }, { "Intellect", 0.52 }, { "Spirit", 0.18 },
                { "Stamina", 0.1 }
            },
            [WowSpecialization.MageFrost] = new()
            {
                { "SpellPower", 1.0 }, { "Haste", 0.85 }, { "Crit", 0.72 },
                { "Hit", 0.98 }, { "Intellect", 0.55 }, { "Spirit", 0.2 },
                { "Stamina", 0.1 }
            },

            // ================ PALADIN ================
            [WowSpecialization.PaladinHoly] = new() // Holy Healer
            {
                { "Intellect", 0.85 }, { "SpellPower", 0.78 }, { "Crit", 0.72 },
                { "Haste", 0.65 }, { "Mp5", 0.82 }, { "Spirit", 0.15 },
                { "Stamina", 0.25 }
            },
            [WowSpecialization.PaladinProtection] = new() // Prot Tank
            {
                { "Stamina", 1.65 }, { "Defense", 1.25 }, { "Dodge", 0.92 },
                { "Parry", 0.88 }, { "Block", 0.75 }, { "Strength", 0.58 },
                { "Expertise", 0.72 }, { "Hit", 0.48 }
            },
            [WowSpecialization.PaladinRetribution] = new() // Ret DPS
            {
                { "Strength", 2.05 }, { "Expertise", 1.25 }, { "Hit", 1.18 },
                { "Crit", 0.95 }, { "Haste", 0.72 }, { "Agility", 0.55 },
                { "AttackPower", 0.5 }, { "Stamina", 0.2 }
            },

            // ================ PRIEST ================
            [WowSpecialization.PriestDiscipline] = new() // Disc Healer
            {
                { "SpellPower", 0.92 }, { "Intellect", 0.78 }, { "Crit", 0.85 },
                { "Haste", 0.68 }, { "Spirit", 0.52 }, { "Mp5", 0.58 },
                { "Stamina", 0.2 }
            },
            [WowSpecialization.PriestHoly] = new() // Holy Healer
            {
                { "SpellPower", 0.88 }, { "Intellect", 0.72 }, { "Haste", 0.78 },
                { "Spirit", 0.85 }, { "Crit", 0.62 }, { "Mp5", 0.65 },
                { "Stamina", 0.2 }
            },
            [WowSpecialization.PriestShadow] = new() // Shadow DPS
            {
                { "SpellPower", 1.0 }, { "Haste", 0.85 }, { "Crit", 0.72 },
                { "Hit", 1.08 }, { "Spirit", 0.58 }, { "Intellect", 0.45 },
                { "Stamina", 0.15 }
            },

            // ================ ROGUE ================
            [WowSpecialization.RogueAssassination] = new() // Mutilate
            {
                { "Agility", 1.82 }, { "Hit", 1.65 }, { "Expertise", 1.42 },
                { "ArmorPen", 0.78 }, { "Crit", 0.92 }, { "Haste", 1.05 },
                { "AttackPower", 0.5 }, { "Stamina", 0.15 }
            },
            [WowSpecialization.RogueCombat] = new()
            {
                { "Agility", 1.75 }, { "ArmorPen", 1.85 }, { "Expertise", 1.52 },
                { "Hit", 1.38 }, { "Crit", 0.95 }, { "Haste", 0.82 },
                { "AttackPower", 0.5 }, { "Stamina", 0.15 }
            },
            [WowSpecialization.RogueSubtlety] = new()
            {
                { "Agility", 1.88 }, { "Hit", 1.55 }, { "Expertise", 1.45 },
                { "Crit", 1.02 }, { "Haste", 0.92 }, { "ArmorPen", 0.72 },
                { "AttackPower", 0.5 }, { "Stamina", 0.15 }
            },

            // ================ SHAMAN ================
            [WowSpecialization.ShamanElemental] = new() // Ele DPS
            {
                { "SpellPower", 1.0 }, { "Haste", 0.88 }, { "Crit", 0.75 },
                { "Hit", 1.02 }, { "Intellect", 0.45 }, { "Spirit", 0.15 },
                { "Stamina", 0.15 }
            },
            [WowSpecialization.ShamanEnhancement] = new() // Enh DPS
            {
                { "Agility", 1.68 }, { "Expertise", 1.55 }, { "Hit", 1.42 },
                { "Strength", 0.78 }, { "Crit", 0.88 }, { "Haste", 0.92 },
                { "AttackPower", 0.5 }, { "SpellPower", 0.35 }, { "Stamina", 0.2 }
            },
            [WowSpecialization.ShamanRestoration] = new() // Resto Healer
            {
                { "SpellPower", 0.82 }, { "Haste", 0.95 }, { "Intellect", 0.68 },
                { "Mp5", 0.78 }, { "Crit", 0.58 }, { "Spirit", 0.25 },
                { "Stamina", 0.2 }
            },

            // ================ WARLOCK ================
            [WowSpecialization.WarlockAffliction] = new()
            {
                { "SpellPower", 1.0 }, { "Haste", 0.92 }, { "Crit", 0.65 },
                { "Hit", 1.05 }, { "Spirit", 0.58 }, { "Intellect", 0.42 },
                { "Stamina", 0.15 }
            },
            [WowSpecialization.WarlockDemonology] = new()
            {
                { "SpellPower", 1.0 }, { "Haste", 0.85 }, { "Crit", 0.72 },
                { "Hit", 1.02 }, { "Spirit", 0.52 }, { "Intellect", 0.45 },
                { "Stamina", 0.15 }
            },
            [WowSpecialization.WarlockDestruction] = new()
            {
                { "SpellPower", 1.0 }, { "Crit", 0.88 }, { "Haste", 0.82 },
                { "Hit", 1.08 }, { "Intellect", 0.48 }, { "Spirit", 0.38 },
                { "Stamina", 0.15 }
            },

            // ================ WARRIOR ================
            [WowSpecialization.WarriorArms] = new()
            {
                { "Strength", 2.18 }, { "ArmorPen", 1.72 }, { "Crit", 1.15 },
                { "Expertise", 1.08 }, { "Hit", 0.95 }, { "Haste", 0.58 },
                { "Agility", 0.62 }, { "AttackPower", 0.5 }, { "Stamina", 0.2 }
            },
            [WowSpecialization.WarriorFury] = new()
            {
                { "Strength", 2.25 }, { "ArmorPen", 1.65 }, { "Expertise", 1.18 },
                { "Crit", 1.12 }, { "Hit", 1.08 }, { "Haste", 0.72 },
                { "Agility", 0.58 }, { "AttackPower", 0.5 }, { "Stamina", 0.2 }
            },
            [WowSpecialization.WarriorProtection] = new() // Prot Tank
            {
                { "Stamina", 1.75 }, { "Defense", 1.35 }, { "Dodge", 0.95 },
                { "Parry", 0.92 }, { "Block", 0.85 }, { "Expertise", 0.78 },
                { "Hit", 0.62 }, { "Strength", 0.55 }
            }
        };

        /// <summary>
        /// Default weights for unknown specs (generic DPS).
        /// </summary>
        public static readonly Dictionary<string, double> DefaultDpsWeights = new()
        {
            { "Strength", 1.5 }, { "Agility", 1.5 }, { "AttackPower", 1.0 },
            { "Crit", 0.8 }, { "Hit", 0.9 }, { "Haste", 0.7 },
            { "Intellect", 0.3 }, { "Stamina", 0.2 }
        };

        /// <summary>
        /// Default weights for healing specs.
        /// </summary>
        public static readonly Dictionary<string, double> DefaultHealWeights = new()
        {
            { "SpellPower", 1.0 }, { "Intellect", 0.7 }, { "Spirit", 0.6 },
            { "Haste", 0.8 }, { "Crit", 0.6 }, { "Mp5", 0.7 },
            { "Stamina", 0.2 }
        };

        /// <summary>
        /// Default weights for tank specs.
        /// </summary>
        public static readonly Dictionary<string, double> DefaultTankWeights = new()
        {
            { "Stamina", 1.8 }, { "Defense", 1.3 }, { "Dodge", 1.0 },
            { "Parry", 0.9 }, { "Block", 0.8 }, { "Expertise", 0.7 },
            { "Hit", 0.5 }, { "Strength", 0.5 }
        };
    }
}
