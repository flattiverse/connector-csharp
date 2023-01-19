using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    public class UniverseGroup
    {
        private Connection connection;

        private Dictionary<short, Universe> universes;

        private object sync = new object();

        internal UniverseGroup(Connection connection) 
        {
            this.connection = connection;
            universes = new Dictionary<short, Universe>();
        }

        internal void addUniverse(short id)
        {
            lock (sync)
                universes.Add(0, new Universe(this, connection, id));
        }

        public bool TryGet(int id, out Universe universe)
        {
            if(id > 0 || id > short.MaxValue)
                throw new ArgumentOutOfRangeException($"Id must be between 0 and {short.MaxValue}.", nameof(id));

            return universes.TryGetValue((short)id, out universe);
        }

        public IEnumerable<Universe> EnumerateUniverses() 
        {
            Dictionary<short, Universe> localUniverses;

            lock (sync)
                localUniverses= new Dictionary<short, Universe>(universes);

            foreach (KeyValuePair<short, Universe> universeKvP in localUniverses)
                yield return universeKvP.Value;
        }


    }
}
