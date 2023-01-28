using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Network
{
    class Query : IDisposable
    {
        private readonly object sync = new object();

        private byte[]? query;
        private MemoryStream ms;

        private byte[]? answer;

        private readonly Connection connection;

        private TaskCompletionSource tcs;
        private JsonDocument result;

        public readonly Utf8JsonWriter Writer;

        public Query(Connection connection, string command, string id)
        {
            this.connection = connection;

            query = ArrayPool<byte>.Shared.Rent(2048);

            ms = new MemoryStream(query, true);

            Writer = new Utf8JsonWriter(ms);

            Writer.WriteStartObject();
            Writer.WriteString("command", command);
            Writer.WriteString("id", id);
        }

        public void Write(string name, string value)
        {
            Writer.WriteString(name, value);
        }

        public void Write(string name, double value)
        {
            Writer.WriteNumber(name, value);
        }

        public async Task Send()
        {
            if (query == null)
                throw new InvalidOperationException("Can't send command twice.");

            tcs = new TaskCompletionSource();

            Writer.WriteEndObject();
            Writer.Dispose();

            await connection.Send(query, (int)ms.Position);

            ArrayPool<byte>.Shared.Return(query);
            query = null;
        }

        public void Answer(byte[] data, JsonDocument document)
        {
            answer = data;

            result = document;

            tcs.SetResult();
        }

        public void Answer(byte[] data, int error)
        {
            answer = data;
            tcs.SetException(new GameException(error));
        }

        public void Answer(int error)
        {
            tcs.SetException(new GameException(error));
        }

        public async Task<double> ReceiveDouble()
        {
            await tcs.Task.ConfigureAwait(false);

            double d;

            if (!Utils.Traverse(result.RootElement, out d, "result"))
                throw new GameException("Required result as double, but there either was no result or it wasn't parsable as double.");

            return d;
        }

        public async Task<int> ReceiveInteger()
        {
            await tcs.Task.ConfigureAwait(false);

            int i;

            if (!Utils.Traverse(result.RootElement, out i, "result"))
                throw new GameException("Required result as integer, but there either was no result or it wasn't parsable as integer.");

            return i;
        }

        public async Task<string> ReceiveString()
        {
            await tcs.Task.ConfigureAwait(false);

            string s;

            if (!Utils.Traverse(result.RootElement, out s, "result"))
                throw new GameException("Required result as string, but there either was no result or it wasn't parsable as string.");

            return s;
        }

        public async Task Wait()
        {
            await tcs.Task.ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (query != null)
            {
                ArrayPool<byte>.Shared.Return(query);
                query = null;
            }

            if (answer != null)
            {
                ArrayPool<byte>.Shared.Return(answer);
                answer = null;
            }
        }
    }
}
