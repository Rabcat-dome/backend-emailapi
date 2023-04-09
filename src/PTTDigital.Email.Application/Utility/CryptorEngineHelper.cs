using System.Security.Cryptography;
using System.Text;

namespace PTTDigital.Email.Application.Utility;

public static class CryptorEngineHelper
{
    // Random key from : 256-bit WEP Keys
    private const string _key = "21685645C76ABACC37EA2AB1B75F5";

    public static string Encrypt(string toEncrypt)
    {
        try
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            var toEncryptArray = utf8.GetBytes(toEncrypt);

            const string key = _key;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var keyArray = md5.ComputeHash(utf8.GetBytes(key));
            md5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch (Exception ex)
        {
            throw new CryptorEngineException(ex.Message);
        }
    }

    public static string Decrypt(string cipherString)
    {
        try
        {
            UTF8Encoding utf8 = new UTF8Encoding();

            var toEncryptArray = Convert.FromBase64String(cipherString);
            const string key = _key;

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(utf8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return utf8.GetString(resultArray);
        }
        catch (Exception ex)
        {
            throw new CryptorEngineException(ex.Message);
        }
    }

    internal static string GetHash(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var sb = new StringBuilder();
        var data = Encoding.UTF8.GetBytes(input);

        using var sha = SHA256.Create();
        var hashValue = sha.ComputeHash(data);
        foreach (byte x in hashValue)
        {
            sb.Append(x.ToString("X2"));
        }
        return sb.ToString();
    }
}
