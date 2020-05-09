using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Matches
{
    public interface IVictory
    {
        IImmutableList<IPlayer> Winners { get; }
        Notification Popup { get; }
        Notification WinnersList { get; }
    }

    public class Victory : IVictory
    {
        public IImmutableList<IPlayer> Winners { get; }
        public Notification Popup { get; }
        public Notification WinnersList { get; }

        public Victory(IList<IPlayer> winners, Notification popup)
        {
            Winners = winners.ToImmutableList();
            Popup = popup;
            var winnerNames = string.Join(", ", Winners.Select(winner => winner.Name));
            WinnersList = Notification.Popup($"Congratulations ${winnerNames}");
        }

        public Victory(IPlayer winner, Notification popup) : this(new List<IPlayer> { winner }, popup)
        {
        }
    }
}
