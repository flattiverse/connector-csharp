using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies the reason why your connection request has been refused.
    /// </summary>
    public enum RefuseReason
    {
        /// <summary>
        /// You have not been refused and the connection has been successfully established.
        /// </summary>
        NotRefused = 0,

        /// <summary>
        /// You are not allowed to connect, because you are already online.
        /// </summary>
        AlreadyOnline,

        /// <summary>
        /// You are not allowed to connect, because your ships are still lingering due to an unordered disconnect.
        /// </summary>
        Pending,

        /// <summary>
        /// You are not allowed to connect, because you didn't complete the optin cycle.
        /// </summary>
        OptIn,

        /// <summary>
        /// You are banned from using flattiverse. Please contact the administration via https://forums.flattiverse.com/.
        /// </summary>
        Banned,

        /// <summary>
        /// Sadly, this server is full. Try again, later.
        /// </summary>
        ServerFull
    }
}
