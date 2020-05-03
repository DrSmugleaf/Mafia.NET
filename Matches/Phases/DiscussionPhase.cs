using Mafia.NET.Matches.Phases.Vote;

namespace Mafia.NET.Matches.Phases
{
    public class DiscussionPhase : BasePhase
    {
        public DiscussionPhase(int duration = 50) : base("Discussion", duration, new VotingPhase())
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
