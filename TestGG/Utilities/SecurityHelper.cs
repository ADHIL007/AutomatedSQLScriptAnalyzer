using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SecurityHelper
{

    private static readonly string EncryptionKey = "5202SEIGOLONHCETTIJOEG";


    private static byte[] GetKeyBytes(string key)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] fullHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            byte[] keyBytes = new byte[16]; 
            Array.Copy(fullHash, keyBytes, 16);
            return keyBytes;
        }
    }

    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = GetKeyBytes(EncryptionKey);
        byte[] iv = new byte[16]; 

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string encryptedText)
    {
        byte[] keyBytes = GetKeyBytes(EncryptionKey);
        byte[] iv = new byte[16];

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}