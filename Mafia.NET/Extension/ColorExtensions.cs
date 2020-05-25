using System.Drawing;

namespace Mafia.NET.Extension
{
    public static class ColorExtensions
    {
        public static string HexRgb(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color Brightness(this Color color, double multiplier)
        {
            return Color.FromArgb(
                color.A,
                (int) (color.R * multiplier),
                (int) (color.G * multiplier),
                (int) (color.B * multiplier));
        }
    }
}