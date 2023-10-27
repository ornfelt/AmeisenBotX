using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    /// <summary>
    /// Represents a basic combat class for a troll character with the race spell Berserking.
    /// </summary>
    public abstract class BasicKamelClass : ICombatClass
    {
        /// <summary>
        /// Race spell for Troll: Berserking.
        /// </summary>
        #region Race Spells

        //Race (Troll)
        private const string BerserkingSpell = "Berserking";

        ///<summary>
        ///The name of the spell "Every Man for Himself" used by the Human race.
        ///</summary>
        //Race (Human)
        private const string EveryManforHimselfSpell = "Every Man for Himself";

        /// <summary>
        /// Represents the Race Draenei and its special ability "Gift of the Naaru".
        /// </summary>
        //Race (Draenei)
        private const string giftOfTheNaaruSpell = "Gift of the Naaru";

        /// <summary>
        /// The constant representing the spell "Stoneform" for the Dwarf race.
        /// </summary>
        //Race (Dwarf)
        private const string StoneformSpell = "Stoneform";

        /// <summary>
        /// The spell used by Shamans to bring an ally back to life with a portion of their health.
        /// </summary>
        #endregion Race Spells

        #region Shaman

        public const string ancestralSpiritSpell = "Ancestral Spirit";

        /// <summary>
        /// The name of the redemption spell for a Paladin.
        /// </summary>
        #endregion Shaman

        #region Paladin

        public const string redemptionSpell = "Redemption";

        /// <summary>
        /// The name of the resurrection spell used by a Priest.
        /// </summary>
        #endregion Paladin

        #region Priest

        public const string resurrectionSpell = "Resurrection";

        /// <summary>
        /// A dictionary that stores the cooldown time for each spell.
        /// The key is the name of the spell, and the value is the time when the spell will be available again.
        /// </summary>
        #endregion Priest

        public readonly Dictionary<string, DateTime> spellCoolDown = new();

        ///<summary>
        ///Array of integers representing the IDs of useable healing items.
        ///The array includes IDs for potions and healthstones.
        ///</summary>
        private readonly int[] useableHealingItems = new int[]
                {
            // potions
            118, 929, 1710, 2938, 3928, 4596, 5509, 13446, 22829, 33447,
            // healthstones
            5509, 5510, 5511, 5512, 9421, 19013, 22103, 36889, 36892,
                };

        /// <summary>
        /// Array of integers representing useable mana items.
        /// </summary>
        private readonly int[] useableManaItems = new int[]
                {
            // potions
            2245, 3385, 3827, 6149, 13443, 13444, 33448, 22832,
                };

        /// <summary>
        /// Constructor for the BasicKamelClass.
        /// </summary>
        protected BasicKamelClass()
        {
            //Revive Spells
            spellCoolDown.Add(ancestralSpiritSpell, DateTime.Now);
            spellCoolDown.Add(redemptionSpell, DateTime.Now);
            spellCoolDown.Add(resurrectionSpell, DateTime.Now);

            //Basic
            AutoAttackEvent = new(TimeSpan.FromSeconds(1));
            TargetSelectEvent = new(TimeSpan.FromSeconds(1));
            RevivePlayerEvent = new(TimeSpan.FromSeconds(4));

            //Race (Troll)
            spellCoolDown.Add(BerserkingSpell, DateTime.Now);

            //Race (Draenei)
            spellCoolDown.Add(giftOfTheNaaruSpell, DateTime.Now);

            //Race (Dwarf)
            spellCoolDown.Add(StoneformSpell, DateTime.Now);

            //Race (Human)
            spellCoolDown.Add(EveryManforHimselfSpell, DateTime.Now);

            PriorityTargetDisplayIds = new List<int>();
        }

        /// <summary>
        /// Gets or sets the name of the author.
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// Gets or sets the TimegatedEvent for the AutoAttackEvent property. 
        /// </summary>
        public TimegatedEvent AutoAttackEvent { get; private set; }

        /// <summary>
        /// Gets or sets the collection of blacklisted target display IDs.
        /// </summary>
        public IEnumerable<int> BlacklistedTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the interface for the AmeisenBot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; internal set; }

        /// <summary>
        /// Gets or sets the dictionary of strings and dynamic values.
        /// </summary>
        public abstract Dictionary<string, dynamic> C { get; set; }

        /// <summary>
        /// Gets or sets the collection of configurable items represented by a dictionary.
        /// The keys of the dictionary are strings, and the values can be of any dynamic type.
        /// </summary>
        public Dictionary<string, dynamic> Configureables { get; set; }

        /// <summary>
        /// Gets the description of the item.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the display name for the object.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this code handles facing.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public bool HandlesFacing => false;

        /// <summary>
        /// Gets a value indicating whether this class handles movement.
        /// </summary>
        /// <returns>True if this class handles movement; otherwise, false.</returns>
        public abstract bool HandlesMovement { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a melee character.
        /// </summary>
        public abstract bool IsMelee { get; }

        /// <summary>
        /// Gets or sets the item comparator used to compare items.
        /// </summary>
        public abstract IItemComparator ItemComparator { get; set; }

        /// <summary>
        /// Gets or sets the collection of priority target display IDs.
        /// </summary>
        /// <returns>
        /// An enumerable collection of integer values representing the priority target display IDs.
        /// </returns>
        public IEnumerable<int> PriorityTargetDisplayIds { get; set; }

        /// <summary>
        /// Gets or sets the revive player event that is time-gated.
        /// </summary>
        public TimegatedEvent RevivePlayerEvent { get; private set; }

        /// <summary>
        /// Gets the WowRole of the object.
        /// </summary>
        public abstract WowRole Role { get; }

        /// <summary>
        /// Gets the talent tree associated with the instance.
        /// </summary>
        public abstract TalentTree Talents { get; }

        /// <summary>
        /// Gets a value indicating whether the target is in the line of sight of the bot.
        /// </summary>
        public bool TargetInLineOfSight => Bot.Objects.IsTargetInLineOfSight;

        /// <summary>
        /// Gets or sets the TimegatedEvent for selecting a target.
        /// </summary>
        public TimegatedEvent TargetSelectEvent { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this character uses automated attacks.
        /// </summary>
        public abstract bool UseAutoAttacks { get; }

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind an enemy.
        /// </summary>
        public abstract bool WalkBehindEnemy { get; }

        /// <summary>
        /// Gets the instance of the WowClass.
        /// </summary>
        public abstract WowClass WowClass { get; }

        /// <summary>
        /// Attacks the target if it is within 3.0 units of the player's position. 
        /// Stops click to move, resets movement, and interacts with the target if close enough. 
        /// Otherwise, sets movement action to move towards the target's position.
        /// </summary>
        //follow the target
        public void AttackTarget()
        {
            IWowUnit target = Bot.Target;
            if (target == null)
            {
                return;
            }

            if (Bot.Player.Position.GetDistance(target.Position) <= 3.0)
            {
                Bot.Wow.StopClickToMove();
                Bot.Movement.Reset();
                Bot.Wow.InteractWithUnit(target);
            }
            else
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, target.Position);
            }
        }

        /// <summary>
        /// Change the target to attack if the current target is too far away.
        /// </summary>
        //Change target if target to far away
        public void ChangeTargetToAttack()
        {
            IEnumerable<IWowPlayer> PlayerNearPlayer = Bot.GetNearEnemies<IWowPlayer>(Bot.Player.Position, 15);

            IWowUnit target = Bot.Target;
            if (target == null)
            {
                return;
            }

            if (PlayerNearPlayer.Any() && Bot.Objects.Target.HealthPercentage >= 60 && Bot.Player.Position.GetDistance(target.Position) >= 20)
            {
                Bot.Wow.ClearTarget();
                return;
            }
        }

        /// <summary>
        /// Checks if a weapon slot has a specific enchantment.
        /// </summary>
        /// <param name="slot">The weapon slot to check.</param>
        /// <param name="enchantmentName">The name of the enchantment to search for.</param>
        /// <param name="spellToCastEnchantment">The spell to cast the enchantment.</param>
        /// <returns>True if the weapon slot does not have the specified enchantment and casting the spell is possible; otherwise, false.</returns>
        public bool CheckForWeaponEnchantment(WowEquipmentSlot slot, string enchantmentName, string spellToCastEnchantment)
        {
            if (Bot.Character.Equipment.Items.ContainsKey(slot))
            {
                int itemId = Bot.Character.Equipment.Items[slot].Id;

                if (itemId > 0)
                {
                    if (slot == WowEquipmentSlot.INVSLOT_MAINHAND)
                    {
                        IWowItem item = Bot.Objects.All.OfType<IWowItem>().FirstOrDefault(e => e.EntryId == itemId);

                        if (item != null
                            && !item.GetEnchantmentStrings().Any(e => e.Contains(enchantmentName))
                            && CustomCastSpellMana(spellToCastEnchantment))
                        {
                            return true;
                        }
                    }
                    else if (slot == WowEquipmentSlot.INVSLOT_OFFHAND)
                    {
                        IWowItem item = Bot.Objects.All.OfType<IWowItem>().LastOrDefault(e => e.EntryId == itemId);

                        if (item != null
                            && !item.GetEnchantmentStrings().Any(e => e.Contains(enchantmentName))
                            && CustomCastSpellMana(spellToCastEnchantment))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Casts a mana spell with the given spell name and optional flag to cast it on self.
        /// </summary>
        /// <param name="spellName">The name of the spell to be cast.</param>
        /// <param name="castOnSelf">Optional flag indicating whether the spell should be cast on self. Default is false.</param>
        /// <returns>
        /// Returns true if the spell was successfully cast, otherwise false.
        /// </returns>
        //Mana Spells
        public bool CustomCastSpellMana(string spellName, bool castOnSelf = false)
        {
            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
            {
                if (Bot.Target != null)
                {
                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if (Bot.Player.Mana >= spell.Costs && IsSpellReady(spellName))
                    {
                        double distance = Bot.Player.Position.GetDistance(Bot.Target.Position);

                        if ((spell.MinRange == 0 && spell.MaxRange == 0) || (spell.MinRange <= distance && spell.MaxRange >= distance))
                        {
                            Bot.Wow.CastSpell(spellName);
                            return true;
                        }
                    }
                }
                else
                {
                    Bot.Wow.ChangeTarget(Bot.Wow.PlayerGuid);

                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if (Bot.Player.Mana >= spell.Costs && IsSpellReady(spellName))
                    {
                        Bot.Wow.CastSpell(spellName);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Executes the actions for the bot.
        /// </summary>
        public void Execute()
        {
            ExecuteCC();

            if (Bot.Player.Race == WowRace.Human
            && (Bot.Player.IsDazed
                || Bot.Player.IsFleeing
                || Bot.Player.IsInfluenced
                || Bot.Player.IsPossessed))
            {
                if (IsSpellReady(EveryManforHimselfSpell))
                {
                    Bot.Wow.CastSpell(EveryManforHimselfSpell);
                }
            }

            if (Bot.Player.HealthPercentage < 50.0
            && (Bot.Player.Race == WowRace.Dwarf))
            {
                if (IsSpellReady(StoneformSpell))
                {
                    Bot.Wow.CastSpell(StoneformSpell);
                }
            }

            // Useable items, potions, etc.
            // ---------------------------- >

            if (Bot.Player.HealthPercentage < 20)
            {
                IWowInventoryItem healthItem = Bot.Character.Inventory.Items.FirstOrDefault(e => useableHealingItems.Contains(e.Id));

                if (healthItem != null)
                {
                    Bot.Wow.UseItemByName(healthItem.Name);
                }
            }

            if (Bot.Player.ManaPercentage < 20)
            {
                IWowInventoryItem manaItem = Bot.Character.Inventory.Items.FirstOrDefault(e => useableManaItems.Contains(e.Id));

                if (manaItem != null)
                {
                    Bot.Wow.UseItemByName(manaItem.Name);
                }
            }
        }

        /// <summary>
        /// Executes the command center.
        /// </summary>
        public abstract void ExecuteCC();

        /// <summary>
        /// Checks if a spell is ready by comparing the current time with the cooldown time of the specified spell.
        /// </summary>
        /// <param name="spellName">The name of the spell to check.</param>
        /// <returns>True if the spell is ready to use, false otherwise.</returns>
        public bool IsSpellReady(string spellName)
        {
            if (DateTime.Now > spellCoolDown[spellName])
            {
                spellCoolDown[spellName] = DateTime.Now + TimeSpan.FromMilliseconds(Bot.Wow.GetSpellCooldown(spellName));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads the dictionary of JSON objects into the Configureables property.
        /// </summary>
        public void Load(Dictionary<string, JsonElement> objects)
        {
            Configureables = objects["Configureables"].ToDyn();
        }

        /// <summary>
        /// This method is used to perform an action when the character is out of combat.
        /// </summary>
        public abstract void OutOfCombatExecute();

        /// <summary>
        /// Revives a party member using a specified revive spell.
        /// </summary>
        /// <param name="reviveSpellName">The name of the revive spell.</param>
        public void RevivePartyMember(string reviveSpellName)
        {
            List<IWowUnit> partyMemberToHeal = new(Bot.Objects.Partymembers)
            {
                Bot.Player
            };

            partyMemberToHeal = partyMemberToHeal.Where(e => e.IsDead).OrderBy(e => e.HealthPercentage).ToList();

            if (RevivePlayerEvent.Run() && partyMemberToHeal.Count > 0)
            {
                Bot.Wow.ChangeTarget(partyMemberToHeal.FirstOrDefault().Guid);
                CustomCastSpellMana(reviveSpellName);
            }
        }

        /// <summary>
        /// Saves the dictionary of configureables.
        /// </summary>
        public Dictionary<string, object> Save()
        {
            return new()
            {
                { "configureables", Configureables }
            };
        }

        /// <summary>
        /// Selects a target for the bot to attack.
        /// </summary>
        public void Targetselection()
        {
            if (TargetSelectEvent.Run())
            {
                IWowUnit nearTarget = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 50)
                .Where(e => !e.IsNotAttackable && (e.Type == WowObjectType.Player && (e.IsPvpFlagged && Bot.Db.GetReaction(e, Bot.Player) != WowUnitReaction.Friendly) || (e.IsInCombat)) || (e.IsInCombat && Bot.Db.GetUnitName(e, out string name) && name != "The Lich King" && !(Bot.Objects.MapId == WowMapId.DrakTharonKeep && e.CurrentlyChannelingSpellId == 47346)))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();//&& e.Type(Player)

                if (nearTarget != null)
                {
                    Bot.Wow.ChangeTarget(nearTarget.Guid);

                    if (!TargetInLineOfSight)
                    {
                        return;
                    }
                }
                else
                {
                    AttackTarget();
                }
            }
        }

        /// <summary>
        /// Method to select a target for tanking.
        /// </summary>
        public void TargetselectionTank()
        {
            if (TargetSelectEvent.Run())
            {
                IWowUnit nearTargetToTank = Bot.GetEnemiesTargetingPartyMembers<IWowUnit>(Bot.Player.Position, 60)
                .Where(e => e.IsInCombat && !e.IsNotAttackable && e.Type != WowObjectType.Player && Bot.Db.GetUnitName(Bot.Target, out string name) && name != "The Lich King" && name != "Anub'Rekhan" && !(Bot.Objects.MapId == WowMapId.DrakTharonKeep && e.CurrentlyChannelingSpellId == 47346))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

                if (nearTargetToTank != null)
                {
                    Bot.Wow.ChangeTarget(nearTargetToTank.Guid);

                    if (!TargetInLineOfSight)
                    {
                        return;
                    }
                    else
                    {
                        AttackTarget();
                    }
                }
                else
                {
                    IWowUnit nearTarget = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 80)
                    .Where(e => e.IsInCombat && !e.IsNotAttackable && e.Type == WowObjectType.Player)
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();//&& e.Type(Player)

                    if (nearTarget != null)
                    {
                        Bot.Wow.ChangeTarget(nearTarget.Guid);

                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                    }
                    else
                    {
                        AttackTarget();
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string representation of the current object, including the WowClass, Role, DisplayName, and Author.
        /// </summary>
        public override string ToString()
        {
            return $"[{WowClass}] [{Role}] {DisplayName} ({Author})";
        }

        /// <summary>
        /// Checks if the character has all four elemental totems (Earth, Air, Water, Fire) in their inventory using a case-insensitive comparison,
        /// or if the character has any item equipped in their ranged slot.
        /// </summary>
        /// <returns>True if the character has all four totems or an item equipped in the ranged slot, otherwise false.</returns>
        public bool TotemItemCheck()
        {
            return (Bot.Character.Inventory.Items.Any(e => e.Name.Equals("Earth Totem", StringComparison.OrdinalIgnoreCase)) &&
                Bot.Character.Inventory.Items.Any(e => e.Name.Equals("Air Totem", StringComparison.OrdinalIgnoreCase)) &&
                Bot.Character.Inventory.Items.Any(e => e.Name.Equals("Water Totem", StringComparison.OrdinalIgnoreCase)) &&
                Bot.Character.Inventory.Items.Any(e => e.Name.Equals("Fire Totem", StringComparison.OrdinalIgnoreCase))) ||
                (Bot.Character.Equipment.Items.ContainsKey(WowEquipmentSlot.INVSLOT_RANGED) &&
                Bot.Character.Equipment.Items[WowEquipmentSlot.INVSLOT_RANGED] != null);
        }
    }
}