using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse
{
    internal class BlockManager
    {
        private readonly Connection connection;

        private object sync;

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
            lock (sync) 
            {
                Block block = new Block(this, generateId());
                blocks.Add(block.Id, block);
                return block;
            }
        }

        public void ConnectionClosed(Exception? ex)
        {
            lock (sync)
            {
                foreach (KeyValuePair<string,Block> blockKvP in blocks)
                    blockKvP.Value.Answer(null, null);

                blocks.Clear();
            }

            connection.UniverseGroup.ConnectionClose(ex);
        }

        public void Answer(string blockId, byte[] recv, JsonDocument response)
        {
            lock (sync)
            {
                if (!blocks.TryGetValue(blockId, out Block? block))
                    return;

                block.Answer(recv, response);
            }
        }

        public void Unblock(string id)
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
