using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    /// <summary>
    /// Represents a Restoration Shaman class that inherits from the BasicKamelClass.
    /// </summary>
    internal class RestorationShaman : BasicKamelClass
    {
        /// <summary>
        /// This is a private constant string with the value "Bloodlust".
        /// </summary>
        private const string Bloodlust = "Bloodlust";

        /// <summary>
        /// Represents the name of the spell "Call of the Elements".
        /// </summary>
        private const string CalloftheElementsSpell = "Call of the Elements";

        /// <summary>
        /// The name of the chain heal spell.
        /// </summary>
        private const string chainHealSpell = "Chain Heal";

        /// <summary>
        /// The name of the Earthliving buff.
        /// </summary>
        private const string earthlivingBuff = "Earthliving ";

        /// <summary>
        /// The constant representing the spell name for Earthliving Weapon.
        /// </summary>
        private const string earthlivingWeaponSpell = "Earthliving Weapon";

        /// <summary>
        /// Represents the name of the Earth Shield spell.
        /// </summary>
        private const string earthShieldSpell = "Earth Shield";

        /// <summary>
        /// Represents the name of the Earth Shock spell.
        /// </summary>
        private const string earthShockSpell = "Earth Shock";

        /// <summary>
        /// Represents the name of the spell "Flame Shock".
        /// </summary>
        private const string flameShockSpell = "Flame Shock";

        /// <summary>
        /// Represents the Gift of the Naaru spell for the Draenei race.
        /// </summary>
        //Race (Draenei)
        private const string giftOfTheNaaruSpell = "Gift of the Naaru";

        /// <summary>
        /// The healing wave spell.
        /// </summary>
        //Spells
        private const string healingWaveSpell = "Healing Wave";

        /// <summary>
        /// Represents the name of the heroism spell.
        /// </summary>
        private const string heroismSpell = "Heroism";
        /// <summary>
        /// Represents the name of the lesser healing wave spell.
        /// </summary>
        private const string lesserHealingWaveSpell = "Lesser Healing Wave";

        /// <summary>
        /// Represents the Lightning Bolt spell.
        /// </summary>
        //Spells / DMG
        private const string LightningBoltSpell = "Lightning Bolt";

        /// <summary>
        /// The constant representing the Lightning Shield spell.
        /// </summary>
        private const string LightningShieldSpell = "Lightning Shield";
        /// <summary>
        /// The name of the Mana Spring Totem spell.
        /// </summary>
        private const string ManaSpringTotemSpell = "Mana Spring Totem";
        /// <summary>
        /// Represents the spell name for Mana Tide Totem.
        /// </summary>
        private const string ManaTideTotemSpell = "Mana Tide Totem";

        /// <summary>
        /// The constant string representing the spell "Nature's Swiftness"
        /// </summary>
        //CD|Buffs
        private const string naturesswiftSpell = "Nature's Swiftness";

        /// <summary>
        /// The name of the constant riptide spell.
        /// </summary>
        private const string riptideSpell = "Riptide";
        /// <summary>
        /// Represents the name of the "Strength of Earth Totem" spell.
        /// </summary>
        private const string StrengthofEarthTotemSpell = "Strength of Earth Totem";
        /// <summary>
        /// Represents the name of the tidal force spell.
        /// </summary>
        private const string tidalForceSpell = "Tidal Force";
        /// <summary>
        /// The name of the water shield spell.
        /// </summary>
        private const string watershieldSpell = "Water shield";

        /// <summary>
        /// The name of the Windfury Totem spell.
        /// </summary>
        //Totem
        private const string WindfuryTotemSpell = "Windfury Totem";

        /// <summary>
        /// Constant representing the name of the spell "Wind Shear".
        /// </summary>
        private const string windShearSpell = "Wind Shear";

        /// <summary>
        /// Constructor for the RestorationShaman class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for communication with the bot.</param>
        public RestorationShaman(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Race
            //spellCoolDown.Add(giftOfTheNaaruSpell, DateTime.Now);

            //Spells / DMG
            spellCoolDown.Add(LightningBoltSpell, DateTime.Now);
            spellCoolDown.Add(flameShockSpell, DateTime.Now);
            spellCoolDown.Add(earthShockSpell, DateTime.Now);

            //Spells
            spellCoolDown.Add(healingWaveSpell, DateTime.Now);
            spellCoolDown.Add(lesserHealingWaveSpell, DateTime.Now);
            spellCoolDown.Add(riptideSpell, DateTime.Now);
            spellCoolDown.Add(watershieldSpell, DateTime.Now);
            spellCoolDown.Add(LightningShieldSpell, DateTime.Now);
            spellCoolDown.Add(chainHealSpell, DateTime.Now);
            spellCoolDown.Add(earthlivingBuff, DateTime.Now);
            spellCoolDown.Add(earthlivingWeaponSpell, DateTime.Now);
            spellCoolDown.Add(windShearSpell, DateTime.Now);

            //CD|Buffs
            spellCoolDown.Add(naturesswiftSpell, DateTime.Now);
            spellCoolDown.Add(heroismSpell, DateTime.Now);
            spellCoolDown.Add(Bloodlust, DateTime.Now);
            spellCoolDown.Add(tidalForceSpell, DateTime.Now);
            spellCoolDown.Add(earthShieldSpell, DateTime.Now);

            //Totem
            spellCoolDown.Add(WindfuryTotemSpell, DateTime.Now);
            spellCoolDown.Add(StrengthofEarthTotemSpell, DateTime.Now);
            spellCoolDown.Add(ManaSpringTotemSpell, DateTime.Now);
            spellCoolDown.Add(ManaTideTotemSpell, DateTime.Now);
            spellCoolDown.Add(CalloftheElementsSpell, DateTime.Now);

            //Time event
            EarthShieldEvent = new(TimeSpan.FromSeconds(7));
            ManaTideTotemEvent = new(TimeSpan.FromSeconds(12));
            TotemcastEvent = new(TimeSpan.FromSeconds(4));
        }

        /// <summary>
        /// Gets the author name of the code.
        /// </summary>
        /// <returns>The name of the author, which is Lukas.</returns>
        public override string Author => "Lukas";

        /// <summary>
        /// Gets or sets the dictionary of string keys and dynamic values.
        /// </summary>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the Resto Shaman.
        /// </summary>
        /// <returns>
        /// The description of the Resto Shaman as a string.
        /// </returns>
        public override string Description => "Resto Shaman";

        /// <summary>
        /// Gets the display name for the Shaman Restoration class.
        /// </summary>
        public override string DisplayName => "Shaman Restoration";

        /// <summary>
        /// Represents a time event for Earth Shield.
        /// </summary>
        //Time event
        public TimegatedEvent EarthShieldEvent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this component handles movement.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the character is a melee fighter.
        /// </summary>
        /// <returns>False, indicating the character is not a melee fighter.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the current object.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the TimegatedEvent for the ManaTideTotem event.
        /// </summary>
        public TimegatedEvent ManaTideTotemEvent { get; private set; }

        /// <summary>
        /// Gets the role of the character as a healer.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// <summary>
        /// Gets the TalentTree object containing the talent tree configuration.
        /// </summary>
        /// <returns>The TalentTree object.</returns>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 5, new(3, 5, 5) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 3) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 3) },
                { 10, new(3, 10, 3) },
                { 11, new(3, 11, 5) },
                { 12, new(3, 12, 3) },
                { 13, new(3, 13, 1) },
                { 15, new(3, 15, 5) },
                { 17, new(3, 17, 1) },
                { 19, new(3, 19, 2) },
                { 20, new(3, 20, 2) },
                { 21, new(3, 21, 3) },
                { 22, new(3, 22, 3) },
                { 23, new(3, 23, 1) },
                { 24, new(3, 24, 2) },
                { 25, new(3, 25, 5) },
                { 26, new(3, 26, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the target is in range.
        /// </summary>
        public bool TargetIsInRange { get; set; }

        /// <summary>
        /// Represents a timegated event for casting a totem.
        /// </summary>
        public TimegatedEvent TotemcastEvent { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character should use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets or sets a value indicating whether the spell can only be used during combat.
        /// </summary>
        public bool UseSpellOnlyInCombat { get; private set; }

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public override string Version => "2.1";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind an enemy.
        /// </summary>
        /// <returns>
        ///   <c>false</c> as the character cannot walk behind an enemy. 
        /// </returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass of this character as Shaman.
        /// </summary>
        public override WowClass WowClass => WowClass.Shaman;

        /// <summary>
        /// Executes the CC ability. Sets the flag to use spells only in combat, then performs shield and starts healing.
        /// </summary>
        public override void ExecuteCC()
        {
            UseSpellOnlyInCombat = true;
            Shield();
            StartHeal();
        }

        /// <summary>
        /// Executes the specified actions when the player character is out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            RevivePartyMember(ancestralSpiritSpell);

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, earthlivingBuff, earthlivingWeaponSpell))
            {
                return;
            }
            UseSpellOnlyInCombat = false;
            Shield();
            StartHeal();
        }

        /// <summary>
        /// Activates the Water Shield aura for the bot's player character if it is not already active, and there is sufficient mana to cast the associated spell.
        /// </summary>
        private void Shield()
        {
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Water Shield") && CustomCastSpellMana(watershieldSpell))
            {
                return;
            }
        }

        /// <summary>
        /// Method to start healing process.
        /// </summary>
        private void StartHeal()
        {
            // List<IWowUnit> partyMemberToHeal = Bot.ObjectManager.Partymembers.Where(e =>
            // e.HealthPercentage <= 94 && !e.IsDead).OrderBy(e =>
            // e.HealthPercentage).ToList();//FirstOrDefault => tolist

            List<IWowUnit> partyMemberToHeal = new(Bot.Objects.Partymembers)
            {
                //healableUnits.AddRange(Bot.ObjectManager.PartyPets);
                Bot.Player
            };

            partyMemberToHeal = partyMemberToHeal.Where(e => e.HealthPercentage <= 94 && !e.IsDead).OrderBy(e => e.HealthPercentage).ToList();

            if (partyMemberToHeal.Count > 0)
            {
                if (Bot.Wow.TargetGuid != partyMemberToHeal.FirstOrDefault().Guid)
                {
                    Bot.Wow.ChangeTarget(partyMemberToHeal.FirstOrDefault().Guid);
                }

                if (Bot.Wow.TargetGuid != 0 && Bot.Target != null)
                {
                    TargetIsInRange = Bot.Player.Position.GetDistance(Bot.GetWowObjectByGuid<IWowUnit>(partyMemberToHeal.FirstOrDefault().Guid).Position) <= 30;
                    if (TargetIsInRange)
                    {
                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                        if (Bot.Movement.Status != Movement.Enums.MovementAction.None)
                        {
                            Bot.Wow.StopClickToMove();
                            Bot.Movement.Reset();
                        }

                        if (Bot.Target != null && Bot.Target.HealthPercentage >= 90)
                        {
                            Bot.Wow.LuaDoString("SpellStopCasting()");
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Player.HealthPercentage < 20 && CustomCastSpellMana(heroismSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Target.HealthPercentage < 20 && CustomCastSpellMana(naturesswiftSpell) && CustomCastSpellMana(healingWaveSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Target.HealthPercentage < 40 && CustomCastSpellMana(tidalForceSpell))
                        {
                            return;
                        }

                        //if (partyMemberToHeal.Count >= 3 && Bot.Target.HealthPercentage < 40 && CustomCastSpell(Bloodlust))
                        //{
                        //    return;
                        //}
                        //Race Draenei
                        if (Bot.Player.Race == WowRace.Draenei && Bot.Target.HealthPercentage < 50 && CustomCastSpellMana(giftOfTheNaaruSpell))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage <= 50 && CustomCastSpellMana(healingWaveSpell))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage <= 75 && CustomCastSpellMana(lesserHealingWaveSpell))
                        {
                            return;
                        }

                        if (partyMemberToHeal.Count >= 4 && Bot.Target.HealthPercentage >= 80 && CustomCastSpellMana(chainHealSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && EarthShieldEvent.Run() && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Earth Shield") && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Water Shield") && Bot.Target.HealthPercentage < 90 && CustomCastSpellMana(earthShieldSpell))
                        {
                            return;
                        }

                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Riptide") && Bot.Target.HealthPercentage < 90 && CustomCastSpellMana(riptideSpell))
                        {
                            return;
                        }
                    }

                    if (TotemcastEvent.Run() && TotemItemCheck())
                    {
                        if (Bot.Player.ManaPercentage <= 10 && CustomCastSpellMana(ManaTideTotemSpell))
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                TotemItemCheck();

                if (TotemcastEvent.Run() && TotemItemCheck())
                {
                    if (Bot.Player.ManaPercentage >= 50
                        && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Windfury Totem")
                        && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Stoneskin")
                        && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Flametongue Totem")
                        && CustomCastSpellMana(CalloftheElementsSpell))
                    {
                        return;
                    }
                }

                if (TargetSelectEvent.Run())
                {
                    IWowUnit nearTarget = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 30)
                    .Where(e => e.IsInCombat && !e.IsNotAttackable && e.IsCasting && Bot.Db.GetUnitName(Bot.Target, out string name) && name != "The Lich King" && !(Bot.Objects.MapId == WowMapId.DrakTharonKeep && e.CurrentlyChannelingSpellId == 47346))
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();

                    if (Bot.Wow.TargetGuid != 0 && Bot.Target != null && nearTarget != null)
                    {
                        Bot.Wow.ChangeTarget(nearTarget.Guid);

                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                        if (Bot.Movement.Status != Movement.Enums.MovementAction.None)
                        {
                            Bot.Wow.StopClickToMove();
                            Bot.Movement.Reset();
                        }
                        if (UseSpellOnlyInCombat && Bot.Target.IsCasting && CustomCastSpellMana(windShearSpell))
                        {
                            return;
                        }
                        if (UseSpellOnlyInCombat && Bot.Player.ManaPercentage >= 80 && CustomCastSpellMana(flameShockSpell))
                        {
                            return;
                        }
                        //if (UseSpellOnlyInCombat && Bot.Player.ManaPercentage >= 90 && CustomCastSpell(earthShockSpell))
                        //{
                        //    return;
                        //}
                    }
                }
            }
        }
    }
}