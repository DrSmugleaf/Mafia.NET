using System.Linq;

namespace Mafia.NET.Matches.Phases
{
    public class BallotPhase : BasePhase
    {
        public BallotPhase(int duration = 80) : base("Time Left", duration, new NightPhase())
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }

        public override IPhase End(IMatch match)
        {
            var votes = match.Settings.DayType.VotesAgainst.OrderByDescending(vote => vote.Value).ToList();

            if (votes.Count() > 1 && votes[0].Value != 0 && votes[0].Value != 1)
            {
                return new TrialPhase(votes[0].Key);
            }

            return new NightPhase();
        }
    }
}
