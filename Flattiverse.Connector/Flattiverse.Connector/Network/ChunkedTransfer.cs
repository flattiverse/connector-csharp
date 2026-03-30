namespace Flattiverse.Connector.Network;

internal delegate void ByteChunkRequestWriter(ref PacketWriter writer, int offset, ushort maximumLength);
internal delegate void CountChunkRequestWriter(ref PacketWriter writer, int offset, ushort maximumCount);
internal delegate bool CountChunkItemReader<T>(ref PacketReaderLarge reader, out T item) where T : class;

internal static class ChunkedTransfer
{
    internal const ushort AvatarChunkMaximumLength = 16384;
    internal const ushort AccountChunkMaximumCount = 8;
    internal const ushort EditableUnitChunkMaximumCount = 16;

    internal static async Task<byte[]> DownloadBytes(Connection connection, ByteChunkRequestWriter requestWriter, ProgressState? progressState,
        string description)
    {
        int offset = 0;
        byte[]? data = null;

        progressState?.Reset();

        while (true)
        {
            PacketReaderLarge reply = await connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
            {
                requestWriter(ref writer, offset, AvatarChunkMaximumLength);
            }).ConfigureAwait(false);

            if (!reply.Read(out int totalLength) ||
                !reply.Read(out int returnedOffset) ||
                !reply.Read(out ushort chunkLength))
                throw new InvalidDataException($"Couldn't read {description} chunk header.");

            if (returnedOffset != offset)
                throw new InvalidDataException($"Unexpected {description} chunk offset {returnedOffset}, expected {offset}.");

            if (data is null)
            {
                if (totalLength < 0)
                    throw new InvalidDataException($"Negative {description} length {totalLength}.");

                data = new byte[totalLength];
            }
            else if (totalLength != data.Length)
                throw new InvalidDataException($"Unexpected {description} total length {totalLength}, expected {data.Length}.");

            if (offset + chunkLength > data.Length)
                throw new InvalidDataException($"The received {description} chunk overruns the announced total length.");

            byte[] chunk = new byte[chunkLength];

            if (!reply.Read(chunk))
                throw new InvalidDataException($"Couldn't read {description} chunk payload.");

            Buffer.BlockCopy(chunk, 0, data, offset, chunkLength);

            offset += chunkLength;
            progressState?.Report(offset, data.Length);

            if (offset == data.Length)
                return data;

            if (chunkLength == 0)
                throw new InvalidDataException($"The server returned an empty {description} chunk before the transfer finished.");
        }
    }

    internal static async Task<T[]> DownloadItems<T>(Connection connection, CountChunkRequestWriter requestWriter, CountChunkItemReader<T> itemReader,
        ProgressState? progressState, ushort maximumCount, string description) where T : class
    {
        int offset = 0;
        T[]? items = null;

        progressState?.Reset();

        while (true)
        {
            PacketReaderLarge reply = await connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
            {
                requestWriter(ref writer, offset, maximumCount);
            }).ConfigureAwait(false);

            if (!reply.Read(out int totalCount) ||
                !reply.Read(out int returnedOffset) ||
                !reply.Read(out ushort chunkCount))
                throw new InvalidDataException($"Couldn't read {description} chunk header.");

            if (returnedOffset != offset)
                throw new InvalidDataException($"Unexpected {description} chunk offset {returnedOffset}, expected {offset}.");

            if (items is null)
            {
                if (totalCount < 0)
                    throw new InvalidDataException($"Negative {description} item count {totalCount}.");

                if (totalCount == 0)
                {
                    progressState?.Report(0, 0);
                    return Array.Empty<T>();
                }

                items = new T[totalCount];
            }
            else if (totalCount != items.Length)
                throw new InvalidDataException($"Unexpected {description} total count {totalCount}, expected {items.Length}.");

            if (offset + chunkCount > items.Length)
                throw new InvalidDataException($"The received {description} chunk overruns the announced total count.");

            for (int index = 0; index < chunkCount; index++)
            {
                if (!itemReader(ref reply, out T item))
                    throw new InvalidDataException($"Couldn't read {description} item.");

                items[offset + index] = item;
            }

            offset += chunkCount;
            progressState?.Report(offset, items.Length);

            if (offset == items.Length)
                return items;

            if (chunkCount == 0)
                throw new InvalidDataException($"The server returned an empty {description} chunk before the transfer finished.");
        }
    }
}
