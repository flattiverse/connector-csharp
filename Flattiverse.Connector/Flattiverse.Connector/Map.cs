using Flattiverse.Connector.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    internal class Map
    {
        // TODO: MALUK Durch die unithaltende Map ersetzen

        internal bool TryGet(string name, [NotNullWhen(true)] out Unit? unit)
        {
            unit = null;
            return false;
        }
    }
}
