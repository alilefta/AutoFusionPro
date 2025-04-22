using AutoFusionPro.Application.Interfaces.DataServices;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace AutoFusionPro.Application.Services
{
    public class BCryptPasswordHashingService : IPasswordHashingService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;
        private readonly ILogger<BCryptPasswordHashingService> _logger;

        public BCryptPasswordHashingService(ILogger<BCryptPasswordHashingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(string hash, string salt)> HashPassword(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                _logger.LogInformation("Password hashed successfully.");
                return (key, salt);
            }
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, Convert.FromBase64String(storedSalt), Iterations, HashAlgorithmName.SHA256))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var result = key == storedHash;

                _logger.LogInformation("Password verification {Result}.", result ? "succeeded" : "failed");
                return result;
            }
        }
    }
}
