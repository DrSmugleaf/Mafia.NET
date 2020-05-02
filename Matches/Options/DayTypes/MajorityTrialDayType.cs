using Mafia.NET.Matches.Phases;
using System.Linq;

namespace Mafia.NET.Matches.Options.DayTypes
{
    public class MajorityTrialDayType : BaseDayType
    {
        public MajorityTrialDayType(IMatch match) : base(match, false)
        {
        }

        public override BasePhase VotingPhase()
        {
            return new TrialVotePhase();
        }

        protected override void OnTrialVoteAdd(TrialVoteAddEventArgs e)
        {
            if (VotesAgainst[e.Accused] > Match.LivingPlayers.Values.Count() / 2)
            {
                Match.SupersedePhase(new TrialPhase(e.Accused));
            }

            base.OnTrialVoteAdd(e);
        }
    }
}
