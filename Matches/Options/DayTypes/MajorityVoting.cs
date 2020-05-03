using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public class MajorityVoting : BaseVoting
    {
        public MajorityVoting(Dictionary<int, IPlayer> voters, IPhase procedure) : base(voters, procedure)
        {
        }
    }
}
