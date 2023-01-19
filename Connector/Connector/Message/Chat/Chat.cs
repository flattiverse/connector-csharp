using System.Text.Json;

namespace Flattiverse.Message.Chat
{
    public class Chat
    {
        private Connection connection;

        public delegate void ChatHandler(Message message);

        public event ChatHandler? MessageReceived;

        public Chat(Connection connection)
        {
            this.connection = connection;
        }

        public async Task Send(Message message)
        {
            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "message";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter paramType = new ClientCommandParameter("type");
                paramType.SetValue(message.type.ToString());
                parameters.Add(paramType);

                ClientCommandParameter paramUniverse = new ClientCommandParameter("message");
                paramUniverse.SetJsonValue(message.ToJson());
                parameters.Add(paramUniverse);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument? response = block.Response;

                if (!Connection.responseSuccess(response, out string error))
                    throw new Exception(error);
            }
        }

        internal bool handleMessage(JsonElement data)
        {
            Message message;
            try
            {
                message = Message.FromJson(data, connection);
            }
            catch (Exception ex)
            {
                connection.payloadExceptionSocketClose(new Exception($"Command can't be null or empty.")).Wait();
                return false;
            }

            try
            {
                if (MessageReceived != null)
                    MessageReceived(message);
            }
            catch
            {
            }

            return true;
        }
    }
}
