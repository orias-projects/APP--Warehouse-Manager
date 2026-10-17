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
    public class CustomersPanel : Panel, ISearchable
    {
        private DataGridView dgvCustomers;
        private CustomerController _controller;
        private List<Customer> _allCustomers;

        public CustomersPanel()
        {
            _controller = new CustomerController();
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
            dgvCustomers = new DataGridView
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
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "CustomerID", 
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter, 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });
            
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên Khách Hàng", 
                DataPropertyName = "CustomerName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
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

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
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

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
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
            dgvCustomers.Columns.Add(new DataGridViewLinkColumn 
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
            foreach (DataGridViewColumn col in dgvCustomers.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvCustomers.CellFormatting += DgvCustomers_CellFormatting;
            dgvCustomers.CellClick += DgvCustomers_CellClick;

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
            tablePanel.Controls.Add(dgvCustomers);
            Controls.Add(tablePanel);
            
            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvCustomers);

            LoadData();
        }

        private void OnThemeChanged(object sender, EventArgs e) => ApplyTheme();

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvCustomers.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvCustomers.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvCustomers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvCustomers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            
            // Apply Selection Effect through Helper
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvCustomers);
            
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = UIConstants.TextOnColor.Default;
        }

        public void LoadData()
        {
            try
            {
                _allCustomers = _controller.GetAllCustomers(SettingsForm.ShowHiddenItems);
                dgvCustomers.DataSource = _allCustomers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu khách hàng: " + ex.Message);
            }
        }

        public void Search(string text)
        {
            if (_allCustomers == null) return;
            string keyword = text.ToLower();
            var filtered = _allCustomers.FindAll(c => 
                c.CustomerName.ToLower().Contains(keyword) || 
                (c.Phone != null && c.Phone.Contains(keyword)) || 
                (c.Email != null && c.Email.ToLower().Contains(keyword)));
            dgvCustomers.DataSource = filtered;
        }

        private void DgvCustomers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCustomers.Rows.Count) return;
            if (e.ColumnIndex != 5) return;

            var customer = dgvCustomers.Rows[e.RowIndex].DataBoundItem as Customer;
            if (customer != null)
                e.Value = customer.Visible ? UIConstants.Icons.Noo : UIConstants.Icons.Left;
        }

        private void DgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var customer = dgvCustomers.Rows[e.RowIndex].DataBoundItem as Customer;
            if (customer == null) return;

            int customerId = customer.CustomerID;
            string customerName = customer.CustomerName ?? "";

            // Cột Xóa/Restore (index 5)
            if (e.ColumnIndex == 5)
            {
                if (customer.Visible)
                {
                    DialogResult result = MessageBox.Show(
                        $"{UIConstants.Icons.Hmm} Bạn chắc chắn muốn xóa khách hàng '{customerName}'?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _controller.SoftDeleteCustomer(customerId);
                            MessageBox.Show($"{UIConstants.Icons.Yes} Khách hàng đã được xóa thành công.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi xóa khách hàng: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        $"{UIConstants.Icons.Down} Bạn muốn phục hồi khách hàng '{customerName}'?",
                        "Xác nhận phục hồi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _controller.RestoreCustomer(customerId);
                            MessageBox.Show($"{UIConstants.Icons.Yes} Khách hàng đã được phục hồi.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{UIConstants.Icons.Noo} Lỗi phục hồi khách hàng: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                return;
            }

            // Other columns open Edit
            var customerToEdit = dgvCustomers.Rows[e.RowIndex].DataBoundItem as Customer;
            if (customerToEdit != null)
            {
                CustomerForm form = new CustomerForm(customerToEdit);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
    }
}
