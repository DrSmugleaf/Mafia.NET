using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players;

namespace Mafia.NET.Matches
{
    public interface IVictory
    {
        IImmutableList<IPlayer> Winners { get; }
        Entry Popup { get; }
        Entry WinnersList { get; }
    }

    public class Victory : IVictory
    {
        public Victory(IList<IPlayer> winners, Entry popup)
        {
            Winners = winners.ToImmutableList();
            Popup = popup;
            var winnerNames = string.Join(", ", Winners.Select(winner => winner.Name));
            WinnersList = Entry.Popup(DayKey.Congratulations, winnerNames);
        }

        public Victory(IPlayer winner, Entry popup) : this(new List<IPlayer> {winner}, popup)
        {
        }

        public IImmutableList<IPlayer> Winners { get; }
        public Entry Popup { get; }
        public Entry WinnersList { get; }
    }
}