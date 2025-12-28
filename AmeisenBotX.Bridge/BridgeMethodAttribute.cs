namespace AmeisenBotX.Bridge;

/// <summary>
/// Marks a method as a bridge method that can be called across the IPC boundary.
/// </summary>
/// <param name="opCode">The operation code for this method.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class BridgeMethodAttribute(BridgeOpCode opCode) : Attribute
{
    /// <summary>
    /// Gets the operation code for this method.
    /// </summary>
    public BridgeOpCode OpCode { get; } = opCode;

    /// <summary>
    /// Gets or sets the timeout in milliseconds (default: 5000).
    /// </summary>
    public int TimeoutMs { get; init; } = 5000;

    /// <summary>
    /// Gets or sets whether this method should retry on transient failures.
    /// </summary>
    public bool AllowRetry { get; init; } = false;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts (default: 3).
    /// </summary>
    public int MaxRetries { get; init; } = 3;
}
