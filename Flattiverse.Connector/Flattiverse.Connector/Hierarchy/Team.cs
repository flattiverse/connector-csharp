using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class Team : INamedUnit
    {
        public readonly byte ID;
        private string name;

        private byte red;
        private byte green;
        private byte blue;

        internal Team(byte id, PacketReader reader)
        {
            ID = id;

            name = reader.ReadString();
            red = reader.ReadByte();
            green = reader.ReadByte();
            blue = reader.ReadByte();
        }

        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name => name;
        public byte Red => red;
        public byte Green => green;
        public byte Blue => blue;
    }
}
