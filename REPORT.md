# BÁO CÁO ĐỒ ÁN: ỨNG DỤNG QUẢN LÝ KHO HÀNG (WAREHOUSE MANAGEMENT)

**Phiên bản:** NoSQL (SQLite) v1.0.2  
**Ngôn ngữ:** C# (.NET Framework 4.7.2 – Windows Forms)  
**Cơ sở dữ liệu:** SQLite 3  

---

## CHƯƠNG 1. GIỚI THIỆU CHUNG

### 1.1. Lý do chọn đề tài

- **Vai trò thực tiễn:** Quản lý kho hàng là nghiệp vụ cốt lõi trong mọi doanh nghiệp sản xuất, thương mại. Việc theo dõi nhập – xuất – tồn kho thủ công dễ phát sinh sai sót, mất thời gian và khó truy vết khi xảy ra chênh lệch.
- **Nhu cầu tin học hóa:** Ứng dụng quản lý kho giúp tự động hóa các quy trình nhập/xuất, kiểm kê, lập phiếu, thống kê báo cáo, từ đó nâng cao hiệu quả vận hành.
- **Ý nghĩa học thuật:** Đề tài cho phép thực hành toàn diện các kỹ năng: phân tích yêu cầu, thiết kế CSDL, xây dựng kiến trúc phần mềm phân lớp, lập trình giao diện WinForms, và kiểm thử phần mềm.

### 1.2. Mục tiêu của đề tài

- Xây dựng ứng dụng desktop quản lý kho hàng hoàn chỉnh với đầy đủ chức năng CRUD (Create – Read – Update – Delete).
- Giao diện người dùng (UI) hiện đại, hỗ trợ Light/Dark theme, custom components tự thiết kế.
- Cơ sở dữ liệu SQLite nhúng (embedded), không cần cài đặt server riêng.
- Hệ thống phân quyền 3 cấp (Admin – Manager – Staff) đảm bảo an toàn dữ liệu.
- Hỗ trợ in phiếu nhập/xuất, phiếu kiểm kê; xuất báo cáo Excel/PDF; biểu đồ thống kê trực quan.
- Cơ chế Undo hành động và nhật ký hoạt động (Action Log) để truy vết thay đổi.

### 1.3. Phạm vi và đối tượng nghiên cứu

- **Đối tượng quản lý:** Sản phẩm (Products), Danh mục (Categories), Nhà cung cấp (Suppliers), Khách hàng (Customers), Phiếu nhập/xuất kho (Transactions), Phiếu kiểm kê (Inventory Checks), Người dùng (Users), Nhật ký hành động (Actions).
- **Phạm vi chức năng:**
  - Quản lý danh mục, sản phẩm, NCC, khách hàng (CRUD + tìm kiếm + soft delete/restore)
  - Lập phiếu nhập/xuất kho (với chi tiết dòng sản phẩm, chiết khấu, tính tổng tự động)
  - Quy trình duyệt phiếu (Pending → Approved/Cancelled) có cập nhật tồn kho tự động
  - Kiểm kê kho (so sánh tồn hệ thống – tồn thực tế, điều chỉnh chênh lệch)
  - Dashboard thống kê tổng quan, biểu đồ nhập/xuất theo ngày/tháng
  - Phân quyền người dùng, đăng nhập xác thực SHA-256
  - In phiếu và xuất báo cáo (Excel qua ClosedXML, PDF qua iTextSharp)

### 1.4. Công cụ và công nghệ sử dụng


| Thành phần         | Công nghệ / Thư viện     | Phiên bản |
| ------------------ | ------------------------ | --------- |
| Ngôn ngữ lập trình | C# (LangVersion 8.0)     | —         |
| Framework          | .NET Framework           | 4.7.2     |
| Giao diện          | Windows Forms (WinForms) | —         |
| Cơ sở dữ liệu      | SQLite (embedded)        | 3         |
| Kết nối DB         | System.Data.SQLite.Core  | 1.0.118   |
| Xuất Excel         | ClosedXML                | 0.105.0   |
| Xuất PDF           | iTextSharp               | 5.5.13.5  |
| Biểu đồ            | OxyPlot.WindowsForms     | 2.2.0     |
| Icon               | FontAwesome.Sharp        | 6.3.0     |
| Xử lý JSON         | Newtonsoft.Json          | 13.0.3    |
| IDE                | Visual Studio / Cursor   | —         |
| Quản lý mã nguồn   | Git                      | —         |


---

## CHƯƠNG 2. PHÂN TÍCH YÊU CẦU HỆ THỐNG

### 2.1. Khảo sát và mô tả bài toán

**Quy trình quản lý kho thủ công hiện tại:**

- Nhân viên kho ghi chép nhập/xuất bằng sổ sách hoặc file Excel rời rạc.
- Kiểm kê thủ công theo định kỳ, khó phát hiện chênh lệch kịp thời.
- Không có cơ chế phân quyền: mọi người đều có thể sửa đổi dữ liệu.
- Báo cáo thống kê phải tổng hợp thủ công, mất nhiều thời gian và dễ sai sót.

**Khó khăn khi chưa có phần mềm:**

- Sai sót trong ghi chép số lượng nhập/xuất.
- Không kiểm soát được tồn kho tối thiểu (MinThreshold), dẫn đến hết hàng.
- Khó truy vết nguồn gốc lỗi khi phát hiện chênh lệch.
- Không có lịch sử thay đổi (audit trail) để đối chiếu.

### 2.2. Các chức năng của hệ thống


| STT | Chức năng                  | Mô tả                                                                                           |
| --- | -------------------------- | ----------------------------------------------------------------------------------------------- |
| 1   | Đăng nhập / Phân quyền     | Xác thực bằng Username + Password (SHA-256). 3 vai trò: Admin, Manager (Manag), Staff.          |
| 2   | Quản lý danh mục           | Thêm, sửa, xóa mềm (soft delete), khôi phục danh mục sản phẩm.                                  |
| 3   | Quản lý sản phẩm           | CRUD sản phẩm. Cảnh báo tồn kho thấp (IsLowStock). Tự động tính InventoryValue.                 |
| 4   | Quản lý NCC                | CRUD nhà cung cấp (tên, SĐT, địa chỉ, email). Soft delete/restore.                              |
| 5   | Quản lý khách hàng         | CRUD khách hàng. Soft delete/restore.                                                           |
| 6   | Phiếu nhập/xuất kho        | Tạo phiếu (Import/Export), thêm chi tiết sản phẩm, tính chiết khấu, tổng tiền tự động.          |
| 7   | Quy trình duyệt phiếu      | Trạng thái: Pending → Approved/Cancelled. Khi duyệt Nhập → tăng tồn. Khi duyệt Xuất → giảm tồn. |
| 8   | Kiểm kê kho                | Tạo phiếu kiểm kê, so sánh tồn hệ thống vs thực tế. Khi duyệt → điều chỉnh tồn kho.             |
| 9   | Dashboard                  | 4 thẻ thống kê (tổng SP, tổng giá trị, phiếu chờ, kiểm kê). 3 biểu đồ (nhập/xuất/tỷ lệ).        |
| 10  | Báo cáo                    | Biểu đồ nhập/xuất 30 ngày. Xuất Excel (ClosedXML) và PDF (iTextSharp).                          |
| 11  | In phiếu                   | In phiếu nhập/xuất và phiếu kiểm kê qua PrintDocument/PrintPreview.                             |
| 12  | Nhật ký hành động          | Ghi log mọi thao tác (thêm/sửa/xóa). Hỗ trợ Undo hành động gần nhất.                            |
| 13  | Quản lý người dùng (Admin) | Thêm/sửa/ẩn tài khoản. Phân vai trò. Kích hoạt/vô hiệu hóa tài khoản.                           |
| 14  | Cài đặt                    | Chuyển đổi Dark/Light theme. Hiển thị/ẩn bản ghi đã xóa mềm.                                    |


### 2.3. Yêu cầu phi chức năng

- **Giao diện:** Thiết kế hiện đại với custom components (bo góc, placeholder, Dark/Light mode), font Segoe UI 8 cấp kích thước.
- **Dữ liệu:** Sử dụng SQLite nhúng, không yêu cầu cài đặt DBMS riêng. Hỗ trợ Foreign Key, PRAGMA encoding UTF-8.
- **An toàn:** Mật khẩu lưu dạng hash SHA-256. Phân quyền 3 cấp (Admin > Manag > Staff). Soft delete thay vì xóa vật lý.
- **Hiệu suất:** Sử dụng parameterized queries để chống SQL injection. Computed columns trong DB (SubTotal, Difference).
- **Khả năng mở rộng:** Kiến trúc 4 lớp rõ ràng, dễ thêm entity mới hoặc chuyển sang DBMS khác.

---

## CHƯƠNG 3. THIẾT KẾ HỆ THỐNG

### 3.1. Thiết kế tổng thể

**Kiến trúc 4 lớp (4-Layer Architecture):**

```
┌────────────────────────────────────────────────────────┐
│  VIEWS (Presentation Layer)                            │
│  ├── Login, Main                                       │
│  ├── Commons: AppName, MenuBar, ToolsBar,              │
│  │            Content, Footer                          │
│  ├── Panels:  DashboardPanel, ProductsPanel,           │
│  │            CategoriesPanel, SuppliersPanel,         │
│  │            CustomersPanel, TransactionsPanel,       │
│  │            InventoryChecksPanel, UsersPanel         │
│  ├── Forms:   ProductForm, CategoryForm,               │
│  │            SupplierForm, CustomerForm,              │
│  │            UserForm, TransactionAllForm,            │
│  │            TransactionDetailForm,                   │
│  │            TransactionReportForm,                   │
│  │            InventoryCheckForm,                      │
│  │            InventoryCheckDetailForm,                │
│  │            SettingsForm, StatusActionDialog         │
│  └── UI/Components: CustomButton, CustomTextBox,       │
│                      CustomComboBox, CustomTextArea,   │
│                      CustomPanel, CustomDateTimePicker │
│                      MenuButton, ThemeManager,         │
│                      UIConstants, DataGridViewHelper   │
├────────────────────────────────────────────────────────┤
│  CONTROLLERS (Controller Layer)                        │
│  ├── ProductController                                 │
│  ├── CategoryController                                │
│  ├── SupplierController                                │
│  ├── CustomerController                                │
│  ├── InventoryController                               │
│  ├── InventoryCheckController                          │
│  ├── StockTransactionController                        │
│  ├── TransactionDetailController                       │
│  ├── UserController                                    │
│  └── ActionsController                                 │
├────────────────────────────────────────────────────────┤
│  SERVICES (Business Logic Layer)                       │
│  ├── ProductService                                    │
│  ├── CategoryService                                   │
│  ├── SupplierService                                   │
│  ├── CustomerService                                   │
│  ├── InventoryService                                  │
│  ├── InventoryCheckService                             │
│  ├── StockTransactionService                           │
│  ├── TransactionDetailService                          │
│  ├── UserService                                       │
│  ├── ActionsService (Singleton)                        │
│  ├── ChartService                                      │
│  └── PrintService                                      │
├────────────────────────────────────────────────────────┤
│  REPOSITORIES (Data Access Layer)                      │
│  ├── BaseRepository (kết nối SQLite)                   │
│  ├── ProductRepository                                 │
│  ├── CategoryRepository                                │
│  ├── SupplierRepository                                │
│  ├── CustomerRepository                                │
│  ├── TransactionRepository                             │
│  ├── TransactionDetailRepository                       │
│  ├── InventoryCheckRepository                          │
│  ├── UserRepository                                    │
│  ├── ActionsRepository                                 │
│  └── LogRepository                                     │
├────────────────────────────────────────────────────────┤
│  DATABASE: SQLite 3 (Warehouse.db)                     │
└────────────────────────────────────────────────────────┘
```

**Luồng dữ liệu:** `View → Controller → Service → Repository → SQLite`

**Design Patterns sử dụng:**

- **Singleton:** ThemeManager, ActionsService – đảm bảo một instance duy nhất toàn ứng dụng.
- **Repository Pattern:** Mỗi entity có một Repository riêng kế thừa BaseRepository, đóng gói mọi truy vấn SQL.
- **Soft Delete:** Tất cả các bảng đều có cột `Visible` (1/0). Xóa mềm bằng `SET Visible = 0` thay vì `DELETE`.
- **Observer (Event):** ThemeManager phát sự kiện `ThemeChanged`, các component đăng ký lắng nghe để cập nhật giao diện.
- **Global State:** GlobalUser lưu thông tin người dùng đang đăng nhập.

### 3.2. Thiết kế cơ sở dữ liệu

#### 3.2.1. Danh sách các bảng


| STT | Tên bảng              | Mô tả                          | Số cột |
| --- | --------------------- | ------------------------------ | ------ |
| 1   | Suppliers             | Nhà cung cấp                   | 6      |
| 2   | Customers             | Khách hàng                     | 6      |
| 3   | Users                 | Người dùng hệ thống            | 8      |
| 4   | Categories            | Danh mục sản phẩm              | 4      |
| 5   | Products              | Sản phẩm / Hàng hóa            | 8      |
| 6   | Transactions          | Phiếu nhập/xuất kho (Header)   | 11     |
| 7   | TransactionDetails    | Chi tiết phiếu nhập/xuất       | 9      |
| 8   | InventoryChecks       | Phiếu kiểm kê                  | 6      |
| 9   | InventoryCheckDetails | Chi tiết kiểm kê               | 7      |
| 10  | Actions               | Nhật ký hành động (System Log) | 5      |


#### 3.2.2. Mô tả chi tiết từng bảng

**Bảng Suppliers (Nhà cung cấp)**


| Tên cột      | Kiểu dữ liệu | Ràng buộc                 | Mô tả               |
| ------------ | ------------ | ------------------------- | ------------------- |
| SupplierID   | INTEGER      | PRIMARY KEY AUTOINCREMENT | Mã NCC              |
| SupplierName | TEXT         | NOT NULL                  | Tên NCC             |
| Phone        | TEXT         | —                         | Số điện thoại       |
| Address      | TEXT         | —                         | Địa chỉ             |
| Email        | TEXT         | —                         | Email               |
| Visible      | INTEGER      | DEFAULT 1                 | Trạng thái hiển thị |


**Bảng Customers (Khách hàng)**


| Tên cột      | Kiểu dữ liệu | Ràng buộc                 | Mô tả               |
| ------------ | ------------ | ------------------------- | ------------------- |
| CustomerID   | INTEGER      | PRIMARY KEY AUTOINCREMENT | Mã KH               |
| CustomerName | TEXT         | NOT NULL                  | Tên KH              |
| Phone        | TEXT         | —                         | Số điện thoại       |
| Address      | TEXT         | —                         | Địa chỉ             |
| Email        | TEXT         | —                         | Email               |
| Visible      | INTEGER      | DEFAULT 1                 | Trạng thái hiển thị |


**Bảng Users (Người dùng)**


| Tên cột   | Kiểu dữ liệu | Ràng buộc                                           | Mô tả                   |
| --------- | ------------ | --------------------------------------------------- | ----------------------- |
| UserID    | INTEGER      | PRIMARY KEY AUTOINCREMENT                           | Mã người dùng           |
| Username  | TEXT         | NOT NULL UNIQUE                                     | Tên đăng nhập           |
| Password  | TEXT         | NOT NULL                                            | Mật khẩu (SHA-256 hash) |
| FullName  | TEXT         | —                                                   | Họ tên đầy đủ           |
| Role      | TEXT         | DEFAULT 'Staff', CHECK IN ('Admin','Manag','Staff') | Vai trò                 |
| IsActive  | INTEGER      | DEFAULT 1                                           | Trạng thái kích hoạt    |
| CreatedAt | TEXT         | DEFAULT datetime('now','localtime')                 | Thời gian tạo           |
| Visible   | INTEGER      | DEFAULT 1                                           | Trạng thái hiển thị     |


**Bảng Categories (Danh mục)**


| Tên cột      | Kiểu dữ liệu | Ràng buộc                 | Mô tả               |
| ------------ | ------------ | ------------------------- | ------------------- |
| CategoryID   | INTEGER      | PRIMARY KEY AUTOINCREMENT | Mã danh mục         |
| CategoryName | TEXT         | NOT NULL                  | Tên danh mục        |
| Description  | TEXT         | —                         | Mô tả               |
| Visible      | INTEGER      | DEFAULT 1                 | Trạng thái hiển thị |


**Bảng Products (Sản phẩm)**


| Tên cột        | Kiểu dữ liệu | Ràng buộc                   | Mô tả                       |
| -------------- | ------------ | --------------------------- | --------------------------- |
| ProductID      | INTEGER      | PRIMARY KEY AUTOINCREMENT   | Mã sản phẩm                 |
| ProductName    | TEXT         | NOT NULL                    | Tên sản phẩm                |
| CategoryID     | INTEGER      | FK → Categories(CategoryID) | Mã danh mục                 |
| Price          | REAL         | DEFAULT 0                   | Đơn giá                     |
| Quantity       | INTEGER      | DEFAULT 0                   | Số lượng tồn kho            |
| MinThreshold   | INTEGER      | DEFAULT 10                  | Ngưỡng tồn kho tối thiểu    |
| InventoryValue | REAL         | DEFAULT 0                   | Giá trị tồn kho (Qty×Price) |
| Visible        | INTEGER      | DEFAULT 1                   | Trạng thái hiển thị         |


**Bảng Transactions (Phiếu nhập/xuất kho)**


| Tên cột         | Kiểu dữ liệu | Ràng buộc                                                      | Mô tả               |
| --------------- | ------------ | -------------------------------------------------------------- | ------------------- |
| TransactionID   | INTEGER      | PRIMARY KEY AUTOINCREMENT                                      | Mã phiếu            |
| Type            | TEXT         | NOT NULL, CHECK IN ('Import','Export')                         | Loại: Nhập/Xuất     |
| DateCreated     | TEXT         | DEFAULT datetime('now','localtime')                            | Ngày tạo            |
| CreatedByUserID | INTEGER      | FK → Users(UserID)                                             | Người lập phiếu     |
| SupplierID      | INTEGER      | FK → Suppliers(SupplierID), nullable                           | Mã NCC (khi nhập)   |
| CustomerID      | INTEGER      | FK → Customers(CustomerID), nullable                           | Mã KH (khi xuất)    |
| TotalAmount     | REAL         | DEFAULT 0                                                      | Tổng tiền hàng      |
| Discount        | REAL         | DEFAULT 0                                                      | Tổng chiết khấu     |
| FinalAmount     | REAL         | DEFAULT 0                                                      | Thành tiền sau CK   |
| Note            | TEXT         | —                                                              | Ghi chú             |
| Status          | TEXT         | DEFAULT 'Pending', CHECK IN ('Pending','Approved','Cancelled') | Trạng thái          |
| Visible         | INTEGER      | DEFAULT 1                                                      | Trạng thái hiển thị |


**Bảng TransactionDetails (Chi tiết phiếu)**


| Tên cột       | Kiểu dữ liệu | Ràng buộc                                          | Mô tả                   |
| ------------- | ------------ | -------------------------------------------------- | ----------------------- |
| DetailID      | INTEGER      | PRIMARY KEY AUTOINCREMENT                          | Mã chi tiết             |
| TransactionID | INTEGER      | FK → Transactions(TransactionID) ON DELETE CASCADE | Mã phiếu                |
| ProductID     | INTEGER      | FK → Products(ProductID)                           | Mã sản phẩm             |
| ProductName   | TEXT         | —                                                  | Snapshot tên SP         |
| Quantity      | INTEGER      | NOT NULL                                           | Số lượng                |
| UnitPrice     | REAL         | —                                                  | Đơn giá                 |
| DiscountRate  | REAL         | DEFAULT 0                                          | % chiết khấu (0-100)    |
| SubTotal      | REAL         | GENERATED ALWAYS AS (Quantity × UnitPrice) STORED  | Thành tiền tự động tính |
| Visible       | INTEGER      | DEFAULT 1                                          | Trạng thái hiển thị     |


**Bảng InventoryChecks (Phiếu kiểm kê)**


| Tên cột         | Kiểu dữ liệu | Ràng buộc                                                       | Mô tả               |
| --------------- | ------------ | --------------------------------------------------------------- | ------------------- |
| CheckID         | INTEGER      | PRIMARY KEY AUTOINCREMENT                                       | Mã phiếu kiểm kê    |
| CheckDate       | TEXT         | DEFAULT datetime('now','localtime')                             | Ngày kiểm kê        |
| CreatedByUserID | INTEGER      | FK → Users(UserID)                                              | Người tạo phiếu     |
| Status          | TEXT         | DEFAULT 'Pending', CHECK IN ('Pending','Completed','Cancelled') | Trạng thái          |
| Note            | TEXT         | —                                                               | Ghi chú             |
| Visible         | INTEGER      | DEFAULT 1                                                       | Trạng thái hiển thị |


**Bảng InventoryCheckDetails (Chi tiết kiểm kê)**


| Tên cột        | Kiểu dữ liệu | Ràng buộc                                       | Mô tả                     |
| -------------- | ------------ | ----------------------------------------------- | ------------------------- |
| DetailID       | INTEGER      | PRIMARY KEY AUTOINCREMENT                       | Mã chi tiết               |
| CheckID        | INTEGER      | FK → InventoryChecks(CheckID) ON DELETE CASCADE | Mã phiếu kiểm kê          |
| ProductID      | INTEGER      | FK → Products(ProductID)                        | Mã sản phẩm               |
| SystemQuantity | INTEGER      | DEFAULT 0                                       | Tồn hệ thống              |
| ActualQuantity | INTEGER      | DEFAULT 0                                       | Tồn thực tế               |
| Difference     | INTEGER      | GENERATED ALWAYS AS (Actual − System) STORED    | Chênh lệch (tự động tính) |
| Reason         | TEXT         | —                                               | Lý do chênh lệch          |


**Bảng Actions (Nhật ký hành động)**


| Tên cột      | Kiểu dữ liệu | Ràng buộc                           | Mô tả                             |
| ------------ | ------------ | ----------------------------------- | --------------------------------- |
| LogID        | INTEGER      | PRIMARY KEY AUTOINCREMENT           | Mã log                            |
| ActionType   | TEXT         | —                                   | Loại hành động (UPDATE_STOCK,...) |
| Descriptions | TEXT         | —                                   | Mô tả hành động                   |
| DataBefore   | TEXT         | —                                   | Dữ liệu trước thay đổi (JSON)     |
| Visible      | INTEGER      | DEFAULT 1                           | Trạng thái hiển thị               |
| CreatedAt    | TEXT         | DEFAULT datetime('now','localtime') | Thời gian ghi log                 |


#### 3.2.3. Sơ đồ ERD (Quan hệ giữa các bảng)

```
                    ┌──────────────┐
                    │   Users      │
                    │──────────────│
                    │ UserID (PK)  │
                    │ Username     │
                    │ Password     │
                    │ FullName     │
                    │ Role         │
                    │ IsActive     │
                    └──────┬───────┘
                           │ 1
                   ┌───────┴────────┐
                   │                │
                   ▼ N              ▼ N
        ┌──────────────┐   ┌──────────────────┐
        │ Transactions │   │ InventoryChecks  │
        │──────────────│   │──────────────────│
        │ TransactionID│   │ CheckID (PK)     │
        │ Type         │   │ CheckDate        │
        │ DateCreated  │   │ CreatedByUserID  │
        │ SupplierID───┼──►│ Status           │
        │ CustomerID───┼──►│ Note             │
        │ TotalAmount  │   └────────┬─────────┘
        │ Discount     │            │ 1
        │ FinalAmount  │            ▼ N
        │ Status       │   ┌────────────────────────┐
        └──────┬───────┘   │ InventoryCheckDetails  │
               │ 1         │────────────────────────│
               ▼ N         │ DetailID (PK)          │
    ┌────────────────────┐ │ CheckID (FK)           │
    │ TransactionDetails │ │ ProductID (FK)─────────┼───┐
    │────────────────────│ │ SystemQuantity         │   │
    │ DetailID (PK)      │ │ ActualQuantity         │   │
    │ TransactionID (FK) │ │ Difference (Computed)  │   │
    │ ProductID (FK)─────┼─┤ Reason                 │   │
    │ ProductName        │ └────────────────────────┘   │
    │ Quantity           │                              │
    │ UnitPrice          │         ┌────────────────┐   │
    │ DiscountRate       │         │   Products     │◄──┘
    │ SubTotal (Computed)│         │────────────────│
    └────────────────────┘         │ ProductID (PK) │
                                   │ ProductName    │
    ┌──────────────┐               │ CategoryID (FK)│──►┌──────────────┐
    │  Suppliers   │               │ Price          │   │  Categories  │
    │──────────────│               │ Quantity       │   │──────────────│
    │ SupplierID   │◄── (FK)       │ MinThreshold   │   │ CategoryID   │
    │ SupplierName │               │ InventoryValue │   │ CategoryName │
    │ Phone        │               └────────────────┘   │ Description  │
    │ Address      │                                    └──────────────┘
    │ Email        │    ┌──────────────┐
    └──────────────┘    │  Customers   │    ┌──────────────┐
                        │──────────────│    │   Actions    │
                        │ CustomerID   │◄── │──────────────│
                        │ CustomerName │(FK)│ LogID (PK)   │
                        │ Phone        │    │ ActionType   │
                        │ Address      │    │ Descriptions │
                        │ Email        │    │ DataBefore   │
                        └──────────────┘    │ CreatedAt    │
                                            └──────────────┘
```

**Các khóa ngoại chính:**

- `Products.CategoryID` → `Categories.CategoryID`
- `Transactions.CreatedByUserID` → `Users.UserID`
- `Transactions.SupplierID` → `Suppliers.SupplierID`
- `Transactions.CustomerID` → `Customers.CustomerID`
- `TransactionDetails.TransactionID` → `Transactions.TransactionID` (ON DELETE CASCADE)
- `TransactionDetails.ProductID` → `Products.ProductID`
- `InventoryChecks.CreatedByUserID` → `Users.UserID`
- `InventoryCheckDetails.CheckID` → `InventoryChecks.CheckID` (ON DELETE CASCADE)
- `InventoryCheckDetails.ProductID` → `Products.ProductID`

### 3.3. Thiết kế giao diện

#### 3.3.1. Cấu trúc layout chính (Main Form)

```
┌───────────────────────────────────────────────────┐
│  AppName (Tên ứng dụng + Logo)                    │
├───────────┬───────────────────────────────────────┤
│           │  ToolsBar (Tìm kiếm + Nút hành động)  │
│  MenuBar  ├───────────────────────────────────────┤
│  (Sidebar │  Content (Vùng nội dung chính)        │
│  menu)    │    ┌────────────────────────────────┐ │
│           │    │  Active Panel                  │ │
│ Dashboard │    │  (Dashboard / Products /       │ │
│ Categories│    │   Categories / Suppliers /     │ │
│ Products  │    │   Customers / Transactions /   │ │
│ Suppliers │    │   InventoryChecks / Users)     │ │
│ Customers │    │                                │ │
│ Phiếu NX  │    └────────────────────────────────┘ │
│ Kiểm kê   │                                       │
│ Users     ├───────────────────────────────────────┤
│ Settings  │  Footer (Phiên bản + Thời gian)       │
│ Account   │                                       │
└───────────┴───────────────────────────────────────┘
```

#### 3.3.2. Danh sách các Form/Màn hình


| STT | Form/Panel               | Chức năng                                           |
| --- | ------------------------ | --------------------------------------------------- |
| 1   | Login                    | Đăng nhập hệ thống                                  |
| 2   | Main                     | Form chính chứa layout MenuBar + ToolsBar + Content |
| 3   | DashboardPanel           | Thống kê tổng quan: 4 stat cards + 3 biểu đồ        |
| 4   | CategoriesPanel          | Danh sách danh mục (DataGridView)                   |
| 5   | ProductsPanel            | Danh sách sản phẩm (DataGridView)                   |
| 6   | SuppliersPanel           | Danh sách NCC (DataGridView)                        |
| 7   | CustomersPanel           | Danh sách khách hàng (DataGridView)                 |
| 8   | TransactionsPanel        | Danh sách phiếu nhập/xuất (DataGridView)            |
| 9   | InventoryChecksPanel     | Danh sách phiếu kiểm kê (DataGridView)              |
| 10  | UsersPanel               | Danh sách người dùng – chỉ Admin (DataGridView)     |
| 11  | CategoryForm             | Form thêm/sửa danh mục                              |
| 12  | ProductForm              | Form thêm/sửa sản phẩm                              |
| 13  | SupplierForm             | Form thêm/sửa nhà cung cấp                          |
| 14  | CustomerForm             | Form thêm/sửa khách hàng                            |
| 15  | UserForm                 | Form thêm/sửa người dùng                            |
| 16  | TransactionAllForm       | Form tạo phiếu nhập/xuất (TabControl Import/Export) |
| 17  | TransactionDetailForm    | Xem chi tiết phiếu nhập/xuất + In                   |
| 18  | TransactionReportForm    | Báo cáo nhập/xuất 30 ngày + Xuất Excel/PDF          |
| 19  | InventoryCheckForm       | Form tạo/xem phiếu kiểm kê                          |
| 20  | InventoryCheckDetailForm | Chi tiết phiếu kiểm kê + In + Duyệt                 |
| 21  | StatusActionDialog       | Dialog xác nhận Duyệt/Hủy phiếu                     |
| 22  | SettingsForm             | Cài đặt theme, hiển thị bản ghi ẩn                  |


#### 3.3.3. Custom UI Components


| Component            | Base class | Đặc điểm                                                                                       |
| -------------------- | ---------- | ---------------------------------------------------------------------------------------------- |
| CustomButton         | Button     | 7 kiểu (Primary, Secondary, Success, Danger, Warning, Info, Text). Bo góc. Hover/Press effect. |
| CustomTextBox        | Control    | TextBox bo góc, placeholder text, focus highlight.                                             |
| CustomComboBox       | ComboBox   | Bo góc, custom draw items, focus highlight.                                                    |
| CustomTextArea       | Control    | TextArea nhiều dòng, bo góc, placeholder.                                                      |
| CustomPanel          | Panel      | Bo góc, shadow effect.                                                                         |
| CustomDateTimePicker | Control    | DateTimePicker bo góc, custom styling.                                                         |
| MenuButton           | Button     | Nút sidebar với trạng thái Selected, hover effect.                                             |


#### 3.3.4. Hệ thống Theme (Light/Dark)


| Thành phần     | Light Mode        | Dark Mode          |
| -------------- | ----------------- | ------------------ |
| Background     | FFFFFF → D8D8D8   | 1E1E1E → 0D0D0D    |
| Text Primary   | 212121            | FFFFFF             |
| Text Secondary | 666666            | B0B0B0             |
| Primary Color  | 5A9F68 (xanh lá)  | 5A9F68 (không đổi) |
| Font Family    | Segoe UI          | Segoe UI           |
| Font Sizes     | 9f – 24f (8 cấp)  | 9f – 24f (8 cấp)   |


---

## CHƯƠNG 4. XÂY DỰNG ỨNG DỤNG

### 4.1. Cấu trúc thư mục dự án

```
nosql version/
├── Assets/
│   ├── CSS/style.css              # Stylesheet (dự phòng)
│   ├── ICO/appIcon.ico            # Icon ứng dụng
│   └── SQL/
│       ├── sqlite_schema.sql      # Script tạo bảng + dữ liệu mặc định
│       ├── sqlite_seed.sql        # Script dữ liệu mẫu (50 SP, 200 phiếu, 600 chi tiết)
│       ├── Warehouse.db           # Database chính
│       └── WarehouseTest.db       # Database phát triển/test
├── Controllers/                   # 10 controllers
│   ├── ActionsController.cs
│   ├── CategoryController.cs
│   ├── CustomerController.cs
│   ├── InventoryCheckController.cs
│   ├── InventoryController.cs
│   ├── ProductController.cs
│   ├── StockTransactionController.cs
│   ├── SupplierController.cs
│   ├── TransactionDetailController.cs
│   └── UserController.cs
├── Helpers/                       # 5 helper classes
│   ├── AppVersion.cs              # Đọc phiên bản từ assembly
│   ├── DataConverter.cs           # Format tiền, ngày, JSON
│   ├── DatabaseHelper.cs          # Kết nối, migration, backup
│   ├── IdGenerator.cs             # Tạo ID, hash SHA-256
│   └── SQLiteDataReaderExtensions.cs # Extension methods cho SQLiteDataReader
├── Models/                        # 11 entity models
│   ├── Actions.cs
│   ├── Category.cs
│   ├── Customer.cs
│   ├── GlobalUser.cs              # Static class lưu user đang đăng nhập
│   ├── InventoryCheck.cs
│   ├── InventoryCheckDetail.cs
│   ├── Product.cs
│   ├── Supplier.cs
│   ├── Transaction.cs
│   ├── TransactionDetail.cs
│   └── User.cs
├── Repositories/                  # 11 repositories
│   ├── ActionsRepository.cs
│   ├── BaseRepository.cs          # Lớp cơ sở: GetConnection(), TestConnection()
│   ├── CategoryRepository.cs
│   ├── CustomerRepository.cs
│   ├── InventoryCheckRepository.cs
│   ├── LogRepository.cs
│   ├── ProductRepository.cs
│   ├── SupplierRepository.cs
│   ├── TransactionDetailRepository.cs
│   ├── TransactionRepository.cs
│   └── UserRepository.cs
├── Scripts/
│   └── InitializeDatabase.cs     # (dự phòng – chưa sử dụng)
├── Services/                      # 12 services
│   ├── ActionsService.cs          # Singleton – quản lý nhật ký
│   ├── CategoryService.cs
│   ├── ChartService.cs            # Chuẩn bị dữ liệu biểu đồ
│   ├── CustomerService.cs
│   ├── InventoryCheckService.cs
│   ├── InventoryService.cs        # Nghiệp vụ nhập/xuất/duyệt/undo
│   ├── PrintService.cs            # In phiếu nhập/xuất và kiểm kê
│   ├── ProductService.cs
│   ├── StockTransactionService.cs
│   ├── SupplierService.cs
│   ├── TransactionDetailService.cs
│   └── UserService.cs
├── UI/
│   ├── Components/                # 8 custom UI components
│   │   ├── ComponentsTestPanel.cs
│   │   ├── CustomButton.cs
│   │   ├── CustomComboBox.cs
│   │   ├── CustomDateTimePicker.cs
│   │   ├── CustomPanel.cs
│   │   ├── CustomTextArea.cs
│   │   ├── CustomTextBox.cs
│   │   └── MenuButton.cs
│   ├── DataGridViewHelper.cs      # Hover/selection helper cho bảng
│   ├── ThemeManager.cs            # Singleton – quản lý Dark/Light theme
│   └── UIConstants.cs             # Hằng số: màu, font, kích thước, spacing, icons
├── Views/
│   ├── Commons/                   # 5 layout components
│   │   ├── AppName.cs
│   │   ├── Content.cs
│   │   ├── Footer.cs
│   │   ├── MenuBar.cs
│   │   └── ToolsBar.cs
│   ├── Forms/                     # 12 form dialogs
│   │   ├── CategoryForm.cs
│   │   ├── CustomerForm.cs
│   │   ├── InventoryCheckDetailForm.cs
│   │   ├── InventoryCheckForm.cs
│   │   ├── ProductForm.cs
│   │   ├── SettingsForm.cs
│   │   ├── StatusActionDialog.cs
│   │   ├── SupplierForm.cs
│   │   ├── TransactionAllForm.cs
│   │   ├── TransactionDetailForm.cs
│   │   ├── TransactionReportForm.cs
│   │   └── UserForm.cs
│   ├── Panels/                    # 8 panels + 1 interface
│   │   ├── CategoriesPanel.cs
│   │   ├── CustomersPanel.cs
│   │   ├── DashboardPanel.cs
│   │   ├── InventoryChecksPanel.cs
│   │   ├── ISearchable.cs         # Interface tìm kiếm
│   │   ├── ProductsPanel.cs
│   │   ├── SuppliersPanel.cs
│   │   ├── TransactionsPanel.cs
│   │   └── UsersPanel.cs
│   ├── Actions.cs                 # Xử lý Save/Undo
│   ├── Login.cs                   # Form đăng nhập
│   └── Main.cs                    # Form chính
├── .gitignore
├── App.config                     # Connection string SQLite
├── Directory.Build.props          # Cấu hình đường dẫn build output
├── Program.cs                     # Entry point
├── WarehouseManagement.csproj     # File dự án .NET
└── WarehouseManagement.sln        # Solution file
```

**Tổng cộng: ~80 file mã nguồn C#**

### 4.2. Xây dựng cơ sở dữ liệu

#### 4.2.1. Tạo database

- **File database:** `Assets/SQL/Warehouse.db` (production) và `WarehouseTest.db` (development).
- **Connection string** được cấu hình trong `App.config`:
  ```xml
  <add name="WarehouseDB"
       connectionString="Data Source=Assets\SQL\WarehouseTest.db;Version=3;Foreign Keys=True;"
       providerName="System.Data.SQLite" />
  ```
- **PRAGMA:** `foreign_keys = ON` và `encoding = 'UTF-8'` được bật ngay trong schema.
- **Migration:** `DatabaseHelper.RunMigration()` được gọi tự động khi khởi động ứng dụng.

#### 4.2.2. Tạo bảng và ràng buộc

- **Script:** `Assets/SQL/sqlite_schema.sql` tạo 10 bảng với đầy đủ:
  - Khóa chính AUTOINCREMENT
  - Khóa ngoại (FK) với ON DELETE CASCADE cho chi tiết phiếu
  - CHECK constraints (Role, Type, Status)
  - Computed columns: `SubTotal` (TransactionDetails) và `Difference` (InventoryCheckDetails)
  - Dữ liệu mặc định: 3 tài khoản hệ thống (admin, manag, staff) với mật khẩu hash SHA-256

#### 4.2.3. Nhập dữ liệu mẫu

- **Script:** `Assets/SQL/sqlite_seed.sql` tạo dữ liệu demo phong phú:
  - 3 người dùng hệ thống
  - 5 nhà cung cấp (Apple, Samsung, Việt Tiến, Honda, Digiworld)
  - 7 khách hàng (cá nhân và doanh nghiệp)
  - 10 danh mục sản phẩm (Điện thoại, Máy tính, Gia dụng, Thời trang, Mỹ phẩm, Sách, Thể thao, Đồ chơi, Ô tô)
  - 50 sản phẩm đa dạng (từ iPhone đến Ducati)
  - 1 phiếu nhập ban đầu (Approved) + 200 phiếu nhập/xuất (Pending) + 600 dòng chi tiết
  - 2 phiếu kiểm kê (Completed) với chi tiết mẫu

### 4.3. Xây dựng giao diện người dùng

#### 4.3.1. Form đăng nhập (Login.cs)

- Giao diện: CustomPanel chứa Logo, Label tiêu đề, CustomTextBox (username/password), CustomButton (Đăng nhập/Thoát).
- Xác thực qua `UserController.Login()` → kiểm tra SHA-256 hash.
- Kết quả: `DialogResult.OK` nếu thành công → `GlobalUser.CurrentUser` được gán → mở Main.

#### 4.3.2. Các Panel quản lý chính

Tất cả Panel implement interface `ISearchable` để hỗ trợ tìm kiếm từ ToolsBar:

```csharp
public interface ISearchable
{
    void Search(string searchText);
}
```

Mỗi Panel sử dụng DataGridView với:

- Hover effect tùy chỉnh (DataGridViewHelper)
- Cột hành động (Sửa, Xóa/Ẩn, Khôi phục)
- Tìm kiếm real-time qua ISearchable
- Refresh dữ liệu sau mỗi thao tác

#### 4.3.3. Các nút chức năng (ToolsBar)


| Nút            | Chức năng                                             |
| -------------- | ----------------------------------------------------- |
| Search         | Tìm kiếm trên Panel hiện tại (gọi ISearchable.Search) |
| Add            | Mở Form thêm mới tương ứng với Panel đang hiển thị    |
| Transaction    | Mở TransactionAllForm tạo phiếu nhập/xuất             |
| Inventory      | Mở InventoryCheckForm tạo phiếu kiểm kê               |
| Undo           | Hoàn tác hành động gần nhất (qua InventoryController) |
| Save           | Lưu trạng thái (qua ActionsService)                   |
| Report         | Mở TransactionReportForm báo cáo                      |
| DateTimePicker | Chọn khoảng thời gian cho báo cáo                     |


### 4.4. Xử lý nghiệp vụ và kết nối CSDL

#### 4.4.1. Kết nối cơ sở dữ liệu

- **BaseRepository:** Lớp cơ sở cho tất cả Repository, đọc connection string từ `ConfigurationManager`.
- Mỗi thao tác tạo connection mới (`GetConnection()`) → sử dụng → đóng (using pattern).
- `TestConnection()` kiểm tra kết nối trước khi ứng dụng khởi động.

#### 4.4.2. Thực hiện các thao tác CRUD

- Tất cả Repository dùng **parameterized queries** (`@param` + `AddWithValue`) để chống SQL injection.
- Lấy ID mới sau INSERT bằng `SELECT last_insert_rowid()`.
- Soft delete bằng `UPDATE SET Visible = 0` thay vì `DELETE`.
- Một số Repository hỗ trợ Hard delete khi entity không có liên kết FK (vd: sản phẩm không có trong phiếu).

#### 4.4.3. Kiểm tra dữ liệu đầu vào

Validation được thực hiện tại tầng **Service**:

- Kiểm tra tên không rỗng (Category, Product, Supplier, Customer).
- Kiểm tra giá, số lượng > 0 (Product).
- Kiểm tra Username unique (User).
- Kiểm tra tồn kho đủ khi xuất (InventoryService).
- Kiểm tra danh mục không còn sản phẩm trước khi xóa (CategoryService).
- Kiểm tra trùng sản phẩm trong phiếu kiểm kê (InventoryCheckService).

#### 4.4.4. Xử lý lỗi

- **Program.cs:** try-catch toàn cục bắt lỗi khởi động, hiển thị MessageBox chi tiết.
- **Migration:** Lỗi migration không chặn ứng dụng, chỉ bỏ qua.
- **Kết nối DB:** Kiểm tra trước khi chạy, hiển thị hướng dẫn sửa lỗi nếu thất bại.
- **Service layer:** Mỗi phương thức có validation rõ ràng, throw exception hoặc return false khi dữ liệu không hợp lệ.

#### 4.4.5. Cơ chế Undo và Nhật ký

- **ActionsService (Singleton):** Ghi log mọi thao tác vào bảng Actions dưới dạng JSON.
- **DataBefore:** Lưu trạng thái dữ liệu trước thay đổi (serialize bằng Newtonsoft.Json).
- **Undo:** `InventoryService.UndoLastAction()` đọc log gần nhất, parse JSON, khôi phục dữ liệu.
- **TryRemoveConsecutiveOppositeLog:** Gộp cặp Delete/Restore liên tiếp để tránh log thừa.

#### 4.4.6. Quy trình duyệt phiếu

**Phiếu nhập/xuất (Transactions):**

1. Tạo phiếu → Status = `Pending`, tồn kho chưa thay đổi.
2. Duyệt (Approve) → Status = `Approved`:
  - Phiếu nhập (Import): Tồn kho += Quantity cho mỗi chi tiết.
  - Phiếu xuất (Export): Tồn kho -= Quantity cho mỗi chi tiết.
  - Cập nhật InventoryValue = Quantity × Price.
3. Hủy (Cancel) → Status = `Cancelled`, tồn kho không thay đổi.

**Phiếu kiểm kê (InventoryChecks):**

1. Tạo phiếu → Status = `Pending`.
2. Duyệt (Approve) → Status = `Completed`:
  - Điều chỉnh tồn kho theo chênh lệch: Quantity += (ActualQuantity − SystemQuantity).
3. Hủy → Status = `Cancelled`, không điều chỉnh.

#### 4.4.7. Tính năng in và xuất báo cáo

- **PrintService:** Render phiếu nhập/xuất và phiếu kiểm kê qua `PrintDocument`/`PrintPreviewDialog`.
  - Font: Times New Roman (tiêu đề), Arial (nội dung).
  - Hỗ trợ chuyển số sang chữ tiếng Việt.
- **TransactionReportForm:**
  - Biểu đồ nhập/xuất 30 ngày (OxyPlot).
  - Xuất Excel qua ClosedXML.
  - Xuất PDF qua iTextSharp.

---

## CHƯƠNG 5. KIỂM THỬ VÀ ĐÁNH GIÁ

### 5.1. Kiểm thử chức năng

#### 5.1.1. Kiểm thử CRUD (Thêm / Sửa / Xóa)


| Chức năng          | Kịch bản kiểm thử                                      | Kết quả mong đợi                   |
| ------------------ | ------------------------------------------------------ | ---------------------------------- |
| Thêm sản phẩm      | Nhập đầy đủ thông tin hợp lệ → Lưu                     | SP mới xuất hiện trong danh sách   |
| Sửa sản phẩm       | Chọn SP → Sửa tên/giá → Lưu                            | Thông tin cập nhật đúng            |
| Xóa mềm sản phẩm   | Chọn SP → Xóa                                          | SP ẩn khỏi danh sách (Visible = 0) |
| Khôi phục sản phẩm | Bật "Hiển thị bản ghi ẩn" → Chọn SP đã xóa → Khôi phục | SP hiển thị lại (Visible = 1)      |
| Thêm danh mục      | Nhập tên danh mục → Lưu                                | Danh mục mới xuất hiện             |
| Xóa danh mục có SP | Chọn danh mục còn SP → Xóa                             | Từ chối xóa, hiển thị cảnh báo     |
| Thêm/sửa NCC, KH   | Nhập thông tin → Lưu                                   | Dữ liệu cập nhật đúng              |


#### 5.1.2. Kiểm thử nhập dữ liệu sai


| Kịch bản                     | Kết quả mong đợi                |
| ---------------------------- | ------------------------------- |
| Tên sản phẩm rỗng            | Từ chối lưu, hiển thị lỗi       |
| Giá sản phẩm < 0             | Từ chối lưu, yêu cầu nhập lại   |
| Số lượng xuất > tồn kho      | Từ chối duyệt phiếu xuất        |
| Username trùng khi thêm user | Từ chối tạo, thông báo trùng    |
| Đăng nhập sai mật khẩu       | Từ chối, hiển thị thông báo lỗi |
| Tạo phiếu kiểm kê trùng SP   | Từ chối, yêu cầu xóa dòng trùng |


#### 5.1.3. Kiểm thử tìm kiếm và thống kê


| Chức năng                | Kịch bản                                     | Kết quả mong đợi                    |
| ------------------------ | -------------------------------------------- | ----------------------------------- |
| Tìm kiếm sản phẩm        | Nhập từ khóa "iPhone" vào thanh tìm kiếm     | Hiển thị các SP có chứa "iPhone"    |
| Tìm kiếm danh mục        | Nhập từ khóa "Điện thoại"                    | Hiển thị danh mục phù hợp           |
| Dashboard thống kê       | Mở Dashboard                                 | Hiển thị 4 stat cards + 3 biểu đồ   |
| Báo cáo theo khoảng ngày | Chọn ngày bắt đầu/kết thúc → Xem báo cáo     | Biểu đồ và bảng dữ liệu đúng khoảng |
| Xuất Excel               | Nhấn nút xuất Excel từ TransactionReportForm | File .xlsx tạo thành công           |


### 5.2. Đánh giá kết quả đạt được

#### 5.2.1. So sánh mục tiêu và kết quả


| Mục tiêu                        | Trạng thái | Ghi chú                                |
| ------------------------------- | ---------- | -------------------------------------- |
| Ứng dụng quản lý kho hoàn chỉnh | Hoàn thành | Đầy đủ CRUD cho 8 entity               |
| Giao diện hiện đại              | Hoàn thành | Custom components, Dark/Light theme    |
| CSDL SQLite nhúng               | Hoàn thành | Không cần cài đặt server               |
| Phân quyền 3 cấp                | Hoàn thành | Admin, Manag, Staff                    |
| In phiếu                        | Hoàn thành | PrintService hỗ trợ phiếu NX + kiểm kê |
| Xuất báo cáo Excel/PDF          | Hoàn thành | ClosedXML + iTextSharp                 |
| Biểu đồ thống kê                | Hoàn thành | OxyPlot: biểu đồ cột, đường, tròn      |
| Cơ chế Undo                     | Hoàn thành | ActionsService + JSON DataBefore       |


#### 5.2.2. Những chức năng đã hoàn thành

- Đăng nhập xác thực SHA-256 với phân quyền Admin/Manag/Staff.
- CRUD đầy đủ cho: Danh mục, Sản phẩm, NCC, Khách hàng, Người dùng.
- Lập phiếu nhập/xuất kho với chi tiết sản phẩm, chiết khấu, tính tổng tự động.
- Quy trình duyệt phiếu (Pending → Approved/Cancelled) với cập nhật tồn kho.
- Kiểm kê kho: so sánh tồn hệ thống vs thực tế, điều chỉnh tự động khi duyệt.
- Dashboard: 4 stat cards + 3 biểu đồ (nhập/xuất theo ngày, theo tháng, tỷ lệ).
- In phiếu nhập/xuất và phiếu kiểm kê (PrintPreview).
- Xuất báo cáo Excel/PDF.
- Dark/Light theme toàn ứng dụng.
- Nhật ký hành động + Undo.
- Soft delete/restore tất cả entity.
- Tìm kiếm real-time trên mỗi panel.
- Dữ liệu mẫu phong phú (50 SP, 200 phiếu, 600 chi tiết).

#### 5.2.3. Những hạn chế còn tồn tại

- `StockTransactionService.UpdateTransaction()` chưa được implement (throw `NotImplementedException`).
- File `InitializeDatabase.cs` trong thư mục Scripts rỗng, chưa sử dụng.
- `LogRepository` và `ActionsRepository` có phần logic trùng lặp (cùng thao tác trên bảng Actions).
- Chưa có unit test tự động.
- File `Assets/CSS/style.css` rỗng (chưa sử dụng).
- Chưa hỗ trợ multi-language (hiện tại hardcode tiếng Việt + tiếng Anh lẫn lộn).

---

## CHƯƠNG 6. KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

### 6.1. Kết luận

**Tổng kết quá trình thực hiện:**

- Dự án đã xây dựng thành công ứng dụng quản lý kho hàng desktop hoàn chỉnh với kiến trúc 4 lớp rõ ràng (Views → Controllers → Services → Repositories).
- Cơ sở dữ liệu SQLite nhúng với 10 bảng, đầy đủ khóa chính, khóa ngoại, CHECK constraints và computed columns.
- Giao diện WinForms hiện đại với 7 custom components tự thiết kế, hỗ trợ Dark/Light theme.
- Tổng cộng khoảng 80 file mã nguồn C#, phân bổ hợp lý giữa các lớp kiến trúc.

**Kiến thức và kỹ năng đạt được:**

- Phân tích yêu cầu và thiết kế hệ thống phần mềm quản lý.
- Thiết kế CSDL quan hệ với SQLite (foreign keys, computed columns, CHECK constraints).
- Lập trình C# WinForms với kiến trúc phân lớp (MVC-like).
- Xây dựng custom UI components (bo góc, theme, placeholder).
- Sử dụng các thư viện: ClosedXML (Excel), iTextSharp (PDF), OxyPlot (biểu đồ), Newtonsoft.Json (JSON).
- Áp dụng design patterns: Singleton, Repository, Observer, Soft Delete.
- Quản lý mã nguồn với Git.

### 6.2. Hướng phát triển

- **Bổ sung chức năng:**
  - Implement `UpdateTransaction()` còn thiếu.
  - Thêm chức năng xuất/nhập dữ liệu backup (CSV/JSON).
  - Hỗ trợ mã vạch/QR code cho sản phẩm.
  - Thêm module báo cáo lãi/lỗ chi tiết.
  - Thêm unit test tự động cho tầng Service và Repository.
- **Nâng cấp giao diện:**
  - Hoàn thiện responsive layout.
  - Thêm animation chuyển panel mượt mà.
  - Hỗ trợ đa ngôn ngữ (i18n: Việt, Anh).
- **Phát triển lên Web/Mobile:**
  - Chuyển đổi sang ASP.NET Core Web API + React/Angular frontend.
  - Phát triển app mobile (Xamarin/MAUI) để kiểm kê tại kho.
- **Áp dụng phân quyền, bảo mật nâng cao:**
  - Thêm chính sách mật khẩu (độ dài tối thiểu, ký tự đặc biệt).
  - Thêm chức năng khóa tài khoản sau nhiều lần đăng nhập sai.
  - Mã hóa dữ liệu nhạy cảm trong database.
  - Hỗ trợ đăng nhập 2 lớp (2FA).
- **Nâng cấp CSDL:**
  - Chuyển sang MySQL/PostgreSQL cho môi trường multi-user.
  - Thêm stored procedures và triggers.
  - Cài đặt connection pooling.

---

## PHỤ LỤC

### A. Thống kê mã nguồn


| Lớp kiến trúc | Số file | Ghi chú                                            |
| ------------- | ------- | -------------------------------------------------- |
| Models        | 11      | 8 entity + GlobalUser + Actions + Details          |
| Repositories  | 11      | BaseRepository + 10 repository cụ thể              |
| Services      | 12      | 10 business services + ChartService + PrintService |
| Controllers   | 10      | Mỗi entity group một controller                    |
| Views/Commons | 5       | Layout components                                  |
| Views/Panels  | 9       | 8 panels + ISearchable interface                   |
| Views/Forms   | 12      | CRUD forms + Report + Settings + Dialog            |
| Views (root)  | 3       | Main, Login, Actions                               |
| UI/Components | 8       | 7 custom controls + ComponentsTestPanel            |
| UI (root)     | 3       | ThemeManager, UIConstants, DataGridViewHelper      |
| Helpers       | 5       | Utilities                                          |
| **Tổng cộng** | **~89** |                                                    |


### B. Thư viện NuGet sử dụng


| Thư viện                | Phiên bản | Mục đích                           |
| ----------------------- | --------- | ---------------------------------- |
| System.Data.SQLite.Core | 1.0.118   | Kết nối và thao tác SQLite         |
| ClosedXML               | 0.105.0   | Xuất báo cáo Excel (.xlsx)         |
| iTextSharp              | 5.5.13.5  | Xuất báo cáo PDF                   |
| OxyPlot.WindowsForms    | 2.2.0     | Vẽ biểu đồ (cột, đường, tròn)      |
| FontAwesome.Sharp       | 6.3.0     | Icon cho giao diện                 |
| Newtonsoft.Json         | 13.0.3    | Serialize/Deserialize JSON cho log |


### C. Tài khoản mặc định


| Username | Password (plain) | Password (SHA-256 hash)                                          | Role  |
| -------- | ---------------- | ---------------------------------------------------------------- | ----- |
| admin    | 123              | a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3 | Admin |
| manag    | 123              | a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3 | Manag |
| staff    | 123              | a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3 | Staff |


