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
            ChatManager.Open(Match.AllPlayers.Values, true);
            ChatManager.Chats[0].Participants[Player].Muted = false;

            var notification = Notification.Popup($"{Player.Name}, you are on trial for conspiracy against the town. What is your defense?");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
