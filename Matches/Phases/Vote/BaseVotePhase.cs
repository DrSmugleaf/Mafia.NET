using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Phases.Vote
{
    public abstract class BaseVotePhase : BasePhase, IVotePhase
    {
        private Dictionary<IPlayer, IPlayer> _lynchVotes { get; }
        public IReadOnlyDictionary<IPlayer, IPlayer> LynchVotes { get => _lynchVotes; }
        public IReadOnlyDictionary<IPlayer, int> VotesAgainst { get => LynchVotes.Values.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()); }
        public bool AnonymousVoting { get; }
        public event EventHandler<AccuseEventArgs> TrialVoteAdd;
        public event EventHandler<UnaccuseEventArgs> TrialVoteRemove;

        public BaseVotePhase(string name, int duration = 80, bool anonymousVoting = false) : base(name, duration)
        {
            _lynchVotes = new Dictionary<IPlayer, IPlayer>();
            AnonymousVoting = anonymousVoting;
        }

        public void Add(IPlayer voter, IPlayer accused)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IPlayer voter)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnTrialVoteAdd(AccuseEventArgs e)
        {
            TrialVoteAdd?.Invoke(this, e);
        }

        protected virtual void OnTrialVoteRemove(UnaccuseEventArgs e)
        {
            TrialVoteRemove?.Invoke(this, e);
        }
    }
}
