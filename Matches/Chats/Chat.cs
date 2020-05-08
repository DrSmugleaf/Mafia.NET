using Mafia.NET.Players;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    public interface IChat
    {
        string Name { get; }
        IDictionary<IPlayer, IChatParticipant> Participants { get; }
        bool Paused { get; set; }

        bool CanSend(Message message);
        bool Send(Message message);
        void Close();
    }

    public class Chat : IChat
    {
        public string Name { get; }
        private Dictionary<IPlayer, IChatParticipant> _participants { get; }
        public IDictionary<IPlayer, IChatParticipant> Participants { get => _participants; }
        public bool Paused { get; set; }

        public Chat(string name, Dictionary<IPlayer, IChatParticipant> participants)
        {
            Name = name;
            _participants = participants;
        }

        public bool CanSend(Message message)
        {
            return !Paused &&
                _participants.ContainsKey(message.Sender.Owner) &&
                !message.Sender.Muted &&
                message.Text.Length > 0;
        }

        public bool Send(Message message)
        {
            if (!CanSend(message)) return false;

            foreach (var participant in _participants.Values)
            {
                if (!participant.Deaf) participant.Owner.OnMessage(message);
            }

            return true;
        }

        public void Close()
        {
            _participants.Clear();
        }
    }
}
