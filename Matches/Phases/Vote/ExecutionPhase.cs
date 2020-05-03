using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class ExecutionPhase : BasePhase
    {
        public IPlayer Player { get; }

        public ExecutionPhase(IMatch match, IPlayer player) : base(match, "Execution", nextPhase: new ExecutionRevealPhase(match, player))
        {
            Player = player;
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
