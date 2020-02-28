using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, if your action would lead to the situation, that too many entries are in some kind of list.
    /// </summary>
    public class TooManyEntriesException : InvalidOperationException
    {
        internal TooManyEntriesException() : base("Your action would lead to too many entries.")
        {
        }
    }
}
