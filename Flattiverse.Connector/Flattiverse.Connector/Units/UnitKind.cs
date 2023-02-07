using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    public enum UnitKind
    {
        /// <summary>
        /// A sun, which may have a corona or corona sections.
        /// </summary>
        Sun,
        /// <summary>
        /// A planet.
        /// </summary>
        Planet,
        /// <summary>
        /// A moon.
        /// </summary>
        Moon,
        /// <summary>
        /// A meteoroid.
        /// </summary>
        Meteoroid,
        /// <summary>
        /// A comet.
        /// </summary>
        Comet,
        /// <summary>
        /// An asteroid.
        /// </summary>
        Asteroid,
        /// <summary>
        /// A buoy, which may contain a message.
        /// </summary>
        Buoy,
        /// <summary>
        /// A missiontarget, which you may have to shoot.
        /// </summary>
        MissionTarget,
        /// <summary>
        /// A playerunit. May be friendly. Or not.
        /// </summary>
        PlayerUnit,
        /// <summary>
        /// A shot. Better not touch.
        /// </summary>
        Shot,
        /// <summary>
        /// An explosion. Hope you are far away.
        /// </summary>
        Explosion,
        /// <summary>
        /// A black hole, which may have an gravitational well or gravitational well sections.
        /// </summary>
        BlackHole
    }
}
