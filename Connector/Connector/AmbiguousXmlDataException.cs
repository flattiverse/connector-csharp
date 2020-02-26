using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when you specify invalid XML data.
    /// </summary>
    public class AmbiguousXmlDataException : ArgumentException
    {
        internal AmbiguousXmlDataException() : base("The XML specification is invalid. Please check the syntax and/or content.")
        {
        }
    }
}
