using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

public static class AESHelper
{
    // Convert BigInteger to a byte array of a specified size
    private static byte[] BigIntegerToBytes(BigInteger bigInt, int size)
    {
        byte[] fullArray = bigInt.ToByteArray();
        if (fullArray.Length == size)
        {
            return fullArray;
        }

        byte[] result = new byte[size];
        Array.Copy(fullArray, result, Math.Min(fullArray.Length, size));
        return result;
    }

    // Encrypt a string using AES encryption.
    public static string Encrypt(string text, BigInteger keyBigInt, int ivInt)
    {
        byte[] encrypted;

        // Convert BigInteger and int to byte arrays
        byte[] key = BigIntegerToBytes(keyBigInt, 16); // 128 bits for AES key
        byte[] IV = BitConverter.GetBytes(ivInt); // Convert int to byte array
        if (IV.Length < 16)
        {
            Array.Resize(ref IV, 16); // Ensure the IV is 16 bytes
        }

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = IV;

            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                }
                encrypted = msEncrypt.ToArray();
            }
        }

        return Convert.ToBase64String(encrypted);
    }

    // Decrypt a string using AES encryption.
    public static string Decrypt(string cipherText, BigInteger keyBigInt, int ivInt)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);

        // Convert BigInteger and int to byte arrays
        byte[] key = BigIntegerToBytes(keyBigInt, 16); // 128 bits for AES key
        byte[] IV = BitConverter.GetBytes(ivInt); // Convert int to byte array
        if (IV.Length < 16)
        {
            Array.Resize(ref IV, 16); // Ensure the IV is 16 bytes
        }

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = IV;

            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(buffer))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
