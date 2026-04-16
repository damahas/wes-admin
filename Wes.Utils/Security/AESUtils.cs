using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Wes.Utils.Security
{
    public static class AESUtils
    {
        /// <summary>
        /// 默认密钥
        /// </summary>
        private const string PublicKey = "91e2445fe7143bacve48659f3172c04e";

        /// <summary>
        /// 默认向量
        /// </summary>
        private const string Iv = "qe24dt4y0qw8kezr";

        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="str">需要加密字符串</param>  
        /// <returns>加密后字符串</returns>  
        public static String Encrypt(string str)
        {
            return Encrypt(str, PublicKey, Iv);
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="str">需要解密字符串</param>  
        /// <returns>解密后字符串</returns>  
        public static String Decrypt(string str)
        {
            return Decrypt(str, PublicKey, Iv);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">32位密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string str, string key, string iv)
        {
            var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(
                new KeyParameter(Encoding.UTF8.GetBytes(key)),
                128, //128 = 16 * 8 => (tag size * 8)
                Encoding.UTF8.GetBytes(iv),
                null);
            gcmBlockCipher.Init(true, parameters);

            var data = Encoding.UTF8.GetBytes(str);
            var cipherData = new byte[gcmBlockCipher.GetOutputSize(data.Length)];

            var length = gcmBlockCipher.ProcessBytes(data, 0, data.Length, cipherData, 0);
            gcmBlockCipher.DoFinal(cipherData, length);
            return Convert.ToBase64String(cipherData);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">32位密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string str, string key, string iv)
        {
            var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(
                new KeyParameter(Encoding.UTF8.GetBytes(key)),
                128,  //128 = 16 * 8 => (tag size * 8)
                Encoding.UTF8.GetBytes(iv),
                null);
            gcmBlockCipher.Init(false, parameters);

            var data = Convert.FromBase64String(str);
            var plaintext = new byte[gcmBlockCipher.GetOutputSize(data.Length)];

            var length = gcmBlockCipher.ProcessBytes(data, 0, data.Length, plaintext, 0);
            gcmBlockCipher.DoFinal(plaintext, length);
            return Encoding.UTF8.GetString(plaintext);
            //var keyBytes = Encoding.UTF8.GetBytes(key);
            //var nonceBytes = Encoding.UTF8.GetBytes(iv);
            ////var associatedBytes = associatedData == null ? null : Encoding.UTF8.GetBytes(associatedData);

            //var encryptedBytes = Convert.FromBase64String(str);
            ////tag size is 16
            //var cipherBytes = encryptedBytes[..^16];
            //var tag = encryptedBytes[^16..];
            //var decryptedData = new byte[cipherBytes.Length];
            //using var cipher = new AesGcm(keyBytes);
            //cipher.Decrypt(nonceBytes, cipherBytes, tag, decryptedData, null);
            //return Encoding.UTF8.GetString(decryptedData);
        }


    }
}
