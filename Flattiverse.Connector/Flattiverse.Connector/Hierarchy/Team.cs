using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Hierarchy
{
    class Team
    {
        public readonly byte ID;
        private string name;

        private byte red;
        private byte green;
        private byte blue;

        public Team(byte id, PacketReader reader)
        {
            ID = id;

            name = reader.ReadString();
            red = reader.ReadByte();
            green = reader.ReadByte();
            blue = reader.ReadByte();
        }

        public string Name => name;
        public byte Red => red;
        public byte Green => green;
        public byte Blue => blue;
    }
}
