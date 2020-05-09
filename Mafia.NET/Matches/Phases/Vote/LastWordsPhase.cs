using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class LastWordsPhase : BasePhase
    {
        public IPlayer Player { get; }

        public LastWordsPhase(IMatch match, IPlayer player, uint duration = 10) : base(match, "Last Words", duration)
        {
            Player = player;
        }

        public override IPhase NextPhase() => new ExecutionPhase(Match, Player);

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers.Values, true);
            ChatManager.Main().Participants[Player].Muted = false;
            Notification notification = Notification.Popup("Do you have any last words?");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }
    }
}
