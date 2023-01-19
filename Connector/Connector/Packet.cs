using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Flattiverse
{
    internal class Packet
    {
        public string BlockId;

        public string? Command;

        public readonly List<CommandParameter> Parameters;

        public readonly JsonDocument? document;

        public Packet(string blockID)
        {
            BlockId = blockID;
            Parameters = new List<CommandParameter>();
        }

        public Packet(string blockID, JsonDocument document)
        {
            BlockId = blockID;
            this.document = document;
            Parameters = new List<CommandParameter>();
        }

        public byte[] Compile()
        {
            JsonWriterOptions options= new JsonWriterOptions();
            options.Indented = false;

            byte[] data;

            using (MemoryStream ms = new MemoryStream())
            using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, options))
            {
                writer.WriteStartObject();

                writer.WriteString("command", Command);
                writer.WriteString("id", BlockId);

                foreach (CommandParameter cp in Parameters)
                    cp.WriteJson(writer);

                writer.WriteEndObject();

                writer.Flush();
                data = ms.ToArray();
            }

            return data;
        }
    }
}
