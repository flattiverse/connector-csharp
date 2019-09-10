using System;

namespace Flattiverse
{
    public class Packet
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
        public byte Session;

        public uint ID;            // Identifikation (z.B. Offline-Account)
        public byte Helper;        // Hilfs-Variable für verschiedenes.

        public ushort BaseAddress; // ConnectionID, Universe
        public byte SubAddress;    // Controllable, Map

        public byte[] Payload;     // Chat-Message
        public int Offset;
        public int Length;

        public bool OutOfBand;

        /// <summary>
        /// Parses a packet. Returns false, if the packet can't be received.
        /// </summary>
        /// <param name="data">byte[] of datas where the packet shall be parsed from.</param>
        /// <param name="position">Offset in the byte[] where we start to parse.</param>
        /// <param name="length">The used length of data.</param>
        /// <returns>true, if the Packet could be parsed. false otherwise.</returns>
        public unsafe bool Parse(byte[] data, ref int position, int length)
        {
            if (position >= length)
                return false;

            byte header = data[position];
            OutOfBand = (header & 0b0011_0000) == 0b0011_0000;

            int packetLength;

            fixed (byte* bData = data)
            {
                byte* pData = bData + position + 1;

                if (OutOfBand)
                {
                    Length = header & 0b0000_1111;
                    packetLength = 1 + Length;
                }
                else
                {
                    packetLength = 1
                                 + (((header & 0b1000_0000) == 0b1000_0000) ? 1 : 0)
                                 + (((header & 0b0100_0000) == 0b0100_0000) ? 1 : 0)
                                 + (((header & 0b0000_1000) == 0b0000_1000) ? 2 : 0)
                                 + (((header & 0b0000_0100) == 0b0000_0100) ? 1 : 0)
                                 + (((header & 0b0000_0010) == 0b0000_0010) ? 4 : 0)
                                 + (((header & 0b0000_0001) == 0b0000_0001) ? 1 : 0);

                    switch ((header & 0b0011_0000) >> 4)
                    {
                        case 0b00: // Keine Längen-Angabe: Kein Pay-Load.
                            Length = 0;
                            break;
                        case 0b01: // 1 weiteres Byte Längen-Information.
                            if (length - position < 2)
                                return false;

                            Length = data[position + 1] + 1;

                            pData++;

                            packetLength += 1 + Length;
                            break;
                        case 0b10: // 2 weitere Bytes Längen-Information.
                            if (length - position < 3)
                                return false;

                            Length = *(ushort*)pData + 1;
                            pData += 2;

                            packetLength += 2 + Length;
                            break;
                        // case 0b11: // Keine Längen-Angabe: OutOfBand-Packet.
                        //     break;
                    }
                }

                // Paket noch nicht ganz empfangen.
                if (length - position < packetLength)
                    return false;

                // Command
                if ((header & 0b1000_0000) == 0b1000_0000)
                    Command = *pData++;
                else
                    Command = 0;

                // Session
                if ((header & 0b0100_0000) == 0b0100_0000)
                    Session = *pData++;
                else
                    Session = 0;

                // BaseAddress
                if ((header & 0b0000_1000) == 0b0000_1000)
                {
                    BaseAddress = *(ushort*)pData;
                    pData += 2;
                }
                else
                    BaseAddress = 0;

                // SubAddress
                if ((header & 0b0000_0100) == 0b0000_0100)
                    SubAddress = *pData++;
                else
                    SubAddress = 0;

                // ID
                if ((header & 0b0000_0010) == 0b0000_0010)
                {
                    ID = *(uint*)pData;
                    pData += 4;
                }
                else
                    ID = 0;

                // Helper
                if ((header & 0b0000_0001) == 0b0000_0001)
                    Helper = *pData++;
                else
                    Helper = 0;

                if (Length == 0)
                {
                    Offset = 0;
                    Payload = null;
                }
                else
                {
                    Offset = (int)(pData - bData);
                    Payload = data;
                }
            }

            position += packetLength;

            return true;
        }

        public unsafe bool Write(byte[] data, ref int position)
        {
            int packetLength = 1;
            byte header = 0b0000_0000;

            if (OutOfBand)
                packetLength += Length;
            else
            {
                if (Command > 0)
                {
                    header |= 0b1000_0000;
                    packetLength++;
                }

                if (Session > 0)
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

                if (Length > 65536)
                    throw new ArgumentException("Payload can't exceed 64 KiB.", "Payload");
                else if (Length > 256)
                {
                    header |= 0b0010_0000;
                    packetLength += 2 + Length;
                }
                else if (Length > 0)
                {
                    header |= 0b0001_0000;
                    packetLength += 1 + Length;
                }
            }

            if (data.Length < position + packetLength)
                return false;

            fixed (byte* bData = data)
            {
                byte* pData = bData + position;

                if (OutOfBand)
                {
                    *pData++ = (byte)(0b0011_0000 | Length);

                    for (; Length > 0; Length--)
                        *pData++ = 0x55;
                }
                else
                {
                    *pData++ = header;

                    if (Length > 256)
                    {
                        *(ushort*)pData = (ushort)(Length - 1);
                        pData += 2;
                    }
                    else if (Length > 0)
                    {
                        *pData = (byte)(Length - 1);
                        pData++;
                    }

                    if (Command > 0)
                        *pData++ = Command;

                    if (Session > 0)
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

                    else if (Length > 0)
                        fixed (byte* bPayload = Payload)
                            Buffer.MemoryCopy(bPayload + Offset, pData, data.Length - (pData - bData), Length);
                }
            }

            position += packetLength;

            return true;
        }

        public override string ToString()
        {
            return $"CMD=0x{Command.ToString("X02")} SESSION=0x{Session.ToString("X02")} PLL={Length} Bytes.";
        }
    }
}
