namespace Domain.Services
{
    public interface ICryptoService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    }
}