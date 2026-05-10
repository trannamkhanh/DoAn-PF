-- ============================================================
-- Script tạo bảng Staff cho hệ thống quản lý thư viện
-- Chạy script này TRƯỚC khi chạy InsertDemoAccounts.sql
-- ============================================================

-- Xóa bảng Staff nếu đã tồn tại (cẩn thận với dữ liệu)
-- DROP TABLE Staff;

-- Tạo bảng Staff
CREATE TABLE Staff (
    StaffID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'Staff',
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE()
);

-- Tạo index cho Username để tăng tốc độ query
CREATE UNIQUE INDEX UQ_Staff_Username ON Staff(Username);

-- Tạo index cho IsActive để tối ưu query login
CREATE INDEX IX_Staff_IsActive ON Staff(IsActive);

-- In ra thông báo
PRINT 'Bảng Staff đã được tạo thành công!';
PRINT 'Bây giờ chạy script InsertDemoAccounts.sql để thêm tài khoản demo.';
