using System.Security.Cryptography;
using System.Text;

namespace PTTDigital.Email.Common.EncryptDecrypt.Cryptography;

/// <summary>
/// AES 256 bits Encryption Library with Salt
/// <see cref="https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt"/>
/// </summary>
public class EncryptDecryptHelper : IEncryptDecryptHelper
{
    public string? SymmetricKey { get; set; }

    public string Encrypt(string text, string p455w0rd, bool isSalt = true)
    {
        return EncryptWithSalt(text, p455w0rd);
    }

    public byte[] EncryptBytes(string text, bool isSalt = true)
    {
        return EncryptWithSaltBytes(text, SymmetricKey!);
    }

    public string Decrypt(string encryptedText, string p455w0rd, bool isSalt = false)
    {
        return DecryptWithSalt(encryptedText, p455w0rd);
    }

    public string DecryptBytes(byte[] encryptedText, bool isSalt = false)
    {
        return DecryptWithSaltBytes(encryptedText, SymmetricKey!);
    }

    private string EncryptWithSalt(string text, string p455w0rd)
    {
        // For additional security Pin the p@ssw0rd of your files
        byte[] originalBytes = Encoding.UTF8.GetBytes(text);
        byte[] encryptedBytes = null;

        byte[] salt = new byte[Pbkdf2Helper.SALT_SIZE];
        Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(p455w0rd, salt, Pbkdf2Helper.ITERATIONS);
        var p455w0rdBytes = pbkdf2.GetBytes(Pbkdf2Helper.HASH_SIZE);

        // Getting the salt size
        int saltSize = GetSaltSize(p455w0rdBytes);
        // Generating salt bytes
        byte[] saltBytes = GetRandomBytes(saltSize);

        // Appending salt bytes to original bytes
        byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
        for (int i = 0; i < saltBytes.Length; i++)
        {
            bytesToBeEncrypted[i] = saltBytes[i];
        }
        for (int i = 0; i < originalBytes.Length; i++)
        {
            bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
        }

        encryptedBytes = AES_Encrypt(bytesToBeEncrypted, p455w0rdBytes);

        return Convert.ToBase64String(encryptedBytes);
    }

    private byte[] EncryptWithSaltBytes(string text, string p455w0rd)
    {
        // For additional security Pin the p@ssw0rd of your files
        byte[] originalBytes = Encoding.UTF8.GetBytes(text);
        byte[] encryptedBytes = null;

        byte[] salt = new byte[Pbkdf2Helper.SALT_SIZE];
        Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(p455w0rd, salt, Pbkdf2Helper.ITERATIONS);
        var p455w0rdBytes = pbkdf2.GetBytes(Pbkdf2Helper.HASH_SIZE);

        // Getting the salt size
        int saltSize = GetSaltSize(p455w0rdBytes);
        // Generating salt bytes
        byte[] saltBytes = GetRandomBytes(saltSize);

        // Appending salt bytes to original bytes
        byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
        for (int i = 0; i < saltBytes.Length; i++)
        {
            bytesToBeEncrypted[i] = saltBytes[i];
        }
        for (int i = 0; i < originalBytes.Length; i++)
        {
            bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
        }

        return AES_Encrypt(bytesToBeEncrypted, p455w0rdBytes);
    }

    private string DecryptWithSalt(string encryptedText, string p455w0rd)
    {
        // For additional security Pin the p@ssw0rd of your files
        //GCHandle gch = GCHandle.Alloc(p@ssw0rd, GCHandleType.Pinned);

        byte[] bytesToBeDecrypted = Convert.FromBase64String(encryptedText);

        byte[] salt = new byte[Pbkdf2Helper.SALT_SIZE];
        Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(p455w0rd, salt, Pbkdf2Helper.ITERATIONS);
        var p455w0rdBytes = pbkdf2.GetBytes(Pbkdf2Helper.HASH_SIZE);

        byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, p455w0rdBytes);

        // Getting the size of salt
        int saltSize = GetSaltSize(p455w0rdBytes);

        // Removing salt bytes, retrieving original bytes
        byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
        for (int i = saltSize; i < decryptedBytes.Length; i++)
        {
            originalBytes[i - saltSize] = decryptedBytes[i];
        }

        // To increase the security of the encryption, delete the given p@ssw0rd from the memory !
        //ZeroMemory(gch.AddrOfPinnedObject(), p@ssw0rd.Length * 2);
        //gch.Free();

        return Encoding.UTF8.GetString(originalBytes);
    }

    private string DecryptWithSaltBytes(byte[] encryptedText, string p455w0rd)
    {
        // For additional security Pin the p@ssw0rd of your files
        //GCHandle gch = GCHandle.Alloc(p@ssw0rd, GCHandleType.Pinned);

        byte[] salt = new byte[Pbkdf2Helper.SALT_SIZE];
        Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(p455w0rd, salt, Pbkdf2Helper.ITERATIONS);
        var p455w0rdBytes = pbkdf2.GetBytes(Pbkdf2Helper.HASH_SIZE);

        byte[] decryptedBytes = AES_Decrypt(encryptedText, p455w0rdBytes);

        // Getting the size of salt
        int saltSize = GetSaltSize(p455w0rdBytes);

        // Removing salt bytes, retrieving original bytes
        byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
        for (int i = saltSize; i < decryptedBytes.Length; i++)
        {
            originalBytes[i - saltSize] = decryptedBytes[i];
        }

        // To increase the security of the encryption, delete the given p@ssw0rd from the memory !
        //ZeroMemory(gch.AddrOfPinnedObject(), p@ssw0rd.Length * 2);
        //gch.Free();

        return Encoding.UTF8.GetString(originalBytes);
    }

    #region Private Methods
    private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] p455w0rdBytes)
    {
        byte[] encryptedBytes = null;

        // Set your salt here, change it to meet your flavor:
        byte[] saltBytes = new byte[Pbkdf2Helper.SALT_SIZE];
        // Example:
        //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(p455w0rdBytes, saltBytes, Pbkdf2Helper.ITERATIONS);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }

        return encryptedBytes;
    }

    private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] p455w0rdBytes)
    {
        byte[] decryptedBytes = null;
        // Set your salt here to meet your flavor:
        byte[] saltBytes = new byte[Pbkdf2Helper.SALT_SIZE];
        // Example:
        //saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(p455w0rdBytes, saltBytes, Pbkdf2Helper.ITERATIONS);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }

    private int GetSaltSize(byte[] p455w0rdBytes)
    {
        byte[] salt = new byte[Pbkdf2Helper.SALT_SIZE];
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(p455w0rdBytes, salt, 1000);
        byte[] ba = key.GetBytes(Pbkdf2Helper.HASH_SIZE - 30);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ba.Length; i++)
        {
            sb.Append(Convert.ToInt32(ba[i]).ToString());
        }
        int saltSize = 0;
        string s = sb.ToString();
        foreach (char c in s)
        {
            int intc = Convert.ToInt32(c.ToString());
            saltSize = saltSize + intc;
        }

        return saltSize;
    }

    private byte[] GetRandomBytes(int length)
    {
        byte[] ba = new byte[length];
        RNGCryptoServiceProvider.Create().GetBytes(ba);
        return ba;
    }

    //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
    private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        byte[] encrypted;

        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt =
                        new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
    private static string DecryptStringFromBytes_Aes(string cipherTextString, byte[] Key, byte[] IV)
    {
        byte[] cipherText = Convert.FromBase64String(cipherTextString);

        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        string plaintext = null;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt =
                        new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
    #endregion
}