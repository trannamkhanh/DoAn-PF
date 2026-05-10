# 📚 Hướng dẫn setup bảng Staff - SQL Server

## 🎯 Mục đích
Script này giúp bạn tạo bảng `Staff` và thêm tài khoản demo vào database `LibraryDB`.

## 📋 Thứ tự thực hiện

### **Bước 1: Tạo bảng Staff**
1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối tới SQL Server (localhost hoặc .\SQLEXPRESS)
3. Chọn database **LibraryDB**
4. Chọn **File** → **Open** → **File**
5. Mở file: `Database/CreateStaffTable.sql`
6. Nhấn **Execute** (F5)

**Kết quả:** Bảng Staff được tạo với 2 index tối ưu hóa

### **Bước 2: Thêm tài khoản demo**
1. Mở file: `Database/InsertDemoAccounts.sql`
2. Nhấn **Execute** (F5)

**Kết quả:** 2 tài khoản demo được thêm vào

### **Bước 3: Kiểm tra dữ liệu**
1. Mở file: `Database/QueryStaffData.sql`
2. Thực hiện query đầu tiên (XEM TẤT CẢ TÀI KHOẢN)
3. Nhấn **Execute** (F5)

## 🔑 Tài khoản Demo

| Username | Password | Role  | Hash SHA256 |
|----------|----------|-------|------------|
| admin    | admin123 | Admin | JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk= |
| staff    | staff123 | Staff | EBdue3sk0xes/PjSBkz9LyThVPe1qWYDB31e+BPWprY= |

## 📂 File SQL cần dùng

```
Database/
├── CreateStaffTable.sql      ← Chạy trước (tạo bảng)
├── InsertDemoAccounts.sql    ← Chạy sau (thêm dữ liệu)
└── QueryStaffData.sql        ← Dùng để kiểm tra & quản lý
```

## 🛠️ Cách sử dụng QueryStaffData.sql

File này chứa 10 query hữu ích:

```sql
-- 1. Xem tất cả tài khoản (mở comment để chạy)
SELECT * FROM Staff;

-- 2. Xem chi tiết một tài khoản
SELECT * FROM Staff WHERE Username = 'admin';

-- 3. Đếm tổng số tài khoản
SELECT COUNT(*) FROM Staff;

-- 4. Xem tài khoản đang hoạt động
SELECT * FROM Staff WHERE IsActive = 1;

-- 5. Cập nhật IsActive thành 0 (khóa tài khoản)
UPDATE Staff SET IsActive = 0 WHERE Username = 'staff';

-- 6. Thay đổi mật khẩu
UPDATE Staff SET PasswordHash = 'HASH_MỚI' WHERE Username = 'admin';

-- 7. Xóa tài khoản (CẬN THẬN!)
DELETE FROM Staff WHERE Username = 'staff';
```

## ❌ Vấn đề thường gặp

### Lỗi: "Bảng Staff chưa được tạo"
→ Chạy **CreateStaffTable.sql** TRƯỚC

### Lỗi: "Duplicate key" khi insert
→ Tài khoản đã tồn tại. Script sẽ tự xóa cũ rồi thêm mới.

### Lỗi: "Invalid hash"
→ Kiểm tra hash password bằng PowerShell:
```powershell
$sha256 = New-Object System.Security.Cryptography.SHA256CryptoServiceProvider
$hash = [Convert]::ToBase64String($sha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes("admin123")))
Write-Host $hash
```

### Login vẫn không được
1. Kiểm tra `IsActive = 1`: `SELECT * FROM Staff;`
2. Kiểm tra đúng username/password
3. Kiểm tra hash chính xác

## 🔐 Bảo mật

- ✅ Mật khẩu được hash SHA256 + Base64
- ✅ Không lưu mật khẩu plain text
- ✅ Chỉ cho phép user có `IsActive = 1` login
- ✅ Có index tối ưu hóa query login


```sql
CREATE TABLE Staff (
    StaffID INT PRIMARY KEY IDENTITY(1,1),      -- ID (tự động tăng)
    Username NVARCHAR(50) NOT NULL UNIQUE,      -- Tên đăng nhập (duy nhất)
    PasswordHash NVARCHAR(255) NOT NULL,        -- Hash mật khẩu
    FullName NVARCHAR(100),                     -- Họ tên
    Role NVARCHAR(20) DEFAULT 'Staff',          -- Vai trò (Admin/Staff)
    IsActive BIT DEFAULT 1,                     -- Trạng thái (1=hoạt động, 0=khóa)
    CreatedDate DATETIME DEFAULT GETDATE(),     -- Ngày tạo
    UpdatedDate DATETIME DEFAULT GETDATE()      -- Ngày cập nhật
);
```

## 💡 Tips

- **Thêm tài khoản mới:** Insert vào Staff với PasswordHash được hash SHA256
- **Khóa tài khoản:** `UPDATE Staff SET IsActive = 0 WHERE Username = '...'`
- **Đổi mật khẩu:** Hash mật khẩu mới rồi `UPDATE PasswordHash`
- **Xóa tài khoản:** `DELETE FROM Staff WHERE Username = '...'`

---

**Cần hỗ trợ sinh hash? Xem file `Utilities/PasswordHashHelper.cs`**
