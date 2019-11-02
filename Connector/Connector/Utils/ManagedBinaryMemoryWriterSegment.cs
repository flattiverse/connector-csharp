using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Flattiverse.Utils
{
    class ManagedBinaryMemoryWriterSegment
    {
        public readonly ManagedBinaryMemoryWriter writer;
        public ManagedBinaryMemoryWriterSegment? Next;

        private readonly byte[] data;
        private int position;
        private int size;

        public ManagedBinaryMemoryWriterSegment(ManagedBinaryMemoryWriter writer, int size)
        {
            this.writer = writer;
            data = new byte[size];
            this.size = size;
            writer.MoveSegment(this);
        }

        public int Length => position;

        /// <summary>
        /// Writes a string in UTF-8 encoding with 7 bit encoded length prefix.
        /// </summary>
        /// <param name="text">The string to write.</param>
        public unsafe void Write(string? text)
        {
            if (text == null)
            {
                if (size < 1)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                    Next.Write(text);
                    return;
                }

                data[position++] = 0x00;
                size--;

                return;
            }

            int length = Encoding.UTF8.GetByteCount(text);

            if (length < 128)
            {
                if (size < length + 1)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                    Next.Write(text);
                    return;
                }

                data[position++] = (byte)length;

                fixed (char* chars = text)
                fixed (byte* bData = data)
                    Encoding.UTF8.GetBytes(chars, text.Length, bData + position, size - 1);

                size -= length + 1;
            }
            else if (length < 16384)
            {
                if (size < length + 2)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, length + 1024);
                    Next.Write(text);
                    return;
                }

                data[position++] = (byte)(length | 0x80);
                data[position++] = (byte)(length >> 7);

                fixed (char* chars = text)
                fixed (byte* bData = data)
                    Encoding.UTF8.GetBytes(chars, text.Length, bData + position, size - 2);

                size -= length + 2;
            }
            else if (length < 2097152)
            {
                if (size < length + 3)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, length + 1024);
                    Next.Write(text);
                    return;
                }

                data[position++] = (byte)(length | 0x80);
                data[position++] = (byte)((length >> 7) | 0x80);
                data[position++] = (byte)(length >> 14);

                fixed (char* chars = text)
                fixed (byte* bData = data)
                    Encoding.UTF8.GetBytes(chars, text.Length, bData + position, size - 3);

                size -= length + 3;
            }
            else if (length < 268435456)
            {
                if (size < length + 4)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, length + 1024);
                    Next.Write(text);
                    return;
                }

                data[position++] = (byte)(length | 0x80);
                data[position++] = (byte)((length >> 7) | 0x80);
                data[position++] = (byte)((length >> 14) | 0x80);
                data[position++] = (byte)(length >> 21);

                fixed (char* chars = text)
                fixed (byte* bData = data)
                    Encoding.UTF8.GetBytes(chars, text.Length, bData + position, size - 4);

                size -= length + 4;
            }
            else
            {
                if (size < length + 5)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, length + 5);
                    Next.Write(text);
                    return;
                }

                data[position++] = (byte)(length | 0x80);
                data[position++] = (byte)((length >> 7) | 0x80);
                data[position++] = (byte)((length >> 14) | 0x80);
                data[position++] = (byte)((length >> 21) | 0x80);
                data[position++] = (byte)(length >> 28);

                fixed (char* chars = text)
                fixed (byte* bData = data)
                    Encoding.UTF8.GetBytes(chars, text.Length, bData + position, size - 5);

                size -= length + 5;
            }

            position += length;
        }

        /// <summary>
        /// Writes a string in UTF-8 encoding.
        /// </summary>
        /// <param name="text">The string to write.</param>
        /// <returns>The number of bytes written.</returns>
        public unsafe int WriteVanillaString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int requiredSize = Encoding.UTF8.GetByteCount(text);

            if (size < requiredSize)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, requiredSize + 1024);
                Next.Write(text);
                return requiredSize;
            }

            fixed (char* chars = text)
            fixed (byte* bData = data)
                requiredSize = Encoding.UTF8.GetBytes(chars, text.Length, bData + position, size);

            size -= requiredSize;
            position += requiredSize;

            return requiredSize;
        }

        /// <summary>
        /// Writes a bool.
        /// </summary>
        /// <param name="data">The vool to write.</param>
        public void Write(bool data)
        {
            if (size < 1)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            this.data[position++] = data ? (byte)0xFF : (byte)0x00;
            size--;
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">The byte to write.</param>
        public void Write(byte data)
        {
            if (size < 1)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            this.data[position++] = data;
            size--;
        }

        /// <summary>
        /// Writes a signed byte.
        /// </summary>
        /// <param name="data">The signed byte to write.</param>
        public unsafe void Write(sbyte data)
        {
            if (size < 1)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(sbyte*)(bData + position) = data;

            position++;
            size--;
        }

        /// <summary>
        /// Writes an unsigned short.
        /// </summary>
        /// <param name="data">The unsigned short to write.</param>
        public unsafe void Write(ushort data)
        {
            if (size < 2)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(ushort*)(bData + position) = data;

            position += 2;
            size -= 2;
        }

        /// <summary>
        /// Writes a short.
        /// </summary>
        /// <param name="data">The short to write.</param>
        public unsafe void Write(short data)
        {
            if (size < 2)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(short*)(bData + position) = data;

            position += 2;
            size -= 2;
        }

        /// <summary>
        /// Writes a 3 byte integer.
        /// </summary>
        /// <param name="data">The integer to write.</param>
        public unsafe void WriteUInt24(int data)
        {
            if (size < 3)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            this.data[position++] = (byte)(data / 65536);

            fixed (byte* bData = this.data)
                *(ushort*)(bData + position) = (ushort)data;

            position += 2;
            size -= 3;
        }

        /// <summary>
        /// Writes an unsigned int.
        /// </summary>
        /// <param name="data">The unsigned integer to write.</param>
        public unsafe void Write(uint data)
        {
            if (size < 4)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(uint*)(bData + position) = data;

            position += 4;
            size -= 4;
        }

        /// <summary>
        /// Writes an int.
        /// </summary>
        /// <param name="data">The integer to write.</param>
        public unsafe void Write(int data)
        {
            if (size < 4)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(int*)(bData + position) = data;

            position += 4;
            size -= 4;
        }

        /// <summary>
        /// Writes an unsigned long.
        /// </summary>
        /// <param name="data">The unsigned long to write.</param>
        public unsafe void Write(ulong data)
        {
            if (size < 8)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(ulong*)(bData + position) = data;

            position += 8;
            size -= 8;
        }

        /// <summary>
        /// Writes a long.
        /// </summary>
        /// <param name="data">The long to write.</param>
        public unsafe void Write(long data)
        {
            if (size < 8)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(long*)(bData + position) = data;

            position += 8;
            size -= 8;
        }

        /// <summary>
        /// Writes a char.
        /// </summary>
        /// <param name="data">The char to write.</param>
        public void Write(char data)
        {
            if (data < 128)
            {
                if (size < 1)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                    Next.Write(data);
                    return;
                }

                this.data[position++] = (byte)data;

                size--;

                return;
            }

            if (data < 2048)
            {
                if (size < 2)
                {
                    Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                    Next.Write(data);
                    return;
                }

                this.data[position++] = (byte)((data >> 6) | 0b11000000);
                this.data[position++] = (byte)((data & 0b00111111) | 0b10000000);

                size -= 2;

                return;
            }

            if (size < 3)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            this.data[position++] = (byte)((data >> 12) | 0b11100000);
            this.data[position++] = (byte)(((data >> 6) & 0b00111111) | 0b10000000);
            this.data[position++] = (byte)((data & 0b00111111) | 0b10000000);

            size -= 3;
        }

        /// <summary>
        /// Writes a float.
        /// </summary>
        /// <param name="data">The float to write.</param>
        public unsafe void Write(float data)
        {
            if (size < 4)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(float*)(bData + position) = data;

            position += 4;
            size -= 4;
        }

        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="data">The double to write.</param>
        public unsafe void Write(double data)
        {
            if (size < 8)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(double*)(bData + position) = data;

            position += 8;
            size -= 8;
        }

        /// <summary>
        /// Writes a timespan.
        /// </summary>
        /// <param name="data">The timespan to write.</param>
        public unsafe void Write(TimeSpan data)
        {
            if (size < 8)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(long*)(bData + position) = data.Ticks;

            position += 8;
            size -= 8;
        }

        /// <summary>
        /// Writes a datetime.
        /// </summary>
        /// <param name="data">The datetime to write.</param>
        public unsafe void Write(DateTime data)
        {
            if (size < 8)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            fixed (byte* bData = this.data)
                *(long*)(bData + position) = data.Ticks;

            position += 8;
            size -= 8;
        }

        /// <summary>
        /// Writes a decimal.
        /// </summary>
        /// <param name="data">The decimal to write.</param>
        public unsafe void Write(decimal data)
        {
            if (size < 16)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, 1024);
                Next.Write(data);
                return;
            }

            int[] values = decimal.GetBits(data);

            fixed (byte* bData = this.data)
            {
                *(int*)(bData + position) = values[0];
                *(int*)(bData + position + 4) = values[1];
                *(int*)(bData + position + 8) = values[2];
                *(int*)(bData + position + 12) = values[3];
            }

            size -= 16;
            position += 16;
        }

        /// <summary>
        /// Writes count bytes from the current position into data starting at offset.
        /// </summary>
        /// <param name="data">The byte array where data will be read from.</param>
        /// <param name="offset">The position in the byte array where those data will be read from.</param>
        /// <param name="count">The amount of bytes which will be read.</param>
        /// <remarks>BEWARE: This method is also NOT DOING input checks of the given parameters.</remarks>
        public unsafe void WriteBytes(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException("data", "data can't be null.");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "offset can't be negative.");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "count can't be negative.");

            if (offset + count > data.Length)
                throw new ArgumentOutOfRangeException("count", "offset + count bigger than data.Length.");

            if (size < count)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, count > 1024 ? count : 1024);
                Next.WriteBytes(data, offset, count);
                return;
            }

            fixed (byte* bData = this.data)
            fixed (byte* pData = data)
                Buffer.MemoryCopy(pData + offset, bData + position, count, count);

            position += count;
            size -= count;
        }

        /// <summary>
        /// Fills length bytes memory with data.
        /// </summary>
        /// <param name="data">The byte to fill teh data.</param>
        /// <param name="length">The amount of bytes to fill.</param>
        public unsafe void Fill(byte data, int length)
        {
            if (length > size)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, length > 1024 ? length : 1024);
                Next.Fill(data, length);
                return;
            }

            size -= length;

            if (length > 80)
            {
                ulong oData = ((ulong)data << 56) | ((ulong)data << 48) | ((ulong)data << 40) | ((ulong)data << 32) | ((ulong)data << 24) | ((ulong)data << 16) | ((ulong)data << 8) | (ulong)data;

                while (length > 7)
                {
                    fixed (byte* bData = this.data)
                        *(ulong*)(bData + position) = oData;

                    length -= 8;
                    position += 8;
                }
            }

            while (length-- > 0)
                this.data[position++] = data;
        }

        /// <summary>
        /// Fills length bytes memory with random data.
        /// </summary>
        /// <param name="rng">The random number generator to use.</param>
        /// <param name="length">The amount of bytes to fill.</param>
        public unsafe void Fill(Random rng, int length)
        {
            if (length > size)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, length > 1024 ? length : 1024);
                Next.Fill(rng, length);
                return;
            }

            fixed (byte* bData = data)
            {
                for (; length >= 4; position += 4, size -= 4, length -= 4)
                    *(int*)(bData + position) = rng.Next(int.MinValue, int.MaxValue);

                for (; length >= 0; position++, size--, length--)
                    *(int*)(bData + position) = (byte)rng.Next(256);
            }
        }

        /// <summary>
        /// Fills length bytes memory with random data.
        /// </summary>
        /// <param name="rng">The random number generator to use.</param>
        /// <param name="length">The amount of bytes to fill.</param>
        public void Fill(RNGCryptoServiceProvider rng, int length)
        {
            if (length > size)
            {
                Next = new ManagedBinaryMemoryWriterSegment(writer, length > 1024 ? length : 1024);
                Next.Fill(rng, length);
                return;
            }

            rng.GetBytes(data, position, length);

            position += length;
            size -= length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToArray(byte[] data, ref int position)
        {
            if (this.position > 0)
            {
                Buffer.BlockCopy(this.data, 0, data, position, this.position);
                position += this.position;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long ToPointer(ref byte* ptr)
        {
            if (position > 0)
            {
                fixed (byte* pData = data)
                    Buffer.MemoryCopy(pData, ptr, position, position);

                ptr += position;
            }

            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToStream(Stream stream)
        {
            if (position > 0)
                stream.Write(data, 0, position);

            return position;
        }
    }
}