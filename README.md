# Quản lý thư viện (WPF)

Ứng dụng WPF (.NET 10) quản lý sách, bạn đọc, mượn/trả và báo cáo cho thư viện.

---

## Chạy nhanh cho người mới pull code

### 1) Yêu cầu
- Visual Studio 2026 (hoặc bản hỗ trợ .NET 10 + WPF)
- SQL Server (Express/Developer) hoặc LocalDB

### 2) Tạo database
- Mở `SQL Server Management Studio`.
- Chạy file script: `database/init.sql`.
- Script sẽ tự tạo DB `LibraryDB`, tạo bảng và dữ liệu mẫu.

### 3) Kiểm tra connection string
File `đồ án 2/App.config` đang để mặc định:

`Server=localhost;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;`

Nếu máy bạn không dùng `localhost` (Ví dụ: `.\SQLEXPRESS`) thì sửa `Server=` cho đúng.

### 4) Chạy app
- Mở solution `đồ án 2.slnx`
- Set startup project: `đồ án 2`
- Nhấn `F5` hoặc `Ctrl + F5`

App sẽ chạy dạng desktop `.exe` và đọc/ghi trực tiếp vào DB.

---

## 🔐 Hệ thống Đăng Nhập

### ✨ Tính năng

- **Login Screen** - Giao diện đăng nhập hiện đại
- **Password Security** - Hash SHA256 + Base64
- **Database Integration** - Xác thực từ bảng Staff
- **Access Control** - Chỉ user có `IsActive=1` mới login được

### 🔑 Cách hoạt động

1. Ứng dụng khởi động → **LoginWindow**
2. User nhập **Username** & **Password**
3. Hệ thống query bảng **Staff** từ database
4. So sánh PasswordHash (SHA256)
5. ✅ Thành công → Mở **MainWindow**
6. ❌ Thất bại → Hiển thị thông báo lỗi

### 🆕 Thêm tài khoản mới

**Bước 1: Sinh hash mật khẩu**
```powershell
$sha256 = New-Object System.Security.Cryptography.SHA256CryptoServiceProvider
$hash = [Convert]::ToBase64String($sha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes("matkhai123")))
Write-Host $hash
```

**Bước 2: Insert vào Staff**
```sql
INSERT INTO Staff (Username, PasswordHash, FullName, Role, IsActive)
VALUES ('newuser', 'HASH_VỪA_SINH', 'Tên đầy đủ', 'Staff', 1);
```

**Bước 3: Kiểm tra**
```sql
SELECT * FROM Staff WHERE Username = 'newuser';
```

---

## 💾 Cấu trúc Database

### Bảng Staff (Nhân viên)
```sql
StaffID INT PRIMARY KEY           -- ID tự tăng
Username NVARCHAR(50) UNIQUE      -- Tên đăng nhập (duy nhất)
PasswordHash NVARCHAR(255)        -- Hash mật khẩu SHA256
FullName NVARCHAR(100)            -- Họ tên
Role NVARCHAR(20)                 -- Vai trò (Admin/Staff)
IsActive BIT                      -- Trạng thái (1=hoạt động, 0=khóa)
CreatedDate DATETIME              -- Ngày tạo
UpdatedDate DATETIME              -- Ngày cập nhật
```

### Các bảng khác
- **Books** - Danh sách sách
- **Members** - Bạn đọc
- **Loans** - Mượn/Trả sách
- **Reports** - Báo cáo thống kê

---

## 📂 Cấu trúc Project

```
đồ án 2/
├── LoginWindow.xaml(.cs)         # ✨ Cửa sổ đăng nhập
├── MainWindow.xaml(.cs)          # Dashboard chính
├── App.xaml(.cs)                 # Khởi động ứng dụng
├── Models/
│   ├── Staff.cs
│   ├── Book.cs
│   ├── Member.cs
│   ├── Loan.cs
│   ├── Report.cs
│   └── LibraryDbContext.cs       # EF Core
├── Utilities/
│   ├── PasswordHasher.cs         # 🔐 Hash/verify password
│   └── PasswordHashHelper.cs
├── Database/
│   ├── CreateStaffTable.sql      # 📋 Tạo bảng Staff
│   ├── InsertDemoAccounts.sql    # 👤 Thêm 2 tài khoản demo
│   ├── QueryStaffData.sql        # 📊 10 query quản lý Staff
│   ├── STAFF_SETUP_GUIDE.md      # 📖 Hướng dẫn chi tiết
│   └── GeneratePasswordHash.ps1  # 🔑 Script sinh hash
├── LOGIN_GUIDE.md                # Hướng dẫn chức năng Login
└── README.md                     # File này
```

---

## 🎯 Chức năng Chính

### ✅ Hoàn thành
- 🔐 **Đăng nhập** - Xác thực tài khoản & password
- 📖 **Quản lý Sách** - Thêm/sửa/xóa/tìm kiếm
- 👥 **Quản lý Bạn đọc** - Thông tin thành viên
- 📤 **Quản lý Mượn/Trả** - Theo dõi mượn sách & tính phạt
- 📊 **Báo cáo** - Thống kê & lưu trữ
- 👨‍💼 **Quản lý Nhân viên** - Tài khoản & phân quyền

### 🔄 Cách sử dụng

1. **Login** - Đăng nhập bằng tài khoản
2. **Dashboard** - Xem tổng quan (Home)
3. **Manage Books** - Quản lý danh sách sách
4. **Manage Members** - Quản lý bạn đọc
5. **Manage Loans** - Mượn/trả sách & tính phạt
6. **Reports** - Xem báo cáo

---

## 🐛 Troubleshooting

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-----------|----------|
| **"Database not found"** | LibraryDB chưa tạo | Chạy: `CREATE DATABASE LibraryDB;` |
| **"Table Staff not found"** | Chưa chạy CreateStaffTable.sql | Chạy: `Database/CreateStaffTable.sql` |
| **"Login failed"** | Tài khoản sai/chưa tạo | Chạy: `Database/InsertDemoAccounts.sql` |
| **"Cannot connect to SQL Server"** | Server không chạy/sai host | Kiểm tra connection string trong LibraryDbContext |
| **"IsActive = 0"** | Tài khoản bị khóa | Kiểm tra IsActive trong bảng Staff |

---

## 📚 Tài liệu & Hướng dẫn

- 📖 **Login Setup**: Xem `LOGIN_GUIDE.md`
- 📖 **Database Setup**: Xem `Database/STAFF_SETUP_GUIDE.md`
- 🔍 **Query Staff**: Xem `Database/QueryStaffData.sql` (10 mẫu query)
- 🔗 **.NET 10 Docs**: [learn.microsoft.com/dotnet](https://learn.microsoft.com/en-us/dotnet/)
- 🔗 **EF Core**: [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- 🔗 **WPF**: [Windows Presentation Foundation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)

---

## 👥 Đóng góp

1. **Fork** repository
2. **Tạo branch** mới: `git checkout -b feature/YourFeature`
3. **Commit** thay đổi: `git commit -m 'Add YourFeature'`
4. **Push** branch: `git push origin feature/YourFeature`
5. **Tạo Pull Request**

---

## 📝 License

MIT License - Tự do sử dụng & phát triển

---

## 📧 Hỗ trợ

- 🐛 **Report bugs**: [GitHub Issues](https://github.com/trannamkhanh/DoAn-PF/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/trannamkhanh/DoAn-PF/discussions)

---

## 🎯 Roadmap (Sắp tới)

- [ ] Export báo cáo Excel/PDF
- [ ] Import dữ liệu từ Excel
- [ ] Backup/Restore database
- [ ] Gửi email thông báo quá hạn
- [ ] QR code cho thẻ bạn đọc
- [ ] Dashboard analytics nâng cao
- [ ] Dark mode

---

**Chúc bạn học tập & phát triển vui vẻ! 🚀**

*Cập nhật lần cuối: 2024*
