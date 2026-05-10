using System.Security.Cryptography;
using System.Text;

namespace WpfApp3.Utilities
{
    /// <summary>
    /// Utility class for password hashing and verification
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hash a password using SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Verify if a plain password matches a hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            try
            {
                string hashOfInput = HashPassword(password);
                return hashOfInput == hash;
            }
            catch
            {
                return false;
            }
        }
    }
}
