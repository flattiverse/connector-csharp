using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies an account status.
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// You (or the account you are watching) are/is banned from using flattiverse.
        /// </summary>
        Banned = 0,
        /// <summary>
        /// This account need to complete the opt-in sequence.
        /// </summary>
        OptIn,
        /// <summary>
        /// This player is a normal player and opped in.
        /// </summary>
        Normal,
        /// <summary>
        /// This player is opped in with its current email address but the new one needs to be confirmed.
        /// </summary>
        Reoptin,
        /// <summary>
        /// The player is an admin.
        /// </summary>
        Admin
    }
}
