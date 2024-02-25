using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class Team : INamedUnit
    {
        public readonly Galaxy Galaxy;

        private byte id;
        private TeamConfig config;
        private bool isActive;

        internal Team(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;

            config = new TeamConfig(reader);
        }

        public int ID => id;
        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name => config.Name;
        public TeamConfig Config => config;

        public bool IsActive => isActive;

        /// <summary>
        /// Sets given values in this team.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<TeamConfig> config)
        {
            TeamConfig changes = new TeamConfig(this.config);
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x48;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            await session.SendWait(packet);
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

            await session.SendWait(packet);
        }

        internal void Update(PacketReader reader)
        {
            config = new TeamConfig(reader);
        }


        internal void DynamicUpdate(PacketReader reader)
        {
            //TODO
            //config = new TeamConfig(reader);
        }

        internal void Deactivate()
        {
            isActive = false;
        }
    }
}
