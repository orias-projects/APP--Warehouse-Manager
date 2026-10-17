using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    public class StatusActionDialog : Form
    {
        /// <param name="allowApprove">False thì nút Duyệt vẫn hiển thị nhưng Enabled = false (không xử lý click), dùng cho user không có quyền admin.</param>
        /// <param name="allowCancel">False thì nút Hủy phiếu bị vô hiệu hóa, chỉ cho phép admin hủy.</param>
        public StatusActionDialog(string title, string message, bool allowApprove = true, bool allowCancel = true)
        {
            InitializeComponent(title, message, allowApprove, allowCancel);
            ApplyTheme();
        }

        private void InitializeComponent(string title, string message, bool allowApprove, bool allowCancel)
        {
            this.Text = title;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(400, 200);
            this.Padding = new Padding(20);

            var lblMessage = new Label
            {
                Text = message,
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = ThemeManager.Instance.FontRegular
            };

            var btnApprove = new CustomButton
            {
                Text = "Duyệt",
                DialogResult = DialogResult.Yes,
                Height = UIConstants.Sizes.ButtonHeight,
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                Enabled = allowApprove
            };
            btnApprove.BackColor = UIConstants.SemanticColors.Success;
            btnApprove.ForeColor = UIConstants.TextOnColor.Default;

            var btnCancelTrans = new CustomButton
            {
                Text = "Huỷ",
                DialogResult = DialogResult.No,
                Height = UIConstants.Sizes.ButtonHeight,
                ButtonStyleType = ButtonStyle.FilledNoOutlineRed,
                Enabled = allowCancel
            };

            var btnExit = new CustomButton
            {
                Text = "Thoát",
                DialogResult = DialogResult.Cancel,
                Height = UIConstants.Sizes.ButtonHeight,
                ButtonStyleType = ButtonStyle.Outlined
            };
            btnExit.BackColor = ThemeManager.Instance.BackgroundDefault;
            btnExit.ForeColor = ThemeManager.Instance.TextPrimary;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                ColumnCount = 3,
                RowCount = 1
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableLayout.Controls.Add(btnApprove, 0, 0);
            tableLayout.Controls.Add(btnCancelTrans, 1, 0);
            tableLayout.Controls.Add(btnExit, 2, 0);
            btnApprove.Dock = DockStyle.Fill;
            btnCancelTrans.Dock = DockStyle.Fill;
            btnExit.Dock = DockStyle.Fill;
            btnApprove.Margin = new Padding(5);
            btnCancelTrans.Margin = new Padding(5);
            btnExit.Margin = new Padding(5);

            this.Controls.Add(tableLayout);
            this.Controls.Add(lblMessage);
        }

        private void ApplyTheme()
        {
            this.BackColor = ThemeManager.Instance.BackgroundDefault;
            this.ForeColor = ThemeManager.Instance.TextPrimary;
        }
    }
}
