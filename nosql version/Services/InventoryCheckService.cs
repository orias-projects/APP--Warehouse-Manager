using System;
using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Services
{
    public class InventoryCheckService
    {
        private readonly InventoryCheckRepository _checkRepo;
        private readonly ProductRepository _productRepo;

        public InventoryCheckService()
        {
            _checkRepo = new InventoryCheckRepository();
            _productRepo = new ProductRepository(); 
        }

        public List<InventoryCheck> GetAllChecks()
        {
            return _checkRepo.GetAllChecks();
        }

        public InventoryCheck GetCheckById(int id)
        {
            return _checkRepo.GetCheckById(id);
        }

        public int CreateCheck(int userId, string note, List<InventoryCheckDetail> details, string status = "Pending")
        {
            try
            {
                if (details == null || details.Count == 0)
                    throw new ArgumentException("Danh sách kiểm kê không được trống");

                var check = new InventoryCheck
                {
                    CreatedByUserID = userId,
                    Note = note,
                    Status = status,
                    Visible = true,
                    CheckDate = DateTime.Now
                };

                // Validate products and check duplicates
                var productIds = new List<int>();
                foreach(var detail in details) {
                     if (!_productRepo.ProductIdExists(detail.ProductID))
                        throw new ArgumentException($"Sản phẩm ID {detail.ProductID} không tồn tại");
                     if (productIds.Contains(detail.ProductID))
                        throw new ArgumentException($"Sản phẩm ID {detail.ProductID} bị trùng lặp");
                     productIds.Add(detail.ProductID);
                }

                int checkId = _checkRepo.CreateCheck(check);
                
                if (checkId > 0)
                {
                    foreach (var detail in details)
                    {
                        detail.CheckID = checkId;
                        _checkRepo.AddCheckDetail(detail);
                    }
                    if (status == "Approved")
                    {
                        ProcessStockAdjustment(checkId, details, userId);
                    }
                }
                
                return checkId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo phiếu kiểm kê: " + ex.Message);
            }
        }

        public void ApproveCheck(int checkId, int userId)
        {
            try
            {
                var check = _checkRepo.GetCheckById(checkId);
                if (check == null) throw new Exception("Không tìm thấy phiếu kiểm kê");
                if (check.Status != "Pending") throw new Exception("Chỉ có thể duyệt phiếu đang chờ");

                _checkRepo.UpdateStatus(checkId, "Approved");
                ProcessStockAdjustment(checkId, check.Details, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi duyệt kiểm kê: " + ex.Message);
            }
        }

        public void CancelCheck(int checkId, int userId)
        {
            try
            {
                var check = _checkRepo.GetCheckById(checkId);
                if (check == null) throw new Exception("Không tìm thấy phiếu kiểm kê");
                if (check.Status != "Pending") throw new Exception("Chỉ có thể hủy phiếu đang chờ");

                _checkRepo.UpdateStatus(checkId, "Cancelled");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi hủy kiểm kê: " + ex.Message);
            }
        }

        /// <summary>
        /// Khi duyệt kiểm kê: cộng/trừ số lượng trong kho theo lượng chênh lệch (ActualQuantity - SystemQuantity).
        /// </summary>
        private void ProcessStockAdjustment(int checkId, List<InventoryCheckDetail> details, int userId)
        {
            foreach (var detail in details)
            {
                int diff = detail.ActualQuantity - detail.SystemQuantity;
                if (diff == 0) continue;

                var product = _productRepo.GetProductById(detail.ProductID);
                if (product == null)
                    throw new Exception($"Sản phẩm ID {detail.ProductID} không tồn tại.");

                int newQuantity = product.Quantity + diff;
                if (newQuantity < 0)
                    throw new Exception($"Sản phẩm '{product.ProductName}' (ID {detail.ProductID}): không thể điều chỉnh tồn kho xuống âm (hiện {product.Quantity}, chênh lệch {diff}).");

                _productRepo.UpdateQuantity(detail.ProductID, newQuantity);
            }
        }
    }
}
