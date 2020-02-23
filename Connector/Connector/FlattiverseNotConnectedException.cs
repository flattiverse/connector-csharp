using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// You are currently not connected.
    /// </summary>
    public class FlattiverseNotConnectedException : Exception
    {
        internal FlattiverseNotConnectedException() : base("You are not connected to flattiverse. Either you got disconnected or didn't login until now.")
        {
        }
    }
}
