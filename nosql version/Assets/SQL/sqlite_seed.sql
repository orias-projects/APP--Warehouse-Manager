-- Run: .\Assets\sqlite3.exe Assets/SQL/Warehouse.db ".read Assets/SQL/sqlite_seed.sql"

-- Disable foreign keys so tables can be cleared in any order.
PRAGMA foreign_keys = OFF;

DELETE FROM TransactionDetails;
DELETE FROM Transactions;
DELETE FROM InventoryCheckDetails;
DELETE FROM InventoryChecks;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Suppliers;
DELETE FROM Customers;
DELETE FROM Users;

DELETE FROM sqlite_sequence WHERE name = 'TransactionDetails';
DELETE FROM sqlite_sequence WHERE name = 'Transactions';
DELETE FROM sqlite_sequence WHERE name = 'InventoryCheckDetails';
DELETE FROM sqlite_sequence WHERE name = 'InventoryChecks';
DELETE FROM sqlite_sequence WHERE name = 'Products';
DELETE FROM sqlite_sequence WHERE name = 'Categories';
DELETE FROM sqlite_sequence WHERE name = 'Suppliers';
DELETE FROM sqlite_sequence WHERE name = 'Customers';
DELETE FROM sqlite_sequence WHERE name = 'Users';

PRAGMA foreign_keys = ON;

-- Default users; required before Transactions (CreatedByUserID references Users).
INSERT INTO Users (Username, Password, FullName, Role)
VALUES
    ('admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'QTV', 'Admin'),
    ('manag', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'QLY', 'Manag'),
    ('staff', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'NVK', 'Staff');

-- Seed suppliers (partners for import transactions).
INSERT INTO Suppliers (
    SupplierName,
    Phone,
    Address,
    Email
) VALUES
    ('Apple Distribution VN',            '0909123456',   'Q1, TP.HCM',               'contact@apple.vn'),
    ('Samsung Vina',                     '0909888999',   'KCN Yên Bình, Thái Nguyên','sales@samsung.com'),
    ('Công ty May Việt Tiến',            '02838640800',  'Tân Bình, TP.HCM',         'viettien@corp.vn'),
    ('Honda Việt Nam',                   '18008001',     'Phúc Thắng, Vĩnh Phúc',    'cr@honda.com.vn'),
    ('Nhà Phân Phối Digiworld',          '02839290059',  'Q3, TP.HCM',               'support@dgw.com.vn');

-- Seed customers (partners for export transactions).
INSERT INTO Customers (
    CustomerName,
    Phone,
    Address,
    Email
) VALUES
    ('Nguyễn Văn A',                 '0912345678',  'Ba Đình, Hà Nội',         'anv@gmail.com'),
    ('Trần Thị B',                   '0987654321',  'Cầu Giấy, Hà Nội',        'btran@yahoo.com'),
    ('Công ty TNHH ABC',             '0243777888',  'Đống Đa, Hà Nội',         'admin@abc.com'),
    ('Nguyễn Văn An',                '0912345678',  'Ba Đình, Hà Nội',         'anv@gmail.com'),
    ('Trần Thị Bích',                '0987654321',  'Cầu Giấy, Hà Nội',        'btran@yahoo.com'),
    ('Công ty TNHH Thương Mại ABC',  '0243777888',  'Đống Đa, Hà Nội',         'admin@abc.com'),
    ('Lê Hoàng Nam',                 '0933444555',  'Hải Châu, Đà Nẵng',       'namlh@outlook.com');

-- Seed product categories (referenced by Products).
INSERT INTO Categories (
    CategoryName,
    Description
) VALUES
    ('Điện thoại & Phụ kiện',   'Điện thoại, phụ kiện'),
    ('Máy tính & Laptop',       'Máy tính, laptop, ngoại vi'),
    ('Thiết bị gia dụng',       'Đồ gia dụng'),
    ('Thời trang Nam',          'Thời trang nam'),
    ('Thời trang Nữ',           'Thời trang nữ'),
    ('Mỹ phẩm & Làm đẹp',       'Mỹ phẩm, làm đẹp'),
    ('Sách & Văn phòng phẩm',   'Sách, văn phòng phẩm'),
    ('Thể thao & Du lịch',      'Thể thao, du lịch'),
    ('Đồ chơi & Mẹ bé',         'Đồ chơi, mẹ và bé'),
    ('Ô tô & Xe máy',           'Ô tô, xe máy');

-- Seed products (50 items; InventoryValue updated later from approved transaction 0).
INSERT INTO Products (
    ProductName,
    CategoryID,
    Price,
    Quantity,
    MinThreshold,
    InventoryValue
) VALUES
    ('iPhone 15 Pro Max 1TB',                 1,  45000000,  999,  5,  0),
    ('Samsung Galaxy S24 Ultra',              1,  30000000,  999,  5,  0),
    ('Xiaomi 14 Ultra',                       1,  25000000,  999,  5,  0),
    ('Ốp lưng MagSafe Cao cấp',               1,   500000,   999, 20,  0),
    ('Sạc dự phòng Anker 20000mAh',           1,  1200000,   999, 10,  0),
    ('MacBook Pro 16 inch M3 Max',            2,  90000000,  999,  2,  0),
    ('Dell XPS 15 9530',                      2,  55000000,  999,  3,  0),
    ('Màn hình LG UltraGear 27"',             2,   8500000,  999,  5,  0),
    ('Bàn phím cơ Keychron Q1',               2,   4500000,  999,  5,  0),
    ('Chuột Logitech MX Master 3S',           2,   2500000,  999, 10,  0),
    ('Tủ lạnh Hitachi Inverter 569L',         3,  35000000,  999,  2,  0),
    ('Máy giặt Electrolux 10kg',              3,  12000000,  999,  5,  0),
    ('Robot hút bụi Roborock S8',             3,  18000000,  999,  5,  0),
    ('Nồi chiên không dầu Philips',           3,   3500000,  999, 10,  0),
    ('Máy lọc không khí Dyson',               3,  15000000,  999,  5,  0),
    ('Áo sơ mi Pierre Cardin',                4,   1500000,  999, 20,  0),
    ('Quần âu Việt Tiến',                     4,    800000,  999, 20,  0),
    ('Giày da nam Bitis Hunter',              4,   1200000,  999, 15,  0),
    ('Thắt lưng da cá sấu thật',              4,   5000000,  999,  5,  0),
    ('Đồng hồ Rolex Submariner Date',         4, 350000000,  999,  1,  0),
    ('Đầm dạ hội thiết kế',                   5,   5000000,  999,  5,  0),
    ('Túi xách Hermes Birkin 30',             5, 850000000,  999,  1,  0),
    ('Giày cao gót Louboutin',                5,  25000000,  999,  2,  0),
    ('Áo dài lụa tơ tằm',                     5,   8000000,  999,  5,  0),
    ('Kính mát Gucci chính hãng',             5,  10000000,  999,  5,  0),
    ('Son môi Tom Ford',                      6,   1500000,  999, 20,  0),
    ('Nước hoa Chanel No.5 100ml',            6,   4500000,  999, 10,  0),
    ('Kem dưỡng ẩm La Mer',                   6,  12000000,  999,  5,  0),
    ('Máy rửa mặt Foreo Luna 4',              6,   5000000,  999, 10,  0),
    ('Bộ trang điểm chuyên nghiệp MAC',       6,   8000000,  999,  5,  0),
    ('Bộ sách Harry Potter (Full)',           7,   3000000,  999,  5,  0),
    ('Bút ký Parker Sonnet',                  7,   5000000,  999,  5,  0),
    ('Máy tính Casio FX-580VN X',             7,    800000,  999, 50,  0),
    ('Sổ tay Moleskine Classic',              7,   1200000,  999, 20,  0),
    ('Bàn cắt giấy công nghiệp A3',           7,   2500000,  999,  3,  0),
    ('Máy chạy bộ Kingsport BK',              8,  15000000,  999,  2,  0),
    ('Xe đạp địa hình Giant ATX',             8,  12000000,  999,  3,  0),
    ('Lều cắm trại 4 người Naturehike',       8,   3500000,  999,  5,  0),
    ('Vợt Tennis Wilson Pro Staff',           8,   4500000,  999,  5,  0),
    ('Vali Samsonite size L',                 8,  10000000,  999,  5,  0),
    ('Xe đẩy em bé Combi',                    9,  12000000,  999,  3,  0),
    ('Máy hút sữa Medela Pump',               9,   8000000,  999,  5,  0),
    ('Bộ LEGO Technic Bugatti Chiron',        9,  10000000,  999,  2,  0),
    ('Ghế ngồi ô tô cho bé Joie',             9,   6000000,  999,  5,  0),
    ('Nhà banh cầu trượt liên hoàn',          9,   2500000,  999,  5,  0),
    ('Xe máy Honda SH 350i',                 10, 150000000,  999,  1,  0),
    ('Xe mô tô Ducati Panigale V4',          10, 890000000,  999,  1,  0),
    ('Mũ bảo hiểm AGV Pista GP',             10,  35000000,  999,  2,  0),
    ('Lốp xe Michelin Pilot Road 6',         10,   4500000,  999, 10,  0),
    ('Camera hành trình Vietmap SpeedMap',   10,   5000000,  999, 10,  0);

-- First import voucher (TransactionID 0): Approved; one detail row per product (qty 80–100, random discount); used as initial stock source.
INSERT INTO Transactions (TransactionID, Type, DateCreated, CreatedByUserID, SupplierID, CustomerID, Note, TotalAmount, Discount, FinalAmount, Status)
VALUES (
    0, 'Import', datetime('2026-03-02 16:00:00', '-968 hours'), 1, 1, NULL, 'Nhập hàng lần đầu tiên', 0, 0, 0, 'Approved'
);
INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice, DiscountRate)
SELECT 0, p.ProductID, p.ProductName, 80 + abs(random() % 21), p.Price, 5 + abs(random() % 11)
FROM Products p;

-- Insert 200 transaction headers (IDs 1..200): alternating Import/Export, dates stepping back from anchor; all Pending.
INSERT INTO Transactions (Type, DateCreated, CreatedByUserID, SupplierID, CustomerID, Note, TotalAmount, Discount, FinalAmount)
WITH nums(n) AS (
    SELECT 0 UNION ALL SELECT n+1 FROM nums WHERE n < 199
)
SELECT
    CASE WHEN n % 2 = 0 THEN 'Import' ELSE 'Export' END,
    datetime('2026-03-02 16:00:00', '-'||(((199-n)/5)*24 + ((199-n)%5)*2)||' hours'),
    CASE WHEN n % 2 = 0 THEN 1 ELSE 2 END,
    CASE WHEN n % 2 = 0 THEN 1 + abs(random() % 5) ELSE NULL END,
    CASE WHEN n % 2 = 1 THEN 1 + abs(random() % 4) ELSE NULL END,
    'Phiếu '||CASE WHEN n % 2 = 0 THEN 'nhập' ELSE 'xuất' END||' tự động #'||(n+1),
    0, 0, 0
FROM nums;

-- Insert 600 detail rows for transactions 1..200 (3 lines per transaction; random product, qty, discount).
WITH nums(n) AS (
    SELECT 0 UNION ALL SELECT n+1 FROM nums WHERE n < 599
),
rp AS (
    SELECT n, (1 + abs(random() % 50)) AS pid FROM nums
)
INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice, DiscountRate)
SELECT
    1 + (rp.n / 3),
    rp.pid,
    (SELECT ProductName FROM Products WHERE ProductID = rp.pid),
    CASE WHEN (1 + (rp.n / 3)) % 2 = 1 THEN 10 + abs(random() % 11) ELSE 5 + abs(random() % 16) END,
    (SELECT Price FROM Products WHERE ProductID = rp.pid),
    5 + abs(random() % 11)
FROM rp;

-- Recompute TotalAmount, Discount and FinalAmount for every transaction.
UPDATE Transactions
SET
    TotalAmount = (SELECT COALESCE(SUM(Quantity * UnitPrice), 0) FROM TransactionDetails WHERE TransactionID = Transactions.TransactionID),
    Discount = (SELECT COALESCE(SUM(Quantity * UnitPrice * DiscountRate / 100), 0) FROM TransactionDetails WHERE TransactionID = Transactions.TransactionID);
UPDATE Transactions
SET FinalAmount = TotalAmount - Discount;

-- Set product Quantity from approved transaction 0 only; then recompute InventoryValue. Transactions 1..200 remain Pending and do not affect stock here.
UPDATE Products SET Quantity = 0;

UPDATE Products
SET Quantity = (SELECT COALESCE(td.Quantity, 0) FROM TransactionDetails td WHERE td.TransactionID = 0 AND td.ProductID = Products.ProductID);

UPDATE Products SET InventoryValue = Quantity * Price;

-- Seed two completed inventory checks with sample detail rows (system = actual for demo).
INSERT INTO InventoryChecks (CheckDate, CreatedByUserID, Status, Note)
VALUES ('2026-02-15 17:00:00', 1, 'Completed', 'Kiểm kê giữa tháng 02/2026');

INSERT INTO InventoryCheckDetails (CheckID, ProductID, SystemQuantity, ActualQuantity, Reason)
SELECT (SELECT max(CheckID) FROM InventoryChecks), ProductID, Quantity, Quantity, 'Kiểm kê giữa tháng 02/2026'
FROM Products ORDER BY random() LIMIT 5;

INSERT INTO InventoryChecks (CheckDate, CreatedByUserID, Status, Note)
VALUES ('2026-02-28 08:00:00', 1, 'Completed', 'Kiểm kê cuối tháng 02/2026');

INSERT INTO InventoryCheckDetails (CheckID, ProductID, SystemQuantity, ActualQuantity, Reason)
SELECT (SELECT max(CheckID) FROM InventoryChecks), ProductID, Quantity, Quantity, 'Kiểm kê cuối tháng 02/2026'
FROM Products ORDER BY random() LIMIT 10;
