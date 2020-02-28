using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown when you query an account which doesn't exist or which can't be found.
    /// </summary>
    public class AccountDoesntExistException : ArgumentException
    {
        internal AccountDoesntExistException(string name) : base($"Account \"{name}\" doesn't exist or has already been deleted.")
        {
        }

        internal AccountDoesntExistException() : base($"Account doesn't exist or has already been deleted.")
        {
        }
    }
}
