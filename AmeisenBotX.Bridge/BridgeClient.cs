using System.Buffers;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Client-side bridge for calling methods in the injected implant.
/// Uses modern C# features and high-performance marshalling.
/// </summary>
public sealed class BridgeClient : IDisposable
{
    private readonly SharedMemoryRing _ring;
    private readonly BridgeTransport _transport;
    private bool _disposed;

    public BridgeClient(string ipcName)
    {
        _ring = new SharedMemoryRing(ipcName, isBotSide: true);
        _transport = new BridgeTransport(_ring);
    }

    /// <summary>Gets whether the implant is connected.</summary>
    public bool IsConnected => (_ring.Status & BridgeStatus.ImplantConnected) != 0;

    // ==========================================
    // Bridge Methods (Manually implemented)
    // ==========================================

    [BridgeMethod(BridgeOpCode.Ping, TimeoutMs = 1000)]
    public BridgeResult<bool> Ping()
    {
        Span<byte> response = stackalloc byte[1];
        return !_transport.SendRequest(BridgeOpCode.Ping, [], response, out _, 1000)
            ? BridgeResult<bool>.Failure(BridgeError.Timeout)
            : BridgeResult<bool>.Success(BridgeMarshaller.ReadBool(response));
    }

    [BridgeMethod(BridgeOpCode.GetPlayerGuid)]
    public BridgeResult<ulong> GetPlayerGuid()
    {
        Span<byte> response = stackalloc byte[sizeof(ulong)];
        return !_transport.SendRequest(BridgeOpCode.GetPlayerGuid, [], response, out _)
            ? BridgeResult<ulong>.Failure(BridgeError.Timeout)
            : BridgeResult<ulong>.Success(BridgeMarshaller.Read<ulong>(response));
    }

    [BridgeMethod(BridgeOpCode.GetUnitName)]
    public BridgeResult<string> GetUnitName(ulong unitGuid)
    {
        Span<byte> request = stackalloc byte[sizeof(ulong)];
        BridgeMarshaller.Write(request, unitGuid);

        // Allocating 512 bytes on stack for name response is safe and fast
        Span<byte> response = stackalloc byte[512];
        return !_transport.SendRequest(BridgeOpCode.GetUnitName, request, response, out int recvLen)
            ? BridgeResult<string>.Failure(BridgeError.Timeout)
            : BridgeResult<string>.Success(BridgeMarshaller.ReadString(response[..recvLen]));
    }

    [BridgeMethod(BridgeOpCode.CastSpellByName)]
    public BridgeResult<bool> CastSpellByName(string spellName, ulong targetGuid)
    {
        // Calculate size needed
        int spellSize = BridgeMarshaller.GetStringSize(spellName);
        int totalSize = spellSize + sizeof(ulong);

        // Avoid ArrayPool if size is reasonably small
        byte[]? rented = null;
        Span<byte> request = totalSize <= 1024
            ? stackalloc byte[totalSize]
            : (rented = ArrayPool<byte>.Shared.Rent(totalSize)).AsSpan(0, totalSize);

        try
        {
            BufferWriter writer = new(request);
            writer.WriteString(spellName);
            writer.Write(targetGuid);

            Span<byte> response = stackalloc byte[1];
            return !_transport.SendRequest(BridgeOpCode.CastSpellByName, request, response, out _)
                ? BridgeResult<bool>.Failure(BridgeError.Timeout)
                : BridgeResult<bool>.Success(BridgeMarshaller.ReadBool(response));
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    [BridgeMethod(BridgeOpCode.ExecuteLua)]
    public BridgeResult<string> ExecuteLua(string luaCode)
    {
        int size = BridgeMarshaller.GetStringSize(luaCode);

        byte[]? rented = null;
        Span<byte> request = size <= 2048
            ? stackalloc byte[size]
            : (rented = ArrayPool<byte>.Shared.Rent(size)).AsSpan(0, size);

        try
        {
            BridgeMarshaller.WriteString(request, luaCode);

            // Response can be large, use ArrayPool
            byte[] responseBuffer = ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                return !_transport.SendRequest(BridgeOpCode.ExecuteLua, request, responseBuffer, out int recvLen)
                    ? BridgeResult<string>.Failure(BridgeError.Timeout)
                    : BridgeResult<string>.Success(BridgeMarshaller.ReadString(responseBuffer.AsSpan(0, recvLen)));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(responseBuffer);
            }
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _transport.Dispose();
        _ring.Dispose();
    }
}
