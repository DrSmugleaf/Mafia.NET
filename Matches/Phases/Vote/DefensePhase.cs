using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    class DefensePhase : BasePhase
    {
        public IPlayer Player { get; }

        public DefensePhase(IPlayer player, int duration = 15) : base("Defense", duration, new VerdictVotePhase())
        {
            Player = player;
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
