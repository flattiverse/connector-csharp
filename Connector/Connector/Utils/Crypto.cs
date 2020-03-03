using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Flattiverse.Utils
{
    /// <summary>
    /// Crypto helper functions.
    /// </summary>
    public static class Crypto
    {
        /// <summary>
        /// Hashes a salt (usually the username) and a password to the key flattiverse uses to encrypt the connection.
        /// 
        /// This special hash function has been implemented to have the SAME implementation on any platform, because AES is MANDATORY
        /// for the implementation of the connector. However other fancy functions like scrypt, bcrypt or Rfc2898 may not be implemented
        /// in the framework of your programming language.
        /// 
        /// Don't use this method for your own crypto, because it may not fit your needs and may be a false friend.
        /// </summary>
        /// <param name="salt">The username.</param>
        /// <param name="password">The password used.</param>
        /// <returns>The key.</returns>
        public static unsafe byte[] HashPassword(string salt, string password)
        {
            SHA512Managed sha = new SHA512Managed();

            byte[] sData = sha.ComputeHash(Encoding.UTF8.GetBytes(salt.ToLower()));
            byte[] pData = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            byte[] slowness = new byte[80000000];

            fixed (byte* bSlowness = slowness)
            fixed (byte* bSalt = sData)
            fixed (byte* bPassword = pData)
            {
                long* pSlowness = (long*)bSlowness;

                for (int position = 0; position < 80000000; position += 80)
                {
                    *pSlowness++ = *(long*)(bPassword + 48);

                    *pSlowness++ = *(long*)bSalt;
                    *pSlowness++ = *(long*)(bSalt + 8);
                    *pSlowness++ = *(long*)(bSalt + 16);
                    *pSlowness++ = *(long*)(bSalt + 24);
                    *pSlowness++ = *(long*)(bSalt + 32);
                    *pSlowness++ = *(long*)(bSalt + 40);
                    *pSlowness++ = *(long*)(bSalt + 48);
                    *pSlowness++ = *(long*)(bSalt + 56);

                    *pSlowness++ = *(long*)(bPassword + 56);
                }
            }

            AesManaged aes = new AesManaged();

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            Buffer.BlockCopy(pData, 0, key, 0, 32);
            Buffer.BlockCopy(sData, 32, iv, 0, 16);

            using (ICryptoTransform crypt = aes.CreateEncryptor(key, iv))
            {
                for (int amount = 0; amount < 7; amount++)
                    crypt.TransformBlock(slowness, 0, 80000000, slowness, 0);

                crypt.TransformBlock(slowness, 0, 16, slowness, 0);
            }

            Buffer.BlockCopy(slowness, 0, iv, 0, 16);

            return iv;
        }
    }
}
