using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Logic.Routines
{
    public static class TryCollectGobjectRoutine
    {
        public static bool Run(AmeisenBotInterfaces bot, IWowGameobject gobject)
        {
            if (bot == null || gobject == null)
            {
                return false;
            }

            if (!BotMath.IsFacing(bot.Objects.Player.Position, bot.Objects.Player.Rotation, gobject.Position, 0.5f))
            {
                bot.Wow.FacePosition(bot.Objects.Player.BaseAddress, bot.Player.Position, gobject.Position);
            }

            return !bot.Player.IsCasting;
        }
    }
}
