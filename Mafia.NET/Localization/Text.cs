using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;

namespace Mafia.NET.Localization
{
    public interface IText
    {
        string String { get; }
        IImmutableList<IContent> Contents { get; }
        Color Background { get; }
        int Length { get; }
    }

    public class Text : IText
    {
        public static readonly Text Empty = new Text(new List<IContent>(), Color.Empty);

        public Text(IEnumerable<IContent> contents, Color background)
        {
            Contents = contents.ToImmutableList();
            String = string.Join("", Contents.Select(content => content.Str));
            Background = background;
            Length = Contents.Select(content => content.Str.Length).Sum();
        }

        public string String { get; }
        public IImmutableList<IContent> Contents { get; }
        public Color Background { get; }
        public int Length { get; }

        public override string ToString()
        {
            return String;
        }
    }
}