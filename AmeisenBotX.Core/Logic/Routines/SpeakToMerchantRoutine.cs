using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;

/// <summary>
/// Utility class for performing routine actions related to interacting with a merchant in the game.
/// </summary>
namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Utility class for performing routine actions related to interacting with a merchant in the game.
    /// </summary>
    public static class SpeakToMerchantRoutine
    {
        /// <summary>
        /// Checks if the provided AmeisenBotInterfaces bot and IWowUnit selectedUnit are valid.
        /// Changes the target of the bot to the selectedUnit if it is different from the bot's current target.
        /// Faces the selectedUnit if the bot is not already facing it.
        /// Interacts with the selectedUnit if the GossipFrame and MerchantFrame UIs are not visible.
        /// Selects the appropriate gossip option (vendor or repair) if the GossipFrame UI is visible.
        /// Returns true if all conditions are met, otherwise returns false.
        /// </summary>
        public static bool Run(AmeisenBotInterfaces bot, IWowUnit selectedUnit)
        {
            if (bot == null || selectedUnit == null)
            {
                return false;
            }

            if (bot.Wow.TargetGuid != selectedUnit.Guid)
            {
                bot.Wow.ChangeTarget(selectedUnit.Guid);
                return false;
            }

            if (!BotMath.IsFacing(bot.Objects.Player.Position, bot.Objects.Player.Rotation, selectedUnit.Position, 0.5f))
            {
                bot.Wow.FacePosition(bot.Objects.Player.BaseAddress, bot.Player.Position, selectedUnit.Position);
            }

            if (!bot.Wow.UiIsVisible("GossipFrame", "MerchantFrame"))
            {
                bot.Wow.InteractWithUnit(selectedUnit);
                return false;
            }

            if (selectedUnit.IsGossip)
            {
                if (bot.Wow.UiIsVisible("GossipFrame"))
                {
                    string[] gossipTypes = bot.Wow.GetGossipTypes();

                    for (int i = 0; i < gossipTypes.Length; ++i)
                    {
                        if (gossipTypes[i].Equals("vendor", StringComparison.OrdinalIgnoreCase)
                            || gossipTypes[i].Equals("repair", StringComparison.OrdinalIgnoreCase))
                        {
                            bot.Wow.SelectGossipOption(i + 1);
                        }
                    }
                }

                if (!bot.Wow.UiIsVisible("MerchantFrame"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}