using System.Security.Cryptography;
using System.Text;

namespace PTTDigital.Email.Common.EncryptDecrypt.Cryptography;

public static class Pbkdf2Helper
{
    public const int SALT_SIZE = 32; // size in bytes
    public const int HASH_SIZE = 32; // size in bytes
    internal const int HASH_SIZE64 = 64; // size in bytes
    internal const int SALT_SIZE16 = 16; // size in bytes
    internal const int HASH_SIZE16 = 16; // size in bytes
    public const int ITERATIONS = 10000; // number of pbkdf2 iterations
    public static string CreatePbkdf2(string input)
    {
        byte[] salt = new byte[SALT_SIZE16];
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, ITERATIONS))
        {
            var hashBytes = pbkdf2.GetBytes(HASH_SIZE16);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public static byte[] CreatePbkdf2Bytes(string input)
    {
        byte[] salt = new byte[SALT_SIZE16];
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, ITERATIONS))
        {
            var hashBytes = pbkdf2.GetBytes(HASH_SIZE16);
            return hashBytes;
        }
    }
}