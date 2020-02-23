using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when your connection attempt has been denied.
    /// </summary>
    public class ClientRefusedException : InvalidOperationException
    {
        internal ClientRefusedException(RefuseReason reason) : base (refuseReason(reason))
        {
        }

        private static string refuseReason(RefuseReason reason)
        {
            switch (reason)
            {
                case RefuseReason.AlreadyOnline:
                    return "You are not allowed to connect, because you are already online.";
                case RefuseReason.Pending:
                    return "You are not allowed to connect, because some of your ships are still lingering due to an unordered disconnect.";
                case RefuseReason.OptIn:
                    return "You are not allowed to connect, because you didn't complete the optin cycle.";
                case RefuseReason.Banned:
                    return "You are banned from using flattiverse. Please contact the administration via https://forums.flattiverse.com/.";
                case RefuseReason.ServerFull:
                    return "This server is full with clients. Try again later.";
                default:
                    return "Your connection has been refused due to an unknown refuse reason. Please update your connector.";
            }
        }
    }
}
