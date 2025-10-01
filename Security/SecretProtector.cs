using System.Security.Cryptography;
using System.Text;

namespace VkOrdApiWrapper.Security
{
    public class SecretProtector : ISecretProtector
    {
        private readonly byte[] _key;

        public SecretProtector(IConfiguration configuration)
        {
            var keyBase64 = configuration["Secrets:EncryptionKey"];
            if (string.IsNullOrWhiteSpace(keyBase64))
            {
                throw new InvalidOperationException("Encryption key is missing. Set Secrets:EncryptionKey in configuration.");
            }
            _key = Convert.FromBase64String(keyBase64);
            if (_key.Length != 32)
            {
                throw new InvalidOperationException("Encryption key must be 32 bytes (Base64 of 32-byte key) for AES-256-GCM.");
            }
        }

        public string Encrypt(string plaintext)
        {
            using var aes = new AesGcm(_key);
            var nonce = RandomNumberGenerator.GetBytes(12);
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[16];
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            var combined = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, combined, nonce.Length + tag.Length, ciphertext.Length);
            return Convert.ToBase64String(combined);
        }

        public string Decrypt(string ciphertext)
        {
            var combined = Convert.FromBase64String(ciphertext);
            var nonce = new byte[12];
            var tag = new byte[16];
            var encrypted = new byte[combined.Length - nonce.Length - tag.Length];
            Buffer.BlockCopy(combined, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(combined, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(combined, nonce.Length + tag.Length, encrypted, 0, encrypted.Length);

            using var aes = new AesGcm(_key);
            var plaintext = new byte[encrypted.Length];
            aes.Decrypt(nonce, encrypted, tag, plaintext);
            return Encoding.UTF8.GetString(plaintext);
        }
    }
}


