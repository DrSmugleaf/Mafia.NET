using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ExecutionRevealPhase : BasePhase
    {
        public IPlayer Player;

        public ExecutionRevealPhase(IMatch match, IPlayer player) : base(match, "Execution Reveal", nextPhase: new NightPhase(match))
        {
            Player = player;
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
