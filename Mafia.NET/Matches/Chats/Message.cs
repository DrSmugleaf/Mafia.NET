using System;

namespace Mafia.NET.Matches.Chats
{
    public class Message : EventArgs
    {
        public Message(IChatParticipant sender, string text)
        {
            Sender = sender;
            Text = text.Trim();
        }

        public IChatParticipant Sender { get; }
        public string Text { get; }
    }
}