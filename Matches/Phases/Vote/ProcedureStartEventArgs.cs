using Mafia.NET.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ProcedureStartEventArgs : EventArgs
    {
        public IPlayer Against;
        public IImmutableDictionary<int, IPlayer> Voters;
        public IPhase Phase;

        public ProcedureStartEventArgs(IPlayer player, IReadOnlyDictionary<int, IPlayer> voters, IPhase phase)
        {
            Against = player;
            Voters = voters.ToImmutableDictionary();
            Phase = phase;
        }
    }
}
