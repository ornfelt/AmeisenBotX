using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Message transport layer handles request/response correlation and timeouts.
/// </summary>
public sealed unsafe class BridgeTransport : IDisposable
{
    private readonly SharedMemoryRing _ring;
    private readonly ConcurrentDictionary<uint, PendingRequest> _pendingRequests = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Thread _receiveThread;
    private uint _nextMessageId;
    private bool _disposed;

    public BridgeTransport(SharedMemoryRing ring)
    {
        _ring = ring;
        _receiveThread = new Thread(ReceiveLoop)
        {
            IsBackground = true,
            Name = "BridgeTransport-Receive",
            Priority = ThreadPriority.AboveNormal
        };
        _receiveThread.Start();
    }

    /// <summary>
    /// Sends a request and waits for a response.
    /// </summary>
    public bool SendRequest(BridgeOpCode opCode, ReadOnlySpan<byte> payload, Span<byte> response, out int responseSize, int timeoutMs = 5000)
    {
        responseSize = 0;

        if (payload.Length > BridgeProtocol.MaxPayloadSize)
            return false;

        var msgId = Interlocked.Increment(ref _nextMessageId);
        var header = MessageHeader.CreateRequest(msgId, opCode, payload.Length);
        
        var pending = new PendingRequest(response.Length);
        if (!_pendingRequests.TryAdd(msgId, pending))
            return false;

        try
        {
            // Serialize message header + payload
            var totalSize = sizeof(MessageHeader) + payload.Length;
            
            // Stackalloc optimization for small messages
            byte[]? rented = null;
            Span<byte> buffer = totalSize <= 2048
                ? stackalloc byte[totalSize] 
                : (rented = ArrayPool<byte>.Shared.Rent(totalSize)).AsSpan(0, totalSize);
            
            try
            {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), header);
                payload.CopyTo(buffer[sizeof(MessageHeader)..]);

                if (!_ring.TryWrite(buffer))
                {
                    _pendingRequests.TryRemove(msgId, out _);
                    return false;
                }
            }
            finally
            {
                if (rented is not null)
                    ArrayPool<byte>.Shared.Return(rented);
            }

            // Wait for response or timeout
            if (!pending.WaitHandle.Wait(timeoutMs))
                return false;
                
            return pending.TryGetResponse(destination: response, out responseSize);
        }
        finally
        {
            // Cleanup request tracking
            _pendingRequests.TryRemove(msgId, out _);
            pending.Dispose();
        }
    }

    /// <summary>
    /// Sends a one-way event (no response expected).
    /// </summary>
    public bool SendEvent(BridgeOpCode opCode, ReadOnlySpan<byte> payload)
    {
        if (payload.Length > BridgeProtocol.MaxPayloadSize)
            return false;

        var header = MessageHeader.CreateEvent(opCode, payload.Length);
        var totalSize = sizeof(MessageHeader) + payload.Length;
        
        byte[]? rented = null;
        Span<byte> buffer = totalSize <= 2048 
            ? stackalloc byte[totalSize] 
            : (rented = ArrayPool<byte>.Shared.Rent(totalSize)).AsSpan(0, totalSize);
            
        try
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), header);
            payload.CopyTo(buffer[sizeof(MessageHeader)..]);
            return _ring.TryWrite(buffer);
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Event raised when a message is received (for server-side processing).
    /// </summary>
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    private void ReceiveLoop()
    {
        // Reuse buffer for receiving messages
        var buffer = new byte[sizeof(MessageHeader) + BridgeProtocol.MaxPayloadSize];
        var spin = new SpinWait();

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                if (_ring.TryRead(buffer, out var bytesRead))
                {
                    spin.Reset();
                    
                    if (bytesRead < sizeof(MessageHeader))
                        continue;

                    var header = Unsafe.ReadUnaligned<MessageHeader>(ref buffer[0]);
                    var payload = buffer.AsSpan(sizeof(MessageHeader), header.PayloadSize);

                    if ((header.Flags & MessageFlags.Response) != 0)
                    {
                        // Handle Response: Set result on pending request
                        if (_pendingRequests.TryGetValue(header.MessageId, out var pending))
                        {
                            pending.SetResponse(payload, (header.Flags & MessageFlags.Error) == 0);
                        }
                    }
                    else
                    {
                        // Handle Request/Event: Raise event
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(header, payload.ToArray()));
                    }
                }
                else
                {
                    spin.SpinOnce();
                }
            }
            catch
            {
                // Ignore transient errors
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _cts.Cancel();
        _receiveThread.Join(1000);
        _cts.Dispose();
        
        foreach (var req in _pendingRequests.Values)
            req.Dispose();
        _pendingRequests.Clear();
    }

    /// <summary>
    /// Helper class to coordinate async response waiting.
    /// Thread-safe handling of buffer to prevent race conditions during timeout/dispose.
    /// </summary>
    private sealed class PendingRequest : IDisposable
    {
        public ManualResetEventSlim WaitHandle { get; } = new(false);
        
        private readonly object _lock = new();
        private byte[]? _buffer;
        private int _responseSize;
        private bool _success;

        public PendingRequest(int capacity)
        {
            _buffer = ArrayPool<byte>.Shared.Rent(capacity);
        }

        public void SetResponse(ReadOnlySpan<byte> data, bool success)
        {
            lock (_lock)
            {
                // If buffer is null, we've already timed out/disposed
                if (_buffer is null) return;

                _success = success;
                if (success && data.Length <= _buffer.Length)
                {
                    data.CopyTo(_buffer);
                    _responseSize = data.Length;
                }
            }
            WaitHandle.Set();
        }

        public bool TryGetResponse(Span<byte> destination, out int size)
        {
            lock (_lock)
            {
                size = _responseSize;
                if (_success && _responseSize > 0 && _buffer is not null)
                {
                    if (destination.Length < _responseSize) return false;
                    _buffer.AsSpan(0, _responseSize).CopyTo(destination);
                }
                return _success;
            }
        }
        
        public void Dispose()
        {
            byte[]? bufToReturn;
            lock (_lock)
            {
                bufToReturn = _buffer;
                _buffer = null;
            }
            
            if (bufToReturn is not null)
            {
                ArrayPool<byte>.Shared.Return(bufToReturn);
            }
            WaitHandle.Dispose();
        }
    }
}

/// <summary>
/// Event args for received messages.
/// </summary>
public sealed class MessageReceivedEventArgs(MessageHeader header, byte[] payload) : EventArgs
{
    public MessageHeader Header { get; } = header;
    public byte[] Payload { get; } = payload;
}
