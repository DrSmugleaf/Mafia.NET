using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public interface IDayType
    {
        IMatch Match { get; }
        IReadOnlyDictionary<IPlayer, IPlayer> LynchVotes { get; }
        IReadOnlyDictionary<IPlayer, int> VotesAgainst { get; }
        bool AnonymousVoting { get; }
        event EventHandler<TrialVoteAddEventArgs> TrialVoteAdd;
        event EventHandler<TrialVoteRemoveEventArgs> TrialVoteRemove;

        public BasePhase VotingPhase();
        public void Add(IPlayer voter, IPlayer accused);
        public bool Remove(IPlayer voter);
    }
}
