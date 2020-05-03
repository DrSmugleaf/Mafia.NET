namespace Mafia.NET.Matches.Phases
{
    public class ConclusionPhase : BasePhase
    {
        public ConclusionPhase(IMatch match, int duration) : base(match, "Conclusion", duration)
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override IPhase End()
        {
            Match.End();
            return this;
        }
    }
}
