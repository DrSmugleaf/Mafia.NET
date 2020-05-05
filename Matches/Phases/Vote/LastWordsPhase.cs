using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class LastWordsPhase : BasePhase
    {
        public IPlayer Player { get; }

        public LastWordsPhase(IMatch match, IPlayer player, int duration = 10) : base(match, "Last Words", duration, new ExecutionPhase(match, player))
        {
            Player = player;
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers.Values, true);
            ChatManager.Chats[0].Participants[Player].Muted = false;
            Notification notification = Notification.Popup("Do you have any last words?");
            
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
