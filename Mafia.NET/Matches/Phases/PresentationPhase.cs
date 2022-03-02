using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Matches.Phases
{
    public class PresentationPhase : BasePhase
    {
        public PresentationPhase(IMatch match, uint duration = 30) : base(match, "Presentation", duration,
            actionable: false)
        {
        }

        public override IPhase? NextPhase()
        {
            return new DiscussionPhase(Match);
        }

        public override void Start()
        {
            var names = new EntryBundle();
            foreach (var player in Match.AllPlayers)
                names.Chat(DayKey.MoveIntoTown, player.Name);

            foreach (var player in Match.AllPlayers)
            {
                var role = Notification.Popup(DayKey.YourRole, player.Role,
                    player.Role.Summary); // Todo role abilities and information

                player.OnNotification(names);
                player.OnNotification(role);
            }
        }
    }
}