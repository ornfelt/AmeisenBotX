using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Leafs.Movement
{
    public class DungeonMechanicsLeaf(AmeisenBotInterfaces bot) : INode
    {
        public string Name { get; } = "Movement.Dungeon";

        private AmeisenBotInterfaces Bot { get; } = bot;

        public BtStatus Execute()
        {
            if (Bot.Objects.MapId == WowMapId.TempleOfTheJadeSerpent)
            {
                // Wise Mari Hydrolance Logic
                if (Bot.Objects.All.OfType<IWowUnit>().Any(e => e.CurrentlyCastingSpellId == 106055 || e.CurrentlyChannelingSpellId == 106055))
                {
                    // StayAround Logic
                    (IWowUnit unit, float angle, float distance) = GetMariTarget();

                    if (IWowUnit.IsValid(unit))
                    {
                        Vector3 targetPos = BotMath.CalculatePositionAround(unit.Position, unit.Rotation, angle, distance);
                        Bot.Movement.SetMovementAction(MovementAction.Move, targetPos);
                        return BtStatus.Ongoing;
                    }
                }
            }

            return BtStatus.Failed;
        }

        private (IWowUnit, float, float) GetMariTarget()
        {
            return (Bot.Target, MathF.PI * 0.75f, Bot.CombatClass == null || Bot.CombatClass.IsMelee ? Bot.Player.MeleeRangeTo(Bot.Target) : 7.5f);
        }

        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}
