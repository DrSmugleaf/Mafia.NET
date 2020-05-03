namespace Mafia.NET.Matches.Phases
{
    class NightPhase : BasePhase
    {
        public NightPhase(IMatch match, int duration = 40) : base(match, "Night", duration, new DeathsPhase(match))
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
