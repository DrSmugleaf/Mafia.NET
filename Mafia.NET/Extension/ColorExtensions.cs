using System.Drawing;

namespace Mafia.NET.Extension
{
    public static class ColorExtensions
    {
        public static string HexRgb(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}