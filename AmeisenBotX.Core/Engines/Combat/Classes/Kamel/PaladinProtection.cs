using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    internal class PaladinProtection : BasicKamelClass
    {
        /// <summary>
        /// The name of the spell "Avenger's Shield".
        /// </summary>
        //Spell
        private const string avengersShieldSpell = "Avenger's Shield";

        /// <summary>
        /// The name of the Avenging Wrath spell.
        /// </summary>
        private const string AvengingWrathSpell = "Avenging Wrath";

        /// <summary>
        /// The name of the spell "Blessing of Kings".
        /// </summary>
        //Buff
        private const string blessingofKingsSpell = "Blessing of Kings";

        /// <summary>
        /// The name of the consecration spell.
        /// </summary>
        private const string consecrationSpell = "Consecration";

        /// <summary>
        /// The name of the devotion aura spell.
        /// </summary>
        private const string devotionAuraSpell = "Devotion Aura";

        /// <summary>
        /// The constant representing the spell called "Divine Plea".
        /// </summary>
        private const string DivinePleaSpell = "Divine Plea";

        /// <summary>
        /// The name of the Divine Protection spell.
        /// </summary>
        private const string divineProtectionSpell = "Divine Protection";

        /// <summary>
        /// Represents the <see cref="EveryManforHimselfSpell"/> spell, which allows a player to break free from any crowd control effects and regain control of their character.
        /// </summary>
        //Spells Race
        private const string EveryManforHimselfSpell = "Every Man for Himself";

        /// <summary>
        /// The constant string representing the exorcism spell.
        /// </summary>
        private const string exorcismSpell = "Exorcism";
        /// <summary>
        /// Represents the constant string for the "Hammer of Justice" spell.
        /// </summary>
        private const string hammerofJusticeSpell = "Hammer of Justice";
        /// <summary>
        /// The spell name for Hammer of the Righteous.
        /// </summary>
        private const string hammeroftheRighteousSpell = "Hammer of the Righteous";
        /// <summary>
        /// The name of the spell "Hammer of Wrath".
        /// </summary>
        private const string hammerofWrathSpell = "Hammer of Wrath";
        /// <summary>
        /// Represents the constant string for the "Hand of Reckoning" spell.
        /// </summary>
        private const string handofReckoningSpell = "Hand of Reckoning";
        /// <summary>
        /// Represents the name of the Holy Light spell.
        /// </summary>
        private const string holyLightSpell = "Holy Light";
        /// <summary>
        /// Represents the name of the Holy Shield spell.
        /// </summary>
        private const string holyShieldSpell = "Holy Shield";
        /// <summary>
        /// Represents the name of the "Judgement of Light" spell.
        /// </summary>
        private const string judgementofLightSpell = "Judgement of Light";
        /// <summary>
        /// The name of the spell "Lay on Hands".
        /// </summary>
        private const string layonHandsSpell = "Lay on Hands";
        /// <summary>
        /// The name of the constant string representing the "Righteous Fury" spell.
        /// </summary>
        private const string righteousFurySpell = "Righteous Fury";
        /// <summary>
        /// Represents the name of the "Sacred Shield" spell.
        /// </summary>
        private const string SacredShieldSpell = "Sacred Shield";
        /// <summary>
        /// The constant value representing the spell "Seal of Light".
        /// </summary>
        private const string sealofLightSpell = "Seal of Light";
        /// <summary>
        /// The constant string for the Seal of Wisdom spell.
        /// </summary>
        private const string sealofWisdomSpell = "Seal of Wisdom";

        /// <summary>
        /// Constructor for the PaladinProtection class.
        /// Initializes the PaladinProtection object with the specified bot.
        /// </summary>
        /// <param name="bot">The bot used for the PaladinProtection instance.</param>
        public PaladinProtection(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Spells Race
            //spellCoolDown.Add(EveryManforHimselfSpell, DateTime.Now);

            //Spell
            spellCoolDown.Add(avengersShieldSpell, DateTime.Now);
            spellCoolDown.Add(consecrationSpell, DateTime.Now);
            spellCoolDown.Add(judgementofLightSpell, DateTime.Now);
            spellCoolDown.Add(holyShieldSpell, DateTime.Now);
            spellCoolDown.Add(hammeroftheRighteousSpell, DateTime.Now);
            spellCoolDown.Add(hammerofWrathSpell, DateTime.Now);
            spellCoolDown.Add(exorcismSpell, DateTime.Now);
            spellCoolDown.Add(divineProtectionSpell, DateTime.Now);
            spellCoolDown.Add(handofReckoningSpell, DateTime.Now);
            spellCoolDown.Add(hammerofJusticeSpell, DateTime.Now);
            spellCoolDown.Add(layonHandsSpell, DateTime.Now);
            spellCoolDown.Add(holyLightSpell, DateTime.Now);
            spellCoolDown.Add(AvengingWrathSpell, DateTime.Now);
            spellCoolDown.Add(DivinePleaSpell, DateTime.Now);
            spellCoolDown.Add(SacredShieldSpell, DateTime.Now);

            //Buff
            spellCoolDown.Add(blessingofKingsSpell, DateTime.Now);
            spellCoolDown.Add(sealofLightSpell, DateTime.Now);
            spellCoolDown.Add(sealofWisdomSpell, DateTime.Now);
            spellCoolDown.Add(devotionAuraSpell, DateTime.Now);
            spellCoolDown.Add(righteousFurySpell, DateTime.Now);

            //Time event
            ShieldEvent = new(TimeSpan.FromSeconds(8));
        }

        /// <summary>
        /// Gets the author's name.
        /// </summary>
        /// <returns>The author's name, which is "Lukas".</returns>
        public override string Author => "Lukas";

        /// <summary>
        /// Gets or sets the Dictionary C.
        /// </summary>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the Paladin Protection 1.0.
        /// </summary>
        public override string Description => "Paladin Protection 1.0";

        /// <summary>
        /// Gets or sets the display name for a Paladin Protection.
        /// </summary>
        public override string DisplayName => "Paladin Protection";

        /// <summary>
        /// Gets or sets the TimegatedEvent that can be executed.
        /// </summary>
        public TimegatedEvent ExecuteEvent { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> if this instance does not handle movement; otherwise, <c>true</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is considered melee.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the object is considered melee; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for the character.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStaminaComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger });

        /// <summary>
        /// Gets or sets the role for the Wow character, which is a Tank.
        /// </summary>
        public override WowRole Role => WowRole.Tank;

        /// <summary>
        /// Gets or sets the TimegatedEvent for the ShieldEvent.
        /// </summary>
        public TimegatedEvent ShieldEvent { get; private set; }

        /// <summary>
        /// Gets or sets the talent tree for the character.
        /// </summary>
        /// <value>
        /// The talent tree contains multiple trees, including Tree1, Tree2, and Tree3. Each tree consists of various talents, identified by their respective IDs and associated with their corresponding abilities.
        /// Tree1 contains no talents.
        /// Tree2 contains the following talents:
        /// - Talent ID 2: Ability [2, 2, 5]
        /// - Talent ID 5: Ability [2, 5, 5]
        /// - Talent ID 6: Ability [2, 6, 1]
        /// - Talent ID 7: Ability [2, 7, 3]
        /// - Talent ID 8: Ability [2, 8, 5]
        /// - Talent ID 9: Ability [2, 9, 2]
        /// - Talent ID 11: Ability [2, 11, 3]
        /// - Talent ID 12: Ability [2, 12, 1]
        /// - Talent ID 14: Ability [2, 14, 2]
        /// - Talent ID 15: Ability [2, 15, 3]
        /// - Talent ID 16: Ability [2, 16, 1]
        /// - Talent ID 17: Ability [2, 17, 1]
        /// - Talent ID 18: Ability [2, 18, 3]
        /// - Talent ID 19: Ability [2, 19, 3]
        /// - Talent ID 20: Ability [2, 20, 3]
        /// - Talent ID 21: Ability [2, 21, 3]
        /// - Talent ID 22: Ability [2, 22, 1]
        /// - Talent ID 23: Ability [2, 23, 2]
        /// - Talent ID 24: Ability [2, 24, 3]
        /// - Talent ID 25: Ability [2, 25, 2]
        /// - Talent ID 26: Ability [2, 26, 1]
        /// Tree3 contains the following talents:
        /// - Talent ID 1: Ability [3, 1, 5]
        /// - Talent ID 3: Ability [3, 3, 2]
        /// - Talent ID 4: Ability [3, 4, 3]
        /// - Talent ID 7: Ability [3, 7, 5]
        /// - Talent ID 12: Ability [3, 12, 3]
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new(),
            Tree2 = new()
            {
                { 2, new(2, 2, 5) },
                { 5, new(2, 5, 5) },
                { 6, new(2, 6, 1) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 5) },
                { 9, new(2, 9, 2) },
                { 11, new(2, 11, 3) },
                { 12, new(2, 12, 1) },
                { 14, new(2, 14, 2) },
                { 15, new(2, 15, 3) },
                { 16, new(2, 16, 1) },
                { 17, new(2, 17, 1) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 3) },
                { 20, new(2, 20, 3) },
                { 21, new(2, 21, 3) },
                { 22, new(2, 22, 1) },
                { 23, new(2, 23, 2) },
                { 24, new(2, 24, 3) },
                { 25, new(2, 25, 2) },
                { 26, new(2, 26, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 3, new(3, 3, 2) },
                { 4, new(3, 4, 3) },
                { 7, new(3, 7, 5) },
                { 12, new(3, 12, 3) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether this instance uses auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance uses auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code. The version is set to "1.0".
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind an enemy. Returns false.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the wow class of the object.
        /// </summary>
        /// <value>The wow class.</value>
        public override WowClass WowClass => WowClass.Paladin;

        /// <summary>
        /// Executes the CC attack.
        /// </summary>
        public override void ExecuteCC()
        {
            StartAttack();
        }

        /// <summary>
        /// Executes actions when out of combat,
        /// reviving a party member with the redemption spell,
        /// managing buffs,
        /// selecting the tank as the target,
        /// and starting the attack.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            RevivePartyMember(redemptionSpell);
            BuffManager();
            TargetselectionTank();
            StartAttack();
        }

        /// <summary>
        /// Executes the BuffManager routine to apply buffs to party members.
        /// </summary>
        private void BuffManager()
        {
            if (TargetSelectEvent.Run())
            {
                List<IWowUnit> CastBuff = new(Bot.Objects.Partymembers)
                {
                    Bot.Player
                };

                CastBuff = CastBuff.Where(e => !e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Blessing of Kings") && !e.IsDead).OrderBy(e => e.HealthPercentage).ToList();

                if (CastBuff != null)
                {
                    if (CastBuff.Count > 0)
                    {
                        if (Bot.Wow.TargetGuid != CastBuff.FirstOrDefault().Guid)
                        {
                            Bot.Wow.ChangeTarget(CastBuff.FirstOrDefault().Guid);
                        }
                    }
                    if (Bot.Wow.TargetGuid != 0 && Bot.Target != null)
                    {
                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Blessing of Kings") && CustomCastSpell(blessingofKingsSpell))
                        {
                            return;
                        }
                    }
                }
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Seal of Wisdom") && CustomCastSpell(sealofWisdomSpell))
            {
                return;
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Devotion Aura") && CustomCastSpell(devotionAuraSpell))
            {
                return;
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Righteous Fury") && CustomCastSpell(righteousFurySpell))
            {
                return;
            }
        }

        /// <summary>
        /// Checks if the bot can cast a specified spell and casts it if conditions are met.
        /// Returns true if the spell was successfully cast, false otherwise.
        /// </summary>
        /// <param name="spellName">The name of the spell to be cast.</param>
        /// <returns>True if the spell was cast, false otherwise.</returns>
        private bool CustomCastSpell(string spellName)
        {
            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
            {
                if (Bot.Target != null)
                {
                    double distance = Bot.Player.Position.GetDistance(Bot.Target.Position);
                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if ((Bot.Player.Mana >= spell.Costs && IsSpellReady(spellName)))
                    {
                        if ((spell.MinRange == 0 && spell.MaxRange == 0) || (spell.MinRange <= distance && spell.MaxRange >= distance))
                        {
                            Bot.Wow.CastSpell(spellName);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        ///<summary>
        /// Method to initiate an attack.
        ///</summary>
        private void StartAttack()
        {
            // IWowUnit wowUnit =
            // Bot.ObjectManager.GetClosestWowUnitByDisplayId(AnubRhekanDisplayId, false);

            if (Bot.Wow.TargetGuid != 0)
            {
                if (Bot.Wow.TargetGuid != Bot.Wow.PlayerGuid)
                {
                    TargetselectionTank();
                }

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

                    if ((Bot.Player.IsConfused || Bot.Player.IsSilenced || Bot.Player.IsDazed) && CustomCastSpell(EveryManforHimselfSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(AvengingWrathSpell))
                    {
                        return;
                    }

                    if (Bot.Player.ManaPercentage <= 20 && CustomCastSpell(DivinePleaSpell))
                    {
                        return;
                    }

                    if (ShieldEvent.Run() && CustomCastSpell(SacredShieldSpell))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 15 && CustomCastSpell(layonHandsSpell))
                    {
                        return;
                    }
                    if (Bot.Player.HealthPercentage <= 25 && CustomCastSpell(holyLightSpell))
                    {
                        return;
                    }
                    if (Bot.Player.HealthPercentage <= 50 && CustomCastSpell(divineProtectionSpell))
                    {
                        return;
                    }
                    if (Bot.Target.HealthPercentage <= 20 && CustomCastSpell(hammerofWrathSpell))
                    {
                        return;
                    }
                    if ((Bot.Target.HealthPercentage <= 20 || Bot.Player.HealthPercentage <= 30 || Bot.Target.IsCasting) && CustomCastSpell(hammerofJusticeSpell))
                    {
                        return;
                    }
                    if (Bot.Db.GetUnitName(Bot.Target, out string name) && name != "Anub'Rekhan" && CustomCastSpell(handofReckoningSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(avengersShieldSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(consecrationSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(judgementofLightSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(holyShieldSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(exorcismSpell))
                    {
                        return;
                    }
                }
                else//Range
                {
                    if (CustomCastSpell(avengersShieldSpell))
                    {
                        return;
                    }
                }
            }
            else
            {
                TargetselectionTank();
            }
        }
    }
}