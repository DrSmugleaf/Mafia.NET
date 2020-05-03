using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public abstract class BaseVoting : IVoting
    {
        public IPhase Procedure { get; }
        public IReadOnlyDictionary<int, IPlayer> Voters { get; }
        public event EventHandler<ProcedureStartEventArgs> ProcedureStart;

        public BaseVoting(Dictionary<int, IPlayer> voters, IPhase procedure)
        {
            Voters = voters;
            Procedure = procedure;

            foreach (var voter in voters.Values)
            {
                voter.Accuse += Accused;
                voter.Unaccuse += Unaccused;
            }
        }

        public IList<IPlayer> VotesAgainst(IPlayer accused)
        {
            IList<IPlayer> votes = new List<IPlayer>();

            foreach (var living in Voters.Values)
            {
                if (living.Accuses == accused)
                {
                    votes.Add(living);
                }
            }

            return votes;
        }

        protected void Accused(object sender, AccuseEventArgs e)
        {
            if (VotesAgainst(e.Accused).Count > Voters.Count / 2)
            {
                var procedure = new ProcedureStartEventArgs(e.Accused, Voters, Procedure);
                OnProcedureStart(procedure);
            }
        }

        protected void Unaccused(object sender, UnaccuseEventArgs e)
        {
        }

        protected virtual void OnProcedureStart(ProcedureStartEventArgs e)
        {
            ProcedureStart?.Invoke(this, e);
        }
    }
}
