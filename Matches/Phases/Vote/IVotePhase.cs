using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases.Vote
{
    public interface IVotePhase : IPhase
    {
        IReadOnlyDictionary<IPlayer, IPlayer> LynchVotes { get; }
        IReadOnlyDictionary<IPlayer, int> VotesAgainst { get; }
        bool AnonymousVoting { get; }
        event EventHandler<AccuseEventArgs> TrialVoteAdd;
        event EventHandler<UnaccuseEventArgs> TrialVoteRemove;

        public void Add(IPlayer voter, IPlayer accused);
        public bool Remove(IPlayer voter);
    }
}
