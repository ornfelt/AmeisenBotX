using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Managers.Party
{
    /// <summary>
    /// Manages party member classification with role and spec detection.
    /// Uses hybrid approach: Lua inspect for accurate spec, aura fallback when unavailable.
    /// </summary>
    public class PartyManager : IPartyManager
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly Dictionary<ulong, PartyMemberInfo> memberCache = [];
        private readonly TimegatedEvent updateEvent;
        private readonly TimegatedEvent inspectEvent;
        private readonly TimegatedEvent periodicUpdateEvent;
        private List<PartyMemberInfo> membersList = [];
        private int currentInspectIndex = 0;
        private ulong pendingInspectGuid = 0;

        // ===== SPEC DETECTION AURA MAPPINGS =====
        private static readonly HashSet<string> WarriorProtAuras = ["Defensive Stance"];
        private static readonly HashSet<string> WarriorFuryAuras = ["Berserker Stance", "Bloodthirst"];
        private static readonly HashSet<string> PaladinProtAuras = ["Righteous Fury"];
        private static readonly HashSet<string> PaladinHolyAuras = ["Beacon of Light", "Sacred Shield"];
        private static readonly HashSet<string> DruidBearAuras = ["Bear Form", "Dire Bear Form"];
        private static readonly HashSet<string> DruidCatAuras = ["Cat Form"];
        private static readonly HashSet<string> DruidTreeAuras = ["Tree of Life"];
        private static readonly HashSet<string> DruidMoonkinAuras = ["Moonkin Form"];
        private static readonly HashSet<string> PriestShadowAuras = ["Shadowform"];
        private static readonly HashSet<string> ShamanEleAuras = ["Elemental Focus"];
        private static readonly HashSet<string> ShamanEnhAuras = ["Maelstrom Weapon"];
        // DK: Blood is the tank spec in 3.3.5a, Frost Presence alone doesn't mean tank
        private static readonly HashSet<string> DkBloodTankAuras = ["Blood Presence", "Blade Barrier", "Will of the Necropolis"];
        private static readonly HashSet<string> DkUnholyAuras = ["Unholy Presence", "Bone Shield"];

        // Talent tree indices for 3.3.5a (1-indexed)
        // Warrior: 1=Arms, 2=Fury, 3=Protection
        // Paladin: 1=Holy, 2=Protection, 3=Retribution
        // Hunter: 1=BM, 2=MM, 3=Survival
        // Rogue: 1=Assassination, 2=Combat, 3=Subtlety
        // Priest: 1=Discipline, 2=Holy, 3=Shadow
        // DK: 1=Blood (TANK in 3.3.5), 2=Frost (DPS), 3=Unholy (DPS)
        // Shaman: 1=Elemental, 2=Enhancement, 3=Restoration
        // Mage: 1=Arcane, 2=Fire, 3=Frost
        // Warlock: 1=Affliction, 2=Demonology, 3=Destruction
        // Druid: 1=Balance, 2=Feral, 3=Restoration

        public PartyManager(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            updateEvent = new TimegatedEvent(TimeSpan.FromSeconds(2));
            inspectEvent = new TimegatedEvent(TimeSpan.FromSeconds(3));
            periodicUpdateEvent = new TimegatedEvent(TimeSpan.FromSeconds(60));
        }

        public IReadOnlyList<PartyMemberInfo> Members => membersList;
        public IEnumerable<PartyMemberInfo> GetByRole(WowRole role) => membersList.Where(m => m.Role == role);
        public IEnumerable<PartyMemberInfo> GetHealers() => membersList.Where(m => m.IsHealer);
        public IEnumerable<PartyMemberInfo> GetTanks() => membersList.Where(m => m.IsTank);
        public IEnumerable<PartyMemberInfo> GetDps() => membersList.Where(m => m.Role == WowRole.Dps);
        public IEnumerable<PartyMemberInfo> GetCasters() => membersList.Where(m => m.IsCaster);
        public PartyMemberInfo GetMember(ulong guid) => memberCache.TryGetValue(guid, out PartyMemberInfo info) ? info : null;
        public bool IsHealer(ulong guid) => GetMember(guid)?.IsHealer ?? false;
        public bool IsCaster(ulong guid) => GetMember(guid)?.IsCaster ?? false;
        public bool IsTank(ulong guid) => GetMember(guid)?.IsTank ?? false;

        public void ForceUpdate()
        {
            DoUpdate();
            TryInspectNextMember();
        }

        public void Update()
        {
            if (updateEvent.Run())
            {
                DoUpdate();
            }

            if (periodicUpdateEvent.Run())
            {
                TryInspectNextMember();
            }
        }

        private void DoUpdate()
        {
            IEnumerable<IWowPlayer> partyPlayers = Bot.Objects.Partymembers.OfType<IWowPlayer>();
            HashSet<ulong> currentGuids = partyPlayers.Select(p => p.Guid).ToHashSet();

            // Remove members no longer in party (O(n) instead of O(n²))
            foreach (ulong guid in memberCache.Keys.Where(g => !currentGuids.Contains(g)).ToList())
            {
                memberCache.Remove(guid);
            }

            // Update/add current party members
            foreach (IWowPlayer player in partyPlayers)
            {
                if (!memberCache.TryGetValue(player.Guid, out PartyMemberInfo info))
                {
                    info = new PartyMemberInfo { Guid = player.Guid, Class = player.Class };
                    memberCache[player.Guid] = info;
                }
                UpdateMemberInfo(info, player);
            }

            membersList = [.. memberCache.Values];
            TryInspectNextMember();
        }

        private void TryInspectNextMember()
        {
            if (!inspectEvent.Run() || membersList.Count == 0 || Bot.Player == null)
            {
                return;
            }

            // Find members needing spec data who are nearby
            List<PartyMemberInfo> needsInspect = membersList
                .Where(m => m.Spec == WowSpecialization.None && m.Player != null && m.Player.DistanceTo(Bot.Player) < 30f)
                .ToList();

            if (needsInspect.Count == 0)
            {
                return;
            }

            currentInspectIndex = (currentInspectIndex + 1) % needsInspect.Count;
            PartyMemberInfo target = needsInspect[currentInspectIndex];

            // Find the party unit ID
            int partyIndex = 1;
            foreach (IWowUnit member in Bot.Objects.Partymembers)
            {
                if (member.Guid == target.Guid)
                {
                    pendingInspectGuid = target.Guid;
                    Bot.Wow.InspectUnit($"party{partyIndex}");
                    return;
                }
                partyIndex++;
            }
        }

        public void OnInspectReady()
        {
            if (pendingInspectGuid == 0)
            {
                return;
            }

            (int primary, int _, int _, int _) = Bot.Wow.GetInspectedUnitTalentSpec();
            if (primary > 0 && memberCache.TryGetValue(pendingInspectGuid, out PartyMemberInfo info))
            {
                info.Spec = MapTalentTreeToSpec(info.Class, primary);
                (info.Role, info.IsTank, info.IsHealer) = GetRoleFromSpec(info.Spec);
                info.IsCaster = IsCasterSpec(info.Spec);
            }

            pendingInspectGuid = 0;
        }

        private void UpdateMemberInfo(PartyMemberInfo info, IWowPlayer player)
        {
            info.Player = player;
            info.Name = Bot.Db.GetUnitName(player, out string name) ? name : $"Player_{player.Guid}";
            info.LastUpdated = DateTime.UtcNow;

            // Skip if we already have valid talent-based spec
            if (info.Spec != WowSpecialization.None)
            {
                return;
            }

            // Fallback: Aura-based detection (only if no talent data yet)
            HashSet<string> auras = player.Auras.Select(a => Bot.Db.GetSpellName(a.SpellId)).ToHashSet();
            (info.Spec, info.Role) = DetectSpecFromAuras(player.Class, auras);
            info.IsTank = info.Role == WowRole.Tank;
            info.IsHealer = info.Role == WowRole.Heal;
            info.IsCaster = IsCasterSpec(info.Spec);
        }

        private static WowSpecialization MapTalentTreeToSpec(WowClass playerClass, int primaryTree)
        {
            return playerClass switch
            {
                WowClass.Warrior => primaryTree switch { 1 => WowSpecialization.WarriorArms, 2 => WowSpecialization.WarriorFury, 3 => WowSpecialization.WarriorProtection, _ => WowSpecialization.None },
                WowClass.Paladin => primaryTree switch { 1 => WowSpecialization.PaladinHoly, 2 => WowSpecialization.PaladinProtection, 3 => WowSpecialization.PaladinRetribution, _ => WowSpecialization.None },
                WowClass.Hunter => primaryTree switch { 1 => WowSpecialization.HunterBeastmastery, 2 => WowSpecialization.HunterMarksmanship, 3 => WowSpecialization.HunterSurvival, _ => WowSpecialization.None },
                WowClass.Rogue => primaryTree switch { 1 => WowSpecialization.RogueAssassination, 2 => WowSpecialization.RogueCombat, 3 => WowSpecialization.RogueSubtlety, _ => WowSpecialization.None },
                WowClass.Priest => primaryTree switch { 1 => WowSpecialization.PriestDiscipline, 2 => WowSpecialization.PriestHoly, 3 => WowSpecialization.PriestShadow, _ => WowSpecialization.None },
                // DK: Blood=Tank (tree 1), Frost=DPS (tree 2), Unholy=DPS (tree 3) in 3.3.5a
                WowClass.Deathknight => primaryTree switch { 1 => WowSpecialization.DeathknightBlood, 2 => WowSpecialization.DeathknightFrost, 3 => WowSpecialization.DeathknightUnholy, _ => WowSpecialization.None },
                WowClass.Shaman => primaryTree switch { 1 => WowSpecialization.ShamanElemental, 2 => WowSpecialization.ShamanEnhancement, 3 => WowSpecialization.ShamanRestoration, _ => WowSpecialization.None },
                WowClass.Mage => primaryTree switch { 1 => WowSpecialization.MageArcane, 2 => WowSpecialization.MageFire, 3 => WowSpecialization.MageFrost, _ => WowSpecialization.None },
                WowClass.Warlock => primaryTree switch { 1 => WowSpecialization.WarlockAffliction, 2 => WowSpecialization.WarlockDemonology, 3 => WowSpecialization.WarlockDestruction, _ => WowSpecialization.None },
                WowClass.Druid => primaryTree switch { 1 => WowSpecialization.DruidBalance, 2 => WowSpecialization.DruidFeralCat, 3 => WowSpecialization.DruidRestoration, _ => WowSpecialization.None },
                _ => WowSpecialization.None
            };
        }

        private static (WowRole role, bool isTank, bool isHealer) GetRoleFromSpec(WowSpecialization spec)
        {
            return spec switch
            {
                // Tanks - Blood DK is tank in 3.3.5a
                WowSpecialization.WarriorProtection or WowSpecialization.PaladinProtection or
                WowSpecialization.DruidFeralBear or WowSpecialization.DeathknightBlood => (WowRole.Tank, true, false),

                // Healers
                WowSpecialization.PaladinHoly or WowSpecialization.PriestHoly or WowSpecialization.PriestDiscipline or
                WowSpecialization.ShamanRestoration or WowSpecialization.DruidRestoration => (WowRole.Heal, false, true),

                _ => (WowRole.Dps, false, false)
            };
        }

        private (WowSpecialization, WowRole) DetectSpecFromAuras(WowClass playerClass, HashSet<string> auras)
        {
            return playerClass switch
            {
                WowClass.Warrior => auras.Overlaps(WarriorProtAuras) ? (WowSpecialization.WarriorProtection, WowRole.Tank) :
                                    auras.Overlaps(WarriorFuryAuras) ? (WowSpecialization.WarriorFury, WowRole.Dps) :
                                    (WowSpecialization.WarriorArms, WowRole.Dps),
                WowClass.Paladin => auras.Overlaps(PaladinProtAuras) ? (WowSpecialization.PaladinProtection, WowRole.Tank) :
                                    auras.Overlaps(PaladinHolyAuras) ? (WowSpecialization.PaladinHoly, WowRole.Heal) :
                                    (WowSpecialization.PaladinRetribution, WowRole.Dps),
                WowClass.Druid => auras.Overlaps(DruidBearAuras) ? (WowSpecialization.DruidFeralBear, WowRole.Tank) :
                                  auras.Overlaps(DruidCatAuras) ? (WowSpecialization.DruidFeralCat, WowRole.Dps) :
                                  auras.Overlaps(DruidMoonkinAuras) ? (WowSpecialization.DruidBalance, WowRole.Dps) :
                                  auras.Overlaps(DruidTreeAuras) ? (WowSpecialization.DruidRestoration, WowRole.Heal) :
                                  (WowSpecialization.DruidRestoration, WowRole.Heal),
                WowClass.Priest => auras.Overlaps(PriestShadowAuras) ? (WowSpecialization.PriestShadow, WowRole.Dps) :
                                   (WowSpecialization.PriestHoly, WowRole.Heal),
                WowClass.Shaman => auras.Overlaps(ShamanEleAuras) ? (WowSpecialization.ShamanElemental, WowRole.Dps) :
                                   auras.Overlaps(ShamanEnhAuras) ? (WowSpecialization.ShamanEnhancement, WowRole.Dps) :
                                   (WowSpecialization.ShamanRestoration, WowRole.Heal),
                // DK: Blood is tank in 3.3.5a
                WowClass.Deathknight => auras.Overlaps(DkBloodTankAuras) ? (WowSpecialization.DeathknightBlood, WowRole.Tank) :
                                        auras.Overlaps(DkUnholyAuras) ? (WowSpecialization.DeathknightUnholy, WowRole.Dps) :
                                        (WowSpecialization.DeathknightFrost, WowRole.Dps),
                WowClass.Rogue => (WowSpecialization.RogueAssassination, WowRole.Dps),
                WowClass.Hunter => (WowSpecialization.HunterMarksmanship, WowRole.Dps),
                WowClass.Mage => (WowSpecialization.MageFire, WowRole.Dps),
                WowClass.Warlock => (WowSpecialization.WarlockAffliction, WowRole.Dps),
                _ => (WowSpecialization.None, WowRole.Dps)
            };
        }

        private static bool IsCasterSpec(WowSpecialization spec)
        {
            return spec is WowSpecialization.MageArcane or WowSpecialization.MageFire or WowSpecialization.MageFrost
                or WowSpecialization.WarlockAffliction or WowSpecialization.WarlockDemonology or WowSpecialization.WarlockDestruction
                or WowSpecialization.PriestShadow or WowSpecialization.PriestHoly or WowSpecialization.PriestDiscipline
                or WowSpecialization.ShamanElemental or WowSpecialization.ShamanRestoration
                or WowSpecialization.DruidBalance or WowSpecialization.DruidRestoration
                or WowSpecialization.PaladinHoly;
        }
    }
}
