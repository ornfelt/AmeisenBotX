using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class DruidBalance : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DruidBalance"/> class.
        /// </summary>
        /// <param name="bot">The <see cref="AmeisenBotInterfaces"/> instance to use.</param>
        public DruidBalance(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MoonkinForm, () => TryCastSpell(Druid335a.MoonkinForm, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.Thorns, () => TryCastSpell(Druid335a.Thorns, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MarkOfTheWild, () => TryCastSpell(Druid335a.MarkOfTheWild, Bot.Wow.PlayerGuid, true, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.Moonfire, () => LunarEclipse && TryCastSpell(Druid335a.Moonfire, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.InsectSwarm, () => SolarEclipse && TryCastSpell(Druid335a.InsectSwarm, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Druid335a.FaerieFire, x.Guid, true) },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Druid335a.MarkOfTheWild, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            SolarEclipse = false;
            LunarEclipse = true;
        }

        /// <summary>
        /// The description of the FCFS based CombatClass for the Balance (Owl) Druid spec.
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Balance (Owl) Druid spec.";

        /// <summary>
        /// Gets the display name for a balance druid.
        /// </summary>
        /// <returns>
        /// The display name, which is "Druid Balance".
        /// </returns>
        public override string DisplayName2 => "Druid Balance";

        /// This property indicates that this class does not handle movement.
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the code represents a melee attack or not.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for the player character.
        /// The default value is a BasicIntellectComparator that compares items based on
        /// WowArmorType.Shield and WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe });

        /// <summary>
        /// Gets or sets the last time the eclipse check was performed.
        /// </summary>
        public DateTime LastEclipseCheck { get; private set; }

        /// Gets or sets a value indicating whether a lunar eclipse occurs.
        public bool LunarEclipse { get; set; }

        /// <summary>
        /// Gets or sets the role of the character in the World of Warcraft game. The role is set to Dps, indicating that the character is a damage-dealer.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets or sets a value indicating whether a solar eclipse is happening.
        /// </summary>
        public bool SolarEclipse { get; set; }

        ///<summary>
        ///The TalentTree object representing the talent trees for a character.
        ///</summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },
                { 3, new(1, 3, 1) },
                { 4, new(1, 4, 2) },
                { 5, new(1, 5, 2) },
                { 7, new(1, 7, 3) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 5) },
                { 11, new(1, 11, 3) },
                { 12, new(1, 12, 3) },
                { 13, new(1, 13, 1) },
                { 16, new(1, 16, 3) },
                { 17, new(1, 17, 2) },
                { 18, new(1, 18, 1) },
                { 19, new(1, 19, 3) },
                { 20, new(1, 20, 3) },
                { 22, new(1, 22, 5) },
                { 23, new(1, 23, 3) },
                { 25, new(1, 25, 1) },
                { 26, new(1, 26, 2) },
                { 27, new(1, 27, 3) },
                { 28, new(1, 28, 1) },
            },
            Tree2 = new(),
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 3, new(3, 3, 5) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 3) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 2) },
            },
        };

        /// This property indicates that the character does not use auto attacks.
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets or sets the version of the code.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind enemies. 
        /// In this case, it is set to false, indicating that the player cannot walk behind the enemy.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the wow class of the character as a Druid.
        /// </summary>
        public override WowClass WowClass => WowClass.Druid;

        /// <summary>
        /// Gets or sets the WoW version to Wrath of the Lich King 3.3.5a.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// This method is used to execute the specified code. It first calls the base.Execute() method, then checks for Eclipse procs. If the method is successful in finding a target, it attempts to cast NaturesGrasp. If the distance between the target and player is less than 12.0 and the target has the EntanglingRoots aura, it tries to cast EntanglingRoots. If the player needs to heal themselves, the method returns. If the player's mana percentage is less than 30, it tries to cast Innervate. If the player's health percentage is less than 70, it tries to cast Barkskin. If LunarEclipse is active, it tries to cast Starfire. If SolarEclipse is active, it tries to cast Wrath. If there are less than 4 non-combat units within a 35 unit radius of the player, it tries to cast Starfall. Finally, it tries to cast ForceOfNature and clicks on the terrain.
        public override void Execute()
        {
            base.Execute();

            CheckForEclipseProcs();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (TryCastSpell(Druid335a.NaturesGrasp, 0))
                {
                    return;
                }

                double distance = Bot.Target.Position.GetDistance(Bot.Player.Position);

                if (distance < 12.0
                    && Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.EntanglingRoots)
                    && TryCastSpellDk(Druid335a.EntanglingRoots, Bot.Wow.TargetGuid, false, false, true))
                {
                    return;
                }

                if (NeedToHealMySelf())
                {
                    return;
                }

                if ((Bot.Player.ManaPercentage < 30
                        && TryCastSpell(Druid335a.Innervate, 0))
                    || (Bot.Player.HealthPercentage < 70
                        && TryCastSpell(Druid335a.Barkskin, 0, true))
                    || (LunarEclipse
                        && TryCastSpell(Druid335a.Starfire, Bot.Wow.TargetGuid, true))
                    || (SolarEclipse
                        && TryCastSpell(Druid335a.Wrath, Bot.Wow.TargetGuid, true))
                    || (Bot.Objects.All.OfType<IWowUnit>().Where(e => !e.IsInCombat && Bot.Player.Position.GetDistance(e.Position) < 35).Count() < 4
                        && TryCastSpell(Druid335a.Starfall, Bot.Wow.TargetGuid, true)))
                {
                    return;
                }

                if (TryCastSpell(Druid335a.ForceOfNature, 0, true))
                {
                    Bot.Wow.ClickOnTerrain(Bot.Player.Position);
                }
            }
        }

        /// <summary>
        /// Method to execute when out of combat. Calls the base method first and checks if there is a need to heal.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealMySelf())
            {
                return;
            }
        }

        /// <summary>
        /// Checks if there are any Eclipse procs active on the player.
        /// If Eclipse Lunar proc is active, sets SolarEclipse to false and LunarEclipse to true.
        /// If Eclipse Solar proc is active, sets SolarEclipse to true and LunarEclipse to false.
        /// Updates the LastEclipseCheck variable to the current UTC time.
        /// Returns false.
        /// </summary>
        private bool CheckForEclipseProcs()
        {
            if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.EclipseLunar))
            {
                SolarEclipse = false;
                LunarEclipse = true;
            }
            else if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.EclipseSolar))
            {
                SolarEclipse = true;
                LunarEclipse = false;
            }

            LastEclipseCheck = DateTime.UtcNow;
            return false;
        }

        /// Checks if the player needs to heal themselves based on their current health percentage and available healing spells. Returns true if healing is needed, false otherwise.
        private bool NeedToHealMySelf()
        {
            if (Bot.Player.HealthPercentage < 60
                && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Rejuvenation)
                && TryCastSpell(Druid335a.Rejuvenation, 0, true))
            {
                return true;
            }

            if (Bot.Player.HealthPercentage < 40
                && TryCastSpell(Druid335a.HealingTouch, 0, true))
            {
                return true;
            }

            return false;
        }
    }
}