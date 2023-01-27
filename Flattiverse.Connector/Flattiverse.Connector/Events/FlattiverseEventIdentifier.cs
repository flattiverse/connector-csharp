using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    class FlattiverseEventIdentifier : Attribute
    {
        public readonly string Identifier;

        public FlattiverseEventIdentifier(string identifier)
        {
            Identifier = identifier;
        }
    }
}
