using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Logic.Routines
{
    public static class SpeakToClassTrainerRoutine
    {
        public static bool Run(AmeisenBotInterfaces bot, IWowUnit selectedUnit)
        {
            if (bot == null || selectedUnit == null)
            {
                return false;
            }

            if (bot.Wow.TargetGuid != selectedUnit.Guid)
            {
                bot.Wow.ChangeTarget(selectedUnit.Guid);
            }

            if (bot.Target == null || bot.Player.DistanceTo(bot.Target) > 6.0f)
            {
                return false;
            }

            if (!BotMath.IsFacing(bot.Objects.Player.Position, bot.Objects.Player.Rotation, selectedUnit.Position, 0.5f))
            {
                bot.Wow.FacePosition(bot.Objects.Player.BaseAddress, bot.Player.Position, selectedUnit.Position);
            }

            if (!bot.Wow.UiIsVisible("GossipFrame", "ClassTrainerFrame"))
            {
                bot.Wow.InteractWithUnit(selectedUnit);
            }

            if (selectedUnit.IsGossip && bot.Wow.UiIsVisible("GossipFrame"))
            {
                string[] gossipTypes = bot.Wow.GetGossipTypes();

                for (int i = 0; i < gossipTypes.Length; ++i)
                {
                    if (!gossipTypes[i].Equals("trainer", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // +1 is due to implicit conversion between lua array (indexed at 1 not 0) and c# array
                    bot.Wow.SelectGossipOptionSimple(i + 1);
                    break;
                }
            }

            if (bot.Wow.UiIsVisible("ClassTrainerFrame"))
            {
                TrainAllSpellsRoutine.Run(bot);
                return true;
            }

            return false;
        }
    }
}
