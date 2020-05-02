using Mafia.NET.Matches.Phases;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public class BallotDayType : BaseDayType
    {
        public BallotDayType(IMatch match) : base(match, true)
        {
        }

        public override BasePhase VotingPhase()
        {
            return new BallotPhase();
        }
    }
}
