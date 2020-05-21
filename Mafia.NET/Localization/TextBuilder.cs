using System.Collections.Generic;
using System.Drawing;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Localization
{
    public class TextBuilder
    {
        public TextBuilder()
        {
            Location = NotificationLocation.Chat;
            Contents = new List<IContent>();
            Background = Color.Empty;
        }
        
        public NotificationLocation Location { get; set; }
        public List<IContent> Contents { get; set; }
        public Color Background { get; set; }
        
        public TextBuilder Add(string str, Color color = default, double size = 1)
        {
            var content = new Content(str, color, size);
            Contents.Add(content);

            return this;
        }

        public TextBuilder Add(int number, Color color = default, double size = 1)
        {
            return Add(number.ToString(), color, size);
        }

        public TextBuilder Add(IPlayer player, double size = 1)
        {
            return Add(player.Name, player.Color, size);
        }

        public Text Build()
        {
            return new Text(Location, Contents, Background);
        }
    }
}