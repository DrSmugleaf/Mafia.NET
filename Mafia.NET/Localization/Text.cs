using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Localization
{
    public interface IText
    {
        NotificationLocation Location { get; }
        List<IContent> Contents { get; }
        Color Background { get; }

        Text Add(string str, Color color = default, double size = 1);
        Text Add(int number, Color color = default, double size = 1);
        Text Add(IPlayer player, double size = 1);
        int Length();
    }

    public class Text : IText
    {
        public Text(NotificationLocation location, Color background = default)
        {
            Contents = new List<IContent>();
            Background = background;
            Location = location;
        }

        public NotificationLocation Location { get; }
        public List<IContent> Contents { get; }
        public Color Background { get; set; }

        public Text Add(string str, Color color = default, double size = 1)
        {
            var content = new Content(str, color, size);
            Contents.Add(content);

            return this;
        }

        public Text Add(int number, Color color = default, double size = 1)
        {
            return Add(number.ToString(), color, size);
        }

        public Text Add(IPlayer player, double size = 1)
        {
            return Add(player.Name, player.Color, size);
        }

        public int Length()
        {
            return Contents.Select(content => content.Str.Length).Sum();
        }
    }
}