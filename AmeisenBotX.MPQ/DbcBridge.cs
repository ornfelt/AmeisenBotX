using AmeisenBotX.MPQ.Dbc;
using AmeisenBotX.MPQ.Dbc.Records;

namespace AmeisenBotX.MPQ
{
    public class DbcBridge(MpqBridge mpqBridge) : IDisposable
    {
        private readonly MpqBridge _mpqBridge = mpqBridge;

        private DbcReader<SpellRecord> _spellDbc;
        private DbcReader<SpellIconRecord> _iconDbc;
        private DbcReader<ItemRecord> _itemDbc;
        private DbcReader<ItemDisplayInfoRecord> _itemDisplayInfoDbc;
        private DbcReader<WorldMapAreaRecord> _worldMapAreaDbc;

        private DbcReader<SpellRecord> SpellDbc => GetOrLoad(ref _spellDbc, "DBFilesClient\\Spell.dbc");

        private DbcReader<SpellIconRecord> IconDbc => GetOrLoad(ref _iconDbc, "DBFilesClient\\SpellIcon.dbc");

        private DbcReader<ItemRecord> ItemDbc => GetOrLoad(ref _itemDbc, "DBFilesClient\\Item.dbc");
        
        private DbcReader<ItemDisplayInfoRecord> ItemDisplayInfoDbc => GetOrLoad(ref _itemDisplayInfoDbc, "DBFilesClient\\ItemDisplayInfo.dbc");

        private DbcReader<WorldMapAreaRecord> WorldMapAreaDbc => GetOrLoad(ref _worldMapAreaDbc, "DBFilesClient\\WorldMapArea.dbc");


        private DbcReader<T> GetOrLoad<T>(ref DbcReader<T> field, string path) where T : unmanaged
        {
            if (field != null)
            {
                return field;
            }

            byte[] data = _mpqBridge.ReadFileBytes(path) ?? throw new FileNotFoundException($"DBC file not found in MPQ: {path}");
            field = new DbcReader<T>(data);
            return field;
        }

        public string GetSpellName(int spellId)
        {
            return SpellDbc.TryGetRecord(spellId, out SpellRecord record) ? SpellDbc.GetString(record.NameOffset) : $"Unknown Spell {spellId}";
        }

        public string GetSpellIconPath(int spellId)
        {
            if (!SpellDbc.TryGetRecord(spellId, out SpellRecord spellRec))
            {
                return null;
            }

            if (!IconDbc.TryGetRecord((int)spellRec.SpellIconID, out SpellIconRecord iconRec))
            {
                return null;
            }

            string path = IconDbc.GetString(iconRec.PathOffset);

            return string.IsNullOrEmpty(path) ? null : !path.Contains('\\') ? "Interface\\Icons\\" + path : path;
        }

        public string GetItemIconPath(int itemId)
        {
            if (!ItemDbc.TryGetRecord(itemId, out ItemRecord itemRec))
            {
                return null;
            }

            if (!ItemDisplayInfoDbc.TryGetRecord((int)itemRec.DisplayInfoID, out ItemDisplayInfoRecord displayRec))
            {
                return null;
            }

            string iconName = ItemDisplayInfoDbc.GetString(displayRec.InventoryIconOffset);

            return string.IsNullOrEmpty(iconName) ? null : "Interface\\Icons\\" + iconName;
        }

        public IEnumerable<(int Id, string Name)> GetAllSpells()
        {
            DbcReader<SpellRecord> dbc = SpellDbc;
            for (int i = 0; i < dbc.RecordCount; i++)
            {
                SpellRecord record = dbc.GetRecordAtRow(i);
                string name = dbc.GetString(record.NameOffset);
                yield return ((int)record.Id, name);
            }
        }

        public int GetSpellIdByName(string searchName)
        {
            DbcReader<SpellRecord> dbc = SpellDbc;
            for (int i = 0; i < dbc.RecordCount; i++)
            {
                SpellRecord record = dbc.GetRecordAtRow(i);
                string name = dbc.GetString(record.NameOffset);

                if (name.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                {
                    return (int)record.Id;
                }
            }
            return -1;
        }

        public bool TryGetWorldMapArea(int mapId, out WorldMapAreaRecord result)
        {
            // Scan for the record matching the MapID. 
            // Note: WorldMapArea.dbc can have multiple entries per MapID (subzones).
            // For QuestPOI, we usually operate on the "base" map or zone map.
            // We'll iterate and find the first match where the AreaID matches the current zone logic, 
            // or just the first match for that MapID if we assume 1-to-1 for main continents (which isn't true).
            // Ideally we pass AreaID too.
            
            DbcReader<WorldMapAreaRecord> dbc = WorldMapAreaDbc;
            for (int i = 0; i < dbc.RecordCount; i++)
            {
                WorldMapAreaRecord record = dbc.GetRecordAtRow(i);
                if (record.MapId == mapId && record.AreaId != 0) // Basic filter
                {
                   result = record;
                   return true;
                }
            }
            result = default;
            return false;
        }

        public bool TryGetWorldMapAreaByAreaID(int areaId, out WorldMapAreaRecord result)
        {
            DbcReader<WorldMapAreaRecord> dbc = WorldMapAreaDbc;
            for (int i = 0; i < dbc.RecordCount; i++)
            {
                WorldMapAreaRecord record = dbc.GetRecordAtRow(i);
                // In some DBs AreaID is 0 for root maps, check carefully
                if (record.AreaId == areaId)
                {
                    result = record;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public void Dispose()
        {
            _spellDbc?.Dispose();
            _iconDbc?.Dispose();
            _itemDbc?.Dispose();
            _itemDbc?.Dispose();
            _itemDisplayInfoDbc?.Dispose();
        }
    }
}
