using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for App
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Khởi động với LoginWindow thay vì MainWindow
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            // Không gán null vào StartupUri, thay vào đó ẩn cửa sổ mặc định
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
    }
}
