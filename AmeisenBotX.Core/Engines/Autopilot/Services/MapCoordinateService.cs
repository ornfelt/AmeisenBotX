using AmeisenBotX.Common.Math;
using AmeisenBotX.MPQ;
using AmeisenBotX.MPQ.Dbc.Records;

namespace AmeisenBotX.Core.Engines.Autopilot.Services
{
    public class MapCoordinateService
    {
        private readonly DbcBridge Dbc;

        public MapCoordinateService(DbcBridge dbc)
        {
            Dbc = dbc;
        }

        public Vector3? GetWorldPos(int mapId, int areaId, float mapX, float mapY)
        {
            // 1. Try to get Area-specific record first (most accurate for subzones)
            if (!Dbc.TryGetWorldMapAreaByAreaID(areaId, out WorldMapAreaRecord record))
            {
                // Fallback to MapID generic
                if (!Dbc.TryGetWorldMapArea(mapId, out record))
                {
                    return null;
                }
            }

            // 2. Perform Conversion
            // WoW Map Coords: (0,0) is Top-Left, (1,1) is Bottom-Right relative to the zone rect.
            // LocLeft = Y_Max, LocRight = Y_Min
            // LocTop = X_Max, LocBottom = X_Min
            // Formula:
            // WorldX = MaxX - (mapY * (MaxX - MinX))  <-- Note: mapY is often the vertical component on 2D map, which maps to X in World
            // WorldY = MaxY - (mapX * (MaxY - MinY))  <-- And mapX is horizontal, mapping to Y
            
            // Wait, standard WoW UI:
            // X is horizontal (East-West) -> MapY? No.
            // In Lua: GetPlayerMapPosition -> returns x, y. 
            // x (0..1) is horizontal axis (Left to Right) -> Y Axis in World?
            // y (0..1) is vertical axis (Top to Bottom) -> X Axis in World?
            
            // World X is North-South. North is +X. South is -X.
            // World Y is East-West. West is +Y. East is -Y.
            
            // Map Top-Left (0,0) corresponds to (MaxX, MaxY) in World.
            // Map Bottom-Right (1,1) corresponds to (MinX, MinY) in World.
            
            // Map X (0->1) moves Left->Right (West->East). West is MaxY, East is MinY.
            // So MapX * Width = (MaxY - currentY). 
            // currentY = MaxY - (MapX * (MaxY - MinY))
            
            // Map Y (0->1) moves Top->Bottom (North->South). North is MaxX, South is MinX.
            // So MapY * Height = (MaxX - currentX).
            // currentX = MaxX - (MapY * (MaxX - MinX))
            
            float worldX = record.LocTop - (mapY * (record.LocTop - record.LocBottom)); // Vertical on map -> X axis
            float worldY = record.LocLeft - (mapX * (record.LocLeft - record.LocRight)); // Horizontal on map -> Y axis
            
            // Z is unknown from 2D map. Return 0 or high value to raycast down.
            // We use 0 as placeholder.
            return new Vector3(worldX, worldY, 0);
        }
    }
}
