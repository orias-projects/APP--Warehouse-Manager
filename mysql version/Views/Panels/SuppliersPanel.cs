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
    public class SuppliersPanel : Panel, ISearchable
    {
        private DataGridView dgvSuppliers;
        private SupplierController _controller;
        private List<Supplier> _allSuppliers;

        public SuppliersPanel()
        {
            _controller = new SupplierController();
            InitializeComponent();
            SettingsForm.SettingsChanged += (s, e) => LoadData();
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Instance.BackgroundDefault;

            // DataGridView
            dgvSuppliers = new DataGridView
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

            // Columns
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "SupplierID", 
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter, 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });
            
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên Nhà Cung Cấp", 
                DataPropertyName = "SupplierName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Số Điện Thoại", 
                DataPropertyName = "Phone", 
                Width = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter, 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Email", 
                DataPropertyName = "Email", 
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Địa Chỉ", 
                DataPropertyName = "Address", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            // Action Column: Xóa (ẩn trong DB)
            dgvSuppliers.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Noo,
                LinkColor = ThemeManager.Instance.TextPrimary,
                ActiveLinkColor = UIConstants.SemanticColors.Error,
                VisitedLinkColor = ThemeManager.Instance.TextPrimary,
                LinkBehavior = LinkBehavior.NeverUnderline,
                TrackVisitedState = false,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 10, 5)
                }
            });

            // Header Styling Sync
            foreach (DataGridViewColumn col in dgvSuppliers.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvSuppliers.CellFormatting += DgvSuppliers_CellFormatting;
            dgvSuppliers.CellClick += DgvSuppliers_CellClick;

            // Container Panel
            CustomPanel tablePanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                HasShadow = true,
                ShowBorder = false,
                Padding = new Padding(10),
                BorderRadius = UIConstants.Borders.RadiusMedium,
                BackColor = ThemeManager.Instance.BackgroundDefault
            };
            tablePanel.Controls.Add(dgvSuppliers);
            Controls.Add(tablePanel);

            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvSuppliers);

            LoadData();
        }

        private void OnThemeChanged(object sender, EventArgs e) => ApplyTheme();

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvSuppliers.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvSuppliers.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvSuppliers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvSuppliers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            
            // Apply Selection Effect through Helper
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvSuppliers);

            dgvSuppliers.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = UIConstants.TextOnColor.Default;
        }

        public void LoadData()
        {
            try
            {
                _allSuppliers = _controller.GetAllSuppliers(SettingsForm.ShowHiddenItems);
                dgvSuppliers.DataSource = _allSuppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách nhà cung cấp: " + ex.Message);
            }
        }

        public void Search(string text)
        {
            if (_allSuppliers == null) return;
            string keyword = text.ToLower();
            var filtered = _allSuppliers.FindAll(s => 
                s.SupplierID.ToString().Contains(keyword) || 
                s.SupplierName.ToLower().Contains(keyword) || 
                (s.Phone != null && s.Phone.Contains(keyword)) ||
                (s.Email != null && s.Email.ToLower().Contains(keyword)));
            dgvSuppliers.DataSource = filtered;
        }

        private void DgvSuppliers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvSuppliers.Rows.Count) return;
            if (e.ColumnIndex != 5) return;

            var supplier = dgvSuppliers.Rows[e.RowIndex].DataBoundItem as Supplier;
            if (supplier != null)
                e.Value = supplier.Visible ? UIConstants.Icons.Noo : UIConstants.Icons.Left;
        }

        private void DgvSuppliers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var supplier = dgvSuppliers.Rows[e.RowIndex].DataBoundItem as Supplier;
            if (supplier == null) return;

            int supplierId = supplier.SupplierID;
            string supplierName = supplier.SupplierName ?? "";

            // Cột Xóa/Restore (index 5)
            if (e.ColumnIndex == 5)
            {
                if (supplier.Visible)
                {
                    DialogResult result = MessageBox.Show(
                        $"{UIConstants.Icons.Hmm} Bạn chắc chắn muốn xóa nhà cung cấp '{supplierName}'?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _controller.SoftDeleteSupplier(supplierId);
                            MessageBox.Show($"{UIConstants.Icons.Yes} Nhà cung cấp đã được xóa thành công.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi xóa nhà cung cấp: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        $"{UIConstants.Icons.Down} Bạn muốn phục hồi nhà cung cấp '{supplierName}'?",
                        "Xác nhận phục hồi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _controller.RestoreSupplier(supplierId);
                            MessageBox.Show($"{UIConstants.Icons.Yes} Nhà cung cấp đã được phục hồi.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi phục hồi nhà cung cấp: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                return;
            }

            // Other columns open Edit
            var supplierToEdit = dgvSuppliers.Rows[e.RowIndex].DataBoundItem as Supplier;
            if (supplierToEdit != null)
            {
                SupplierForm form = new SupplierForm(supplierToEdit);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
    }
}
