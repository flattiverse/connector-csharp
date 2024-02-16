using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class Team : INamedUnit
    {
        public readonly Galaxy Galaxy;

        private byte id;
        private string name;
        private byte red;
        private byte green;
        private byte blue;

        internal Team(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;

            name = reader.ReadString();
            red = reader.ReadByte();
            green = reader.ReadByte();
            blue = reader.ReadByte();
        }

        public int ID => id;
        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name => name;
        public int Red => red;
        public int Green => green;
        public int Blue => blue;

        /// <summary>
        /// Sets given values in this team.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<TeamConfig> config)
        {
            TeamConfig changes = new TeamConfig(this);
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x48;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            packet = await session.SendWait(packet);

            if (GameException.Check(packet) is GameException ex)
                throw ex;
        }

        /// <summary>
        /// Removes this team.
        /// </summary>
        /// <returns></returns>
        public async Task Remove()
        {
            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x49;
            packet.Header.Param0 = id;

            packet = await session.SendWait(packet);

            if (GameException.Check(packet) is GameException ex)
                throw ex;
        }
    }
}
