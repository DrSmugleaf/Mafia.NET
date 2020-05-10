using System;
using System.Linq;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public class PresentationPhase : BasePhase
    {
        public PresentationPhase(IMatch match, uint duration = 30) : base(match, "Presentation", duration,
            actionable: false)
        {
        }

        public override IPhase NextPhase()
        {
            return new DiscussionPhase(Match);
        }

        public override void Start()
        {
            var names = string.Join(Environment.NewLine,
                Match.AllPlayers.Select(player => $"{player.Name} has moved into town."));
            var namesNotification = Notification.Chat(names);

            foreach (var player in Match.AllPlayers)
            {
                var role = Notification.Popup(
                    $"Your role is {player.Role.Name}{Environment.NewLine}{player.Role.Summary}"); // Todo role abilities and information

                player.OnNotification(namesNotification);
                player.OnNotification(role);
            }
        }
    }
}