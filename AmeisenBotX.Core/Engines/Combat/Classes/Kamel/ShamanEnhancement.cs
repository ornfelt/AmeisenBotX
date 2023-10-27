using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    internal class ShamanEnhancement : BasicKamelClass
    {
        /// <summary>
        /// Represents the name of the Earthbind Totem.
        /// </summary>
        private const string earthbindTotem = "Earthbind Totem";

        /// <summary>
        /// The name of the Earth Elemental Totem.
        /// </summary>
        private const string earthElementalTotem = "Earth Elemental Totem";

        /// <summary>
        /// Represents the name of the "Earth Shock" spell.
        /// </summary>
        private const string earthShockSpell = "Earth Shock";

        /// <summary>
        /// Represents the name of the spell "Feral Spirit".
        /// </summary>
        private const string feralSpiritSpell = "Feral Spirit";

        /// <summary>
        /// Represents the name of the fire elemental totem.
        /// </summary>
        //Totem
        private const string fireElementalTotem = "Fire Elemental Totem";

        /// <summary>
        /// Represents the name of the Flame Shock spell.
        /// </summary>
        private const string flameShockSpell = "Flame Shock";

        /// <summary>
        /// This constant represents the name of the buff called Flametongue.
        /// </summary>
        private const string flametongueBuff = "Flametongue";

        /// <summary>
        /// The name of the Flametongue Weapon spell.
        /// </summary>
        private const string flametongueSpell = "Flametongue Weapon";

        /// <summary>
        /// The constant string representing the spell "Frost Shock".
        /// </summary>
        private const string frostShockSpell = "Frost Shock";

        /// <summary>
        /// The constant string representing "Grounding Totem".
        /// </summary>
        private const string groundingTotem = "Grounding Totem";

        /// <summary>
        /// The name of the healing spell is "Healing Wave".
        /// </summary>
        //Heal Spells
        private const string healingWaveSpell = "Healing Wave";

        /// <summary>
        /// Represents the name of the "Lava Lash" spell.
        /// </summary>
        private const string lavaLashSpell = "Lava Lash";

        /// <summary>
        /// Represents the name of the attack spell "Lightning Bolt".
        /// </summary>
        //Attack Spells
        private const string lightningBoltSpell = "Lightning Bolt";

        /// <summary>
        /// The name of the lightning shield spell.
        /// </summary>
        //Shield
        private const string lightningShieldSpell = "Lightning Shield";

        /// <summary>
        /// The constant string containing the Spell to be purged.
        /// </summary>
        private const string purgeSpell = "Purge";

        /// <summary>
        /// The name of the spell "Shamanistic Rage".
        /// </summary>
        //Buff
        private const string shamanisticRageSpell = "Shamanistic Rage";

        /// <summary>
        /// Represents the name of the stormstrike spell.
        /// </summary>
        private const string stormstrikeSpell = "Stormstrike";

        /// <summary>
        /// Constant representing the windfury enhancement for weapons.
        /// </summary>
        //Weapon Enhancement
        private const string windfuryBuff = "Windfury";

        /// <summary>
        /// The name of the Windfury Weapon spell.
        /// </summary>
        private const string windfurySpell = "Windfury Weapon";

        /// <summary>
        /// The name of the spell that stuns and interrupts the target.
        /// </summary>
        //Stunns|Interrupting
        private const string windShearSpell = "Wind Shear";

        /// <summary>
        /// Constructor for the ShamanEnhancement class.
        /// Initializes the ShamanEnhancement object with the provided bot parameter.
        /// Initializes spell cooldowns for various spells and totems.
        /// Initializes EnhancementEvent and PurgeEvent.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        public ShamanEnhancement(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Shield
            spellCoolDown.Add(lightningShieldSpell, DateTime.Now);

            //Weapon Enhancement
            spellCoolDown.Add(windfuryBuff, DateTime.Now);
            spellCoolDown.Add(flametongueBuff, DateTime.Now);
            spellCoolDown.Add(flametongueSpell, DateTime.Now);
            spellCoolDown.Add(windfurySpell, DateTime.Now);

            //Heal Spells
            spellCoolDown.Add(healingWaveSpell, DateTime.Now);

            //Totem
            spellCoolDown.Add(fireElementalTotem, DateTime.Now);
            spellCoolDown.Add(earthElementalTotem, DateTime.Now);
            spellCoolDown.Add(groundingTotem, DateTime.Now);
            spellCoolDown.Add(earthbindTotem, DateTime.Now);

            //Attack Spells
            spellCoolDown.Add(lightningBoltSpell, DateTime.Now);
            spellCoolDown.Add(lavaLashSpell, DateTime.Now);
            spellCoolDown.Add(stormstrikeSpell, DateTime.Now);
            spellCoolDown.Add(flameShockSpell, DateTime.Now);
            spellCoolDown.Add(frostShockSpell, DateTime.Now);
            spellCoolDown.Add(earthShockSpell, DateTime.Now);
            spellCoolDown.Add(feralSpiritSpell, DateTime.Now);

            //Stunns|Interrupting
            spellCoolDown.Add(windShearSpell, DateTime.Now);
            spellCoolDown.Add(purgeSpell, DateTime.Now);

            //Buff
            spellCoolDown.Add(shamanisticRageSpell, DateTime.Now);

            //Event
            EnhancementEvent = new(TimeSpan.FromSeconds(2));
            PurgeEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets or sets the author of the code.
        /// </summary>
        /// <value>
        /// The author of the code.
        /// </value>
        public override string Author => "Lukas";

        /// <summary>
        /// A dictionary property named C that holds a collection of key-value pairs.
        /// Each key is of type string and each value can be of any type(dynamic).
        /// Initially, an empty dictionary is assigned to C.
        /// </summary>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the Shaman Enhancement.
        /// </summary>
        public override string Description => "Shaman Enhancement";

        /// <summary>
        /// Gets the display name for a Shaman Enhancement.
        /// </summary>
        public override string DisplayName => "Shaman Enhancement";

        /// <summary>
        /// Gets or sets the enhancement event.
        /// </summary>
        //Event
        public TimegatedEvent EnhancementEvent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object handles movement or not.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the entity is a melee type.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for the specified item.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand });

        /// <summary>
        /// Gets or sets the TimegatedEvent used to purge an event.
        /// </summary>
        public TimegatedEvent PurgeEvent { get; private set; }

        /// <summary>
        /// Gets or sets the role of the World of Warcraft character as a Damage Per Second (DPS) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talent tree.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 3, new(1, 3, 3) },
                { 5, new(1, 5, 3) },
                { 8, new(1, 8, 5) },
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 3) },
                { 9, new(2, 9, 1) },
                { 11, new(2, 11, 5) },
                { 13, new(2, 13, 2) },
                { 14, new(2, 14, 1) },
                { 15, new(2, 15, 3) },
                { 16, new(2, 16, 3) },
                { 17, new(2, 17, 3) },
                { 19, new(2, 19, 3) },
                { 20, new(2, 20, 1) },
                { 21, new(2, 21, 1) },
                { 22, new(2, 22, 3) },
                { 23, new(2, 23, 1) },
                { 24, new(2, 24, 2) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 1) },
                { 28, new(2, 28, 5) },
                { 29, new(2, 29, 1) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether the character can use auto attacks.
        /// </summary>
        /// <returns>True if auto attacks are enabled, otherwise false.</returns>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind enemies.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character cannot walk behind enemies; otherwise, <c>false</c>.
        /// </value>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class of the character as a Shaman.
        /// </summary>
        public override WowClass WowClass => WowClass.Shaman;

        /// <summary>
        /// Executes the CC attack by starting the attack.
        /// </summary>
        public override void ExecuteCC()
        {
            StartAttack();
        }

        /// <summary>
        /// Executes actions when the character is out of combat.
        /// Calls the Shield method to apply a shield to the character.
        /// Calls the WeaponEnhancement method to enhance the character's weapon.
        /// Calls the RevivePartyMember method with the ancestralSpiritSpell parameter to revive a party member.
        /// Calls the TargetSelection method to select the target for the upcoming attack.
        /// Calls the StartAttack method to initiate the attack.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            Shield();
            WeaponEnhancement();
            RevivePartyMember(ancestralSpiritSpell);
            Targetselection();
            StartAttack();
        }

        /// <summary>
        /// Checks if the player does not have the "Lightning Shield" aura and sufficient mana to cast the spell,
        /// and returns if that condition is met.
        /// </summary>
        private void Shield()
        {
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Lightning Shield") && CustomCastSpellMana(lightningShieldSpell))
            {
                return;
            }
        }

        /// <summary>
        /// Method to start the attack sequence.
        /// </summary>
        private void StartAttack()
        {
            if (Bot.Wow.TargetGuid != 0)
            {
                ChangeTargetToAttack();

                if (Bot.Db.GetReaction(Bot.Player, Bot.Target) == WowUnitReaction.Friendly)
                {
                    Bot.Wow.ClearTarget();
                    return;
                }

                if (Bot.Player.IsInMeleeRange(Bot.Target))
                {
                    if (!Bot.Player.IsAutoAttacking && AutoAttackEvent.Run())
                    {
                        Bot.Wow.StartAutoAttack();
                    }

                    if (Bot.Player.Auras.FirstOrDefault(e => Bot.Db.GetSpellName(e.SpellId) == "Maelstrom Weapon").StackCount >= 5
                    && ((Bot.Player.HealthPercentage >= 50 && CustomCastSpellMana(lightningBoltSpell)) || CustomCastSpellMana(healingWaveSpell)))
                    {
                        return;
                    }
                    if (Bot.Target.IsCasting && CustomCastSpellMana(windShearSpell))
                    {
                        return;
                    }
                    if (PurgeEvent.Run() &&
                        (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Mana Shield")
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Power Word: Shield")
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Renew")
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Riptide")
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Earth Shield")) && CustomCastSpellMana(purgeSpell))
                    {
                        return;
                    }
                    if (TotemItemCheck() && CustomCastSpellMana(fireElementalTotem))
                    {
                        return;
                    }
                    if (TotemItemCheck() && CustomCastSpellMana(earthElementalTotem))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(lavaLashSpell))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(stormstrikeSpell))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(feralSpiritSpell))
                    {
                        return;
                    }
                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Flame Shock") && CustomCastSpellMana(flameShockSpell))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(frostShockSpell))
                    {
                        return;
                    }
                }
            }
            else
            {
                Targetselection();
            }
        }

        /// <summary>
        /// Method for enhancing the weapon.
        /// </summary>
        private void WeaponEnhancement()
        {
            if (EnhancementEvent.Run())
            {
                if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, windfuryBuff, windfurySpell))
                {
                    return;
                }

                if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_OFFHAND, flametongueBuff, flametongueSpell))
                {
                    return;
                }
            }
        }
    }
}