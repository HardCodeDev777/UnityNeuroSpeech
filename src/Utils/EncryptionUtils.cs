using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UnityNeuroSpeech.Utils
{
    internal static class EncryptionUtils
    {
        private static byte[] _encryptedData;

        public static string Encrypt(string dataToEncrypt, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16];

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs)) 
                        { 
                            sw.Write(dataToEncrypt); 
                        }
                    }
                    _encryptedData = ms.ToArray();
                    return Convert.ToBase64String(_encryptedData);
                }
            }
        }

        public static string Decrypt(string dataToDecrypt, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16];

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(Convert.FromBase64String(dataToDecrypt)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
