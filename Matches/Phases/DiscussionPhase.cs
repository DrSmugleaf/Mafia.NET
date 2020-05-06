using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases.Vote;

namespace Mafia.NET.Matches.Phases
{
    public class DiscussionPhase : BasePhase
    {
        public DiscussionPhase(IMatch match, uint duration = 50) : base(match, "Discussion", duration)
        {
        }

        public override IPhase NextPhase() => new AccusePhase(Match);

        public override void Start()
        {
            ChatManager.Open(Match.LivingPlayers.Values);
            var notification = Notification.Popup($"Day {Match.PhaseManager.Day}");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }
    }
}
