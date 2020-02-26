using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown when you chose an illegal name.
    /// </summary>
    public class IllegalNameException : ArgumentException
    {
        internal IllegalNameException() : base("The given name is illegal.")
        {
        }
    }
}
