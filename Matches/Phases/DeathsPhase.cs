using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    class DeathsPhase : BasePhase
    {
        public DeathsPhase() : base("Deaths", nextPhase: new DiscussionPhase())
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
