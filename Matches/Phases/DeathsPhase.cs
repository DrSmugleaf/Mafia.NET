namespace Mafia.NET.Matches.Phases
{
    class DeathsPhase : BasePhase
    {
        public DeathsPhase() : base("Deaths Phase", nextPhase: new ChatPhase())
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
