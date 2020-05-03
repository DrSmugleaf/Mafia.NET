using Mafia.NET.Players;
using System;

namespace Mafia.NET.Matches.Players.Votes
{
    public class AccuseEventArgs : EventArgs
    {
        public IPlayer Voter { get; }
        public IPlayer Accused { get; }

        public AccuseEventArgs(IPlayer voter, IPlayer accused)
        {
            Voter = voter;
            Accused = accused;
        }
    }
}
