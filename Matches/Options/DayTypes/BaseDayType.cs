using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public abstract class BaseDayType : IDayType
    {
        private Dictionary<IPlayer, IPlayer> _lynchVotes { get; }
        public IMatch Match { get; }
        public IReadOnlyDictionary<IPlayer, IPlayer> LynchVotes { get => _lynchVotes; }
        public IReadOnlyDictionary<IPlayer, int> VotesAgainst { get => LynchVotes.Values.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()); }
        public bool AnonymousVoting { get; }
        public event EventHandler<TrialVoteAddEventArgs> TrialVoteAdd;
        public event EventHandler<TrialVoteRemoveEventArgs> TrialVoteRemove;

        public BaseDayType(IMatch match, bool anonymousVoting = false)
        {
            Match = match;
            _lynchVotes = new Dictionary<IPlayer, IPlayer>();
            AnonymousVoting = anonymousVoting;
        }

        public abstract BasePhase VotingPhase();

        public void Add(IPlayer voter, IPlayer accused)
        {
            _lynchVotes[voter] = accused;
            var ev = new TrialVoteAddEventArgs(voter, accused);
            OnTrialVoteAdd(ev);
        }

        public bool Remove(IPlayer voter)
        {
            var remove = _lynchVotes.Remove(voter);
            var ev = new TrialVoteRemoveEventArgs(voter);
            OnTrialVoteRemove(ev);
            return remove;
        }

        protected virtual void OnTrialVoteAdd(TrialVoteAddEventArgs e)
        {
            TrialVoteAdd?.Invoke(this, e);
        }

        protected virtual void OnTrialVoteRemove(TrialVoteRemoveEventArgs e)
        {
            TrialVoteRemove?.Invoke(this, e);
        }
    }
}
