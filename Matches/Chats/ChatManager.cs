using Mafia.NET.Players;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    public class ChatManager
    {
        private List<IChat> _chats { get; }
        public IReadOnlyList<IChat> Chats { get => _chats; }

        public ChatManager()
        {
            _chats = new List<IChat>();
        }

        public void Open(IChat chat)
        {
            _chats.Add(chat);
        }

        public void Open(IEnumerable<IPlayer> players, bool muted = false)
        {
            var participants = new Dictionary<IPlayer, IChatParticipant>();

            foreach (var player in players)
            {
                var participant = new ChatParticipant(player, !player.Alive || muted);
                participants[player] = participant;
            }

            var chat = new Chat(participants);
            Open(chat);
        }

        public void Send(Message message)
        {
            foreach (var chat in _chats) chat.Send(message);
        }

        public void Pause()
        {
            foreach (var chat in _chats) chat.Paused = true;
        }

        public void Resume()
        {
            foreach (var chat in _chats) chat.Paused = false;
        }

        public void Close()
        {
            foreach (var chat in _chats) chat.Close();
            _chats.Clear();
        }
    }
}
