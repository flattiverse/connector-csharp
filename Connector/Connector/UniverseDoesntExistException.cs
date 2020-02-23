using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Is thrown, when you try to access a universe, which doesn't exist.
    /// </summary>
    class UniverseDoesntExistException : ArgumentException
    {
        internal UniverseDoesntExistException() : base ("The specified universe doesn't exist.")
        {
        }
    }
}
