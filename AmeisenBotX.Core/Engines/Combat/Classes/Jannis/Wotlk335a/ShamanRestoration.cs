using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a restoration shaman combat class that is derived from the BasicCombatClass.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a restoration shaman combat class that is derived from the BasicCombatClass.
    /// </summary>
    public class ShamanRestoration : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the ShamanRestoration class.
        /// </summary>
        /// <param name="bot">An instance of the AmeisenBotInterfaces that the ShamanRestoration class is based on.</param>
        public ShamanRestoration(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Shaman335a.WaterShield, () => TryCastSpell(Shaman335a.WaterShield, 0, true)));

            SpellUsageHealDict = new Dictionary<int, string>()
            {
                { 0, Shaman335a.Riptide },
                { 5000, Shaman335a.HealingWave },
            };
        }

        /// <summary>
        /// Gets the description of the FCFS based CombatClass for the Restoration Shaman spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Restoration Shaman spec.";

        /// <summary>
        /// Gets the display name for a Shaman with the Restoration specialization.
        /// </summary>
        public override string DisplayName2 => "Shaman Restoration";

        /// <summary>
        /// Gets a value indicating whether this code handles movement.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether it is a melee attack.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for comparing items.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator(new() { WowArmorType.Shield });

        /// <summary>
        /// Gets the role of the character as a healer.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// <summary>
        /// Gets or sets the talent tree of the character.
        /// </summary>
        /// <value>
        /// The talent tree consisting of three trees: Tree1, Tree2, and Tree3. Each tree contains a dictionary with key-value pairs representing the talent points and the corresponding talents.
        /// </value>
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
        /// Gets or sets a value indicating whether the character should use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character should use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => false;

        ///<summary>Returns the version of the object.</summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind an enemy.
        /// </summary>
        /// <returns>Returns false indicating that walking behind an enemy is not allowed.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass for the Shaman character.
        /// </summary>
        public override WowClass WowClass => WowClass.Shaman;

        /// <summary>
        /// Gets or sets the World of Warcraft version, which is set to Wrath of the Lich King (3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the dictionary that stores the spell ID as the key and the corresponding heal value as the value.
        /// </summary>
        private Dictionary<int, string> SpellUsageHealDict { get; }

        /// <summary>
        /// Executes the action, including base execution and checking if healing someone is needed.
        /// If healing is not needed, attempts to find a target and casts spells accordingly.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (NeedToHealSomeone())
            {
                return;
            }

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Shaman335a.FlameShock)
                    && TryCastSpell(Shaman335a.FlameShock, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (TryCastSpell(Shaman335a.LightningBolt, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Executes actions when the character is out of combat. This method
        /// checks if any party members are dead and uses the Ancestral Spirit
        /// ability to revive them if necessary. If any party members are revived,
        /// the method returns. Otherwise, it checks if the character's main hand
        /// weapon has the Earthliving buff and the Earthliving weapon enchantment.
        /// If not, the method returns.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HandleDeadPartymembers(Shaman335a.AncestralSpirit))
            {
                return;
            }

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, Shaman335a.EarthlivingBuff, Shaman335a.EarthlivingWeapon))
            {
                return;
            }
        }

        /// <summary>
        /// Checks if there is a need to heal someone.
        /// </summary>
        /// <returns>True if there is a need to heal someone, otherwise false.</returns>
        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                Bot.Wow.ChangeTarget(unitsToHeal.First().Guid);

                if (Bot.Target != null)
                {
                    if (Bot.Target.HealthPercentage < 25
                        && TryCastSpell(Shaman335a.EarthShield, 0, true))
                    {
                        return true;
                    }

                    if (unitsToHeal.Count() > 4
                        && TryCastSpell(Shaman335a.ChainHeal, Bot.Wow.TargetGuid, true))
                    {
                        return true;
                    }

                    if (unitsToHeal.Count() > 6
                        && (TryCastSpell(Shaman335a.NaturesSwiftness, 0, true)
                        || TryCastSpell(Shaman335a.TidalForce, Bot.Wow.TargetGuid, true)))
                    {
                        return true;
                    }

                    double healthDifference = Bot.Target.MaxHealth - Bot.Target.Health;
                    List<KeyValuePair<int, string>> spellsToTry = SpellUsageHealDict.Where(e => e.Key <= healthDifference).ToList();

                    foreach (KeyValuePair<int, string> keyValuePair in spellsToTry.OrderByDescending(e => e.Value))
                    {
                        if (TryCastSpell(keyValuePair.Value, Bot.Wow.TargetGuid, true))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}