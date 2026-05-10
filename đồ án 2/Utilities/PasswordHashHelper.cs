using System.Security.Cryptography;
using System.Text;

namespace WpfApp3.Utilities
{
    /// <summary>
    /// Helper class để sinh hash password cho script SQL
    /// Chỉ dùng một lần khi cần tạo tài khoản mới
    /// </summary>
    public static class PasswordHashHelper
    {
        /// <summary>
        /// Sinh hash SHA256 cho mật khẩu (dùng để insert vào database)
        /// Ví dụ: var hash = GenerateHash("admin123");
        /// </summary>
        public static string GenerateHash(string password)
        {
            return HashPassword(password);
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
