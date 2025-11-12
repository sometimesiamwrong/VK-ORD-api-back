namespace WebApp.Security
{
    public interface ISecretProtector
    {
        string Encrypt(string plaintext);
        string Decrypt(string ciphertext);
    }
}


