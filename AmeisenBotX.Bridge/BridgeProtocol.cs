using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Bridge;

/// <summary>
/// Bridge protocol version and constants.
/// </summary>
public static class BridgeProtocol
{
    /// <summary>Magic number for protocol validation.</summary>
    public const uint MagicNumber = 0xAFEFCFFE;

    /// <summary>Current protocol version.</summary>
    public const uint ProtocolVersion = 1;

    /// <summary>Size of the shared memory header in bytes.</summary>
    public const int HeaderSize = 64;

    /// <summary>Size of each ring buffer (bot→implant and implant→bot).</summary>
    public const int RingBufferSize = 256 * 1024; // 256KB

    /// <summary>Total shared memory size.</summary>
    public const int TotalSize = HeaderSize + (RingBufferSize * 2);

    /// <summary>Maximum message payload size.</summary>
    public const int MaxPayloadSize = 64 * 1024; // 64KB
}

/// <summary>
/// Shared memory header structure (64 bytes).
/// Uses InlineArray for modern fixed buffer support.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 64)]
public unsafe struct BridgeHeader
{
    /// <summary>Magic number (0xAFEFCFFE) for validation.</summary>
    [FieldOffset(0)] public uint Magic;

    /// <summary>Protocol version.</summary>
    [FieldOffset(4)] public uint Version;

    /// <summary>Bot write sequence (incremented after each write).</summary>
    [FieldOffset(8)] public uint BotSeq;

    /// <summary>Implant write sequence (incremented after each write).</summary>
    [FieldOffset(12)] public uint ImplantSeq;

    /// <summary>Status flags.</summary>
    [FieldOffset(16)] public BridgeStatus Status;

    /// <summary>Reserved for future use.</summary>
    [FieldOffset(20)] public ReservedBuffer Reserved;

    /// <summary>Checks if the header is valid.</summary>
    public readonly bool IsValid => Magic == BridgeProtocol.MagicNumber && Version == BridgeProtocol.ProtocolVersion;

    /// <summary>Checks if both sides are connected.</summary>
    public readonly bool IsFullyConnected => (Status & (BridgeStatus.BotConnected | BridgeStatus.ImplantConnected)) == 
                                              (BridgeStatus.BotConnected | BridgeStatus.ImplantConnected);
}

/// <summary>
/// Reserved buffer using InlineArray (C# 12 feature).
/// </summary>
[InlineArray(11)]
public struct ReservedBuffer
{
    private uint _element0;
}

/// <summary>
/// Bridge status flags.
/// </summary>
[Flags]
public enum BridgeStatus : uint
{
    None = 0,

    /// <summary>Bridge is initialized and ready.</summary>
    Ready = 1 << 0,

    /// <summary>Bot is connected.</summary>
    BotConnected = 1 << 1,

    /// <summary>Implant is connected.</summary>
    ImplantConnected = 1 << 2,

    /// <summary>Error occurred.</summary>
    Error = 1u << 31,
}

/// <summary>
/// Message header (16 bytes, aligned).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
public struct MessageHeader
{
    /// <summary>Unique message ID for request/response correlation.</summary>
    public uint MessageId;

    /// <summary>Operation code identifying the method to call.</summary>
    public ushort OpCode;

    /// <summary>Payload size in bytes (max 64KB).</summary>
    public ushort PayloadSize;

    /// <summary>Message flags.</summary>
    public MessageFlags Flags;

    /// <summary>Reserved for future use.</summary>
    public uint Reserved;

    /// <summary>Creates a request header.</summary>
    public static MessageHeader CreateRequest(uint messageId, BridgeOpCode opCode, int payloadSize) => new()
    {
        MessageId = messageId,
        OpCode = (ushort)opCode,
        PayloadSize = (ushort)payloadSize,
        Flags = MessageFlags.None
    };

    /// <summary>Creates a response header.</summary>
    public static MessageHeader CreateResponse(uint messageId, int payloadSize, bool success = true) => new()
    {
        MessageId = messageId,
        OpCode = 0,
        PayloadSize = (ushort)payloadSize,
        Flags = MessageFlags.Response | (success ? MessageFlags.None : MessageFlags.Error)
    };

    /// <summary>Creates an event header.</summary>
    public static MessageHeader CreateEvent(BridgeOpCode opCode, int payloadSize) => new()
    {
        MessageId = 0,
        OpCode = (ushort)opCode,
        PayloadSize = (ushort)payloadSize,
        Flags = MessageFlags.Event
    };
}

/// <summary>
/// Message flags.
/// </summary>
[Flags]
public enum MessageFlags : uint
{
    None = 0,

    /// <summary>This is a response to a request.</summary>
    Response = 1 << 0,

    /// <summary>This is an event notification (one-way).</summary>
    Event = 1 << 1,

    /// <summary>Error occurred during execution.</summary>
    Error = 1 << 2,

    /// <summary>Request timed out.</summary>
    Timeout = 1 << 3,
}

/// <summary>
/// Bridge operation codes.
/// </summary>
public enum BridgeOpCode : ushort
{
    // === Handshake (0-99) ===
    Ping = 0,
    Pong = 1,
    Disconnect = 2,

    // === Player Info (100-199) ===
    GetPlayerGuid = 100,
    GetPlayerName = 101,
    GetPlayerPosition = 102,
    GetPlayerHealth = 103,
    GetPlayerMana = 104,

    // === Unit Operations (200-299) ===
    GetUnitName = 200,
    GetUnitHealth = 201,
    GetUnitPosition = 202,

    // === Spells & Combat (300-399) ===
    CastSpellById = 300,
    CastSpellByName = 301,
    StopCasting = 302,

    // === Lua Execution (400-499) ===
    ExecuteLua = 400,
    ExecuteLuaWithResult = 401,

    // === Movement (500-599) ===
    ClickToMove = 500,
    FacePosition = 501,
    FaceUnit = 502,

    // === Events from Implant (1000+) ===
    OnCombatStart = 1000,
    OnCombatEnd = 1001,
    OnLootReceived = 1002,
    OnChatMessage = 1003,
}

/// <summary>
/// Error codes returned by bridge operations.
/// </summary>
public enum BridgeError : uint
{
    None = 0,

    /// <summary>Request timed out.</summary>
    Timeout = 1,

    /// <summary>Invalid operation code.</summary>
    InvalidOpCode = 2,

    /// <summary>Payload too large.</summary>
    PayloadTooLarge = 3,

    /// <summary>Serialization failed.</summary>
    SerializationError = 4,

    /// <summary>Bridge not connected.</summary>
    NotConnected = 5,

    /// <summary>WoW function execution failed.</summary>
    ExecutionFailed = 6,

    /// <summary>Unknown error.</summary>
    Unknown = 0xFFFF,
}
