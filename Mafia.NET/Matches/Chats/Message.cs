using System.Collections.Generic;
using System.Collections.Immutable;
using Mafia.NET.Localization;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats
{
    public class MessageIn
    {
        public MessageIn(IChatParticipant sender, string text)
        {
            Sender = sender;
            Text = text.Trim();
        }

        public IChatParticipant Sender { get; }
        public string Text { get; }
    }

    public class MessageOut
    {
        public MessageOut(IChatParticipant sender, string text, HashSet<IPlayer> listeners = null)
        {
            Sender = sender;
            Listeners = listeners?.ToImmutableHashSet() ?? ImmutableHashSet<IPlayer>.Empty;
            Text = text;
        }

        public MessageOut(MessageIn message, HashSet<IPlayer> listeners = null) : this(message.Sender, message.Text,
            listeners)
        {
        }

        public IChatParticipant Sender { get; }
        public IImmutableSet<IPlayer> Listeners { get; }
        public string Text { get; }

        public Text DisplayText(IPlayer listener)
        {
            return Sender.DisplayName(listener).With($": {Text}");
        }
    }
}