using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Panels
{
    public class ProductsPanel : Panel, ISearchable
    {
        private DataGridView dgvProducts;
        private ProductController _productController;
        private CategoryController _categoryController;
        private List<Product> allProducts;
        private Dictionary<int, string> _categoryNames = new Dictionary<int, string>();

        public ProductsPanel()
        {
            _productController = new ProductController();
            _categoryController = new CategoryController();
            InitializeComponent();
            SettingsForm.SettingsChanged += (s, e) => LoadData();
            
            // Subscribe to theme changes
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Instance.BackgroundDefault;

            // DataGridView
            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = UIConstants.PrimaryColor.Default,
                    ForeColor = UIConstants.TextOnColor.Default,
                    Font = ThemeManager.Instance.FontBold
                },
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToResizeRows = false,
                Font = ThemeManager.Instance.FontRegular,
                RowTemplate = { Height = UIConstants.Sizes.TableRowHeight },
                ColumnHeadersHeight = UIConstants.Sizes.TableHeaderHeight
            };

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "ProductID", 
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 30, 5)
                }
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên Sản Phẩm", 
                DataPropertyName = "ProductName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5)
                }
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Danh Mục", 
                DataPropertyName = "CategoryID", // Value overridden in CellFormatting
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5)
                }
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Giá", 
                DataPropertyName = "Price", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0", 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 5, 30, 5)
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tồn Kho", 
                DataPropertyName = "Quantity", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 5, 30, 5)
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Ngưỡng Min", 
                DataPropertyName = "MinThreshold", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 5, 30, 5)
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tổng Giá Trị", 
                DataPropertyName = "InventoryValue", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0", 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 5, 30, 5)
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Noo,
                LinkBehavior = LinkBehavior.NeverUnderline,
                LinkColor = ThemeManager.Instance.TextPrimary,
                ActiveLinkColor = UIConstants.SemanticColors.Error,
                VisitedLinkColor = ThemeManager.Instance.TextPrimary,
                TrackVisitedState = false,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 10, 5)
                }
            });

            // Sync alignment và padding từ content lên header
            foreach (DataGridViewColumn col in dgvProducts.Columns)
            {
                // Copy alignment
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                {
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                }
                // Copy padding (vì khi set alignment, padding có thể bị mất)
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvProducts.CellFormatting += DgvProducts_CellFormatting;
            dgvProducts.CellClick += DgvProducts_CellClick;
            dgvProducts.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                    LoadData();
            };

            CustomPanel tablePanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                HasShadow = true,
                ShowBorder = false,
                Padding = new Padding(10),
                BorderRadius = UIConstants.Borders.RadiusMedium,
                BackColor = ThemeManager.Instance.BackgroundDefault // Ensure card look
            };
            tablePanel.Controls.Add(dgvProducts);
            Controls.Add(tablePanel);
            
            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvProducts);
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvProducts.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvProducts.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvProducts.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvProducts.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            
            // Apply Selection Effect through Helper
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvProducts);

            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = UIConstants.TextOnColor.Default;
        }

        public void LoadData()
        {
            try
            {
                // Load Categories for lookup
                var categories = _categoryController.GetAllCategories();
                _categoryNames = new Dictionary<int, string>();
                foreach (var cat in categories)
                {
                    if (!_categoryNames.ContainsKey(cat.CategoryID))
                        _categoryNames.Add(cat.CategoryID, cat.CategoryName);
                }

                allProducts = _productController.GetAllProducts(SettingsForm.ShowHiddenItems);
                dgvProducts.DataSource = allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Search(string searchText)
        {
            try
            {
                string search = searchText.ToLower();
                List<Product> filtered = allProducts.FindAll(p => 
                    p.ProductID.ToString().Contains(search) ||
                    p.ProductName.ToLower().Contains(search) ||
                    p.Price.ToString().Contains(search) ||
                    p.Quantity.ToString().Contains(search)
                );
                dgvProducts.DataSource = filtered;
            }
            catch { }
        }

        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvProducts.Rows.Count) return;

            var product = dgvProducts.Rows[e.RowIndex].DataBoundItem as Product;
            if (product == null) return;

            // Cột Xóa/Restore (index 7): hiển thị Restore nếu đã ẩn, Delete nếu đang hiện
            if (e.ColumnIndex == 7)
            {
                e.Value = product.Visible ? UIConstants.Icons.Noo : UIConstants.Icons.Left;
                return;
            }

            // Resolve Category Name
            if (dgvProducts.Columns[e.ColumnIndex].DataPropertyName == "CategoryID")
            {
                if (_categoryNames != null && _categoryNames.TryGetValue(product.CategoryID, out string catName))
                {
                    e.Value = catName;
                    e.FormattingApplied = true;
                }
            }

            // Low Stock Highlighting
            if (product.IsLowStock)
            {
                e.CellStyle.BackColor = UIConstants.SemanticColors.Error;
                e.CellStyle.ForeColor = UIConstants.TextOnColor.Default;
                e.CellStyle.Font = ThemeManager.Instance.FontBold;
            }
            else
            {
                 // Chỉ reset Font để bỏ bold; không set BackColor/ForeColor để row inherit
                 // từ Row.DefaultCellStyle — giúp hover (DataGridViewHelper) hiển thị đúng.
                 e.CellStyle.Font = dgvProducts.DefaultCellStyle.Font;
            }
        }

        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Cột Xóa/Restore (index 7)
            if (e.ColumnIndex == 7)
            {
                var product = dgvProducts.Rows[e.RowIndex].DataBoundItem as Product;
                if (product == null) return;

                int productId = product.ProductID;
                string productName = product.ProductName ?? "";

                if (product.Visible)
                {
                    DialogResult result = MessageBox.Show(
                        $"{UIConstants.Icons.Hmm} Bạn chắc chắn muốn xóa sản phẩm '{productName}'?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _productController.HideProduct(productId);
                            MessageBox.Show($"{UIConstants.Icons.Yes} Sản phẩm đã được xóa thành công.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi xóa sản phẩm: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        $"{UIConstants.Icons.Down} Bạn muốn phục hồi sản phẩm '{productName}'?",
                        "Xác nhận phục hồi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _productController.RestoreProduct(productId);
                            MessageBox.Show($"{UIConstants.Icons.Yes} Sản phẩm đã được phục hồi.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi phục hồi sản phẩm: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                return;
            }

            // Other columns open the edit form
            int id = (int)dgvProducts.Rows[e.RowIndex].Cells[0].Value;
            ProductForm form = new ProductForm(id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.Instance.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }
    }
}
