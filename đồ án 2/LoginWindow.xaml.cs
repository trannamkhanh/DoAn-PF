using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using WpfApp3.Models;

namespace WpfApp3
{
    public partial class LoginWindow : Window
    {
        private readonly LibraryDbContext _db;

        public LoginWindow()
        {
            InitializeComponent();
            _db = new LibraryDbContext();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text?.Trim() ?? "";
            string password = PasswordInput.Password ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage.Text = "Vui lòng nhập tên đăng nhập và mật khẩu";
                return;
            }

            try
            {
                // Query staff từ database
                var staff = _db.Staff
                    .AsNoTracking()
                    .FirstOrDefault(s => s.Username == username && s.IsActive == true);

                if (staff == null)
                {
                    ErrorMessage.Text = "Tên đăng nhập hoặc mật khẩu không chính xác";
                    PasswordInput.Clear();
                    return;
                }

                // Kiểm tra mật khẩu (so sánh hash)
                if (!VerifyPassword(password, staff.PasswordHash))
                {
                    ErrorMessage.Text = "Tên đăng nhập hoặc mật khẩu không chính xác";
                    PasswordInput.Clear();
                    return;
                }

                // Đăng nhập thành công
                MainWindow mainWindow = new MainWindow();
                mainWindow.CurrentUser = staff; // Lưu thông tin user (sẽ thêm property này vào MainWindow)
                mainWindow.Show();

                // Đóng cửa sổ đăng nhập
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Lỗi: {ex.Message}";
            }
        }

        /// <summary>
        /// Xác minh mật khẩu bằng cách so sánh hash
        /// </summary>
        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                // Hash mật khẩu nhập vào và so sánh
                string hashOfInput = HashPassword(password);
                return hashOfInput == hash;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Hash mật khẩu bằng SHA256
        /// </summary>
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
