using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    public class FirstAidService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;

        private readonly TimegatedEvent CheckThrottle = new(TimeSpan.FromMilliseconds(1000));
        private readonly TimegatedEvent UseThrottle = new(TimeSpan.FromMilliseconds(2000));

        public bool ShouldBandage { get; private set; }

        public void CheckState()
        {
            if (!CheckThrottle.Ready) return;
            CheckThrottle.Run();

            if (!Config.UseFirstAid || Bot.Player.IsInCombat || Bot.Player.IsDead || Bot.Player.IsGhost)
            {
                ShouldBandage = false;
                return;
            }

            // check for Debuff "Recently Bandaged" (11196)
            if (Bot.Player.Auras.Any(a => a.SpellId == 11196 || Bot.Db.GetSpellName(a.SpellId) == "Recently Bandaged"))
            {
                ShouldBandage = false;
                return;
            }

            // Health check (Use if health is missing > 15-20%)
            if (Bot.Player.HealthPercentage > 85)
            {
                ShouldBandage = false;
                return;
            }

            // Check if we have bandages
            if (!GetBestBandage(out _))
            {
                ShouldBandage = false;
                return;
            }

            ShouldBandage = true;
        }

        public BtStatus Execute()
        {
            if (Bot.Player.IsInCombat) return BtStatus.Failed;
            
            // If we are channeling (First Aid is a channel), waiting is success/ongoing
            if (Bot.Player.IsCasting)
            {
                return BtStatus.Ongoing;
            }

            // Re-check conditions
            if (Bot.Player.Auras.Any(a => a.SpellId == 11196 || Bot.Db.GetSpellName(a.SpellId) == "Recently Bandaged"))
                return BtStatus.Success; // Done

            if (GetBestBandage(out IWowInventoryItem bandage))
            {
                if (UseThrottle.Run())
                {
                    Bot.Movement.StopMovement(); // Must stop to bandage
                    if (Bot.Objects.Target?.Guid != Bot.Player.Guid)
                    {
                        Bot.Wow.ChangeTarget(Bot.Player.Guid);
                    }
                    
                    Bot.Wow.UseItemByName(bandage.Name);
                    
                    return BtStatus.Ongoing;
                }
                return BtStatus.Ongoing;
            }

            return BtStatus.Failed;
        }

        private bool GetBestBandage(out IWowInventoryItem bandage)
        {
            // Find best bandage we can use (Quality/Level high to low)
            bandage = Bot.Character.Inventory.Items
                .Where(i => i.Type == "Consumable" && i.Subtype == "Bandage" && i.RequiredLevel <= Bot.Player.Level)
                .OrderByDescending(i => i.ItemLevel)
                .FirstOrDefault();

            return bandage != null;
        }

        public void Reset()
        {
            ShouldBandage = false;
        }
    }
}
