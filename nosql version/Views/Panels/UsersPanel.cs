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
    public class UsersPanel : Panel, ISearchable
    {
        private DataGridView dgvUsers;
        private UserController _controller;
        private List<User> _allUsers;

        public UsersPanel()
        {
            _controller = new UserController();
            InitializeComponent();
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
            LoadData();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Instance.BackgroundDefault;

            dgvUsers = new DataGridView
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

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                DataPropertyName = "UserID",
                Width = 80,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 20, 5)
                }
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tài khoản",
                DataPropertyName = "Username",
                Width = 140,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 20, 5)
                }
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Họ tên",
                DataPropertyName = "FullName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Padding = new Padding(10, 5, 20, 5)
                }
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Vai trò",
                DataPropertyName = "Role",
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 20, 5)
                }
            });

            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Hoạt động",
                DataPropertyName = "IsActive",
                Width = 90,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                ThreeState = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày tạo",
                DataPropertyName = "CreatedAt",
                Width = 160,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy HH:mm",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 20, 5)
                }
            });

            dgvUsers.Columns.Add(new DataGridViewLinkColumn
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

            foreach (DataGridViewColumn col in dgvUsers.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvUsers.CellFormatting += DgvUsers_CellFormatting;
            dgvUsers.CellClick += DgvUsers_CellClick;

            CustomPanel tablePanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                HasShadow = true,
                ShowBorder = false,
                Padding = new Padding(10),
                BorderRadius = UIConstants.Borders.RadiusMedium,
                BackColor = ThemeManager.Instance.BackgroundDefault
            };
            tablePanel.Controls.Add(dgvUsers);
            Controls.Add(tablePanel);

            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvUsers);
        }

        private void OnThemeChanged(object sender, EventArgs e) => ApplyTheme();

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvUsers.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvUsers.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvUsers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;

            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvUsers);

            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = UIConstants.TextOnColor.Default;
        }

        public void LoadData()
        {
            try
            {
                var repo = new Repositories.UserRepository();
                // Không hiển thị / thao tác trên tài khoản Admin trong UI
                _allUsers = repo.GetAllUsers(SettingsForm.ShowHiddenItems)
                    .FindAll(u => !u.IsAdmin);
                dgvUsers.DataSource = _allUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu nhân viên: " + ex.Message);
            }
        }

        public void Search(string text)
        {
            if (_allUsers == null) return;
            string keyword = text.ToLower();
            var filtered = _allUsers.FindAll(u =>
                u.Username.ToLower().Contains(keyword) ||
                (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(u.Role) && u.Role.ToLower().Contains(keyword)));
            dgvUsers.DataSource = filtered;
        }

        private void DgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvUsers.Rows.Count) return;
            if (e.ColumnIndex != dgvUsers.Columns.Count - 1) return;

            var user = dgvUsers.Rows[e.RowIndex].DataBoundItem as User;
            if (user == null) return;

            // Hiển thị icon giống ProductsPanel: Visible => icon xóa, ẩn => icon restore
            e.Value = user.Visible ? UIConstants.Icons.Noo : UIConstants.Icons.Left;
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var user = dgvUsers.Rows[e.RowIndex].DataBoundItem as User;
            if (user == null || user.IsAdmin) return;

            // Cột cuối: toggle Visible (ẩn/hiện user)
            if (e.ColumnIndex == dgvUsers.Columns.Count - 1)
            {
                if (user.UserID == GlobalUser.CurrentUser?.UserID)
                {
                    MessageBox.Show("Không thể ẩn tài khoản đang đăng nhập.", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var repo = new Repositories.UserRepository();
                try
                {
                    if (user.Visible)
                    {
                        var result = MessageBox.Show(
                            $"{UIConstants.Icons.Hmm} Bạn chắc chắn muốn ẩn tài khoản '{user.Username}'?",
                            "Xác nhận",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            repo.SoftDeleteUser(user.UserID);
                            LoadData();
                        }
                    }
                    else
                    {
                        user.Visible = true;
                        repo.UpdateUser(user);
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi cập nhật nhân viên: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // Click vào các cột khác: mở form sửa giống các bảng khác
            using (var form = new UserForm(user))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
    }
}

