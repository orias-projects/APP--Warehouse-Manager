using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.Controllers;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    public class UserForm : Form
    {
        private readonly UserController _controller;
        private User _user;
        private readonly bool _isEditMode;

        private CustomTextBox txtUsername;
        private CustomTextBox txtFullName;
        private CustomTextBox txtPassword;
        private CustomComboBox cmbRole;
        private CheckBox chkIsActive;
        private CustomButton btnSave;
        private CustomButton btnCancel;

        public UserForm(User user = null)
        {
            _controller = new UserController();
            _user = user;
            _isEditMode = user != null;

            InitializeComponent();
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            Text = _isEditMode ? "Sửa Nhân Viên" : "Thêm Nhân Viên";

            CustomPanel mainPanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = false,
                Padding = new Padding(0)
            };

            const int INPUT_WIDTH = 400;
            const int LEFT_MARGIN = 40;
            int currentY = 30;
            int inputSpacing = 20;

            Label CreateStyledLabel(string text, int y)
            {
                return new Label
                {
                    Text = text,
                    Left = LEFT_MARGIN,
                    Top = y,
                    AutoSize = true,
                    Font = ThemeManager.Instance.FontSmall,
                    ForeColor = Color.FromArgb(180,
                        UIConstants.PrimaryColor.Default.R,
                        UIConstants.PrimaryColor.Default.G,
                        UIConstants.PrimaryColor.Default.B),
                    TabStop = false
                };
            }

            // Username
            mainPanel.Controls.Add(CreateStyledLabel("Tài khoản (*)", currentY));
            currentY += 20;

            txtUsername = new CustomTextBox
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH,
                Placeholder = "Nhập tài khoản đăng nhập..."
            };
            mainPanel.Controls.Add(txtUsername);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Full name
            mainPanel.Controls.Add(CreateStyledLabel("Họ tên", currentY));
            currentY += 20;

            txtFullName = new CustomTextBox
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH,
                Placeholder = "Nhập họ tên nhân viên..."
            };
            mainPanel.Controls.Add(txtFullName);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Password
            mainPanel.Controls.Add(CreateStyledLabel(_isEditMode ? "Mật khẩu (bỏ trống nếu giữ nguyên)" : "Mật khẩu (*)", currentY));
            currentY += 20;

            txtPassword = new CustomTextBox
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH,
                Placeholder = _isEditMode ? "Để trống nếu không đổi mật khẩu..." : "Nhập mật khẩu...",
                IsPassword = true
            };
            mainPanel.Controls.Add(txtPassword);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Role
            mainPanel.Controls.Add(CreateStyledLabel("Vai trò", currentY));
            currentY += 20;

            cmbRole = new CustomComboBox
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH,
                TabIndex = 3
            };
            // UI không cho chọn role Admin; chỉ Manager/Staff
            cmbRole.Items.AddRange(new object[] { "Manager", "Staff" });
            mainPanel.Controls.Add(cmbRole);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // IsActive
            chkIsActive = new CheckBox
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Text = "Đang hoạt động",
                AutoSize = true,
                Checked = true
            };
            mainPanel.Controls.Add(chkIsActive);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Buttons
            int totalButtonWidth = 120 + 10 + 120;
            int buttonStartX = LEFT_MARGIN + (INPUT_WIDTH - totalButtonWidth) / 2;

            btnSave = new CustomButton
            {
                Text = "Lưu",
                Left = buttonStartX,
                Top = currentY,
                Width = 120,
                Height = 35,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new CustomButton
            {
                Text = "Hủy",
                Left = buttonStartX + 120 + 10,
                Top = currentY,
                Width = 120,
                Height = 35,
                ButtonStyleType = ButtonStyle.Outlined,
                CausesValidation = false
            };
            btnCancel.Click += (s, e) => Close();

            mainPanel.Controls.Add(btnSave);
            mainPanel.Controls.Add(btnCancel);

            Controls.Add(mainPanel);

            // Kích thước form: căn theo ProductForm
            ClientSize = new Size(480, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundDefault;
            AcceptButton = btnSave;
            CancelButton = btnCancel;

            if (_isEditMode && _user != null)
            {
                txtUsername.Text = _user.Username;
                txtUsername.Enabled = false;
                txtFullName.Text = _user.FullName;
                chkIsActive.Checked = _user.IsActive;

                // Nếu user là Admin, không cho mở form này từ UI (đã chặn ở UsersPanel),
                // nhưng để an toàn: khóa role và không map Admin vào combo.
                if (_user.IsAdmin)
                {
                    cmbRole.Enabled = false;
                }
                else
                {
                    cmbRole.SelectedItem = string.IsNullOrEmpty(_user.Role) || _user.IsManager
                        ? "Manager"
                        : "Staff";
                }
            }
            else
            {
                cmbRole.SelectedItem = "Staff";
                chkIsActive.Checked = true;
            }

            Load += (s, e) =>
            {
                if (btnSave != null) ActiveControl = btnSave;
            };
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string fullName = txtFullName.Text.Trim();
                string password = txtPassword.Text;
                string role = cmbRole.SelectedItem?.ToString() ?? "Staff";
                bool isActive = chkIsActive.Checked;

                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Vui lòng nhập tài khoản đăng nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!_isEditMode && string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!_isEditMode && _controller.UsernameExists(username))
                {
                    MessageBox.Show("Tài khoản đã tồn tại, vui lòng chọn tên khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var userRepo = new Repositories.UserRepository();

                if (_isEditMode)
                {
                    var existing = userRepo.GetUserById(_user.UserID);
                    if (existing == null)
                    {
                        MessageBox.Show("Không tìm thấy người dùng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    existing.FullName = fullName;
                    existing.Role = role;
                    existing.IsActive = isActive;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        existing.SetPassword(password);
                    }

                    if (userRepo.UpdateUser(existing))
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật người dùng thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var newUser = new User
                    {
                        Username = username,
                        FullName = fullName,
                        Role = role,
                        IsActive = isActive,
                        CreatedAt = DateTime.Now,
                        Visible = true
                    };
                    newUser.SetPassword(password);

                    int newId = userRepo.AddUser(newUser);
                    if (newId > 0)
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Thêm người dùng thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

