using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public interface IVoting
    {
        IPhase Procedure { get; }
        IReadOnlyDictionary<int, IPlayer> Voters { get; }
        event EventHandler<ProcedureStartEventArgs> ProcedureStart;

        public IList<IPlayer> VotesAgainst(IPlayer accused);
    }
}
