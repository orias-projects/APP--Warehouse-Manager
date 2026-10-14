using System;
using System.Data.SQLite;

namespace WarehouseManagement.Helpers
{
    /// <summary>
    /// Extension methods cho SQLiteDataReader để đọc theo tên cột (giống MySqlDataReader).
    /// </summary>
    public static class SQLiteDataReaderExtensions
    {
        public static int GetInt32(this SQLiteDataReader reader, string name)
        {
            return reader.GetInt32(reader.GetOrdinal(name));
        }

        public static string GetString(this SQLiteDataReader reader, string name)
        {
            return reader.GetString(reader.GetOrdinal(name));
        }

        public static decimal GetDecimal(this SQLiteDataReader reader, string name)
        {
            return reader.GetDecimal(reader.GetOrdinal(name));
        }

        public static bool GetBoolean(this SQLiteDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            if (reader.IsDBNull(ordinal)) return false;
            return reader.GetInt64(ordinal) != 0;
        }

        public static DateTime GetDateTime(this SQLiteDataReader reader, string name)
        {
            return reader.GetDateTime(reader.GetOrdinal(name));
        }

        public static double GetDouble(this SQLiteDataReader reader, string name)
        {
            return reader.GetDouble(reader.GetOrdinal(name));
        }
    }
}
