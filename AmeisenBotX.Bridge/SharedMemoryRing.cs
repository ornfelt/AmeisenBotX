using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Lock-free single-producer single-consumer ring buffer for high-performance IPC.
/// Memory layout: [Header][BotRing][ImplantRing]
/// Uses NTDLL APIs for improved stealth and modern unsafe C# for performance.
/// </summary>
public sealed unsafe class SharedMemoryRing : IDisposable
{
    private readonly nint _sectionHandle;
    private readonly byte* _basePtr;
    private readonly BridgeHeader* _header;
    private readonly byte* _botRing;
    private readonly byte* _implantRing;
    private readonly bool _isBotSide;
    private bool _disposed;

    /// <summary>
    /// Creates or opens a shared memory ring buffer.
    /// </summary>
    /// <summary>
    /// Creates or opens a shared memory ring buffer.
    /// Uses OBJ_OPENIF to allow either side to create the section.
    /// </summary>
    public SharedMemoryRing(string name, bool isBotSide)
    {
        _isBotSide = isBotSide;
        
        var ntPath = NtApi.ToNtPath(name);
        NtApi.RtlInitUnicodeString(out var objectName, ntPath);
        
        // Setup ObjectAttributes with OBJ_OPENIF
        // Note: We bypass NtApi.OBJECT_ATTRIBUTES.Create to set specific flags
        var objAttr = new NtApi.OBJECT_ATTRIBUTES
        {
            Length = sizeof(NtApi.OBJECT_ATTRIBUTES),
            ObjectName = &objectName,
            Attributes = NtApi.OBJ_CASE_INSENSITIVE | NtApi.OBJ_OPENIF
        };

        NtApi.LARGE_INTEGER maxSize = BridgeProtocol.TotalSize;

        var status = NtApi.NtCreateSection(
            out _sectionHandle,
            NtApi.SECTION_ALL_ACCESS,
            &objAttr,
            &maxSize,
            NtApi.PAGE_READWRITE,
            NtApi.SEC_COMMIT,
            nint.Zero);

        if (!NtApi.NT_SUCCESS(status))
            ThrowHelper.ThrowNtStatus("create/open section", status);

        // Check if we created it (Success) or opened existing (Exists)
        var createdNew = status != NtApi.STATUS_OBJECT_NAME_EXISTS;

        status = NtApi.MapViewOfSection(_sectionHandle, (nuint)BridgeProtocol.TotalSize, out var baseAddr);
        if (!NtApi.NT_SUCCESS(status))
        {
            NtApi.Close(_sectionHandle);
            ThrowHelper.ThrowNtStatus("map view", status);
        }
        
        _basePtr = (byte*)baseAddr;
        _header = (BridgeHeader*)_basePtr;

        if (createdNew)
        {
            // We created it -> Initialize
            Unsafe.InitBlockUnaligned(_basePtr, 0, (uint)BridgeProtocol.HeaderSize);
            _header->Magic = BridgeProtocol.MagicNumber;
            _header->Version = BridgeProtocol.ProtocolVersion;
            _header->Status = BridgeStatus.Ready;
        }
        else
        {
            // We opened it -> Validate
            if (_header->Magic != BridgeProtocol.MagicNumber)
                throw new InvalidOperationException("Invalid magic number in shared memory");

            if (_header->Version != BridgeProtocol.ProtocolVersion)
                throw new InvalidOperationException($"Version mismatch: expected {BridgeProtocol.ProtocolVersion}, got {_header->Version}");
        }

        // Update connection status
        if (_isBotSide)
            _header->Status |= BridgeStatus.BotConnected;
        else
            _header->Status |= BridgeStatus.ImplantConnected;

        _botRing = _basePtr + BridgeProtocol.HeaderSize;
        _implantRing = _botRing + BridgeProtocol.RingBufferSize;
    }

    /// <summary>Gets the current bridge status.</summary>
    public BridgeStatus Status => _header->Status;

    /// <summary>Sets an error status.</summary>
    public void SetError() => _header->Status |= BridgeStatus.Error;

    /// <summary>
    /// Writes a message to the ring buffer (non-blocking).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWrite(ReadOnlySpan<byte> data)
    {
        if (data.Length > BridgeProtocol.MaxPayloadSize + sizeof(MessageHeader))
            return false;

        var ring = _isBotSide ? _botRing : _implantRing;
        ref var writeSeq = ref (_isBotSide ? ref _header->BotSeq : ref _header->ImplantSeq);
        ref var readSeq = ref (_isBotSide ? ref _header->ImplantSeq : ref _header->BotSeq);

        var available = (uint)BridgeProtocol.RingBufferSize - (writeSeq - readSeq);
        if (available < data.Length + sizeof(uint))
            return false;

        // Write length prefix
        var writePos = writeSeq % (uint)BridgeProtocol.RingBufferSize;
        WriteWrapping(ring, writePos, (uint)data.Length);
        writePos = (writePos + sizeof(uint)) % (uint)BridgeProtocol.RingBufferSize;

        // Write data using Unsafe.CopyBlock for better codegen
        fixed (byte* pData = data)
        {
            WriteWrapping(ring, writePos, pData, data.Length);
        }

        // Update sequence with release semantics
        Interlocked.Add(ref writeSeq, (uint)(sizeof(uint) + data.Length));
        return true;
    }

    /// <summary>
    /// Attempts to read a message from the ring buffer (non-blocking).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryRead(Span<byte> buffer, out int bytesRead)
    {
        bytesRead = 0;

        var ring = _isBotSide ? _implantRing : _botRing;
        ref var readSeq = ref (_isBotSide ? ref _header->ImplantSeq : ref _header->BotSeq);
        ref var writeSeq = ref (_isBotSide ? ref _header->BotSeq : ref _header->ImplantSeq);

        var available = writeSeq - readSeq;
        if (available < sizeof(uint))
            return false;

        // Read length prefix
        var readPos = readSeq % (uint)BridgeProtocol.RingBufferSize;
        var length = ReadWrapping(ring, readPos);
        readPos = (readPos + sizeof(uint)) % (uint)BridgeProtocol.RingBufferSize;

        if (available < sizeof(uint) + length || length > (uint)buffer.Length)
            return false;

        // Read data
        fixed (byte* pBuffer = buffer)
        {
            ReadWrapping(ring, readPos, pBuffer, (int)length);
        }

        bytesRead = (int)length;

        // Update sequence with release semantics
        Interlocked.Add(ref readSeq, sizeof(uint) + length);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteWrapping(byte* ring, uint pos, uint value)
    {
        if (pos + sizeof(uint) <= BridgeProtocol.RingBufferSize)
        {
            Unsafe.WriteUnaligned(ring + pos, value);
        }
        else
        {
            // Wrap around - copy byte by byte
            var src = (byte*)&value;
            var remaining = (uint)BridgeProtocol.RingBufferSize - pos;
            Unsafe.CopyBlockUnaligned(ring + pos, src, remaining);
            Unsafe.CopyBlockUnaligned(ring, src + remaining, sizeof(uint) - remaining);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteWrapping(byte* ring, uint pos, byte* data, int length)
    {
        var len = (uint)length;
        if (pos + len <= BridgeProtocol.RingBufferSize)
        {
            Unsafe.CopyBlockUnaligned(ring + pos, data, len);
        }
        else
        {
            // Wrap around
            var firstPart = (uint)BridgeProtocol.RingBufferSize - pos;
            Unsafe.CopyBlockUnaligned(ring + pos, data, firstPart);
            Unsafe.CopyBlockUnaligned(ring, data + firstPart, len - firstPart);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ReadWrapping(byte* ring, uint pos)
    {
        if (pos + sizeof(uint) <= BridgeProtocol.RingBufferSize)
        {
            return Unsafe.ReadUnaligned<uint>(ring + pos);
        }
        
        // Wrap around - read byte by byte
        uint value = 0;
        var dst = (byte*)&value;
        var remaining = (uint)BridgeProtocol.RingBufferSize - pos;
        Unsafe.CopyBlockUnaligned(dst, ring + pos, remaining);
        Unsafe.CopyBlockUnaligned(dst + remaining, ring, sizeof(uint) - remaining);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReadWrapping(byte* ring, uint pos, byte* dest, int length)
    {
        var len = (uint)length;
        if (pos + len <= BridgeProtocol.RingBufferSize)
        {
            Unsafe.CopyBlockUnaligned(dest, ring + pos, len);
        }
        else
        {
            // Wrap around
            var firstPart = (uint)BridgeProtocol.RingBufferSize - pos;
            Unsafe.CopyBlockUnaligned(dest, ring + pos, firstPart);
            Unsafe.CopyBlockUnaligned(dest + firstPart, ring, len - firstPart);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_basePtr is not null)
            NtApi.UnmapViewOfSection((nint)_basePtr);

        if (_sectionHandle != nint.Zero)
            NtApi.Close(_sectionHandle);
    }
}

/// <summary>
/// Helper class for throwing exceptions (aids JIT inlining of hot paths).
/// </summary>
file static class ThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNtStatus(string operation, int status)
        => throw new InvalidOperationException($"Failed to {operation}: NTSTATUS 0x{status:X8}");
}
