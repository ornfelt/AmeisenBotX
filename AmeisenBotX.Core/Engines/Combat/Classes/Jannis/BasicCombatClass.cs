using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Storage;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis
{
    /// <summary>
    /// This class serves as a base for all combat classes and implements the necessary interfaces for configuration and combat.
    /// </summary>
    public abstract class BasicCombatClass : SimpleConfigurable, ICombatClass
    {
        /// This array holds the IDs of various healing items that can be used by the player. It includes IDs for potions and healthstones.
        private readonly int[] useableHealingItems = new int[]
                                                                                                                                                                                                                                                                                                                                                                                        {
            // potions
            118, 929, 1710, 2938, 3928, 4596, 5509, 13446, 22829, 33447,
            // healthstones
            5509, 5510, 5511, 5512, 9421, 19013, 22103, 36889, 36892,
                };

        /// <summary>
        /// An array of item IDs representing usable mana items.
        /// </summary>
        private readonly int[] useableManaItems = new int[]
                {
            // potions
            2245, 3385, 3827, 6149, 13443, 13444, 33448, 22832,
                };

        /// <summary>
        /// Initializes a new instance of the BasicCombatClass class with the provided bot.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object that represents the bot.</param>
        protected BasicCombatClass(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            SpellAbortFunctions = new();

            CooldownManager = new(Bot.Character.SpellBook.Spells);
            RessurrectionTargets = new();

            TargetProviderDps = new TargetManager(Bot, WowRole.Dps, TimeSpan.FromMilliseconds(250));
            TargetProviderTank = new TargetManager(Bot, WowRole.Tank, TimeSpan.FromMilliseconds(250));
            TargetProviderHeal = new TargetManager(Bot, WowRole.Heal, TimeSpan.FromMilliseconds(250));

            MyAuraManager = new(Bot);
            TargetAuraManager = new(Bot);
            GroupAuraManager = new(Bot);

            InterruptManager = new();

            EventCheckFacing = new(TimeSpan.FromMilliseconds(500));
            EventAutoAttack = new(TimeSpan.FromMilliseconds(500));

            Configurables.TryAdd("HealthItemThreshold", 30.0);
            Configurables.TryAdd("ManaItemThreshold", 30.0);
        }

        /// <summary>
        /// Gets or sets the name of the author.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// Gets or sets the blacklisted target display ids.
        public IEnumerable<int> BlacklistedTargetDisplayIds { get => TargetProviderDps.BlacklistedTargets; set => TargetProviderDps.BlacklistedTargets = value; }

        /// Gets or sets the instance of the CooldownManager class that manages cooldown periods.
        public CooldownManager CooldownManager { get; private set; }

        /// <summary>
        /// Gets the description of the object as a string.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the display name with the WowVersion surrounded by square brackets and the DisplayName2.
        /// </summary>
        public string DisplayName => $"[{WowVersion}] {DisplayName2}";

        /// <summary>
        /// Gets the display name of the object.
        /// </summary>
        public abstract string DisplayName2 { get; }

        /// Represents the time-limited event of an automatic attack occurrence.
        public TimegatedEvent EventAutoAttack { get; private set; }

        /// Gets or sets the TimegatedEvent for checking the facing of an event.
        public TimegatedEvent EventCheckFacing { get; set; }

        /// Property for accessing the GroupAuraManager object.
        public GroupAuraManager GroupAuraManager { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating if the code handles facing.
        /// </summary>
        public bool HandlesFacing => true;

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        /// <returns>True if this object handles movement; otherwise, false.</returns>
        public abstract bool HandlesMovement { get; }

        /// <summary>
        /// Gets or sets the reference to the InterruptManager object.
        /// </summary>
        public InterruptManager InterruptManager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this character is a melee character.
        /// </summary>
        public abstract bool IsMelee { get; }

        ///<summary>
        /// Gets or sets the item comparator used for comparing items.
        ///</summary>
        public abstract IItemComparator ItemComparator { get; set; }

        /// Gets or sets the AuraManager object associated with this instance.
        public AuraManager MyAuraManager { get; private set; }

        /// Gets or sets the display IDs of the priority targets for the target provider DPS.
        public IEnumerable<int> PriorityTargetDisplayIds { get => TargetProviderDps.PriorityTargets; set => TargetProviderDps.PriorityTargets = value; }

        /// <summary>
        /// Gets or sets the dictionary containing the resurrection targets as key-value pairs,
        /// where the key is a string and the value is a DateTime object.
        /// </summary>
        public Dictionary<string, DateTime> RessurrectionTargets { get; private set; }

        /// <summary>
        /// Gets or sets the role of the Wow object.
        /// </summary>
        public abstract WowRole Role { get; }

        /// <summary>
        /// Gets the talent tree for the current instance.
        /// </summary>
        public abstract TalentTree Talents { get; }

        /// <summary>
        /// Gets or sets the target AuraManager for this object.
        /// </summary>
        public AuraManager TargetAuraManager { get; private set; }

        /// <summary>
        /// Gets the target provider for Dps.
        /// </summary>
        public ITargetProvider TargetProviderDps { get; private set; }

        /// Gets or sets the target provider for healing operations.
        public ITargetProvider TargetProviderHeal { get; private set; }

        /// Gets or sets the target provider for tanks.
        public ITargetProvider TargetProviderTank { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity will use auto attacks.
        /// </summary>
        /// <returns>True if the entity will use auto attacks; otherwise, false.</returns>
        public abstract bool UseAutoAttacks { get; }

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// Gets a value indicating whether the character can walk behind enemies.
        /// </summary>
        /// <returns>True if the character can walk behind enemies, false otherwise.</returns>
        public abstract bool WalkBehindEnemy { get; }

        /// <summary>
        /// Gets or sets the WowClass property.
        /// </summary>
        public abstract WowClass WowClass { get; }

        /// Gets the version of Wow.
        public abstract WowVersion WowVersion { get; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces class that represents the bot.
        /// </summary>
        protected AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the date and time of when the last spell was cast.
        /// </summary>
        protected DateTime LastSpellCast { get; private set; }

        /// <summary>
        /// Gets or sets the list of functions that determine whether a spell should be aborted or not.
        /// </summary>
        protected List<Func<bool, bool>> SpellAbortFunctions { get; }

        /// Gets or sets the unique identifier of the current cast target.
        private ulong CurrentCastTargetGuid { get; set; }

        /// <summary>
        /// Attacks the target if it is within melee range, otherwise moves towards the target.
        /// </summary>
        public virtual void AttackTarget()
        {
            if (Bot.Target == null)
            {
                return;
            }

            if (Bot.Player.IsInMeleeRange(Bot.Target))
            {
                Bot.Wow.StopClickToMove();
                Bot.Movement.Reset();
                Bot.Wow.InteractWithUnit(Bot.Target);
            }
            else if (!Bot.Tactic.PreventMovement)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
            }
        }

        /// This method represents the execution logic of the bot. It first checks if the target is not null and if the event check facing is successful. If both conditions are met, the bot will call the CheckFacing method on the target. 
        public virtual void Execute()
        {
            if (Bot.Target != null && EventCheckFacing.Run())
            {
                CheckFacing(Bot.Target);
            }

            if (Bot.Player.IsCasting)
            {
                if (!Bot.Objects.IsTargetInLineOfSight || SpellAbortFunctions.Any(e => e(CurrentCastTargetGuid == Bot.Player.Guid)))
                {
                    Bot.Wow.StopCasting();
                }

                return;
            }

            // Update Priority Units
            // --------------------------- >
            if (Bot.Dungeon.Profile != null
                && Bot.Dungeon.Profile.PriorityUnits != null
                && Bot.Dungeon.Profile.PriorityUnits.Count > 0)
            {
                TargetProviderDps.PriorityTargets = Bot.Dungeon.Profile.PriorityUnits;
            }

            // Autoattacks
            // --------------------------- >
            if (UseAutoAttacks)
            {
                if (EventAutoAttack.Run()
                    //&& !Bot.Player.IsAutoAttacking
                    && Bot.Player.IsInMeleeRange(Bot.Target))
                {
                    Bot.Wow.StartAutoAttack();
                }
            }

            // Buffs, Debuffs, Interrupts
            // --------------------------- >
            if (MyAuraManager.Tick(Bot.Player.Auras)
                || GroupAuraManager.Tick())
            {
                return;
            }

            if (Bot.Target != null
                && TargetAuraManager.Tick(Bot.Target.Auras))
            {
                return;
            }

            if (InterruptManager.Tick(Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, IsMelee ? 5.0f : 30.0f)))
            {
                return;
            }

            // Useable items, potions, etc.
            // ---------------------------- >
            if (Bot.Player.HealthPercentage < Configurables["HealthItemThreshold"])
            {
                IWowInventoryItem healthItem = Bot.Character.Inventory.Items.FirstOrDefault(e => useableHealingItems.Contains(e.Id));

                if (healthItem != null)
                {
                    Bot.Wow.UseItemByName(healthItem.Name);
                }
            }

            if (Bot.Player.ManaPercentage < Configurables["ManaItemThreshold"])
            {
                IWowInventoryItem manaItem = Bot.Character.Inventory.Items.FirstOrDefault(e => useableManaItems.Contains(e.Id));

                if (manaItem != null)
                {
                    Bot.Wow.UseItemByName(manaItem.Name);
                }
            }

            // Race abilities
            // -------------- >
            if (Bot.Player.Race == WowRace.Human
                && (Bot.Player.IsDazed
                    || Bot.Player.IsFleeing
                    || Bot.Player.IsInfluenced
                    || Bot.Player.IsPossessed)
                && TryCastSpell("Every Man for Himself", 0))
            {
                return;
            }

            if (Bot.Player.HealthPercentage < 50.0
                && ((Bot.Player.Race == WowRace.Draenei && TryCastSpell("Gift of the Naaru", 0))
                    || (Bot.Player.Race == WowRace.Dwarf && TryCastSpell("Stoneform", 0))))
            {
                return;
            }
        }

        /// This method is responsible for executing actions when the player is out of combat. It first checks if the player is currently casting a spell, and if so, it checks if the target is in line of sight or if any spell abort functions are triggered. If either condition is met, the player's casting is stopped. 
        public virtual void OutOfCombatExecute()
        {
            if (Bot.Player.IsCasting)
            {
                if (!Bot.Objects.IsTargetInLineOfSight || SpellAbortFunctions.Any(e => e(CurrentCastTargetGuid == Bot.Player.Guid)))
                {
                    Bot.Wow.StopCasting();
                }

                return;
            }

            if ((Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Food") && Bot.Player.HealthPercentage < 100.0)
                || (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Drink") && Bot.Player.ManaPercentage < 100.0))
            {
                return;
            }

            if (MyAuraManager.Tick(Bot.Player.Auras)
                || GroupAuraManager.Tick())
            {
                return;
            }
        }

        /// <summary>
        /// Override method that returns a string representation of the object in the format: [WowClass] [Role] DisplayName (Author)
        /// </summary>
        public override string ToString()
        {
            return $"[{WowClass}] [{Role}] {DisplayName} ({Author})";
        }

        /// <summary>
        /// Checks if a weapon enchantment is present on the specified equipment slot.
        /// </summary>
        /// <param name="slot">The equipment slot to check.</param>
        /// <param name="enchantmentName">The name of the weapon enchantment to look for.</param>
        /// <param name="spellToCastEnchantment">The spell to cast the enchantment.</param>
        /// <returns>True if the weapon enchantment is not present and the spell to cast the enchantment was successfully cast, otherwise false.</returns>
        protected bool CheckForWeaponEnchantment(WowEquipmentSlot slot, string enchantmentName, string spellToCastEnchantment)
        {
            if (Bot.Character.Equipment.Items.ContainsKey(slot))
            {
                int itemId = Bot.Character.Equipment.Items[slot].Id;

                if (itemId > 0)
                {
                    IWowItem item = Bot.Objects.All.OfType<IWowItem>().FirstOrDefault(e => e.EntryId == itemId);

                    if (item != null
                        && !item.GetEnchantmentStrings().Any(e => e.Contains(enchantmentName, StringComparison.OrdinalIgnoreCase))
                        && TryCastSpell(spellToCastEnchantment, 0, true))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// This method handles dead party members by checking if the given spell is available, has enough mana cost, and is not on cooldown. 
        /// It then iterates through the party members, finds the first dead player who is not in the resurrection targets dictionary or has expired in the dictionary, 
        /// then adds the player's name to the resurrection targets dictionary with a 10-second expiration time and attempts to cast the spell on the player. 
        /// If the player's name is already in the resurrection targets dictionary and has not expired, it attempts to cast the spell on the player. 
        /// Finally, it returns true if the method successfully handles a dead party member, otherwise false.
        protected bool HandleDeadPartymembers(string spellName)
        {
            Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

            if (spell != null
                && spell.Costs < Bot.Player.Mana
                && !CooldownManager.IsSpellOnCooldown(spellName))
            {
                IEnumerable<IWowPlayer> groupPlayers = Bot.Objects.Partymembers
                    .OfType<IWowPlayer>()
                    .Where(e => e.IsDead);

                if (groupPlayers.Any())
                {
                    IWowPlayer player = groupPlayers.FirstOrDefault(e => (Bot.Db.GetUnitName(e, out string name) && !RessurrectionTargets.ContainsKey(name)) || RessurrectionTargets[name] < DateTime.UtcNow);

                    if (player != null)
                    {
                        if (Bot.Db.GetUnitName(player, out string name))
                        {
                            if (!RessurrectionTargets.ContainsKey(name))
                            {
                                RessurrectionTargets.Add(name, DateTime.UtcNow + TimeSpan.FromSeconds(10));
                                return TryCastSpell(spellName, player.Guid, true);
                            }

                            if (RessurrectionTargets[name] < DateTime.UtcNow)
                            {
                                return TryCastSpell(spellName, player.Guid, true);
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a spell is within the specified range of a unit.
        /// </summary>
        /// <param name="spell">The spell to check the range for.</param>
        /// <param name="wowUnit">The unit to check the range against.</param>
        /// <returns>True if the spell is within the specified range of the unit, otherwise false.</returns>
        protected bool IsInRange(Spell spell, IWowUnit wowUnit)
        {
            if ((spell.MinRange == 0 && spell.MaxRange == 0) || spell.MaxRange == 0)
            {
                return Bot.Player.IsInMeleeRange(wowUnit);
            }

            double distance = Bot.Player.Position.GetDistance(wowUnit.Position);
            return distance >= spell.MinRange && distance <= spell.MaxRange - 1.0;
        }

        ///<summary>
        ///Attempts to cast an area of effect (AOE) spell with the specified spellName, targeting the entity with the given guid.
        ///</summary>
        ///<param name="spellName">The name of the spell to be cast.</param>
        ///<param name="guid">The unique identifier of the target entity.</param>
        ///<param name="needsResource">(Optional) Determines whether the spell requires a resource to be cast.</param>
        ///<param name="currentResourceAmount">(Optional) The current amount of resource available for casting the spell.</param>
        ///<param name="forceTargetSwitch">(Optional) Specifies whether the target of the spell should be switched forcefully.</param>
        ///<returns>
        ///Returns true if the spell was successfully cast and the AOE effect was applied, otherwise false.
        ///</returns>
        protected bool TryCastAoeSpell(string spellName, ulong guid, bool needsResource = false, int currentResourceAmount = 0, bool forceTargetSwitch = false)
        {
            return TryCastSpell(spellName, guid, needsResource, currentResourceAmount, forceTargetSwitch)
                && CastAoeSpell(guid);
        }

        /// <summary>
        /// Tries to cast an area of effect spell for a Death Knight character with the specified parameters.
        /// </summary>
        /// <param name="spellName">The name of the spell to be cast.</param>
        /// <param name="guid">The unique identifier of the target.</param>
        /// <param name="needsRunicPower">Optional parameter to indicate if the spell requires runic power.</param>
        /// <param name="needsBloodrune">Optional parameter to indicate if the spell requires a blood rune.</param>
        /// <param name="needsFrostrune">Optional parameter to indicate if the spell requires a frost rune.</param>
        /// <param name="needsUnholyrune">Optional parameter to indicate if the spell requires an unholy rune.</param>
        /// <param name="forceTargetSwitch">Optional parameter to determine if the target should be switched.</param>
        /// <returns>True if the spell was successfully cast; otherwise, false.</returns>
        protected bool TryCastAoeSpellDk(string spellName, ulong guid, bool needsRunicPower = false, bool needsBloodrune = false, bool needsFrostrune = false, bool needsUnholyrune = false, bool forceTargetSwitch = false)
        {
            return TryCastSpellDk(spellName, guid, needsRunicPower, needsBloodrune, needsFrostrune, needsUnholyrune, forceTargetSwitch)
                && CastAoeSpell(guid);
        }

        /// <summary>
        /// Tries to cast a spell with the given parameters.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="guid">The GUID of the target for the spell. Set to 0 for self-targeting spells.</param>
        /// <param name="needsResource">Optional. Determines whether the spell requires a resource. Default is false.</param>
        /// <param name="currentResourceAmount">Optional. The current amount of resources available. Default is 0.</param>
        /// <param name="forceTargetSwitch">Optional. Determines whether to force a target switch. Default is false.</param>
        /// <param name="additionalValidation">Optional. Additional validation function to check before casting the spell. Default is null.</param>
        /// <param name="additionalPreperation">Optional. Additional preparation function to execute before casting the spell. Default is null.</param>
        /// <returns>True if the spell was cast successfully, false otherwise.</returns>
        protected bool TryCastSpell(string spellName, ulong guid, bool needsResource = false, int currentResourceAmount = 0, bool forceTargetSwitch = false, Func<bool> additionalValidation = null, Func<bool> additionalPreperation = null)
        {
            if (!Bot.Character.SpellBook.IsSpellKnown(spellName) || (guid != 0 && guid != Bot.Wow.PlayerGuid && !Bot.Objects.IsTargetInLineOfSight)) { return false; }

            if (ValidateTarget(guid, out IWowUnit target, out bool needToSwitchTarget))
            {
                if (currentResourceAmount == 0)
                {
                    currentResourceAmount = Bot.Player.Resource;
                }

                bool isTargetMyself = guid == 0 || guid == Bot.Player.Guid;
                Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                if (ValidateSpell(spell, target, currentResourceAmount, needsResource, isTargetMyself)
                    && (additionalValidation == null || additionalValidation()))
                {
                    if (additionalPreperation?.Invoke() == true)
                    {
                        return false;
                    }

                    PrepareCast(isTargetMyself, target, needToSwitchTarget || forceTargetSwitch, spell);
                    LastSpellCast = DateTime.UtcNow;
                    return CastSpell(spellName, isTargetMyself);
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to cast a spell for a Death Knight character.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="guid">The unique identifier of the character.</param>
        /// <param name="needsRunicPower">Determines if the spell needs runic power.</param>
        /// <param name="needsBloodrune">Determines if the spell needs a blood rune.</param>
        /// <param name="needsFrostrune">Determines if the spell needs a frost rune.</param>
        /// <param name="needsUnholyrune">Determines if the spell needs an unholy rune.</param>
        /// <param name="forceTargetSwitch">Determines if the target needs to be switched.</param>
        /// <returns>True if the spell was cast successfully, otherwise false.</returns>
        protected bool TryCastSpellDk(string spellName, ulong guid, bool needsRunicPower = false, bool needsBloodrune = false, bool needsFrostrune = false, bool needsUnholyrune = false, bool forceTargetSwitch = false)
        {
            return TryCastSpell(spellName, guid, needsRunicPower, Bot.Player.RunicPower, forceTargetSwitch, () =>
            {
                Dictionary<int, int> runes = Bot.Wow.GetRunesReady();
                return (!needsBloodrune || runes[(int)WowRuneType.Blood] > 0 || runes[(int)WowRuneType.Death] > 0)
                    && (!needsFrostrune || runes[(int)WowRuneType.Frost] > 0 || runes[(int)WowRuneType.Death] > 0)
                    && (!needsUnholyrune || runes[(int)WowRuneType.Unholy] > 0 || runes[(int)WowRuneType.Death] > 0);
            });
        }

        /// <summary>
        /// Tries to cast a spell for a rogue character, with the option to specify energy, combo points, and a target switch.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="guid">The unique identifier of the rogue character.</param>
        /// <param name="needsEnergy">Indicates whether the spell requires energy.</param>
        /// <param name="needsCombopoints">Indicates whether the spell requires combo points.</param>
        /// <param name="requiredCombopoints">The minimum number of combo points required to cast the spell.</param>
        /// <param name="forceTargetSwitch">Indicates whether a target switch should be forced.</param>
        /// <returns>True if the spell was successfully cast, otherwise false.</returns>
        protected bool TryCastSpellRogue(string spellName, ulong guid, bool needsEnergy = false, bool needsCombopoints = false, int requiredCombopoints = 1, bool forceTargetSwitch = false)
        {
            return TryCastSpell(spellName, guid, needsEnergy, Bot.Player.Energy, forceTargetSwitch, () =>
            {
                return !needsCombopoints || Bot.Player.ComboPoints >= requiredCombopoints;
            });
        }

        /// <summary>
        /// Tries to cast a spell for a warrior character.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="requiredStance">The required stance for the spell.</param>
        /// <param name="guid">The unique identifier of the character.</param>
        /// <param name="needsResource">Optional. Indicates if the spell needs a resource.</param>
        /// <param name="forceTargetSwitch">Optional. Indicates if the target should be switched forcibly.</param>
        /// <returns>True if the spell was cast successfully; otherwise, false.</returns>
        protected bool TryCastSpellWarrior(string spellName, string requiredStance, ulong guid, bool needsResource = false, bool forceTargetSwitch = false)
        {
            return TryCastSpell(spellName, guid, needsResource, Bot.Player.Rage, forceTargetSwitch, null, () =>
            {
                if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == requiredStance)
                    && Bot.Character.SpellBook.IsSpellKnown(requiredStance)
                    && !CooldownManager.IsSpellOnCooldown(requiredStance))
                {
                    CastSpell(requiredStance, true);
                    return false;
                }

                return true;
            });
        }

        /// <summary>
        /// Attempts to find a target using the given targetProvider and sets the specified targets out parameter.
        /// Returns true if a valid target is found, false otherwise.
        /// </summary>
        protected bool TryFindTarget(ITargetProvider targetProvider, out IEnumerable<IWowUnit> targets)
        {
            if (targetProvider.Get(out targets))
            {
                IWowUnit unit = targets.FirstOrDefault();

                if (unit != null)
                {
                    if (Bot.Player.TargetGuid == unit.Guid)
                    {
                        if (IWowUnit.IsValidAlive(Bot.Target))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Bot.Wow.ChangeTarget(unit.Guid);
                        return false;
                    }
                }
            }

            Bot.Wow.ChangeTarget(0);
            return false;
        }

        /// <summary>
        /// Casts an area-of-effect spell on the specified target.
        /// </summary>
        /// <param name="targetGuid">The unique identifier of the target to cast the spell on.</param>
        /// <returns>Returns true if the spell was successfully cast, otherwise false.</returns>
        private bool CastAoeSpell(ulong targetGuid)
        {
            if (ValidateTarget(targetGuid, out IWowUnit target, out bool _))
            {
                Bot.Wow.ClickOnTerrain(target.Position);
                LastSpellCast = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /// This method checks if a spell can be cast and performs the casting if possible. It retrieves the cooldown of the spell using Lua and parses the result. If the cooldown is negative, it sets it to a default value of 100 ms. Then it sets the cooldown of the spell using the CooldownManager. If the spell casting is successful, it logs the casting target and the cooldown. If the casting fails, it logs the failure along with the cooldown. Returns true if the spell was cast successfully, false otherwise.
        private bool CastSpell(string spellName, bool castOnSelf)
        {
            // spits out stuff like this "1;300" (1 or 0 whether the cast was successful or
            // not);(the cooldown in ms)
            if (Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:3}},{{v:4}}=GetSpellCooldown(\"{spellName}\"){{v:2}}=({{v:3}}+{{v:4}}-GetTime())*1000;if {{v:2}}<=0 then {{v:2}}=0;CastSpellByName(\"{spellName}\"{(castOnSelf ? ", \"player\"" : string.Empty)}){{v:5}},{{v:6}}=GetSpellCooldown(\"{spellName}\"){{v:1}}=({{v:5}}+{{v:6}}-GetTime())*1000;{{v:0}}=\"1;\"..{{v:1}} else {{v:0}}=\"0;\"..{{v:2}} end"), out string result))
            {
                AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: COOLDOWN RESULT: \"{result}\"", LogLevel.Verbose);

                if (result.Length < 3)
                {
                    return false;
                }

                string[] parts = result.Split(";", StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    return false;
                }

                if (parts[1].Contains(','))
                {
                    parts[1] = parts[1].Split(',')[0];
                }
                else
                {
                    parts[1] = parts[1].Split('.')[0];
                }

                if (int.TryParse(parts[0], out int castSuccessful)
                    && int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int cooldown))
                {
                    if (cooldown < 0)
                    {
                        // TODO: find bug that causes negative cooldowns
                        cooldown = 100;
                    }

                    CooldownManager.SetSpellCooldown(spellName, cooldown);

                    if (castSuccessful == 1)
                    {
                        CurrentCastTargetGuid = Bot.Target == null || castOnSelf ? Bot.Player.Guid : Bot.Target.Guid;
                        AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: Casting Spell \"{spellName}\" on \"{(castOnSelf ? "self" : Bot.Target?.Guid)}\"", LogLevel.Verbose);
                        AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: Spell \"{spellName}\" is on cooldown for {cooldown} ms", LogLevel.Verbose);
                        return true;
                    }
                    else
                    {
                        AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: Unable to cast Spell \"{spellName}\" on \"{(castOnSelf ? "self" : Bot.Target?.Guid)}\"", LogLevel.Verbose);
                        AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: Spell \"{spellName}\" is on cooldown for {cooldown} ms", LogLevel.Verbose);
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the target is facing the player, and makes the player face the target if not.
        /// </summary>
        private void CheckFacing(IWowUnit target)
        {
            if (target != null
                && target.Guid != Bot.Wow.PlayerGuid
                && !BotMath.IsFacing(Bot.Player.Position, Bot.Player.Rotation, target.Position, 1.0f))
            {
                Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, target.Position);
            }
        }

        /// <summary>
        /// This method prepares for the cast of a spell by potentially changing the target and preventing movement.
        /// </summary>
        /// <param name="isTargetMyself">A boolean indicating whether the target is the player or not.</param>
        /// <param name="target">The target of the spell.</param>
        /// <param name="switchTarget">A boolean indicating whether to switch the target or not.</param>
        /// <param name="spell">The spell to be cast.</param>
        private void PrepareCast(bool isTargetMyself, IWowUnit target, bool switchTarget, Spell spell)
        {
            if (!isTargetMyself && switchTarget)
            {
                Bot.Wow.ChangeTarget(target.Guid);
            }

            if (spell.CastTime > 0)
            {
                Bot.Movement.PreventMovement(TimeSpan.FromMilliseconds(spell.CastTime), PreventMovementType.SpellCast);
            }
        }

        /// <summary>
        /// Validates if a spell can be cast on a target with the provided parameters.
        /// </summary>
        /// <param name="spell">The spell to be cast.</param>
        /// <param name="target">The target on which the spell will be cast.</param>
        /// <param name="resource">The available resource for casting the spell.</param>
        /// <param name="needsResource">Determines if the spell requires a resource.</param>
        /// <param name="isTargetMyself">Determines if the target is the caster themself.</param>
        /// <returns>True if the spell is valid to be cast, otherwise false.</returns>
        private bool ValidateSpell(Spell spell, IWowUnit target, int resource, bool needsResource, bool isTargetMyself)
        {
            return spell != null
                && !CooldownManager.IsSpellOnCooldown(spell.Name)
                && (!needsResource || spell.Costs <= resource)
                && (isTargetMyself || IsInRange(spell, target));
        }

        /// <summary>
        /// Validates the target based on the given GUID and retrieves the corresponding IWowUnit object. 
        /// Sets the target and needToSwitchTargets flags accordingly.
        /// Returns true if the target is not null.
        /// </summary>
        private bool ValidateTarget(ulong guid, out IWowUnit target, out bool needToSwitchTargets)
        {
            if (guid == 0)
            {
                target = Bot.Player;
                needToSwitchTargets = false;
            }
            else if (guid == Bot.Wow.TargetGuid)
            {
                target = Bot.Target;
                needToSwitchTargets = false;
            }
            else
            {
                target = Bot.GetWowObjectByGuid<IWowUnit>(guid);
                needToSwitchTargets = true;
            }

            return target != null;
        }
    }
}