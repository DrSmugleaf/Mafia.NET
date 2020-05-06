using Mafia.NET.Matches.Chats;
using System;
using System.Linq;

namespace Mafia.NET.Matches.Phases
{
    public class PresentationPhase : BasePhase
    {
        public PresentationPhase(IMatch match, uint duration = 30) : base(match, "Presentation", duration)
        {
        }

        public override IPhase NextPhase() => new DiscussionPhase(Match);

        public override void Start()
        {
            var names = string.Join(Environment.NewLine, Match.AllPlayers.Values.Select(player => $"{player.Name} has moved into town."));
            var namesNotification = Notification.Chat(names);

            foreach (var player in Match.AllPlayers.Values)
            {
                var role = Notification.Popup($"Your role is {player.Role.Name}"); // TODO Add summary

                player.OnNotification(namesNotification);
                player.OnNotification(role);
            }
        }
    }
}
