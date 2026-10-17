-- Run: .\Assets\sqlite3.exe Assets/SQL/Warehouse.db ".read Assets/SQL/sqlite_schema.sql"

-- =====================================================
-- DATABASE SCHEMA FOR WAREHOUSE MANAGEMENT (SQLite)
-- =====================================================

PRAGMA encoding = 'UTF-8';

-- =====================================================
-- 0. DROP DATABASE
-- =====================================================
PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS TransactionDetails;
DROP TABLE IF EXISTS InventoryCheckDetails;
DROP TABLE IF EXISTS Transactions;
DROP TABLE IF EXISTS InventoryChecks;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Actions;
DROP TABLE IF EXISTS Suppliers;
DROP TABLE IF EXISTS Customers;

PRAGMA foreign_keys = ON;

-- =====================================================
-- 1. NHÓM ĐỐI TƯỢNG
-- =====================================================

-- 1.1 Nhà cung cấp
CREATE TABLE IF NOT EXISTS Suppliers (
    SupplierID INTEGER PRIMARY KEY AUTOINCREMENT,
    SupplierName TEXT NOT NULL,
    Phone TEXT,
    Address TEXT,
    Email TEXT,
    Visible INTEGER DEFAULT 1
);

-- 1.2 Khách hàng
CREATE TABLE IF NOT EXISTS Customers (
    CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerName TEXT NOT NULL,
    Phone TEXT,
    Address TEXT,
    Email TEXT,
    Visible INTEGER DEFAULT 1
);

-- =====================================================
-- 2. NHÓM QUẢN TRỊ & HÀNG HÓA
-- =====================================================

-- 2.1 Người dùng
CREATE TABLE IF NOT EXISTS Users (
    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    FullName TEXT,
    Role TEXT DEFAULT 'Staff' CHECK (Role IN ('Admin', 'Manag', 'Staff')),
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    Visible INTEGER DEFAULT 1
);

-- 2.2 Danh mục
CREATE TABLE IF NOT EXISTS Categories (
    CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT NOT NULL,
    Description TEXT,
    Visible INTEGER DEFAULT 1
);

-- 2.3 Sản phẩm
CREATE TABLE IF NOT EXISTS Products (
    ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductName TEXT NOT NULL,
    CategoryID INTEGER,
    Price REAL DEFAULT 0,
    Quantity INTEGER DEFAULT 0,
    MinThreshold INTEGER DEFAULT 10,
    InventoryValue REAL DEFAULT 0,
    Visible INTEGER DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Categories (CategoryID)
);

-- =====================================================
-- 3. NHÓM GIAO DỊCH
-- =====================================================

-- 3.1 Phiếu Nhập/Xuất (Header)
CREATE TABLE IF NOT EXISTS Transactions (
    TransactionID INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL CHECK (Type IN ('Import', 'Export')),
    DateCreated TEXT DEFAULT (datetime('now', 'localtime')),
    CreatedByUserID INTEGER,
    SupplierID INTEGER,
    CustomerID INTEGER,
    TotalAmount REAL DEFAULT 0,
    Discount REAL DEFAULT 0,
    FinalAmount REAL DEFAULT 0,
    Note TEXT,
    Status TEXT DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Cancelled')),
    Visible INTEGER DEFAULT 1,
    FOREIGN KEY (CreatedByUserID) REFERENCES Users (UserID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers (SupplierID),
    FOREIGN KEY (CustomerID) REFERENCES Customers (CustomerID)
);

-- 3.2 Chi tiết phiếu
CREATE TABLE IF NOT EXISTS TransactionDetails (
    DetailID INTEGER PRIMARY KEY AUTOINCREMENT,
    TransactionID INTEGER,
    ProductID INTEGER,
    ProductName TEXT,
    Quantity INTEGER NOT NULL,
    UnitPrice REAL,
    DiscountRate REAL DEFAULT 0,
    SubTotal REAL GENERATED ALWAYS AS (Quantity * UnitPrice) STORED,
    Visible INTEGER DEFAULT 1,
    FOREIGN KEY (TransactionID) REFERENCES Transactions (TransactionID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products (ProductID)
);

-- =====================================================
-- 4. NHÓM KIỂM KÊ
-- =====================================================

-- 4.1 Phiếu kiểm kê
CREATE TABLE IF NOT EXISTS InventoryChecks (
    CheckID INTEGER PRIMARY KEY AUTOINCREMENT,
    CheckDate TEXT DEFAULT (datetime('now', 'localtime')),
    CreatedByUserID INTEGER,
    Status TEXT DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Completed', 'Cancelled')),
    Note TEXT,
    Visible INTEGER DEFAULT 1,
    FOREIGN KEY (CreatedByUserID) REFERENCES Users (UserID)
);

-- 4.2 Chi tiết kiểm kê
CREATE TABLE IF NOT EXISTS InventoryCheckDetails (
    DetailID INTEGER PRIMARY KEY AUTOINCREMENT,
    CheckID INTEGER,
    ProductID INTEGER,
    SystemQuantity INTEGER DEFAULT 0,
    ActualQuantity INTEGER DEFAULT 0,
    Difference INTEGER GENERATED ALWAYS AS (ActualQuantity - SystemQuantity) STORED,
    Reason TEXT,
    FOREIGN KEY (CheckID) REFERENCES InventoryChecks (CheckID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products (ProductID)
);

-- =====================================================
-- 5. NHÓM SYSTEM LOGS
-- =====================================================
CREATE TABLE IF NOT EXISTS Actions (
    LogID INTEGER PRIMARY KEY AUTOINCREMENT,
    ActionType TEXT,
    Descriptions TEXT,
    DataBefore TEXT,
    Visible INTEGER DEFAULT 1,
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime'))
);

-- =====================================================
-- 6. DEFAULT DATA
-- =====================================================
INSERT INTO Users (Username, Password, FullName, Role)
VALUES
    ('admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'QTV', 'Admin'),
    ('manag', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'QLY', 'Manag'),
    ('staff', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'NVK', 'Staff');
