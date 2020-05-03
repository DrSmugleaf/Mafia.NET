using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class DefensePhase : BasePhase
    {
        public IPlayer Player { get; }

        public DefensePhase(IMatch match, IPlayer player, int duration = 15) : base(match, "Defense", duration, new VerdictVotePhase(match, player))
        {
            Player = player;
        }

        public override void Start()
        {
            var notification = NotificationEventArgs.Popup($"{Player.Name}, you are on trial for conspiracy against the town. What is your defense?");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
