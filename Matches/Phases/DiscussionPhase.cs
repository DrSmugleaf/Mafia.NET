namespace Mafia.NET.Matches.Phases
{
    public class DiscussionPhase : BasePhase
    {
        public DiscussionPhase(int duration = 50) : base("Discussion", duration, new TrialVotePhase())
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }

        public override void End(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
