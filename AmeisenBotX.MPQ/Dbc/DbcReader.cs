using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc
{
    public unsafe class DbcReader<T> : IDisposable where T : unmanaged
    {
        public int RecordCount { get; }

        public int RecordSize { get; }

        public int StringBlockSize { get; }

        private GCHandle _handle;
        private readonly byte* _headerPtr;
        private readonly byte* _recordsStartPtr;
        private readonly byte* _stringBlockPtr;
        private readonly Dictionary<int, int> _idToRowIndex;

        public DbcReader(byte[] data)
        {
            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            _headerPtr = (byte*)_handle.AddrOfPinnedObject();

            RecordCount = *(int*)(_headerPtr + 4);
            RecordSize = *(int*)(_headerPtr + 12);
            StringBlockSize = *(int*)(_headerPtr + 16);

            _recordsStartPtr = _headerPtr + 20;
            _stringBlockPtr = _recordsStartPtr + (RecordCount * RecordSize);
            _idToRowIndex = new Dictionary<int, int>(RecordCount);

            for (int i = 0; i < RecordCount; i++)
            {
                byte* rowPtr = _recordsStartPtr + (i * RecordSize);
                int id = *(int*)rowPtr;
                _idToRowIndex[id] = i;
            }
        }

        public bool TryGetRecord(int id, out T record)
        {
            if (_idToRowIndex.TryGetValue(id, out int rowIndex))
            {
                byte* rowPtr = _recordsStartPtr + (rowIndex * RecordSize);
                record = *(T*)rowPtr;
                return true;
            }

            record = default;
            return false;
        }

        public T GetRecordAtRow(int index)
        {
            if (index < 0 || index >= RecordCount)
            {
                throw new IndexOutOfRangeException();
            }

            byte* rowPtr = _recordsStartPtr + (index * RecordSize);
            return *(T*)rowPtr;
        }

        public string GetString(uint offset)
        {
            return offset == 0 || offset >= StringBlockSize ? string.Empty : Marshal.PtrToStringUTF8((IntPtr)(_stringBlockPtr + offset));
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }
    }
}
