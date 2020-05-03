using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class DefensePhase : BasePhase
    {
        public IPlayer Player { get; }

        public DefensePhase(IMatch match, IPlayer player, int duration = 15) : base(match, "Defense", duration, new VerdictVotePhase(match))
        {
            Player = player;
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
