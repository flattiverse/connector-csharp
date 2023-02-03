using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    public enum UnitKind
    {
        Sun,
        Planet,
        Moon,
        /// <summary>
        /// TOG: Überall XML-Kommentare.
        /// </summary>
        Meteoroid,
        Comet,
        Asteroid,
        Buoy,
        MissionTarget,
        PlayerUnit,
        Shot,
        Explosion,
        BlackHole
    }
}
