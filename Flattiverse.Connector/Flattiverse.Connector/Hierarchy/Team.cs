using Flattiverse.Connector.Network;
using System;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// For game mods that require teams, this class contains the configuration parameters of a team.
    /// </summary>
    public class Team : INamedUnit
    {
        /// <summary>
        /// The galaxy this team belongs to.
        /// </summary>
        public readonly Galaxy Galaxy;

        private byte id;
        private TeamConfig config;
        private bool isActive;

        internal Team(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;
            isActive = true;

            config = new TeamConfig(reader);
        }

        /// <summary>
        /// The ID of the team. This is unique in the galaxy.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name => config.Name;

        /// <summary>
        /// The configuration of the team.
        /// It contains the actual values of the team.
        /// </summary>
        public TeamConfig Config => config;

        /// <summary>
        /// This flag indicates if the team is still part of the active simulation.
        /// </summary>
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
            packet.Header.Id0 = id;

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
            packet.Header.Id0 = id;

            await session.SendWait(packet);
        }

        internal void Update(PacketReader reader)
        {
            config = new TeamConfig(reader);
        }

        /// <summary>
        /// Sends a chat message to the team.
        /// </summary>
        /// <param name="message">A message with a maximum of 512 chars.</param>
        /// <exception cref="GameException"></exception>
        public async Task Chat(string message)
        {
            if (!Utils.CheckMessage(message))
                throw new GameException(0x31);
            
            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x21;
            packet.Header.Id0 = id;

            packet.Header.Size = (ushort)System.Text.Encoding.UTF8.GetBytes(message.AsSpan(), packet.Payload.AsSpan(8, 1024));

            await session.SendWait(packet);
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
