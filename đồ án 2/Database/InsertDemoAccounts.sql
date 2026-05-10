-- ============================================================
-- Script thêm tài khoản demo vào bảng Staff
-- 
-- CẬP NHẬT: Tạo bảng Staff TRƯỚC khi chạy script này!
-- Thứ tự:
-- 1. Chạy CreateStaffTable.sql (tạo bảng)
-- 2. Chạy InsertDemoAccounts.sql (thêm dữ liệu)
-- ============================================================

-- Kiểm tra xem bảng Staff tồn tại không
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff')
BEGIN
    PRINT 'Lỗi: Bảng Staff chưa được tạo!';
    PRINT 'Vui lòng chạy CreateStaffTable.sql TRƯỚC.';
    RETURN;
END;

-- Xóa tài khoản demo nếu đã tồn tại (để insert mới)
DELETE FROM Staff WHERE Username IN ('admin', 'staff');

-- ============================================================
-- THÊM TÀI KHOẢN DEMO
-- ============================================================

-- Tài khoản Admin
-- Username: admin
-- Mật khẩu: admin123
-- Hash SHA256: JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=
INSERT INTO Staff (Username, PasswordHash, FullName, Role, IsActive)
VALUES ('admin', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', 'Admin User', 'Admin', 1);

-- Tài khoản Nhân viên
-- Username: staff
-- Mật khẩu: staff123
-- Hash SHA256: EBdue3sk0xes/PjSBkz9LyThVPe1qWYDB31e+BPWprY=
INSERT INTO Staff (Username, PasswordHash, FullName, Role, IsActive)
VALUES ('staff', 'EBdue3sk0xes/PjSBkz9LyThVPe1qWYDB31e+BPWprY=', 'Staff User', 'Staff', 1);

PRINT 'Đã thêm 2 tài khoản demo thành công!';
PRINT '';
PRINT '=== THÔNG TIN TÀI KHOẢN ===';
PRINT 'Admin:  admin / admin123';
PRINT 'Staff:  staff / staff123';
PRINT '';
PRINT 'Chạy lệnh này để kiểm tra dữ liệu:';
PRINT 'SELECT * FROM Staff;';
