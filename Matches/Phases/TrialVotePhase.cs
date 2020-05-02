namespace Mafia.NET.Matches.Phases
{
    class TrialVotePhase : BasePhase
    {
        public TrialVotePhase(int duration = 80) : base("Time Left", duration, new NightPhase())
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
