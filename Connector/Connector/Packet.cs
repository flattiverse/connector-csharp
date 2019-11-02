using Flattiverse.Utils;
using System;

namespace Flattiverse
{
    class Packet
    {
        // Header-Byte: 0b0000 0000

        // 0b1000 0000: Command
        // 0b0100 0000: Session

        // 0b00XX 0000: 00: Kein Payload.
        //              01: Payload bis 256 Bytes.
        //              10: Payload bis 65536 Bytes.
        //              11: OOB (Auffüll-Pakete ohne Payload.)

        // 0b0000 1000: BaseAddress
        // 0b0000 0100: SubAddress
        // 0b0000 0010: ID
        // 0b0000 0001: Helper

        public byte Command;

        public bool SessionUsed;
        public byte Session;

        public uint ID;            // Identifikation (z.B. Offline-Account)
        public byte Helper;        // Hilfs-Variable für verschiedenes.

        public ushort BaseAddress; // ConnectionID, Universe
        public byte SubAddress;    // Controllable, Map

        internal BinaryMemoryReader reader;
        private ManagedBinaryMemoryWriter? writer;

        /// <summary>
        /// OutOfBand is 0, if the packet is a regular Packet. When OutOfband is > 0, then the values represents
        /// the about of OOB Bytes.
        /// </summary>
        public byte OutOfBand;

        /// <summary>
        /// Parses a packet. Returns false, if the packet can't be received.
        /// </summary>
        /// <param name="reader">Offset in the byte[] where we start to parse.</param>
        /// <returns>true, if the Packet could be parsed. false otherwise.</returns>
        public bool Parse(ref BinaryMemoryReader reader)
        {
            if (reader.Size <= 0)
                return false;

            BinaryMemoryReader start = reader;

            byte header = reader.ReadByte();
            int length;

            int packetLength;

            if ((header & 0b0011_0000) == 0b0011_0000)
            {
                OutOfBand = (byte)(header & 0b0000_1111);

                if (reader.Size < OutOfBand)
                {
                    reader = start;
                    return false;
                }

                if (OutOfBand > 0)
                    reader.Jump(OutOfBand);

                OutOfBand++;

                return true;
            }

            // Theoretically we need to set this here, if packet classes will be re-used.
            // OutOfBand = 0;

            packetLength = (((header & 0b1000_0000) == 0b1000_0000) ? 1 : 0)
                            + (((header & 0b0100_0000) == 0b0100_0000) ? 1 : 0)
                            + (((header & 0b0000_1000) == 0b0000_1000) ? 2 : 0)
                            + (((header & 0b0000_0100) == 0b0000_0100) ? 1 : 0)
                            + (((header & 0b0000_0010) == 0b0000_0010) ? 4 : 0)
                            + (((header & 0b0000_0001) == 0b0000_0001) ? 1 : 0);

            switch ((header & 0b0011_0000) >> 4)
            {
                default: // 0b00: Keine Längen-Angabe: Kein Pay-Load.
                    length = 0;
                    break;
                case 0b01: // 1 weiteres Byte Längen-Information.
                    if (reader.Size <= 0)
                        return false;

                    length = reader.ReadByte() + 1;

                    packetLength += length;
                    break;
                case 0b10: // 2 weitere Bytes Längen-Information.
                    if (reader.Size <= 1)
                        return false;

                    length = reader.ReadUInt16() + 1;

                    packetLength += length;
                    break;
            }

            // Paket noch nicht ganz empfangen.
            if (reader.Size < packetLength)
            {
                reader = start;
                return false;
            }

            // Command
            if ((header & 0b1000_0000) == 0b1000_0000)
                Command = reader.ReadByte();
            // else
            //     Command = 0;

            // Session
            if ((header & 0b0100_0000) == 0b0100_0000)
            {
                SessionUsed = true;
                Session = reader.ReadByte();
            }
            // else
            //     Session = 0;

            // BaseAddress
            if ((header & 0b0000_1000) == 0b0000_1000)
                BaseAddress = reader.ReadUInt16();
            // else
            //     BaseAddress = 0;

            // SubAddress
            if ((header & 0b0000_0100) == 0b0000_0100)
                SubAddress = reader.ReadByte();
            // else
            //     SubAddress = 0;

            // ID
            if ((header & 0b0000_0010) == 0b0000_0010)
                ID = reader.ReadUInt32();
            // else
            //     ID = 0;

            // Helper
            if ((header & 0b0000_0001) == 0b0000_0001)
                Helper = reader.ReadByte();
            // else
            //     Helper = 0;

            if (length == 0)
                this.reader = reader.Cut(0);
            else
                this.reader = reader.Cut(length);

            return true;
        }

        public ManagedBinaryMemoryWriter Write()
        {
            writer = new ManagedBinaryMemoryWriter();
            return writer;
        }

        public BinaryMemoryReader Read()
        {
            return reader;
        }

        internal unsafe bool write(byte[] data, ref int position)
        {
            int packetLength = 1;
            byte header = 0b0000_0000;
            long length;

            if (OutOfBand > 0)
            {
                packetLength = OutOfBand;
                length = 0;
            }
            else
            {
                if (Command > 0)
                {
                    header |= 0b1000_0000;
                    packetLength++;
                }

                if (SessionUsed)
                {
                    header |= 0b0100_0000;
                    packetLength++;
                }

                if (BaseAddress > 0)
                {
                    header |= 0b0000_1000;
                    packetLength += 2;
                }

                if (SubAddress > 0)
                {
                    header |= 0b0000_0100;
                    packetLength++;
                }

                if (ID > 0)
                {
                    header |= 0b0000_0010;
                    packetLength += 4;
                }

                if (Helper > 0)
                {
                    header |= 0b0000_0001;
                    packetLength++;
                }

                length = writer == null ? 0 : writer.Size;

                if (length > 65536)
                    throw new ArgumentException("Payload can't exceed 64 KiB.", "Payload");
                else if (length > 256)
                {
                    header |= 0b0010_0000;
                    packetLength += 2 + (int)length;
                }
                else if (length > 0)
                {
                    header |= 0b0001_0000;
                    packetLength += 1 + (int)length;
                }
            }

            if (data.Length < position + packetLength)
                return false;

            fixed (byte* bData = data)
            {
                byte* pData = bData + position;

                if (OutOfBand > 0)
                {
                    *pData++ = (byte)(0b0011_0000 | (OutOfBand - 1));

                    OutOfBand--;

                    for (; OutOfBand > 0; OutOfBand--)
                        *pData++ = 0x55;
                }
                else
                {
                    *pData++ = header;

                    if (length > 256)
                    {
                        *(ushort*)pData = (ushort)(length - 1);
                        pData += 2;
                    }
                    else if (length > 0)
                    {
                        *pData = (byte)(length - 1);
                        pData++;
                    }

                    if (Command > 0)
                        *pData++ = Command;

                    if (SessionUsed)
                        *pData++ = Session;

                    if (BaseAddress > 0)
                    {
                        *(ushort*)pData = BaseAddress;
                        pData += 2;
                    }

                    if (SubAddress > 0)
                        *pData++ = SubAddress;

                    if (ID > 0)
                    {
                        *(uint*)pData = ID;
                        pData += 4;
                    }

                    if (Helper > 0)
                        *pData++ = Helper;

                    if (length > 0)
                        writer!.ToPointer(pData);
                }
            }

            position += packetLength;

            return true;
        }

        public override string ToString()
        {
            return $"CMD=0x{Command.ToString("X02")} SESSION=0x{Session.ToString("X02")} PLL={(writer == null ? reader.Size : writer.Size)} Bytes.";
        }
    }
}
