using System.Runtime.CompilerServices;

namespace Flattiverse.Connector.Network;

/// <summary>
/// Raw byte buffer that holds one or more serialized Flattiverse packets before they are sent.
/// </summary>
class SendBuffer
{
    /// <summary>
    /// Current write boundary inside <see cref="Data" /> in bytes.
    /// </summary>
    public int Position;

    /// <summary>
    /// Underlying raw byte buffer.
    /// </summary>
    public readonly byte[] Data;

    /// <summary>
    /// Creates one raw send buffer with the specified capacity.
    /// </summary>
    /// <param name="size">Buffer capacity in bytes.</param>
    public SendBuffer(int size)
    {
        Data = new byte[size];
    }

    /// <summary>
    /// Opens a new packet writer at the current end of the buffer.
    /// </summary>
    public PacketWriter Write()
    {
        return new PacketWriter(this);
    }

    /// <summary>
    /// Opens a new packet writer at the current end of the buffer with an explicit command byte.
    /// </summary>
    public PacketWriter Write(byte command)
    {
        return new PacketWriter(this, command);
    }

    /// <summary>
    /// Opens a new packet writer at the current end of the buffer with explicit command and session bytes.
    /// </summary>
    public PacketWriter Write(byte command, byte session)
    {
        return new PacketWriter(this, command, session);
    }

    /// <summary>
    /// Appends the occupied bytes of another serialized send buffer.
    /// </summary>
    /// <param name="buffer">Source buffer whose occupied bytes should be appended.</param>
    /// <returns><see langword="true" /> if the bytes fit; otherwise <see langword="false" />.</returns>
    public bool Send(SendBuffer buffer)
    {
        if (Position + buffer.Position > Data.Length)
            return false;

        Unsafe.CopyBlock(ref Data[Position], ref buffer.Data[0], (uint)buffer.Position);

        Position += buffer.Position;

        return true;
    }

    /// <summary>
    /// Occupied prefix of the buffer that is ready to send.
    /// </summary>
    public Memory<byte> Buffer => Data.AsMemory(0, Position);

    /// <summary>
    /// Whether the buffer currently contains at least one serialized byte.
    /// </summary>
    public bool HasData => Position > 0;

    /// <summary>
    /// Total capacity of the underlying buffer in bytes.
    /// </summary>
    public int Length => Data.Length;

    /// <summary>
    /// Clears the occupied prefix and resets the buffer to empty.
    /// </summary>
    public void Reset()
    {
        Position = 0;
    }

    /// <summary>
    /// Advances the occupied prefix by a caller-provided number of bytes.
    /// </summary>
    /// <param name="size">Number of bytes that were written manually.</param>
    public void DoneWriting(int size)
    {
        Position += size;
    }
}
