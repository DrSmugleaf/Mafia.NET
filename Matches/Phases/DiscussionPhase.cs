using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases.Vote;

namespace Mafia.NET.Matches.Phases
{
    public class DiscussionPhase : BasePhase
    {
        public DiscussionPhase(IMatch match, int duration = 50) : base(match, "Discussion", duration, new AccusePhase(match))
        {
        }

        public override void Start()
        {
            Match.Day++;
            var notification = NotificationEventArgs.Popup($"Day {Match.Day}");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
