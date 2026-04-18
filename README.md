# WpfApp3 - Library Management (WPF)

## Chạy nhanh cho người mới pull code

### 1) Yêu cầu
- Visual Studio 2026 (hoặc bản hỗ trợ .NET 10 + WPF)
- SQL Server (Express/Developer) hoặc LocalDB

### 2) Tạo database
- Mở `SQL Server Management Studio`.
- Chạy file script: `database/init.sql`.
- Script sẽ tự tạo DB `LibraryDB`, tạo bảng và dữ liệu mẫu.

### 3) Kiểm tra connection string
File `WpfApp3/App.config` đang để mặc định:

`Server=localhost;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;`

Nếu máy bạn không dùng `localhost` thì sửa `Server=` cho đúng.

### 4) Chạy app
- Mở solution `WpfApp3.slnx`
- Set startup project: `WpfApp3`
- Nhấn `F5` hoặc `Ctrl + F5`

App sẽ chạy dạng desktop `.exe` và đọc/ghi trực tiếp vào DB.
