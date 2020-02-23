using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A flattiverse account. This account doesn't needs to be online necessarily.
    /// </summary>
    public class Account
    {
        private readonly uint ID;

        /// <summary>
        /// The name of the account.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The status of the account.
        /// </summary>
        public readonly AccountStatus Status;

        /// <summary>
        /// Total kills of the account.
        /// </summary>
        public readonly uint Kills;

        /// <summary>
        /// Total deaths of this account.
        /// </summary>
        public readonly uint Deaths;

        /// <summary>
        /// The eMail of the account. This will be null, if you don't have administrative access.
        /// </summary>
        public readonly string? EMail;

        /// <summary>
        /// The new eMail of the account which will be setup after this account reoppedin. This will be null, if you don't have administrative access or no reoptin is in progress.
        /// </summary>
        public readonly string? NewEMail;

        internal Account(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            ID = reader.ReadUInt32();
            Name = reader.ReadStringNonNull();
            Status = (AccountStatus)reader.ReadByte();
            Kills = reader.ReadUInt32();
            Deaths = reader.ReadUInt32();
            EMail = reader.ReadString();
            NewEMail = reader.ReadString();
        }

        /// <summary>
        /// Checks if the given string is a valid account name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>true, if the name is valid. false otherwise.</returns>
        public static bool CheckName(string? name)
        {
            if (name == null || name.Length <= 2 || name.Length > 32)
                return false;

            if (name.StartsWith(" ") || name.EndsWith(" ") || name.StartsWith(".") || name.EndsWith(".") || name.StartsWith("-") || name.EndsWith("-") || name.StartsWith("_") || name.EndsWith("_"))
                return false;

            foreach (char c in name)
            {
                if (c >= 'a' && c <= 'z')
                    continue;

                if (c >= 'A' && c <= 'Z')
                    continue;

                if (c >= '0' && c <= '9')
                    continue;

                if (c >= 192 && c <= 214)
                    continue;

                if (c >= 216 && c <= 246)
                    continue;

                if (c >= 248 && c <= 687)
                    continue;

                if (c == ' ' || c == '.' || c == '_' || c == '-')
                    continue;

                return false;
            }

            return true;
        }
    }
}
