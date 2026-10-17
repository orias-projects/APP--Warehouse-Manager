using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    public class CustomerRepository : BaseRepository
    {
        public List<Customer> GetAllCustomers(bool includeHidden = false)
        {
            var list = new List<Customer>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = includeHidden 
                        ? "SELECT * FROM Customers" 
                        : "SELECT * FROM Customers WHERE Visible = 1";
                    
                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Customer
                            {
                                CustomerID = reader.GetInt32("CustomerID"),
                                CustomerName = reader.GetString("CustomerName"),
                                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                Visible = reader.GetBoolean("Visible")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách khách hàng: " + ex.Message);
            }
            return list;
        }

        public Customer GetCustomerById(int id)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Customers WHERE CustomerID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Customer
                                {
                                    CustomerID = reader.GetInt32("CustomerID"),
                                    CustomerName = reader.GetString("CustomerName"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                    Visible = reader.GetBoolean("Visible")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy khách hàng ID {id}: " + ex.Message);
            }
            return null;
        }

        public int AddCustomer(Customer customer)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Customers (CustomerName, Phone, Address, Email, Visible) VALUES (@name, @phone, @address, @email, @visible); SELECT last_insert_rowid();", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@phone", customer.Phone);
                        cmd.Parameters.AddWithValue("@address", customer.Address);
                        cmd.Parameters.AddWithValue("@email", customer.Email);
                        cmd.Parameters.AddWithValue("@visible", customer.Visible);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm khách hàng: " + ex.Message);
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "UPDATE Customers SET CustomerName=@name, Phone=@phone, Address=@address, Email=@email, Visible=@visible WHERE CustomerID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@phone", customer.Phone);
                        cmd.Parameters.AddWithValue("@address", customer.Address);
                        cmd.Parameters.AddWithValue("@email", customer.Email);
                        cmd.Parameters.AddWithValue("@visible", customer.Visible);
                        cmd.Parameters.AddWithValue("@id", customer.CustomerID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật khách hàng: " + ex.Message);
            }
        }

        public bool SoftDeleteCustomer(int id)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("UPDATE Customers SET Visible=0 WHERE CustomerID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa mềm khách hàng: " + ex.Message);
            }
        }
    }
}
