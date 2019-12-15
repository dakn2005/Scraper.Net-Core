using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace StC.Classes
{
    public static class Secure
    {
        private static int saltLengthLimit = 24;

        public static byte[] GetSalt()
        {
            return GetSalt(saltLengthLimit);
        }

        private static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(salt);
            }

            return salt;
        }

        public static string Hasher(string password, byte[] salt)
        {
            byte[] passwordBytes = new byte[Encoding.UTF8.GetByteCount(password) + salt.Length];  // Create buffer for password bytes and hash
            int passwordLength = Encoding.UTF8.GetBytes(password, 0, password.Length, passwordBytes, 0);
            salt.CopyTo(passwordBytes, passwordLength);
            byte[] hash = null;
            using (SHA512Managed hasher = new SHA512Managed())
            {
                hash = hasher.ComputeHash(passwordBytes);
            }

            return System.Text.Encoding.Unicode.GetString(hash);
        }

    }

    public class Encryptor
    {
        public static byte[] genKey(string pwd, byte[] salt)
        {
            byte[] key;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                int KeyStrengthInBytes = aes.KeySize / 8;
                System.Security.Cryptography.Rfc2898DeriveBytes rfc2898 =
                    new System.Security.Cryptography.Rfc2898DeriveBytes(pwd, salt, 100);
                aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                //aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);

                key = aes.Key;
            }

            return key;
        }

        public static byte[] genIV(string pwd, byte[] salt)
        {
            byte[] iv;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                int KeyStrengthInBytes = aes.KeySize / 8;
                System.Security.Cryptography.Rfc2898DeriveBytes rfc2898 =
                    new System.Security.Cryptography.Rfc2898DeriveBytes(pwd, salt, 100);
                //aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);

                iv = aes.IV;
            }

            return iv;
        }

        // vector and key have to match between encryption and decryption
        public static string Encrypt(string text, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (text == null || text.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        // vector and key have to match between encryption and decryption
        public static string Decrypt(byte[] encrypted, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (encrypted == null || encrypted.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                //aesAlg.Padding = PaddingMode.PKCS7;
                //aesAlg.KeySize = 128;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                //var cipher = new byte[encrypted.Length - iv.Length];
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }



    }
}
