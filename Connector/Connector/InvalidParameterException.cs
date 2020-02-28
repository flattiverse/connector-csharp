using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when one or more of your parameters are invalid. This exception is usually used, when there is no special exception for your case.
    /// You most likely will only see this exception, when the connector you are using has some kind of bugs.
    /// </summary>
    public class InvalidParameterException : ArgumentException
    {
        internal InvalidParameterException() : base ("One or more of the parameters given are invalid.")
        {
        }
    }
}
