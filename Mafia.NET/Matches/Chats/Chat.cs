using System.Collections.Generic;
using JetBrains.Annotations;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats
{
    public interface IChat
    {
        string Name { get; }
        IDictionary<IPlayer, IChatParticipant> Participants { get; }
        bool Paused { get; set; }

        IChat Add(IDictionary<IPlayer, IChatParticipant> players);
        IChat Add(IEnumerable<IPlayer> players, bool muted = false, bool deaf = false);
        bool CanSend(MessageIn messageIn);
        MessageOut Send(MessageIn messageIn);
        [CanBeNull] MessageOut Send(IPlayer player, string message);
        void Close();
    }

    public class Chat : IChat
    {
        public Chat(string name)
        {
            Name = name;
            _participants = new Dictionary<IPlayer, IChatParticipant>();
        }

        public Chat(string name, Dictionary<IPlayer, IChatParticipant> participants)
        {
            Name = name;
            _participants = participants;
        }

        private readonly Dictionary<IPlayer, IChatParticipant> _participants;
        public string Name { get; }
        public IDictionary<IPlayer, IChatParticipant> Participants => _participants;
        public bool Paused { get; set; }

        public IChat Add(IDictionary<IPlayer, IChatParticipant> participants)
        {
            foreach (var participant in participants)
            {
                if (Participants.ContainsKey(participant.Key)) continue;
                Participants.Add(participant);
            }

            return this;
        }

        public IChat Add(IEnumerable<IPlayer> players, bool muted = false, bool deaf = false)
        {
            var participants = new Dictionary<IPlayer, IChatParticipant>();

            foreach (var player in players)
            {
                var participant = new ChatParticipant(player, muted, deaf);
                participants[player] = participant;
            }

            return Add(participants);
        }

        public bool CanSend(MessageIn messageIn)
        {
            return !Paused &&
                   _participants.ContainsKey(messageIn.Sender.Owner) &&
                   !messageIn.Sender.Muted &&
                   messageIn.Text.Length > 0;
        }

        public MessageOut Send(MessageIn messageIn)
        {
            if (!CanSend(messageIn)) return new MessageOut(messageIn);

            var listeners = new HashSet<IPlayer>();
            
            foreach (var participant in _participants.Values)
                if (!participant.Deaf)
                    listeners.Add(participant.Owner);

            return new MessageOut(messageIn, listeners);
        }

        public MessageOut Send(IPlayer player, string text)
        {
            if (!_participants.ContainsKey(player)) return null;
            
            var participant = _participants[player];
            var message = new MessageIn(participant, text);
            
            return Send(message);
        }

        public void Close()
        {
            _participants.Clear();
        }
    }
}