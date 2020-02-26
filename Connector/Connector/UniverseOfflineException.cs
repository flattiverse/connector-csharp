using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Gets thrown, when the universe is offline but you try to do some kind of action with it.
    /// </summary>
    public class UniverseOfflineException : InvalidOperationException
    {
        internal UniverseOfflineException() : base("The universe is offline.")
        {
        }
    }
}
