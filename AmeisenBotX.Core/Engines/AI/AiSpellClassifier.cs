using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.AI
{
    public static class AiSpellClassifier
    {
        private static readonly Dictionary<string, AiSpellCategory> SpellCategories = new(StringComparer.OrdinalIgnoreCase)
        {
            // Priest
            { "Flash Heal", AiSpellCategory.HealSelf },
            { "Lesser Heal", AiSpellCategory.HealSelf },
            { "Heal", AiSpellCategory.HealSelf },
            { "Greater Heal", AiSpellCategory.HealSelf },
            { "Power Word: Shield", AiSpellCategory.ShieldSelf },
            { "Renew", AiSpellCategory.HealSelf },
            { "Smite", AiSpellCategory.Damage },
            { "Shadow Word: Pain", AiSpellCategory.DamageDot },
            { "Mind Blast", AiSpellCategory.Damage },
            { "Mind Flay", AiSpellCategory.Damage },
            { "Shadow Word: Death", AiSpellCategory.BurstCooldown },
            { "Devouring Plague", AiSpellCategory.DamageDot },
            { "Psychic Scream", AiSpellCategory.CrowdControl },
            { "Power Word: Fortitude", AiSpellCategory.BuffGroup },
            { "Shadow Protection", AiSpellCategory.BuffGroup },
            { "Dispersion", AiSpellCategory.DefensiveCooldown },
            { "Vampiric Embrace", AiSpellCategory.BuffSelf },
            { "Vampiric Touch", AiSpellCategory.DamageDot },
            
            // Warrior
            { "Charge", AiSpellCategory.Utility },
            { "Intercept", AiSpellCategory.Utility },
            { "Heroic Strike", AiSpellCategory.Damage },
            { "Mortal Strike", AiSpellCategory.Damage },
            { "Execute", AiSpellCategory.Damage },
            { "Overpower", AiSpellCategory.Damage },
            { "Rend", AiSpellCategory.DamageDot },
            { "Thunder Clap", AiSpellCategory.Damage },
            { "Bladestorm", AiSpellCategory.BurstCooldown },
            { "Shield Wall", AiSpellCategory.DefensiveCooldown },
            { "Last Stand", AiSpellCategory.DefensiveCooldown },
            { "Enraged Regeneration", AiSpellCategory.HealSelf },
            { "Battle Shout", AiSpellCategory.BuffGroup },
            { "Intimidating Shout", AiSpellCategory.CrowdControl },
            
            // Paladin
            { "Holy Light", AiSpellCategory.HealSelf },
            { "Flash of Light", AiSpellCategory.HealSelf },
            { "Lay on Hands", AiSpellCategory.DefensiveCooldown },
            { "Divine Shield", AiSpellCategory.DefensiveCooldown },
            { "Hand of Protection", AiSpellCategory.DefensiveCooldown },
            { "Hammer of Justice", AiSpellCategory.CrowdControl },
            { "Judgement of Light", AiSpellCategory.Damage },
            { "Judgement of Wisdom", AiSpellCategory.Damage },
            { "Divine Storm", AiSpellCategory.Damage },
            { "Crusader Strike", AiSpellCategory.Damage },
            { "Avenging Wrath", AiSpellCategory.BurstCooldown },
            
            // Druid
            { "Rejuvenation", AiSpellCategory.HealSelf },
            { "Regrowth", AiSpellCategory.HealSelf },
            { "Lifebloom", AiSpellCategory.HealSelf },
            { "Healing Touch", AiSpellCategory.HealSelf },
            { "Moonfire", AiSpellCategory.DamageDot },
            { "Insect Swarm", AiSpellCategory.DamageDot },
            { "Wrath", AiSpellCategory.Damage },
            { "Starfire", AiSpellCategory.Damage },
            { "Entangling Roots", AiSpellCategory.CrowdControl },
            { "Barkskin", AiSpellCategory.DefensiveCooldown },
            
            // Mage
            { "Fireball", AiSpellCategory.Damage },
            { "Frostbolt", AiSpellCategory.Damage },
            { "Ice Lance", AiSpellCategory.Damage },
            { "Polymorph", AiSpellCategory.CrowdControl },
            { "Frost Nova", AiSpellCategory.CrowdControl },
            { "Blink", AiSpellCategory.Utility },
            { "Ice Block", AiSpellCategory.DefensiveCooldown },
            { "Ice Barrier", AiSpellCategory.ShieldSelf },
            { "Evocation", AiSpellCategory.Utility },
            { "Icy Veins", AiSpellCategory.BurstCooldown },
            { "Mirror Image", AiSpellCategory.BurstCooldown },
            
            // Rogue
            { "Sinister Strike", AiSpellCategory.Damage },
            { "Eviscerate", AiSpellCategory.Damage },
            { "Backstab", AiSpellCategory.Damage },
            { "Kidney Shot", AiSpellCategory.CrowdControl },
            { "Cheap Shot", AiSpellCategory.CrowdControl },
            { "Blind", AiSpellCategory.CrowdControl },
            { "Vanish", AiSpellCategory.DefensiveCooldown },
            { "Evasion", AiSpellCategory.DefensiveCooldown },
            { "Cloak of Shadows", AiSpellCategory.DefensiveCooldown },
            { "Sprint", AiSpellCategory.Utility },
            
            // Hunter
            { "Serpent Sting", AiSpellCategory.DamageDot },
            { "Arcane Shot", AiSpellCategory.Damage },
            { "Steady Shot", AiSpellCategory.Damage },
            { "Kill Command", AiSpellCategory.Damage },
            { "Bestial Wrath", AiSpellCategory.BurstCooldown },
            { "Deterrence", AiSpellCategory.DefensiveCooldown },
            { "Feign Death", AiSpellCategory.DefensiveCooldown },
            { "Disengage", AiSpellCategory.Utility },
            { "Freezing Trap", AiSpellCategory.CrowdControl },
            
            // Warlock
            { "Shadow Bolt", AiSpellCategory.Damage },
            { "Incinerate", AiSpellCategory.Damage },
            { "Immolate", AiSpellCategory.DamageDot },
            { "Corruption", AiSpellCategory.DamageDot },
            { "Curse of Agony", AiSpellCategory.DamageDot },
            { "Fear", AiSpellCategory.CrowdControl },
            { "Death Coil", AiSpellCategory.CrowdControl }, // Also heals
            { "Drain Life", AiSpellCategory.HealSelf },
            
            // Shaman
            { "Lightning Bolt", AiSpellCategory.Damage },
            { "Chain Lightning", AiSpellCategory.Damage },
            { "Earth Shock", AiSpellCategory.Damage },
            { "Flame Shock", AiSpellCategory.DamageDot },
            { "Lava Burst", AiSpellCategory.Damage },
            { "Healing Wave", AiSpellCategory.HealSelf },
            { "Lesser Healing Wave", AiSpellCategory.HealSelf },
            { "Chain Heal", AiSpellCategory.HealGroup },
            { "Hex", AiSpellCategory.CrowdControl },
            { "Bloodlust", AiSpellCategory.BurstCooldown },
            { "Heroism", AiSpellCategory.BurstCooldown },
            { "Feral Spirit", AiSpellCategory.BurstCooldown },
            { "Shamanistic Rage", AiSpellCategory.DefensiveCooldown },
            
            // DK
            { "Icy Touch", AiSpellCategory.Damage },
            { "Plague Strike", AiSpellCategory.Damage },
            { "Obliterate", AiSpellCategory.Damage },
            { "Death Strike", AiSpellCategory.HealSelf }, // Also damage
            { "Blood Strike", AiSpellCategory.Damage },
            { "Heart Strike", AiSpellCategory.Damage },
            { "Death Grip", AiSpellCategory.Utility },
            { "Icebound Fortitude", AiSpellCategory.DefensiveCooldown },
            { "Anti-Magic Shell", AiSpellCategory.ShieldSelf },
            { "Empower Rune Weapon", AiSpellCategory.BurstCooldown },
            { "Army of the Dead", AiSpellCategory.BurstCooldown },
        };

        public static AiSpellCategory Classify(string spellName)
        {
            if (string.IsNullOrWhiteSpace(spellName)) return AiSpellCategory.Unknown;

            if (SpellCategories.TryGetValue(spellName, out AiSpellCategory category))
            {
                return category;
            }

            return AiSpellCategory.Unknown;
        }

        public static void Register(string spellName, AiSpellCategory category)
        {
            if (!SpellCategories.ContainsKey(spellName))
            {
                SpellCategories.Add(spellName, category);
            }
        }
    }
}
