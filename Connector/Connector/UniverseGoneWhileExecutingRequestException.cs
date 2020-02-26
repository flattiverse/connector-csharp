using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// This exception gets thrown, when the universe server gets disconnected from the proxy while you are executing a command.
    /// </summary>
    public class UniverseGoneWhileExecutingRequestException : InvalidOperationException
    {
        internal UniverseGoneWhileExecutingRequestException() : base ("The universe disconnected from the proxy while executing your command.")
        {
        }
    }
}
