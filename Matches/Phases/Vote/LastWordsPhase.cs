using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class LastWordsPhase : BasePhase
    {
        public IPlayer Player { get; }

        public LastWordsPhase(IMatch match, IPlayer player, int duration = 10) : base(match, "Last Words", duration, new ExecutionPhase(match, player))
        {
            Player = player;
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
