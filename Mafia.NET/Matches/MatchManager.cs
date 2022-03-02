using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Mafia.NET.Localization;

namespace Mafia.NET.Matches
{
    public class MatchManager
    {
        public MatchManager()
        {
            Matches = new ConcurrentDictionary<Guid, IMatch>();
        }

        protected ConcurrentDictionary<Guid, IMatch> Matches { get; }

        public void Add(IMatch match)
        {
            Matches[match.Id] = match;
            match.MatchEnd -= OnMatchEnd;
            match.MatchEnd += OnMatchEnd;

            foreach (var player in match.AllPlayers)
            {
                player.Chat -= OnChat;
                player.Chat += OnChat;

                player.Popup -= OnPopup;
                player.Popup += OnPopup;
            }

            Task.Delay(3000).ContinueWith(t => match.Start());
        }

        public virtual void OnMatchEnd(object? sender, MatchEnd match)
        {
            Matches.TryRemove(match.Match.Id, out _);
        }

        public virtual void OnChat(object? sender, Text notification)
        {
        }

        public virtual void OnPopup(object? sender, Text notification)
        {
        }
    }
}