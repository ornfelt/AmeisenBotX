using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Constants;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Bia10
{
    /// <summary>
    /// This is an abstract class that represents a basic combat class for the Bia10 bot.
    /// </summary>
    public abstract class BasicCombatClassBia10 : ICombatClass
    {
        /// <summary>
        /// Initializes a new instance of the BasicCombatClassBia10 class with the specified bot.
        /// </summary>
        /// <param name="bot">The bot instance to be used.</param>
        protected BasicCombatClassBia10(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            SpellAbortFunctions = new List<Func<bool>>();
            ResurrectionTargets = new Dictionary<string, DateTime>();

            CooldownManager = new CooldownManager(Bot.Character.SpellBook.Spells);
            InterruptManager = new InterruptManager();
            MyAuraManager = new AuraManager(Bot);
            TargetAuraManager = new AuraManager(Bot);
            GroupAuraManager = new GroupAuraManager(Bot);

            EventCheckFacing = new TimegatedEvent(TimeSpan.FromMilliseconds(500));

            Configureables = new Dictionary<string, dynamic>
            {
                { "HealthItemThreshold", 30.0 },
                { "ManaItemThreshold", 30.0 }
            };
        }

        /// <summary>
        /// Gets the name of the author.
        /// </summary>
        public string Author => "Bia10";

        /// <summary>
        /// Gets or sets the collection of blacklisted target display IDs.
        /// </summary>
        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of configureable items, where the key is a string and the value is dynamic.
        /// </summary>
        public Dictionary<string, dynamic> Configureables { get; set; }

        /// <summary>
        /// Gets or sets the CooldownManager object used for managing cooldowns.
        /// </summary>
        public CooldownManager CooldownManager { get; private set; }

        /// <summary>
        /// Gets or sets the description of the object.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets or sets the TimegatedEvent used for checking the event's facing.
        /// </summary>
        public TimegatedEvent EventCheckFacing { get; set; }

        /// <summary>
        /// Gets or sets the GroupAuraManager instance used to manage group auras.
        /// </summary>
        public GroupAuraManager GroupAuraManager { get; private set; }

        /// <summary>
        /// Determines if the facing is handled.
        /// </summary>
        public bool HandlesFacing => true;

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        public abstract bool HandlesMovement { get; }

        /// <summary>
        /// Gets the InterruptManager object and allows for private setting
        /// </summary>
        public InterruptManager InterruptManager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object uses melee attacks.
        /// </summary>
        public abstract bool IsMelee { get; }

        /// Gets or sets the item comparator used for comparing items.
        public abstract IItemComparator ItemComparator { get; set; }

        /// Gets or sets the AuraManager object that manages auras.
        public AuraManager MyAuraManager { get; private set; }

        /// <summary>
        /// Gets or sets the display IDs of priority targets.
        /// </summary>
        /// <value>
        /// An enumerable collection of integers representing the display IDs of priority targets.
        /// </value>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of resurrection targets, where the keys are the targets' names and the values are the resurrection timestamps.
        /// </summary>
        public Dictionary<string, DateTime> ResurrectionTargets { get; private set; }

        /// <summary>
        /// Gets the role for the Wow object.
        /// </summary>
        public abstract WowRole Role { get; }

        /// <summary>
        /// Gets the talent tree associated with the instance.
        /// </summary>
        /// <returns>The talent tree.</returns>
        public abstract TalentTree Talents { get; }

        /// <summary>
        /// Gets the target AuraManager instance.
        /// </summary>
        public AuraManager TargetAuraManager { get; private set; }

        /// <summary>
        /// Gets or sets the target provider used for DPS (Domain Print Services).
        /// </summary>
        public ITargetProvider TargetProviderDps { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity should use auto-attacks.
        /// </summary>
        public abstract bool UseAutoAttacks { get; }

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the character has the ability to walk behind enemies.
        /// </summary>
        public abstract bool WalkBehindEnemy { get; }

        /// <summary>
        /// Gets the wow class.
        /// </summary>
        /// <returns>The wow class.</returns>
        public abstract WowClass WowClass { get; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces class that represents the bot.
        /// </summary>
        protected AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the date and time when the last spell was cast.
        /// </summary>
        protected DateTime LastSpellCast { get; private set; }

        /// <summary>
        /// Gets or sets the list of functions used to determine if a spell should be aborted.
        /// </summary>
        protected List<Func<bool>> SpellAbortFunctions { get; }

        /// <summary>
        /// Gets or sets the time it took to compute the greatest common divisor (GCD).
        /// </summary>
        private double GCDTime { get; set; }

        /// <summary>
        /// Gets or sets the last updated date and time for the GCD (Greatest Common Divisor) calculation.
        /// </summary>
        private DateTime LastGCD { get; set; }

        /// <summary>
        /// Method for attacking the target.
        /// </summary>
        public virtual void AttackTarget()
        {
            IWowUnit target = Bot.Target;
            if (target == null)
            {
                return;
            }

            switch (IsMelee)
            {
                case true when Bot.Player.Position.GetDistance(Bot.Target.Position) <= WowClickToMoveDistance.AttackGuid:
                    {
                        if (Bot.Player.IsCasting)
                        {
                            Bot.Wow.StopCasting();
                        }

                        // todo: kinda buggy
                        Bot.Wow.StopClickToMove();
                        Bot.Movement.Reset();
                        Bot.Wow.InteractWithUnit(target);
                        break;
                    }

                case true when Bot.Player.Position.GetDistance(Bot.Target.Position) > WowClickToMoveDistance.AttackGuid:
                    Bot.Movement.SetMovementAction(MovementAction.Move, target.Position);
                    break;
            }

            static string SpellToCheck(WowClass wowClass)
            {
                return wowClass switch
                {
                    WowClass.None => string.Empty,
                    WowClass.Warrior => Warrior335a.HeroicStrike,
                    WowClass.Paladin => string.Empty,
                    WowClass.Hunter => string.Empty,
                    WowClass.Rogue => string.Empty,
                    WowClass.Priest => Priest335a.Smite,
                    WowClass.Deathknight => string.Empty,
                    WowClass.Shaman => Shaman335a.LightningBolt,
                    WowClass.Mage => Mage335a.Fireball,
                    WowClass.Warlock => string.Empty,
                    WowClass.Druid => string.Empty,
                    _ => throw new ArgumentOutOfRangeException(nameof(wowClass), $"Not expected wowClass value: {wowClass}")
                };
            }

            if (!IsInSpellRange(target, SpellToCheck(Bot.Player.Class))
                || !Bot.Wow.IsInLineOfSight(Bot.Player.Position, target.Position))
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, target.Position);
            }
        }

        /// <summary>
        /// This method executes the main logic for the character's actions. It
        /// first checks if the character is currently casting a spell and if
        /// the target is not in line of sight or if any of the spell abort
        /// functions return true, it stops casting and returns.  Then, it
        /// checks if there is a target and if the event check facing function
        /// returns true, it calls the check facing method for the target.
        /// After that, it attacks the target.  Next, it checks for buffs,
        /// debuffs, and interrupts. If either the personal aura manager or
        /// group aura manager ticks, it returns.  If there is a target and the
        /// target aura manager ticks for the target's auras, it returns.  If
        /// the interrupt manager ticks for nearby enemies, it returns.  After
        /// that, it switches based on the player's race and performs
        /// race-specific actions. For example, if the player's race is Human
        /// and they are dazed, fleeing, influenced, or possessed, it tries to
        /// cast the Every Man For Himself racial ability.  The same logic
        /// applies for other races, each with their own specific conditions
        /// and racial abilities.  Finally, if none of the race cases match the
        /// player's race, it throws an ArgumentOutOfRangeException.
        /// </summary>
        public virtual void Execute()
        {
            if (Bot.Player.IsCasting && (!Bot.Objects.IsTargetInLineOfSight
                                         || SpellAbortFunctions.Any(e => e())))
            {
                Bot.Wow.StopCasting();
                return;
            }

            if (Bot.Target != null && EventCheckFacing.Run())
            {
                CheckFacing(Bot.Target);
            }

            AttackTarget();

            // Buffs, Debuffs, Interrupts
            // --------------------------- >
            if (MyAuraManager.Tick(Bot.Player.Auras) || GroupAuraManager.Tick())
            {
                return;
            }

            if (Bot.Target != null && TargetAuraManager.Tick(Bot.Target.Auras))
            {
                return;
            }

            if (InterruptManager.Tick(Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, IsMelee ? 5.0f : 30.0f)))
            {
                return;
            }

            // Race abilities
            // -------------- >
            switch (Bot.Player.Race)
            {
                // -------- Alliance -------- >
                case WowRace.Human:
                    if (Bot.Player.IsDazed || Bot.Player.IsFleeing || Bot.Player.IsInfluenced || Bot.Player.IsPossessed)
                    {
                        if (ValidateSpell(Racials335a.EveryManForHimself, false))
                        {
                            TryCastSpell(Racials335a.EveryManForHimself, Bot.Player.Guid, false, 0);
                        }
                    }

                    break;

                case WowRace.Gnome:
                    break;

                case WowRace.Draenei:
                    if (Bot.Player.HealthPercentage < 50.0)
                    {
                        if (ValidateSpell(Racials335a.GiftOfTheNaaru, false))
                        {
                            TryCastSpell(Racials335a.GiftOfTheNaaru, Bot.Player.Guid, false, 0);
                        }
                    }

                    break;

                case WowRace.Dwarf:
                    if (Bot.Player.HealthPercentage < 50.0)
                    {
                        if (ValidateSpell(Racials335a.Stoneform, false))
                        {
                            TryCastSpell(Racials335a.Stoneform, Bot.Player.Guid, false, 0);
                        }
                    }

                    break;

                case WowRace.Nightelf:
                    break;
                // -------- Horde -------- >
                case WowRace.Orc:
                    if (Bot.Player.HealthPercentage < 50.0
                        && Bot.GetEnemiesOrNeutralsInCombatWithMe<IWowUnit>(Bot.Player.Position, 10).Count() >= 2)
                    {
                        if (ValidateSpell(Racials335a.BloodFury, false))
                        {
                            TryCastSpell(Racials335a.BloodFury, Bot.Player.Guid, false, 0);
                        }
                    }

                    break;

                case WowRace.Undead:
                    break;

                case WowRace.Tauren:
                    if (Bot.Player.HealthPercentage < 50.0
                        && Bot.GetEnemiesOrNeutralsInCombatWithMe<IWowUnit>(Bot.Player.Position, 10).Count() >= 2)
                    {
                        if (ValidateSpell(Racials335a.WarStomp, false))
                        {
                            TryCastSpell(Racials335a.WarStomp, Bot.Player.Guid, false, 0);
                        }
                    }

                    break;

                case WowRace.Troll:
                    if (Bot.Player.ManaPercentage > 45.0
                        && Bot.GetEnemiesOrNeutralsInCombatWithMe<IWowUnit>(Bot.Player.Position, 10).Count() >= 2)
                    {
                        if (ValidateSpell(Racials335a.Berserking, false))
                        {
                            TryCastSpell(Racials335a.Berserking, Bot.Player.Guid, false, 0);
                        }
                    }

                    break;

                case WowRace.Bloodelf:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Loads the objects into the Configureables property, if the "Configureables" key exists in the dictionary.
        /// </summary>
        /// <param name="objects">A dictionary containing string keys and JsonElement values.</param>
        public virtual void Load(Dictionary<string, JsonElement> objects)
        {
            if (objects.ContainsKey("Configureables")) { Configureables = objects["Configureables"].ToDyn(); }
        }

        /// <summary>
        /// Executes the code when the player is out of combat.
        /// </summary>
        public virtual void OutOfCombatExecute()
        {
            if (Bot.Player.IsCasting)
            {
                if (!Bot.Objects.IsTargetInLineOfSight || SpellAbortFunctions.Any(e => e()))
                {
                    Bot.Wow.StopCasting();
                }

                return;
            }

            MyAuraManager.Tick(Bot.Player.Auras);
            GroupAuraManager.Tick();
        }

        /// <summary>
        /// Saves the configureables and returns it as a dictionary of string and object. 
        /// </summary>
        public virtual Dictionary<string, object> Save()
        {
            return new() { { "Configureables", Configureables } };
        }

        /// <summary>
        /// Returns a string representation of the object in the format:
        /// [WowClass] [Role] DisplayName (Author)
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            return $"[{WowClass}] [{Role}] {DisplayName} ({Author})";
        }

        /// <summary>
        /// Validates a spell by checking if it is known and if the target is in line of sight, as well as if the spell is on cooldown or if the global cooldown is active.
        /// </summary>
        /// <param name="spellName">The name of the spell to validate.</param>
        /// <param name="checkGCD">A flag indicating whether to also check the global cooldown.</param>
        /// <returns>True if the spell is valid, otherwise false.</returns>
        public bool ValidateSpell(string spellName, bool checkGCD)
        {
            if (!Bot.Character.SpellBook.IsSpellKnown(spellName) || !Bot.Objects.IsTargetInLineOfSight)
            {
                return false;
            }

            if (CooldownManager.IsSpellOnCooldown(spellName) || checkGCD && IsGCD())
            {
                return false;
            }

            return !Bot.Player.IsCasting;
        }

        /// <summary>
        /// Checks if a weapon has a specific enchantment and casts a spell to apply it if not.
        /// </summary>
        /// <param name="slot">The equipment slot containing the weapon.</param>
        /// <param name="enchantmentName">The name of the enchantment to check for.</param>
        /// <param name="spellToCastEnchantment">The name of the spell to cast for applying the enchantment.</param>
        /// <returns>True if the enchantment is not found and the spell is successfully cast; otherwise, false.</returns>
        protected bool CheckForWeaponEnchantment(WowEquipmentSlot slot, string enchantmentName, string spellToCastEnchantment)
        {
            if (!Bot.Character.Equipment.Items.ContainsKey(slot))
            {
                return false;
            }

            int itemId = Bot.Character.Equipment.Items[slot].Id;
            if (itemId <= 0)
            {
                return false;
            }

            IWowItem item = Bot.Objects.All.OfType<IWowItem>().FirstOrDefault(e => e.EntryId == itemId);
            if (item == null)
            {
                return false;
            }

            string enchantNameClean = enchantmentName.Split(" ", 2)[0];
            return !item.GetEnchantmentStrings().Any(e => e.Contains(enchantNameClean, StringComparison.OrdinalIgnoreCase))
                   && TryCastSpell(spellToCastEnchantment, 0, true);
        }

        /// <summary>
        /// Handles dead party members by attempting to resurrect them with a specified spell.
        /// </summary>
        /// <param name="spellName">The name of the spell to use for resurrection.</param>
        /// <returns>Returns true if a party member was successfully resurrected, otherwise returns false.</returns>
        protected bool HandleDeadPartyMembers(string spellName)
        {
            Managers.Character.Spells.Objects.Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);
            if (spell == null || CooldownManager.IsSpellOnCooldown(spellName)
                              || spell.Costs >= Bot.Player.Mana)
            {
                return false;
            }

            List<IWowPlayer> groupPlayers = Bot.Objects.Partymembers
                .OfType<IWowPlayer>()
                .Where(e => e.Health == 0)
                .ToList();

            if (!groupPlayers.Any())
            {
                return false;
            }

            IWowPlayer player = groupPlayers.FirstOrDefault(e => Bot.Db.GetUnitName(e, out string name)
                && !ResurrectionTargets.ContainsKey(name) || ResurrectionTargets[name] < DateTime.Now);

            if (player == null)
            {
                return false;
            }

            if (!Bot.Db.GetUnitName(player, out string name))
            {
                return false;
            }

            if (ResurrectionTargets.ContainsKey(name))
            {
                return ResurrectionTargets[name] >= DateTime.Now || TryCastSpell(spellName, player.Guid, true);
            }

            ResurrectionTargets.Add(name, DateTime.Now + TimeSpan.FromSeconds(10));
            return TryCastSpell(spellName, player.Guid, true);
        }

        /// <summary>
        /// Checks if the specified unit is within the range of the given spell.
        /// </summary>
        /// <param name="unit">The unit to check the range against.</param>
        /// <param name="spellName">The name of the spell.</param>
        /// <returns>True if the unit is within the spell's range, false otherwise.</returns>
        protected bool IsInSpellRange(IWowUnit unit, string spellName)
        {
            if (string.IsNullOrEmpty(spellName))
            {
                return false;
            }

            if (unit == null)
            {
                return false;
            }

            Managers.Character.Spells.Objects.Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);
            if (spell == null)
            {
                return false;
            }

            if (spell.MinRange == 0 && spell.MaxRange == 0 || spell.MaxRange == 0)
            {
                return Bot.Player.IsInMeleeRange(unit);
            }

            double distance = Bot.Player.Position.GetDistance(unit.Position);
            return distance >= spell.MinRange && distance <= spell.MaxRange - 1.0;
        }

        /// <summary>
        /// Tries to cast a spell with the given parameters.
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="guid">The identifier of the target.</param>
        /// <param name="needsResource">Specify whether the spell requires a resource.</param>
        /// <param name="GCD">The global cooldown time for casting the spell.</param>
        /// <returns>Returns true if the spell is successfully cast, otherwise false.</returns>
        protected bool TryCastSpell(string spellName, ulong guid, bool needsResource = true, double GCD = 1.5)
        {
            Managers.Character.Spells.Objects.Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);
            if (spell == null)
            {
                return false;
            }

            if (needsResource)
            {
                switch (Bot.Player.PowerType)
                {
                    case WowPowerType.Health when (spell.Costs > Bot.Player.Health): return false;
                    case WowPowerType.Mana when (spell.Costs > Bot.Player.Mana): return false;
                    case WowPowerType.Rage when (spell.Costs > Bot.Player.Rage): return false;
                    case WowPowerType.Energy when (spell.Costs > Bot.Player.Energy): return false;
                    case WowPowerType.RunicPower when (spell.Costs > Bot.Player.RunicPower): return false;
                }
            }

            if (guid != 9999999)
            {
                if (!ValidateTarget(guid, out IWowUnit target, out bool needToSwitchTarget))
                {
                    return false;
                }

                if (target != null && !IsInSpellRange(target, spellName))
                {
                    return false;
                }

                bool isTargetMyself = guid == Bot.Player.Guid;
                if (!isTargetMyself && needToSwitchTarget)
                {
                    Bot.Wow.ChangeTarget(guid);
                }

                if (!isTargetMyself && target != null && !BotMath.IsFacing(Bot.Player.Position, Bot.Player.Rotation, target.Position))
                {
                    Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, target.Position);
                }

                switch (spell.CastTime)
                {
                    case 0:
                        Bot.Movement.PreventMovement(TimeSpan.FromMilliseconds(300));
                        CheckFacing(target);
                        GCD += 0.1; // some timing is off with casting after instant cast spells
                        break;

                    case > 0:
                        Bot.Movement.PreventMovement(TimeSpan.FromMilliseconds(spell.CastTime));
                        CheckFacing(target);
                        break;
                }

                if (!CastSpell(spellName, isTargetMyself))
                {
                    return false;
                }

                if (GCD == 0)
                {
                    return true;
                }
            }
            else if (guid == 9999999)
            {
                switch (spell.CastTime)
                {
                    case 0:
                        Bot.Movement.PreventMovement(TimeSpan.FromMilliseconds(300));
                        GCD += 0.1; // some timing is off with casting after instant cast spells
                        break;

                    case > 0:
                        Bot.Movement.PreventMovement(TimeSpan.FromMilliseconds(spell.CastTime));
                        break;
                }

                if (!CastSpell(spellName, false))
                {
                    return false;
                }

                if (GCD == 0)
                {
                    return true;
                }
            }

            SetGCD(GCD);
            return true;
        }

        /// <summary>
        /// This method casts a spell with the given spell name and target
        /// type. It executes a Lua script to perform the spell cast and
        /// retrieves the result. If the cast is successful, it logs the
        /// casting information and returns true. If the cast fails or
        /// encounters any errors, it returns false.
        /// </summary>
        private bool CastSpell(string spellName, bool castOnSelf)
        {
            // spits out stuff like this "1;300" (1 or 0 whether the cast was successful or
            // not);(the cooldown in ms)
            if (!Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua(
                    DataConstants.GetCastSpellString(spellName, castOnSelf)), out string result))
            {
                return false;
            }

            if (result.Length < 3)
            {
                return false;
            }

            string[] parts = result.Split(";", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return false;
            }

            // replace comma with dot in the cooldown
            if (parts[1].Contains(',', StringComparison.OrdinalIgnoreCase))
            {
                parts[1] = parts[1].Replace(',', '.');
            }

            if (!int.TryParse(parts[0], out int castSuccessful)
                || !double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double cooldown))
            {
                return false;
            }

            cooldown = Math.Max(cooldown, 0);
            CooldownManager.SetSpellCooldown(spellName, (int)cooldown);

            if (castSuccessful == 1)
            {
                AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: Casting Spell \"{spellName}\" on \"{Bot.Target?.Guid}\"", LogLevel.Verbose);
                return true;
            }

            AmeisenLogger.I.Log("CombatClass", $"[{DisplayName}]: Spell \"{spellName}\" is on cooldown for \"{cooldown}\"ms", LogLevel.Verbose);
            return false;
        }

        /// Checks if the target needs to be faced and faces the target if necessary.
        private void CheckFacing(IWowObject target)
        {
            if (target == null || target.Guid == Bot.Wow.PlayerGuid)
            {
                return;
            }

            float facingAngle = BotMath.GetFacingAngle(Bot.Player.Position, target.Position);
            float angleDiff = facingAngle - Bot.Player.Rotation;

            switch (angleDiff)
            {
                case < 0:
                    angleDiff += MathF.Tau;
                    break;

                case > MathF.Tau:
                    angleDiff -= MathF.Tau;
                    break;
            }

            if (angleDiff > 1.0)
            {
                Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, target.Position);
            }
        }

        ///<summary>
        ///Checks if the time difference between LastGCD and current time is less than GCDTime.
        ///Returns true if the time difference is less than GCDTime, otherwise false.
        ///</summary>
        private bool IsGCD()
        {
            return DateTime.Now.Subtract(LastGCD).TotalSeconds < GCDTime;
        }

        /// <summary>
        /// Sets the value of the GCDTime property to the specified value and updates the LastGCD property with the current date and time.
        /// </summary>
        private void SetGCD(double gcdInSec)
        {
            GCDTime = gcdInSec;
            LastGCD = DateTime.Now;
        }

        /// Validates the target based on the provided GUID and returns the result along with the target object and a flag indicating if switching targets is needed.
        private bool ValidateTarget(ulong guid, out IWowUnit target, out bool needToSwitchTargets)
        {
            if (guid == Bot.Player.Guid)
            {
                target = Bot.Player;
                needToSwitchTargets = false;
                return true;
            }
            if (guid == Bot.Wow.TargetGuid)
            {
                target = Bot.Target;
                needToSwitchTargets = false;
                return true;
            }

            target = Bot.GetWowObjectByGuid<IWowUnit>(guid);
            needToSwitchTargets = true;
            return target != null;
        }
    }
}