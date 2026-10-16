using System.Drawing;

namespace WarehouseManagement.UI
{
    /// <summary>
    /// Định nghĩa tất cả các constants cho UI - Colors, Fonts, Sizes, Spacing
    /// </summary>
    public static class UIConstants
    {
        // ========== COLORS ==========
        
        // Primary Color - 5A9F68 (xanh lá) và các sắc độ
        public static class PrimaryColor
        {
            public static readonly Color Default = ColorTranslator.FromHtml("#5A9F68");
            public static readonly Color Active = ColorTranslator.FromHtml("#4A8F58");   // Đậm hơn khi active
            public static readonly Color Hover = ColorTranslator.FromHtml("#6EAD7A");   // Sáng hơn khi hover
            public static readonly Color Pressed = ColorTranslator.FromHtml("#4A8A55"); // Tối hơn khi pressed
            public static readonly Color Light = ColorTranslator.FromHtml("#E0F2E4");   // Màu nhẹ cho selection
            public static readonly Color HoverLight = ColorTranslator.FromHtml("#F1F8F2"); // Rất nhạt cho hover hàng
            public static readonly Color Disabled = ColorTranslator.FromHtml("#A8D4B0"); // Mờ đi khi disabled
            public static readonly Color Dark = ColorTranslator.FromHtml("#3D6B47");     // Rất tối
        }

        // Background Colors - Light Theme
        public static class BackgroundLight
        {
            public static readonly Color Default = ColorTranslator.FromHtml("#FFFFFF");
            public static readonly Color Lighter = ColorTranslator.FromHtml("#FAFAFA");
            public static readonly Color Light = ColorTranslator.FromHtml("#F5F5F5");
            public static readonly Color Medium = ColorTranslator.FromHtml("#F0F0F0");
            public static readonly Color Dark = ColorTranslator.FromHtml("#E8E8E8");
            public static readonly Color Darker = ColorTranslator.FromHtml("#D8D8D8");
        }

        // Background Colors - Dark Theme
        public static class BackgroundDark
        {
            public static readonly Color Default = ColorTranslator.FromHtml("#1E1E1E");
            public static readonly Color Lighter = ColorTranslator.FromHtml("#2D2D2D");
            public static readonly Color Light = ColorTranslator.FromHtml("#252525");
            public static readonly Color Medium = ColorTranslator.FromHtml("#1A1A1A");
            public static readonly Color Dark = ColorTranslator.FromHtml("#151515");
            public static readonly Color Darker = ColorTranslator.FromHtml("#0D0D0D");
        }

        // Text Colors - Light Theme
        public static class TextLight
        {
            public static readonly Color Primary = ColorTranslator.FromHtml("#212121");
            public static readonly Color Secondary = ColorTranslator.FromHtml("#666666");
            public static readonly Color Disabled = ColorTranslator.FromHtml("#9E9E9E");
            public static readonly Color Hint = ColorTranslator.FromHtml("#BDBDBD");
        }

        // Text Colors - Dark Theme
        public static class TextDark
        {
            public static readonly Color Primary = ColorTranslator.FromHtml("#FFFFFF");
            public static readonly Color Secondary = ColorTranslator.FromHtml("#B0B0B0");
            public static readonly Color Disabled = ColorTranslator.FromHtml("#6E6E6E");
            public static readonly Color Hint = ColorTranslator.FromHtml("#4A4A4A");
        }

        // Semantic Colors
        public static class SemanticColors
        {
            public static readonly Color Success = ColorTranslator.FromHtml("#4CAF50");
            public static readonly Color SuccessLight = ColorTranslator.FromHtml("#81C784");
            public static readonly Color Warning = ColorTranslator.FromHtml("#FF9800");
            public static readonly Color Error = ColorTranslator.FromHtml("#F44336");
            public static readonly Color ErrorLight = ColorTranslator.FromHtml("#EF9A9A");
            public static readonly Color Info = ColorTranslator.FromHtml("#2196F3");
        }

        // Text on colored background (primary/semantic buttons, headers)
        public static class TextOnColor
        {
            public static readonly Color Default = Color.White;
        }

        // Chart & grid (biểu đồ, lưới)
        public static class ChartColors
        {
            public static readonly Color GridLine = Color.FromArgb(230, 230, 230);
            public static readonly Color Background = Color.White;
            public static readonly Color Label = Color.Gray;
            public static readonly Color LegendBorder = Color.Gray;
        }

        // Neutral UI (border, shadow)
        public static class NeutralColors
        {
            public static readonly Color Border = Color.Gray;
            public static readonly Color Shadow = Color.FromArgb(20, 0, 0, 0);
        }

        // ========== FONTS ==========
        
        public static class Fonts
        {
            public const string FontFamily = "Segoe UI";
            
            // Font Sizes (8 cấp độ)
            public const float XXSmall = 9f;   // Rất nhỏ - chú thích phụ
            public const float XSmall = 10f;   // Nhỏ - label phụ
            public const float Small = 11f;    // Nhỏ vừa - label thường
            public const float Regular = 12f;  // Chuẩn - text thường
            public const float Medium = 14f;   // Vừa - subheading
            public const float Large = 16f;    // Lớn - heading
            public const float XLarge = 20f;   // Rất lớn - title
            public const float XXLarge = 24f;  // Cực lớn - main title
        }

        // ========== SIZES ==========
        
        public static class Sizes
        {
            // Button Sizes
            public const int ButtonHeight = 36;
            public const int ButtonWidthSmall = 80;
            public const int ButtonWidthMedium = 120;
            public const int ButtonWidthLarge = 160;

            // Input Sizes
            public const int InputHeight = 36;
            public const int InputHeightSmall = 28;
            public const int InputHeightLarge = 44;

            // Table Row Height
            public const int TableRowHeight = 40;
            public const int TableHeaderHeight = 44;

            // Icon Sizes
            public const int IconSmall = 16;
            public const int IconMedium = 20;
            public const int IconLarge = 24;
        }

        // ========== SPACING ==========
        
        public static class Spacing
        {
            // Padding (vùng an toàn trong)
            public static class Padding
            {
                public const int XXSmall = 2;
                public const int XSmall = 4;
                public const int Small = 8;
                public const int Medium = 12;
                public const int Large = 16;
                public const int XLarge = 20;
                public const int XXLarge = 24;
                
                // Button Padding
                public static readonly System.Windows.Forms.Padding Button = 
                    new System.Windows.Forms.Padding(Medium, Small, Medium, Small);
                
                // Input Padding
                public static readonly System.Windows.Forms.Padding Input = 
                    new System.Windows.Forms.Padding(Medium, Small, Medium, Small);
                
                // Panel Padding
                public static readonly System.Windows.Forms.Padding Panel = 
                    new System.Windows.Forms.Padding(Large);
            }

            // Margin (vùng an toàn ngoài)
            public static class Margin
            {
                public const int XXSmall = 2;
                public const int XSmall = 4;
                public const int Small = 8;
                public const int Medium = 12;
                public const int Large = 16;
                public const int XLarge = 20;
                public const int XXLarge = 24;
                
                // Button Margin
                public static readonly System.Windows.Forms.Padding Button = 
                    new System.Windows.Forms.Padding(Small);
                
                // Input Margin
                public static readonly System.Windows.Forms.Padding Input = 
                    new System.Windows.Forms.Padding(Small);
                
                // Panel Margin
                public static readonly System.Windows.Forms.Padding Panel = 
                    new System.Windows.Forms.Padding(Medium);
            }
        }

        // ========== BORDERS ==========
        
        public static class Borders
        {
            // Border Radius
            public const int RadiusNone = 0;
            public const int RadiusSmall = 4;
            public const int RadiusMedium = 8;
            public const int RadiusLarge = 12;
            public const int RadiusXLarge = 16;
            public const int RadiusFull = 999; // Circular

            // Border Width
            public const int BorderThickness = 1;
            public const int BorderThicknessMedium = 2;
            public const int BorderThicknessThick = 3;
        }

        // ========== ANIMATIONS ==========
        
        public static class Animations
        {
            public const int DurationFast = 100;      // ms
            public const int DurationNormal = 200;    // ms
            public const int DurationSlow = 300;      // ms
        }

        // ========== ICONS ==========
        // Bộ ký hiệu Unicode đơn ký tự, đồng kích thước, hiển thị tốt với Segoe UI
        public static class Icons
        {
            public const string Add = "\u271A";         // ✚
            public const string Edit = "\u270E";        // ✎
            public const string Noo = "\u2716";         // ✖
            public const string Hmm = "\u26A0";         // ⚠
            public const string Yes = "\u2714";         // ✔
            public const string Print = "\u2399";       // ⎙
            public const string Down = "\u25BC";        // ▼
            public const string Up = "\u25B2";          // ▲
            public const string Left = "\u25C0";        // ◀
            public const string Right = "\u25B6";       // ▶
            public const string Report = "\u259A";      // ▚

            public const string Details = "\u27C1";     // ⟁
            public const string Dashboard = "\u259E";   // ▞
            
            public const string Categories = "\u2637";  // ☷
            public const string Actions = "\u2714";     // ✔
            public const string Settings = "\u274B";    // ❋
            public const string Account = "\u2726";     // ✦

            public const string Money = "\u25C8";       // ◈
            public const string Moon = "\u25CB";        // ○
            public const string Sun = "\u25C9";         // ◉
            public const string Radio = "\u29BF";       // ⦿
            public const string Important = "\u2757";   // ❗
        }
    }
}
