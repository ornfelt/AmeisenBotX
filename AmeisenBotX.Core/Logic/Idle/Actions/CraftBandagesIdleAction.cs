using AmeisenBotX.Common.Utils;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    public class CraftBandagesIdleAction(AmeisenBotInterfaces bot, AmeisenBotConfig config) : IIdleAction
    {
        public bool AutopilotOnly => false;

        public DateTime Cooldown { get; set; }

        // Cooldown between crafting sessions
        public int MaxCooldown => 60 * 1000;
        public int MinCooldown => 30 * 1000;

        // How long to spend crafting
        public int MaxDuration => 45 * 1000;
        public int MinDuration => 15 * 1000;

        private AmeisenBotInterfaces Bot { get; } = bot;
        private AmeisenBotConfig Config { get; } = config;

        private string RecipeToCraft { get; set; }
        private TimegatedEvent CraftThrottle { get; } = new(TimeSpan.FromSeconds(2.5)); // Cast time is usually 2s + latency

        // Bandage definitions: Name, ClothName, CountRequired
        // Ordered High to Low
        private readonly (string Recipe, string Cloth, int Count)[] Recipes =
        [
            ("Heavy Frostweave Bandage", "Frostweave Cloth", 2),
            ("Frostweave Bandage", "Frostweave Cloth", 1),
            ("Heavy Netherweave Bandage", "Netherweave Cloth", 2),
            ("Netherweave Bandage", "Netherweave Cloth", 1),
            ("Heavy Runecloth Bandage", "Runecloth Cloth", 2),
            ("Runecloth Bandage", "Runecloth", 1),
            ("Heavy Mageweave Bandage", "Mageweave Cloth", 2),
            ("Mageweave Bandage", "Mageweave Cloth", 1),
            ("Heavy Silk Bandage", "Silk Cloth", 2),
            ("Silk Bandage", "Silk Cloth", 1),
            ("Heavy Wool Bandage", "Wool Cloth", 2),
            ("Wool Bandage", "Wool Cloth", 1),
            ("Heavy Linen Bandage", "Linen Cloth", 2),
            ("Linen Bandage", "Linen Cloth", 1),
        ];

        public bool Enter()
        {
            if (!Config.CraftBandages || Bot.Player.IsInCombat || Bot.Player.IsMounted || Bot.Player.IsDead)
            {
                return false;
            }

            // Find best craftable bandage
            foreach ((string recipe, string cloth, int count) in Recipes)
            {
                if (Bot.Character.SpellBook.IsSpellKnown(recipe))
                {
                    int clothCount = GetItemCount(cloth);
                    if (clothCount >= count)
                    {
                        // Check if we already have plenty of these bandages (e.g. > 20)
                        // If we have > 20, don't craft more unless we are overflowing with cloth (> 40 cloth)
                        int bandageCount = GetItemCount(recipe);
                        if (bandageCount < 20 || clothCount > 60)
                        {
                            RecipeToCraft = recipe;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(RecipeToCraft))
            {
                return;
            }

            if (CraftThrottle.Run())
            {
                // Simple verify we still can content
                if (!Bot.Character.SpellBook.IsSpellKnown(RecipeToCraft))
                {
                    return;
                }

                // Cast the spell (this usually crafts one item)
                Bot.Wow.CastSpell(RecipeToCraft);
            }
        }

        private int GetItemCount(string name)
        {
            return Bot.Character.Inventory.Items
                .Where(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.Count);
        }

        public override string ToString()
        {
            return $"Crafting {RecipeToCraft}";
        }
    }
}
