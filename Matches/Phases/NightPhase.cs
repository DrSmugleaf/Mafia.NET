namespace Mafia.NET.Matches.Phases
{
    class NightPhase : BasePhase
    {
        public NightPhase(int duration = 40) : base("Night", duration, new DeathsPhase())
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
