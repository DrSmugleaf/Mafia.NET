using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mafia.NET.Matches.Chats;

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
                player.Notification -= OnNotification;
                player.Notification += OnNotification;
            }

            Task.Delay(3000).ContinueWith(t => match.Start());
        }

        public virtual void OnMatchEnd(object sender, MatchEnd match)
        {
            Matches.TryRemove(match.Match.Id, out _);
        }

        public virtual void OnNotification(object sender, Notification notification) => Expression.Empty();
    }
}