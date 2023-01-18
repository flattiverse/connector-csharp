namespace Flattiverse
{
    public class Units
    {
        private Connection connection;

        internal Units(Connection connection)
        {
            this.connection = connection;
        }   

        public async Task Create()
        {
            using (Block block = connection.blockManager.GetBlock())
            {

                Packet packet = new Packet(block.Id);
                packet.Command = "CreateUnit";

                CommandParameter param = new CommandParameter("Unit");
                param.SetJsonValue("{ \"Test\":\"text\", \"Test2\":\"text2\" }");

                packet.Parameters.Add(param);
                
                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;

                //ResponsePacket lesen
            }

        }

        public async Task Delete()
        {
            using (Block block = connection.blockManager.GetBlock())
            {

                Packet packet = new Packet(block.Id);
                packet.Command = "DeleteUnit";

                //CommandParameter param = new CommandParameter("Unit");
                //param.SetJsonValue("Hier muss die create json rein");

                //packet.Parameters.Add(param);

                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;

                //ResponsePacket lesen
            }
        }

        public async Task Change()
        {
            using (Block block = connection.blockManager.GetBlock())
            {

                Packet packet = new Packet(block.Id);
                packet.Command = "ChangeUnit";

                //CommandParameter param = new CommandParameter("Unit");
                //param.SetJsonValue("Hier muss die create json rein");

                //packet.Parameters.Add(param);

                await connection.SendCommand(packet);

                await block.Wait();

                Packet? responsePacket = block.Packet;

                //ResponsePacket lesen
            }
        }


    }
}
