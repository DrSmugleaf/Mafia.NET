namespace Mafia.NET.Matches.Phases
{
    public class StartingPhase : BasePhase
    {
        public StartingPhase() : base("Starting Phase", nextPhase: new ChatPhase())
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
