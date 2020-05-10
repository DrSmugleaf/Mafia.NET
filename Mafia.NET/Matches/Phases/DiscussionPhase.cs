using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases.Vote;

namespace Mafia.NET.Matches.Phases
{
    public class DiscussionPhase : BasePhase
    {
        public DiscussionPhase(IMatch match, uint duration = 50) : base(match, "Discussion", duration)
        {
        }

        public override IPhase NextPhase()
        {
            return new AccusePhase(Match);
        }

        public override void Start()
        {
            ChatManager.Open(Match.LivingPlayers);
            var notification = Notification.Popup($"Day {Match.Phase.Day}");

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }
    }
}