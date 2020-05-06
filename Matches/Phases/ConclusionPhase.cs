using Mafia.NET.Matches.Chats;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Phases
{
    public class ConclusionPhase : BasePhase
    {
        public IList<IVictory> Victories;

        public ConclusionPhase(IList<IVictory> victories, IMatch match, uint duration = 120) : base(match, "Conclusion", duration)
        {
            Victories = victories;
        }

        public override IPhase NextPhase()
        {
            return this;
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers.Values);
            foreach (var participant in ChatManager.Chats[0].Participants.Values)
            {
                participant.Muted = false;
                participant.Deaf = false;
            }

            var popup = Notification.Popup("We have come to a conclusion...");
            var roles = Match.AllPlayers.Values
                .Select(player => $"{player.Name} was the {player.Role.Name}")
                .Select(message => Notification.Chat(message))
                .ToList();
            var rolesNotification = new Notification(NotificationType.CHAT, roles);

            foreach (var player in Match.AllPlayers.Values)
            {
                foreach (var victory in Victories)
                {
                    player.OnNotification(popup);
                    player.OnNotification(victory.Popup);
                    player.OnNotification(victory.WinnersList);
                }

                player.OnNotification(rolesNotification);
            }

            base.Start();
        }

        public override void End()
        {
            base.End();
            Match.End();
        }
    }
}
