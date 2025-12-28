using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Logic.Leafs.Combat
{
    public class UsePotionsLeaf : INode
    {
        public string Name { get; } = "UsePotions";

        private AmeisenBotInterfaces Bot;
        private AmeisenBotConfig Config;
        private TimegatedEvent CheckEvent;

        // Constants used for logic
        private const int DangerousEnemyCount = 3;
        private const int DangerousLevelDiff = 2;

        public UsePotionsLeaf(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;
            CheckEvent = new TimegatedEvent(TimeSpan.FromSeconds(1));
        }

        public BtStatus Execute()
        {
            // Only run periodically to save performance
            if (!CheckEvent.Run())
            {
                return BtStatus.Success; // Never block, just pass
            }

            if (Bot.Player == null || Bot.Player.IsDead || !Bot.Player.IsInCombat)
            {
                return BtStatus.Success;
            }

            // 1. Health Potions (Crucial)
            if (Bot.Player.HealthPercentage < Config.HealthPotionThreshold)
            {
                // Use "Heal" to match "Health" and "Healing"
                if (TryUsePotionByType("Heal"))
                {
                    return BtStatus.Success;
                }
            }

            // 2. Mana Potions (Power users)
            if (Bot.Player.PowerType == WowPowerType.Mana && Bot.Player.ManaPercentage < Config.ManaPotionThreshold)
            {
                if (TryUsePotionByType("Mana"))
                {
                    return BtStatus.Success;
                }
            }

            // 3. Conditional Buff/Protection Potions
            if (IsDangerousSituation())
            {
                // Try use "Potion of Speed", "Wild Magic", "Armor", etc.
                // We'll search generic "Potion" and filter by buff effects via Lua or name match
                // For now, let's look for known beneficial combat potions
                TryUseBuffPotion();
            }

            return BtStatus.Success;
        }

        private bool IsDangerousSituation()
        {
            // Check for tough enemies
            IEnumerable<IWowUnit> enemies = Bot.GetEnemiesOrNeutralsInCombatWithParty<IWowUnit>(Bot.Player.Position, 40f);

            int count = 0;
            foreach (IWowUnit unit in enemies)
            {
                count++;
                // Boss or Elite or High Level
                if (unit.Level >= Bot.Player.Level + DangerousLevelDiff)
                {
                    return true;
                }
            }

            return count >= DangerousEnemyCount;
        }

        private bool TryUsePotionByType(string type)
        {
            // Lua script returns "1" if used, "0" if not
            string luaScript = $@"
                local function UseBestPotion(pType)
                    for bag = 0, 4 do
                        for slot = 1, GetContainerNumSlots(bag) do
                            local itemId = GetContainerItemID(bag, slot)
                            if itemId then
                                local name, _, _, _, minLevel, type, subType, _, _, _, _, _, _, _, _, _ = GetItemInfo(itemId)
                                if (type == 'Consumable' or type == 'Verbrauchbar') then
                                    local lowerName = string.lower(name)
                                    local lowerType = string.lower(pType)
                                    if (string.find(lowerName, 'potion') or string.find(lowerName, 'trank')) and (string.find(lowerName, lowerType)) then
                                        -- Check cooldown
                                        local start, duration, enabled = GetContainerItemCooldown(bag, slot)
                                        if start == 0 and enabled == 1 then
                                            UseContainerItem(bag, slot)
                                            return '1'
                                        end
                                    end
                                end
                            end
                        end
                    end
                    return '0'
                end
                
                {{v:0}} = UseBestPotion('{type}')
            ";

            return Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua(luaScript), out string result) && result == "1";
        }

        private bool TryUseBuffPotion()
        {
            // Returns "1" if used
            string luaScript = @"
                local function UseBuffPotion()
                    -- Priority list of keywords for buffs
                    local keywords = {'Speed', 'Wild Magic', 'Armor', 'Geschwindigkeit', 'Wilder Magie', 'Rüstung'}
                    
                    for bag = 0, 4 do
                        for slot = 1, GetContainerNumSlots(bag) do
                            local itemId = GetContainerItemID(bag, slot)
                            if itemId then
                                local name = GetItemInfo(itemId)
                                if name and (string.find(name, 'Potion') or string.find(name, 'Trank')) then
                                    for _, keyword in ipairs(keywords) do
                                        if string.find(name, keyword) then
                                            local start, duration, enabled = GetContainerItemCooldown(bag, slot)
                                            if start == 0 and enabled == 1 then
                                                UseContainerItem(bag, slot)
                                                return '1'
                                            end
                                        end
                                    end
                                end
                            end
                        end
                    end
                    return '0'
                end
                
                {v:0} = UseBuffPotion()
            ";

            return Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua(luaScript), out string result) && result == "1";
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}
