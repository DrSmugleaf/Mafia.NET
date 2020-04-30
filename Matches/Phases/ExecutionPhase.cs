namespace Mafia.NET.Matches.Phases
{
    class ExecutionPhase : BasePhase
    {
        public ExecutionPhase() : base("Execution", nextPhase: new ExecutionRevealPhase())
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
