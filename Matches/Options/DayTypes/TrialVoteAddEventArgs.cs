using Mafia.NET.Players;
using System;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public class TrialVoteAddEventArgs : EventArgs
    {
        public IPlayer Voter { get; }
        public IPlayer Accused { get; }

        public TrialVoteAddEventArgs(IPlayer voter, IPlayer accused)
        {
            Voter = voter;
            Accused = accused;
        }
    }
}
