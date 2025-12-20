using AmeisenBotX.MPQ.Dbc;
using AmeisenBotX.MPQ.Dbc.Records;

namespace AmeisenBotX.MPQ
{
    public class DbcBridge : IDisposable
    {
        private readonly MpqBridge _mpqBridge;

        private DbcReader<SpellRecord> _spellDbc;
        private DbcReader<SpellIconRecord> _iconDbc;
        private DbcReader<ItemRecord> _itemDbc;
        private DbcReader<ItemDisplayInfoRecord> _itemDisplayInfoDbc;

        public DbcBridge(MpqBridge mpqBridge)
        {
            _mpqBridge = mpqBridge;
        }

        private DbcReader<SpellRecord> SpellDbc => GetOrLoad(ref _spellDbc, "DBFilesClient\\Spell.dbc");

        private DbcReader<SpellIconRecord> IconDbc => GetOrLoad(ref _iconDbc, "DBFilesClient\\SpellIcon.dbc");

        private DbcReader<ItemRecord> ItemDbc => GetOrLoad(ref _itemDbc, "DBFilesClient\\Item.dbc");

        private DbcReader<ItemDisplayInfoRecord> ItemDisplayInfoDbc => GetOrLoad(ref _itemDisplayInfoDbc, "DBFilesClient\\ItemDisplayInfo.dbc");


        private DbcReader<T> GetOrLoad<T>(ref DbcReader<T> field, string path) where T : unmanaged
        {
            if (field != null)
            {
                return field;
            }

            byte[] data = _mpqBridge.ReadFileBytes(path);

            if (data == null)
            {
                throw new FileNotFoundException($"DBC file not found in MPQ: {path}");
            }

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

        public void Dispose()
        {
            _spellDbc?.Dispose();
            _iconDbc?.Dispose();
            _itemDbc?.Dispose();
            _itemDisplayInfoDbc?.Dispose();
        }
    }
}
