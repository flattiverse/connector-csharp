using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Is thrown, when your request has been denied because of missing privileges.
    /// </summary>
    public class PermissionDeniedException : UnauthorizedAccessException
    {
        internal PermissionDeniedException(byte privileges) : base (formulate((Privileges)privileges))
        {
        }

        private static string formulate(Privileges privileges)
        {
            List<Privileges> requiredPrivileges = new List<Privileges>();

            foreach (Privileges p in Enum.GetValues(typeof(Privileges)))
                if (p != 0 && (privileges & p) == p)
                    requiredPrivileges.Add(p);

            if (requiredPrivileges.Count == 0)
                return "Access denied. However, it seems like you don't need any privileges for what you tried to do?!";
            else if (requiredPrivileges.Count == 1)
                return $"Access denied. You require the \"{privileges}\" privilege.";

            StringBuilder builder = new StringBuilder();

            builder.Append("Access denied. You require the following privileges for this call: \"");

            for (int position = 0; position < requiredPrivileges.Count - 1; position++)
            {
                if (position > 0)
                    builder.Append(", \"");

                builder.Append(requiredPrivileges[position]);

                builder.Append("\"");
            }

            builder.Append(" and \"");

            builder.Append(requiredPrivileges[requiredPrivileges.Count - 1]);

            builder.Append("\".");

            return builder.ToString();
        }
    }
}
