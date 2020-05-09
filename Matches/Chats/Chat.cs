﻿using Mafia.NET.Players;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Chats
{
    public interface IChat
    {
        string Name { get; }
        IDictionary<IPlayer, IChatParticipant> Participants { get; }
        bool Paused { get; set; }

        IChat Add(IDictionary<IPlayer, IChatParticipant> players);
        IChat Add(IEnumerable<IPlayer> players, bool muted = false, bool deaf = false);
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
