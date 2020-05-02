namespace Mafia.NET.Matches.Phases
{
    public class DiscussionPhase : BasePhase
    {
        public DiscussionPhase(int duration = 50) : base("Discussion", duration)
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }

        public override IPhase End(IMatch match)
        {
            return match.Settings.DayType.VotingPhase();
        }
    }
}
