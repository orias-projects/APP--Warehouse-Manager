using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Repository để quản lý nhật ký hành động (hỗ trợ Undo)
    /// </summary>
    public class ActionsRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách nhật ký (chỉ visible records)
        /// </summary>
        public List<Actions> GetAllLogs()
        {
            var logs = new List<Actions>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Actions WHERE Visible=1 ORDER BY CreatedAt DESC", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new Actions
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhật ký: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Lấy nhật ký theo ID
        /// </summary>
        public Actions GetLogById(int logId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Actions WHERE LogID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", logId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Actions
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy nhật ký ID {logId}: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Thêm nhật ký hành động mới
        /// </summary>
        public int LogAction(Actions log)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Actions (ActionType, Descriptions, DataBefore, CreatedAt, Visible) " +
                        "VALUES (@type, @desc, @dataBefore, @createdAt, 1); SELECT last_insert_rowid();", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", log.ActionType);
                        cmd.Parameters.AddWithValue("@desc", log.Descriptions ?? "");
                        
                        // Xử lý DataBefore - nếu rỗng hoặc không hợp lệ JSON, lưu NULL hoặc "{}"
                        string dataBefore = log.DataBefore ?? "";
                        if (string.IsNullOrWhiteSpace(dataBefore) || dataBefore.Trim() == "")
                        {
                            dataBefore = "{}";
                        }
                        cmd.Parameters.AddWithValue("@dataBefore", dataBefore);
                        
                        cmd.Parameters.AddWithValue("@createdAt", log.CreatedAt);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ghi nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm nhật ký hành động mới (overload với parameters)
        /// </summary>
        public int LogAction(string actionType, string descriptions, string dataBefore = "")
        {
            var log = new Actions
            {
                ActionType = actionType,
                Descriptions = descriptions,
                DataBefore = dataBefore,
                CreatedAt = DateTime.Now
            };
            return LogAction(log);
        }

        /// <summary>
        /// Xóa nhật ký (soft delete)
        /// </summary>
        public bool DeleteLog(int logId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "UPDATE Actions SET Visible=0 WHERE LogID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", logId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy bản ghi log mới nhất (toàn bảng, Visible=1), dùng để kiểm tra hành động liên tiếp xóa/khôi phục.
        /// </summary>
        public Actions GetLatestLog()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT * FROM Actions WHERE Visible=1 ORDER BY CreatedAt DESC LIMIT 1", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Actions
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký mới nhất: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Lấy nhật ký theo loại hành động
        /// </summary>
        public List<Actions> GetLogsByActionType(string actionType)
        {
            var logs = new List<Actions>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT * FROM Actions WHERE ActionType=@type AND Visible=1 ORDER BY CreatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", actionType);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new Actions
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký theo loại: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Lấy nhật ký trong một khoảng thời gian
        /// </summary>
        public List<Actions> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            var logs = new List<Actions>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT * FROM Actions WHERE CreatedAt BETWEEN @start AND @end AND Visible=1 ORDER BY CreatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", endDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new Actions
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký theo ngày: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Xóa hành động khỏi undo stack (soft delete - set Visible=0 để không hiển thị và không undo lại)
        /// </summary>
        public bool RemoveFromUndoStack(int logId)
        {
            return DeleteLog(logId);
        }

        /// <summary>
        /// Xóa tất cả nhật ký (hard delete - xóa hoàn toàn khỏi database)
        /// </summary>
        public bool ClearAllLogs()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "DELETE FROM Actions", conn))
                    {
                        return cmd.ExecuteNonQuery() >= 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa tất cả nhật ký: " + ex.Message);
            }
        }
    }
}