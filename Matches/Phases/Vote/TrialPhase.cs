using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class TrialPhase : BasePhase
    {
        public IPlayer Player { get; }

        public TrialPhase(IMatch match, IPlayer player) : base(match, "Trial", nextPhase: new DefensePhase(match, player))
        {
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
