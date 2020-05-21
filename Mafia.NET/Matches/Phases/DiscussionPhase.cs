using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Notifications;

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
            var notification = Notification.Popup(DayKey.Day, Match.Phase.Day);

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }
    }
}