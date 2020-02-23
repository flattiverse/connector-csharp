using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// This exception will be thrown, when the you try to login but are already connected.
    /// </summary>
    public class AlreadyConnectedException : InvalidOperationException
    {
        internal AlreadyConnectedException() : base ("This instance is already connected.")
        {
        }
    }
}
