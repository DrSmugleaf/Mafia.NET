namespace Mafia.NET.Matches.Phases
{
    public class PresentationPhase : BasePhase
    {
        public PresentationPhase() : base("Presentation", nextPhase: new DiscussionPhase())
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
