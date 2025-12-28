using AmeisenBotX.Wow.Objects;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Core
{
    /// <summary>
    /// PERFECTED CC Protection module with comprehensive aura validation.
    /// NEVER break crowd control - causes wipes.
    /// Always active - CC protection is critical.
    /// </summary>
    public class CCProtectionModule : ITargetSelectionModule
    {
        public string Name => "CCProtection";

        // Comprehensive CC spell list using centralized constants
        private static readonly HashSet<string> CCSpellNames =
        [
            // Mage
            CrowdControlWotlk.Polymorph, CrowdControlWotlk.PolymorphSheep, CrowdControlWotlk.PolymorphPig,
            CrowdControlWotlk.PolymorphTurtle, CrowdControlWotlk.PolymorphRabbit, CrowdControlWotlk.PolymorphBlackCat,

            // Shaman
            CrowdControlWotlk.Hex, CrowdControlWotlk.HexFrog,

            // Druid
            CrowdControlWotlk.Hibernate, CrowdControlWotlk.Cyclone, CrowdControlWotlk.EntanglingRoots, CrowdControlWotlk.NaturesGrasp,

            // Rogue
            CrowdControlWotlk.Sap, CrowdControlWotlk.Blind, CrowdControlWotlk.Gouge,

            // Hunter
            CrowdControlWotlk.FreezingTrap, CrowdControlWotlk.FreezingArrow, CrowdControlWotlk.WyvernSting, CrowdControlWotlk.ScatterShot,

            // Warlock
            CrowdControlWotlk.Fear, CrowdControlWotlk.Seduction, CrowdControlWotlk.Banish, CrowdControlWotlk.HowlOfTerror, CrowdControlWotlk.DeathCoilWarlock,

            // Priest
            CrowdControlWotlk.PsychicScream, CrowdControlWotlk.ShackleUndead, CrowdControlWotlk.MindControl,

            // Paladin
            CrowdControlWotlk.Repentance, CrowdControlWotlk.TurnEvil, CrowdControlWotlk.HammerOfJustice,

            // Warrior
            CrowdControlWotlk.IntimidatingShout,

            // Other
            CrowdControlWotlk.Sleep, CrowdControlWotlk.Incapacitate
        ];

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Always active - CC protection is critical in all contexts
            return true;
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (target == null || bot?.Db == null)
            {
                return 0f;
            }

            // Edge case: Target has no auras
            if (target.Auras == null || !target.Auras.Any())
            {
                return 0f;
            }

            // Edge case: Dead target with lingering auras
            if (target.IsDead)
            {
                return 0f; // Dead targets are fine to "target" (for looting), CC irrelevant
            }

            // Check each aura for CC effects
            try
            {
                foreach (IWowAura aura in target.Auras)
                {
                    // Edge case: Invalid spell ID (0 or negative)
                    if (aura.SpellId <= 0)
                    {
                        continue;
                    }

                    string spellName = bot.Db.GetSpellName(aura.SpellId);

                    // Edge case: Spell name is null or empty
                    if (string.IsNullOrEmpty(spellName))
                    {
                        continue;
                    }

                    // Check if it's a CC spell
                    if (CCSpellNames.Contains(spellName))
                    {
                        // MASSIVE penalty - DO NOT BREAK CC!
                        return -1000f;
                    }

                    // Also check partial matches (some spells have variations)
                    foreach (string ccSpell in CCSpellNames)
                    {
                        if (spellName.Contains(ccSpell, StringComparison.OrdinalIgnoreCase))
                        {
                            return -1000f;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Edge case: Aura enumeration fails - better safe than sorry
                // Return small penalty to discourage targeting this unit
                return -10f;
            }

            return 0f;
        }
    }
}

