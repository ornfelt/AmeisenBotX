using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AmeisenBotX.Bridge;

/// <summary>
/// High-performance marshalling for bridge method parameters and return values.
/// Uses unsafe code and span-based APIs for zero-allocation paths.
/// </summary>
public static unsafe class BridgeMarshaller
{
    /// <summary>
    /// Writes a primitive value to a span using direct memory access.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write<T>(Span<byte> buffer, T value) where T : unmanaged
    {
        if (buffer.Length < sizeof(T))
        {
            ThrowBufferTooSmall();
        }

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
        return sizeof(T);
    }

    /// <summary>
    /// Reads a primitive value from a span using direct memory access.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(ReadOnlySpan<byte> buffer) where T : unmanaged
    {
        if (buffer.Length < sizeof(T))
        {
            ThrowBufferTooSmall();
        }

        return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <summary>
    /// Writes a string as length-prefixed UTF-8.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteString(Span<byte> buffer, ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), (ushort)0);
            return sizeof(ushort);
        }

        int maxBytes = Encoding.UTF8.GetMaxByteCount(value.Length);
        if (buffer.Length < sizeof(ushort) + maxBytes)
        {
            ThrowBufferTooSmall();
        }

        int bytesWritten = Encoding.UTF8.GetBytes(value, buffer[sizeof(ushort)..]);
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), (ushort)bytesWritten);
        return sizeof(ushort) + bytesWritten;
    }

    /// <summary>
    /// Reads a length-prefixed UTF-8 string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadString(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(ushort))
        {
            ThrowBufferTooSmall();
        }

        ushort length = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(buffer));

        return length switch
        {
            0 => string.Empty,
            _ when buffer.Length < sizeof(ushort) + length => throw new ArgumentException("Buffer too small for string data"),
            _ => Encoding.UTF8.GetString(buffer.Slice(sizeof(ushort), length))
        };
    }

    /// <summary>
    /// Writes a boolean value (1 byte).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBool(Span<byte> buffer, bool value)
    {
        if (buffer.IsEmpty)
        {
            ThrowBufferTooSmall();
        }

        MemoryMarshal.GetReference(buffer) = value ? (byte)1 : (byte)0;
        return 1;
    }

    /// <summary>
    /// Reads a boolean value (1 byte).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadBool(ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            ThrowBufferTooSmall();
        }

        return MemoryMarshal.GetReference(buffer) != 0;
    }

    /// <summary>
    /// Writes a byte array with length prefix.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBytes(Span<byte> buffer, ReadOnlySpan<byte> value)
    {
        if (buffer.Length < sizeof(int) + value.Length)
        {
            ThrowBufferTooSmall();
        }

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value.Length);
        value.CopyTo(buffer[sizeof(int)..]);
        return sizeof(int) + value.Length;
    }

    /// <summary>
    /// Reads a byte array with length prefix.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadBytes(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(int))
        {
            ThrowBufferTooSmall();
        }

        int length = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(buffer));

        return length switch
        {
            0 => [],
            _ when buffer.Length < sizeof(int) + length => throw new ArgumentException("Buffer too small for byte array data"),
            _ => buffer.Slice(sizeof(int), length).ToArray()
        };
    }

    /// <summary>
    /// Writes raw bytes without length prefix (for fixed-size data).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteRaw(Span<byte> buffer, ReadOnlySpan<byte> value)
    {
        if (buffer.Length < value.Length)
        {
            ThrowBufferTooSmall();
        }

        value.CopyTo(buffer);
        return value.Length;
    }

    /// <summary>
    /// Writes a struct directly to memory.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteStruct<T>(Span<byte> buffer, in T value) where T : unmanaged
    {
        if (buffer.Length < sizeof(T))
        {
            ThrowBufferTooSmall();
        }

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
        return sizeof(T);
    }

    /// <summary>
    /// Reads a struct directly from memory.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadStruct<T>(ReadOnlySpan<byte> buffer) where T : unmanaged
    {
        if (buffer.Length < sizeof(T))
        {
            ThrowBufferTooSmall();
        }

        return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <summary>
    /// Calculates the size needed to serialize a string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetStringSize(ReadOnlySpan<char> value)
        => value.IsEmpty ? sizeof(ushort) : sizeof(ushort) + Encoding.UTF8.GetByteCount(value);

    /// <summary>
    /// Calculates the size needed to serialize a byte array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetBytesSize(ReadOnlySpan<byte> value)
        => sizeof(int) + value.Length;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowBufferTooSmall()
        => throw new ArgumentException("Buffer too small");
}

/// <summary>
/// Interface for custom serializable types.
/// </summary>
public interface IBridgeSerializable
{
    /// <summary>Gets the size in bytes needed for serialization.</summary>
    int GetSize();

    /// <summary>Serializes to a byte buffer.</summary>
    int WriteTo(Span<byte> buffer);

    /// <summary>Deserializes from a byte buffer.</summary>
    void ReadFrom(ReadOnlySpan<byte> buffer);
}

/// <summary>
/// Ref struct for zero-allocation buffer building.
/// </summary>
public ref struct BufferWriter
{
    private readonly Span<byte> _buffer;
    private int _position;

    public BufferWriter(Span<byte> buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public readonly int Position => _position;
    public readonly int Remaining => _buffer.Length - _position;
    public readonly Span<byte> Written => _buffer[.._position];
    public readonly Span<byte> Free => _buffer[_position..];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T value) where T : unmanaged
    {
        BridgeMarshaller.Write(Free, value);
        _position += Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(ReadOnlySpan<char> value)
        => _position += BridgeMarshaller.WriteString(Free, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(ReadOnlySpan<byte> value)
        => _position += BridgeMarshaller.WriteBytes(Free, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteRaw(ReadOnlySpan<byte> value)
        => _position += BridgeMarshaller.WriteRaw(Free, value);
}

/// <summary>
/// Ref struct for zero-allocation buffer reading.
/// </summary>
public ref struct BufferReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private int _position;

    public BufferReader(ReadOnlySpan<byte> buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public readonly int Position => _position;
    public readonly int Remaining => _buffer.Length - _position;
    public readonly ReadOnlySpan<byte> Unread => _buffer[_position..];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        T value = BridgeMarshaller.Read<T>(Unread);
        _position += Unsafe.SizeOf<T>();
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        ushort length = BridgeMarshaller.Read<ushort>(Unread);
        string value = BridgeMarshaller.ReadString(Unread);
        _position += sizeof(ushort) + length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadBytes()
    {
        int length = BridgeMarshaller.Read<int>(Unread);
        byte[] value = BridgeMarshaller.ReadBytes(Unread);
        _position += sizeof(int) + length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Skip(int count)
        => _position += count;
}
