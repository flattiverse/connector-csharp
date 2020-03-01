using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown when an internal server error in happens in the universe server while handling your request.
    /// </summary>
    public class UniverseServerUnhandledException : Exception
    {
        internal UniverseServerUnhandledException() : base("The universe server encountered an exception which has not been handled properly in the code. Additionally the proxy can't understand the exact details of this exception. Please forward this incident to info@flattiverse.com.")
        {
        }
    }
}
