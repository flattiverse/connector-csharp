using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when you want to do something, but aren't an admin.
    /// </summary>
    public class OperationRequiresAdminStatusException : InvalidOperationException
    {
        internal OperationRequiresAdminStatusException() : base("You need to be an administrator to execute this operation.")
        {
        }
    }
}
