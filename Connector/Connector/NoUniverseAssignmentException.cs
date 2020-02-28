using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when you try to do something, which requires a universe assignment.
    /// </summary>
    public class NoUniverseAssignmentException : InvalidOperationException
    {
        internal NoUniverseAssignmentException() : base("You need to be assigned (Joinned) to a universe.")
        {
        }
    }
}
