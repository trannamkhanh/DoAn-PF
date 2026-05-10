# Hướng dẫn Chức năng Login

## 🎯 Tính năng đã thêm

1. **LoginWindow** - Giao diện đăng nhập
   - Tên đăng nhập (Username)
   - Mật khẩu (Password)
   - Xác thực từ database
   - Thông báo lỗi chi tiết

2. **PasswordHasher** - Utility mã hóa mật khẩu
   - SHA256 Hash
   - Base64 encoding
   - Xác minh mật khẩu

3. **Database Integration**
   - Query Staff từ LibraryDbContext
   - Kiểm tra Username + IsActive
   - So sánh PasswordHash

## 📋 Cách sử dụng

### 1. Thêm tài khoản demo vào database

Chạy script SQL trong SQL Server Management Studio:

```sql
-- Mở file: Database/InsertDemoAccounts.sql
-- Chọn database LibraryDB
-- Thực thi script
```

**Tài khoản demo:**
- **Username:** `admin`
- **Password:** `admin123`
- **Role:** Admin

Hoặc:
- **Username:** `staff`
- **Password:** `staff123`
- **Role:** Staff

### 2. Chạy ứng dụng

1. Build project: `Ctrl+Shift+B`
2. Chạy: `F5`
3. Nhập tên đăng nhập & mật khẩu
4. Nhấn "Đăng nhập"

### 3. Thêm tài khoản mới

**Cách 1: Dùng SQL**
```sql
-- Sinh hash mật khẩu
-- admin123 -> JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=

INSERT INTO Staff (Username, PasswordHash, FullName, Role, IsActive)
VALUES ('newuser', 'HASH_ĐƯỢC_SINH_RA', 'Tên Full', 'Staff', 1);
```

**Cách 2: Dùng PowerShell để sinh hash**
```powershell
$pwd = "matkhai_moi"
$sha256 = New-Object System.Security.Cryptography.SHA256CryptoServiceProvider
$hash = [Convert]::ToBase64String($sha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($pwd)))
Write-Host $hash
```

## 🔐 Bảo mật

- Mật khẩu được hash bằng **SHA256**
- Không lưu mật khẩu plain text
- Chỉ kiểm tra users có `IsActive = true`
- Thông báo lỗi chung chung để tránh leak username

## 📁 File thêm/sửa

- **LoginWindow.xaml** - Giao diện Login (new)
- **LoginWindow.xaml.cs** - Logic Login (new)
- **Utilities/PasswordHasher.cs** - Hashing utility (new)
- **App.xaml.cs** - Khởi động từ LoginWindow (modified)
- **App.xaml** - Xóa StartupUri (modified)
- **MainWindow.xaml.cs** - Thêm CurrentUser property (modified)
- **Database/InsertDemoAccounts.sql** - Script demo accounts (new)

## ❓ Troubleshooting

### "Tên đăng nhập hoặc mật khẩu không chính xác"
- Kiểm tra Username trong bảng Staff
- Kiểm tra `IsActive = 1` (true)
- Kiểm tra PasswordHash đúng

### "Lỗi: Cannot connect to database"
- Kiểm tra SQL Server đang chạy
- Kiểm tra connection string trong `LibraryDbContext.OnConfiguring()`
- Đảm bảo database `LibraryDB` tồn tại

### Hash không khớp
- Dùng script PowerShell ở trên để sinh hash chính xác
- Đảm bảo mật khẩu được nhập chính xác
