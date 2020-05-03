using Mafia.NET.Players;
using System;

namespace Mafia.NET.Matches.Players.Votes
{
    public class UnaccuseEventArgs : EventArgs
    {
        public IPlayer Voter { get; }
        public IPlayer Unaccused { get; }

        public UnaccuseEventArgs(IPlayer voter, IPlayer unaccused)
        {
            Voter = voter;
            Unaccused = unaccused;
        }
    }
}
