# Script PowerShell để sinh hash password SHA256 Base64
# Chạy lệnh này để lấy hash mật khẩu để insert vào database

function Get-PasswordHash {
    param(
        [string]$password
    )

    $sha256 = New-Object System.Security.Cryptography.SHA256CryptoServiceProvider
    $hashBytes = $sha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($password))
    [Convert]::ToBase64String($hashBytes)
}

Write-Host "=== Password Hash Generator ===" -ForegroundColor Cyan
Write-Host ""

# Tính hash cho các mật khẩu demo
$passwords = @("admin123", "staff123", "test123")

foreach ($pwd in $passwords) {
    $hash = Get-PasswordHash -password $pwd
    Write-Host "Password: $pwd" -ForegroundColor Green
    Write-Host "Hash:     $hash" -ForegroundColor Yellow
    Write-Host ""
}

Write-Host "=== Sao chép hash vào SQL script ==="
