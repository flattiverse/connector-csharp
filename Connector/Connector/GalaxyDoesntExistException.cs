using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when you requested an action on a galaxy which doesn't exist.
    /// </summary>
    public class GalaxyDoesntExistException : ArgumentException
    {
        internal GalaxyDoesntExistException() : base ("The specified galaxy doesn't exist.")
        {
        }
    }
}
