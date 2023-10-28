using AmeisenBotX.Core.Engines.Combat.Helpers;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;

/// <summary>
/// Namespace for the ShamanElemental combat class in the AmeisenBotX.Core.Engines.Combat.Classes.Bia10 namespace.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Bia10
{
    /// <summary>
    /// Initializes a new instance of the ShamanElemental class with the specified bot.
    /// </summary>
    public class ShamanElemental : BasicCombatClassBia10
    {
        /// <summary>
        /// Initializes a new instance of the ShamanElemental class with the specified bot. 
        /// Adds jobs to the MyAuraManager and TargetAuraManager to keep active auras.
        /// Sets the interrupt spells for the InterruptManager.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public ShamanElemental(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.LightningShield, () =>
                Bot.Player.ManaPercentage > 60.0
                && ValidateSpell(Shaman335a.LightningShield, true)
                && TryCastSpell(Shaman335a.LightningShield, Bot.Player.Guid)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.WaterShield, () =>
                Bot.Player.ManaPercentage < 20.0
                && ValidateSpell(Shaman335a.WaterShield, true)
                && TryCastSpell(Shaman335a.WaterShield, Bot.Player.Guid)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.FlameShock, () =>
                Bot.Target?.HealthPercentage >= 5
                && ValidateSpell(Shaman335a.FlameShock, true)
                && TryCastSpell(Shaman335a.FlameShock, Bot.Wow.TargetGuid)));

            InterruptManager.InterruptSpells = new SortedList<int, InterruptManager.CastInterruptFunction>
            {
                { 0, x => TryCastSpell(Shaman335a.WindShear, x.Guid) },
                { 1, x => TryCastSpell(Shaman335a.Hex, x.Guid) }
            };
        }

        /// <summary>
        /// Gets the description for the CombatClass of the Elemental Shaman spec.
        /// </summary>
        public override string Description => "CombatClass for the Elemental Shaman spec.";

        /// <summary>
        /// Gets the display name for a Shaman Elemental character.
        /// </summary>
        public override string DisplayName => "Shaman Elemental";

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        /// Always returns false.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this entity is a melee entity.
        /// </summary>
        /// <returns>Always returns false as the entity is not melee.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for ShamanElementalComparator.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } =
                    new ShamanElementalComparator(null, new List<WowWeaponType>
                    {
                WowWeaponType.AxeTwoHand,
                WowWeaponType.MaceTwoHand,
                WowWeaponType.SwordTwoHand
                    });

        /// <summary>
        /// Gets or sets the role of the character in the World of Warcraft game, which is set to Damage Per Second (DPS).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets the talents of the talent tree.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new Dictionary<int, Talent>(),
            Tree2 = new Dictionary<int, Talent>(),
            Tree3 = new Dictionary<int, Talent>(),
        };

        /// <summary>
        /// Returns true if the character should use auto attacks; otherwise, returns false.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// This property indicates that the character cannot walk behind enemies.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class of type Shaman.
        /// </summary>
        public override WowClass WowClass => WowClass.Shaman;

        /// <summary>
        /// Overrides the base Execute method. Selects a spell by assigning a spell name to the variable spellName and obtaining the target's GUID. Tries to cast the selected spell on the target with the specified GUID.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            string spellName = SelectSpell(out ulong targetGuid);
            TryCastSpell(spellName, targetGuid);
        }

        /// <summary>
        /// Executes the specified code block when the character is out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HandleDeadPartyMembers(Shaman335a.AncestralSpirit))
            {
                return;
            }

            string enchSpellName = DecideWeaponEnchantment(out string enchantName);
            CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, enchantName, enchSpellName);
        }

        /// <summary>
        /// Determines the weapon enchantment based on the spells known by the character.
        /// </summary>
        /// <param name="enchantName">Outputs the name of the weapon enchantment.</param>
        /// <returns>The name of the chosen weapon enchantment, or an empty string if no enchantment is chosen.</returns>
        private string DecideWeaponEnchantment(out string enchantName)
        {
            if (Bot.Character.SpellBook.IsSpellKnown(Shaman335a.FlametongueWeapon))
            {
                enchantName = Shaman335a.FlametongueBuff;
                return Shaman335a.FlametongueWeapon;
            }
            if (Bot.Character.SpellBook.IsSpellKnown(Shaman335a.RockbiterWeapon))
            {
                enchantName = Shaman335a.RockbiterBuff;
                return Shaman335a.RockbiterWeapon;
            }

            enchantName = string.Empty;
            return string.Empty;
        }

        /// <summary>
        /// Selects the appropriate spell based on certain conditions and assigns the target's GUID to the 'targetGuid' parameter.
        /// </summary>
        /// <param name="targetGuid">The GUID of the target enemy or the player.</param>
        /// <returns>The spell to be used or an empty string if no spell is suitable.</returns>
        private string SelectSpell(out ulong targetGuid)
        {
            if (Bot.Player.HealthPercentage < DataConstants.HealSelfPercentage
                && ValidateSpell(Shaman335a.HealingWave, true))
            {
                targetGuid = Bot.Player.Guid;
                return Shaman335a.HealingWave;
            }
            if (Bot.Target.HealthPercentage >= 3
                && IsInSpellRange(Bot.Target, Shaman335a.EarthShock)
                && ValidateSpell(Shaman335a.EarthShock, true))
            {
                targetGuid = Bot.Target.Guid;
                return Shaman335a.EarthShock;
            }
            if (IsInSpellRange(Bot.Target, Shaman335a.LightningBolt)
                && ValidateSpell(Shaman335a.LightningBolt, true))
            {
                targetGuid = Bot.Target.Guid;
                return Shaman335a.LightningBolt;
            }

            targetGuid = 9999999;
            return string.Empty;
        }
    }
}