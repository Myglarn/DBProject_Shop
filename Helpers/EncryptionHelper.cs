using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Helpers
{
    /// <summary>
    /// Clas containing methods for encrypting and decrypting
    /// </summary>
    public class EncryptionHelper
    {
        
        private const byte Key = 0x42;

        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);            
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }            
            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return encryptedText;
            }
            var bytes = Convert.FromBase64String(encryptedText);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }            
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
