# Warehouse Management

## Introduction

Warehouse Management is a Windows desktop application (WinForms) for tracking inventory and stock in/out operations. It helps control stock levels and reduce operational errors.

This repository provides **two versions** with the same features but different data backends:


| Version    | Database            | Folder           |
| ---------- | ------------------- | ---------------- |
| **SQLite** | SQLite (`.db` file) | `nosql version/` |
| **MySQL**  | MySQL Server        | `mysql version/` |


**Main features:**

- **Product management:** CRUD and search for products.
- **Transaction management:** Create stock in/out vouchers; inventory updates automatically.
- **Low-stock alerts:** Alerts when quantity falls below a minimum threshold.
- **Undo:** Revert the last action via the system log.
- **Reports:** Charts for in/out trends over time.

---

## Repository structure

```
WarehouseManagement/
│
├── Assets/
│   ├── SQL/
│   └── sqlite3.exe      # SQLite CLI
├── Controllers/         # UI-to-service wiring; handles user actions and calls services
├── Helpers/             # Utilities (database helper, converters, ID generator, etc.)
├── Models/              # Data entities / DTOs
├── Repositories/        # Data access layer; direct DB queries
├── Services/            # Business logic and undo/redo
├── UI/                  # Reusable UI components, theme, constants
├── Views/               # WinForms screens (panels, forms, menu, layout)
├── .gitignore
├── App.config
├── App.config.example
├── Directory.Build.props
├── Program.cs
├── WarehouseManagement.csproj
└── WarehouseManagement.sln
```

---

## Installation

### 1. System requirements

- **Windows** (WinForms app).
- **.NET Framework 4.7.2** (included with SDK / Visual Studio).
- **SQLite version:** you need **sqlite3.exe** (see section 3 below).
- **MySQL version:** **MySQL Server** (localhost or remote) and the connection user password.

### 2. NuGet packages (dotnet)

Packages are declared in each `.csproj`; run `dotnet restore` in the corresponding folder to install them.

**Shared by both versions:**

- `Newtonsoft.Json` (13.0.3) — Undo log.
- `ClosedXML` — Excel export.
- `FontAwesome.Sharp` — Icons.
- `iTextSharp` — PDF.
- `OxyPlot.WindowsForms` — Charts.

**Per version:**


| Package                             | SQLite | MySQL |
| ----------------------------------- | ------ | ----- |
| `System.Data.SQLite.Core` (1.0.118) | ✓      | —     |
| `MySql.Data` (8.0.33)               | —      | ✓     |


To install manually (e.g. Package Manager Console):

```powershell
# From "nosql version" or "mysql version" folder
dotnet restore
```

### 3. Additional application (SQLite version only)

The **SQLite** version uses **sqlite3.exe** to create and initialize the database from scripts (as suggested in the SQL files).

- **Download sqlite3.exe:** [Release sqlite3.exe](https://release-assets.githubusercontent.com/github-production-release-asset/1169826967/d5025b81-93ba-4459-b6d3-c2af460f557f?sp=r&sv=2018-11-09&sr=b&spr=https&se=2026-03-01T10%3A27%3A18Z&rscd=attachment%3B+filename%3Dsqlite3.exe&rsct=application%2Foctet-stream&skoid=96c2d410-5711-43a1-aedd-ab1947aa7ab0&sktid=398a6654-997b-47e9-b12b-9515b896b4de&skt=2026-03-01T09%3A26%3A26Z&ske=2026-03-01T10%3A27%3A18Z&sks=b&skv=2018-11-09&sig=%2Bn5KsoDZoyNQrOOpNVbXcprudGTTlWQ%2F5lC2SvF63Vo%3D&jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmVsZWFzZS1hc3NldHMuZ2l0aHVidXNlcmNvbnRlbnQuY29tIiwia2V5Ijoia2V5MSIsImV4cCI6MTc3MjM1NzU1MSwibmJmIjoxNzcyMzU3MjUxLCJwYXRoIjoicmVsZWFzZWFzc2V0cHJvZHVjdGlvbi5ibG9iLmNvcmUud2luZG93cy5uZXQifQ.SM7k-MMdOBYaK813CONdxjmicAEXeztdRnPoR6kLBOA&response-content-disposition=attachment%3B%20filename%3Dsqlite3.exe&response-content-type=application%2Foctet-stream).
- Place `sqlite3.exe` in `**nosql version/Assets/`** (same level as the `SQL/` folder).

### 4. Create the database

#### SQLite version

1. Go to the `nosql version/` folder.
2. Ensure `Assets/sqlite3.exe` and the files `Assets/SQL/sqlite_schema.sql`, `Assets/SQL/sqlite_seed.sql` exist.
3. Create and initialize the database (run from the root of `nosql version`):
  ```cmd
   .\Assets\sqlite3.exe Assets\SQL\Warehouse.db ".read Assets/SQL/sqlite_schema.sql"

   .\Assets\sqlite3.exe Assets\SQL\WarehouseTest.db ".read Assets/SQL/sqlite_schema.sql"
   .\Assets\sqlite3.exe Assets\SQL\WarehouseTest.db ".read Assets/SQL/sqlite_seed.sql"
  ```
   The file `Warehouse.db` will be at `Assets/SQL/Warehouse.db`.
4. By default, built exe will use Testing Database, you can swap to Oficial Dartabase by edit this line in App.config:
  `connectionString="Data Source=Assets\SQL\WarehouseTest.db;Version=3;Foreign Keys=True;"`  
   *change to* 
  `connectionString="Data Source=Assets\SQL\Warehouse.db;Version=3;Foreign Keys=True;"`

#### MySQL version

1. Open MySQL Workbench (or any SQL client) and connect to the server.
2. Create the database if needed (e.g. `Warehouse`).
3. Run in order:
  - `mysql version/Assets/SQL/mysql_schema.sql` — creates tables.
  - `mysql version/Assets/SQL/mysql_seed.sql` — (optional) sample data.

### 5. Create App.config

Both versions read the connection string from `App.config`. Do not commit the file with your password; only the template is committed.

- **Step 1:** Copy the example file and remove the `.example` suffix:
  - **SQLite:** `nosql version/App.config.example` → `nosql version/App.config`
  - **MySQL:**  `mysql version/App.config.example`  → `mysql version/App.config`
- **Step 2:** Open the new `App.config` and edit if needed:
  - **SQLite:** Usually no change (path `Data Source=Assets\SQL\Warehouse.db`).
  - **MySQL:** Replace `Pwd=YOUR_PASSWORD` with your MySQL password; change `Database=...` if you used a name other than `Warehouse`.

Example MySQL section in `App.config`:

```xml
<connectionStrings>
  <add name="WarehouseDB"
       connectionString="Server=localhost;Database=Warehouse;Uid=root;Pwd=YOUR_PASSWORD;CharSet=utf8mb4;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

---

## How to build and run

Run all commands from **one** version folder: `nosql version` or `mysql version`.

```powershell
cd "nosql version"   # or cd "mysql version"

dotnet restore
dotnet build
dotnet run
```

Or open the corresponding solution (`.sln`) in Visual Studio and press F5.

---

## Usage

- **Dashboard:** View product list and stock status.
- **Product management:** CRUD for products, categories, suppliers, customers.
- **Stock in/out:** Create transaction vouchers; inventory updates automatically.
- **Search:** Enter a name (or keyword) on the main screen to filter the list.
- **Low-stock alert:** Products below the minimum threshold may be highlighted (e.g. in red) in the grid.
- **Undo:** Use the **Undo** button on the toolbar to revert the last transaction; stock is restored to the previous state.
- **Reports:** Open the reports screen to view in/out trend charts over time.

---

## Outro

The project offers two implementations (SQLite and MySQL) for flexibility: use a local file (SQLite) or a server (MySQL). Code structure and features are the same; only the database connection and initialization scripts differ.

If you get connection errors, check `App.config`, the database file path (SQLite) or server/user/password (MySQL), and ensure the database was created as described above.

## License

MIT License — see `LICENSE`.


## Repository workflow
- `orias` — feature development branch
- `dev` — integration branch for medium-sized features
- `main` — stable releases for large features
