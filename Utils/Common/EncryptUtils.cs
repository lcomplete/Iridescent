using System;
using System.Security.Cryptography;
using System.Text;

namespace Iridescent.Utils.Common
{
    public static class EncryptUtils
    {
        public static string Md5(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public static string GenerateSalt()
        {
            byte[] numArray = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(numArray);
            return Convert.ToBase64String(numArray);
        }

        /// <summary>
        /// 将密码进行sha1加密
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string EncodePassword(string pass, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] numArray1 = Convert.FromBase64String(salt);
            byte[] inArray;

            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");
            if (hashAlgorithm is KeyedHashAlgorithm)
            {
                KeyedHashAlgorithm keyedHashAlgorithm = (KeyedHashAlgorithm)hashAlgorithm;
                if (keyedHashAlgorithm.Key.Length == numArray1.Length)
                    keyedHashAlgorithm.Key = numArray1;
                else if (keyedHashAlgorithm.Key.Length < numArray1.Length)
                {
                    byte[] numArray2 = new byte[keyedHashAlgorithm.Key.Length];
                    Buffer.BlockCopy((Array)numArray1, 0, (Array)numArray2, 0, numArray2.Length);
                    keyedHashAlgorithm.Key = numArray2;
                }
                else
                {
                    byte[] numArray2 = new byte[keyedHashAlgorithm.Key.Length];
                    int dstOffset = 0;
                    while (dstOffset < numArray2.Length)
                    {
                        int count = Math.Min(numArray1.Length, numArray2.Length - dstOffset);
                        Buffer.BlockCopy((Array)numArray1, 0, (Array)numArray2, dstOffset, count);
                        dstOffset += count;
                    }
                    keyedHashAlgorithm.Key = numArray2;
                }
                inArray = keyedHashAlgorithm.ComputeHash(bytes);
            }
            else
            {
                byte[] buffer = new byte[numArray1.Length + bytes.Length];
                Buffer.BlockCopy((Array)numArray1, 0, (Array)buffer, 0, numArray1.Length);
                Buffer.BlockCopy((Array)bytes, 0, (Array)buffer, numArray1.Length, bytes.Length);
                inArray = hashAlgorithm.ComputeHash(buffer);
            }

            return Convert.ToBase64String(inArray);
        }

    }
}
