using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    ///<summary>
    ///Constructor for ShamanEnhancement class.
    ///Initializes the AuraManager with two KeepActiveAuraJobs: one for Lightning Shield and one for Water Shield.
    ///The Lightning Shield job is added if the player's mana percentage is greater than 60.0 and the TryCastSpell method returns true for casting Lightning Shield.
    ///The Water Shield job is added if the player's mana percentage is less than 20.0 and the TryCastSpell method returns true for casting Water Shield.
    ///Initializes the TargetAuraManager with a KeepActiveAuraJobs: one for Flametongue Weapon and one for Windfury Weapon.
    ///The Flametongue Weapon job is added if the player's mana percentage is greater than 30.0 and the TryCastSpell method returns true for casting Flametongue Weapon.
    ///The Windfury Weapon job is added if the player's mana percentage is greater than 50.0 and the TryCastSpell method returns true for casting Windfury Weapon.
    ///</summary>
    public class ShamanEnhancement : BasicCombatClass
    {
        ///<summary>
        ///Constructor for ShamanEnhancement class.
        ///Initializes the AuraManager with two KeepActiveAuraJobs: one for Lightning Shield and one for Water Shield.
        ///The Lightning Shield job is added if the player's mana percentage is greater than 60.0 and the TryCastSpell method returns true for casting Lightning Shield.
        ///The Water Shield job is added if the player's mana percentage is less than 20.0 and the TryCastSpell method returns true for casting Water Shield.
        ///Initializes the TargetAuraManager with a KeepActiveAuraJob for Flame Shock.
        ///The Flame Shock job is added if the TryCastSpell method returns true for casting Flame Shock on the target.
        ///Sets the InterruptSpells dictionary of the InterruptManager with two entries:
        ///Entry 0 maps to the TryCastSpell method for casting Wind Shear with the target's GUID as the parameter.
        ///Entry 1 maps to the TryCastSpell method for casting Hex with the target's GUID as the parameter.
        ///</summary>
        public ShamanEnhancement(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.LightningShield, () => Bot.Player.ManaPercentage > 60.0 && TryCastSpell(Shaman335a.LightningShield, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.WaterShield, () => Bot.Player.ManaPercentage < 20.0 && TryCastSpell(Shaman335a.WaterShield, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.FlameShock, () => TryCastSpell(Shaman335a.FlameShock, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Shaman335a.WindShear, x.Guid, true) },
                { 1, (x) => TryCastSpell(Shaman335a.Hex, x.Guid, true) }
            };
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Enhancement Shaman spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Enhancement Shaman spec.";

        /// <summary>
        /// Gets or sets the display name for the Shaman Enhancement.
        /// </summary>
        public override string DisplayName2 => "Shaman Enhancement";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this object is a melee type.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for the current object.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand });

        /// <summary>
        /// Gets or sets the role of the character in the game "World of Warcraft" which is "Damage Per Second" (DPS).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// The available talent trees for this object.
        /// 
        /// - Tree1: Represents the first talent tree, with the following talents:
        ///     - Talent 2: Requires 1 point, has a rank of 2, and costs 5 resources.
        ///     - Talent 3: Requires 1 point, has a rank of 3, and costs 3 resources.
        ///     - Talent 5: Requires 1 point, has a rank of 5, and costs 3 resources.
        ///     - Talent 8: Requires 1 point, has a rank of 8, and costs 5 resources.
        /// 
        /// - Tree2: Represents the second talent tree, with various talents:
        ///     - Talent 3: Requires 2 points, has a rank of 3, and costs 5 resources.
        ///     - Talent 5: Requires 2 points, has a rank of 5, and costs 5 resources.
        ///     - Talent 7: Requires 2 points, has a rank of 7, and costs 3 resources.
        ///     - Talent 8: Requires 2 points, has a rank of 8, and costs 3 resources.
        ///     - Talent 9: Requires 2 points, has a rank of 9, and costs 1 resource.
        ///     - Talent 11: Requires 2 points, has a rank of 11, and costs 5 resources.
        ///     - Talent 13: Requires 2 points, has a rank of 13, and costs 2 resources.
        ///     - Talent 14: Requires 2 points, has a rank of 14, and costs 1 resource.
        ///     - Talent 15: Requires 2 points, has a rank of 15, and costs 3 resources.
        ///     - Talent 16: Requires 2 points, has a rank of 16, and costs 3 resources.
        ///     - Talent 17: Requires 2 points, has a rank of 17, and costs 3 resources.
        ///     - Talent 19: Requires 2 points, has a rank of 19, and costs 3 resources.
        ///     - Talent 20: Requires 2 points, has a rank of 20, and costs 1 resource.
        ///     - Talent 21: Requires 2 points, has a rank of 21, and costs 1 resource.
        ///     - Talent 22: Requires 2 points, has a rank of 22, and costs 3 resources.
        ///     - Talent 23: Requires 2 points, has a rank of 23, and costs 1 resource.
        ///     - Talent 24: Requires 2 points, has a rank of 24, and costs 2 resources.
        ///     - Talent 25: Requires 2 points, has a rank of 25, and costs 3 resources.
        ///     - Talent 26: Requires 2 points, has a rank of 26, and costs 1 resource.
        ///     - Talent 28: Requires 2 points, has a rank of 28, and costs 5 resources.
        ///     - Talent 29: Requires 2 points, has a rank of 29, and costs 1 resource.
        /// 
        /// - Tree3: Represents the third talent tree, which is empty.
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
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character should use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether this character can walk behind enemy units.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property, specifying that the class is Shaman.
        /// </summary>
        public override WowClass WowClass => WowClass.Shaman;

        /// <summary>
        /// Gets or sets the World of Warcraft version to Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets a value indicating whether the target is hexed.
        /// </summary>
        private bool HexedTarget { get; set; }

        /// <summary>
        /// Method for executing the specified logic.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, Shaman335a.FlametongueBuff, Shaman335a.FlametongueWeapon)
                    || CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_OFFHAND, Shaman335a.WindfuryBuff, Shaman335a.WindfuryWeapon))
                {
                    return;
                }

                if (Bot.Player.HealthPercentage < 30
                    && Bot.Target.Type == WowObjectType.Player
                    && TryCastSpell(Shaman335a.Hex, Bot.Wow.TargetGuid, true))
                {
                    HexedTarget = true;
                    return;
                }

                if (Bot.Player.HealthPercentage < 60
                    && TryCastSpell(Shaman335a.HealingWave, Bot.Wow.PlayerGuid, true))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if ((Bot.Target.MaxHealth > 10000000
                            && Bot.Target.HealthPercentage < 25
                            && TryCastSpell(Shaman335a.Heroism, 0))
                        || TryCastSpell(Shaman335a.Stormstrike, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Shaman335a.LavaLash, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Shaman335a.EarthShock, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Shaman335a.MaelstromWeapon)
                        && Bot.Player.Auras.FirstOrDefault(e => Bot.Db.GetSpellName(e.SpellId) == Shaman335a.MaelstromWeapon).StackCount >= 5
                        && TryCastSpell(Shaman335a.LightningBolt, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Executes actions when the character is out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HandleDeadPartymembers(Shaman335a.AncestralSpirit))
            {
                return;
            }

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, Shaman335a.FlametongueBuff, Shaman335a.FlametongueWeapon)
                || CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_OFFHAND, Shaman335a.WindfuryBuff, Shaman335a.WindfuryWeapon))
            {
                return;
            }

            HexedTarget = false;
        }
    }
}