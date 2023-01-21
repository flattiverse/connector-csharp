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

        public async Task Set(string unitJson)
        {
            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "setUnit";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();
                ClientCommandParameter param = new ClientCommandParameter("data");

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

                parameters.Add(param);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument response = block.Response!;

                if(!Connection.responseSuccess(response, out string? error))
                    throw new Exception(error);
            }
        }

        public async Task Delete(string name)
        {
            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "deleteUnit";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter paramName = new ClientCommandParameter("name");
                paramName.SetValue(name);
                parameters.Add(paramName);

                ClientCommandParameter paramUniverse = new ClientCommandParameter("universe");
                paramUniverse.SetValue(ID);
                parameters.Add(paramUniverse);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument response = block.Response!;

                if (!Connection.responseSuccess(response, out string? error))
                    throw new Exception(error);
            }
        }


    }
}
