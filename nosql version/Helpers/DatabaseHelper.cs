using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Data.SQLite;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Helpers
{
    /// <summary>
    /// Helper kiểm tra kết nối database và backup
    /// </summary>
    public class DatabaseHelper
    {
        public DatabaseHelper()
        {
            // BaseRepository là abstract, không cần khởi tạo
        }

        /// <summary>
        /// Kiểm tra kết nối đến database
        /// </summary>
        public static bool TestDatabaseConnection()
        {
            try
            {
                var repo = new ProductRepository();
                return repo.TestConnection();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy thông báo lỗi kết nối
        /// </summary>
        public static string GetConnectionError()
        {
            try
            {
                if (!TestDatabaseConnection())
                    return "Không thể kết nối tới database. Vui lòng kiểm tra App.config";
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }

        /// <summary>
        /// Migration cho SQLite (thêm cột nếu thiếu - tương thích schema mới)
        /// </summary>
        public static void RunMigration()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    // SQLite dùng sqlite_schema.sql; RunMigration chỉ giữ tương thích nếu cần ALTER sau này
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Migration: " + ex.Message);
            }
        }

        private static SQLiteConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["WarehouseDB"].ConnectionString;
            return new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// Chạy schema SQLite để tạo/cập nhật database
        /// </summary>
        public static bool ExecuteSchema()
        {
            try
            {
                string schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "SQL", "sqlite_schema.sql");
                if (!File.Exists(schemaPath))
                    throw new FileNotFoundException("Không tìm thấy file sqlite_schema.sql");

                string sqlScript = File.ReadAllText(schemaPath, Encoding.UTF8);

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var pragmaCmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn))
                        pragmaCmd.ExecuteNonQuery();
                    string[] commands = sqlScript.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string command in commands)
                    {
                        string trimmed = command.Trim();
                        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("--"))
                            continue;
                        using (var cmd = new SQLiteCommand(trimmed, conn))
                        {
                            cmd.CommandTimeout = 30;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi chạy schema: " + ex.Message);
            }
        }

        /// <summary>
        /// Tạo file backup đơn giản (chỉ lưu thông tin)
        /// </summary>
        public static bool CreateBackup(string backupPath)
        {
            try
            {
                if (!Directory.Exists(backupPath))
                    Directory.CreateDirectory(backupPath);

                string filename = Path.Combine(backupPath, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                using (var writer = new StreamWriter(filename))
                {
                    writer.WriteLine($"Backup created at: {DateTime.Now}");
                    writer.WriteLine("Database: Warehouse.db (SQLite)");
                    writer.WriteLine("Status: OK");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo backup: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy kích thước database (file SQLite)
        /// </summary>
        public static string GetDatabaseSize()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["WarehouseDB"]?.ConnectionString ?? "";
                string dataSource = "";
                foreach (var part in connStr.Split(';'))
                {
                    var kv = part.Trim().Split(new[] { '=' }, 2, StringSplitOptions.None);
                    if (kv.Length == 2 && kv[0].Trim().Equals("Data Source", StringComparison.OrdinalIgnoreCase))
                    {
                        dataSource = kv[1].Trim();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(dataSource))
                    return "N/A";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataSource);
                if (!File.Exists(fullPath))
                    return "0 KB";
                long bytes = new FileInfo(fullPath).Length;
                if (bytes < 1024) return bytes + " B";
                if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("F1") + " KB";
                return (bytes / (1024.0 * 1024)).ToString("F1") + " MB";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }
    }
}
