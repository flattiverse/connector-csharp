using Flattiverse.Connector.UnitConfigurations;
using Flattiverse.Connector.Units.SubComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    public class Sun : CelestialBody
    {
        public readonly ReadOnlyCollection<SunSection> Sections;

        public Sun(SunConfiguration configuration) : base(configuration)
        {
            Sections = configuration.Sections;
        }
    }
}
