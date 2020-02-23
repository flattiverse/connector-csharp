using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Flattiverse.Utils
{
    /// <summary>
    /// Represents a managed binary writer which will also automatically manage it's memory.
    /// </summary>
    public class ManagedBinaryMemoryWriter
    {
        private readonly ManagedBinaryMemoryWriterSegment segment;
        private ManagedBinaryMemoryWriterSegment currentSegment;

        /// <summary>
        /// Instantiates a managed binary memory writer.
        /// </summary>
        public ManagedBinaryMemoryWriter()
        {
            segment = new ManagedBinaryMemoryWriterSegment(this, 64);
            currentSegment = segment;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MoveSegment(ManagedBinaryMemoryWriterSegment segment)
        {
            currentSegment = segment;
        }

        /// <summary>
        /// Writes a string in UTF-8 encoding with 7 bit encoded length prefix.
        /// </summary>
        /// <param name="text">The string to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string? text)
        {
            currentSegment.Write(text);
        }

        /// <summary>
        /// Writes a string in UTF-8 encoding.
        /// </summary>
        /// <param name="text">The string to write.</param>
        /// <returns>The number of bytes written.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVanillaString(string text)
        {
            return currentSegment.WriteVanillaString(text);
        }

        /// <summary>
        /// Writes a boolean.
        /// </summary>
        /// <param name="data">The byte to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">The byte to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a signed byte.
        /// </summary>
        /// <param name="data">The signed byte to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes an unsigned short.
        /// </summary>
        /// <param name="data">The unsigned short to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a short.
        /// </summary>
        /// <param name="data">The short to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a 3 byte integer.
        /// </summary>
        /// <param name="data">The integer to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt24(int data)
        {
            currentSegment.WriteUInt24(data);
        }

        /// <summary>
        /// Writes an unsigned int.
        /// </summary>
        /// <param name="data">The unsigned integer to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes an int.
        /// </summary>
        /// <param name="data">The integer to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes an unsigned long.
        /// </summary>
        /// <param name="data">The unsigned long to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a long.
        /// </summary>
        /// <param name="data">The long to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a char.
        /// </summary>
        /// <param name="data">The char to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a float.
        /// </summary>
        /// <param name="data">The float to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(float data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="data">The double to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(double data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a timespan.
        /// </summary>
        /// <param name="data">The timespan to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(TimeSpan data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a datetime.
        /// </summary>
        /// <param name="data">The datetime to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(DateTime data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes a decimal.
        /// </summary>
        /// <param name="data">The decimal to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(decimal data)
        {
            currentSegment.Write(data);
        }

        /// <summary>
        /// Writes count bytes from the current position into data starting at offset.
        /// </summary>
        /// <param name="data">The byte array where data will be read from.</param>
        /// <param name="offset">The position in the byte array where those data will be read from.</param>
        /// <param name="count">The amount of bytes which will be read.</param>
        /// <remarks>BEWARE: This method is also NOT DOING input checks of the given parameters.</remarks>
        public void WriteBytes(byte[] data, int offset, int count)
        {
            currentSegment.WriteBytes(data, offset, count);
        }

        /// <summary>
        /// Fills length bytes memory with data.
        /// </summary>
        /// <param name="data">The byte to fill teh data.</param>
        /// <param name="length">The amount of bytes to fill.</param>
        public void Fill(byte data, int length)
        {
            currentSegment.Fill(data, length);
        }

        /// <summary>
        /// Fills length bytes memory with random data.
        /// </summary>
        /// <param name="rng">The random number generator to use.</param>
        /// <param name="length">The amount of bytes to fill.</param>
        public void Fill(Random rng, int length)
        {
            currentSegment.Fill(rng, length);
        }

        /// <summary>
        /// Fills length bytes memory with random data.
        /// </summary>
        /// <param name="rng">The random number generator to use.</param>
        /// <param name="length">The amount of bytes to fill.</param>
        public void Fill(RNGCryptoServiceProvider rng, int length)
        {
            currentSegment.Fill(rng, length);
        }

        /// <summary>
        /// Generates a new array with the contents of this writer. Beware, the content shouldn't exceed .NET memory array limit of nearly 2 GiB.
        /// </summary>
        /// <returns>The array.</returns>
        public byte[] ToArray()
        {
            long size = 0;

            ManagedBinaryMemoryWriterSegment segment = this.segment;

            while (segment != null)
            {
                size += segment.Length;

                segment = segment.Next;
            }

            if (size > 2147000000)
                throw new OutOfMemoryException("Array buffers can't exceed 2147000000 bytes. Write to a stream, if you generated bigger data than the near 2 GiB limit.");

            byte[] data = new byte[size];

            int position = 0;

            segment = this.segment;

            while (segment != null)
            {
                segment.ToArray(data, ref position);

                segment = segment.Next;
            }

            return data;
        }

        /// <summary>
        /// Saves writer content to this stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <returns>The bytes written.</returns>
        public long ToStream(Stream stream)
        {
            long size = 0;

            ManagedBinaryMemoryWriterSegment segment = this.segment;

            while (segment != null)
            {
                size += segment.ToStream(stream);

                segment = segment.Next;
            }

            return size;
        }

        /// <summary>
        /// Saves writer content to the given byte[] and it's offset.
        /// </summary>
        /// <param name="data">The byte[].</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public int ToArray(byte[] data, int offset)
        {
            long size = 0;

            ManagedBinaryMemoryWriterSegment segment = this.segment;

            while (segment != null)
            {
                size += segment.Length;

                segment = segment.Next;
            }

            if (data.Length < size + offset)
                throw new ArgumentException("Not enough memory in data.", "data");

            int position = offset;

            segment = this.segment;

            while (segment != null)
            {
                segment.ToArray(data, ref position);

                segment = segment.Next;
            }

            return (int)size;
        }

        /// <summary>
        /// Saves writer content to the given pointer.
        /// </summary>
        /// <param name="ptr">The Pointer we write to.</param>
        /// <returns>The amount of bytes written.</returns>
        public unsafe long ToPointer(byte* ptr)
        {
            long size = 0;
            ManagedBinaryMemoryWriterSegment segment = this.segment;

            while (segment != null)
            {
                size += segment.ToPointer(ref ptr);

                segment = segment.Next;
            }

            return (int)size;
        }

        /// <summary>
        /// Returns the size of the data in the writer.
        /// </summary>
        public long Size
        {
            get
            {
                long size = 0;

                ManagedBinaryMemoryWriterSegment segment = this.segment;

                while (segment != null)
                {
                    size += segment.Length;

                    segment = segment.Next;
                }

                return size;
            }
        }
    }
}