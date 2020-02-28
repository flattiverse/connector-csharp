using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when you call an operation which can only be served by a connected universe instance, but theres no instance connected.
    /// 
    /// Some commands will be multiplexed to different universes, like the CheckUnitXml-Command. However, if there is no universe availabe at all this exception get's thrown.
    /// </summary>
    public class NoUniverseAvailableException : InvalidOperationException
    {
        internal NoUniverseAvailableException() : base ("There is currently no universe available to handle your request.")
        {
        }
    }
}
