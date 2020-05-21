using System.Drawing;

namespace Mafia.NET.Localization
{
    public interface IContent
    {
        public string Str { get; set; }
        public Color Color { get; set; }
        public double Size { get; set; }
    }

    public class Content : IContent
    {
        public Content(string str, Color color, double size = 1)
        {
            Str = str;
            Color = color;
            Size = size;
        }

        public string Str { get; set; }
        public Color Color { get; set; }
        public double Size { get; set; }
    }
}