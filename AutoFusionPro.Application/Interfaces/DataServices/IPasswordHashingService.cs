namespace AutoFusionPro.Application.Interfaces.DataServices
{
    public interface IPasswordHashingService
    {
        /// <summary>
        /// Hashes a plain-text password.
        /// </summary>
        /// <param name="password">The plain-text password.</param>
        /// <returns>A tuple containing the generated hash and salt.</returns>
        Task<(string hash, string salt)> HashPassword(string password);

        /// <summary>
        /// Verifies a plain-text password against a stored hash and salt.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hash">The stored password hash.</param>
        /// <param name="salt">The stored salt.</param>
        /// <returns>True if the password matches the hash, false otherwise.</returns>
        bool VerifyPassword(string password, string hash, string salt);
    }
}
