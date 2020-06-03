using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats
{
    public class ChatManager
    {
        public const string MainName = "Day Chat";

        private readonly Dictionary<string, IChat> _chats;

        public ChatManager(IMatch match)
        {
            Match = match;
            _chats = new Dictionary<string, IChat>();
        }

        public IMatch Match { get; }
        public IReadOnlyDictionary<string, IChat> Chats => _chats;

        public IChat Open(string id)
        {
            if (_chats.TryGetValue(id, out var chat))
                return chat;

            chat = new Chat(id);
            chat.Initialize(Match);
            return _chats[id] = chat;
        }

        public IChat Open<T>(string id = null) where T : IChat, new()
        {
            if (id != null && _chats.TryGetValue(id, out var chat))
                return chat;

            chat = new T();
            if (id != null) chat.Id = id;
            id = chat.Id;

            chat.Initialize(Match);

            return _chats[id] = chat;
        }

        public IChat Open(IEnumerable<IPlayer> players, string id, bool muted = false)
        {
            var chat = Open(id);
            return chat.Add(players, muted);
        }

        public IChat Open(string name, bool muted = false, params IPlayer[] players)
        {
            return Open(players.AsEnumerable(), name, muted);
        }

        public IChat Open(string name, params IPlayer[] players)
        {
            return Open(name, false, players);
        }

        public void DisableForExcept(IPlayer player, IChat except = null)
        {
            foreach (var chat in Chats.Values)
                if (chat != except)
                    chat.Disable(player);
        }

        public IChat Main()
        {
            return Open<MainChat>(MainName);
        }

        public List<MessageOut> Send(IPlayer player, string text)
        {
            var messages = new List<MessageOut>();

            foreach (var chat in _chats.Values)
                if (chat.TrySend(player, text, out var messageOut))
                    messages.Add(messageOut);

            return messages;
        }

        public void Close()
        {
            foreach (var chat in _chats.Values) chat.Close();
            _chats.Clear();
        }
    }

    public class MainChat : Chat
    {
        public static readonly string MainName = "Day Chat";

        public MainChat() : base(MainName)
        {
        }

        public override void Initialize(IMatch match)
        {
            if (Initialized) return;

            foreach (var player in match.AllPlayers)
                Add(player);

            Initialized = true;
        }
    }
}