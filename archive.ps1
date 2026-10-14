# .\archive.ps1

param(
    [string]$OutputDir = ".release"
)

$ErrorActionPreference = "Stop"

Write-Host "=== WarehouseManagement Release Packaging ==="

# Resolve root path (script directory)
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $root

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

$nosqlSource = Join-Path $root "nosql version\Build\bin\Release\Warehouse Management"
$mysqlSource = Join-Path $root "mysql version\Build\bin\Release\Warehouse Management"

if (-not (Test-Path $nosqlSource)) {
    Write-Error "Không tìm thấy thư mục build NoSQL: '$nosqlSource'. Vui lòng build bản NoSQL ở cấu hình Release trước."
}
if (-not (Test-Path $mysqlSource)) {
    Write-Error "Không tìm thấy thư mục build MySQL: '$mysqlSource'. Vui lòng build bản MySQL ở cấu hình Release trước."
}

$nosqlZipName = "Warehouse Management NoSQL.zip"
$mysqlZipName = "Warehouse Management MySQL.zip"

$nosqlZipPath = Join-Path $OutputDir $nosqlZipName
$mysqlZipPath = Join-Path $OutputDir $mysqlZipName

$nosqlTempDir = Join-Path $OutputDir "Warehouse Management NoSQL"
$mysqlTempDir = Join-Path $OutputDir "Warehouse Management MySQL"

# Xoá file cũ nếu tồn tại
if (Test-Path $nosqlZipPath) { Remove-Item $nosqlZipPath -Force }
if (Test-Path $mysqlZipPath) { Remove-Item $mysqlZipPath -Force }

# Xoá thư mục tạm nếu tồn tại
if (Test-Path $nosqlTempDir) { Remove-Item $nosqlTempDir -Recurse -Force }
if (Test-Path $mysqlTempDir) { Remove-Item $mysqlTempDir -Recurse -Force }

# Sao chép thư mục build sang .release với tên mong muốn
Write-Host "Đang sao chép NoSQL build -> $nosqlTempDir"
Copy-Item -Path $nosqlSource -Destination $nosqlTempDir -Recurse

Write-Host "Đang sao chép MySQL build -> $mysqlTempDir"
Copy-Item -Path $mysqlSource -Destination $mysqlTempDir -Recurse

Write-Host "Đang nén NoSQL version -> $nosqlZipPath"
Compress-Archive -Path (Join-Path $nosqlTempDir '*') -DestinationPath $nosqlZipPath -Force

Write-Host "Đang nén MySQL version -> $mysqlZipPath"
Compress-Archive -Path (Join-Path $mysqlTempDir '*') -DestinationPath $mysqlZipPath -Force

# Tuỳ chọn: giữ lại thư mục tạm hoặc xoá đi sau khi nén
# Xoá để thư mục .release chỉ còn file zip
# Remove-Item $nosqlTempDir -Recurse -Force
# Remove-Item $mysqlTempDir -Recurse -Force

Write-Host "Hoàn tất. File được lưu tại:"
Write-Host " - $nosqlZipPath"
Write-Host " - $mysqlZipPath"

