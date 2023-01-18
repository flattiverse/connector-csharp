using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    internal class BlockManager
    {
        private readonly Connection connection;

        private object sync;

        private int blockLimit = 256;

        private Dictionary<string, Block> blocks = new Dictionary<string, Block>();

        private const string idSampleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        Random rnd;

        public BlockManager(Connection connection)
        {
            this.connection = connection;
            sync = new object();
            rnd = new Random();
        }

        public Block GetBlock()
        {
            if(blocks.Count >= blockLimit)
                throw new Exception("No free Slot in current Communication Channel.");

            lock (sync) 
            {
                if (blocks.Count >= blockLimit)
                    throw new Exception("No free Slot in current Communication Channel.");

                Block block = new Block(this, generateId());
                blocks.Add(block.Id, block);
                return block;
            }
        }

        public void Answer(Packet packet)
        {
            lock (sync)
            {
                if (!blocks.TryGetValue(packet.BlockId, out Block block))
                    return;

                block.Answer(packet);
            }

        }

        public void Unblock(string? id)
        {
            lock (sync)
                blocks.Remove(id);
        }

        private string generateId()
        {
            string id;
            char[] idChars = new char[3];

            while (true)
            {
                for (int i = 0; i < idChars.Length; i++)
                    idChars[i] = idSampleChars[rnd.Next(idSampleChars.Length)];

                id = new string(idChars);

                if (!blocks.ContainsKey(id))
                    break;
            }

            return id;
        }
    }
}
