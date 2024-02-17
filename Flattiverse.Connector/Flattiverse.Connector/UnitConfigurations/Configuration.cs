using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class Configuration
    {
        private string name;

        protected internal Configuration() { }

        internal Configuration(PacketReader reader)
        {
            name = reader.ReadString();
        }

        internal Packet ToPacket()
        {
            Packet packet = new Packet();

            using (PacketWriter writer = packet.Write())
                Write(writer);

            packet.Header.Param0 = (byte)Kind;

            return packet;
        }

        internal static Configuration FromPacket(Packet packet)
        {
            switch ((UnitKind)packet.Header.Param0)
            {
                case UnitKind.Sun:
                    return new SunConfiguration(packet.Read());
            }

            throw new GameException(0x33);
        }

        internal virtual void Write(PacketWriter writer)
        {
            writer.Write(name);
        }

        public virtual UnitKind Kind => UnitKind.Sun;

        /// <summary>
        /// The name of the configured unit.
        /// </summary>
        /// <exception cref="GameException">0x32 may be thrown, if the name violates rules.</exception>
        public string Name
        {
            get => name;
            set
            {
                if (!Utils.CheckName(value))
                    throw new GameException(0x31);

                name = value;
            }
        }

        internal static Configuration Default => new Configuration();
    }
}
