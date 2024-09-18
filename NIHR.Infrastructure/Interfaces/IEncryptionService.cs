namespace NIHR.Infrastructure.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string input);
        string Decrypt(string input);
    }
}
