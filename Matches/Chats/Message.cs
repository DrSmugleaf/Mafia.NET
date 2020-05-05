using System;

namespace Mafia.NET.Matches.Chats
{
    public class Message : EventArgs
    {
        public IChatParticipant Sender { get; }
        public string Text { get; }

        public Message(IChatParticipant sender, string text)
        {
            Sender = sender;
            Text = text;
        }
    }
}
