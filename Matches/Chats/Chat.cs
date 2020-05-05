﻿using Mafia.NET.Players;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    public interface IChat
    {
        IReadOnlyDictionary<IPlayer, IChatParticipant> Participants { get; }

        void Send(Message message);
        void Close();
    }

    public class Chat : IChat
    {
        private Dictionary<IPlayer, IChatParticipant> _participants;
        public IReadOnlyDictionary<IPlayer, IChatParticipant> Participants { get => _participants; }

        public Chat(Dictionary<IPlayer, IChatParticipant> participants)
        {
            _participants = participants;
        }

        public void Send(Message message)
        {
            if (!_participants.ContainsKey(message.Sender.Owner) || message.Sender.Muted) return;
            
            foreach (var participant in _participants.Values)
            {
                if (!participant.Deaf) participant.Owner.OnMessage(message);
            }
        }

        public void Close()
        {
            _participants.Clear();
        }
    }
}
