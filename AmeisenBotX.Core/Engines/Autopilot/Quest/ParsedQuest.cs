using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Autopilot.Quest
{
    public class ParsedQuest
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public int LogIndex { get; set; }
        public int Level { get; set; }
        public bool IsComplete { get; set; }
        
        public List<ParsedQuestObjective> Objectives { get; set; } = [];
        public List<ParsedQuestReward> Rewards { get; set; } = [];

        public float PoiX { get; set; }
        public float PoiY { get; set; }
    }

    public class ParsedQuestReward
    {
        public string Name { get; set; }
        public string ItemLink { get; set; }
        public string Texture { get; set; } // URL or file path for icon? Lua returns texture path.
        public int ChoiceIndex { get; set; } // 1-based index for SelectQuestLogEntry

        // Parsed Info
        public int ItemId { get; set; }
        public int ItemLevel { get; set; }
        public int RequiredLevel { get; set; }
        public string Type { get; set; } // Armor, Weapon etc
        public string SubType { get; set; } // Plate, SwordOneHand etc
        public string EquipLoc { get; set; }
        
        // Calculated Score
        public float Score { get; set; }
        public bool IsUsable { get; set; }
    }
}
