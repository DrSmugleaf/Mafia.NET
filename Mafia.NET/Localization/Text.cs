using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Localization
{
    public interface IText
    {
        NotificationLocation Location { get; }
        IImmutableList<IContent> Contents { get; }
        Color Background { get; }
        int Length { get; }
    }

    public class Text : IText
    {
        public static readonly Text Empty = new Text(NotificationLocation.Chat, new List<IContent>(), Color.Empty);
        
        public Text(NotificationLocation location, IEnumerable<IContent> contents, Color background)
        {
            Contents = contents.ToImmutableList();
            Background = background;
            Location = location;
            Length = Contents.Select(content => content.Str.Length).Sum();
        }

        public NotificationLocation Location { get; }
        public IImmutableList<IContent> Contents { get; }
        public Color Background { get; }
        public int Length { get; }
    }
}