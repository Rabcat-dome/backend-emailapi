namespace PTTDigital.Email.Common.EncryptDecrypt.Cryptography;

public interface IEncryptDecryptHelper
{
    string? SymmetricKey { get; set; }

    byte[] EncryptBytes(string text, bool isSalt = true);

    string Encrypt(string text, string p455w0rd, bool isSalt = true);

    string Decrypt(string encryptedText, string p455w0rd, bool isSalt = false);

    string DecryptBytes(byte[] encryptedText, bool isSalt = false);
}