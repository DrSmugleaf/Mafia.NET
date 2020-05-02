using Mafia.NET.Players;
using System;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public class TrialVoteRemoveEventArgs : EventArgs
    {
        public IPlayer Voter { get; }

        public TrialVoteRemoveEventArgs(IPlayer voter)
        {
            Voter = voter;
        }
    }
}
