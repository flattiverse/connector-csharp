using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies the universe status.
    /// </summary>
    public enum UniverseStatus
    {
        /// <summary>
        /// The universe is online and available for you to play (if you have access).
        /// </summary>
        Online = 0,

        /// <summary>
        /// The universe is offline. :( You need to wait until its online again. Maybe it's down for
        /// maintenance (update of the universe container, etc.)
        /// </summary>
        Offline,

        /// <summary>
        /// The universe is doen for maintenance. Calm down and go into another universe until it is online
        /// again.
        /// </summary>
        Maintenance
    }
}
