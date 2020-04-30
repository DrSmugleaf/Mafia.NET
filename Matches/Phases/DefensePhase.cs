namespace Mafia.NET.Matches.Phases
{
    class DefensePhase : BasePhase
    {
        public DefensePhase(int duration = 15) : base("Defense", duration, new VerdictVotePhase())
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
