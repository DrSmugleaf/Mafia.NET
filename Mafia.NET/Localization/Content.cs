using System;
using System.Drawing;

namespace Mafia.NET.Localization;

public interface IContent
{
    public string Str { get; }
    public Color Color { get; }
    public double Size { get; }
}

public class Content : IContent
{
    public Content(string str, Color color = default, double size = 1)
    {
        Str = str;
        Color = color;
        Size = size;
    }

    public string Str { get; }
    public Color Color { get; }
    public double Size { get; }

    public override bool Equals(object? obj)
    {
        return obj is IContent o &&
               Str.Equals(o.Str) &&
               Color.Equals(o.Color) &&
               Size.Equals(o.Size);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Str, Color, Size);
    }

    public override string ToString()
    {
        return Str;
    }
}