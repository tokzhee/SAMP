using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace App.Core.Utilities
{
    public static class EncryptionUtility
    {
        private static byte[] Encrypt(byte[] input, string password)
        {
            try
            {
                AesCryptoServiceProvider service = new AesCryptoServiceProvider();
                // Use PBKDF2 for key and IV derivation
                byte[] salt = Encoding.UTF8.GetBytes("App.Core.Utilities.EncryptionUtility.Salt"); // Fixed salt for compatibility
                using (var keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    byte[] key = keyDerivation.GetBytes(service.KeySize / 8);
                    byte[] iv = keyDerivation.GetBytes(service.BlockSize / 8);
                    return Transform(input, service.CreateEncryptor(key, iv));
                }
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }
        private static byte[] Decrypt(byte[] input, string password)
        {
            try
            {
                AesCryptoServiceProvider service = new AesCryptoServiceProvider();
                // Use PBKDF2 for key and IV derivation
                byte[] salt = Encoding.UTF8.GetBytes("App.Core.Utilities.EncryptionUtility.Salt"); // Fixed salt for compatibility
                using (var keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    byte[] key = keyDerivation.GetBytes(service.KeySize / 8);
                    byte[] iv = keyDerivation.GetBytes(service.BlockSize / 8);
                    return Transform(input, service.CreateDecryptor(key, iv));
                }
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }
        public static string Encrypt(string text, string password)
        {
            byte[] input = Encoding.UTF8.GetBytes(text);
            byte[] output = Encrypt(input, password);
            return Convert.ToBase64String(output);
        }
        public static string Decrypt(string text, string password)
        {
            byte[] input = Convert.FromBase64String(text);
            byte[] output = Decrypt(input, password);
            return Encoding.UTF8.GetString(output);
        }
        private static byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
        {
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);

            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();

            memStream.Position = 0;
            byte[] result = new byte[Convert.ToInt32(memStream.Length)];
            memStream.Read(result, 0, Convert.ToInt32(result.Length));

            memStream.Close();
            cryptStream.Close();

            return result;
        }
    }
}
