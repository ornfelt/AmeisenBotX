using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Server-side bridge for handling requests from the external bot process.
/// Runs inside the injected implant.
/// </summary>
public sealed unsafe class BridgeServer : IDisposable
{
    private readonly SharedMemoryRing _ring;
    private readonly BridgeTransport _transport;
    private readonly Thread _messageThread;
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;

    /// <summary>
    /// Creates a new bridge server (implant side).
    /// </summary>
    /// <param name="ipcName">IPC name (e.g., "Local\WM_IPC_12345")</param>
    public BridgeServer(string ipcName)
    {
        _ring = new SharedMemoryRing(ipcName, isBotSide: false);
        _transport = new BridgeTransport(_ring);
        _transport.MessageReceived += OnMessageReceived;

        _messageThread = new Thread(MessagePump)
        {
            IsBackground = true,
            Name = "BridgeServer-MessagePump",
            Priority = ThreadPriority.AboveNormal
        };
    }

    /// <summary>
    /// Starts the message processing loop.
    /// </summary>
    public void Start() => _messageThread.Start();

    private void MessagePump()
    {
        BridgeLogger.Log("[BridgeServer] Message pump started");

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                BridgeLogger.Log($"[BridgeServer] Error in message pump: {ex.Message}");
            }
        }

        BridgeLogger.Log("[BridgeServer] Message pump stopped");
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        try
        {
            var opCode = (BridgeOpCode)e.Header.OpCode;
            BridgeLogger.Log($"[BridgeServer] Received {opCode} (MsgID: {e.Header.MessageId})");

            // Rent a buffer for the response to avoid allocations in handlers
            // Max payload size is safe upper bound
            var responseBuffer = ArrayPool<byte>.Shared.Rent(BridgeProtocol.MaxPayloadSize);
            try
            {
                var responseSpan = responseBuffer.AsSpan();
                var success = false;
                var responseSize = 0;

                try
                {
                    success = opCode switch
                    {
                        BridgeOpCode.Ping => HandlePing(e.Payload, responseSpan, out responseSize),
                        BridgeOpCode.GetPlayerGuid => HandleGetPlayerGuid(e.Payload, responseSpan, out responseSize),
                        BridgeOpCode.GetUnitName => HandleGetUnitName(e.Payload, responseSpan, out responseSize),
                        BridgeOpCode.CastSpellByName => HandleCastSpellByName(e.Payload, responseSpan, out responseSize),
                        BridgeOpCode.ExecuteLua => HandleExecuteLua(e.Payload, responseSpan, out responseSize),
                        _ => HandleUnknown(opCode)
                    };

                    if (responseSize > 0)
                    {
                        SendResponse(e.Header.MessageId, success, responseSpan[..responseSize]);
                    }
                }
                catch (Exception ex)
                {
                    BridgeLogger.Log($"[BridgeServer] Handler error: {ex.Message}");
                    SendResponse(e.Header.MessageId, false, []);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(responseBuffer);
            }
        }
        catch (Exception ex)
        {
            BridgeLogger.Log($"[BridgeServer] Critical error handling message: {ex.Message}");
        }
    }

    private bool HandleUnknown(BridgeOpCode opCode)
    {
        BridgeLogger.Log($"[BridgeServer] Unknown opcode: {opCode}");
        return false;
    }

    private void SendResponse(uint messageId, bool success, ReadOnlySpan<byte> payload)
    {
        var header = MessageHeader.CreateResponse(messageId, payload.Length, success);
        var totalSize = sizeof(MessageHeader) + payload.Length;
        
        // Stackalloc for small responses
        byte[]? rented = null;
        Span<byte> buffer = totalSize <= 2048 
            ? stackalloc byte[totalSize] 
            : (rented = ArrayPool<byte>.Shared.Rent(totalSize)).AsSpan(0, totalSize);
            
        try
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), header);
            payload.CopyTo(buffer[sizeof(MessageHeader)..]);
            _ring.TryWrite(buffer);
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    // ==========================================
    // Method Handlers (server-side implementations)
    // ==========================================

    private static bool HandlePing(ReadOnlySpan<byte> request, Span<byte> response, out int responseSize)
    {
        responseSize = BridgeMarshaller.WriteBool(response, true);
        return true;
    }

    private static bool HandleGetPlayerGuid(ReadOnlySpan<byte> request, Span<byte> response, out int responseSize)
    {
        // TODO: Get actual player GUID from WoW memory
        const ulong playerGuid = 0x0000000012345678; // Placeholder
        responseSize = BridgeMarshaller.Write(response, playerGuid);
        return true;
    }

    private static bool HandleGetUnitName(ReadOnlySpan<byte> request, Span<byte> response, out int responseSize)
    {
        var guid = BridgeMarshaller.Read<ulong>(request);

        // TODO: Get actual unit name from WoW
        var name = $"Unit_{guid:X}"; // Placeholder
        responseSize = BridgeMarshaller.WriteString(response, name);
        return true;
    }

    private static bool HandleCastSpellByName(ReadOnlySpan<byte> request, Span<byte> response, out int responseSize)
    {
        var buffer = new BufferReader(request);
        var spellName = buffer.ReadString();
        var targetGuid = buffer.Read<ulong>();

        BridgeLogger.Log($"[BridgeServer] CastSpell: {spellName} on {targetGuid:X}");

        // TODO: Actually cast spell in WoW
        const bool success = true; // Placeholder
        responseSize = BridgeMarshaller.WriteBool(response, success);
        return true;
    }

    private static bool HandleExecuteLua(ReadOnlySpan<byte> request, Span<byte> response, out int responseSize)
    {
        var luaCode = BridgeMarshaller.ReadString(request);

        BridgeLogger.Log($"[BridgeServer] ExecuteLua: {luaCode}");

        // TODO: Actually execute Lua in WoW
        const string result = "Lua execution not yet implemented"; // Placeholder
        responseSize = BridgeMarshaller.WriteString(response, result);
        return true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _cts.Cancel();
        _messageThread.Join(1000);

        _transport.Dispose();
        _ring.Dispose();
        _cts.Dispose();
    }
}
