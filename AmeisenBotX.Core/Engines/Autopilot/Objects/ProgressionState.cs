using AmeisenBotX.Wow.Objects.Enums;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Autopilot.Objects
{
    public enum ProgressionPhase
    {
        Leveling,
        FreshDinged,
        NormalDungeons,
        HeroicFarming,
        EmblemOptimization,
        RaidReady
    }

    public class ProgressionState
    {
        private readonly AmeisenBotInterfaces Bot;

        public ProgressionState(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        public int Level => Bot.Player?.Level ?? 0;
        
        public int GearScore => 0; // Bot.Character.Equipment.CalculateGearScore();
        
        public int AverageItemLevel => 0; // Bot.Character.Equipment.CalculateAverageILvl();

        public ProgressionPhase CurrentPhase => DeterminePhase();

        private ProgressionPhase DeterminePhase()
        {
            if (Level < 80) return ProgressionPhase.Leveling;
            if (AverageItemLevel < 180) return ProgressionPhase.FreshDinged;
            if (AverageItemLevel < 200) return ProgressionPhase.NormalDungeons;
            if (AverageItemLevel < 219) return ProgressionPhase.HeroicFarming;
            if (AverageItemLevel < 232) return ProgressionPhase.EmblemOptimization;
            return ProgressionPhase.RaidReady;
        }
    }
}
