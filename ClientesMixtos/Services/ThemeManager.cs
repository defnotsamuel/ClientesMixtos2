using System.Windows;
using System.Windows.Media;

namespace ClientesMixtos.Services
{
    public static class ThemeManager
    {
        public static void Apply(string theme)
        {
            bool dark = theme == "Dark";

            SetBrush("BrushWindowBg", dark ? Color.FromRgb(0x11, 0x18, 0x27) : Color.FromRgb(0xF3, 0xF4, 0xF6));
            SetBrush("BrushCardBg", dark ? Color.FromRgb(0x1F, 0x29, 0x37) : Color.FromRgb(0xFF, 0xFF, 0xFF));
            SetBrush("BrushSurfaceLight", dark ? Color.FromRgb(0x37, 0x41, 0x51) : Color.FromRgb(0xF9, 0xFA, 0xFB));

            SetBrush("BrushTextPrimary", dark ? Color.FromRgb(0xF9, 0xFA, 0xFB) : Color.FromRgb(0x11, 0x18, 0x27));
            SetBrush("BrushTextSecondary", dark ? Color.FromRgb(0xD1, 0xD5, 0xDB) : Color.FromRgb(0x37, 0x41, 0x51));
            SetBrush("BrushTextMuted", dark ? Color.FromRgb(0x9C, 0xA3, 0xAF) : Color.FromRgb(0x6B, 0x72, 0x80));
            SetBrush("BrushTextDisabled", dark ? Color.FromRgb(0x6B, 0x72, 0x80) : Color.FromRgb(0x9C, 0xA3, 0xAF));

            SetBrush("BrushBorder", dark ? Color.FromRgb(0x37, 0x41, 0x51) : Color.FromRgb(0xE5, 0xE7, 0xEB));
            SetBrush("BrushBorderInput", dark ? Color.FromRgb(0x4B, 0x55, 0x63) : Color.FromRgb(0xD1, 0xD5, 0xDB));
            SetBrush("BrushBorderLight", dark ? Color.FromRgb(0x37, 0x41, 0x51) : Color.FromRgb(0xF3, 0xF4, 0xF6));

            SetBrush("BrushPrimaryLight", dark ? Color.FromRgb(0x1E, 0x3A, 0x5F) : Color.FromRgb(0xDB, 0xEA, 0xFE));
            SetBrush("BrushSelected", dark ? Color.FromRgb(0x1E, 0x3A, 0x5F) : Color.FromRgb(0xBF, 0xDB, 0xFE));

            SetBrush("BrushDangerLight", dark ? Color.FromRgb(0x7F, 0x1D, 0x1D) : Color.FromRgb(0xFC, 0xA5, 0xA5));
            SetBrush("BrushWarningLight", dark ? Color.FromRgb(0x71, 0x3F, 0x12) : Color.FromRgb(0xFD, 0xE0, 0x47));
            SetBrush("BrushSuccessLight", dark ? Color.FromRgb(0x14, 0x53, 0x2D) : Color.FromRgb(0x86, 0xEF, 0xAC));
        }

        private static void SetBrush(string key, Color color)
        {
            Application.Current.Resources[key] = new SolidColorBrush(color);
        }
    }
}
