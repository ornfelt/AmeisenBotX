using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Inventory;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Managers
{
    public class ProfessionManager(AmeisenBotInterfaces bot)
    {
        private AmeisenBotInterfaces Bot { get; } = bot;
        private DateTime _lastProfessionAction = DateTime.MinValue;
        private const double ProfessionCooldown = 2; // Seconds between actions

        #region Recipes & Structures

        public class DiscoveredRecipe
        {
            public int ResultItemId; // The item produced
            public string Name;
            public int SpellId; // ID to cast (if available via GetTradeSkillInfo)
            public int Index; // Index in the TradeSkill window (Volatile! Only valid during scan session)
            public List<Reagent> Reagents = [];

            // Heuristic Data
            public string ItemClass;
            public string ItemSubClass;

            // Optimization Cache
            public bool? IsUpgrade;
            public bool? IsTradeGood;
            public DateTime LastUpgradeCheck;
        }

        public struct Reagent
        {
            public int ItemId;
            public int Count;
        }

        private List<DiscoveredRecipe> _knownRecipes = [];
        private DateTime _lastRecipeScan = DateTime.MinValue;

        // Crafting station detection
        private const float CraftingStationInteractionRange = 10.0f;
        private const float CraftingStationSearchRadius = 50.0f;
        private IWowGameobject _targetCraftingStation = null;

        /// <summary>
        /// Finds the closest forge/anvil object in range (for Smelting, Blacksmithing).
        /// </summary>
        private IWowGameobject FindNearbyForge()
        {
            return Bot.Objects.All.OfType<IWowGameobject>()
                .Where(g => g != null
                    && (g.GameObjectType == WowGameObjectType.SpellFocus || g.GameObjectType == WowGameObjectType.Goober)
                    && (g.Name.Contains("Forge", StringComparison.OrdinalIgnoreCase)
                        || g.Name.Contains("Anvil", StringComparison.OrdinalIgnoreCase)))
                .Where(g => g.Position.GetDistance(Bot.Player.Position) <= CraftingStationSearchRadius)
                .OrderBy(g => g.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Finds the closest fire/campfire object in range (for Cooking).
        /// </summary>
        private IWowGameobject FindNearbyCookingFire()
        {
            return Bot.Objects.All.OfType<IWowGameobject>()
                .Where(g => g != null
                    && (g.GameObjectType == WowGameObjectType.SpellFocus || g.GameObjectType == WowGameObjectType.Goober)
                    && (g.Name.Contains("Fire", StringComparison.OrdinalIgnoreCase)
                        || g.Name.Contains("Campfire", StringComparison.OrdinalIgnoreCase)
                        || g.Name.Contains("Brazier", StringComparison.OrdinalIgnoreCase)
                        || g.Name.Contains("Stove", StringComparison.OrdinalIgnoreCase)))
                .Where(g => g.Position.GetDistance(Bot.Player.Position) <= CraftingStationSearchRadius)
                .OrderBy(g => g.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Checks if we are close enough to a forge to smelt/blacksmith.
        /// </summary>
        private bool IsNearForge()
        {
            IWowGameobject forge = FindNearbyForge();
            return forge != null && forge.Position.GetDistance(Bot.Player.Position) <= CraftingStationInteractionRange;
        }

        /// <summary>
        /// Checks if we are close enough to a cooking fire.
        /// </summary>
        private bool IsNearCookingFire()
        {
            IWowGameobject fire = FindNearbyCookingFire();
            return fire != null && fire.Position.GetDistance(Bot.Player.Position) <= CraftingStationInteractionRange;
        }

        #endregion

        /// <summary>
        /// Scans the currently open TradeSkill window for recipes.
        /// Ensure a profession window is OPEN before calling this, or logic to open it is handled.
        /// </summary>
        private void ScanRecipes(string professionName)
        {
            // Map profession spell names to actual skill names for check
            string skillName = professionName switch
            {
                "Smelting" => "Mining",
                _ => professionName
            };

            if (!Bot.Character.Skills.ContainsKey(skillName))
            {
                return;
            }

            (string, string) script = BotUtils.ObfuscateLua($@"
                CastSpellByName('{professionName}');
                {{v:0}}='';
                local numSkills = GetNumTradeSkills();
                if numSkills and numSkills > 0 then
                    for i=1, numSkills do
                        local name, type, _, _, _ = GetTradeSkillInfo(i);
                        if type ~= 'header' then
                            local link = GetTradeSkillItemLink(i);
                            if link then
                                local _,_,id = string.find(link, 'item:(%d+):');
                                if id then
                                    {{v:0}}={{v:0}} .. id .. ':' .. i .. '=';
                                    local numReagents = GetTradeSkillNumReagents(i);
                                    for r=1, numReagents do
                                        local rName, _, rCount, _ = GetTradeSkillReagentInfo(i, r);
                                        local rLink = GetTradeSkillReagentItemLink(i, r);
                                        if rLink then
                                            local _,_,rid = string.find(rLink, 'item:(%d+):');
                                            if rid then
                                                {{v:0}}={{v:0}} .. rid .. ',' .. rCount .. ';';
                                            end
                                        end
                                    end
                                    {{v:0}}={{v:0}} .. '|';
                                end
                            end
                        end
                    end
                end
                CloseTradeSkill();
            ");

            if (Bot.Wow.ExecuteLuaAndRead(script, out string result))
            {
                string data = result;

                if (!string.IsNullOrEmpty(data))
                {
                    _knownRecipes.Clear(); // Replace list on fresh scan
                    string[] recipes = data.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string r in recipes)
                    {
                        string[] parts = r.Split('=');
                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        string[] info = parts[0].Split(':');
                        string reagentsStr = parts[1];

                        if (int.TryParse(info[0], out int resultId) && int.TryParse(info[1], out int index))
                        {
                            DiscoveredRecipe recipe = new()
                            {
                                ResultItemId = resultId,
                                Index = index,
                                Reagents = []
                            };

                            string[] reagentParts = reagentsStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string rp in reagentParts)
                            {
                                string[] rInfo = rp.Split(',');
                                if (rInfo.Length == 2 && int.TryParse(rInfo[0], out int rId) && int.TryParse(rInfo[1], out int rCount))
                                {
                                    recipe.Reagents.Add(new Reagent { ItemId = rId, Count = rCount });
                                }
                            }
                            _knownRecipes.Add(recipe);
                        }
                    }
                    _lastRecipeScan = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Attempts to execute dynamic crafting based on known recipes and heuristics.
        /// </summary>
        private bool TryCraftDynamic()
        {
            // 1. Scan if needed (retry every 30s if empty, else every 5min to refresh)
            double scanCooldown = _knownRecipes.Count == 0 ? 0.5 : 5; // 30s if empty, 5min otherwise
            if ((DateTime.Now - _lastRecipeScan).TotalMinutes > scanCooldown)
            {
                // Prioritize professions - scan all that we have
                if (Bot.Character.Skills.ContainsKey("Alchemy"))
                {
                    ScanRecipes("Alchemy");
                }
                else if (Bot.Character.Skills.ContainsKey("Mining"))
                {
                    ScanRecipes("Smelting");
                }
                else if (Bot.Character.Skills.ContainsKey("Leatherworking"))
                {
                    ScanRecipes("Leatherworking");
                }
                else if (Bot.Character.Skills.ContainsKey("First Aid"))
                {
                    ScanRecipes("First Aid");
                }
                else if (Bot.Character.Skills.ContainsKey("Cooking"))
                {
                    ScanRecipes("Cooking");
                }
                else if (Bot.Character.Skills.ContainsKey("Blacksmithing"))
                {
                    ScanRecipes("Blacksmithing");
                }
                else if (Bot.Character.Skills.ContainsKey("Tailoring"))
                {
                    ScanRecipes("Tailoring");
                }
                else if (Bot.Character.Skills.ContainsKey("Engineering"))
                {
                    ScanRecipes("Engineering");
                }
                else if (Bot.Character.Skills.ContainsKey("Jewelcrafting"))
                {
                    ScanRecipes("Jewelcrafting");
                }
                // Note: Only scans one per tick effectively if multiple exist, but fine for now.
                return true; // Busy scanning
            }

            // 2. Evaluate Recipes
            foreach (DiscoveredRecipe recipe in _knownRecipes)
            {
                // Safety: Stop if inventory is full
                if (Bot.Character.Inventory.FreeBagSlots < 1)
                {
                    return false;
                }

                if (CanCraft(recipe) && ShouldCraft(recipe))
                {
                    // Determine which profession to open and station requirements
                    string professionSpell = null;
                    bool needsForge = false;
                    bool needsFire = false;

                    // Determine profession based on skills (in priority order)
                    if (Bot.Character.Skills.ContainsKey("Alchemy"))
                    {
                        professionSpell = "Alchemy";
                    }
                    else if (Bot.Character.Skills.ContainsKey("Mining"))
                    {
                        professionSpell = "Smelting";
                        needsForge = true;
                    }
                    else if (Bot.Character.Skills.ContainsKey("Blacksmithing"))
                    {
                        professionSpell = "Blacksmithing";
                        needsForge = true;
                    }
                    else if (Bot.Character.Skills.ContainsKey("Cooking"))
                    {
                        professionSpell = "Cooking";
                        needsFire = true;
                    }
                    else if (Bot.Character.Skills.ContainsKey("Leatherworking"))
                    {
                        professionSpell = "Leatherworking";
                    }
                    else if (Bot.Character.Skills.ContainsKey("First Aid"))
                    {
                        professionSpell = "First Aid";
                    }
                    else if (Bot.Character.Skills.ContainsKey("Tailoring"))
                    {
                        professionSpell = "Tailoring";
                    }
                    else if (Bot.Character.Skills.ContainsKey("Engineering"))
                    {
                        professionSpell = "Engineering";
                    }
                    else if (Bot.Character.Skills.ContainsKey("Jewelcrafting"))
                    {
                        professionSpell = "Jewelcrafting";
                    }

                    if (professionSpell == null)
                    {
                        return false;
                    }

                    // Check if we need a forge and handle movement
                    if (needsForge)
                    {
                        IWowGameobject forge = FindNearbyForge();
                        if (forge == null)
                        {
                            // No forge in range at all, can't smelt/blacksmith
                            continue;
                        }

                        if (forge.Position.GetDistance(Bot.Player.Position) > CraftingStationInteractionRange)
                        {
                            _targetCraftingStation = forge;
                            Bot.Movement.SetMovementAction(MovementAction.Move, forge.Position);
                            return true; // Busy moving
                        }
                    }

                    // Check if we need a fire and handle movement
                    if (needsFire)
                    {
                        IWowGameobject fire = FindNearbyCookingFire();
                        if (fire == null)
                        {
                            // No fire in range at all, can't cook
                            continue;
                        }

                        if (fire.Position.GetDistance(Bot.Player.Position) > CraftingStationInteractionRange)
                        {
                            _targetCraftingStation = fire;
                            Bot.Movement.SetMovementAction(MovementAction.Move, fire.Position);
                            return true; // Busy moving
                        }
                    }

                    // Execute Craft
                    if (!Bot.Player.IsCasting)
                    {
                        // Combined Lua: Open window + Find recipe + Craft in one call
                        (string, string) script = BotUtils.ObfuscateLua($@"
                            CastSpellByName('{professionSpell}');
                            local targetId = {recipe.ResultItemId};
                            local numSkills = GetNumTradeSkills();
                            local done = false;
                            if numSkills and numSkills > 0 then
                                for i=1, numSkills do
                                    if not done then
                                        local link = GetTradeSkillItemLink(i);
                                        if link then
                                            local _,_,id = string.find(link, 'item:(%d+):');
                                            if id and tonumber(id) == targetId then
                                                 DoTradeSkill(i, 1);
                                                 done = true;
                                            end
                                        end
                                    end
                                end
                            end
                            CloseTradeSkill();
                        ");

                        Bot.Wow.LuaDoString(script.Item1);
                        _lastProfessionAction = DateTime.Now;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CanCraft(DiscoveredRecipe recipe)
        {
            foreach (Reagent r in recipe.Reagents)
            {
                if (Bot.Character.Inventory.GetItemCount(r.ItemId) < r.Count)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ShouldCraft(DiscoveredRecipe recipe)
        {
            // Heuristic: Is this item useful?
            if (CanCraft(recipe))
            {
                // Smelting/Leatherworking basic processing (Compression)
                // Always craft bars, bolts, leather (Basic Materials)
                // New Dynamic Logic: Check if Item Type is "Trade Goods"

                // We need Item Info for both Trade Goods check AND Upgrade check
                // So we do one combined check if cache is missing
                if (!recipe.IsUpgrade.HasValue || !recipe.IsTradeGood.HasValue || (DateTime.Now - recipe.LastUpgradeCheck).TotalMinutes > 30)
                {
                    // Reset Cache
                    recipe.IsUpgrade = false;
                    recipe.IsTradeGood = false;
                    recipe.LastUpgradeCheck = DateTime.Now;

                    try
                    {
                        // 1. Fetch Item Info via Lua
                        string json = GetItemInfoById(recipe.ResultItemId);
                        if (!string.IsNullOrEmpty(json) && json != "noItem")
                        {
                            // 2. Build Item Object
                            WowBasicItem basicItem = ItemFactory.ParseItem(json);
                            if (basicItem != null)
                            {
                                basicItem.Id = recipe.ResultItemId; // Ensure ID is set

                                // Check Trade Goods (Dynamic "always craft")
                                if (string.Equals(basicItem.Type, "Trade Goods", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(basicItem.Type, "Handwerkswaren", StringComparison.OrdinalIgnoreCase)) // German fallback just in case
                                {
                                    recipe.IsTradeGood = true;
                                }

                                // Check Improvement
                                WowBasicItem inventoryItem = ItemFactory.BuildSpecificItem(basicItem);
                                if (Bot.Character.IsItemAnImprovement(inventoryItem, out _))
                                {
                                    recipe.IsUpgrade = true;
                                }
                            }
                        }
                    }
                    catch { /* Ignore failures */ }
                }

                // Decision Logic
                if (recipe.IsTradeGood.Value)
                {
                    return true; // Always craft Trade Goods (Bars, Bolts, Leather)
                }

                if (recipe.IsUpgrade.Value)
                {
                    return true;   // Always craft Upgrades
                }

                // Consumables fallback
                int currentCount = Bot.Character.Inventory.GetItemCount(recipe.ResultItemId);
                if (currentCount < 10)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetItemInfoById(int itemId)
        {
            return Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:1}}={itemId};{{v:0}}='noItem';{{v:2}},{{v:3}},{{v:4}},{{v:5}},{{v:6}},{{v:7}},{{v:8}},{{v:9}},{{v:10}},{{v:11}},{{v:12}}=GetItemInfo({{v:1}});{{v:13}}=GetItemStats({{v:3}}){{v:14}}={{}}for c,d in pairs({{v:13}})do table.insert({{v:14}},string.format(\"\\\"%s\\\":\\\"%s\\\"\",c,d))end;{{v:0}}='{{'..'\"id\": \"0\",'..'\"count\": \"1\",'..'\"quality\": \"'..tostring({{v:4}} or 0)..'\",'..'\"curDurability\": \"0\",'..'\"maxDurability\": \"0\",'..'\"cooldownStart\": \"0\",'..'\"cooldownEnd\": \"0\",'..'\"name\": \"'..tostring({{v:2}} or 0)..'\",'..'\"link\": \"'..tostring({{v:3}} or 0)..'\",'..'\"level\": \"'..tostring({{v:5}} or 0)..'\",'..'\"minLevel\": \"'..tostring({{v:6}} or 0)..'\",'..'\"type\": \"'..tostring({{v:7}} or 0)..'\",'..'\"subtype\": \"'..tostring({{v:8}} or 0)..'\",'..'\"maxStack\": \"'..tostring({{v:9}} or 0)..'\",'..'\"equiplocation\": \"'..tostring({{v:10}} or 0)..'\",'..'\"sellprice\": \"'..tostring({{v:12}} or 0)..'\",'..'\"stats\": '..\"{{\"..table.concat({{v:14}},\",\")..\"}}\"..'}}';"), out string result) ? result : string.Empty;
        }

        private DateTime _lastTick = DateTime.MinValue;

        /// <summary>
        /// Executes idle profession jobs (Crafting, Smelting).
        /// Returns true if a job was executed (busy).
        /// </summary>
        public bool Tick()
        {
            // General Time Gating: Only check every 1.5s
            if ((DateTime.Now - _lastTick).TotalSeconds < 1.5)
            {
                return false;
            }

            _lastTick = DateTime.Now;

            if ((DateTime.Now - _lastProfessionAction).TotalSeconds < ProfessionCooldown)
            {
                return false;
            }

            if (Bot.Player.IsCasting || Bot.Player.IsInCombat)
            {
                return true;
            }

            // Ensure inventory is fresh before crafting to prevent "phantom" crafts
            Bot.Character.UpdateBags();

            // Try dynamic crafting first
            if (TryCraftDynamic())
            {
                return true;
            }

            // Disenchanting is a special case, not covered by dynamic crafting
            // DISABLED FOR SAFETY: Logic is too aggressive (disenchants all low-level greens/blues/epics)
            // if (TryDisenchanting()) return true;

            return false;
        }

        public bool HasPendingJob
        {
            get
            {
                if (Bot.Player.IsInCombat || Bot.Player.IsCasting)
                {
                    return false;
                }

                // Check if we need to scan (either empty or time for refresh)
                double scanCooldown = _knownRecipes.Count == 0 ? 0.5 : 5; // 30s if empty, 5min otherwise
                if ((DateTime.Now - _lastRecipeScan).TotalMinutes > scanCooldown)
                {
                    // Check if we have any scannable profession
                    if (Bot.Character.Skills.ContainsKey("Alchemy")
                        || Bot.Character.Skills.ContainsKey("Mining")
                        || Bot.Character.Skills.ContainsKey("Leatherworking")
                        || Bot.Character.Skills.ContainsKey("First Aid")
                        || Bot.Character.Skills.ContainsKey("Cooking")
                        || Bot.Character.Skills.ContainsKey("Blacksmithing")
                        || Bot.Character.Skills.ContainsKey("Tailoring")
                        || Bot.Character.Skills.ContainsKey("Engineering")
                        || Bot.Character.Skills.ContainsKey("Jewelcrafting"))
                    {
                        return true; // Need to scan!
                    }
                }

                // Check if any recipe ShouldCraft
                if (_knownRecipes.Any(r => CanCraft(r) && ShouldCraft(r)))
                {
                    return true;
                }

                // Check Disenchanting
                if (Bot.Character.Skills.ContainsKey("Enchanting"))
                {
                    // Check for any Disenchantable items
                    if (Bot.Character.Inventory.Items.Any(i => i.ItemQuality >= 2 && (i.Type == "Armor" || i.Type == "Weapon")))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool TryUseBandage()
        {
            // "Recently Bandaged" debuff ID = 11196
            if (Bot.Player.HasBuffById(11196))
            {
                return false;
            }

            // Find best bandage in inventory (Consumable, Bandage)
            // Since we don't have easy class checks, we loop items and check names/ids?
            // Or just check commonly known IDs? 
            // User wanted dynamic.
            // We can scan inventory for items with "Bandage" in name.

            List<IWowInventoryItem> bandages = Bot.Character.Inventory.Items
                .Where(i => i.Name.Contains("Bandage", StringComparison.OrdinalIgnoreCase) || i.Name.Contains("Verband", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(i => i.ItemLevel) // Best first
                .ToList();

            foreach (IWowInventoryItem b in bandages)
            {
                Bot.Wow.UseItemByName(b.Name);
                return true;
            }
            return false;
        }

        private bool TryDisenchanting()
        {
            // Spell ID 13262 = Disenchant
            if (!Bot.Character.Skills.ContainsKey("Enchanting"))
            {
                return false;
            }

            if (Bot.Character.Inventory.FreeBagSlots < 1)
            {
                return false;
            }

            // Check if we have items to disenchant
            // Policy: Green (2) to Purple (4), Armor/Weapon, Lower level than player
            IWowInventoryItem trashItem = Bot.Character.Inventory.Items.FirstOrDefault(i =>
                i.ItemQuality >= 2 && i.ItemQuality <= 4
                && (i.Type == "Armor" || i.Type == "Weapon")
                && i.ItemLevel < Bot.Player.Level - 5
                && i.RequiredLevel < Bot.Player.Level
                && i.BagId >= 0
            );

            if (trashItem != null)
            {
                if (!Bot.Player.IsCasting)
                {
                    Bot.Wow.CastSpellById(13262);
                    Bot.Wow.LuaDoString($"UseContainerItem({trashItem.BagId}, {trashItem.BagSlot})");
                    _lastProfessionAction = DateTime.Now;
                    return true;
                }
            }

            return false;
        }
    }
}
