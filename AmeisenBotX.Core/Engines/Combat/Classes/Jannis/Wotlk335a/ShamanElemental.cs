using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class ShamanElemental : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the ShamanElemental class.
        /// Initializes the jobs for MyAuraManager and TargetAuraManager, and sets the interrupt spells for InterruptManager.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public ShamanElemental(AmeisenBotInterfaces bot) : base(bot)
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
        /// Gets the description of the FCFS based CombatClass for the Elemental Shaman spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Elemental Shaman spec.";

        /// <summary>
        /// Gets the display name for a Shaman Elemental character.
        /// </summary>
        public override string DisplayName2 => "Shaman Elemental";

        /// <summary>
        /// Specifies that this method does not handle movement.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is not a melee type.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for comparing items.
        /// The default comparator is set to BasicIntellectComparator with the specified weapon types.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(null, new() { WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand });

        /// <summary>
        /// Gets the role of the WoW character, which is Damage per Second (DPS).
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// The talent tree for the character.
        /// 
        /// Tree 1:
        /// - Talent 1: Increases a certain attribute by 3.
        /// - Talent 2: Increases a certain attribute by 5.
        /// - Talent 3: Increases a certain attribute by 3.
        /// - Talent 7: Increases a certain attribute by 1.
        /// - Talent 8: Increases a certain attribute by 5.
        /// - Talent 9: Increases a certain attribute by 2.
        /// - Talent 10: Increases a certain attribute by 3.
        /// - Talent 11: Increases a certain attribute by 2.
        /// - Talent 12: Increases a certain attribute by 1.
        /// - Talent 13: Increases a certain attribute by 3.
        /// - Talent 14: Increases a certain attribute by 3.
        /// - Talent 15: Increases a certain attribute by 5.
        /// - Talent 16: Increases a certain attribute by 1.
        /// - Talent 17: Increases a certain attribute by 3.
        /// - Talent 18: Increases a certain attribute by 2.
        /// - Talent 19: Increases a certain attribute by 2.
        /// - Talent 20: Increases a certain attribute by 3.
        /// - Talent 22: Increases a certain attribute by 1.
        /// - Talent 23: Increases a certain attribute by 3.
        /// - Talent 24: Increases a certain attribute by 5.
        /// - Talent 25: Increases a certain attribute by 1.
        /// 
        /// Tree 2:
        /// - Talent 3: Increases a certain attribute by 5.
        /// - Talent 5: Increases a certain attribute by 5.
        /// - Talent 8: Increases a certain attribute by 3.
        /// - Talent 9: Increases a certain attribute by 1.
        /// 
        /// Tree 3: Empty talent tree.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 2, new(1, 2, 5) },
                { 3, new(1, 3, 3) },
                { 7, new(1, 7, 1) },
                { 8, new(1, 8, 5) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 2) },
                { 12, new(1, 12, 1) },
                { 13, new(1, 13, 3) },
                { 14, new(1, 14, 3) },
                { 15, new(1, 15, 5) },
                { 16, new(1, 16, 1) },
                { 17, new(1, 17, 3) },
                { 18, new(1, 18, 2) },
                { 19, new(1, 19, 2) },
                { 20, new(1, 20, 3) },
                { 22, new(1, 22, 1) },
                { 23, new(1, 23, 3) },
                { 24, new(1, 24, 5) },
                { 25, new(1, 25, 1) },
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 8, new(2, 8, 3) },
                { 9, new(2, 9, 1) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether this character should use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>false</c> if auto attacks should not be used; otherwise, <c>true</c>.
        /// </value>
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the enemy can be walked behind.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the <see cref="WowClass"/> of the character as a Shaman.
        /// </summary>
        public override WowClass WowClass => WowClass.Shaman;

        /// <summary>
        /// Gets the WOW version as WotLK335a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets a value indicating whether the target is hexed.
        /// </summary>
        private bool HexedTarget { get; set; }

        /// <summary>
        /// Executes the specified code.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
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
                    if ((Bot.Target.Position.GetDistance(Bot.Player.Position) < 6
                            && TryCastSpell(Shaman335a.Thunderstorm, Bot.Wow.TargetGuid, true))
                        || (Bot.Target.MaxHealth > 10000000
                            && Bot.Target.HealthPercentage < 25
                            && TryCastSpell(Shaman335a.Heroism, 0))
                        || TryCastSpell(Shaman335a.LavaBurst, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Shaman335a.ElementalMastery, 0))
                    {
                        return;
                    }

                    if ((Bot.Objects.All.OfType<IWowUnit>().Where(e => Bot.Target.Position.GetDistance(e.Position) < 16).Count() > 2 && TryCastSpell(Shaman335a.ChainLightning, Bot.Wow.TargetGuid, true))
                        || TryCastSpell(Shaman335a.LightningBolt, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the specified code block when the player is out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HandleDeadPartymembers(Shaman335a.AncestralSpirit))
            {
                return;
            }

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, Shaman335a.FlametongueBuff, Shaman335a.FlametongueWeapon))
            {
                return;
            }

            HexedTarget = false;
        }
    }
}