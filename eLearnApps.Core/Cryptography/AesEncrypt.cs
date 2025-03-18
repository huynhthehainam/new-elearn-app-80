using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace eLearnApps.Core.Cryptography
{
    public static class AesEncrypt
    {
        private static byte[][] GetHashKeys(string key)
        {
            var result = new byte[2][];
            var enc = Encoding.UTF8;
            using (SHA256 sha2 = SHA256.Create())
            {
                var rawKey = enc.GetBytes(key);
                var rawIv = enc.GetBytes(key);
                var hashKey = sha2.ComputeHash(rawKey);
                var hashIv = sha2.ComputeHash(rawIv);
                Array.Resize(ref hashIv, 16);
                result[0] = hashKey;
                result[1] = hashIv;
            }
            return result;
        }

        public static string Encrypt(string plainText, string key = "")
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] encrypted;
            var aesKey = GetHashKeys(key ?? Constants.CipherKey);
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey[0];
                aesAlg.IV = aesKey[1];
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Base64UrlEncoder.Encode(encrypted);
        }

        public static string Decrypt(string cipherText, string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(cipherText))
                    return string.Empty;

                var byteEncrypted = Base64UrlEncoder.DecodeBytes(cipherText);
                string plaintext;
                var aesKey = GetHashKeys(key ?? Constants.CipherKey);
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = aesKey[0];
                    aesAlg.IV = aesKey[1];
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var msDecrypt = new MemoryStream(byteEncrypted))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
