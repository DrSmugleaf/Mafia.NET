namespace Mafia.NET.Matches.Phases
{
    public class ExecutionRevealPhase : BasePhase
    {
        public ExecutionRevealPhase() : base("Execution Reveal Phase", nextPhase: new NightPhase())
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
