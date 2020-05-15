using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats
{
    public class ChatManager
    {
        public const string MainName = "Main Chat";

        public ChatManager()
        {
            _chats = new Dictionary<string, IChat>();
        }

        private readonly Dictionary<string, IChat> _chats;
        public IReadOnlyDictionary<string, IChat> Chats => _chats;

        public IChat Open(IChat chat)
        {
            if (_chats.ContainsKey(chat.Name))
            {
                var old = _chats[chat.Name];

                foreach (var participant in chat.Participants) old.Participants.Add(participant);

                return old;
            }

            _chats[chat.Name] = chat;
            return chat;
        }

        public IChat Open(IEnumerable<IPlayer> players, bool muted = false, string name = MainName)
        {
            var participants = new Dictionary<IPlayer, IChatParticipant>();

            foreach (var player in players)
            {
                var participant = new ChatParticipant(player, !player.Alive || muted);
                participants[player] = participant;
            }

            if (Chats.TryGetValue(name, out var existing)) return existing.Add(participants);

            var chat = new Chat(name, participants);
            return Open(chat);
        }

        public IChat Open(string name = MainName, bool muted = false, params IPlayer[] players)
        {
            return Open(players.AsEnumerable(), muted, name);
        }

        public IChat Open(string name = MainName, params IPlayer[] players)
        {
            return Open(name, false, players);
        }

        public void DisableExcept(IPlayer player, IChat except = null)
        {
            foreach (var chat in Chats.Values)
            {
                if (!chat.Participants.ContainsKey(player) || chat == except) continue;
                var participant = chat.Participants[player];
                participant.Muted = true;
                participant.Deaf = true;
            }
        }

        public void UnMute()
        {
            foreach (var participant in Chats[MainName].Participants) participant.Value.Muted = false;
        }

        public void UnDeafen()
        {
            foreach (var participant in Chats[MainName].Participants) participant.Value.Deaf = false;
        }

        public void UnMuteUnDeafen()
        {
            UnMute();
            UnDeafen();
        }

        public IChat Main()
        {
            return Chats[MainName];
        }

        public List<MessageOut> Send(IPlayer player, string text)
        {
            var messages = new List<MessageOut>();
            
            foreach (var chat in _chats.Values)
            {
                var message = chat.Send(player, text);
                messages.Add(message);
            }
            
            return messages;
        }

        public void Pause()
        {
            foreach (var chat in _chats.Values) chat.Paused = true;
        }

        public void Resume()
        {
            foreach (var chat in _chats.Values) chat.Paused = false;
        }

        public void Close()
        {
            foreach (var chat in _chats.Values) chat.Close();
            _chats.Clear();
        }
    }
}