using System.Buffers;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Result type for bridge operations.
/// </summary>
public readonly struct BridgeResult<T>
{
    private readonly T? _value;
    private readonly BridgeError _error;
    private readonly bool _isSuccess;

    private BridgeResult(T value)
    {
        _value = value;
        _error = BridgeError.None;
        _isSuccess = true;
    }

    private BridgeResult(BridgeError error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }

    /// <summary>Gets whether the operation succeeded.</summary>
    public bool IsSuccess => _isSuccess;

    /// <summary>Gets whether the operation failed.</summary>
    public bool IsError => !_isSuccess;

    /// <summary>Gets the error code (only valid if IsError is true).</summary>
    public BridgeError Error => _error;

    /// <summary>Gets the value (only valid if IsSuccess is true).</summary>
    public T Value => _isSuccess ? _value! : throw new InvalidOperationException($"Cannot access value of failed result (Error: {_error})");

    /// <summary>Creates a successful result.</summary>
    public static BridgeResult<T> Success(T value) => new(value);

    /// <summary>Creates a failed result.</summary>
    public static BridgeResult<T> Failure(BridgeError error) => new(error);

    /// <summary>Tries to get the value.</summary>
    public bool TryGetValue(out T value)
    {
        value = _value!;
        return _isSuccess;
    }

    /// <summary>Gets the value or a default if failed.</summary>
    public T GetValueOrDefault(T defaultValue = default!) => _isSuccess ? _value! : defaultValue;
    
    public static implicit operator BridgeResult<T>(T value) => new(value);
    public static implicit operator BridgeResult<T>(BridgeError error) => new(error);
}

/// <summary>
/// Simple result type for void operations.
/// </summary>
public readonly struct BridgeResult
{
    private readonly BridgeError _error;
    private readonly bool _isSuccess;

    private BridgeResult(bool success, BridgeError error = BridgeError.None)
    {
        _isSuccess = success;
        _error = error;
    }

    /// <summary>Gets whether the operation succeeded.</summary>
    public bool IsSuccess => _isSuccess;

    /// <summary>Gets whether the operation failed.</summary>
    public bool IsError => !_isSuccess;

    /// <summary>Gets the error code (only valid if IsError is true).</summary>
    public BridgeError Error => _error;

    /// <summary>Creates a successful result.</summary>
    public static BridgeResult Success() => new(true);

    /// <summary>Creates a failed result.</summary>
    public static BridgeResult Failure(BridgeError error) => new(false, error);
    
    public static implicit operator BridgeResult(BridgeError error) => new(false, error);
}

/// <summary>
/// Pool for renting temporary buffers during bridge calls.
/// </summary>
file static class BufferPool
{
    private static readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

    /// <summary>Rents a buffer of at least the specified size.</summary>
    public static byte[] Rent(int minimumLength) => _pool.Rent(minimumLength);

    /// <summary>Returns a rented buffer to the pool.</summary>
    public static void Return(byte[] buffer, bool clearArray = false) => _pool.Return(buffer, clearArray);
}
