namespace Mafia.NET.Matches.Phases
{
    public class ChatPhase : BasePhase
    {
        public ChatPhase(int duration = 50) : base("Chat Phase", duration, new TrialVotePhase())
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
