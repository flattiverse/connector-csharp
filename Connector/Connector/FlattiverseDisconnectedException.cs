using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// The flattiverse server has been disconnected.
    /// </summary>
    public class FlattiverseDisconnectedException : Exception
    {
        internal FlattiverseDisconnectedException() : base("Flattiverse proxy has been disconnected.")
        {
        }
    }
}
