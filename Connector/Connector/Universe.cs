using System;
using System.Text.Json;
using System.Xml.Linq;

namespace Flattiverse
{
    public class Universe
    {
        private Connection connection;

        private UniverseGroup universeGroup;

        public readonly short ID;


        internal Universe(UniverseGroup universeGroup, Connection connection, short ID) 
        { 
            this.universeGroup = universeGroup;
            this.connection = connection;
            this.ID = ID;
        }

        public async Task Create(string unitJson)
        {
            await createUpdateUnit("CreateUnit", unitJson);
        }

        public async Task Update(string unitJson)
        {
            await createUpdateUnit("UpdateUnit", unitJson);
        }

        private async Task createUpdateUnit(string command, string unitJson)
        {
            using (Block block = connection.blockManager.GetBlock())
            {
                Packet packet = new Packet(block.Id);
                packet.Command = command;

                CommandParameter param = new CommandParameter("data");

                try
                {
                    JsonDocument.Parse(unitJson, Connection.jsonDocOptions);
                }
                catch
                {
                    throw new ArgumentException($"Json format invalid.", nameof(unitJson));
                }

                string data;

                using (MemoryStream ms = new MemoryStream())               
                {
                    using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, Connection.jsonOptions))
                    {
                        writer.WritePropertyName("data");

                        writer.WriteStartObject();

                        writer.WriteNumber("universe", ID);

                        writer.WritePropertyName("unit");

                        writer.WriteRawValue(unitJson);

                        writer.WriteEndObject();         
                    }

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

        public async Task Delete(string name)
        {
            using (Block block = connection.blockManager.GetBlock())
            {
                Packet packet = new Packet(block.Id);
                packet.Command = "DeleteUnit";

                CommandParameter param = new CommandParameter("name");
                param.SetValue(name);

                packet.Parameters.Add(param);

                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;

                //ResponsePacket lesen
            }
        }


    }
}
