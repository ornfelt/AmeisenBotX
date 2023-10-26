using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Combat.Helpers.Healing;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class PaladinHoly : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the PaladinHoly class with the specified bot.
        /// </summary>
        /// <param name="bot">The bot instance to use.</param>
        public PaladinHoly(AmeisenBotInterfaces bot) : base(bot)
        {
            Configurables.TryAdd("AttackInGroups", true);
            Configurables.TryAdd("AttackInGroupsUntilManaPercent", 85.0);
            Configurables.TryAdd("AttackInGroupsCloseCombat", false);
            Configurables.TryAdd("BeaconOfLightSelfHealth", 85.0);
            Configurables.TryAdd("BeaconOfLightPartyHealth", 85.0);
            Configurables.TryAdd("DivinePleaMana", 60.0);
            Configurables.TryAdd("DivineIlluminationManaAbove", 20.0);
            Configurables.TryAdd("DivineIlluminationManaUntil", 50.0);

            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.BlessingOfWisdom, () => TryCastSpell(Paladin335a.BlessingOfWisdom, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.DevotionAura, () => TryCastSpell(Paladin335a.DevotionAura, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.SealOfWisdom, () => Bot.Character.SpellBook.IsSpellKnown(Paladin335a.SealOfWisdom) && TryCastSpell(Paladin335a.SealOfWisdom, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Paladin335a.SealOfVengeance, () => !Bot.Character.SpellBook.IsSpellKnown(Paladin335a.SealOfWisdom) && TryCastSpell(Paladin335a.SealOfVengeance, Bot.Wow.PlayerGuid, true)));

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Paladin335a.BlessingOfWisdom, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            HealingManager = new(bot, (string spellName, ulong guid) => { return TryCastSpell(spellName, guid); });

            // make sure all new spells get added to the healing manager
            Bot.Character.SpellBook.OnSpellBookUpdate += () =>
            {
                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin335a.FlashOfLight, out Spell spellFlashOfLight))
                {
                    HealingManager.AddSpell(spellFlashOfLight);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin335a.HolyLight, out Spell spellHolyLight))
                {
                    HealingManager.AddSpell(spellHolyLight);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin335a.HolyShock, out Spell spellHolyShock))
                {
                    HealingManager.AddSpell(spellHolyShock);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin335a.LayOnHands, out Spell spellLayOnHands))
                {
                    HealingManager.AddSpell(spellLayOnHands);
                }
            };

            SpellAbortFunctions.Add(HealingManager.ShouldAbortCasting);

            ChangeBeaconEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Represents a Half-Smart CombatClass for the Holy Paladin spec.
        /// </summary>
        public override string Description => "Half-Smart CombatClass for the Holy Paladin spec.";

        /// <summary>
        /// Gets or sets the display name for the Paladin Holy.
        /// </summary>
        public override string DisplayName2 => "Paladin Holy";

        /// This property indicates that this object does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is not melee.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator used to compare items for sorting.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
                (
                    null,
                    new() { WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand },
                    new Dictionary<string, double>()
                    {
                { "ITEM_MOD_CRIT_RATING_SHORT", 0.88 },
                { "ITEM_MOD_INTELLECT_SHORT", 0.2 },
                { "ITEM_MOD_SPELL_POWER_SHORT", 0.68 },
                { "ITEM_MOD_HASTE_RATING_SHORT", 0.71},
                    }
                );

        /// <summary>
        /// Gets or sets the role of the WoW character as a healer.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// This code initializes and sets the Talents property with a new instance of TalentTree class.
        /// The Talents property is an override property which returns a TalentTree object.
        /// The TalentTree object is initialized with three properties: Tree1, Tree2, and Tree3.
        /// Tree1 is initialized with a dictionary containing key-value pairs where the key is an integer and the value is an instance of the Talent class.
        /// Tree2 is initialized with a dictionary containing a single key-value pair.
        /// Tree3 is initialized with a dictionary containing multiple key-value pairs.
        /// The key in each dictionary corresponds to a specific talent index, while the value is an instance of the Talent class with specific parameter values.
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },
                { 3, new(1, 3, 3) },
                { 4, new(1, 4, 5) },
                { 6, new(1, 6, 1) },
                { 7, new(1, 7, 5) },
                { 8, new(1, 8, 1) },
                { 10, new(1, 10, 2) },
                { 13, new(1, 13, 1) },
                { 14, new(1, 14, 3) },
                { 16, new(1, 16, 5) },
                { 17, new(1, 17, 3) },
                { 18, new(1, 18, 1) },
                { 21, new(1, 21, 5) },
                { 22, new(1, 22, 1) },
                { 23, new(1, 23, 5) },
                { 24, new(1, 24, 2) },
                { 25, new(1, 25, 2) },
                { 26, new(1, 26, 1) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 5) },
            },
            Tree3 = new()
            {
                { 2, new(3, 2, 5) },
                { 4, new(3, 4, 3) },
                { 5, new(3, 5, 2) },
                { 7, new(3, 7, 5) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether auto attacks should be used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto attacks should not be used; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version as a string.
        /// </summary>
        public override string Version => "1.1";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind enemy entities.
        /// </summary>
        /// <value>
        /// Returns false, indicating that the character cannot walk behind enemy entities.
        /// </value>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property for this Paladin instance.
        /// </summary>
        public override WowClass WowClass => WowClass.Paladin;

        /// <summary>
        /// Gets or sets the World of Warcraft version for the game, which is set to Wrath of the Lich King (version 3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// Gets or sets the TimegatedEvent for changing the beacon event.
        private TimegatedEvent ChangeBeaconEvent { get; }

        /// Gets the private instance of the HealingManager class.
        private HealingManager HealingManager { get; }

        /// This method is responsible for executing a sequence of actions for the Paladin335a class. 
        /// It first calls the base Execute method. 
        /// Then, it checks if the player's mana percentage is below the specified DivineIlluminationManaUntil value 
        ///     and above the specified DivineIlluminationManaAbove value. If both conditions are met, it tries to cast the DivineIllumination spell. 
        /// If that succeeds, the method returns. 
        /// Next, it checks if the player's mana percentage is below the specified DivinePleaMana value. 
        ///     If so, it tries to cast the DivinePlea spell. If that succeeds, the method returns. 
        /// If the ChangeBeaconEvent is ready, the method checks if the player's health percentage is below the specified BeaconOfLightSelfHealth value. 
        ///     If so, it checks if the player does not have the BeaconOfLight aura and tries to cast the BeaconOfLight spell on the player. 
        ///     If that succeeds, it runs the ChangeBeaconEvent and returns. 
        /// Otherwise, if there are more than one healable targets, the method finds the second lowest target with health percentage below the specified BeaconOfLightPartyHealth value. 
        ///     If such a target exists and it does not have the BeaconOfLight aura, the method tries to cast the BeaconOfLight spell on that target. 
        ///     If that succeeds, it runs the ChangeBeaconEvent and returns. 
        /// If the NeedToHealSomeone method returns true, the method returns. 
        /// Otherwise, if the player is alone or the AttackInGroups configuration value is true and the player's mana percentage is below the specified AttackInGroupsUntilManaPercent value, 
        ///     the method tries to find a target using the TargetProviderDps and if it succeeds, it continues with the following actions. 
        /// If the player has either the SealOfVengeance or SealOfWisdom aura, it tries to cast the JudgementOfLight spell on the target. 
        /// If that succeeds, the method returns. 
        /// Then, it tries to cast the Exorcism spell on the target. If that succeeds, the method returns. 
        /// If the player is alone or the AttackInGroupsCloseCombat configuration value is true, 
        ///     it checks if the player is not auto-attacking, is in melee range of the target, and successfully runs the EventAutoAttack. 
        ///     If all conditions are met, the method starts the auto-attack and returns. 
        /// Otherwise, the method sets the movement action to Move and the target position and returns.
        public override void Execute()
        {
            base.Execute();

            if (Bot.Player.ManaPercentage < Configurables["DivineIlluminationManaUntil"]
               && Bot.Player.ManaPercentage > Configurables["DivineIlluminationManaAbove"]
               && TryCastSpell(Paladin335a.DivineIllumination, 0, true))
            {
                return;
            }

            if (Bot.Player.ManaPercentage < Configurables["DivinePleaMana"]
                && TryCastSpell(Paladin335a.DivinePlea, 0, true))
            {
                return;
            }

            if (ChangeBeaconEvent.Ready)
            {
                if (Bot.Player.HealthPercentage < Configurables["BeaconOfLightSelfHealth"])
                {
                    // keep beacon of light on us to reduce healing ourself
                    if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.BeaconOfLight)
                        && TryCastSpell(Paladin335a.BeaconOfLight, Bot.Player.Guid, true))
                    {
                        ChangeBeaconEvent.Run();
                        return;
                    }
                }
                else
                {
                    IEnumerable<IWowUnit> healableTargets = Bot.Wow.ObjectProvider.Partymembers.Where(e => e != null && !e.IsDead).OrderBy(e => e.HealthPercentage);

                    if (healableTargets.Count() > 1)
                    {
                        IWowUnit t = healableTargets.Skip(1).FirstOrDefault(e => e.HealthPercentage < Configurables["BeaconOfLightPartyHealth"]);

                        // keep beacon of light on second lowest target
                        if (t != null
                            && !t.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.BeaconOfLight)
                            && TryCastSpell(Paladin335a.BeaconOfLight, t.Guid, true))
                        {
                            ChangeBeaconEvent.Run();
                            return;
                        }
                    }
                }
            }

            if (NeedToHealSomeone())
            {
                return;
            }
            else
            {
                bool isAlone = !Bot.Objects.Partymembers.Any(e => e.Guid != Bot.Player.Guid);

                if ((isAlone || (Configurables["AttackInGroups"] && Configurables["AttackInGroupsUntilManaPercent"] < Bot.Player.ManaPercentage))
                    && TryFindTarget(TargetProviderDps, out _))
                {
                    if ((Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.SealOfVengeance) || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin335a.SealOfWisdom))
                        && TryCastSpell(Paladin335a.JudgementOfLight, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (TryCastSpell(Paladin335a.Exorcism, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // either we are alone or allowed to go close combat in groups
                    if (isAlone || Configurables["AttackInGroupsCloseCombat"])
                    {
                        if (!Bot.Player.IsAutoAttacking
                            && Bot.Player.IsInMeleeRange(Bot.Target)
                            && EventAutoAttack.Run())
                        {
                            Bot.Wow.StartAutoAttack();
                            return;
                        }
                        else
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a dictionary of <c>objects</c> into the current object.
        /// If the dictionary contains a key "HealingManager", the healing manager will be loaded with the corresponding value.
        /// </summary>
        /// <param name="objects">A dictionary containing string keys and JsonElement values to be loaded.</param>
        public override void Load(Dictionary<string, JsonElement> objects)
        {
            base.Load(objects);

            if (objects.TryGetValue("HealingManager", out JsonElement elementHealingManager))
            {
                HealingManager.Load(elementHealingManager.To<Dictionary<string, JsonElement>>());
            }
        }

        /// <summary>
        /// Executes the code when the character is out of combat.
        /// If there is a need to heal someone, it returns without further execution.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealSomeone())
            {
                return;
            }
        }

        /// <summary>
        /// Overrides the Save method to add the HealingManager information to the saved data.
        /// </summary>
        /// <returns>A dictionary containing the saved data.</returns>
        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> s = base.Save();
            s.Add("HealingManager", HealingManager.Save());
            return s;
        }

        /// Determines if someone needs to be healed.
        private bool NeedToHealSomeone()
        {
            // TODO: bugged need to figure out why cooldown is always wrong if
            // (targetUnit.HealthPercentage < 50 && CastSpellIfPossible(divineFavor,
            // targetUnit.Guid, true)) { LastHealAction = DateTime.Now; return true; }

            return HealingManager.Tick();
        }
    }
}