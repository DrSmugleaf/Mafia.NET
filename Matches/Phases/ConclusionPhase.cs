namespace Mafia.NET.Matches.Phases
{
    public class ConclusionPhase : BasePhase
    {
        public ConclusionPhase(IMatch match, int duration = 120) : base(match, "Conclusion", duration)
        {
        }

        public override IPhase NextPhase()
        {
            return this;
        }

        public override void End()
        {
            base.End();
            Match.End();
        }
    }
}
