using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// Flattiverse event kinds.
    /// </summary>
    public enum FlattiverseEventKind
    {
        /// <summary>
        /// A hearbeat event.
        /// </summary>
        Heartbeat,
        /// <summary>
        /// A new unit event.
        /// </summary>
        NewUnit,
        /// <summary>
        /// A updated unit event.
        /// </summary>
        UpdatedUnit,
        /// <summary>
        /// A gone unit event. (When a unit leaves your scan horizon.)
        /// </summary>
        GoneUnit,
        /// <summary>
        /// A player joined event.
        /// </summary>
        PlayerJoined,
        /// <summary>
        /// A player parted event.
        /// </summary>
        PlayerParted
    }
}
