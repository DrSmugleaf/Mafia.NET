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

        Text With(string text);
    }

    public class Text : IText
    {
        public static readonly Text Empty = new Text(new List<IContent>());

        public Text(IEnumerable<IContent> contents, Color background = default)
        {
            Contents = contents.ToImmutableList();
            String = string.Join("", Contents.Select(content => content.Str));
            Background = background;
        }

        public Text(Color background = default, params Text[] texts)
        {
            Contents = texts.SelectMany(text => text.Contents).ToImmutableList();
            String = string.Join("", Contents.Select(content => content.Str));
            Background = background;
        }

        public string String { get; }
        public IImmutableList<IContent> Contents { get; }
        public Color Background { get; }

        public Text With(string text)
        {
            return new Text(Contents.Add(new Content(text)), Background);
        }

        public override string ToString()
        {
            return String;
        }
    }
}