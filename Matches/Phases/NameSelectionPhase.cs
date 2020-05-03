namespace Mafia.NET.Matches.Phases
{
    public class NameSelectionPhase : BasePhase
    {
        public NameSelectionPhase(IMatch match) : base(match, "Name Selection", 30, new PresentationPhase(match))
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
