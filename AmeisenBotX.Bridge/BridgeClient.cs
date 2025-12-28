using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        if (!_transport.SendRequest(BridgeOpCode.Ping, [], response, out _, 1000))
            return BridgeResult<bool>.Failure(BridgeError.Timeout);

        return BridgeResult<bool>.Success(BridgeMarshaller.ReadBool(response));
    }

    [BridgeMethod(BridgeOpCode.GetPlayerGuid)]
    public BridgeResult<ulong> GetPlayerGuid()
    {
        Span<byte> response = stackalloc byte[sizeof(ulong)];
        if (!_transport.SendRequest(BridgeOpCode.GetPlayerGuid, [], response, out _))
            return BridgeResult<ulong>.Failure(BridgeError.Timeout);

        return BridgeResult<ulong>.Success(BridgeMarshaller.Read<ulong>(response));
    }

    [BridgeMethod(BridgeOpCode.GetUnitName)]
    public BridgeResult<string> GetUnitName(ulong unitGuid)
    {
        Span<byte> request = stackalloc byte[sizeof(ulong)];
        BridgeMarshaller.Write(request, unitGuid);

        // Allocating 512 bytes on stack for name response is safe and fast
        Span<byte> response = stackalloc byte[512];
        if (!_transport.SendRequest(BridgeOpCode.GetUnitName, request, response, out var recvLen))
            return BridgeResult<string>.Failure(BridgeError.Timeout);

        return BridgeResult<string>.Success(BridgeMarshaller.ReadString(response[..recvLen]));
    }

    [BridgeMethod(BridgeOpCode.CastSpellByName)]
    public BridgeResult<bool> CastSpellByName(string spellName, ulong targetGuid)
    {
        // Calculate size needed
        var spellSize = BridgeMarshaller.GetStringSize(spellName);
        var totalSize = spellSize + sizeof(ulong);

        // Avoid ArrayPool if size is reasonably small
        byte[]? rented = null;
        Span<byte> request = totalSize <= 1024 
            ? stackalloc byte[totalSize] 
            : (rented = ArrayPool<byte>.Shared.Rent(totalSize)).AsSpan(0, totalSize);

        try
        {
            var writer = new BufferWriter(request);
            writer.WriteString(spellName);
            writer.Write(targetGuid);

            Span<byte> response = stackalloc byte[1];
            if (!_transport.SendRequest(BridgeOpCode.CastSpellByName, request, response, out _))
                return BridgeResult<bool>.Failure(BridgeError.Timeout);

            return BridgeResult<bool>.Success(BridgeMarshaller.ReadBool(response));
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    [BridgeMethod(BridgeOpCode.ExecuteLua)]
    public BridgeResult<string> ExecuteLua(string luaCode)
    {
        var size = BridgeMarshaller.GetStringSize(luaCode);
        
        byte[]? rented = null;
        Span<byte> request = size <= 2048
            ? stackalloc byte[size]
            : (rented = ArrayPool<byte>.Shared.Rent(size)).AsSpan(0, size);

        try
        {
            BridgeMarshaller.WriteString(request, luaCode);

            // Response can be large, use ArrayPool
            var responseBuffer = ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                if (!_transport.SendRequest(BridgeOpCode.ExecuteLua, request, responseBuffer, out var recvLen))
                    return BridgeResult<string>.Failure(BridgeError.Timeout);

                return BridgeResult<string>.Success(BridgeMarshaller.ReadString(responseBuffer.AsSpan(0, recvLen)));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(responseBuffer);
            }
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _transport.Dispose();
        _ring.Dispose();
    }
}
