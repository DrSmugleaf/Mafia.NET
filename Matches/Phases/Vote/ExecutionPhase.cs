using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ExecutionPhase : BasePhase
    {
        public IPlayer Player { get; }

        public ExecutionPhase(IMatch match, IPlayer player, int duration = 10) : base(match, "Execution", duration, new ExecutionRevealPhase(match, player))
        {
            Player = player;
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
