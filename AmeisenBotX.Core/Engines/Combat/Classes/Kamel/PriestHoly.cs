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
    /// <summary>
    /// Represents a holy priest specialization in World of Warcraft.
    /// </summary>
    internal class PriestHoly : BasicKamelClass
    {
        /// <summary>
        /// Represents the name of the Circle of Healing spell.
        /// </summary>
        private const string CircleOfHealingSpell = "Circle of Healing";

        /// <summary>
        /// Represents the name of the "Desperate Prayer" spell.
        /// </summary>
        private const string DesperatePrayerSpell = "Desperate Prayer";

        /// <summary>
        /// The name of the Divine Hymn spell.
        /// </summary>
        private const string DivineHymnSpell = "Divine Hymn";

        /// <summary>
        /// The constant string representing the name of the "Divine Spirit" spell.
        /// </summary>
        //Buffs
        private const string DivineSpiritSpell = "Divine Spirit";

        /// <summary>
        /// The spell "Every Man for Himself" grants the user the ability to escape from any root, snare, or stun effect by removing them instantly.
        /// </summary>
        //Spells Race
        private const string EveryManforHimselfSpell = "Every Man for Himself";

        /// <summary>
        /// The name of the fade spell.
        /// </summary>
        private const string FadeSpell = "Fade";

        /// <summary>
        /// The name of the Fear Ward spell.
        /// </summary>
        private const string FearWardSpell = "Fear Ward";

        /// <summary>
        /// The constant string representing the Flash Heal spell.
        /// </summary>
        private const string FlashHealSpell = "Flash Heal";

        /// <summary>
        /// The name of the greater heal spell.
        /// </summary>
        private const string GreaterHealSpell = "Greater Heal";

        /// <summary>
        /// Represents the name of the "Guardian Spirit" spell.
        /// </summary>
        private const string GuardianSpiritSpell = "Guardian Spirit";

        /// <summary>
        /// Represents the name of the Holy Fire spell.
        /// </summary>
        private const string HolyFireSpell = "Holy Fire";

        /// <summary>
        /// The constant string representing the spell "Hymn of Hope".
        /// </summary>
        private const string HymnofHopeSpell = "Hymn of Hope";

        ///<summary>
        /// Represents the constant string value of "Inner Fire".
        ///</summary>
        private const string InnerFireSpell = "Inner Fire";

        /// <summary>
        /// Represents the spell "Power Word: Fortitude".
        /// </summary>
        private const string PowerWordFortitudeSpell = "Power Word: Fortitude";

        /// <summary>
        /// Represents the power word shield spell.
        /// </summary>
        private const string PowerWordShieldSpell = "Power Word: Shield";

        /// <summary>
        /// The name of the prayer of fortitude.
        /// </summary>
        private const string PrayerofFortitude = "Prayer of Fortitude";

        /// <summary>
        /// The name of the spell "Prayer of Healing".
        /// </summary>
        private const string PrayerofHealingSpell = "Prayer of Healing";

        /// <summary>
        /// Represents the name of the "Prayer of Mending" spell.
        /// </summary>
        private const string PrayerofMendingSpell = "Prayer of Mending";

        /// <summary>
        /// The constant string representing the prayer of shadow protection.
        /// </summary>
        private const string PrayerofShadowProtection = "Prayer of Shadow Protection";

        /// <summary>
        /// The name of the spell to renew.
        /// </summary>
        private const string RenewSpell = "Renew";

        ///<summary>
        ///Constant string representing the name of the shadow protection spell.
        ///</summary>
        private const string ShadowProtectionSpell = "Shadow Protection";

        /// <summary>
        /// The name of the spell is "Smite".
        /// </summary>
        //Spells / dmg
        private const string SmiteSpell = "Smite";

        ///<summary>
        ///Constructor for the PriestHoly class.
        ///Initializes the spell cooldowns for the PriestHoly class.
        ///</summary>
        public PriestHoly(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Spells Race
            //spellCoolDown.Add(EveryManforHimselfSpell, DateTime.Now);

            //Spells / dmg
            spellCoolDown.Add(SmiteSpell, DateTime.Now);
            spellCoolDown.Add(HolyFireSpell, DateTime.Now);

            //Spells
            spellCoolDown.Add(RenewSpell, DateTime.Now);
            spellCoolDown.Add(FlashHealSpell, DateTime.Now);
            spellCoolDown.Add(GreaterHealSpell, DateTime.Now);
            spellCoolDown.Add(PowerWordShieldSpell, DateTime.Now);
            spellCoolDown.Add(CircleOfHealingSpell, DateTime.Now);
            spellCoolDown.Add(DesperatePrayerSpell, DateTime.Now);
            spellCoolDown.Add(FadeSpell, DateTime.Now);
            spellCoolDown.Add(PrayerofHealingSpell, DateTime.Now);
            spellCoolDown.Add(PrayerofMendingSpell, DateTime.Now);
            spellCoolDown.Add(GuardianSpiritSpell, DateTime.Now);
            spellCoolDown.Add(HymnofHopeSpell, DateTime.Now);
            spellCoolDown.Add(DivineHymnSpell, DateTime.Now);

            //Buffs
            spellCoolDown.Add(DivineSpiritSpell, DateTime.Now);
            spellCoolDown.Add(InnerFireSpell, DateTime.Now);
            spellCoolDown.Add(FearWardSpell, DateTime.Now);
            spellCoolDown.Add(PowerWordFortitudeSpell, DateTime.Now);
            spellCoolDown.Add(ShadowProtectionSpell, DateTime.Now);
            spellCoolDown.Add(PrayerofFortitude, DateTime.Now);
            spellCoolDown.Add(PrayerofShadowProtection, DateTime.Now);
        }

        /// <summary>
        /// Gets or sets the author of the code.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public override string Author => "Lukas";

        /// <summary>
        /// Gets or sets the dictionary of strings and dynamic values.
        /// </summary>
        /// <returns>A dictionary of strings and dynamic values.</returns>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the Priest Holy.
        /// </summary>
        /// <returns>
        /// The description string "Priest Holy".
        /// </returns>
        public override string Description => "Priest Holy";

        /// <summary>
        /// Gets the display name of the Priest Holy.
        /// </summary>
        public override string DisplayName => "Priest Holy";

        /// <summary>
        /// Gets a value indicating whether or not this code handles movement.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this object is a melee attack.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the ItemComparator.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets the role of this class, which is "Heal".
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// <summary>
        /// Gets or sets the talent tree.
        /// </summary>
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
        /// Gets or sets a value indicating whether auto attacks should be used.
        /// By default, auto attacks are disabled.
        /// </summary>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets or sets a value indicating whether the spell can only be used in combat.
        /// </summary>
        public bool UseSpellOnlyInCombat { get; private set; }

        /// <summary>
        /// Gets the version of the code, which is always "1.0".
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind enemies.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property, which represents the class of a character in World of Warcraft.
        /// The current class is set to Priest.
        /// </summary>
        public override WowClass WowClass => WowClass.Priest;

        /// <summary>
        /// Executes the command 'ExecuteCC'.
        /// Sets 'UseSpellOnlyInCombat' to true.
        /// Calls the 'BuffManager' method.
        /// Calls the 'StartHeal' method.
        /// </summary>
        public override void ExecuteCC()
        {
            UseSpellOnlyInCombat = true;
            BuffManager();
            StartHeal();
        }

        ///<summary>
        ///Executes the specified actions when the character is out of combat.
        ///</summary>
        public override void OutOfCombatExecute()
        {
            RevivePartyMember(resurrectionSpell);
            UseSpellOnlyInCombat = false;
            BuffManager();
            StartHeal();
        }

        /// <summary>
        /// Manages casting buffs on party members.
        /// </summary>
        private void BuffManager()
        {
            if (TargetSelectEvent.Run())
            {
                List<IWowUnit> CastBuff = new(Bot.Objects.Partymembers)
                {
                    Bot.Player
                };

                CastBuff = CastBuff.Where(e => (!e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Prayer of Fortitude") || !e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Prayer of Shadow Protection")) && !e.IsDead).OrderBy(e => e.HealthPercentage).ToList();

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
                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Prayer of Fortitude") && CustomCastSpell(PrayerofFortitude))
                        {
                            return;
                        }
                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Prayer of Shadow Protection") && CustomCastSpell(PrayerofShadowProtection))
                        {
                            return;
                        }
                    }
                }
            }
            //if ((!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Power Word: Fortitude") || !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Power Word: Fortitude")) && CustomCastSpell(PowerWordFortitudeSpell))
            //{
            //    return;
            //}
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Divine Spirit") && CustomCastSpell(DivineSpiritSpell, true))
            {
                return;
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Inner Fire") && CustomCastSpell(InnerFireSpell, true))
            {
                return;
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Fear Ward") && CustomCastSpell(FearWardSpell, true))
            {
                return;
            }
            //if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Shadow Protection") && CustomCastSpell(ShadowProtectionSpell))
            //{
            //    return;
            //}
        }

        /// <summary>
        /// Custom method to cast a spell with the specified name.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="castOnSelf">Optional parameter to determine if the spell should be cast on self. Default is false.</param>
        /// <returns>Returns true if the spell was successfully cast, otherwise false.</returns>
        private bool CustomCastSpell(string spellName, bool castOnSelf = false)
        {
            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
            {
                if (Bot.Target != null)
                {
                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if ((Bot.Player.Mana >= spell.Costs && IsSpellReady(spellName)))
                    {
                        double distance = Bot.Player.Position.GetDistance(Bot.Target.Position);

                        if ((spell.MinRange == 0 && spell.MaxRange == 0) || (spell.MinRange <= distance && spell.MaxRange >= distance))
                        {
                            Bot.Wow.CastSpell(spellName, castOnSelf);
                            return true;
                        }
                    }
                }
                else
                {
                    Bot.Wow.ChangeTarget(Bot.Wow.PlayerGuid);

                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if ((Bot.Player.Mana >= spell.Costs && IsSpellReady(spellName)))
                    {
                        Bot.Wow.CastSpell(spellName);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Begins the healing process by selecting party members to heal and prioritizing them based on health percentage. 
        /// Checks if the current target is a party member to heal and changes target if necessary. 
        /// Ensures the target is within range and in line of sight before casting any spells. 
        /// Considers various conditions and cast spells accordingly to heal party members efficiently. 
        /// If no party members to heal, it checks for nearby enemies to attack. 
        /// </summary>
        private void StartHeal()
        {
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

                        if (UseSpellOnlyInCombat && (Bot.Player.IsConfused || Bot.Player.IsSilenced || Bot.Player.IsDazed) && CustomCastSpell(EveryManforHimselfSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Player.ManaPercentage <= 20 && CustomCastSpell(HymnofHopeSpell))
                        {
                            return;
                        }

                        if (partyMemberToHeal.Count >= 5 && Bot.Target.HealthPercentage < 50 && CustomCastSpell(DivineHymnSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Player.HealthPercentage < 50 && CustomCastSpell(FadeSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Target.HealthPercentage < 30 && CustomCastSpell(GuardianSpiritSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Target.HealthPercentage < 30 && CustomCastSpell(DesperatePrayerSpell))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage < 55 && CustomCastSpell(GreaterHealSpell))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage < 80 && CustomCastSpell(FlashHealSpell))
                        {
                            return;
                        }

                        if (partyMemberToHeal.Count >= 3 && Bot.Target.HealthPercentage < 80 && CustomCastSpell(CircleOfHealingSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && partyMemberToHeal.Count >= 2 && Bot.Target.HealthPercentage < 80 && CustomCastSpell(PrayerofMendingSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && partyMemberToHeal.Count >= 3 && Bot.Target.HealthPercentage < 80 && CustomCastSpell(PrayerofHealingSpell))
                        {
                            return;
                        }

                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Renew") && Bot.Target.HealthPercentage < 90 && CustomCastSpell(RenewSpell))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Weakened Soul") && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Power Word: Shield") && Bot.Target.HealthPercentage < 90 && CustomCastSpell(PowerWordShieldSpell))
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                if (TargetSelectEvent.Run())
                {
                    IWowUnit nearTarget = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 30)
                    .Where(e => e.IsInCombat && !e.IsNotAttackable && Bot.Db.GetUnitName(e, out string name) && name != "The Lich King" && !(Bot.Objects.MapId == WowMapId.DrakTharonKeep && e.CurrentlyChannelingSpellId == 47346))//&& e.IsCasting
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
                        if (UseSpellOnlyInCombat && Bot.Player.ManaPercentage >= 80 && CustomCastSpell(HolyFireSpell))
                        {
                            return;
                        }
                        if (UseSpellOnlyInCombat && Bot.Player.ManaPercentage >= 80 && CustomCastSpell(SmiteSpell))
                        {
                            return;
                        }
                    }
                }
                //target gui id is bigger than null
                //{
                //Bot.NewBot.ClearTarget();
                //return;
                //}
                //Attacken
            }
        }
    }
}