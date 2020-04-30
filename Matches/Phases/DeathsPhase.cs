using Mafia.NET.Matches.Chats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Phases
{
    class DeathsPhase : BasePhase
    {
        public DeathsPhase() : base("Deaths", nextPhase: new DiscussionPhase())
        {
        }

        public override void Start(IMatch match)
        {
            List<Notification> notifications = new List<Notification>();
            string startingMessage;
            switch (match.UndisclosedDeaths.Count)
            {
                case 0:
                    return;
                case 1:
                    startingMessage = "One of us did not survive the night.";
                    break;
                default:
                    startingMessage = "Some of us did not survive the night.";
                    break;
            }

            notifications.Add(new Notification(NotificationType.POPUP, startingMessage));

            foreach (var death in match.UndisclosedDeaths)
            {
                string popupName = $"{death.Of.Name} didn't live to see the morning.";
                string popupCause = ""; // TODO
                string popupRole = $"{death.Of.Name}'s role was {death.Of.Role.Name}";
                string chatLastWillAuthor = $"{death.Of.Name} left us his last will:";
                string chatLastWill = death.LastWill;

                notifications.AddRange(new Notification[] {
                    Notification.Popup(popupName),
                    Notification.Popup(popupCause),
                    Notification.Popup(popupRole),
                    Notification.Chat(chatLastWillAuthor),
                    Notification.Chat(chatLastWill)
                });
            }

            foreach (var notification in notifications)
            {
                var notificationEvent = new NotificationEventArgs(notification);
                
                foreach (var player in match.AllPlayers.Values)
                {
                    player.OnNotification(notificationEvent);
                }
            }

            match.Graveyard.AddRange(match.UndisclosedDeaths);
            match.UndisclosedDeaths.Clear();
        }
    }
}
