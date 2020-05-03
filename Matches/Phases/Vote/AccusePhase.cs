using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases.Vote
{
    class AccusePhase : BasePhase
    {
        IPhase Procedure { get; }
        IReadOnlyDictionary<int, IPlayer> Voters { get; }
        event EventHandler<ProcedureStartEventArgs> ProcedureStart;

        public AccusePhase(IMatch match, int duration = 80) : base(match, "Time Left", duration, new NightPhase(match))
        {
            Procedure = match.Settings.Procedure;
            Voters = match.LivingPlayers;
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

        public override void Start()
        {
            foreach (var voter in Voters.Values)
            {
                voter.Accuse += Accused;
                voter.Unaccuse += Unaccused;
            }
        }

        public override IPhase End(IMatch match)
        {
            foreach (var voter in Voters.Values)
            {
                voter.Accuse -= Accused;
                voter.Unaccuse -= Unaccused;
            }

            return base.End(match);
        }
    }
}
