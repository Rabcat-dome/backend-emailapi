using System.Security.Cryptography;
using System.Text;
using PTTDigital.Email.Common.EncryptDecrypt.Cryptography.RSAx;

namespace PTTDigital.Email.Common.EncryptDecrypt.Cryptography;

public class Rsa
{
    public enum KeySize : int
    {
        Key_512 = 512,
        Key_1024 = 1024,
        Key_2048 = 2048,
        Key_3072 = 3072,
        Key_4096 = 4096,
        Key_6144 = 6144,
        Key_8192 = 8192,
        Key_10240 = 10240,
        Key_16384 = 16384,
    }

    public enum RSAAlgorithm
    {
        UnDefined = 0,
        SHA1 = 1,
        SHA256 = 2,
        SHA512 = 3
    }

    public class RsaKeyPair
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    public RsaKeyPair GenerateKeyPair(KeySize size)
    {
        using var provider = new RSACryptoServiceProvider((int)size);
        return new RsaKeyPair
        {
            PublicKey = provider.ToXmlString(false),
            PrivateKey = provider.ToXmlString(true),
        };
    }

    public string Encrypt(string key, string plainText, RSAAlgorithm algorithm, KeySize size)
    {
        try
        {
            using (var rsax = new RSAx.RSAx(key, (int)size))
            {
                rsax.UseCRTForPublicDecryption = true;
                rsax.RSAxHashAlgorithm = (RSAxParameters.RSAxHashAlgorithm)algorithm;

                var CT = rsax.Encrypt(Encoding.UTF8.GetBytes(plainText), false, true);
                var result = Convert.ToBase64String(CT);
                return result;
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    public string Decrypt(string key, string cipherText, RSAAlgorithm algorithm, KeySize size)
    {
        try
        {
            using (var rsax = new RSAx.RSAx(key, (int)size))
            {
                rsax.UseCRTForPublicDecryption = true;
                rsax.RSAxHashAlgorithm = (RSAxParameters.RSAxHashAlgorithm)algorithm;

                var PT = rsax.Decrypt(Convert.FromBase64String(cipherText), true, true);
                var result = Encoding.UTF8.GetString(PT);
                return result;
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}