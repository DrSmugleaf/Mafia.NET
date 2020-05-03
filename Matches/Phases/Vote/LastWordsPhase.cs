using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class LastWordsPhase : BasePhase
    {
        public LastWordsPhase(IPlayer player, int duration = 10) : base("Last Words", duration, new ExecutionPhase(player))
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
