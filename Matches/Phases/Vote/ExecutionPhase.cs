using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class ExecutionPhase : BasePhase
    {
        public IPlayer Player { get; }

        public ExecutionPhase(IPlayer player) : base("Execution", nextPhase: new ExecutionRevealPhase())
        {
            Player = player;
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
