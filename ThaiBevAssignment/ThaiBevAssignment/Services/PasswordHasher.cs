using System.Security.Cryptography;
using System.Text;

namespace ThaiBevAssignment.Services
{
    internal static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            int iterations = 100_000;
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);
            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split('.');
                if (parts.Length != 3) return false;
                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] stored = Convert.FromBase64String(parts[2]);
                byte[] computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, stored.Length);
                return CryptographicOperations.FixedTimeEquals(computed, stored);
            }
            catch
            {
                return false;
            }
        }
    }
}
