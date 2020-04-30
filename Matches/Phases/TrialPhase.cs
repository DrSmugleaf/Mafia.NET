namespace Mafia.NET.Matches.Phases
{
    class TrialPhase : BasePhase
    {
        public TrialPhase() : base("Trial", nextPhase: new DefensePhase())
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
