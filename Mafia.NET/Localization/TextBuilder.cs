using System.Collections.Generic;
using System.Drawing;
using Mafia.NET.Players;

namespace Mafia.NET.Localization
{
    public class TextBuilder
    {
        public TextBuilder()
        {
            Contents = new List<IContent>();
            Background = Color.Empty;
        }

        public List<IContent> Contents { get; set; }
        public Color Background { get; set; }

        public TextBuilder Add(IContent content)
        {
            Contents.Add(content);
            return this;
        }

        public TextBuilder Add(string str, Color color = default, double size = 1)
        {
            var content = new Content(str, color, size);
            return Add(content);
        }

        public TextBuilder Add(int number, Color color = default, double size = 1)
        {
            return Add(number.ToString(), color, size);
        }

        public TextBuilder Add(IPlayer player, double size = 1)
        {
            foreach (var content in player.Name.Contents)
                Add(content);

            return this;
        }

        public Text Build()
        {
            return new Text(Contents, Background);
        }
    }
}