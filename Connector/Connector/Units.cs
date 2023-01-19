using System;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Flattiverse
{
    public class Units
    {
        private Connection connection;

        internal Units(Connection connection)
        {
            this.connection = connection;
        }   

        public async Task Create(int universeGroup, int universe, string name, UnitKind unitKind, int xPos, int yPos, int radius, int gravity, int corona)
        {
            using (Block block = connection.blockManager.GetBlock())
            {

                Packet packet = new Packet(block.Id);
                packet.Command = "CreateUnit";

                CommandParameter param = new CommandParameter("data");

                JsonWriterOptions options = new JsonWriterOptions();
                options.Indented = false;

                string data;

                using (MemoryStream ms = new MemoryStream())
                using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, options))
                {
                    writer.WriteStartObject();

                    writer.WriteNumber("universegroup", universeGroup);
                    writer.WriteNumber("universe", universe);
                    writer.WriteString("name", name);
                    writer.WriteString("kind", unitKind.ToString());

                    writer.WritePropertyName("position");

                    writer.WriteStartObject();

                    writer.WriteNumber("x", xPos);
                    writer.WriteNumber("y", yPos);

                    writer.WriteEndObject();

                    writer.WriteNumber("radius", radius);
                    writer.WriteNumber("gravity", gravity);
                    writer.WriteNumber("corona", corona);

                    writer.WriteEndObject();

                    writer.Flush();
                    ms.Position = 0;

                    data = new StreamReader(ms).ReadToEnd();
                }

                param.SetJsonValue(data);

                packet.Parameters.Add(param);
                
                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;

                //ResponsePacket lesen
            }

        }

        public async Task Update(int universeGroup, int universe, string name, UnitKind unitKind, int xPos, int yPos, int radius, int gravity, int corona)
        {
            using (Block block = connection.blockManager.GetBlock())
            {

                Packet packet = new Packet(block.Id);
                packet.Command = "UpdateUnit";

                CommandParameter param = new CommandParameter("data");

                JsonWriterOptions options = new JsonWriterOptions();
                options.Indented = false;

                string data;

                using (MemoryStream ms = new MemoryStream())
                using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, options))
                {
                    writer.WriteStartObject();

                    writer.WriteNumber("universegroup", universeGroup);
                    writer.WriteNumber("universe", universe);
                    writer.WriteString("name", name);
                    writer.WriteString("kind", unitKind.ToString());

                    writer.WritePropertyName("position");

                    writer.WriteStartObject();

                    writer.WriteNumber("x", xPos);
                    writer.WriteNumber("y", yPos);

                    writer.WriteEndObject();

                    writer.WriteNumber("radius", radius);
                    writer.WriteNumber("gravity", gravity);
                    writer.WriteNumber("corona", corona);

                    writer.WriteEndObject();

                    writer.Flush();
                    ms.Position = 0;

                    data = new StreamReader(ms).ReadToEnd();
                }

                param.SetJsonValue(data);

                packet.Parameters.Add(param);

                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;
            }
        }

        public async Task Delete()
        {
            using (Block block = connection.blockManager.GetBlock())
            {

                Packet packet = new Packet(block.Id);
                packet.Command = "DeleteUnit";

                CommandParameter param = new CommandParameter("data");
                param.SetJsonValue(@"{""universegroup"":0,""universe"":0,""name"":""test"",""kind"":""Sun"",""position"":{""x"":20,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");

                packet.Parameters.Add(param);

                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;

                //ResponsePacket lesen
            }
        }


    }
}
