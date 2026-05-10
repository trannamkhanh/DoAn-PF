# 📚 Library Management System (WPF .NET 10)

Hệ thống quản lý thư viện - Sách, bạn đọc, mượn/trả, báo cáo

---

## ⚡ Hướng dẫn chạy nhanh

### 1️⃣ Clone & Mở Project
```bash
git clone https://github.com/trannamkhanh/DoAn-PF.git
cd "đồ án 2"
```
Mở file `đồ án 2.sln` trong Visual Studio

### 2️⃣ Tạo Database
Mở SQL Server Management Studio, chạy:
```sql
CREATE DATABASE LibraryDB;
```

### 3️⃣ Tạo bảng Staff
Trong SSMS, chọn database `LibraryDB` → Mở và chạy file:
```
Database/CreateStaffTable.sql
```

### 4️⃣ Thêm tài khoản demo
Chạy file:
```
Database/InsertDemoAccounts.sql
```

**Tài khoản demo:**
- `admin` / `admin123` (Admin)
- `staff` / `staff123` (Staff)

### 5️⃣ Chạy ứng dụng
Visual Studio → Nhấn **F5** (Debug) hoặc **Ctrl+F5** (Run)

---

## 🔑 Thêm tài khoản mới

### Bước 1: Sinh hash password (PowerShell)
```powershell
$sha256 = New-Object System.Security.Cryptography.SHA256CryptoServiceProvider
$hash = [Convert]::ToBase64String($sha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes("matkhai123")))
Write-Host $hash
```

### Bước 2: Insert vào database
```sql
INSERT INTO Staff (Username, PasswordHash, FullName, Role, IsActive)
VALUES ('username', 'HASH_VỪA_SINH', 'Họ Tên', 'Staff', 1);
```

### Bước 3: Kiểm tra
```sql
SELECT * FROM Staff;
```

---

## 📂 File quan trọng

```
Database/
├── CreateStaffTable.sql       ← Tạo bảng Staff
├── InsertDemoAccounts.sql     ← Thêm 2 tài khoản demo
└── QueryStaffData.sql         ← 10 query quản lý Staff
```

---

## 🐛 Lỗi thường gặp

| Lỗi | Giải pháp |
|-----|----------|
| **Database not found** | Chạy: `CREATE DATABASE LibraryDB;` |
| **Table Staff not found** | Chạy: `Database/CreateStaffTable.sql` |
| **Login failed** | Chạy: `Database/InsertDemoAccounts.sql` |
| **Cannot connect to SQL Server** | Kiểm tra connection string trong `LibraryDbContext.OnConfiguring()` |

---

## 🔗 Xem thêm

- `LOGIN_GUIDE.md` - Hướng dẫn chức năng Login
- `Database/STAFF_SETUP_GUIDE.md` - Hướng dẫn chi tiết cấu hình Staff
