using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Wes.Utils.Security
{
    public static class MD5Utils
    {
        public static string Encrypt(string input)
        {
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] byteArr = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Encoding.UTF8.GetString(byteArr);

        }

        public static string EncryptX32(string input)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                int length = data.Length;
                for (int i = 0; i < length; i++)
                    sb.Append(data[i].ToString("X2"));

            }
            return sb.ToString();
        }

        //public static string EncryptX16(string input)
        //{
        //    MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        //    byte[] byteArr = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(input));
        //    return Encoding.UTF8.GetString(byteArr).Substring(8, 16);
        //}
    }
}
