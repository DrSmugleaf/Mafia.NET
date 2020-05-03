using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class LastWordsPhase : BasePhase
    {
        public LastWordsPhase(IMatch match, IPlayer player, int duration = 10) : base(match, "Last Words", duration, new ExecutionPhase(match, player))
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
