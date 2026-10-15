USE Warehouse;

-- Disable foreign key checks so tables can be truncated in any order.
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE TransactionDetails;
TRUNCATE TABLE Transactions;
TRUNCATE TABLE InventoryCheckDetails;
TRUNCATE TABLE InventoryChecks;
TRUNCATE TABLE Suppliers;
TRUNCATE TABLE Customers;
TRUNCATE TABLE Products;
TRUNCATE TABLE Categories;
TRUNCATE TABLE Users;

SET FOREIGN_KEY_CHECKS = 1;

-- Default users; required before inserting Transactions (CreatedByUserID references Users).
INSERT INTO Users (Username, Password, FullName, Role)
VALUES
    ('admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'QTV', 'Admin'),
    ('manager', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'QLY', 'Manag'),
    ('staff', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'NVK', 'Staff');

-- Seed suppliers (used as partners for import transactions).
INSERT INTO Suppliers (
    SupplierName,
    Phone,
    Address,
    Email
) VALUES
    ('Apple Distribution VN',          '0909123456',   'Q1, TP.HCM',                 'contact@apple.vn'),
    ('Samsung Vina',                   '0909888999',   'KCN Yên Bình, Thái Nguyên',  'sales@samsung.com'),
    ('Công ty May Việt Tiến',          '02838640800',  'Tân Bình, TP.HCM',           'viettien@corp.vn'),
    ('Honda Việt Nam',                 '18008001',     'Phúc Thắng, Vĩnh Phúc',      'cr@honda.com.vn'),
    ('Nhà Phân Phối Digiworld',        '02839290059',  'Q3, TP.HCM',                 'support@dgw.com.vn');

-- Seed customers (used as partners for export transactions).
INSERT INTO Customers (
    CustomerName,
    Phone,
    Address,
    Email
) VALUES
    ('Nguyễn Văn A',            '0912345678', 'Ba Đình, Hà Nội',        'anv@gmail.com'),
    ('Trần Thị B',              '0987654321', 'Cầu Giấy, Hà Nội',       'btran@yahoo.com'),
    ('Công ty TNHH ABC',        '0243777888', 'Đống Đa, Hà Nội',        'admin@abc.com'),
    ('Nguyễn Văn An',           '0912345678', 'Ba Đình, Hà Nội',        'anv@gmail.com'),
    ('Trần Thị Bích',           '0987654321', 'Cầu Giấy, Hà Nội',       'btran@yahoo.com'),
    ('Công ty TNHH TM&DV ABC',  '0243777888', 'Đống Đa, Hà Nội',        'admin@abc.com'),
    ('Lê Hoàng Nam',            '0933444555', 'Hải Châu, Đà Nẵng',      'namlh@outlook.com');

-- Seed product categories (referenced by Products).
INSERT INTO Categories (CategoryName, Description) VALUES
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

-- Seed products (initial price, quantity, min threshold; InventoryValue updated later by StockUpdate).
INSERT INTO Products (
    ProductName,
    CategoryID,
    Price,
    Quantity,
    MinThreshold,
    InventoryValue
) VALUES
    ('iPhone 15 Pro Max 1TB',              1,  45000000,   999,   5,  0),
    ('Samsung Galaxy S24 Ultra',           1,  30000000,   999,   5,  0),
    ('Xiaomi 14 Ultra',                    1,  25000000,   999,   5,  0),
    ('Ốp lưng MagSafe Cao cấp',            1,    500000,   999,  20,  0),
    ('Sạc dự phòng Anker 20000mAh',        1,   1200000,   999,  10,  0),

    ('MacBook Pro 16 inch M3 Max',         2,  90000000,   999,   2,  0),
    ('Dell XPS 15 9530',                   2,  55000000,   999,   3,  0),
    ('Màn hình LG UltraGear 27"',          2,   8500000,   999,   5,  0),
    ('Bàn phím cơ Keychron Q1',            2,   4500000,   999,   5,  0),
    ('Chuột Logitech MX Master 3S',        2,   2500000,   999,  10,  0),

    ('Tủ lạnh Hitachi Inverter 569L',      3,  35000000,   999,   2,  0),
    ('Máy giặt Electrolux 10kg',           3,  12000000,   999,   5,  0),
    ('Robot hút bụi Roborock S8',          3,  18000000,   999,   5,  0),
    ('Nồi chiên không dầu Philips',        3,   3500000,   999,  10,  0),
    ('Máy lọc không khí Dyson',            3,  15000000,   999,   5,  0),

    ('Áo sơ mi Pierre Cardin',             4,   1500000,   999,  20,  0),
    ('Quần âu Việt Tiến',                  4,    800000,   999,  20,  0),
    ('Giày da nam Bitis Hunter',           4,   1200000,   999,  15,  0),
    ('Thắt lưng da cá sấu thật',           4,   5000000,   999,   5,  0),
    ('Đồng hồ Rolex Submariner Date',      4, 350000000,   999,   1,  0),

    ('Đầm dạ hội thiết kế',                 5,   5000000,  999,   5,  0),
    ('Túi xách Hermes Birkin 30',           5, 850000000,  999,   1,  0),
    ('Giày cao gót Louboutin',              5,  25000000,  999,   2,  0),
    ('Áo dài lụa tơ tằm',                   5,   8000000,  999,   5,  0),
    ('Kính mát Gucci chính hãng',           5,  10000000,  999,   5,  0),

    ('Son môi Tom Ford',                    6,   1500000,  999,  20,  0),
    ('Nước hoa Chanel No.5 100ml',          6,   4500000,  999,  10,  0),
    ('Kem dưỡng ẩm La Mer',                 6,  12000000,  999,   5,  0),
    ('Máy rửa mặt Foreo Luna 4',            6,   5000000,  999,  10,  0),
    ('Bộ trang điểm chuyên nghiệp MAC',     6,   8000000,  999,   5,  0),

    ('Bộ sách Harry Potter (Full)',         7,   3000000,  999,   5,  0),
    ('Bút ký Parker Sonnet',                7,   5000000,  999,   5,  0),
    ('Máy tính Casio FX-580VN X',           7,    800000,  999,  50,  0),
    ('Sổ tay Moleskine Classic',            7,   1200000,  999,  20,  0),
    ('Bàn cắt giấy công nghiệp A3',         7,   2500000,  999,   3,  0),

    ('Máy chạy bộ Kingsport BK',            8,  15000000,  999,   2,  0),
    ('Xe đạp địa hình Giant ATX',           8,  12000000,  999,   3,  0),
    ('Lều cắm trại 4 người Naturehike',     8,   3500000,  999,   5,  0),
    ('Vợt Tennis Wilson Pro Staff',         8,   4500000,  999,   5,  0),
    ('Vali Samsonite size L',               8,  10000000,  999,   5,  0),

    ('Xe đẩy em bé Combi',                  9,  12000000,  999,   3,  0),
    ('Máy hút sữa Medela Pump',             9,   8000000,  999,   5,  0),
    ('Bộ LEGO Technic Bugatti Chiron',      9,  10000000,  999,   2,  0),
    ('Ghế ngồi ô tô cho bé Joie',           9,   6000000,  999,   5,  0),
    ('Nhà banh cầu trượt liên hoàn',        9,   2500000,  999,   5,  0),

    ('Xe máy Honda SH 350i',               10, 150000000,  999,   1,  0),
    ('Xe mô tô Ducati Panigale V4',        10, 890000000,  999,   1,  0),
    ('Mũ bảo hiểm AGV Pista GP',           10,  35000000,  999,   2,  0),
    ('Lốp xe Michelin Pilot Road 6',       10,   4500000,  999,  10,  0),
    ('Camera hành trình Vietmap SpeedMap', 10,   5000000,  999,  10,  0);

-- Allow explicit TransactionID (e.g. 0) so the first approved import can use ID 0.
SET SESSION sql_mode = CONCAT(@@sql_mode, ',NO_AUTO_VALUE_ON_ZERO');

-- First import voucher (ID 0): Approved; used as the single source for initial stock in this seed.
INSERT INTO Transactions (TransactionID, Type, DateCreated, CreatedByUserID, SupplierID, Note, TotalAmount, Discount, FinalAmount, Status)
VALUES (0, 'Import', '2026-03-02 16:00:00', 1, 1, 'Nhập hàng lần đầu tiên', 0, 0, 0, 'Approved');

-- One detail row per product for transaction 0 (random quantity and discount for demo).
INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice, DiscountRate)
SELECT 0, ProductID, ProductName, FLOOR(80 + RAND() * 21), Price, FLOOR(5 + RAND() * 11)
FROM Products;

-- Recompute header totals and final amount for transaction 0.
UPDATE Transactions
SET
    TotalAmount = (SELECT COALESCE(SUM(Quantity * UnitPrice), 0) FROM TransactionDetails WHERE TransactionID = 0),
    Discount = (SELECT COALESCE(SUM(Quantity * UnitPrice * DiscountRate / 100), 0) FROM TransactionDetails WHERE TransactionID = 0)
WHERE TransactionID = 0;
UPDATE Transactions SET FinalAmount = TotalAmount - Discount WHERE TransactionID = 0;

-- Next transaction will get ID 1; following procedure creates 200 more (IDs 1..200).
ALTER TABLE Transactions AUTO_INCREMENT = 1;

-- Procedure: generate 200 transactions (alternating Import/Export), each with 3 random product lines; all Pending.
DROP PROCEDURE IF EXISTS GenerateTransactions;

DELIMITER $$

CREATE PROCEDURE GenerateTransactions()
BEGIN
    DECLARE i INT DEFAULT 0;
    DECLARE v_date DATETIME;
    DECLARE v_type VARCHAR(10);
    DECLARE v_note VARCHAR(100);
    DECLARE v_user INT;
    DECLARE v_tid INT;
    DECLARE v_supplier INT;
    DECLARE v_customer INT;
    DECLARE v_total DECIMAL(18,2);
    DECLARE v_discount DECIMAL(18,2);

    DECLARE v_rate DECIMAL(5,2);
    DECLARE v_end_date DATETIME DEFAULT '2026-03-02 16:00:00';

    WHILE i < 200 DO
        SET v_date = DATE_SUB(v_end_date, INTERVAL ((199 - i) DIV 5) * 24 + ((199 - i) MOD 5) * 2 HOUR);

        SET v_supplier = NULL;
        SET v_customer = NULL;

        SET v_rate = FLOOR(5 + RAND() * 11);

        IF i % 2 = 0 THEN
            SET v_type = 'Import';
            SET v_note = CONCAT('Phiếu nhập tự động #', i + 1);
            SET v_user = 1;
            SET v_supplier = FLOOR(1 + RAND() * 5);
        ELSE
            SET v_type = 'Export';
            SET v_note = CONCAT('Phiếu xuất tự động #', i + 1);
            SET v_user = 2;
            SET v_customer = FLOOR(1 + RAND() * 4);
        END IF;

        INSERT INTO Transactions (Type, DateCreated, CreatedByUserID, Note, SupplierID, CustomerID, TotalAmount, Discount, FinalAmount)
        VALUES (v_type, v_date, v_user, v_note, v_supplier, v_customer, 0, 0, 0);
        SET v_tid = LAST_INSERT_ID();

        INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice, DiscountRate)
        SELECT v_tid, ProductID, ProductName, FLOOR(5 + RAND() * 20), Price, v_rate
        FROM Products
        ORDER BY RAND()
        LIMIT 3;

        SELECT
            COALESCE(SUM(Quantity * UnitPrice), 0),
            COALESCE(SUM(Quantity * UnitPrice * DiscountRate / 100), 0)
        INTO v_total, v_discount
        FROM TransactionDetails
        WHERE TransactionID = v_tid;

        UPDATE Transactions
        SET TotalAmount = v_total,
            Discount = v_discount,
            FinalAmount = v_total - v_discount
        WHERE TransactionID = v_tid;

        SET i = i + 1;
    END WHILE;
END$$

DELIMITER;

CALL GenerateTransactions();

-- Procedure: set product Quantity from approved transaction 0 details and recompute InventoryValue.
DROP PROCEDURE IF EXISTS StockUpdate;

DELIMITER $$

CREATE PROCEDURE StockUpdate()
BEGIN
    SET SQL_SAFE_UPDATES = 0;

    UPDATE Products SET Quantity = 0;

    UPDATE Products p
    SET p.Quantity = (
        SELECT COALESCE(td.Quantity, 0)
        FROM TransactionDetails td
        WHERE td.TransactionID = 0 AND td.ProductID = p.ProductID
    );

    UPDATE Products SET InventoryValue = Quantity * Price;

    SET SQL_SAFE_UPDATES = 1;
END$$

DELIMITER;

CALL StockUpdate();

-- Procedure: seed two completed inventory checks with sample detail rows (system = actual for demo).
DROP PROCEDURE IF EXISTS SeedInventoryChecks;

DELIMITER $$

CREATE PROCEDURE SeedInventoryChecks()
BEGIN
    DECLARE v_checkID INT;
    INSERT INTO InventoryChecks (CheckDate, CreatedByUserID, Status, Note)
    VALUES ('2026-2-15 17:00:00', 1, 'Completed', 'Kiểm kê chốt sổ cuối năm 2025');

    SET v_checkID = LAST_INSERT_ID();

    INSERT INTO InventoryCheckDetails (CheckID, ProductID, SystemQuantity, ActualQuantity, Reason)
    SELECT v_checkID, ProductID, Quantity, Quantity, 'Khớp số liệu cuối năm'
    FROM Products ORDER BY RAND() LIMIT 5;

    INSERT INTO InventoryChecks (CheckDate, CreatedByUserID, Status, Note)
    VALUES ('2026-02-28 08:00:00', 1, 'Completed', 'Kiểm kê định kỳ Tháng 01/2026');

    SET v_checkID = LAST_INSERT_ID();

    INSERT INTO InventoryCheckDetails (CheckID, ProductID, SystemQuantity, ActualQuantity, Reason)
    SELECT v_checkID, ProductID, Quantity, Quantity, 'Khớp số liệu định kỳ'
    FROM Products ORDER BY RAND() LIMIT 10;

END$$

DELIMITER;

CALL SeedInventoryChecks();
