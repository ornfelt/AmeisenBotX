using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Flags;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a unit in the game world with various attributes and behaviors.
    /// </summary>
    /// <remarks>
    /// This interface inherits from the <see cref="IWowObject"/> interface and provides additional properties and methods specific to units.
    /// </remarks>
    public interface IWowUnit : IWowObject
    {
        /// <summary>
        /// Gets the number of Aura instances.
        /// </summary>
        int AuraCount { get; }

        /// <summary>
        /// Gets the collection of World of Warcraft auras.
        /// </summary>
        /// <returns>An IEnumerable of type IWowAura.</returns>
        IEnumerable<IWowAura> Auras { get; }

        /// <summary>
        /// Represents a class named WowClass.
        /// </summary>
        WowClass Class { get; }

        /// <summary>
        /// Gets the combat reach of the character.
        /// </summary>
        float CombatReach { get; }

        /// <summary>
        /// Gets the ID of the currently casting spell.
        /// </summary>
        int CurrentlyCastingSpellId { get; }

        /// <summary>
        /// Gets the ID of the currently channeling spell.
        /// </summary>
        int CurrentlyChannelingSpellId { get; }

        /// <summary>
        /// Gets the display ID.
        /// </summary>
        int DisplayId { get; }

        /// <summary>
        /// Gets the value of the Energy property.
        /// </summary>
        int Energy { get; }

        /// <summary>
        /// Gets the energy percentage.
        /// </summary>
        double EnergyPercentage { get; }

        /// <summary>
        /// Gets or sets the faction template.
        /// </summary>
        int FactionTemplate { get; }

        /// <summary>
        /// Gets the value of the WowGender property.
        /// </summary>
        WowGender Gender { get; }

        /// <summary>
        /// Gets the health value.
        /// </summary>
        int Health { get; }

        /// <summary>
        /// Gets the health percentage.
        /// </summary>
        double HealthPercentage { get; }

        ///<summary>
        ///Gets the amount of Holy Power.
        ///</summary>
        int HolyPower { get; }

        /// <summary>
        /// Gets a boolean value indicating whether the unit is an ammo vendor.
        /// </summary>
        bool IsAmmoVendor => NpcFlags[(int)WowUnitNpcFlag.AmmoVendor];

        /// <summary>
        /// Gets a value indicating whether the NPC is an auctioneer.
        /// </summary>
        bool IsAuctioneer => NpcFlags[(int)WowUnitNpcFlag.Auctioneer];

        /// <summary>
        /// Gets a value indicating whether the object is currently performing an auto-attack.
        /// </summary>
        bool IsAutoAttacking { get; }

        /// <summary>
        /// Checks if the NPC is a banker.
        /// </summary>
        bool IsBanker => NpcFlags[(int)WowUnitNpcFlag.Banker];

        /// <summary>
        /// Gets a value indicating whether the character is a battlemaster NPC.
        /// </summary>
        bool IsBattlemaster => NpcFlags[(int)WowUnitNpcFlag.Battlemaster];

        /// <summary>
        /// Determines if the character is currently performing a casting or channeling spell.
        /// </summary>
        bool IsCasting => CurrentlyCastingSpellId > 0 || CurrentlyChannelingSpellId > 0;

        /// <summary>
        /// Gets a value indicating whether the unit is a class trainer.
        /// </summary>
        bool IsClassTrainer => NpcFlags[(int)WowUnitNpcFlag.ClassTrainer];

        /// <summary>
        /// Checks if the unit is confused based on the value of the specified flag in the unit flags array.
        /// </summary>
        bool IsConfused => UnitFlags[(int)WowUnitFlag.Confused];

        /// <summary>
        /// Gets a boolean value indicating if the unit is dazed.
        /// </summary>
        bool IsDazed => UnitFlags[(int)WowUnitFlag.Dazed];

        /// <summary>
        /// Gets a value indicating whether the object is dead.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Checks if the unit is disarmed by retrieving the value from the UnitFlags array using the WowUnitFlag.Disarmed index.
        /// </summary>
        bool IsDisarmed => UnitFlags[(int)WowUnitFlag.Disarmed];

        /// <summary>
        /// Gets a boolean value indicating whether the unit is currently fleeing.
        /// </summary>
        bool IsFleeing => UnitFlags[(int)WowUnitFlag.Fleeing];

        /// <summary>
        /// Checks if the character is a flight master based on the NpcFlags.
        /// </summary>
        bool IsFlightMaster => NpcFlags[(int)WowUnitNpcFlag.FlightMaster];

        /// <summary>
        /// Gets a value indicating whether the NPC is a food vendor.
        /// </summary>
        bool IsFoodVendor => NpcFlags[(int)WowUnitNpcFlag.FoodVendor];

        /// <summary>
        /// Gets a value indicating whether the NPC has gossip options.
        /// </summary>
        bool IsGossip => NpcFlags[(int)WowUnitNpcFlag.Gossip];

        /// <summary>
        /// Gets or sets a value indicating whether this NPC is a guard.
        /// </summary>
        bool IsGuard => NpcFlags[(int)WowUnitNpcFlag.Guard];

        /// <summary>
        /// Gets a boolean value indicating whether the character is a guild banker.
        /// </summary>
        bool IsGuildBanker => NpcFlags[(int)WowUnitNpcFlag.GuildBanker];

        /// <summary>
        /// Gets a boolean value indicating whether the unit is currently in combat.
        /// </summary>
        bool IsInCombat => UnitFlags[(int)WowUnitFlag.Combat];

        /// <summary>
        /// Gets a boolean value indicating whether the unit is influenced based on the WoW unit flags.
        /// </summary>
        bool IsInfluenced => UnitFlags[(int)WowUnitFlag.Influenced];

        /// <summary>
        /// Gets a value indicating whether the character is an innkeeper.
        /// </summary>
        bool IsInnkeeper => NpcFlags[(int)WowUnitNpcFlag.Innkeeper];

        /// <summary>
        /// Determines if the unit is in a taxi flight.
        /// </summary>
        bool IsInTaxiFlight => UnitFlags[(int)WowUnitFlag.TaxiFlight];

        /// <summary>
        /// Gets a value indicating whether the object is lootable.
        /// </summary>
        bool IsLootable { get; }

        /// <summary>
        /// Gets a boolean value indicating whether the unit is currently looting.
        /// </summary>
        bool IsLooting => UnitFlags[(int)WowUnitFlag.Looting];

        /// <summary>
        /// Determines whether the unit is mounted by checking the value of the specified unit flag.
        /// </summary>
        bool IsMounted => UnitFlags[(int)WowUnitFlag.Mounted];

        /// <summary>
        /// Checks if the NPC has no flags.
        /// </summary>
        bool IsNoneNpc => NpcFlags[(int)WowUnitNpcFlag.None];

        /// <summary>
        /// Gets a value indicating whether the unit is not attackable.
        /// </summary>
        bool IsNotAttackable => UnitFlags[(int)WowUnitFlag.NotAttackable];

        ///<summary>
        ///This property returns a boolean value indicating whether the current unit is not selectable.
        ///</summary>
        bool IsNotSelectable => UnitFlags[(int)WowUnitFlag.NotSelectable];

        /// <summary>
        /// Gets a value indicating whether the pet is currently in combat.
        /// </summary>
        bool IsPetInCombat => UnitFlags[(int)WowUnitFlag.PetInCombat];

        /// <summary>
        /// Gets a value indicating whether the NPC has the Petitioner flag.
        /// </summary>
        bool IsPetition => NpcFlags[(int)WowUnitNpcFlag.Petitioner];

        /// <summary>
        /// Gets a boolean value indicating whether the unit is controlled by a player.
        /// </summary>
        bool IsPlayerControlled => UnitFlags[(int)WowUnitFlag.PlayerControlled];

        /// <summary>
        /// Gets a boolean value indicating if the unit has the PlusMob flag set.
        /// </summary>
        bool IsPlusMob => UnitFlags[(int)WowUnitFlag.PlusMob];

        /// <summary>
        /// Returns whether the NPC is a poison vendor.
        /// </summary>
        bool IsPoisonVendor => NpcFlags[(int)WowUnitNpcFlag.PoisonVendor];

        /// <summary>
        /// Gets a value indicating whether the unit is possessed based on the UnitFlags property.
        /// </summary>
        bool IsPossessed => UnitFlags[(int)WowUnitFlag.Possessed];

        /// <summary>
        /// Gets a value that indicates whether the NPC is a profession trainer.
        /// </summary>
        bool IsProfessionTrainer => NpcFlags[(int)WowUnitNpcFlag.ProfessionTrainer];

        /// <summary>
        /// Gets a value indicating whether the unit is PvP flagged.
        /// </summary>
        bool IsPvpFlagged => UnitFlags[(int)WowUnitFlag.PvPFlagged];

        /// <summary>
        /// Returns a boolean value indicating whether the character is a quest giver.
        /// </summary>
        bool IsQuestgiver => NpcFlags[(int)WowUnitNpcFlag.Questgiver];

        /// <summary>
        /// Gets a value indicating whether this NPC is a reagent vendor.
        /// </summary>
        bool IsReagentVendor => NpcFlags[(int)WowUnitNpcFlag.ReagentVendor];

        /// <summary>
        /// Gets or sets a value indicating whether the user is linked to a referral friend.
        /// </summary>
        bool IsReferAFriendLinked { get; }

        /// <summary>
        /// Gets a value indicating whether the character is a repairer NPC.
        /// </summary>
        bool IsRepairer => NpcFlags[(int)WowUnitNpcFlag.Repairer];

        /// <summary>
        /// Gets a value indicating whether the unit is silenced.
        /// </summary>
        bool IsSilenced => UnitFlags[(int)WowUnitFlag.Silenced];

        /// <summary>
        /// Gets a value indicating whether the unit is currently sitting.
        /// </summary>
        bool IsSitting => UnitFlags[(int)WowUnitFlag.Sitting];

        /// <summary>
        /// Returns a boolean value indicating if the unit is skinnable based on the value stored in the UnitFlags array at the index corresponding to WowUnitFlag.Skinnable.
        /// </summary>
        bool IsSkinnable => UnitFlags[(int)WowUnitFlag.Skinnable];

        ///<summary>Returns a boolean value indicating whether the information is special or not.</summary>
        bool IsSpecialInfo { get; }

        /// <summary>
        /// Gets a boolean value indicating if the Spellclick flag is set in the NpcFlags array.
        /// </summary>
        bool IsSpellclick => NpcFlags[(int)WowUnitNpcFlag.Spellclick];

        /// <summary>
        /// Gets or sets a value indicating whether this entity is a spirit guide.
        /// </summary>
        bool IsSpiritGuide => NpcFlags[(int)WowUnitNpcFlag.SpiritGuide];

        /// <summary>
        /// Gets a value indicating whether this unit is a spirit healer.
        /// </summary>
        bool IsSpiritHealer => NpcFlags[(int)WowUnitNpcFlag.SpiritHealer];

        /// <summary>
        /// Gets a boolean value indicating whether the current NPC is a stable master.
        /// </summary>
        bool IsStableMaster => NpcFlags[(int)WowUnitNpcFlag.StableMaster];

        /// <summary>
        /// Gets a boolean value indicating if the current NPC is a Tabard Designer.
        /// </summary>
        bool IsTabardDesigner => NpcFlags[(int)WowUnitNpcFlag.TabardDesigner];

        /// <summary>
        /// Gets a value indicating whether the object is tagged by the current user.
        /// </summary>
        bool IsTaggedByMe { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is tagged by other.
        /// </summary>
        bool IsTaggedByOther { get; }

        /// <summary>
        /// Gets a value indicating if the object is tapped by all threat lists.
        /// </summary>
        bool IsTappedByAllThreatList { get; }

        /// <summary>
        /// Returns a boolean value indicating whether the unit is a totem, based on the UnitFlags dictionary.
        /// </summary>
        bool IsTotem => UnitFlags[(int)WowUnitFlag.Totem];

        /// <summary>
        /// Gets a value indicating whether this unit is being tracked.
        /// </summary>
        bool IsTrackedUnit { get; }

        /// <summary>
        /// Gets a value indicating whether the unit is a trainer.
        /// </summary>
        bool IsTrainer => NpcFlags[(int)WowUnitNpcFlag.Trainer];

        /// <summary>
        /// Gets a value indicating whether the character is a vendor.
        /// </summary>
        bool IsVendor => NpcFlags[(int)WowUnitNpcFlag.Vendor];

        /// <summary>
        /// Gets the level.
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets the value of the Mana property.
        /// </summary>
        int Mana { get; }

        /// <summary>
        /// Gets the current percentage of mana.
        /// </summary>
        double ManaPercentage { get; }

        /// <summary>
        /// Gets the maximum energy value.
        /// </summary>
        int MaxEnergy { get; }

        /// <summary>
        /// Gets the maximum health value.
        /// </summary>
        int MaxHealth { get; }

        /// <summary>
        /// Gets the maximum holy power.
        /// </summary>
        int MaxHolyPower { get; }

        /// <summary>
        /// Gets the maximum mana value.
        /// </summary>
        int MaxMana { get; }

        /// <summary>
        /// Gets the maximum rage value.
        /// </summary>
        int MaxRage { get; }

        /// <summary>
        /// Gets the maximum amount of runic power.
        /// </summary>
        int MaxRunicPower { get; }

        /// <summary>
        /// Gets the maximum value for the secondary property.
        /// </summary>
        int MaxSecondary { get; }

        /// <summary>
        /// Gets the BitVector32 that represents the flags of the NPC.
        /// </summary>
        BitVector32 NpcFlags { get; }

        /// <summary>
        /// Gets the power type of the wow character.
        /// </summary>
        WowPowerType PowerType { get; }

        /// <summary>
        /// Gets the WOW race.
        /// </summary>
        WowRace Race { get; }

        /// <summary>
        /// Gets the value of the Rage property.
        /// </summary>
        int Rage { get; }

        /// <summary>
        /// Gets the rage percentage.
        /// </summary>
        double RagePercentage { get; }

        /// <summary>
        /// Returns the resource of the specified class.
        /// </summary>
        /// <param name="class">The class for which to determine the resource.</param>
        /// <returns>The resource type.</returns>
        int Resource => Class switch
        {
            WowClass.Deathknight => RunicPower,
            WowClass.Rogue => Energy,
            WowClass.Warrior => Rage,
            _ => Mana,
        };

        /// <summary>
        /// Gets the rotation value.
        /// </summary>
        float Rotation { get; }

        /// <summary>
        /// Gets the current amount of Runic Power.
        /// </summary>
        int RunicPower { get; }

        /// <summary>
        /// Gets the current Runic Power as a percentage.
        /// </summary>
        double RunicPowerPercentage { get; }

        /// <summary>
        /// Gets the value of the Secondary property.
        /// </summary>
        int Secondary { get; }

        /// <summary>
        /// Gets the secondary percentage.
        /// </summary>
        double SecondaryPercentage { get; }

        /// <summary>
        /// Gets or sets the unique identifier of the entity that summoned this instance.
        /// </summary>
        ulong SummonedByGuid { get; }

        /// <summary>
        /// Gets or sets the target GUID.
        /// </summary>
        ulong TargetGuid { get; }

        /// <summary>
        /// Gets the type of the WowObject, which is a Unit.
        /// </summary>
        public new WowObjectType Type => WowObjectType.Unit;

        /// <summary>
        /// Gets the unit flags represented by a BitVector32.
        /// </summary>
        BitVector32 UnitFlags { get; }

        /// <summary>
        /// Gets or sets the BitVector32 representation of the UnitFlags2 property.
        /// </summary>
        BitVector32 UnitFlags2 { get; }

        /// <summary>
        /// Gets the BitVector32 representing the dynamic unit flags.
        /// </summary>
        BitVector32 UnitFlagsDynamic { get; }

        /// <summary>
        /// Checks if the given unit is valid for attacking or selection.
        /// </summary>
        /// <param name="unit">The unit to be checked.</param>
        /// <returns>True if the unit is valid, otherwise false.</returns>
        static bool IsValid(IWowUnit unit)
        {
            return unit != null
                && !unit.IsNotAttackable
                && !unit.IsNotSelectable;
        }

        /// <summary>
        /// Determines if the provided WoW unit is valid and alive.
        /// </summary>
        /// <param name="unit">The WoW unit to check.</param>
        /// <returns>True if the provided unit is valid and not dead, otherwise false.</returns>
        static bool IsValidAlive(IWowUnit unit)
        {
            return IsValid(unit) && !unit.IsDead;
        }

        /// <summary>
        /// Determines if the given World of Warcraft unit is valid, alive, and currently in combat.
        /// </summary>
        /// <param name="unit">The World of Warcraft unit to check.</param>
        /// <returns>True if the unit is valid, alive, and in combat; otherwise, false.</returns>
        static bool IsValidAliveInCombat(IWowUnit unit)
        {
            return IsValidAlive(unit) && unit.IsInCombat;
        }

        /// <summary>
        /// Calculates the aggro range between the current <see cref="IWowUnit"/> instance and another <see cref="IWowUnit"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="IWowUnit"/> to calculate the aggro range to.</param>
        /// <returns>The aggro range as a float value.</returns>
        float AggroRangeTo(IWowUnit other);

        /// <summary>
        /// Checks if there is a buff with the specified spell ID.
        /// </summary>
        bool HasBuffById(int spellId);

        /// <summary>
        /// Determines whether the specified World of Warcraft unit is within melee range.
        /// </summary>
        /// <param name="wowUnit">The specific World of Warcraft unit to be checked.</param>
        /// <returns>
        /// True if the unit is within melee range; otherwise, false.
        /// </returns>
        bool IsInMeleeRange(IWowUnit wowUnit);

        /// <summary>
        /// Returns the melee range to the specified World of Warcraft unit.
        /// </summary>
        float MeleeRangeTo(IWowUnit wowUnit);

        /// <summary>
        /// Reads and returns a string value representing a name.
        /// </summary>
        string ReadName();

        /// <summary>
        /// Reads the type of the creature.
        /// </summary>
        WowCreatureType ReadType();
    }
}