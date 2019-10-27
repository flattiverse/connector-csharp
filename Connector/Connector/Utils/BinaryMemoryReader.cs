using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Utils
{
    /// <summary>
    /// A binary memory reader. This class can be used to read binary data from a pointer.
    /// </summary>
    public unsafe struct BinaryMemoryReader
    {
        private byte* position;
        private int size;

        private const string spaceError = "Not enough space to complete read operation.";

        /// <summary>
        /// Initializes an UNSAFE binary memory reader.
        /// </summary>
        /// <param name="position">The position you want to start reading from.</param>
        /// <param name="size">The remaining bytes we can read from the given position onwards.</param>
        public BinaryMemoryReader(byte* position, int size)
        {
            this.position = position;
            this.size = size;
        }

        /// <summary>
        /// The remaining bytes we can read.
        /// </summary>
        public int Size => size;

        /// <summary>
        /// The position of the reader.
        /// </summary>
        public byte* Position => position;

        /// <summary>
        /// Reads a string encoded in UTF-8 with 7 bit encoded length prefix.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns null if the string is empty.</remarks>
        public string ReadString()
        {
            int length;
            string result;

            if (size <= 0)
                throw new OutOfMemoryException(spaceError);

            if (*position == 0x00)
            {
                position++;
                size--;

                return null;
            }

            if ((*position & 0x80) == 0x00)
            {
                length = *position;

                if (size < length + 1)
                    throw new OutOfMemoryException(spaceError);

                position++;
                size--;

                result = Encoding.UTF8.GetString(position, length);

                position += length;
                size -= length;

                return result;
            }

            if (size < 2)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 1)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7);

                if (size < length + 2)
                    throw new OutOfMemoryException(spaceError);

                position += 2;
                size -= 2;

                result = Encoding.UTF8.GetString(position, length);

                position += length;
                size -= length;

                return result;
            }

            if (size < 3)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 2)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14);

                if (size < length + 3)
                    throw new OutOfMemoryException(spaceError);

                position += 3;
                size -= 3;

                result = Encoding.UTF8.GetString(position, length);

                position += length;
                size -= length;

                return result;
            }

            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 3)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14) | ((*(position + 3) & 0x7F) << 21);

                if (size < length + 4)
                    throw new OutOfMemoryException(spaceError);

                position += 4;
                size -= 4;

                result = Encoding.UTF8.GetString(position, length);

                position += length;
                size -= length;

                return result;
            }

            if (size < 5)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 4)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14) | ((*(position + 3) & 0x7F) << 21) | ((*(position + 4) & 0x0F) << 28);

                if (length < 0)
                    throw new System.IO.InvalidDataException("Ambiguous length information.");

                if (size < length + 5)
                    throw new OutOfMemoryException(spaceError);

                position += 5;
                size -= 5;

                result = Encoding.UTF8.GetString(position, length);

                position += length;
                size -= length;

                return result;
            }

            throw new OutOfMemoryException(spaceError);
        }

        /// <summary>
        /// Reads a string encoded in UTF-8 with 7 bit encoded length prefix.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns an empty string if the string is empty and not null.</remarks>
        public string ReadStringNonNull()
        {
            return ReadString() ?? "";
        }

        /// <summary>
        /// Reads a string encoded in UTF-8.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns null if the string is empty.</remarks>
        public string ReadVanillaString(int bytes)
        {
            if (bytes <= 0)
                return null;

            if (size < bytes)
                throw new OutOfMemoryException(spaceError);

            size -= bytes;
            position += bytes;

            return Encoding.UTF8.GetString(position - bytes, bytes);
        }

        /// <summary>
        /// Reads a string encoded in UTF-8.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns an empty string if the string is empty and not null.</remarks>
        public string ReadVanillaStringNonNull(int bytes)
        {
            return ReadVanillaString(bytes) ?? "";
        }

        /// <summary>
        /// Reads a boolean.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            size--;
            return *position++ != 0x00;
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            size--;
            return *position++;
        }

        /// <summary>
        /// Reads a signed byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            size--;
            return *(sbyte*)position++;
        }

        /// <summary>
        /// Cuts a sub reader at the current position with the specified length.
        /// </summary>
        /// <param name="size">The size of the new reader.</param>
        /// <returns>The new reader.</returns>
        public BinaryMemoryReader Cut(int size)
        {
            if (this.size < size)
                throw new OutOfMemoryException(spaceError);

            position += size;
            this.size -= size;

            return new BinaryMemoryReader(position - size, size);
        }

        /// <summary>
        /// Jumps step bytes forward.
        /// </summary>
        /// <param name="step">The amount of bytes to jump.</param>
        public void Jump(int step)
        {
            if (step < 0)
                throw new ArgumentException("step can't be negative.", "step");

            if (size < step)
                throw new OutOfMemoryException(spaceError);

            position += step;
            size -= step;
        }

        /// <summary>
        /// Reads an unsigned short.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            if (size < 2)
                throw new OutOfMemoryException(spaceError);

            size -= 2;
            position += 2;

            return *(ushort*)(position - 2);
        }

        /// <summary>
        /// Reads a short.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            if (size < 2)
                throw new OutOfMemoryException(spaceError);

            size -= 2;
            position += 2;

            return *(short*)(position - 2);
        }

        /// <summary>
        /// Reads a 3 byte integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadUInt24()
        {
            if (size < 3)
                throw new OutOfMemoryException(spaceError);

            size -= 3;

            int result = *position++ << 16;

            result |= *(ushort*)position;
            position += 2;

            return result;
        }

        /// <summary>
        /// Reads an unsigned int.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            size -= 4;
            position += 4;

            return *(uint*)(position - 4);
        }

        /// <summary>
        /// Reads a int.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            size -= 4;
            position += 4;

            return *(int*)(position - 4);
        }

        /// <summary>
        /// Reads an unsigned long.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            size -= 8;
            position += 8;

            return *(ulong*)(position - 8);
        }

        /// <summary>
        /// Reads a long.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            size -= 8;
            position += 8;

            return *(long*)(position - 8);
        }

        /// <summary>
        /// Reads a float.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
        {
            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            size -= 4;
            position += 4;

            return *(float*)(position - 4);
        }

        /// <summary>
        /// Reads a char.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            if ((*position & 0b10000000) == 0b00000000)
            {
                size--;
                position++;

                return (char)*(position - 1);
            }

            if ((*position & 0b11100000) == 0b11000000)
            {
                if (size < 2)
                    throw new OutOfMemoryException(spaceError);

                size -= 2;
                position += 2;

                return (char)((*(position - 1) & 0b00111111) | ((*(position - 2) & 0b00011111) << 6));
            }

            if ((*position & 0b11110000) == 0b11100000)
            {
                if (size < 3)
                    throw new OutOfMemoryException(spaceError);

                size -= 3;
                position += 3;

                return (char)((*(position - 1) & 0b00111111) | ((*(position - 2) & 0b00111111) << 6) | ((*(position - 3) & 0b00001111) << 12));
            }

            throw new InvalidOperationException("Char in memory is too wide for char datatype.");
        }

        /// <summary>
        /// Reads a double.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            size -= 8;
            position += 8;

            return *(double*)(position - 8);
        }

        /// <summary>
        /// Reads a timespan.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan ReadTimeSpan()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            size -= 8;
            position += 8;

            return new TimeSpan(*(long*)(position - 8));
        }

        /// <summary>
        /// Reads a datetime.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime ReadDateTime()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            size -= 8;
            position += 8;

            return new DateTime(*(long*)(position - 8));
        }

        /// <summary>
        /// Reads a decimal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ReadDecimal()
        {
            if (size < 16)
                throw new OutOfMemoryException(spaceError);

            size -= 16;
            position += 16;

            return new decimal(new int[] { *(int*)(position - 16), *(int*)(position - 12), *(int*)(position - 8), *(int*)(position - 4) });
        }

        /// <summary>
        /// Reads count bytes from the current position into data starting at offset.
        /// </summary>
        /// <param name="data">The byte array where data will be written to.</param>
        /// <param name="offset">The position in the byte array where those data will be written to.</param>
        /// <param name="count">The amount of bytes which will be written.</param>
        public void ReadBytes(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException("data", "data can't be null.");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "offset can't be negative.");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "count can't be negative.");

            if (offset + count < data.Length)
                throw new ArgumentOutOfRangeException("count", "offset + count bigger than data.Length.");

            if (size < count)
                throw new OutOfMemoryException(spaceError);

            fixed (byte* pData = data)
                Buffer.MemoryCopy(position, pData + offset, count, count);

            position += count;
            size -= count;
        }

        /// <summary>
        /// Peeks a string encoded in UTF-8 with 7 bit encoded length prefix.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns null if the string is empty.</remarks>
        public string PeekString()
        {
            int length;

            if (size <= 0)
                throw new OutOfMemoryException(spaceError);

            if (*position == 0x00)
                return null;

            if ((*position & 0x80) == 0x00)
            {
                length = *position;

                if (size < length + 1)
                    throw new OutOfMemoryException(spaceError);

                return Encoding.UTF8.GetString(position + 1, length);
            }

            if (size < 2)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 1)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7);

                if (size < length + 2)
                    throw new OutOfMemoryException(spaceError);

                return Encoding.UTF8.GetString(position + 2, length);
            }

            if (size < 3)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 2)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14);

                if (size < length + 3)
                    throw new OutOfMemoryException(spaceError);

                return Encoding.UTF8.GetString(position + 3, length);
            }

            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 3)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14) | ((*(position + 3) & 0x7F) << 21);

                if (size < length + 4)
                    throw new OutOfMemoryException(spaceError);

                return Encoding.UTF8.GetString(position + 4, length);
            }

            if (size < 5)
                throw new OutOfMemoryException(spaceError);

            if (((*(position + 4)) & 0x80) == 0x00)
            {
                length = (*position & 0x7F) | ((*(position + 1) & 0x7F) << 7) | ((*(position + 2) & 0x7F) << 14) | ((*(position + 3) & 0x7F) << 21) | ((*(position + 4) & 0x0F) << 28);

                if (length < 0)
                    throw new System.IO.InvalidDataException("Ambiguous length information.");

                if (size < length + 5)
                    throw new OutOfMemoryException(spaceError);

                return Encoding.UTF8.GetString(position + 5, length);
            }

            throw new OutOfMemoryException(spaceError);
        }

        /// <summary>
        /// Peeks a string encoded in UTF-8 with 7 bit encoded length prefix.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns an empty string if the string is empty and not null.</remarks>
        public string PeekStringNonNull()
        {
            return PeekString() ?? "";
        }

        /// <summary>
        /// Peeks a string encoded in UTF-8.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns null if the string is empty.</remarks>
        public string PeekVanillaString(int bytes)
        {
            if (bytes <= 0)
                return null;

            if (size < bytes)
                throw new OutOfMemoryException(spaceError);

            return Encoding.UTF8.GetString(position, bytes);
        }

        /// <summary>
        /// Peeks a string encoded in UTF-8.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>Returns an empty string if the string is empty and not null.</remarks>
        public string PeekVanillaStringNonNull(int bytes)
        {
            return PeekVanillaString(bytes) ?? "";
        }

        /// <summary>
        /// Peeks a boolean.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekBoolean()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            return *position != 0x00;
        }

        /// <summary>
        /// Peeks a byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte PeekByte()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            return *position;
        }

        /// <summary>
        /// Peeks a signed byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte PeekSByte()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            return *(sbyte*)position;
        }

        /// <summary>
        /// Peeks an unsigned short.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort PeekUInt16()
        {
            if (size < 2)
                throw new OutOfMemoryException(spaceError);

            return *(ushort*)position;
        }

        /// <summary>
        /// Peeks a short.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PeekInt16()
        {
            if (size < 2)
                throw new OutOfMemoryException(spaceError);

            return *(short*)position;
        }

        /// <summary>
        /// Peeks a 3 byte integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekUInt24()
        {
            if (size < 3)
                throw new OutOfMemoryException(spaceError);

            return (*position << 16) | *(ushort*)(position + 1);
        }

        /// <summary>
        /// Peeks an unsigned int.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint PeekUInt32()
        {
            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            return *(uint*)position;
        }

        /// <summary>
        /// Peeks a int.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PeekInt32()
        {
            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            return *(int*)position;
        }

        /// <summary>
        /// Peeks an unsigned long.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong PeekUInt64()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            return *(ulong*)position;
        }

        /// <summary>
        /// Peeks a long.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long PeekInt64()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            return *(long*)position;
        }

        /// <summary>
        /// Peeks a char.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char PeekChar()
        {
            if (size < 1)
                throw new OutOfMemoryException(spaceError);

            if ((*position & 0b10000000) == 0b00000000)
                return (char)*position;

            if ((*position & 0b11100000) == 0b11000000)
            {
                if (size < 2)
                    throw new OutOfMemoryException(spaceError);

                return (char)((*(position + 1) & 0b00111111) | ((*position & 0b00011111) << 6));
            }

            if ((*position & 0b11110000) == 0b11100000)
            {
                if (size < 3)
                    throw new OutOfMemoryException(spaceError);

                return (char)((*(position + 2) & 0b00111111) | ((*(position + 1) & 0b00111111) << 6) | ((*position & 0b00001111) << 12));
            }

            throw new InvalidOperationException("Char in memory is too wide for char datatype.");
        }

        /// <summary>
        /// Peeks a float.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PeekSingle()
        {
            if (size < 4)
                throw new OutOfMemoryException(spaceError);

            return *(float*)position;
        }

        /// <summary>
        /// Peeks a double.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double PeekDouble()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            return *(double*)position;
        }

        /// <summary>
        /// Peeks a timespan.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan PeekTimeSpan()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            return new TimeSpan(*(long*)position);
        }

        /// <summary>
        /// Peeks a datetime.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime PeekDateTime()
        {
            if (size < 8)
                throw new OutOfMemoryException(spaceError);

            return new DateTime(*(long*)position);
        }

        /// <summary>
        /// Peeks a decimal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal PeekDecimal()
        {
            if (size < 16)
                throw new OutOfMemoryException(spaceError);

            return new decimal(new int[] { *(int*)position, *(int*)(position + 4), *(int*)(position + 8), *(int*)(position + 12) });
        }

        /// <summary>
        /// Peeks count bytes from the current position into data starting at offset.
        /// </summary>
        /// <param name="data">The byte array where data will be written to.</param>
        /// <param name="offset">The position in the byte array where those data will be written to.</param>
        /// <param name="count">The amount of bytes which will be written.</param>
        public void PeekBytes(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException("data", "data can't be null.");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "offset can't be negative.");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "count can't be negative.");

            if (offset + count < data.Length)
                throw new ArgumentOutOfRangeException("count", "offset + count bigger than data.Length.");

            if (size < count)
                throw new OutOfMemoryException(spaceError);

            fixed (byte* pData = data)
                Buffer.MemoryCopy(position, pData + offset, count, count);
        }
    }
}
