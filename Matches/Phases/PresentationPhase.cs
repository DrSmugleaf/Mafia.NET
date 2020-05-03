namespace Mafia.NET.Matches.Phases
{
    public class PresentationPhase : BasePhase
    {
        public PresentationPhase(IMatch match) : base(match, "Presentation", nextPhase: new DiscussionPhase(match))
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
