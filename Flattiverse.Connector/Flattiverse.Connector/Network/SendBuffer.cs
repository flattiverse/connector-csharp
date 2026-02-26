using System.Runtime.CompilerServices;

namespace Flattiverse.Connector.Network;

class SendBuffer
{
    public int Position;
    public readonly byte[] Data;

    public SendBuffer(int size)
    {
        Data = new byte[size];
    }

    public PacketWriter Write()
    {
        return new PacketWriter(this);
    }

    public PacketWriter Write(byte command)
    {
        return new PacketWriter(this, command);
    }

    public PacketWriter Write(byte command, byte session)
    {
        return new PacketWriter(this, command, session);
    }

    public bool Send(SendBuffer buffer)
    {
        if (Position + buffer.Position > Data.Length)
            return false;

        Unsafe.CopyBlock(ref Data[Position], ref buffer.Data[0], (uint)buffer.Position);

        Position += buffer.Position;

        return true;
    }

    public Memory<byte> Buffer => Data.AsMemory(0, Position);

    public bool HasData => Position > 0;

    /// <summary>
    /// The complete length of data.
    /// </summary>
    public int Length => Data.Length;

    public void Reset()
    {
        Position = 0;
    }

    public void DoneWriting(int size)
    {
        Position += size;
    }
}
