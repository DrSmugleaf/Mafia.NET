using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases
{
    class TrialPhase : BasePhase
    {
        public IPlayer Player { get; }

        public TrialPhase(IPlayer player) : base("Trial", nextPhase: new DefensePhase(player))
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
