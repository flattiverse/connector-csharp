using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// You are in the wrong state of doing this. (eg. You are already viewing the galaxy, but try to "review" it.)
    /// </summary>
    public class WrongStateException : InvalidOperationException
    {
        internal WrongStateException() : base("You/your resource are/is in the wrong state.")
        {
        }
    }
}
