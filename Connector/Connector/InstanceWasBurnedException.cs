using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// This exception is simmilar to the ObjectDisposedException except that it doesn't implement IDisposable because it doesn't hold any system handles.
    /// A method called on an object which isn't "available" any more will throw this exception.
    /// </summary>
    public class InstanceWasBurnedException : InvalidOperationException
    {
        internal InstanceWasBurnedException(string message) : base(message)
        {
        }
    }
}
