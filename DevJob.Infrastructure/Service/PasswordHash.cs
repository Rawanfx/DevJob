using DevJob.Application.ServiceContract;
using System.Security.Cryptography;
namespace DevJob.Infrastructure.Service
{
    public class PasswordHash : IPasswordHash
    {
        private const int saltSize = 16;
        private const int HashSize = 32;
        private readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;
       public string PasswordHashed(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, algorithm, HashSize);
            return $"{Convert.ToHexString(hash)} - {Convert.ToHexString(salt)}";
        }
    }
}
