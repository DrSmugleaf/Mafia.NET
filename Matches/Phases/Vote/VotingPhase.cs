namespace Mafia.NET.Matches.Phases.Vote
{
    class VotingPhase : BasePhase
    {
        public VotingPhase(int duration = 80) : base("Time Left", duration, new NightPhase())
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
