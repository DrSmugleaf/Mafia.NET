namespace Mafia.NET.Matches.Phases
{
    class ConclusionPhase : BasePhase
    {
        public ConclusionPhase(IMatch match, int duration = 120) : base(match, "Conclusion", duration)
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override IPhase End(IMatch match)
        {
            match.End();
            return this;
        }
    }
}
