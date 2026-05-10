-- ============================================================
-- Query dữ liệu bảng Staff - Sử dụng để kiểm tra & quản lý
-- ============================================================

-- 1. XEM TẤT CẢ TÀI KHOẢN
SELECT 
    StaffID,
    Username,
    FullName,
    Role,
    IsActive,
    CreatedDate
FROM Staff
ORDER BY StaffID;

-- 2. XEM CHI TIẾT MỘT TÀI KHOẢN
-- SELECT * FROM Staff WHERE Username = 'admin';

-- 3. ĐẾM TỔNG SỐ TÀI KHOẢN
-- SELECT COUNT(*) AS 'Tổng tài khoản' FROM Staff;

-- 4. XEM CÁC TÀI KHOẢN ĐANG HOẠT ĐỘNG
-- SELECT * FROM Staff WHERE IsActive = 1;

-- 5. XEM CÁC TÀI KHOẢN KHÔNG HOẠT ĐỘNG
-- SELECT * FROM Staff WHERE IsActive = 0;

-- 6. CẬP NHẬT TÀI KHOẢN THÀNH KHÔNG HOẠT ĐỘNG
-- UPDATE Staff SET IsActive = 0 WHERE Username = 'staff';

-- 7. THAY ĐỔI MẬT KHẨU (cần hash SHA256 trước)
-- UPDATE Staff SET PasswordHash = 'HASH_MỚI' WHERE Username = 'admin';

-- 8. XÓA TÀI KHOẢN (CẤN THẬN!)
-- DELETE FROM Staff WHERE Username = 'staff';

-- 9. KIỂM TRA CẤUTRÚC BẢNG
-- EXEC sp_help 'Staff';

-- 10. XEM DUNG LƯỢNG BẢN GHI
-- SELECT 
--     COUNT(*) AS 'Số bản ghi',
--     SUM(DATALENGTH(Username) + DATALENGTH(PasswordHash) + DATALENGTH(FullName) + DATALENGTH(Role)) AS 'Dung lượng (bytes)'
-- FROM Staff;
